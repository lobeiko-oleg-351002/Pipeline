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
                string fileInFolder = Path.Combine(folderName, name);
                Download(filePath, fileInFolder);

            }
            return filePath;
        }

        private static void Download(string fullPath, string pathOnServer)
        {
            FileServiceClient client = new FileServiceClient();
            do
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                using (FileStream output = new FileStream(fullPath, FileMode.Create))
                {
                    Stream downloadStream;
                    downloadStream = client.GetFile(pathOnServer);               
                    downloadStream.CopyTo(output);

                }
            }
            while (!IsFileSizeCorrect(fullPath, pathOnServer, client));
        }

        public static string CheckFileSizeAndDownloadFile(string name, string folderName)
        {
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Properties.Resources.DOWNLOADS_FOLDER;
            string eventFolderPath = Path.Combine(downloadsPath, folderName);
            string filePath = Path.Combine(downloadsPath, folderName, name);
            if (!Directory.Exists(eventFolderPath))
            {
                Directory.CreateDirectory(eventFolderPath);
            }
            string fileInFolder = Path.Combine(folderName, name);
            if (File.Exists(filePath))
            {
                FileServiceClient client = new FileServiceClient();
                if (!IsFileSizeCorrect(filePath, fileInFolder, client))
                {
                    Download(filePath, fileInFolder);
                }
            }
            else
            {
                Download(filePath, fileInFolder);
            }
            return filePath;
        }

        private static bool IsFileSizeCorrect(string path, string pathOnServer, FileServiceClient client)
        {
            try
            {
                var fi = new FileInfo(path);
                if (fi.Length == client.GetFileSize(pathOnServer))
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return false;
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
