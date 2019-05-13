#nullable enable

using Interpreter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtils;

namespace BrainfuckIDE.Controls.ViewModels
{
    class MemoryViewModel : ViewModelBase
    {
        IEditableMemory? _editableMemory;
        private ObservableCollection<MemoryTokenViewModel> _memoryTokens
            = new ObservableCollection<MemoryTokenViewModel>();

        public ObservableCollection<MemoryTokenViewModel> MemoryTokens
        {
            get => _memoryTokens;
            set
            {
                if (_memoryTokens != value)
                {
                    _memoryTokens = value;
                    RaiseNotifyPropertyChanged();
                }
            }
        }
        public void SetMemory(IEditableMemory? editableMemory)
        {
            _editableMemory = editableMemory;
            if (_editableMemory != null)
            {
                var addTokens = editableMemory.Select(p => new MemoryTokenViewModel(p));
                MemoryTokens = new ObservableCollection<MemoryTokenViewModel>(addTokens);
            }
            else
            {
                MemoryTokens.Clear();
            }
        }


        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    RaiseNotifyPropertyChanged();
                }
            }
        }

        internal void ReflectToInterpretor()
        {
            _editableMemory?.ReflectToParent();
        }
    }
}
