using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SpecificExplorer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public const int INVALIDNUMBER = -1;
        public const string COPYFOLDERNAME = "Sortiert";
        public MainViewModel()
        {
            FoundFiles = INVALIDNUMBER;
            FoundFolder = INVALIDNUMBER;

            SelectSourceFolder = new RelayCommand((o) => SelectSourceFolderExecute(o));
            SelectDestinationFolder = new RelayCommand(o => SelectDestinationFolderExecute(o));
            CopyFiles = new RelayCommand((o) => CopyFilesExecute(o));
        }


        private string m_SourceFolder;
        public string SourceFolder
        {
            get => m_SourceFolder;
            set
            {
                if (FoundFiles != INVALIDNUMBER)
                    FoundFiles = INVALIDNUMBER;
                if (FoundFolder != INVALIDNUMBER)
                    FoundFolder = INVALIDNUMBER;

                SetProperty(ref m_SourceFolder, value);
                NotifyOfPropertyChange(nameof(CanCopy));
            }
        }

        private ICommand m_SelectSourceFolder;
        public ICommand SelectSourceFolder
        {
            get => m_SelectSourceFolder;
            set
            {
                SetProperty(ref m_SelectSourceFolder, value);
            }
        }
        private void SelectSourceFolderExecute(object _param)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                EnsurePathExists = true
            };
            CommonFileDialogResult res = dialog.ShowDialog();
            if (res != CommonFileDialogResult.Ok)
            {
                Status1 = "Ordnerauswahl abgebrochen";
                return;
            }

            SourceFolder = dialog.FileName;
        }

        private int m_FoundFolder;
        public int FoundFolder
        {
            get => m_FoundFolder;
            set
            {
                SetProperty(ref m_FoundFolder, value);
            }
        }

        private int m_FoundFiles;
        public int FoundFiles
        {
            get => m_FoundFiles;
            set
            {
                SetProperty(ref m_FoundFiles, value);
            }
        }

        private string m_DestinationFolder;
        public string DestinationFolder
        {
            get => m_DestinationFolder;
            set
            {
                SetProperty(ref m_DestinationFolder, value);
                NotifyOfPropertyChange(nameof(CanCopy));
            }
        }

        private ICommand m_SelectDestinationFolder;
        public ICommand SelectDestinationFolder
        {
            get => m_SelectDestinationFolder;
            set
            {
                SetProperty(ref m_SelectDestinationFolder, value);
            }
        }
        private void SelectDestinationFolderExecute(object _param)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                EnsurePathExists = true
            };
            CommonFileDialogResult res = dialog.ShowDialog();
            if (res != CommonFileDialogResult.Ok)
            {
                Status1 = "Ordnerauswahl abgebrochen";
                return;
            }

            DestinationFolder = dialog.FileName;
        }

        public bool CanCopy
        {
            get => System.IO.Directory.Exists(SourceFolder) &&
                System.IO.Directory.Exists(DestinationFolder) &&
                !IsCopying;
        }

        private bool m_IsCopying;
        public bool IsCopying
        {
            get => m_IsCopying;
            set
            {
                SetProperty(ref m_IsCopying, value);
            }
        }

        private ICommand m_CopyFiles;
        public ICommand CopyFiles
        {
            get => m_CopyFiles;
            set
            {
                SetProperty(ref m_CopyFiles, value);
            }
        }
        private void CopyFilesExecute(object _param)
        {
            MessageBoxResult res = MessageBox.Show(
                "Sollen wirklich alle Dateien kopiert werden?",
                "Alles kopieren?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No
                );

            if (res != MessageBoxResult.Yes)
            {
                Status1 = "Kopieren abgebrochen";
                return;
            }

            Thread t = new Thread((o) => CopyAll());
            t.Start();
        }

        #region Status
        private string m_Status1;
        public string Status1
        {
            get => m_Status1;
            set
            {
                SetProperty(ref m_Status1, value);
            }
        }

        private string m_Status2;
        public string Status2
        {
            get => m_Status2;
            set
            {
                SetProperty(ref m_Status2, value);
            }
        }

        private string m_Status3;
        public string Status3
        {
            get => m_Status3;
            set
            {
                SetProperty(ref m_Status3, value);
            }
        }
        #endregion

        private void CopyAll()
        {
            // stop all possible actions by user
            IsCopying = true;
            NotifyOfPropertyChange(nameof(CanCopy));

            #region content of copying
            //System.Threading.Thread.Sleep(5000);
            DirectoryInfo MainFolder = Directory.CreateDirectory(Path.Combine(SourceFolder, COPYFOLDERNAME));
            string[] allFiles = Directory.GetFiles(SourceFolder, "*", SearchOption.AllDirectories);

            foreach (string file in allFiles)
            {
                FileInfo info = new FileInfo(file);
                DirectoryInfo directoryInfoSource = new DirectoryInfo(file);
                string extension = directoryInfoSource.Extension == string.Empty ? directoryInfoSource.Extension : directoryInfoSource.Extension.Remove(0, 1);
                string folderToCopy = extension == string.Empty ? Path.Combine(MainFolder.FullName, "-") : Path.Combine(MainFolder.FullName, extension);
                DirectoryInfo directoryInfoDestination = null;
                if (!Directory.Exists(folderToCopy))
                    directoryInfoDestination = Directory.CreateDirectory(folderToCopy);
                else
                    directoryInfoDestination = new DirectoryInfo(file);

                // check if file already exists
                int countFiles = Directory.GetFiles(folderToCopy, "*" + info.Name, SearchOption.TopDirectoryOnly).Length;
                string addition = countFiles == 0 ? "" : countFiles.ToString() + " ";

                File.Copy(file, Path.Combine(folderToCopy, addition + info.Name));
            }
            #endregion

            // make possible actions by user possible again
            IsCopying = false;
            NotifyOfPropertyChange(nameof(CanCopy));
        }
    }
}