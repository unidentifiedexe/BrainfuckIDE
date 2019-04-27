#nullable enable

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using BrainfuckInterpreter;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace BrainfuckIDE.Editor
{
    partial class BrainfuckTextEditControl : TextEditor
    {
        private DebuggingColorizeAvalonEdit _debuggingColorizeAvalonEdit = new DebuggingColorizeAvalonEdit();

        public BrainfuckTextEditControl() : base()
        {
            base.Loaded += BrainfuckTextEditControl_Loaded;
            this.TextArea.Document.Changing += Document_Changing;
            this.PreviewMouseRightButtonDown += BrainfuckTextEditControl_PreviewMouseLeftButtonDown;
            this.DataContextChanged += BrainfuckTextEditControl_DataContextChanged;
        }

        private void BrainfuckTextEditControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is INotifyPropertyChanged newVal)
                newVal.PropertyChanged += NewVal_PropertyChanged;

            if (e.OldValue is INotifyPropertyChanged oldVal)
                oldVal.PropertyChanged -= NewVal_PropertyChanged;
        }

        private void NewVal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TextArea.TextView.Redraw();
        }

        private void BrainfuckTextEditControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextEditor editor)
            {
                var pos = editor.GetPositionFromPoint(e.GetPosition(editor));
                if (pos.HasValue)
                    ToglleDebuggingPoint(pos.Value.Location);
            }
        }

        private void Document_Changing(object sender, DocumentChangeEventArgs e)
        {
            var pos = e.Offset;
            var deleteLen = e.RemovalLength;
            var addLen = e.InsertionLength;
            var newArr = _debuggingColorizeAvalonEdit.BreakPoints
                .Select(PositionConverter).Where(p => p != -1).ToArray();
            _debuggingColorizeAvalonEdit.BreakPoints.Clear();
            _debuggingColorizeAvalonEdit.BreakPoints.AddRange(newArr);

            _debuggingColorizeAvalonEdit.RunnningPosition =
                PositionConverter(_debuggingColorizeAvalonEdit.RunnningPosition);


            int PositionConverter(int from)
            {
                if (from < pos) return from;
                else if (from < deleteLen) return -1;
                else return from - deleteLen + addLen;
            }


        }

        private void BrainfuckTextEditControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitializeSyntaxHighlight();
        }

        private void InitializeSyntaxHighlight()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("BrainfuckIDE.Editor.SyntaxHighlight.xshd");

            using (var reader = new XmlTextReader(stream))
            {
                this.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

            }

            this.TextArea.TextView.LineTransformers.Add(_debuggingColorizeAvalonEdit);
        }

        private void ToglleDebuggingPoint(TextLocation location)
        {
            var nearestLoc = this.NearestEfectivPosition(location);
            if (nearestLoc.IsEmpty) return;
            var pos = this.TextArea.Document.GetOffset(nearestLoc);
            if (_debuggingColorizeAvalonEdit.BreakPoints.Contains(pos))
                _debuggingColorizeAvalonEdit.BreakPoints.Remove(pos);
            else
                _debuggingColorizeAvalonEdit.BreakPoints.Add(pos);

            this.TextArea.TextView.Redraw();
        }

        public IEnumerable<TextLocation> GetBreakPoints()
        {
            return
                _debuggingColorizeAvalonEdit.BreakPoints.Select(GetEditPlace);
        }

        public TextLocation RunnningPosition
        {
            get => GetEditPlace(_debuggingColorizeAvalonEdit.RunnningPosition);
            set => _debuggingColorizeAvalonEdit.RunnningPosition = GetOffset(value);
        }

        private TextLocation GetEditPlace(int offset)
        {
            if (offset < 0) return TextLocation.Empty;
            return this.TextArea.Document.GetLocation(offset);
        }


        private int GetOffset(TextLocation location)
        {
            if (location.IsEmpty) return -1;
            return this.TextArea.Document.GetOffset(location);
        }



        public TextLocation NearestEfectivPosition(TextLocation textLocation)
        {
            return 
                new TextDocumentCharFinder(this.TextArea.Document, textLocation)
                .FindFirst(p => EffectiveCharacters.Characters.Contains(p))
                .Location;
        }
    }
}
