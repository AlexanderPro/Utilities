using System.IO;
using System.Xml.Serialization;

namespace Utilities.Xml
{
    public static class XmlUtils
    {
        public static string Serialize<T>(T obj, XmlSerializerNamespaces namespaces = null, string defaultNamespace = null)
        {
            var serializer = new XmlSerializer(obj.GetType(), defaultNamespace);
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj, namespaces);
                return writer.ToString();
            }
        }

        public static T Deserialize<T>(string data)
        {
            var serializer = new XmlSerializer(typeof(T));
            var reader = new StringReader(data);
            return (T)serializer.Deserialize(reader);
        }

        public static T Deserialize<T>(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(T));
            var reader = new StreamReader(stream);
            return (T)serializer.Deserialize(reader);
        }
    }
}
