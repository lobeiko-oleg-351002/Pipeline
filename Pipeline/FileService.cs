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

        public void PutFile(FileUploadMessage msg)
        {
            string storage = Pipeline.Properties.Resources.ResourceManager.GetString("STORAGE_PATH");
            string filePath = Path.Combine(storage, msg.VirtualPath);
            string dir = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                msg.DataStream.CopyTo(outputStream);
            }

        }
    }
}
