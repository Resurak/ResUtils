using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ResUtils.Serialization;

namespace ResUtils.CustomLogger.XML
{
    public class XmlLogger
    {
        internal static string defaultFileName = "outputLog.xml";
        internal static string defaultOutput => Path.Combine(Utils.GetExeAssemblyPath(), defaultFileName);
        internal static string time => Converter.DateToString(DateTime.Now, true);

        internal static object _lock = new object();

        internal static XmlLoggerRoot Xml_Instance = new();

        public static async void StartLogging(bool overwrite)
        {
            if (overwrite) File.Delete(defaultOutput);

            Xml_Instance = new XmlLoggerRoot
            {
                AssemblyName = Utils.GetAssemblyName(),
                Logs = overwrite ? new List<LogInfo>() : await LoadPreviousLogs() ?? new List<LogInfo>()
            };

            Xml_Instance.Logs.Add(new XML.LogInfo
            {
                Type = GetTypeString(LogType.StartLog),
                Log = "----- Start Log ------"
            });

            Save();
        }

        public static void Log(string value)
        {
            lock (_lock)
            {
                AddLog(value, trace: new StackTrace());

                Save();
            }
        }

        public static void LogValueList<T>(string header, T obj, string instanceName = null)
        {
            lock (_lock)
            {
                if (obj != null)
                {
                    Type type = typeof(T);
                    PropertyList _params = new();

                    List<PropertiesModel> list = Utils.GetListOfAllValues(obj);

                    _params.InstanceName = instanceName ?? "";

                    if (list != null && list.Count > 0)
                    {
                        Logger.Log("list was not null");
                        foreach (var item in list)
                        {
                            _params.li.Add(new Property
                            {
                                Indent = item.Indent,
                                PropertyType = item.PropertyType,
                                PropertyName = item.PropertyName,
                                PropertyValue = item.PropertyValue
                            });
                        }
                    }
                    else
                    {
                        Logger.Log("list was null", Logger.Info.Warning);
                        _params = null;
                    }

                    AddLog(header, _params, new StackTrace(), LogType.ObjectValues);
                    Save();
                }
                else Logger.Log("Object was null", Logger.Info.Warning);
            }
        }

        public static void LogException(string exception, string header = null)
        {
            lock (_lock)
            {

            }
        }

        internal static void AddLog(string log, PropertyList paramValues = null, StackTrace trace = null, LogType logType = LogType.Info)
        {
            Xml_Instance.Logs.Add(new LogInfo
            {
                CallerName = (trace is not null) ? Utils.GetInfoCallingMethod(trace) : "",
                Type = GetTypeString(logType),
                PropertyListValues = paramValues, //?? new ValueList(),
                Log = log ?? ""
            });
        }

        public static void debug()
        {
            XmlLoggerRoot test = new XmlLoggerRoot
            {
                AssemblyName = "Testing",
                Logs = new List<LogInfo>()
            };
            test.Logs.Add(new LogInfo { CallerName = "testing", Date = "shish", Log = "shupt", Type = "faccinio", PropertyListValues = new PropertyList { InstanceName = "vafanculu", li = new List<Property>() } });
            test.Logs[0].PropertyListValues.li.Add(new Property { PropertyName = "osignur", PropertyValue = "vafanculu" });

            LogValueList("test", test, nameof(test));
            //LogValueList("test", Xml_Instance, nameof(Xml_Instance));
        }

        internal static void Save()
        {
            CustomSerializer.SerializeTo_XML_File<XmlLoggerRoot>(Xml_Instance, defaultOutput);
        }

        internal static async Task<List<LogInfo>> LoadPreviousLogs()
        {
            XmlLoggerRoot temp = await CustomDeserializer.XML_Deserialize<XmlLoggerRoot>(defaultOutput);

            if (temp != null)
            {
                return (temp.AssemblyName == Utils.GetAssemblyName()) ? temp.Logs : null;
            }
            else
            {
                Logger.Log("temp.assemblyname was not utils.getassemblyname", Logger.Info.Warning);
                return null;
            }
        }

        public static string GetTypeString(LogType logType)
        {
            switch (logType)
            {
                case LogType.Info:
                    return "Info";
                case LogType.StartLog:
                    return "StartLog";
                case LogType.EndLog:
                    return "EndLog";
                case LogType.Debug:
                    return "Debug";
                case LogType.Warning:
                    return "Warning";
                case LogType.Exception:
                    return "Exception";
                case LogType.ObjectValues:
                    return "ObjectValues";
            }
            return "Generic";
        }

        public enum LogType
        {
            Info, StartLog, EndLog, Debug, Warning, Exception, ObjectValues
        }
    }
}
