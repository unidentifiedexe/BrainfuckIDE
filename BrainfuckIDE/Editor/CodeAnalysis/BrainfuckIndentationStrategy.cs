using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainfuckIDE.Utils;

namespace BrainfuckIDE.Editor.CodeAnalysis
{
    class BrainfuckIndentationStrategy : IIndentationStrategy
    {
        public static IIndentationStrategy Instance { get; } = new BrainfuckIndentationStrategy(2);

        private readonly int _nestSpaceNum;

        public int NestSpaceNum => _nestSpaceNum;

        public BrainfuckIndentationStrategy(int nestSpaceNum)
        {
            _nestSpaceNum = nestSpaceNum;
        }

        public void IndentLine(TextDocument document, DocumentLine line)
        {
            var prevText = line?.PreviousLine.ApllyTo(document.GetText) ?? string.Empty;
            var addString = document.GetText(line).TrimStart() ?? string.Empty;
            var nestString = new string(prevText.TakeWhile(p => p == ' ').ToArray()) ?? string.Empty;


            if (prevText.Reverse().TakeWhile(p => p != ']').Contains('['))
            {
                if (addString.FirstOrDefault() == ']')
                {
                    document.Replace(line, $"\n{nestString}{addString}");
                }
                document.Insert(line.Offset, $"{nestString}{SpaceStringMap.GetNestString(NestSpaceNum)}");
            }
            else
            {
                document.Insert(line.Offset, $"{nestString}");
            }
        }

        public void IndentLines(TextDocument document, int beginLine, int endLine)
        {

            if (beginLine == endLine)
            {
                return;
            }

            if (beginLine > endLine)
                (beginLine, endLine) = (endLine, beginLine);

            var lines = RangeLineItr(beginLine, endLine).ToArray();

            var strs = NestRefactor.Refact(lines.Select(document.GetText), NestSpaceNum);

            foreach (var (line,str) in lines.Zip(strs,(l,s) => (l,s)))
                document.Replace(line, str);

            IEnumerable <DocumentLine> RangeLineItr(int b, int e)
            {
                var line = document.GetLineByNumber(b);

                while (line.LineNumber != e)
                {
                    yield return line;
                    line = line.NextLine;
                }
                yield return line;
            }
        }
    }
}
