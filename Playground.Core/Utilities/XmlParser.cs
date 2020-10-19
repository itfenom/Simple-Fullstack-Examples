using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Playground.Core.Utilities
{
    public static class XmlParser
    {
        public static T ReadXmlString<T>(string inputXmlString, XmlRootAttribute xmlRootAttribute)
        {
            var serializer = new XmlSerializer(typeof(T), xmlRootAttribute);
            using (var stringReader = new StringReader(inputXmlString))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }

        public static string WriteXmlString<T>(T objectToWrite)
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            var serializer = new XmlSerializer(typeof(T));
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            {
                serializer.Serialize(stringWriter, objectToWrite, namespaces);
                stringWriter.Flush();
            }
            return stringBuilder.ToString();
        }

        public static T ReadXmlFile<T>(string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return (T)serializer.Deserialize(fileStream);
            }
        }

        public static bool WriteXmlFile<T>(string filePath, T objectToWrite)
        {
            bool retVal = false;
            try
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");
                var serializer = new XmlSerializer(typeof(T));
                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    serializer.Serialize(fileStream, objectToWrite, namespaces);
                }

                retVal = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return retVal;
        }

    }
}
