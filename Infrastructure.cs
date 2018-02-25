using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using CheckDocument.Library;
using NLog;
using NLog.Config;
using System.Reflection;

namespace CheckDocument
{
    public static class Infrastructure
    {
        public static string RootDir { get; private set; }
        public static Logger Logger { get; private set; }

        static Infrastructure()
        {
            RootDir = ConfigurationManager.AppSettings["root_dir"];

            string assemblyFolder = AppDomain.CurrentDomain.BaseDirectory;
            LogManager.Configuration = new XmlLoggingConfiguration(assemblyFolder + "/LogConfig.config");
            Logger = LogManager.GetLogger("logfile");

            if (!Directory.Exists(RootDir))
            {
                Logger.Error("No root directory error: "+RootDir+".");

                throw new DirectoryNotFoundException(RootDir + " does not exist");
            }
        }
        public static string GetUsersDirectory(string userName)
        {
            string ud = Path.Combine(RootDir, userName);
            if (!Directory.Exists(ud))
            {
                Directory.CreateDirectory(ud);
            }
            return ud;
        }

        public static List<string> GetAllowedExts()
        {
            string exts = ConfigurationManager.AppSettings["allowed_exts"];
            Logger.Info("Exts red: {0}", exts);
            return new List<string>(exts.Split('|'));
        }

        public static List<string> GetRuleTypes()
        {
            var type = typeof(IRule);
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            List<string> types = asms
                .First(a => a.FullName.Contains("CheckDocument.Library"))
                .GetTypes()
                .Where(p => type.IsAssignableFrom(p) && p != type)
                .Select(t => t.Name)
                .ToList();

            return types;
        }
    }
}