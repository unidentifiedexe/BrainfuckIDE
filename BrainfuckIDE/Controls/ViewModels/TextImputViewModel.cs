#nullable enable

using BrainfuckIDE.IdeParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfUtils;

namespace BrainfuckIDE.Controls.ViewModels
{
    class TextImputOnlyInitDataViewModel : ViewModelBase
    {
        private string _text = string.Empty;
        private bool _isReadOnly;

        public string ShowText
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RaiseNotifyPropertyChanged();
                }

            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if(_isReadOnly != value)
                {
                    _isReadOnly = value;
                    RaiseNotifyPropertyChanged();
                }

            }
        }


        public IImputTextSender GetTextSender()
        {
            var text = ShowText;
            text = Regex.Replace(text, "\r\n+", "\n");
            return new ImputTextSender(text, 0xff);
        }

        class ImputTextSender : IImputTextSender, IDisposable
        {
            private readonly IEnumerator<char> _enumerator;

            private readonly byte _defaultChar;

            public ImputTextSender(string text, char defaltChar)
            {
                _enumerator = text.GetEnumerator();
                _defaultChar = (byte)defaltChar;
            }


            public ImputTextSender(string text, byte defaltChar)
            {
                _enumerator = text.GetEnumerator();
                _defaultChar = defaltChar;
            }

            public bool TryGetNextChar(out byte letter)
            {
                if (_enumerator.MoveNext())
                    letter = (byte)_enumerator.Current;
                else
                    letter =  _defaultChar;
                return true;
            }


            public void Dispose()
            {
                _enumerator.Dispose();
            }
        }
    }



}
