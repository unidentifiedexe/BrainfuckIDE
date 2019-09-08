using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.CodeAnalysis
{
    class BfFoldingManager
    {
        private readonly FoldingManager _foldingManager;
        private readonly TextDocument _document;
        public BfFoldingManager(TextArea textArea )
        {
            _document = textArea.Document;
            _foldingManager = FoldingManager.Install(textArea);
        }

        public void Update()
        {
            var txt = _document.Text;
            var newFolds = MatchingFarenthesisFinder.FindAllOfAnotherLine(txt).Select(p => new NewFolding(p.Start + 1, p.End)).ToArray();

            foreach (var item in newFolds)
            {
                item.Name = "...";
            }

            var resionFolds = RegionItr(txt.Replace("\r\n", "\n").Split('\n'));
            var folds = newFolds.Concat(resionFolds).OrderBy(p => p.StartOffset);


            _foldingManager.UpdateFoldings(folds, -1);
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
                    stack.Push(new StackNode(_document.GetOffset(line, item.Length - trimed.Length) +1, sub));
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
    }
}
