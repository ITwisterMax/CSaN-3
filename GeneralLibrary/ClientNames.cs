using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GeneralLibrary
{
    [Serializable]
    public class ClientNames
    {
        public List<string[]> Info { get; private set; }

        public ClientNames()
        {
            // Инициализация параметров
            Info = new List<string[]>();
        }

        public string[] GetUsernameByName(string name)
        {
            // Поиск нужного имени в списке имен
            foreach (var temp in Info)
                if (temp[0] == name)
                    return temp;
            return null;
        }

        // Сериализация списка имен
        public static byte[] Serialize(ClientNames clientNames)
        {
            byte[] serializedClientNames;

            using (var memoryStream = new MemoryStream())
            {
                var xmlSerializer = new XmlSerializer(typeof(ClientNames));
                xmlSerializer.Serialize(memoryStream, clientNames);

                memoryStream.Position = 0;
                serializedClientNames = new byte[memoryStream.Length];

                const int memoryStreamOffset = 0;
                memoryStream.Read(serializedClientNames, memoryStreamOffset,
                    serializedClientNames.Length);
            }

            return serializedClientNames;
        }

        // Десериализация списка имен
        public static ClientNames Deserialize(byte[] byteArrayClientNames)
        {
            using var memoryStream = new MemoryStream();
            const int memoryStreamOffset = 0;

            memoryStream.Write(byteArrayClientNames, memoryStreamOffset,
                byteArrayClientNames.Length);
            memoryStream.Position = 0;

            var xmlSerializer = new XmlSerializer(typeof(ClientNames));
            return (ClientNames)xmlSerializer.Deserialize(memoryStream);
        }
    }
}
