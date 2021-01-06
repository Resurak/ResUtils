using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace ResUtils.Models
{
    public class XmlLoggerTree
    {
        [XmlAttribute]
        public string AssemblyName { get; set; }

        public List<LogInfo> Logs { get; set; }
    }

    public class LogInfo
    {
        public LogInfo()
        {
            Date = Converter.DateToString(DateTime.Now, true);
        }

        [XmlAttribute]
        public string Date { get; set; }
        
        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string CallerName { get; set; }

        public string Log { get; set; }

        [XmlElement(IsNullable = false)]
        public PropertyList PropertyListValues { get; set; }
    }

    public class PropertyList
    {
        public PropertyList()
        {
            li = new();
        }

        [XmlAttribute]
        public string InstanceName { get; set; }

        public List<Property> li { get; set; }
    }

    public class Property
    {
        public Property()
        {

        }

        [XmlAttribute]
        public string Indent { get; set; }

        [XmlAttribute]
        public string PropertyType { get; set; }

        [XmlAttribute]
        public string PropertyName { get; set; }

        [XmlAttribute]
        public string PropertyValue { get; set; }
    }
}
