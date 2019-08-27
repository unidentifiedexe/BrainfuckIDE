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
        public BfFoldingManager(TextDocument document,TextArea textArea )
        {
            _document = document;
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

            _foldingManager.UpdateFoldings(newFolds, -1);
        }
    }
}
