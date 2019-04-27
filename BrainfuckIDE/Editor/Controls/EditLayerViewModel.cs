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


        public void SetControl(BrainfuckTextEditControl editControl)
        {
            _baseControl = editControl;
        }


        private string _filePath = string.Empty;

        public string SourceCode => _baseControl.Text;

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

}
