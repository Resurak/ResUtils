using ResUtils.CustomLogger.XML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
        public static string GetInfoCallingMethod(StackTrace stack)
        {
            StringBuilder stackLine = new();
            Type type = stack.GetFrame(1).GetMethod().DeclaringType;

            stackLine.Append(stack.GetFrame(1).GetMethod().DeclaringType.Namespace);
            stackLine.Append(".");
            stackLine.Append(stack.GetFrame(1).GetMethod().DeclaringType.Name);
            stackLine.Append(".");
            stackLine.Append(stack.GetFrame(1).GetMethod().Name);

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
                if (t.GetGenericTypeDefinition() == typeof(List<>))
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
            if (str != null)
                return str.ToString() ?? "null";
            else return "";
        }

        internal static List<List<(string name, string value)>> recursiveHell = new();
        internal static List<(string name, string value)> ExternalList = new();


        internal static Type[] AllowedTypes = new Type[] { typeof(string), typeof(int), typeof(double), typeof(long), typeof(DateTime), typeof(object), typeof(char), typeof(byte), typeof(decimal), typeof(float) };
        internal static int recursiveDeep = 0;

        public static List<(string name, string value)> RecursivePropsNameAndValue<T>(T obj)
        {
            List<(string name, string value)> temp = new();

            recursiveHell.Add(GetPropertyNameAndValue(obj));

            foreach (var item in recursiveHell)
            {
                if (item != null)
                    foreach (var item2 in item)
                        temp.Add(new(item2.name ?? "", item2.value ?? ""));
            }

            return temp;
        }

        public static List<(string name, string value)> GetPropertyNameAndValue<T>(T obj)
        {
            try
            {
                CustomLogger.Logger.Log("Starting GetPropertyNameAndValue", $"Type of Obj = {typeof(T)}");

                string plus = "+ ";

                for (int i = 0; i < recursiveDeep; i++)
                    plus += plus;

                List<(string name, string value)> MainList = new();
                List<(string name, string value)> tempList = new();

                Type MainType = obj.GetType();

                if (TypeIsList(MainType))
                {
                    CustomLogger.Logger.Log($"the passed type is a list. here you have to figure out how to handle it. obj.gettype() : {obj.GetType()}");

                    List<(string name, string value)> ls = new();
                    ls.Add(new(plus + obj.GetType().Name, ""));

                    recursiveHell.Add(ls);

                    int index = 0;
                    foreach (var item in obj as IList)
                    {
                        recursiveDeep++;
                        recursiveHell.Add(GetPropertyNameAndValue(item));
                        recursiveDeep--;
                    }

                    CustomLogger.Logger.Log($"tried to make a new list. list.count = {index}");
                }
                else
                {
                    PropertyInfo[] temp = MainType.GetProperties();

                    foreach (var property in temp)
                    {
                        Type t = property.PropertyType;

                        if (TypeIsList(t))
                        {
                            object o = property.GetValue(obj);
                            MainList = GetPropertyNameAndValue(o);
                        }
                        else if (AllowedTypes.Contains(t))
                        {
                            MainList.Add(new(plus + property.Name, GetStringIfValueNotNull(property.GetValue(obj))));
                        }
                        else if (property.GetValue(obj) != null)
                        {
                            List<(string name, string value)> ls = new();
                            ls.Add(new(plus + obj.GetType().Name, "here should go the other values"));

                            recursiveHell.Add(ls);

                            recursiveHell.Add(GetPropertyNameAndValue(property.GetValue(obj)));
                        }
                    }
                }

                foreach (var item in tempList)
                    MainList.Add(item);

                CustomLogger.Logger.Log($"method ended. total list count = {MainList.Count}");

                return MainList;
            }
            catch (Exception e) { CustomLogger.Logger.LogException($"GetPropertyNameAndValue failed to execute", e.ToString()); return null; }
        }
    }
}
