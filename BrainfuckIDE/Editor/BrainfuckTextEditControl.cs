﻿#nullable enable

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
using BrainfuckIDE.Filer;
using ICSharpCode.AvalonEdit.Editing;

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
            this.TextArea.Document.Changed += Document_Changed;
            this.TextArea.Document.Changed += Document_Changed1;
            this.PreviewMouseDown += BrainfuckTextEditControl_PreviewMouseDown;
            this.DataContextChanged += BrainfuckTextEditControl_DataContextChanged;
            InitBindings();
            base.TextArea.TextEntering += TextArea_TextEntering;
            base.TextArea.TextEntered += TextArea_TextEntered;
            base.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            _bfFoldingManager = new BfFoldingManager(base.TextArea);
            _pointsColorizer.AddStrongerColorizingTransformer(_debuggingColorizeAvalonEdit);
        }

        private void Document_Changed1(object sender, DocumentChangeEventArgs e)
        {
            _bfFoldingManager.Update(e);
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

            var offset = TextArea.Caret.Offset - 1;
            if (offset > 0)
            {
                var letter = base.TextArea.Document.GetCharAt(offset);
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

            var lastLine = this.TextArea.Document.GetLineByNumber(this.TextArea.Document.LineCount);
            if (lastLine.Length != 0)
            {
                var caretOffset = TextArea.Caret.Offset;
                this.TextArea.Document.Insert(lastLine.EndOffset, LocalEnvironmental.Delimiter);
                TextArea.Caret.Offset = caretOffset;
            }
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
                        TextArea.IndentationStrategy?.IndentLines(TextArea.Document, matchedLoc.IsEmpty ? 1 : matchedLoc.Line, loc.Line);
                        #endregion

                    }
                    base.TextArea.Document.EndUpdate();
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

            var converter = e.GetOffsetConverter();
            var newArr = _debuggingColorizeAvalonEdit.BreakPoints
                .Select(p => converter.GetNewOffset(p)).Where(IsEfectiveAt).Distinct().Where(p => p >= 0).ToArray();
            _debuggingColorizeAvalonEdit.BreakPoints.Clear();
            _debuggingColorizeAvalonEdit.BreakPoints.AddRange(newArr);

            _debuggingColorizeAvalonEdit.RunnningPosition
               = NearestEfectivOffset(converter.GetNewOffset(_debuggingColorizeAvalonEdit.RunnningPosition));


            //if (_debuggingColorizeAvalonEdit.RunnningPosition >= TextArea.Document.TextLength)
            //    _debuggingColorizeAvalonEdit.RunnningPosition = TextArea.Document.TextLength - 1;
            //if (_debuggingColorizeAvalonEdit.RunnningPosition < 0) return;

            //var currentChar = this.TextArea.Document.GetCharAt(_debuggingColorizeAvalonEdit.RunnningPosition);
            //if (!EffectiveCharacters.Characters.Contains(currentChar)) return;
            //else
            //{
            //    var loc = GetEditPlace(_debuggingColorizeAvalonEdit.RunnningPosition);
            //    var newLoc = NearestEfectivPosition(loc);
            //    _debuggingColorizeAvalonEdit.RunnningPosition = GetOffset(newLoc);
            //}
            this.TextArea.TextView.Redraw();

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

            //var converter = e.GetOffsetConverter();

            ////var pos = e.Offset;
            ////var deleteLen = e.RemovalLength;
            ////var addLen = e.InsertionLength;
            //var newArr = _debuggingColorizeAvalonEdit.BreakPoints
            //    .Select(p => converter.GetNewOffset(p)).Where(IsEfectiveAt).Distinct().Where(p => p >= 0).ToArray();
            //_debuggingColorizeAvalonEdit.BreakPoints.Clear();
            //_debuggingColorizeAvalonEdit.BreakPoints.AddRange(newArr);

            //_debuggingColorizeAvalonEdit.RunnningPosition
            //   = NearestEfectivOffset(converter.GetNewOffset(_debuggingColorizeAvalonEdit.RunnningPosition));


            //int PositionConverter(int from)
            //{
            //    if (from < pos) return from;
            //    else if (from < pos + deleteLen) return -1;
            //    else return from - deleteLen + addLen;
            //}
        }

        private void BrainfuckTextEditControl_Loaded(object sender, RoutedEventArgs e)
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

        public TextLocation RunningPosition
        {
            get => GetEditPlace(_debuggingColorizeAvalonEdit.RunnningPosition);
            set
            {
                var inPos = value;
                if (!inPos.IsEmpty) inPos = NearestEfectivPosition(inPos);
                _debuggingColorizeAvalonEdit.RunnningPosition = GetOffset(inPos);
                this.TextArea.TextView.Redraw();

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

        public RunningState RunningState
        {
            get => _runningState;
            set
            {
                _runningState = value;
                IsReadOnly = (_runningState == RunningState.Running);
            }
        }
        private RunningState _runningState;


        public bool IsEfectiveAt(int offset)
        {
            if (offset == 0) return false;
            var letter = TextArea.Document.GetCharAt(offset);
            return EffectiveCharacters.IsEffectiveChar(letter);
        }

        public int NearestEfectivOffset(int offset)
        {
            if (offset < 0) return offset;
            else
                return GetOffset(NearestEfectivPosition(GetEditPlace(offset)));
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
            this.InputBindings.Add(GetInputBinding(SelectedTextConvertToPrintCode, Key.T, ModifierKeys.Control));
            this.InputBindings.Add(GetInputBinding(RefactSimplySelectedRange, Key.W, ModifierKeys.Control));
            this.InputBindings.Add(GetInputBinding(SetRunningPositionToCurrentPosition, Key.N, ModifierKeys.Control));
            this.PreviewMouseWheel += BrainfuckTextEditControl_MouseWheel;
        }

        private void BrainfuckTextEditControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0)
                    this.TextArea.FontSize += 1;
                if (e.Delta < 0 && this.TextArea.FontSize > 2)
                    this.TextArea.FontSize -= 1;
            }
        }

        private InputBinding GetInputBinding(Action action, Key key, ModifierKeys modifierKeys)
        {
            return GetInputBinding(new Command(action, CanEdit), key, modifierKeys);
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

            MoveLineHelper(GetNextRange);
            Range? GetNextRange(Range now)
            {
                var next = this.TextArea.Document.GetLineByOffset(now.End).NextLine;
                if (next == null) return null;
                return _bfFoldingManager.GetFoldedLineRange(next.Offset);
            }
        }

        private void MoveLineToPrevious()
        {
            MoveLineHelper(GetPrevRange);
            Range? GetPrevRange(Range now)
            {
                if (now.Start <= 0) return null;
                return _bfFoldingManager.GetFoldedLineRange(now.Start - 1);
            }
        }

        private void MoveLineHelper(Func<Range, Range?> getAnotherLine)
        {
            _bfFoldingManager.CashNowFoldData();
            var caretOffset = this.TextArea.Caret.Offset;
            var selection = TextArea.Selection;
            var selectionRange = GetCurrentSelectRange();
            var x = _bfFoldingManager.GetFoldedLineRange(selectionRange);
            var another = getAnotherLine(x);
            if (another == null) return;
            var y = another.Value;
            var info = this.TextArea.Document.Swap(x.Start, x.Length, y.Start, y.Length);
            this.TextArea.Caret.Offset = info.GetNewOffset(caretOffset, true);
            TextArea.Selection = GetSelection(selection, info, selectionRange);
        }
        private Selection GetSelection(Selection selection, SwapInfo info, Range oldRange)
        {
            var newStart = info.GetNewOffset(oldRange.Start, true);
            var newEnd = info.GetNewOffset(oldRange.End, true);
            var newStartLoc = new TextViewPosition(TextArea.Document.GetLocation(newStart));
            var newEndLoc = new TextViewPosition(TextArea.Document.GetLocation(newEnd));
            return selection switch
            {
                RectangleSelection _ => new RectangleSelection(this.TextArea, newStartLoc, newEndLoc),
                Selection s when s.IsEmpty => s,
                Selection _ => Selection.Create(this.TextArea, newStart, newEnd),
            };
        }


        private Range GetCurrentSelectRange()
        {
            var selection = TextArea.Selection;
            int e, s;
            if (!selection.EndPosition.Location.IsEmpty)
                e = Document.GetOffset(selection.EndPosition.Location);
            else
                e = this.TextArea.Caret.Offset;
            if (!selection.StartPosition.Location.IsEmpty)
                s = Document.GetOffset(selection.StartPosition.Location);
            else
                s = this.TextArea.Caret.Offset;
            return new Range(s, e);
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

        //private ICommand? _selectedTextConvertToPrintCodeCommand;

        //public ICommand SelectedTextConvertToPrintCodeCommand
        //        => _selectedTextConvertToPrintCodeCommand ??= new Command(SelectedTextConvertToPrintCode, CanEdit);

        private void SelectedTextConvertToPrintCode()
        {
            var selectedText = SelectedText;
            var selectionStart = SelectionStart;
            var selectionLength = SelectionLength;
            var newText = TextCodeMaker.AutoMaker.GetSimplyTextOutputer(selectedText);
            Document.Replace(selectionStart, selectionLength, newText);
        }


        private void SetRunningPositionToCurrentPosition()
        {
            if (RunningState == RunningState.Pause)
                RunningPosition = this.TextArea.Caret.Location;
        }

        private bool CanEdit()
        {
            return !IsReadOnly;
        }


        public void OpenAllFoldiongAt(TextLocation loc)
        {
            var offset = GetOffset(loc);

            _bfFoldingManager.OpenAllFoldiongAt(offset);
        }

        public void FocusAt(TextLocation loc)
        {
            OpenAllFoldiongAt(loc);
            ScrollTo(loc.Line, loc.Column);
        }


    }

}
