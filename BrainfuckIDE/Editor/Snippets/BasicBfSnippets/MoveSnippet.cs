#nullable enable

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BrainfuckIDE.Editor.Snippets.BasicBfSnippets.SnippetUtils;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
    class MoveSnippet : ISnippet
    {

        static private MoveSnippet? _instance;

        static public ISnippet Instance => _instance ??= new MoveSnippet();


        private readonly Snippet _snippet;

        private MoveSnippet()
        {
            var pos = new SnippetReplaceableTextElement { Text = ">" };
            _snippet = new Snippet
            {
                Elements =
                {
                     new SnippetTextElement(){  Text = "[-"},
                    pos,
                     new SnippetTextElement(){  Text = "+"},
                     new PointerMoveSnippet.PointerMoveSnipetGenerator(){ TargetElement = pos } ,
                     new SnippetTextElement(){  Text = "]"},

                }
            };
        }

        public string Shortcut => "Move";

        public string Title => "Data move";


        public void Insert(TextArea textArea)
        {
            _snippet.Insert(textArea);
        }
        //class MoveSnipetGenerator :SnippetBoundElement
        //{

        //    public override string ConvertText(string input)
        //    {
        //        if (!int.TryParse(input, out var num))
        //            return string.Empty;
        //        else
        //            return $"[-{GetPointerMoveStr(num)}+{GetPointerMoveStr(-num)}]"; 
        //    }
        //}

    }
}
