using System.IO;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Playground.Core.Utilities
{
    public static class XmlFileParser
    {
        /// <summary>
        /// Serializes the object to XML and writes it to the given file location.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="content">The object to be serialized.</param>
        /// <param name="FileName">The full path of the file to write to.</param>
        public static void WriteXml<T>(T content, string FileName)
        {
            using (TextWriter textWriter = new StreamWriter(FileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(textWriter, content);
            }
        }

        /// <summary>
        /// Reads the given file, deserializes its contents from XML, and returns the contents.
        /// </summary>
        /// <typeparam name="T">The type of the object contained in the file.</typeparam>
        /// <param name="fileName">The full path of the file to read from.</param>
        /// <returns>The object deserialized from the file.</returns>
        public static T ReadXml<T>(string fileName)
        {
            using (TextReader textReader = new StreamReader(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(textReader);
            }
        }

        /// <summary>
        /// Writes the XML schema to the given XSD file location.
        /// </summary>
        /// <param name="schema">The XML schema to write.</param>
        /// <param name="fileName">The full path of the XSD file to write to.</param>
        public static void WriteXsd(XmlSchema schema, string fileName)
        {
            using (TextWriter textWriter = new StreamWriter(fileName))
            {
                schema.Write(textWriter);
            }
        }

        /// <summary>
        /// Reads the given XSD file and returns the XML schema within.
        /// </summary>
        /// <param name="fileName">The full path of the XSD file to read from.</param>
        /// <param name="validationEventHandler">A delegate to handle validation events.</param>
        /// <returns>The XML schema contained in the XSD file.</returns>
        public static XmlSchema ReadXsd(string fileName, ValidationEventHandler validationEventHandler = null)
        {
            using (TextReader textReader = new StreamReader(fileName))
            {
                return XmlSchema.Read(textReader, validationEventHandler);
            }
        }
    }
}
