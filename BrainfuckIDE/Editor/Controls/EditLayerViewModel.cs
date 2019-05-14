#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using ICSharpCode.AvalonEdit.Document;
using Interpreter;
using WpfUtils;

namespace BrainfuckIDE.Editor.Controls
{
    /// <summary>
    /// EditLayerをMVVM風に使うためにVMもどき。実際はViewとVMの切り離しは行われていない
    /// </summary>
    class EditLayerViewModel : ViewModelBase, IDropTarget
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

        public void Load(string filePath)
        {
            if (!FileSaverViewModel.IsSaved)
            {
                var res = MessageBox.Show(Application.Current.MainWindow, "現在のファイルが保存されていません。\r\n保存しますか?", "Notice", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Cancel) return;
                if (res == MessageBoxResult.Yes) Save();
            }

            var text = FileSaverViewModel.LoadText(filePath);

            _baseControl.Changed -= BaseControl_Changed;
            _previousText = text;
            _baseControl.Text = text.Text;
            _isChanged = false;
            _baseControl.Changed += BaseControl_Changed;
        }

        #region FileCommand

        private Command? _saveCommand;
        public Command SaveCommand => _saveCommand ??= new Command(Save);


        private Command? _saveAsCommand;

        public Command SaveAsCommand => _saveAsCommand ??= new Command(SaveAs);
        #endregion

        #endregion

        #region DropTarget

        public void DragOver(IDropInfo dropInfo)
        {
            var haveFiles = ((DataObject)dropInfo.Data).GetFileDropList().OfType<string>().Any();
            dropInfo.Effects = haveFiles
                ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().OfType<string>().ToList();

            if (files.Count == 0) return;

            Load(files[0]);

        }
        #endregion

        #region DebugCommand

        private Command? _toggleBreakPointCommand;
        public Command ToggleBreakPointCommand 
            => _toggleBreakPointCommand ??= new Command(ToggleBreakPointOnCurrentPos);

        private void ToggleBreakPointOnCurrentPos()
        {
            var pos = _baseControl.GetEditPlace(_baseControl.SelectionStart);
            _baseControl.ToglleDebuggingPoint(pos);

        }

        #endregion

        #region ContextMenuItemCommand


        private Command? _selectedTextConvertToPrintCodeCommand;

        public Command SelectedTextConvertToPrintCodeCommand
                => _selectedTextConvertToPrintCodeCommand ??= new Command(SelectedTextConvertToPrintCode);

        private void SelectedTextConvertToPrintCode()
        {
            var selectedText = _baseControl.SelectedText;
            var selectionStart = _baseControl.SelectionStart;
            var selectionLength = _baseControl.SelectionLength;
            var newText = TextCodeMaker.AutoMaker.GetSimplyTextOutputer(selectedText);
            _baseControl.Document.Replace(selectionStart, selectionLength, newText);
        }

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
