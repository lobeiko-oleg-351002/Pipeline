using ServerInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Client.ServerManager
{
    public class FileServiceClient : ClientBase<IFileService>, IFileService, IDisposable
    {
        public FileServiceClient() : base("Server.FileService")
        {
        }

        public Stream GetFile(string virtualPath)
        {
            return base.Channel.GetFile(virtualPath);
        }

        public bool IsFileExists(string filename)
        {
            return Channel.IsFileExists(filename);
        }

        public void PutFile(FileUploadMessage msg)
        {
            base.Channel.PutFile(msg);
        }

        void IDisposable.Dispose()
        {
            if (this.State == CommunicationState.Opened)
                this.Close();
        }

        public long GetFileSize(string path)
        {
            return base.Channel.GetFileSize(path);
        }
    }
}
