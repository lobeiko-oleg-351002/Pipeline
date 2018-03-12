using Client.ServerManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Client.Misc.FileService
{
    public class FileDownloader
    {
        public void Download(string targetPath, string pathOnServer)
        {
            FileServiceClient client = new FileServiceClient();
            do
            {
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                using (FileStream output = new FileStream(targetPath, FileMode.Create))
                {
                    Stream downloadStream;
                    downloadStream = client.GetFile(pathOnServer);
                    downloadStream.CopyTo(output);

                }
            }
            while (!IsFileSizeCorrect(targetPath, pathOnServer, client));
        }

        public bool IsFileSizeCorrect(string path, string pathOnServer, FileServiceClient client)
        {
            try
            {
                var fi = new FileInfo(path);
                if (fi.Length == client.GetFileSize(pathOnServer))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
    }
}
