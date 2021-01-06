using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ResUtils.CustomLogger;
using ResUtils.CustomLogger.Text;

namespace ResUtils.Serialization
{
    public class CustomSerializer
    {
        internal static object _lock = new();

        public static void SerializeTo_XML_File<T>(object objToSerialize, string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                try
                {
                    lock (_lock)
                    {
                        using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                        {
                            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

                            var serializer = new XmlSerializer(typeof(T));
                            serializer.Serialize(file, objToSerialize, emptyNamespaces);

                            file.Close();
                        }
                    }
                }
                catch (Exception e) { TextLogger.LogException("XMLSerialization Failed!", e.ToString()); }
            else TextLogger.Log($"Tried to start {nameof(SerializeTo_XML_File)} - Path was null");
        }
    }
}
