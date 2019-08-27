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
using System.Windows;
using ICSharpCode.AvalonEdit.CodeCompletion;
using BrainfuckIDE.Editor.Snippets;
using System.Windows.Media;
using WpfUtils;
using BrainfuckIDE.Editor.CodeAnalysis;
using System.Diagnostics;
using BrainfuckIDE.Editor.ColorizingTransformer;
using BrainfuckIDE.Utils;
using ICSharpCode.AvalonEdit.Folding;
using System.Collections.Specialized;

namespace BrainfuckIDE.Editor
{
    partial class BrainfuckTextEditControl : TextEditor
    {
        private readonly DebuggingColorizingTransformer _debuggingColorizeAvalonEdit = new DebuggingColorizingTransformer();
        private readonly PointsColorizingTransformer _pointsColorizer = new PointsColorizingTransformer(Colors.LightSkyBlue.AddAlpha(0x7f));

        private readonly BfFoldingManager _bfFoldingManager;

        public event EventHandler<DocumentChangeEventArgs> Changed
        {
            add { this.TextArea.Document.Changed += value; }
            remove { this.TextArea.Document.Changed -= value; }
        }

        public BrainfuckTextEditControl() : base()
        {
            base.TextArea.Options.IndentationSize = 2;
            base.TextArea.Options.ConvertTabsToSpaces = true;
            base.TextArea.IndentationStrategy = BrainfuckIndentationStrategy.Instance;
            base.TextArea.LeftMargins.CollectionChanged += LeftMargins_CollectionChanged;
            base.Loaded += BrainfuckTextEditControl_Loaded;
            this.TextArea.Document.Changing += Document_Changing;
            this.TextArea.Document.Changed += Document_Changed; ;
            this.PreviewMouseDown += BrainfuckTextEditControl_PreviewMouseDown;
            this.DataContextChanged += BrainfuckTextEditControl_DataContextChanged;
            InitBindings();
            base.TextArea.TextEntering += TextArea_TextEntering;
            base.TextArea.TextEntered += TextArea_TextEntered;
            base.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            _bfFoldingManager = new BfFoldingManager(base.Document, base.TextArea);
        }

        private void LeftMargins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems?.OfType<FoldingMargin>() ?? Enumerable.Empty<FoldingMargin>())
            {
                item.FoldingMarkerBackgroundBrush = new SolidColorBrush(EditorColoers.Background);
                item.FoldingMarkerBrush = this.LineNumbersForeground;
                item.SelectedFoldingMarkerBackgroundBrush = item.FoldingMarkerBackgroundBrush;
                item.SelectedFoldingMarkerBrush = new SolidColorBrush(EditorColoers.Foreground);
            }
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {

            var offset = TextArea.Caret.Offset -1;
            if(offset >0)
            {
                var letter = base.TextArea.Document.GetCharAt(offset );
                if (letter == '[' || letter == ']')
                {
                    var loc = base.Document.GetLocation(offset);
                    var cuppledLoc = MatchingFarenthesisFinder.Find(base.Document, loc).Location;
                    var cuppledLocOffset = cuppledLoc.IsEmpty ? -1 : base.Document.GetOffset(cuppledLoc);
                    _pointsColorizer.Points = new[] { offset, cuppledLocOffset };
                }
                else
                    _pointsColorizer.Clear();

                this.TextArea.TextView.Redraw();
            }

        }

