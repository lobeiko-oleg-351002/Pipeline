using BllEntities;
using Client.EventClasses.Events;
using Client.Misc;
using Client.Misc.FileService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.FileControls
{
    public class FileControlsManager
    {
        private readonly FileControls fileControls;

        public FileControlsManager(FileControls fileControls)
        {
            this.fileControls = fileControls;
            fileControls.FileList.MouseDoubleClick += listBox_DoubleClick;
            fileControls.ShowLocation.CheckedChanged += checkBox_CheckedChanged;
        }

        public void PopulateFileListBoxUsingFilepathLib(BllFilepathLib lib)
        {
            fileControls.FileList.Items.Clear();
            foreach (var filename in lib.Entities)
            {
                fileControls.FileList.Items.Add(filename.Path);
            }
        }

        private void listBox_DoubleClick(object sender, MouseEventArgs e)
        {
            if (fileControls.FileList.SelectedIndex >= 0)
            {
                string filename = fileControls.ControllerSet.SelectedEvent.EventData.FilepathLib.Entities[fileControls.FileList.SelectedIndex].Path;
                string foldername = fileControls.ControllerSet.SelectedEvent.EventData.FilepathLib.FolderName;
                try
                {
                    FileDownloader fileDownloader = new FileDownloader();
                    string path = fileDownloader.CheckFileSizeAndDownloadFile(new FilePathMap(filename, foldername));
                    if (fileControls.ShowLocation.Checked == false)
                    {
                        Process.Start(path);
                    }
                    else
                    {
                        OpenFileFolder(path);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Properties.Resources.CANNOT_OPEN_FILE, filename);
                }
            }
        }

        private void OpenFileFolder(string filepath)
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_USE_COMMANDER))
            {
                OpenFileFolderInCommander(filepath);
            }
            else
            {
                Process.Start("explorer.exe", "/select, \"" + filepath + "\"");
            }
        }

        private void OpenFileFolderInCommander(string path)
        {
            try
            {
                Process.Start(AppConfigManager.GetKeyValue(Properties.Resources.TAG_COMMANDER_PATH), "/O \"" + path + "\"");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ClearFileList()
        {
            fileControls.FileList.Items.Clear();
        }

        public void SetOpenFileLocationCheckBoxAccordingToConfigValue()
        {
            fileControls.ShowLocation.Checked = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_OPEN_FILE_LOCATION);
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_OPEN_FILE_LOCATION, fileControls.ShowLocation.Checked.ToString());
        }
    }
}
