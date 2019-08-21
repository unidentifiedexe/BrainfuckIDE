#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.AvalonEdit.Document;

namespace BrainfuckIDE.Editor
{
    internal class TextDocumentCharFinder
    {

        private readonly DocumentLine _firstLine;
        private readonly TextDocument _document;
        private readonly TextLocation _startLocation;
        public TextDocumentCharFinder(TextDocument document):this(document, new TextLocation(1, 1))
        {
        }
        public TextDocumentCharFinder(TextDocument document, TextLocation location)
        {
            if (location.IsEmpty) location = new TextLocation(1, 1);
            _document = document;
            _startLocation = location;
            _firstLine = document.GetLineByNumber(location.Line);
        }



        //TODO : Check
        public CharLocationTuple FindFirstDectimentalOrEmpty(Func<char, bool> predicate)
        {
            var turgetLine = _firstLine;
            var line = _startLocation.Line;
            var startcolumn = _startLocation.Column - 1;
            bool isFirst = true;
            while (turgetLine != null)
            {
                var text = _document.GetText(turgetLine);
                for (int i = (isFirst ? startcolumn : text.Length - 1); i >= 0; i--)
                {
                    var letter = text[i];
                    if (predicate(letter))
                        return new CharLocationTuple(letter, line, i + 1);
                }
                turgetLine = turgetLine.PreviousLine;
                line--;
                isFirst = false;
            }

            return CharLocationTuple.Empty;
        }

        public CharLocationTuple FindFirstIncrimentalOrEmpty(Func<char, bool> predicate)
        {
            var turgetLine = _firstLine;
            var line = _startLocation.Line;
            var startcolumn = _startLocation.Column - 1;

            while (turgetLine != null)
            {
                var text = _document.GetText(turgetLine);
                for (int i = startcolumn; i < text.Length; i++)
                {
                    var letter = text[i];
                    if (predicate(letter))
                        return new CharLocationTuple(letter, line, i + 1);
                }
                startcolumn = 0;
                line++;
                turgetLine = turgetLine.NextLine;
            }

            return CharLocationTuple.Empty;
        }


    }

    internal struct CharLocationTuple
    {

        public char Char { get; }

        public TextLocation Location { get; }


        public CharLocationTuple(char letter, int line, int column)
        {
            Char = letter;
            Location = new TextLocation(line, column);
        }

        public CharLocationTuple(char Letter, TextLocation location)
        {
            Char = Letter;
            Location = location;
        }

        public void Deconstruct(out char Letter, out TextLocation location)
        {
            Letter = Char;
            location = Location;
        }

        public static CharLocationTuple Empty { get; } = new CharLocationTuple(new char(), TextLocation.Empty);

        public static bool IsEmpty(CharLocationTuple data) => data.Location.IsEmpty;


        public bool IsEmpty() => IsEmpty(this);


    }

}
