using System;
using System.IO;

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
                    File.Create(path);
        }
    }
}
