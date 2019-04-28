#nullable enable

using BrainfuckIDE.DialogUtils.Filer;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.DialogUtils
{
    class FileSaveDialog<T>
    {
        private SaveFileDialog _innner;
        private FilterCollection<T>? _fileFilters = null;

        public FileSaveDialog()
        {
            _innner = new SaveFileDialog();
        }
        public string FileName { get => _innner.FileName; set => _innner.FileName = value; }
        public string InitialDirectory { get => _innner.InitialDirectory; set => _innner.InitialDirectory = value; }


        //public object FilterIndex {get => _innner; set => _innner = value;}

        public string Title { get => _innner.Title; set => _innner.Title = value; }
        public bool RestoreDirectory { get => _innner.RestoreDirectory; set => _innner.RestoreDirectory = value; }
        public bool CheckFileExists { get => _innner.CheckFileExists; set => _innner.CheckFileExists = value; }
        public bool CheckPathExists { get => _innner.CheckPathExists; set => _innner.CheckPathExists = value; }

        public FilterCollection<T> FileFilters
        {
            get => _fileFilters ??= new FilterCollection<T>();
            set => _fileFilters = value;
        }

        public bool? ShowDialog()
        {
            var filter = FileFilters;
            filter.IsLocked = true;
            _innner.Filter = string.Join("|",filter.Select(p => $"{p.DescriptionText}|{p.Type}"));

            return _innner.ShowDialog();
        }
    }
}
