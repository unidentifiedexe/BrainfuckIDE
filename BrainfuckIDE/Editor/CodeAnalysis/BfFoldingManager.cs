using BrainfuckIDE.Utils;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrainfuckIDE.Editor.CodeAnalysis
{
    class BfFoldingManager
    {
        private readonly FoldingManager _foldingManager;
        private readonly TextDocument _document;
        public BfFoldingManager(TextArea textArea)
        {
            _document = textArea.Document;
            _foldingManager = FoldingManager.Install(textArea);
        }


        FoldInfo[] _prevFoldings = new FoldInfo[0];

        public void CashNowFoldData()
        {
            _prevFoldings = _foldingManager.AllFoldings.Select(p => new FoldInfo(p)).ToArray();
        }

        public void Update(DocumentChangeEventArgs e)
        {
            if (e.TryCreateSwapInfo(out var swapInfo))
                UpdateBySwap(swapInfo);
            else
                Update();
            CashNowFoldData();
        }
        private void Update()
        {

            var folds = EnumereteNewFoldings(_document.Text);


            _foldingManager.UpdateFoldings(folds, -1);
        }

        private void UpdateBySwap(SwapInfo e)
        {
            if (e is null)
                throw new ArgumentNullException(nameof(e));


            var dic = _prevFoldings.Where(p => p.IsFolded)
                .Where(p => e.X.WasIn(p.Start) && e.X.WasIn(p.End) || (e.Y.WasIn(p.Start) && e.Y.WasIn(p.End)))
                .Select(p => e.GetNewOffset(p.Start))
                .ToHashSet();



            var folds = EnumereteNewFoldings(_document.Text).ToArray();
            _foldingManager.UpdateFoldings(folds, -1);

            foreach (var item in _foldingManager.AllFoldings)
            {
                if (dic.Contains(item.StartOffset))
                    item.IsFolded = true;
            }

        }

        private IEnumerable<NewFolding> EnumereteNewFoldings(string text)
        {
            return MatchingFarenthesisItr(text).Concat(RegionItr(text)).OrderBy(p => p.StartOffset);
        }

        private IEnumerable<NewFolding> MatchingFarenthesisItr(string text)
        {
            return MatchingFarenthesisFinder.FindAllOfAnotherLine(text)
                .Select(p => new NewFolding(p.Start + 1, p.End) { Name = "..." });

        }


        private IEnumerable<NewFolding> RegionItr(string text)
        {
            return RegionItr(text.Replace("\r\n", "\n").Split('\n'));
        }
        private IEnumerable<NewFolding> RegionItr(IEnumerable<string> txts)
        {
            int line = 0;
            var stack = new Stack<StackNode>();
            foreach (var item in txts)
            {
                line++;
                var trimed = item.TrimStart();
                if (trimed.StartsWith("#region"))
                {
                    var sub = trimed.Substring("#region".Length).Trim();

                    if (string.IsNullOrEmpty(sub))
                        sub = "#region";
                    var col = item.Length - trimed.Length;
                    if (col > 0) col++;
                    stack.Push(new StackNode(_document.GetOffset(line, col), sub));
                }
                else if (trimed.StartsWith("#endregion"))
                {
                    if (stack.Any())
                    {
                        var poped = stack.Pop();
                        yield return new NewFolding(poped.Pos, _document.GetOffset(line, item.Length)+1) { Name = poped.Text };
                    }
                }
            }
        }
        private struct StackNode
        {
            public int Pos;
            public string Text;

            public StackNode(int pos, string text)
            {
                Pos = pos;
                Text = text;
            }
        }

        /// <summary>
        /// 指定位置が含まれる表示上の一行の範囲を、終端の改行文字を含めない区間で取得します。
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Range GetFoldedLineRange(int offset)
        {
            var line = _document.GetLineByOffset(offset);

            var sLine = GetLastFoldedSection(line.Offset, p => p.StartOffset) ?? line;
            var eLine = GetLastFoldedSection(line.EndOffset, p => p.EndOffset) ?? line;

            return new Range(sLine.Offset, eLine.EndOffset/* + eLine.DelimiterLength*/);


            DocumentLine GetLastFoldedSection(int x, Func<FoldingSection, int> func)
            {
                var foldinfSection = _foldingManager.GetFoldingsContaining(x).Reverse().TakeWhile(p => p.IsFolded).LastOrDefault();
                return foldinfSection?.ApllyTo(func).ApllyTo(_document.GetLineByOffset);
            }

        }


        private struct FoldInfo
        {
            public int Start { get; }
            public int End { get; }
            public bool IsFolded { get; }

            public FoldInfo(FoldingSection s)
            {
                Start = s.StartOffset;
                End = s.EndOffset;
                IsFolded = s.IsFolded;
            }
        }
    }

    /// <summary> 区間の範囲を示す構造体 </summary>
    struct Range
    {
        public int Start { get; }
        public int End { get; }

        public int Length => End - Start;
        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }

        public void Deconstruct(out int start, out int end) => (start, end) = (Start, End);
    }

}
