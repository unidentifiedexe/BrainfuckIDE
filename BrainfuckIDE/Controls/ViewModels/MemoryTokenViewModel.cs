#nullable enable
using Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WpfUtils;

namespace BrainfuckIDE.Controls.ViewModels
{
    abstract class MemoryTokenViewModel : ViewModelBase
    {
        protected readonly EditableMemoryToken _baseTorken;

        protected bool _isCurrent;
        protected MemoryTokenViewModel(EditableMemoryToken baseTorken)
        {
            _baseTorken = baseTorken;
        }

        public byte Value
        {
            get => _baseTorken.Value;
            set
            {
                if (_baseTorken.Value != value)
                {
                    _baseTorken.Value = value;
                    RaiseNotifyPropertyChanged();
                }
            }
        }

        public bool IsCurrent => _isCurrent;



        public abstract Command SetSelectCommand { get; }

    }


    class MemoriTokenCollection : ViewModelBase , IReadOnlyList<MemoryTokenViewModel>
    {
        readonly IEditableMemory? _editableMemory;

        private readonly List<EditableMemoryTokenViewModel> _list;

        public MemoriTokenCollection(IEditableMemory? editableMemory)
        {
            _editableMemory = editableMemory;

            if (_editableMemory != null)
            {
                var vms = _editableMemory.Select(p => new EditableMemoryTokenViewModel(this, p));
                _list = new List<EditableMemoryTokenViewModel>(vms);
                if (SelectedIndex >= 0)
                    _list[SelectedIndex].IsCurrent = true;
            }
            else
                _list = new List<EditableMemoryTokenViewModel>();
        }

        public MemoryTokenViewModel this[int index] => _list[index];

        public int SelectedIndex
        {
            get => _editableMemory?.CurrentIndex ?? -1;
            set
            {
                if (_editableMemory == null) return;
                if (SelectedIndex == value) return;
                if ((uint)value >= _editableMemory.Count) throw new IndexOutOfRangeException();

                var old = _editableMemory.CurrentIndex;
                _editableMemory.CurrentIndex = value;
                _list[old].IsCurrent = false;
                _list[value].IsCurrent = true;
                RaiseNotifyPropertyChanged();
            }
        }

        public int Count => _list.Count;

        public IEnumerator<MemoryTokenViewModel> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();

        private class EditableMemoryTokenViewModel : MemoryTokenViewModel
        {
            private readonly MemoriTokenCollection _parent;

            public EditableMemoryTokenViewModel(MemoriTokenCollection parent, EditableMemoryToken baseTorken)
                :base(baseTorken)
            {
                _parent = parent;
            }

            public new bool IsCurrent 
            { 
                get => base.IsCurrent;
                set
                {
                    if (IsCurrent == value) return;
                    _isCurrent = value;
                    SetSelectCommand.RaiseCanExecuteChanged();
                }
            }

            private Command? _setSelectCommand;

            public override Command SetSelectCommand =>
                _setSelectCommand ??= new Command(() => _parent.SelectedIndex = _baseTorken.Index, () => !IsCurrent);
        }


        public void ReflectToParent() => _editableMemory?.ReflectToParent();
    }
}
