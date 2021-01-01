﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace ResUtils.CustomLogger.XML
{
    public class XmlLoggerRoot
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

        public List<Param> ValueList { get; set; }
    }

    public class Param
    {
        public Param()
        {

        }

        [XmlAttribute]
        public string ParamName { get; set; }

        [XmlAttribute]
        public string ParamValue { get; set; }
    }
}
