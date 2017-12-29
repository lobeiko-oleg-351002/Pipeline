using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServerInterface
{
    [MessageContract]
    public class FileUploadMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string VirtualPath { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string FolderName { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream DataStream { get; set; }
    }
}
