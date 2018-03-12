using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Client.Misc.FileService
{
    public class FilePathMap
    {
        public readonly string FolderForEvent;
        public readonly string PathToFile;
        public readonly string FolderWithFile;

        public FilePathMap(string fileName, string folderName)
        {
            string FolderWithFiles = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Properties.Resources.DOWNLOADS_FOLDER;
            FolderForEvent = Path.Combine(FolderWithFiles, folderName);
            PathToFile = Path.Combine(FolderWithFiles, folderName, fileName);
            FolderWithFile = Path.Combine(folderName, fileName);
        }
    }
}
