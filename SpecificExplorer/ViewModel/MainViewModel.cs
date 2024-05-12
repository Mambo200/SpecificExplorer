using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpecificExplorer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public const int INVALIDNUMBER = -1;
        public MainViewModel()
        {
            FoundFiles = INVALIDNUMBER;
            FoundFolder = INVALIDNUMBER;
        }


        private string m_SourceFolder;
        public string SourceFolder
        {
            get => m_SourceFolder;
            set
            {
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public bool CanCopy
        {
            get => System.IO.Directory.Exists(SourceFolder) && System.IO.Directory.Exists(DestinationFolder);
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
            throw new System.NotImplementedException();
        }
    }
}