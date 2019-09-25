using BrainfuckIDE.Filer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using WpfUtils;

namespace BrainfuckIDE.Editor.Controls
{
    class FileSaverViewModel : ViewModelBase, INameTabViewModel
    {
        private FileSaverAndLoader _fileSaver = new FileSaverAndLoader();

        private bool _isSaved = true;

        public void NoticeToTextChanged()
        {
            if (_isSaved)
            {
                _isSaved = false;
                RaiseNotifyPropertyChanged(nameof(IsSaved));
            }
        }

        #region INameTabViewModel

        public bool IsSaved => _fileSaver.IsSaved() && _isSaved;

        public string FileName => Path.GetFileName(_fileSaver.FilePath);

        public string FilePath => _fileSaver.HasSaveFilePath ? Path.GetFullPath(_fileSaver.FilePath) : _fileSaver.FilePath;

        #endregion

        public bool CanOverWriteSave => _fileSaver.HasSaveFilePath;



        public bool SaveText(SourceText text)
        {
            if (!CanOverWriteSave) return false;
            _fileSaver.Save(text);
            _isSaved = true;

            UpdateNameTabState();
            return true;
        }

        public void SaveTextAs(string path,SourceText text)
        {
            _fileSaver.SaveAs(path, text);
            _isSaved = true;

            UpdateNameTabState();
        }

        public void SaveTextToTemp(SourceText text)
        {
            _fileSaver.SaveToTemporary(text);
        }

        public SourceText LoadText(string path)
        {
            var ret = _fileSaver.OpenFrom(path);
            _isSaved = true;

            UpdateNameTabState();
            return ret;
        }

        private void UpdateNameTabState()
        {
            RaiseNotifyPropertyChanged(nameof(IsSaved));
            RaiseNotifyPropertyChanged(nameof(FileName));
            RaiseNotifyPropertyChanged(nameof(FilePath));
        }

        public void ForceDeleteTemporaryFile()
        {
            _fileSaver.ForceDeleteTemporaryFile();
        }

    }


}
