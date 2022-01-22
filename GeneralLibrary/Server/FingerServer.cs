using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GeneralLibrary.Server
{
    public class FingerServer
    {
        private const int MaxConnections = 10;
        private const int FingerPort = 79;

        private readonly ManualResetEvent _allDone;

        private readonly ClientNames _clientNames;

        public FingerServer()
        {
            // Инициализация параметров
            _allDone = new ManualResetEvent(false);
            _clientNames = new ClientNames();
        }

        public void Start(string ipString)
        {
            try
            {
                // Соединение ip + порт
                var ipAddress = IPAddress.Parse(ipString);
                var endpoint = new IPEndPoint(ipAddress, FingerPort);

                var server = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                server.Bind(endpoint);
                server.Listen(MaxConnections);

                // Инициируем соединение и начинаем прослушивание
                while (true)
                {
                    _allDone.Reset();
                    server.BeginAccept(new AsyncCallback(AcceptConnection), server);
                    _allDone.WaitOne();
                }
            }
            catch
            {

            }
        }

        private void AcceptConnection(IAsyncResult asyncResult)
        {
            // Завершение соединения и начало приема данных
            _allDone.Set();

            var server = (Socket)asyncResult.AsyncState;
            var handler = server.EndAccept(asyncResult);

            var state = new ServerState
            {
                WorkSocket = handler
            };

            // Ожидание приема данных
            const int offset = 0;
            handler.BeginReceive(state.Buffer, offset,
                ServerState.BufferSize, SocketFlags.None,
                new AsyncCallback(FinishReceiving), state);
        }

        private void FinishReceiving(IAsyncResult asyncResult)
        {
            // Завершение приема данных
            var state = (ServerState)asyncResult.AsyncState;
            var handler = state.WorkSocket;

            int bytesRead = handler.EndReceive(asyncResult);

            // Если есть, что читать
            if (bytesRead > 0)
            {
                // Получение информации о пк и запроса
                (string pcMachineName, string pcUserName, string pcUserDomainName, string pcOSVersion, string query) = ParseReceivedData(state.Buffer, bytesRead);
                state.pcMachineName = pcMachineName;
                state.pcUserName = pcMachineName;
                state.pcUserDomainName = pcMachineName;
                state.pcOSVersion = pcMachineName;
                byte[] dataToSend = ParseQuery(query);
                // Отправка результатов
                Send(state, dataToSend);
            }
        }

        private void Send(ServerState state, byte[] data)
        {
            var handler = state.WorkSocket;
            // Начало отправки данных
            handler.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(FinishSending), state);
        }

        private void FinishSending(IAsyncResult asyncResult)
        {
            try
            {
                // Завершение отправки данных
                var state = (ServerState)asyncResult.AsyncState;
                var handler = state.WorkSocket;
                int bytesSent = handler.EndSend(asyncResult);

                // Закрытие соединения
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch
            {

            }
        }
  
        private (string, string, string, string, string) ParseReceivedData(byte[] data, int count)
        {
            // Перегоняем данные в кодировку ASCII и обновляем список имен
            string receivedData = Encoding.ASCII.GetString(data, 0, count);
            string[] dataArray = receivedData.Split("\n");
            bool flag = false;
            foreach (var temp in _clientNames.Info)
                if (temp[0] == dataArray[0])
                {
                    flag = true;
                    break;
                }
            if (!flag)
                _clientNames.Info.Add(dataArray);
            return (dataArray[0], dataArray[1], dataArray[2], dataArray[3], dataArray[4]);
        }

        private byte[] ParseQuery(string query)
        {
            // Обработка запроса и возврат сериализованного имени
            string[] partsOfQuery = query.Split(" ");
            if (partsOfQuery.Length > 1)
            {
                var resultClientNames = new ClientNames();
                resultClientNames.Info.Add(_clientNames.GetUsernameByName(partsOfQuery[1]));
                return ClientNames.Serialize(resultClientNames);
            }
            return ClientNames.Serialize(_clientNames);
        }   
    }
}
