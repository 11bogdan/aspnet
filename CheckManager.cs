using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace CheckDocument
{
    public class CheckManager
    {
        string userDirectory;

        public CheckManager(string userName)
        {
            userDirectory = Infrastructure.GetUsersDirectory(userName);
        }


        public List<string> GetFiles(string filt)
        {
            string[] files = Directory.GetFiles(userDirectory, filt)
                .Select(x => Path.GetFileName(x))
                .ToArray();
            return new List<string>(files); 
        }

        public string GetFilePath(string fileName)
        {
            string targetFile = Path.Combine(userDirectory, fileName);
            
            if (!File.Exists(targetFile))
            {
                Infrastructure.Logger.Error(
                    "{0} file not found in user dir {1}", targetFile, userDirectory);
            }

            return targetFile.Replace("\\", "/");
        }
    }
}