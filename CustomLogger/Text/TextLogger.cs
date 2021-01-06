using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Diagnostics;

namespace ResUtils.CustomLogger.Text
{
    public static class TextLogger
    {
        internal static string defaultFileName = "outputLog.txt";
        internal static string defaultOutput => Path.Combine(Utils.GetExeAssemblyPath(), defaultFileName);
        internal static string time => Converter.DateToString(DateTime.Now, true);

        internal static object _lock = new object();

        public static void StartLog(bool clear = false)
        {
            if (clear) ClearLog();

            Log(true ,true, 
                "----- Starting Log -----", 
                "", 
                $"Start Time : {time}", 
                "");
        }

        public static void ClearLog()
        {
            Utils.CheckFile(defaultOutput);
            File.Delete(defaultOutput);
        }

        public static void Log(string value)
        {
            StackTrace t = new();

            Utils.CheckFile(defaultOutput);

            lock (_lock)
            {
                using (var file = new StreamWriter(defaultOutput, true))
                {
                    file.WriteLine(CreateLogLine(t, value: value));
                    file.Close();
                }
            }
        }

        public static void Log(params string[] values)
        {
            StackTrace t = new();

            Utils.CheckFile(defaultOutput);

            lock (_lock)
            {

                using (var file = new StreamWriter(defaultOutput, true))
                {
                    file.WriteLine(CreateLogLine(t, values: values));
                    file.Close();
                }
            }

        }

        public static void Log(bool multiLine, params string[] values)
        {
            StackTrace t = new();

            Utils.CheckFile(defaultOutput);

            lock (_lock)
            {
                using (var file = new StreamWriter(defaultOutput, true))
                {
                    file.WriteLine(CreateLogLine(t, values: values, multiline: multiLine));
                    file.Close();
                }
            }


        }

        public static void Log(bool omitInfo, bool multiLine, params string[] values)
        {
            StackTrace t = new();

            Utils.CheckFile(defaultOutput);

            lock (_lock)
            {
                using (var file = new StreamWriter(defaultOutput, true))
                {
                    file.WriteLine(CreateLogLine(t, values: values, multiline: multiLine, omitInfo: omitInfo));
                    file.Close();
                }
            }


        }


        public static void Log(string value, Info info)
        {
            StackTrace t = new();

            Utils.CheckFile(defaultOutput);

            lock (_lock)
            {

                using (var file = new StreamWriter(defaultOutput, true))
                {
                    file.WriteLine(CreateLogLine(t, value: value, info: info));
                    file.Close();
                }
            }

        }


        public static void LogList<T>(T obj, string header = null)
        {
            if (obj != null)
            {
                Type type = typeof(T);
                List<string> temp = new();

                foreach (PropertyInfo prop in type.GetProperties())
                    temp.Add($"{prop.Name} = {prop.GetValue(obj)}");

                StackTrace t = new();

                Utils.CheckFile(defaultOutput);

                lock (_lock)
                {
                    using (var file = new StreamWriter(defaultOutput, true))
                    {
                        file.WriteLine(CreateLogLine(t, values: temp.ToArray(), listHeader: header, info: Info.ValueList));
                        file.Close();
                    }
                }
            }
        }

        public static void LogException(string header, string exception)
        {
            StackTrace t = new();

            Utils.CheckFile(defaultOutput);

            lock (_lock)
            {
                using (var file = new StreamWriter(defaultOutput, true))
                {
                    file.WriteLine(CreateLogLine(t, values: new string[] { header, exception }, listHeader: header, info: Info.Exception));
                    file.Close();
                }
            }
        }

        internal static string CreateLogLine(StackTrace stack, string value = null, string[] values = null, string listHeader = null, Info info = Info.Info, bool omitInfo = false, bool multiline = false, [CallerMemberName] string memberName = "")
        {
            StringBuilder logLine = new();

            if (!omitInfo)
            {
                logLine.Append(time);
                logLine.Append(" || ");

                logLine.Append($"[ {memberName/*Utils.GetInfoCallingMethod(stack, memberName)*/} ]");

                logLine.Append(" || ");

                switch (info)
                {
                    case Info.Info:
                        logLine.Append("[ INFO ] || ");
                        break;
                    case Info.Warning:
                        logLine.Append("[ WARNING ] || ");
                        break;
                    case Info.Debug:
                        logLine.Append("[ DEBUG ] || ");
                        break;
                    case Info.ValueList:
                        logLine.Append("[ VALUES ] || ");
                        break;
                    case Info.Exception:
                        logLine.Append("[ EXCEPTION ] || ");
                        break;
                    case Info.None:
                        break;
                }

                if (info == Info.ValueList)
                {
                    logLine.Append(listHeader ?? "Values: ");

                    logLine.Append("\n");

                    string temp = logLine.ToString().Substring(0, logLine.ToString().LastIndexOf('|') + 2);

                    if (values != null)
                        foreach (var str in values)
                            if (!string.IsNullOrWhiteSpace(str))
                                logLine.AppendLine($"{new string(' ', temp.Length)}{str}");

                }
                else if (info == Info.Exception)
                {
                    string temp = logLine.ToString();
                    logLine.Append(values[0] + "\n" + temp + values[1]);
                    //logLine.AppendLine(temp + values[1]);
                }
                else
                {
                    if (value != null)
                        logLine.Append(value);

                    if (values != null)
                    {
                        if (multiline)
                        {
                            foreach (var str in values)
                                logLine.AppendLine(str);
                        }
                        else foreach (var str in values)
                                logLine.Append(str);
                    }
                }
            }
            else
            {
                if (value != null)
                    logLine.Append(value);

                if (values != null)
                {
                    if (multiline)
                    {
                        foreach (var str in values)
                            logLine.AppendLine(str);
                    }
                    else foreach (var str in values)
                            logLine.Append(str);
                }
            }

            return logLine.ToString();
        }

        public enum Info
        {
            Info, Warning, Debug, ValueList, Exception, None
        }
    }
}
