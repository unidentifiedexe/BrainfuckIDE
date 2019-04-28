using System;
using System.Collections.Generic;
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

        private BrainfuckTextEditControl _baseControl = null;


        private bool _isChanged = false;
        private Guid _textGuid = Guid.NewGuid();

        public void SetControl(BrainfuckTextEditControl editControl)
        {
            _baseControl = editControl;
            _baseControl.Changed += BaseControl_Changed;
        }

        private void BaseControl_Changed(object sender, EventArgs e)
        {
            _isChanged = true;
        }

        private string _filePath = string.Empty;


        public SourceText SourceCode
        {
            get
            {
                if(_isChanged)
                    _textGuid = Guid.NewGuid();
                _isChanged = false;
                return new SourceText(_textGuid, _baseControl.Text);
            }
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
            return new Place(place.Line - 1, place.Column -1);
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

        public bool IsEmpty => (Guid.Empty == Guid);

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
