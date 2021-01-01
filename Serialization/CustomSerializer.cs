using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ResUtils.CustomLogger;

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
                    Logger.Log($"Starting Serialization : {nameof(SerializeTo_XML_File)}");

                    lock (_lock)
                    {
                        using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                        {
                            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

                            var serializer = new XmlSerializer(typeof(T));
                            serializer.Serialize(file, objToSerialize, emptyNamespaces);

                            Logger.Log("Serialization completed");

                            file.Close();
                        }
                    }
                }
                catch (Exception e) { Logger.LogException("Serialization Failed!", e.ToString()); }
            else Logger.Log($"Tried to start {nameof(SerializeTo_XML_File)} - Path was null");
        }

        //public static async Task AsyncSerializeTo_XML_File<T>(object objToSerialize, string path)
        //{
        //    if (!string.IsNullOrWhiteSpace(path))
        //        try
        //        {
        //            Logger.Log($"Starting Serialization : {nameof(SerializeTo_XML_File)}");

        //            lock (_lock)
        //            {
        //                using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
        //                {
        //                    var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

        //                    var serializer = new XmlSerializer(typeof(T));
        //                    serializer.Serialize(file, objToSerialize, emptyNamespaces);

        //                    Logger.Log("Serialization completed");

        //                    file.Close();
        //                }
        //            }
        //        }
        //        catch (Exception e) { Logger.LogException("Serialization Failed!", e.Message.ToString()); }
        //    else Logger.Log($"Tried to start {nameof(SerializeTo_XML_File)} - Path was null");
        //}
    }
}
