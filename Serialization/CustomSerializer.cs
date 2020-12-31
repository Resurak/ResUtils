using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ResUtils.Serialization
{
    public class CustomSerializer
    {
        public static void SerializeTo_XML<T>(object objToSerialize, string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(file, objToSerialize, emptyNamespaces);
                }
        }
    }
}
