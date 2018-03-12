using BllEntities;
using Client.ServerManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Client.Misc.FileService
{
    public static class EventFileDownloader
    {
        public static string DownloadFile(this FileDownloader fd, FilePathMap map)
        {
            if (!Directory.Exists(map.FolderForEvent))
            {
                Directory.CreateDirectory(map.FolderForEvent);
            }
            if (!File.Exists(map.PathToFile))
            {
                fd.Download(map.PathToFile, map.FolderWithFile);

            }
            return map.PathToFile;
        }

        public static string CheckFileSizeAndDownloadFile(this FileDownloader fd, FilePathMap map)
        {
            if (!Directory.Exists(map.FolderForEvent))
            {
                Directory.CreateDirectory(map.FolderForEvent);
            }
            if (File.Exists(map.PathToFile))
            {
                FileServiceClient client = new FileServiceClient();
                if (!fd.IsFileSizeCorrect(map.PathToFile, map.FolderWithFile, client))
                {
                    fd.Download(map.PathToFile, map.FolderWithFile);
                }
            }
            else
            {
                fd.Download(map.PathToFile, map.FolderWithFile);
            }
            return map.PathToFile;
        }

        public static void DownloadEventFilesUsingFilepathLib(this FileDownloader fd, BllFilepathLib lib)
        {
            foreach (var name in lib.Entities)
            {
                new Thread(delegate ()
                {
                    DownloadFile(fd, new FilePathMap(name.Path, lib.FolderName));
                }).Start();
            }
        }
    }

}
