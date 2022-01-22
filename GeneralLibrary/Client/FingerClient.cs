using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GeneralLibrary.Client
{
    public class FingerClient
    {
        private const int FingerPort = 79;

        private readonly ManualResetEvent _connectDone;
        private readonly ManualResetEvent _sendDone;
        private static ManualResetEvent _receiveDone;

        public List<string[]> ServerResponse { get; private set; }

        public FingerClient()
        {
            // Инициализация параметров
            _connectDone = new ManualResetEvent(false);
            _sendDone = new ManualResetEvent(false);
            _receiveDone = new ManualResetEvent(false);
        }

        public void SendQuery(string remoteIPString, string query)
        {
            try
            {
                // Объединение ip + порт
                var remoteIPAddress = IPAddress.Parse(remoteIPString);
                var remoteEndPoint = new IPEndPoint(remoteIPAddress, FingerPort);

                // Начало соединения
                var client = new Socket(remoteIPAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                client.BeginConnect(remoteEndPoint,
                    new AsyncCallback(ConnectCallback), client);
                _connectDone.WaitOne();

                // Отправка данных
                SendData(client, query);
                _sendDone.WaitOne();
 
                // Прием данных
                Receive(client);
                _receiveDone.WaitOne();

                // Закрытие соединения
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch
            {

            }
        }

        private void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Завершение соединения
                var client = (Socket)asyncResult.AsyncState;
                client.EndConnect(asyncResult);
                _connectDone.Set();
            }
            catch
            {

            }
        }

        private void SendData(Socket client, string query)
        {
            // Отправка данных на сервер
            string pcMachineName = Environment.MachineName + "\n";
            string pcUserName = Environment.UserName + "\n";
            string pcUserDomainName = Environment.UserDomainName + "\n";
            string pcOSVersion = Environment.OSVersion + "\n";

            byte[] dataToSend = Encoding.ASCII.GetBytes(pcMachineName + pcUserName + pcUserDomainName + pcOSVersion + query);
            const int offset = 0;
            client.BeginSend(dataToSend, offset,
                dataToSend.Length, SocketFlags.None,
                new AsyncCallback(FinishSending), client);
        }

        private void FinishSending(IAsyncResult asyncResult)
        {
            try
            {
                // Завершение отправки
                var client = (Socket)asyncResult.AsyncState;
                int bytesSent = client.EndSend(asyncResult);
                _sendDone.Set();
            }
            catch
            {

            }
        }

        private void Receive(Socket client)
        {
            try
            {  
                // Прием данных сервера
                var state = new ClientState
                {
                    WorkSocket = client
                };

                client.BeginReceive(state.Buffer, 0, ClientState.BufferSize, 0,
                    new AsyncCallback(FinishReceiving), state);
            }
            catch
            {

            }
        }

        private void FinishReceiving(IAsyncResult asyncResult)
        {
            try
            {
                // Завершение приема данных сервера
                var state = (ClientState)asyncResult.AsyncState;
                Socket client = state.WorkSocket;

                int bytesRead = client.EndReceive(asyncResult);
                byte[] data = state.Buffer.Take(bytesRead).ToArray();
  
                ServerResponse = ClientNames.Deserialize(data).Info;              
                _receiveDone.Set();
            }
            catch
            {

            }
        }
    }
}
