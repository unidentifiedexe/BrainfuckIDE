#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using Snippets;

namespace BrainfuckIDE.Editor.Snippets.SyntaxSnippets
{
    class RegionSnippet : ISnippet
    {

        static private RegionSnippet? _instance;
        private readonly Snippet _snippet;

        static public ISnippet Instance => _instance ??= new RegionSnippet();



        public RegionSnippet()
        {
            var text = new SnippetReplaceableTextElement { Text = "MyRegion" };
            _snippet = new Snippet
            {
                Elements =
                {
                     new SnippetTextElement(){  Text = "#region "},
                     text ,
                     new SnippetTextElement(){  Text = "\n#endregion"},
                }
            };
        }



        public string Shortcut => "region";

        public string Title => "region";

        public virtual void Insert(TextArea textArea)
        {
            _snippet?.Insert(textArea);
        }
    }
}
