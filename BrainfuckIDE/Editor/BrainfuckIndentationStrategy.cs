using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsUtils;

namespace BrainfuckIDE.Editor.CodeAnalysis
{
    class BrainfuckIndentationStrategy : IIndentationStrategy
    {
        public static IIndentationStrategy Instance { get; } = new BrainfuckIndentationStrategy();

        private BrainfuckIndentationStrategy()
        {

        }


        public void IndentLine(TextDocument document, DocumentLine line)
        {
            return;
            var prevText = line?.PreviousLine.ApllyTo(document.GetText) ?? string.Empty;
            var addString = document.GetText(line) ?? string.Empty;
            var nestString = new string(prevText.TakeWhile(p => p == ' ').ToArray()) ?? string.Empty;
            if (prevText.LastOrDefault() == '[' && addString.FirstOrDefault() == ']')
            {

                var offset = line.Offset;
                document.Replace(line, $"\n{nestString}{addString}");
                document.Insert(offset, $"{nestString}{NestRefactor.NestString}");
            }
            else
            {
                var offset = line.Offset;
                document.Insert(offset, $"{nestString}");
            }
        }

        public void IndentLines(TextDocument document, int beginLine, int endLine)
        {
            //throw new NotImplementedException();
        }
    }
}
