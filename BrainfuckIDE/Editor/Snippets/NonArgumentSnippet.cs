using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainfuckIDE.Editor.Snippets.BasicBfSnippets;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;

namespace BrainfuckIDE.Editor.Snippets
{
    class NonArgumentSnippet : SnippetBase, ISnippet
    {


        public NonArgumentSnippet(string shortcut, string title, IEnumerable<string> texts)
        {
            Shortcut = shortcut;
            Title = title;
            _snippet = new Snippet ();
            foreach (var item in texts)
            {
                _snippet.Elements.Add(new SnippetTextElement() { Text = $"{item}\n" });
            }

        }


        public NonArgumentSnippet(string shortcut, string title, string text)
            :this(shortcut,title,text.Split('\n'))
        {
        }


        public override string Shortcut { get; protected set; }

        public override string Title { get; protected set;}

    }
}
