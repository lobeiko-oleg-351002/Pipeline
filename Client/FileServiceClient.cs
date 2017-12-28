using ServerInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Client
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

        public void PutFile(FileUploadMessage msg)
        {
            base.Channel.PutFile(msg);
        }

        void IDisposable.Dispose()
        {
            if (this.State == CommunicationState.Opened)
                this.Close();
        }
    }
}
