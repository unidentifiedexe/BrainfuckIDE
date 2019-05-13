using Interpreter;
using WpfUtils;

namespace BrainfuckIDE.Controls.ViewModels
{
    class MemoryTokenViewModel : ViewModelBase
    {
        private readonly EditableMemoryToken _baseTorken;
        public MemoryTokenViewModel(EditableMemoryToken baseTorken)
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

        public bool IsCurrent => _baseTorken.IsCurrent;
    }
}
