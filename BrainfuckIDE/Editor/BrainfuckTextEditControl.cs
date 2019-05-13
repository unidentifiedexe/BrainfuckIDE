#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Interpreter;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace BrainfuckIDE.Editor
{
    partial class BrainfuckTextEditControl : TextEditor
    {
        private readonly DebuggingColorizeAvalonEdit _debuggingColorizeAvalonEdit = new DebuggingColorizeAvalonEdit();

        public event EventHandler<DocumentChangeEventArgs> Changed
        {
            add { this.TextArea.Document.Changed += value; }
            remove { this.TextArea.Document.Changed -= value; }
        }

        public BrainfuckTextEditControl() : base()
        {
            base.Loaded += BrainfuckTextEditControl_Loaded;
            this.TextArea.Document.Changing += Document_Changing;
            this.TextArea.Document.Changed += Document_Changed; ;
            this.PreviewMouseRightButtonDown += BrainfuckTextEditControl_PreviewMouseLeftButtonDown;
            this.DataContextChanged += BrainfuckTextEditControl_DataContextChanged;
        }

        private void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            if (_debuggingColorizeAvalonEdit.RunnningPosition >= TextArea.Document.TextLength)
                _debuggingColorizeAvalonEdit.RunnningPosition = TextArea.Document.TextLength - 1;
            if (_debuggingColorizeAvalonEdit.RunnningPosition < 0) return;
            var currentChar = this.TextArea.Document.GetCharAt(_debuggingColorizeAvalonEdit.RunnningPosition);
            if (EffectiveCharacters.Characters.Contains(currentChar)) return;
            else
            {
                var loc = GetEditPlace(_debuggingColorizeAvalonEdit.RunnningPosition);
                var newLoc = NearestEfectivPosition(loc);
                _debuggingColorizeAvalonEdit.RunnningPosition = GetOffset(newLoc);
                this.TextArea.TextView.Redraw();
            }
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
                .Select(p=>PositionConverter(p)).Where(p => p != -1).ToArray();
            _debuggingColorizeAvalonEdit.BreakPoints.Clear();
            _debuggingColorizeAvalonEdit.BreakPoints.AddRange(newArr);

            _debuggingColorizeAvalonEdit.RunnningPosition
                = e.OffsetChangeMap.GetNewOffset(_debuggingColorizeAvalonEdit.RunnningPosition);


            int PositionConverter(int from)
            {
                if (from < pos) return from;
                else if (from < pos + deleteLen) return -1;
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
            set
            {
                var inPos = value;
                if (!inPos.IsEmpty) inPos = NearestEfectivPosition(inPos);
                _debuggingColorizeAvalonEdit.RunnningPosition = GetOffset(inPos);
                
             }
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
                .FindFirstIncrimentalOrEmpty(p => EffectiveCharacters.Characters.Contains(p))
                .Location;
        }


        public TextLocation NearestEfectivPositionAsDecrimental(TextLocation textLocation)
        {
            return
                new TextDocumentCharFinder(this.TextArea.Document, textLocation)
                .FindFirstDectimentalOrEmpty(p => EffectiveCharacters.Characters.Contains(p))
                .Location;
        }
    }
}
