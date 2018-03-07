using ServerInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class FileService : IFileService
    {
        public Stream GetFile(string virtualPath)
        {
            string storage = Pipeline.Properties.Resources.ResourceManager.GetString("STORAGE_PATH");
            string filePath = Path.Combine(storage, virtualPath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", Path.GetFileName(filePath));

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public long GetFileSize(string path)
        {
            string storage = Pipeline.Properties.Resources.ResourceManager.GetString("STORAGE_PATH");
            string filePath = Path.Combine(storage, path);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", Path.GetFileName(filePath));

            return (new FileInfo(filePath)).Length;
        }

        public bool IsFileExists(string filename)
        {
            string storage = Pipeline.Properties.Resources.ResourceManager.GetString("STORAGE_PATH");
            string filePath = Path.Combine(storage, filename);

            if (File.Exists(filePath))
            {
                return true;
            }
            return false;
        }

        public void PutFile(FileUploadMessage msg)
        {
            string storage = Pipeline.Properties.Resources.ResourceManager.GetString("STORAGE_PATH");
            string fileFolder = Path.Combine(storage, msg.FolderName);
            string filePath = Path.Combine(fileFolder, msg.VirtualPath);

            if (!Directory.Exists(storage))
                Directory.CreateDirectory(storage);

            if (!Directory.Exists(fileFolder))
                Directory.CreateDirectory(fileFolder);

            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                msg.DataStream.CopyTo(outputStream);
            }

        }
    }
}
