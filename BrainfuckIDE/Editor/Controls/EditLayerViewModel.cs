#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainfuckInterpreter;
using ICSharpCode.AvalonEdit.Document;
using WpfUtils;

namespace BrainfuckIDE.Editor.Controls
{
    /// <summary>
    /// EditLayerをMVVM風に使うためにVMもどき。実際はViewとVMの切り離しは行われていない
    /// </summary>
    class EditLayerViewModel : ViewModelBase
    {

        private BrainfuckTextEditControl _baseControl = null!;

        public FileSaverViewModel FileSaverViewModel { get; } = new FileSaverViewModel();


        private bool _isChanged = true;
        private SourceText _previousText;

        #region FileOperatiions

        private void Save()
        {
            if (FileSaverViewModel.CanOverWriteSave)
                FileSaverViewModel.SaveText(GetSourceCode());
            else
                SaveAs();
        }

        private void SaveAs()
        {
            var fileSaveDialog = new DialogUtils.FileSaveDialog<object?>()
            {
                FileFilters =
                {
                    { "Brainf*ck source code." , "*.bf" , default },
                    { "All type." , "*.*", default }
                },
            };

            var res = fileSaveDialog.ShowDialog();
            if (res.HasValue && res.Value)
                FileSaverViewModel.SaveTextAs(fileSaveDialog.FileName, GetSourceCode());

        }

        #region FileCommand

        private Command? _saveCommand;
        public Command SaveCommand => _saveCommand ??= new Command(Save);


        private Command? _saveAsCommand;
        public Command SaveAsCommand => _saveAsCommand ??= new Command(SaveAs);
        #endregion

        #endregion


        public void SetControl(BrainfuckTextEditControl editControl)
        {
            _baseControl = editControl;
            _baseControl.Changed += BaseControl_Changed;
        }

        private void BaseControl_Changed(object sender, EventArgs e)
        {
            _isChanged = true;
            FileSaverViewModel.NoticeToTextChanged();
        }

        private string _filePath = string.Empty;

        public SourceText GetSourceCode()
        {
            if (_isChanged)
            {
                _previousText = new SourceText(Guid.NewGuid(), _baseControl.Text);
                _isChanged = false;
                FileSaverViewModel.SaveTextToTemp(_previousText);
            }
            return _previousText;
        }

        public IEnumerable<Place> GetBreakPoints()
        {
            return
                _baseControl.GetBreakPoints().Select(ConvertPlace);
        }

        public Place RunnningPosition
        {
            get => ConvertPlace(_baseControl.RunnningPosition);
            set
            {
                var newPos = ConvertPlace(value);
                if (newPos != _baseControl.RunnningPosition)
                {
                    _baseControl.RunnningPosition = newPos;
                    RaiseNotifyPropertyChanged();
                }
            }
        }

        private Place ConvertPlace(TextLocation place)
        {
            return new Place(place.Line - 1, place.Column - 1);
        }


        private TextLocation ConvertPlace(Place place)
        {
            if (place.IsEmpty()) return TextLocation.Empty;
            return new TextLocation(place.Line + 1, place.Colomun + 1);
        }

        public bool IsReadOnly
        {
            get => _baseControl.IsReadOnly;
            set => _baseControl.IsReadOnly = value;
        }


    }
    readonly struct SourceText
    {
        public Guid Guid { get; }

        public string Text { get; }


        public SourceText(Guid guid, string text)
        {
            Guid = guid;
            Text = text;
        }

        static public SourceText Empty => new SourceText(Guid.Empty, string.Empty);
        public bool IsEmpty => (Guid == Guid.Empty);

        public static bool operator ==(SourceText a, SourceText b) => a.Guid == b.Guid;
        public static bool operator !=(SourceText a, SourceText b) => a.Guid != b.Guid;

        public override bool Equals(object obj)
        {
            if (!(obj is SourceText)) return false;
            return Guid.Equals(((SourceText)obj).Guid);
        }
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        internal void Deconstruct(out Guid guid, out string text)
        {
            guid = Guid;
            text = Text;
        }
    }

}
