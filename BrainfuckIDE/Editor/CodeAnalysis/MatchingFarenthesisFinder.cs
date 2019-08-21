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
    }
}
