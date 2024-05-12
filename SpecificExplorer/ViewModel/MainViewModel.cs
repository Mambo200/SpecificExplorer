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
using System.Windows.Shell;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

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

            StatusProgressState = TaskbarItemProgressState.Normal;
            StatusProgressValue = 0;
        }
        Thread copyThread = null;
        public void CloseWindow()
        {
            if (copyThread != null
                && copyThread.ThreadState == ThreadState.Running)
            {
                copyThread.Abort();
            }
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

                if (!Directory.Exists(value))
                    return;
            }
        }

        [Obsolete("Not finished Meshod", true)]
        private string[] GetAllFolders(string _directorypath)
        {
            string[] topDirectoryFolders = Directory.GetDirectories(_directorypath, "*", SearchOption.TopDirectoryOnly);
            List<string> result = new List<string>();
            for (int i = 0; i < topDirectoryFolders.Length; i++)
            {
                DirectoryInfo info = new DirectoryInfo(topDirectoryFolders[i]);
                if (!info.Attributes.HasFlag(System.IO.FileAttributes.System))
                    result.Add(topDirectoryFolders[i]);
            }
            return result.ToArray();
        }
        [Obsolete("Not finished Meshod", true)]
        private string[] GetAllFiles(string _folderPath)
        {
            return null;
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


            copyThread = new Thread((o) => CopyAll());
            copyThread.Start();
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

        #region Statusbar

        private double m_StatusProgressValue;
        public double StatusProgressValue
        {
            get => m_StatusProgressValue;
            set
            {
                SetProperty(ref m_StatusProgressValue, value);
            }
        }


        private TaskbarItemProgressState m_StatusProgressState;
        public TaskbarItemProgressState StatusProgressState
        {
            get => m_StatusProgressState;
            set
            {
                SetProperty(ref m_StatusProgressState, value);
            }
        }
        #endregion

        private void CopyAll()
        {
            // stop all possible actions by user
            IsCopying = true;
            NotifyOfPropertyChange(nameof(CanCopy));
            StatusProgressValue = 0;
            StatusProgressState = TaskbarItemProgressState.Normal;

            #region content of copying
            //System.Threading.Thread.Sleep(5000);
            DirectoryInfo MainFolder = Directory.CreateDirectory(Path.Combine(SourceFolder, COPYFOLDERNAME));
            string[] allFiles = null;
            try
            {
                allFiles = Directory.GetFiles(SourceFolder, "*", SearchOption.AllDirectories);
            }
            catch (Exception _ex)
            {
                StatusProgressState = TaskbarItemProgressState.Error;
                MessageBox.Show($"Ein Fehler ist aufgetreten beim Suchen der Dateien:" + Environment.NewLine + Environment.NewLine + _ex.Message,
                    "Kopieren abgebrochen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);

                // make possible actions by user possible again
                IsCopying = false;
                NotifyOfPropertyChange(nameof(CanCopy));

                // Update UI
                StatusProgressState = TaskbarItemProgressState.None;
                StatusProgressValue = 0;

                return;
            }
            int allFilesLength = allFiles.Length;
            int count = 0;
            foreach (string file in allFiles)
            {
                count++;
                FileInfo info = new FileInfo(file);
                Status1 = info.Name;
                Status3 = $"{allFilesLength} / {count}";
                DirectoryInfo directoryInfoSource = new DirectoryInfo(file);
                string extension = directoryInfoSource.Extension == string.Empty ? directoryInfoSource.Extension : directoryInfoSource.Extension.Remove(0, 1);
                string folderToCopy = extension == string.Empty ? Path.Combine(MainFolder.FullName, "-") : Path.Combine(MainFolder.FullName, extension);
                DirectoryInfo directoryInfoDestination = null;
                if (!Directory.Exists(folderToCopy))
                {
                    try
                    {
                        directoryInfoDestination = Directory.CreateDirectory(folderToCopy);
                    }
                    catch (Exception _ex)
                    {
                        StatusProgressState = TaskbarItemProgressState.Error;
                        MessageBox.Show($"Ein Fehler ist aufgetreten beim Erstellen des Ordners \"{folderToCopy}\":" + Environment.NewLine + Environment.NewLine + _ex.Message, "Kopieren abgebrochen",
                            MessageBoxButton.OK,
                            MessageBoxImage.Stop);

                        break;
                    }
                }
                else
                {
                    directoryInfoDestination = new DirectoryInfo(file);
                }

                // check if file already exists
                int countFiles = Directory.GetFiles(folderToCopy, "*" + info.Name, SearchOption.TopDirectoryOnly).Length;
                string addition = countFiles == 0 ? "" : countFiles.ToString() + " ";

                try
                {
                    File.Copy(file, Path.Combine(folderToCopy, addition + info.Name));
                }
                catch (Exception _ex)
                {
                    StatusProgressState = TaskbarItemProgressState.Error;
                    MessageBox.Show($"Ein Fehler ist aufgetreten beim Kopieren:" + Environment.NewLine + Environment.NewLine + _ex.Message,
                        "Kopieren abgebrochen",
                        MessageBoxButton.OK,
                        MessageBoxImage.Stop);
                    ;
                    break;

                }

                StatusProgressValue = ((double)count / (double)allFilesLength);
                System.Threading.Thread.Sleep(1);
            }
            #endregion

            // make possible actions by user possible again
            IsCopying = false;
            NotifyOfPropertyChange(nameof(CanCopy));

            // Update UI
            StatusProgressState = TaskbarItemProgressState.None;
            StatusProgressValue = 0;
        }
    }
}