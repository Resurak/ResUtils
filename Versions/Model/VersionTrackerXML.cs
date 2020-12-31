using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ResUtils.Versions.Model
{
    public class VersionTrackerXML
    {
        [XmlAttribute]
        public string AssemblyName { get; set; }

        public List<Version> Versions { get; set; }
    }

    public class Version
    {
        [XmlAttribute]
        public string BuildDate { get; set; }

        [XmlAttribute]
        public int Major { get; set; }

        [XmlAttribute]
        public int Minor { get; set; }

        [XmlAttribute]
        public int Build { get; set; }

        [XmlAttribute]
        public int Revision { get; set; }


        public string ver { get; set; }
        public string Comment { get; set; }
    }
}
