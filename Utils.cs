using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    }
}
