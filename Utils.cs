using ResUtils.CustomLogger;
using ResUtils.CustomLogger.Text;
using ResUtils.CustomLogger.XML;
using ResUtils.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResUtils
{
    public class Utils
    {
        /// <summary>
        /// Check if a path has a file or a folder. Return true if a file or a directory exist, false if the path doesn't exist
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns></returns>
        public static bool CheckPathExist(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                bool file = File.Exists(path);
                bool dir = Directory.Exists(path);

                if (file || dir) return true;
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Check if a file exist. If not, create it
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void CheckFile(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                if (!File.Exists(path)) 
                    File.Create(path).Close();
        }

        /// <summary>
        /// Check if a directory exist. If not, create it
        /// </summary>
        /// <param name="path"></param>
        public static void CheckDir(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Get the root directory path of the executing assembly. Works even if it's called from another assembly
        /// </summary>
        /// <returns></returns>
        public static string GetExeAssemblyPath()
        {
            return new FileInfo(new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).AbsolutePath).Directory.FullName;
        }

        public static string GetAssemblyName()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }

        /// <summary>
        /// Get a string like this: Namespace.Class.Method 
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public static string GetInfoCallingMethod(StackTrace stack, string methodName = null)
        {
            StringBuilder stackLine = new();
            Type type = stack.GetFrame(1).GetMethod().DeclaringType;

            stackLine.Append(stack.GetFrame(1).GetMethod().DeclaringType.Namespace);
            stackLine.Append(".");
            stackLine.Append(stack.GetFrame(1).GetMethod().DeclaringType.Name);
            stackLine.Append(".");
            stackLine.Append(methodName ?? stack.GetFrame(1).GetMethod().Name);

            return stackLine.ToString();
        }

        public static string GetCallingMethod(StackTrace stack, string method)
        {
            StringBuilder stackLine = new();
            Type type = stack.GetFrame(1).GetMethod().DeclaringType;

            int frame = 0;

            if (type != typeof(BackgroundWorker) && type != typeof(Task))
                frame = 1;

            string classname = stack.GetFrame(frame).GetMethod().DeclaringType.FullName;

            if (classname.Contains('+'))
                classname = classname.Substring(0, classname.LastIndexOf('+'));

            //stackLine.Append(stack.GetFrame(frame).GetMethod().DeclaringType.Namespace);
            //stackLine.Append(".");
            stackLine.Append(classname);
            stackLine.Append(".");
            stackLine.Append(method ?? "");

            return stackLine.ToString();
        }

        /// <summary>
        /// Returns a list that contains properties name and value
        /// </summary>
        /// <typeparam name="T">the generic type</typeparam>
        /// <param name="obj">the instance of the object</param>
        /// <returns></returns>
        public static List<string> GetListOfProperties<T>(T obj)
        {
            if (obj != null)
            {
                Type type = typeof(T);
                List<string> temp = new();

                foreach (PropertyInfo prop in type.GetProperties())
                    temp.Add($"{prop.Name} = {prop.GetValue(obj)}");

                return temp;
            }
            else return new List<string> { "Object to serialize was null" };
        }

        public static bool TypeIsList(Type t)
        {
            if (t.IsGenericType)
                if (t.GetGenericTypeDefinition() == typeof(List<>) || t.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
                    return true;
            return false;
        }

        public static Type GetTypeFromList(Type t)
        {
            if (t != null)
                return t.GetGenericArguments().First();
            return null;
        }

        public static string GetStringIfValueNotNull(object str)
        {
            try
            {
                if (str != null)
                    return str.ToString() ?? "null";
                else return "";
            } catch (Exception e) { TextLogger.LogException("tried to get string", e.ToString()); return ""; }
        }


        public static Type[] CommonValueTypes = new Type[] { typeof(string), typeof(int), typeof(double), typeof(long), typeof(DateTime), typeof(object), typeof(char), typeof(byte), typeof(decimal), typeof(float) };
        internal static int recursiveDeep = 0;

        internal static List<Properties> ExternalList = new();

        public static List<Properties> GetListOfAllValues<T>(T obj)
        {
            ExternalList.Clear();

            RecursiveInternal(obj);

            recursiveDeep = 0;
            return ExternalList;
        }

        public static void RecursiveInternal<T>(T obj, bool decrement = false)
        {
            if (obj != null)
            {
                try
                {
                    Type MainType = obj.GetType();

                    if (CommonValueTypes.Contains(MainType))
                    {
                        foreach (var property in MainType.GetProperties())
                        {
                            ExternalList.Add(GetParamFromProperty(property, obj));
                        }
                    }
                    else if (TypeIsList(MainType))
                    {
                        //ExternalList.Add(GetParamFromProperty(null, obj, null, true));

                        ExternalList.Add(GetParamFromProperty(null, obj, nameof(List<T>)));

                        if (MainType.GetGenericTypeDefinition() == typeof(ObservableCollection<>)) TextLogger.Log("trying to add observableCollection");

                        recursiveDeep++;

                        foreach (var item in obj as IEnumerable)
                        {
                            TextLogger.Log("trying to add observableCollection", item.ToString());
                            RecursiveInternal(item);
                        }
                    }
                    else
                    {
                        foreach (var property in MainType.GetProperties().Where(x => x.GetIndexParameters().Length == 0))
                        {
                            try
                            {
                                Type SubType = property.PropertyType;

                                object propInstance = null;

                                try
                                {
                                    propInstance = property.GetValue(obj);
                                } catch { }

                                if (CommonValueTypes.Contains(SubType))
                                {
                                    ExternalList.Add(GetParamFromProperty(property, obj));
                                }
                                else
                                {
                                    if (TypeIsList(SubType))
                                        ExternalList.Add(GetParamFromProperty(property, obj, $"List of type {GetTypeFromList(SubType).Name}"));
                                    //else ExternalList.Add(GetParamFromProperty(property, obj));

                                    ExternalList.Add(GetParamFromProperty(null, obj, null, true));

                                    if (propInstance != null)
                                        RecursiveInternal(propInstance, true);
                                }
                            } catch (Exception e) { TextLogger.LogException("trying observable collection, probably erro in propinstance.", e.ToString()); }
                        }
                    }
                }
                catch (Exception e)
                {
                    TextLogger.LogException("Error in RecursivePropertiesHandler", e.ToString());
                }
            }
        }

        internal static List<Properties> Properties = new();

        public static List<Properties> GetListOfPropertiesAndValues<T>(T obj, Type[] allowedTypes = null)
        {
            Stopwatch sw = new();
            sw.Start();
            Properties.Clear();

            RecursiveCall(obj, 0, allowedTypes);

            sw.Stop();
            TextLogger.Log("total time spent on logging in ms: " + sw.Elapsed.TotalMilliseconds);
            return Properties;
        }

        internal static void RecursiveCall<T>(T obj, int index = 0, Type[] allowedTypes = null)
        {
            if (obj != null)
            {
                Type main = obj.GetType();

                try
                {
                    foreach (var prop in main.GetProperties())
                    {
                        Type sub = prop.PropertyType;

                        object propValue = null;

                        try
                        {
                            propValue = prop.GetValue(obj);
                        }
                        catch (Exception e) { TextLogger.LogException($"unable to get value of {prop.Name}", e.ToString()); }

                        if (CommonValueTypes.Contains(prop.PropertyType))
                        {
                            TextLogger.Log($"type was {sub.Name}, added {prop.Name}");
                            Properties.Add(GetPropertyInfos(sub.Name, prop.Name, GetStringIfValueNotNull(propValue), index));
                        }
                        else if (TypeIsList(sub))
                        {
                            Properties.Add(GetPropertyInfos(indent: 25));
                            Properties.Add(GetPropertyInfos($"List of type {GetTypeFromList(sub)}", prop.Name, indent: index));
                            Properties.Add(GetPropertyInfos(indent: 25));

                            TextLogger.Log("type was list, starting recursion");
                            foreach (var item in propValue as IEnumerable)
                            {
                                RecursiveCall(item, index + 1, allowedTypes);
                                Properties.Add(GetPropertyInfos(indent: 25));
                            }
                        }
                        else
                        {
                            Properties.Add(GetPropertyInfos(sub.Name, prop.Name, indent: index));
                            TextLogger.Log($"type was {sub.Name}, added {prop.Name}");

                            if (allowedTypes != null)
                            {
                                if (allowedTypes.Contains(sub))
                                {
                                    TextLogger.Log($"{sub.Name} was allowed, starting recursion");
                                    RecursiveCall(propValue, index + 1, allowedTypes);
                                }
                                else TextLogger.Log($"{sub.Name} was not allowed, skipping");
                            }
                            else TextLogger.Log($"{sub.Name} was not allowed, skipping");
                        }

                    }
                } catch (Exception e) { TextLogger.LogException("something went wrong.", e.ToString()); }
            }
        }

        internal static Properties GetPropertyInfos(string type = null, string name = null, string value = null, int indent = 0)
        {
            string indentResult = "- ";
            for (int i = 0; i < indent; i++)
                indentResult += "- ";

            return new Properties { Indent = indentResult, PropertyName = name, PropertyType = type, PropertyValue = value };
        }

        public static Properties GetParamFromProperty<T>(PropertyInfo property, T obj, string type = null, bool separator = false)
        {
            string indent = "- ";

            for(int i = 0; i < recursiveDeep; i++)
                indent += "- ";

            if (separator) return new Properties { Indent = "- - - - - - - - - - - - - - - - - - - - - - - - -" };

            if (type == null)
                return new Properties { Indent = indent, PropertyType = property.PropertyType.Name, PropertyName = property.Name ?? "NO-NAME-ERROR", PropertyValue = GetStringIfValueNotNull(property.GetValue(obj)) };
            else return new Properties { Indent = indent, PropertyType = type, PropertyName = (property != null) ? property.Name : null };
        }
    }
}
