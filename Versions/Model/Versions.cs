using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ResUtils.Versions.Model
{
    internal class Versions
    {
        [XmlAttribute]
        public string AssemblyName { get; set; }

        public Version Version { get; set; }
    }

    internal class Version
    {
        [XmlAttribute]
        public DateTime BuildDate { get; set; }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revision { get; set; }

        public string Comment { get; set; }
    }
}
