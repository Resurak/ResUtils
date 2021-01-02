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
                    ValueList _params = new();

                    List<(string name, string value)> list = Utils.GetPropertyNameAndValue(obj);

                    _params.InstanceName = instanceName ?? "";

                    if (list != null && list.Count > 0)
                    {
                        Logger.Log("list was not null");
                        foreach (var item in list)
                        {
                            _params.li.Add(new Param
                            {
                                ParamName = item.name,
                                ParamValue = item.value
                            });
                        }
                    }
                    else
                    {
                        Logger.Log("list was null", Logger.Info.Warning);
                        _params = null;
                    }

                    //foreach (var property in type.GetProperties())
                    //{
                    //    Type t = Utils.TypeIsList(property.PropertyType) ? Utils.GetTypeFromList(property.PropertyType) : property.PropertyType;

                    //    Utils.GetPropertyNameAndValue(property);

                    //    _params.li.Add(new Param
                    //    {
                    //        ParamName = property.Name,
                    //        ParamValue = t.Name
                    //    });
                    //}

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

        internal static void AddLog(string log, ValueList paramValues = null, StackTrace trace = null, LogType logType = LogType.Info)
        {
            Xml_Instance.Logs.Add(new LogInfo
            {
                CallerName = (trace is not null) ? Utils.GetInfoCallingMethod(trace) : "",
                Type = GetTypeString(logType),
                ParamListValues = paramValues, //?? new ValueList(),
                Log = log ?? ""
            });
        }

        public static void debug()
        {
            LogValueList("test", Xml_Instance, nameof(Xml_Instance));
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
                if (temp.AssemblyName == Utils.GetAssemblyName())
                {
                    return temp.Logs;
                }
                return null;
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
