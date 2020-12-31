using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ResUtils.Serialization
{
    public class CustomDeserializer
    {
        public static T XML_Deserialize<T>(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (file.Length != 0)
                    {
                        try
                        {
                            var deserializer = new XmlSerializer(typeof(T));
                            object value = deserializer.Deserialize(file);

                            return (T)Convert.ChangeType(value, typeof(T));

                        }
                        catch { return default; }
                    }
                    else return default;
                }
            else return default;
        }
    }
}