        /// <summary> "[]"の処理を記述 </summary>
        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length == 1)
            {
                if (this.TextArea.Selection.Length == 0)
                {
                    base.TextArea.Document.BeginUpdate();
                    if (e.Text[0] == '[')
                    {
                        TextArea.Document.Insert(TextArea.Caret.Offset, "]");
                        TextArea.Caret.Offset--;
                    }
                    else if (e.Text[0] == ']')
                    {
                        if (TextArea.Caret.Offset < TextArea.Document.TextLength &&
                            TextArea.Document.GetCharAt(TextArea.Caret.Offset) == ']')
                            TextArea.Document.Remove(TextArea.Caret.Offset, 1);


                        #region 自動インデント
                        var loc = TextArea.Document.GetLocation(TextArea.Caret.Offset - 1);
                        var matchedLoc = MatchingFarenthesisFinder.Find(TextArea.Document, loc).Location;
                        if (!matchedLoc.IsEmpty)
                            TextArea.IndentationStrategy?.IndentLines(TextArea.Document, matchedLoc.Line, loc.Line);
                        #endregion

                    }
                    base.TextArea.Document.EndUpdate();

                    _bfFoldingManager.Update();
                }

            }
        }


        #region CodeCompletion 

        private Snippets.CompletionWindow? _completionWindow;

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {

            var isShowCompletionWindow =
                e.Text.Length == 1 &&
                !EffectiveCharacters.Characters.Contains(e.Text[0]) &&
                char.IsLetter(e.Text[0]);

            if (isShowCompletionWindow == false)
            {
                _completionWindow?.Close();
            }

            if (_completionWindow == null && isShowCompletionWindow)
            {
                _completionWindow = CompletionWindowCreator.Creato(TextArea);
                _completionWindow.Show();
                // ウインドウを閉じたときの処理
                _completionWindow.Closed += delegate { _completionWindow = null; };
            }



            // e.Handled = true; を設定してはならない
        }
        #endregion

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

            if(e.RemovalLength >2 || e.InsertionLength > 2)
            {
                _bfFoldingManager.Update();
            }
        }

        private void BrainfuckTextEditControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        private void BrainfuckTextEditControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextEditor editor)
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    var pos = editor.GetPositionFromPoint(e.GetPosition(editor));
                    if (pos.HasValue)
                        ToglleDebuggingPoint(pos.Value.Location);
                }
            }
        }

        private void Document_Changing(object sender, DocumentChangeEventArgs e)
        {

            var pos = e.Offset;
            var deleteLen = e.RemovalLength;
            var addLen = e.InsertionLength;
            var newArr = _debuggingColorizeAvalonEdit.BreakPoints
                .Select(p => PositionConverter(p)).Where(p => p != -1).ToArray();
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
            this.TextArea.TextView.LineTransformers.Add(_pointsColorizer);
        }

        public void ToglleDebuggingPoint(TextLocation location)
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

        public TextLocation GetEditPlace(int offset)
        {
            if (offset < 0) return TextLocation.Empty;
            return this.TextArea.Document.GetLocation(offset);
        }


        public int GetOffset(TextLocation location)
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



        #region KeyBindings

        private void InitBindings()
        {
            this.InputBindings.Add(GetInputBinding(CutCrentLine, Key.L, ModifierKeys.Control));
            this.InputBindings.Add(GetInputBinding(MoveLineToNext, Key.Down, ModifierKeys.Alt));
            this.InputBindings.Add(GetInputBinding(MoveLineToPrevious, Key.Up, ModifierKeys.Alt));
            this.InputBindings.Add(GetInputBinding(PointerDirectionInverse, Key.R, ModifierKeys.Control));
            this.InputBindings.Add(GetInputBinding(ConvertToRepeatString, Key.E, ModifierKeys.Control));
            this.InputBindings.Add(GetInputBinding(SelectedTextConvertToPrintCodeCommand, Key.T, ModifierKeys.Control));
            this.InputBindings.Add(GetInputBinding(RefactSimplySelectedRange, Key.W, ModifierKeys.Control));
        }

        private InputBinding GetInputBinding(Action action, Key key, ModifierKeys modifierKeys)
        {
            return GetInputBinding(new WpfUtils.Command(action, CanEdit), key, modifierKeys);
        }
        private static InputBinding GetInputBinding(ICommand command, Key key, ModifierKeys modifierKeys)
        {
            return new InputBinding(command, new KeyGesture(key, modifierKeys));
        }


        private void CutCrentLine()
        {
            var line = this.TextArea.Caret.Line;
            var lineSegment = this.TextArea.Document.GetLineByNumber(line);
            this.TextArea.Document.Remove(lineSegment.Offset, lineSegment.TotalLength);
        }


        private void MoveLineToNext()
        {
            var caretLine = this.TextArea.Caret.Line;
            var caretColum = this.TextArea.Caret.Column;
            var line = this.TextArea.Document.GetLineByNumber(this.TextArea.Caret.Line);
            var next = line.NextLine;
            if (next == null) return;
            var txt = this.TextArea.Document.GetText(next) + Environment.NewLine +
                      this.TextArea.Document.GetText(line);
            if (next.DelimiterLength != 0) txt += Environment.NewLine;

            this.TextArea.Document.Replace(line.Offset, line.TotalLength + next.TotalLength, txt);
            this.TextArea.Caret.Line = caretLine + 1;
            this.TextArea.Caret.Column = caretColum;

        }

        private void MoveLineToPrevious()
        {
            var caretLine = this.TextArea.Caret.Line;
            var caretColum = this.TextArea.Caret.Column;
            var line = this.TextArea.Document.GetLineByNumber(this.TextArea.Caret.Line);
            var prev = line.PreviousLine;
            if (prev == null) return;
            var txt = this.TextArea.Document.GetText(line) + Environment.NewLine +
                       this.TextArea.Document.GetText(prev);
            if (line.DelimiterLength != 0) txt += Environment.NewLine;

            this.TextArea.Document.Replace(prev.Offset, prev.TotalLength + line.TotalLength, txt);
            this.TextArea.Caret.Line = caretLine - 1;
            this.TextArea.Caret.Column = caretColum;

        }

        private void RefactSimplySelectedRange()
        {

            var selection = this.TextArea.Selection;
            var segments = selection.Segments;
            TextArea.Document.BeginUpdate();
            foreach (var item in segments)
            {
                var text = TextArea.Document.GetText(item);
                TextArea.Document.Replace(item, Refacter.CodeConverter.ToSimplyCode(text));
            }
            TextArea.Document.EndUpdate();
        }
        #endregion

        private void PointerDirectionInverse()
        {
            var selection = this.TextArea.Selection;
            var segments = selection.Segments;
            TextArea.Document.BeginUpdate();
            foreach (var item in segments)
            {
                var text = TextArea.Document.GetText(item);
                TextArea.Document.Replace(item, Refacter.CodeConverter.PointerInverse(text));
            }
            TextArea.Document.EndUpdate();
        }


        private void ConvertToRepeatString()
        {
            var selection = this.TextArea.Selection;
            if (selection.Segments.Any()) return;

            var loc = this.TextArea.Caret.Location;

            var firstPos = this.TextArea.Caret.Offset - (loc.Column - 1);
            var txt = this.TextArea.Document.GetText(firstPos, (loc.Column - 1));
            if (txt.Length == 0) return;
            if (!char.IsDigit(txt.Last())) return;

            var digitStack = new Stack<char>();
            var textStack = new Stack<char>();
            var baseLength = 0;
            foreach (var letter in txt.Reverse())
            {
                baseLength++;
                if (!textStack.Any())
                {
                    if (char.IsDigit(letter)) digitStack.Push(letter);
                    else if (EffectiveCharacters.IsEffectiveChar(letter))
                    {
                        textStack.Push(letter);
                        textStack.Push('{');
                        break;
                    }
                    else if (letter == '}')
                        textStack.Push(letter);
                    else
                        break;
                }
                else
                {
                    textStack.Push(letter);
                    if (letter == '{') break;
                }

            }

            if (digitStack.Any() && textStack.Any() && textStack.Pop() == '{')
            {
                var digits = new string(digitStack.ToArray());
                var text = new string(textStack.ToArray());
                text = text.TrimEnd('}');

                if (!byte.TryParse(digits, out var num))
                    return;

                var newText = String.Join("", Enumerable.Repeat(text, num));

                this.TextArea.Document.Replace(this.TextArea.Caret.Offset - baseLength, baseLength, newText);

            }


        }

        private ICommand? _selectedTextConvertToPrintCodeCommand;

        public ICommand SelectedTextConvertToPrintCodeCommand
                => _selectedTextConvertToPrintCodeCommand ??= new Command(SelectedTextConvertToPrintCode, CanEdit);

        private void SelectedTextConvertToPrintCode()
        {
            var selectedText = SelectedText;
            var selectionStart = SelectionStart;
            var selectionLength = SelectionLength;
            var newText = TextCodeMaker.AutoMaker.GetSimplyTextOutputer(selectedText);
            Document.Replace(selectionStart, selectionLength, newText);
        }

        private bool CanEdit()
        {
            return !IsReadOnly;
        }
    }
}
