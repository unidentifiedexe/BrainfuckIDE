using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace BrainfuckIDE.Editor.Snippets
{
    class NonArgumentSnippet : ISnippet
    {
        public NonArgumentSnippet(string shortcut, string title,string text)
        {
            Shortcut = shortcut;
            Title = title;
            _text = text;
        }


        private string _text;

        public string Shortcut { get; protected set; }

        public string Title { get; protected set;}

        public void Insert(TextArea textArea)
        {

            if (textArea == null)
                throw new ArgumentNullException("textArea");

            ISegment selection = textArea.Selection.SurroundingSegment;
            int insertionPosition = textArea.Caret.Offset;


            textArea.Document.Insert(insertionPosition, _text);


        }
    }
}
