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
        MemoriTokenCollection _memoryTokens = new MemoriTokenCollection(null);
        public MemoriTokenCollection MemoryTokens
        {
            get => _memoryTokens;
            private set
            {
                _memoryTokens = value;
                RaiseNotifyPropertyChanged();
            }
        }
        public void SetMemory(IEditableMemory? editableMemory)
        {
            MemoryTokens = new MemoriTokenCollection(editableMemory);
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

        internal void ReflectToInterpretor() => _memoryTokens.ReflectToParent();
    }

}
