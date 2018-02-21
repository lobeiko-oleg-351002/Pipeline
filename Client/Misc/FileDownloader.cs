using BllEntities;
using Client.ServerManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Client.Misc
{
    public static class FileDownloader
    {
        public static string DownloadFile(string name, string folderName)
        {
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Properties.Resources.DOWNLOADS_FOLDER;
            string eventFolderPath = Path.Combine(downloadsPath, folderName);
            string filePath = Path.Combine(downloadsPath, folderName, name);
            if (!Directory.Exists(eventFolderPath))
            {
                Directory.CreateDirectory(eventFolderPath);
            }
            if (!File.Exists(filePath))
            {
                using (FileStream output = new FileStream(filePath, FileMode.Create))
                {
                    Stream downloadStream;
                    using (FileServiceClient client = new FileServiceClient())
                    {
                        downloadStream = client.GetFile(Path.Combine(folderName, name));
                    }
                    downloadStream.CopyTo(output);
                }
            }
            return filePath;
        }

        public static void DownloadEventFilesUsingFilepathLib(BllFilepathLib lib)
        {
            foreach (var name in lib.Entities)
            {
                new Thread(delegate ()
                {
                    DownloadFile(name.Path, lib.FolderName);
                }).Start();
            }
        }
    }
}
