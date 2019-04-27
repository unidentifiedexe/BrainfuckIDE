using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtils;

namespace BrainfuckIDE.Controls.ViewModels
{
    class ResultTextViewModel : ViewModelBase
    {
        private string _resultText;

        public string ResultText
        {
            get => _resultText;
            set
            {
                if(_resultText != value)
                {
                _resultText = value;
                    RaiseNotifyPropertyChanged();
                }
            }
        }


        public void WriteChar(byte letter)
        {
            WriteChar((char)letter);
        }

        private void WriteChar(char letter)
        {
            ResultText += letter;
        }
    }
}
