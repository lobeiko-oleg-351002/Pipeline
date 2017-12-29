using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface
{
    [ServiceContract]
    public interface IFileService
    {
        [OperationContract]
        void PutFile(FileUploadMessage msg);

        [OperationContract]
        Stream GetFile(string virtualPath);

        [OperationContract]
        bool IsFileExists(string filename);

    }
}
