using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public static async void StartLogging()
        {
            Xml_Instance = new XmlLoggerRoot
            {
                AssemblyName = Utils.GetAssemblyName(),
                Logs = await LoadPreviousLogs() ?? new List<LogInfo>()
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
                Xml_Instance.Logs.Add(new XML.LogInfo
                {
                    CallerName = Utils.GetInfoCallingMethod(new StackTrace()),
                    Type = GetTypeString(LogType.Info),
                    Log = value
                });

                Save();
            }
        }

        public static void LogValueList<T>(string header, T obj)
        {
            lock (_lock)
            {
                if (obj != null)
                {
                    Type type = typeof(T);
                    List<Param> _params = new();

                    foreach (var property in type.GetProperties())
                    {
                        _params.Add(new Param
                        {
                            ParamName = property.Name,
                            ParamValue = property.GetValue(obj) == null ? "" : (property.GetValue(obj).ToString() ?? "")
                        });
                    }

                    Xml_Instance.Logs.Add(new XML.LogInfo
                    {
                        CallerName = Utils.GetInfoCallingMethod(new StackTrace()),
                        Type = GetTypeString(LogType.ObjectValues),
                        Log = header,
                        ValueList = _params
                    });

                    Save();
                }
                else Logger.Log("Object was null", Logger.Info.Warning);
            }
        }

        internal static void Save()
        {
            CustomSerializer.SerializeTo_XML_File<XmlLoggerRoot>(Xml_Instance, defaultOutput);
        }

        internal static async Task<List<LogInfo>> LoadPreviousLogs()
        {
            XmlLoggerRoot temp = await CustomDeserializer.XML_Deserialize<XmlLoggerRoot>(defaultOutput);

            if (temp.AssemblyName == Utils.GetAssemblyName() && temp != null)
            {
                return temp.Logs;
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
