using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using ResUtils.Serialization;
using ResUtils.CustomLogger;
using System.Threading.Tasks;
using System.Linq;

namespace ResUtils.Versions
{
    public class VersionTracker
    {
        public event EventHandler Failed;
        protected virtual void OnFailed(EventArgs e)
        {
            LoadFailed = true;
            Failed?.Invoke(this, e);
        }

        public VersionTracker()
        {

        }

        public VersionTracker(Assembly assembly)
        {
            Assembly = assembly;

            Load();
        }

        public VersionTracker(Assembly assembly, int major, int minor, int build)
        {
            Assembly = assembly;

            Major = major;
            Minor = minor;
            Build = build;

            Load();
        }

        public VersionTracker(Assembly assembly, int major, int minor, int build, string comment)
        {
            Assembly = assembly;

            Major = major;
            Minor = minor;
            Build = build;

            Comment = comment;

            Load();
        }



        public Assembly Assembly;

        public Model.VersionTrackerXML Versions = new();

        public string RootFolderName { get; set; }
        public string RootFolderPath { get; set; }
        public string SaveFileName { get; set; }

        public string AssemblyFolderName { get; set; }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public string Comment { get; set; }



        internal string documents => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        internal string root_folderName => "ResUtils - Versions";
        internal string rootPath => 
            Path.Combine(
                string.IsNullOrWhiteSpace(RootFolderPath) ? documents : RootFolderPath, 
                string.IsNullOrWhiteSpace(RootFolderName) ? root_folderName : RootFolderName
                );
        internal string saveFileName => 
            Path.Combine(
                rootPath,
                string.IsNullOrWhiteSpace(SaveFileName) ? "General-Ver.xml" : SaveFileName
            );
        internal bool Loaded = false;
        internal bool LoadFailed = false;
        internal string FailMessage = "";


        public async void Save()
        {
            Logger.Log("Version Tracker - Start save");

            await Task.Run(() =>
            {
                while (Loaded == false) { }

                if (Assembly != null && !LoadFailed)
                {
                    Model.VersionTrackerXML versions = new Model.VersionTrackerXML
                    {
                        AssemblyName = Assembly.GetName().Name,
                        Versions = Versions.Versions ?? new List<Model.Version>()
                    };

                    int lastRev = 0;

                    if (versions.Versions.Count > 0)
                    {
                        Logger.Log($"Total number of versions = {versions.Versions.Count}");

                        if (versions.Versions.Last().Minor != Minor || versions.Versions.Last().Major != Major || versions.Versions.Last().Build != Build)
                            lastRev = 0;
                        else lastRev = versions.Versions.Last().Revision + 1;

                        foreach (var comment in versions.Versions.Where(x => x.Comment == Comment)) 
                            Comment = ""; 
                    }

                    versions.Versions.Add(new Model.Version
                    {
                        BuildDate = Converter.Date_To_String(DateTime.Now),
                        Comment = this.Comment,
                        Major = this.Major,
                        Minor = this.Minor,
                        Build = this.Build,
                        Revision = lastRev,
                        ver = $"{Major}.{Minor}.{Build} rev {lastRev}"
                    });

                    Logger.Log($"Current version = {versions.Versions.Last().ver}");

                    CustomSerializer.SerializeTo_XML_File<Model.VersionTrackerXML>(versions, saveFileName);
                }
            });

            Logger.Log("Version saved");
        }

        internal async void Load()
        {
            Logger.Log("Loading version file");

            AssemblyFolderName = Assembly.GetName().Name;
            SaveFileName = $"{Assembly.GetName().Name}.xml";

            Utils.CheckDir(rootPath);

            Versions = await CustomDeserializer.XML_Deserialize<Model.VersionTrackerXML>(saveFileName) ?? new Model.VersionTrackerXML();

            if (string.IsNullOrWhiteSpace(Versions.AssemblyName))
                Logger.Log("Version file was null. Created new", Logger.Info.Warning);
            else Logger.Log("Version file loaded");


            Loaded = true;
        }
    }
}
