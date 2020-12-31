using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ResUtils.Versions
{
    public class VersionTracker
    {
        public VersionTracker()
        {

        }

        public VersionTracker(Assembly assembly)
        {
            Assembly = assembly;
        }

        public VersionTracker(Assembly assembly, int major, int minor, int build, int revision)
        {
            Assembly = assembly;

            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        public VersionTracker(Assembly assembly, int major, int minor, int build, int revision, string comment)
        {
            Assembly = assembly;

            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;

            Comment = comment;
        }



        public Assembly Assembly;

        public string RootFolderName;
        public string RootFolderPath;

        public string AssemblyFolderName;

        public int Major, Minor, Build, Revision;
        public string Comment = "";

        internal string documents => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        internal string root_folderName => "ResUtils - Versions";
        internal string rootPath => 
            Path.Combine(
                string.IsNullOrWhiteSpace(RootFolderPath) ? documents : RootFolderPath, 
                string.IsNullOrWhiteSpace(RootFolderName) ? root_folderName : RootFolderName
                );

        internal void Save()
        {
            if (Assembly != null)
            {
                Directory.CreateDirectory(rootPath);

                AssemblyFolderName = Assembly.FullName;
            }
        }

        internal void Load()
        {

        }
    }
}
