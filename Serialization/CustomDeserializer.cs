using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ResUtils.CustomLogger;

namespace ResUtils.Serialization
{
    public class CustomDeserializer
    {
        internal static object _lock = new object();
        
        public static async Task<T> XML_Deserialize<T>(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                Logger.Log($"Starting Deserialization : {nameof(XML_Deserialize)}");

                lock (_lock)
                {
                    using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                    {
                        if (file.Length != 0)
                        {
                            try
                            {
                                var deserializer = new XmlSerializer(typeof(T));
                                object value = deserializer.Deserialize(file);

                                Logger.Log("Object deserialized, returning value");

                                file.Close();

                                return (T)Convert.ChangeType(value, typeof(T));
                            }
                            catch (Exception e)
                            {
                                Logger.LogException("Deserialization failed!", e.Message.ToString());
                                return default;
                            }
                        }
                        else
                        {
                            Logger.Log($"Tried to start {nameof(XML_Deserialize)} - File was empty");
                            return default;
                        }
                    }
                }
            }
            else
            {
                Logger.Log($"Tried to start {nameof(XML_Deserialize)} - Path was null");
                return default;
            }
        }
    }
}
