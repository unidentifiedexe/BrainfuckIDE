using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.CodeAnalysis
{
    /// <summary> 対応する括弧を探すためのヘルパークラス </summary>
    class MatchingFarenthesisFinder
    {

        public static CharLocationTuple Find(TextDocument document, TextLocation location)
        {
            return new MatchingFarenthesisFinder(document, location).Find();
        }
        private readonly TextDocumentCharFinder _charFinder;

        public CharLocationTuple TurgetLetter { get; }

        public MatchingFarenthesisFinder(TextDocument document, TextLocation location)
        {
            var currentLetter = document.GetCharAt(document.GetOffset(location));

            TurgetLetter = new CharLocationTuple(currentLetter, location);

            _charFinder = new TextDocumentCharFinder(document, location);
        }

        public CharLocationTuple Find()
        {
            var nest = 0;

            if (TurgetLetter.Char == ']')
                return _charFinder.FindFirstDectimentalOrEmpty(Finder);
            else if (TurgetLetter.Char == '[')
                return _charFinder.FindFirstIncrimentalOrEmpty(Finder);
            else
                return CharLocationTuple.Empty;


            bool Finder(char letter)
            {
                return letter switch
                {
                    '[' => (++nest == 0),
                    ']' => (--nest == 0),
                    _ => false
                };
            }
        }

        public static IEnumerable<StartEndPair> FindAllOfAnotherLine(string text)
        {
            return Itr().OrderBy(p => p.Start);

            IEnumerable<StartEndPair> Itr()
            {
                var stack = new Stack<(int Index,int line)>();
                var line = 0;
                for (int index = 0; index < text.Length; index++)
                {
                    var letter = text[index];
                    if (letter == '[')
                    {
                        stack.Push((index, line));
                    }
                    else if (letter == ']')
                    {
                        if (stack.Count > 0)
                        {
                            var (start, prevLine) = stack.Pop();
                            if(prevLine != line)
                            yield return new StartEndPair(start, index);
                        }
                    }
                    else if (letter == '\n' || letter == '\r')
                        line++;
                }
            }
        }


    }

    readonly struct StartEndPair
    {
        public int Start { get; }
        public int End { get; }
        public StartEndPair(int start, int end)
        {
            Start = start;
            End = end;
        }
    }

}
