using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtils;


namespace BrainfuckIDE.Controls.WIndows.ViewModels
{
    class TextPuterCodeGenerateViewModel : ViewModelBase
    {
        private string _baseText;

        /// <summary> 表示するテキストを取得、設定します。 </summary>
        public string BaseText
        {
            get => _baseText;
            set
            {
                if (_baseText == value) return;

                _baseText = value;
                RaiseNotifyPropertyChanged();
                ResultText = TextCodeMaker.AutoMaker.GetSimplyTextOutputer(_baseText);

            }
        }

        private string _resultText;

        /// <summary> 対応するbrainf*ckのコードを取得します </summary>
        public string ResultText
        {
            get => _resultText;
            private set
            {
                if (_resultText == value) return;

                _resultText = value;
                RaiseNotifyPropertyChanged();
            }
        }

    }
}
