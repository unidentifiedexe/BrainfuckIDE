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

        static public MoveSnippet Instance => _instance ??= new MoveSnippet();


        private readonly Snippet _snippet;

        private MoveSnippet()
        {
            var pos = new SnippetReplaceableTextElement { Text = "1" };
            _snippet = new Snippet
            {
                Elements =
                {
                     new SnippetTextElement { Text = "//Move to " },
                     pos,
                     new SnippetTextElement { Text = "\n" },
                     new MoveSnipetGenerator{ TargetElement = pos},

                }
            };
        }

        public string Shortcut => "Move";

        public string Title => "Data move";

        public void Insert(TextArea textArea)
        {
            _snippet.Insert(textArea);
        }

        class MoveSnipetGenerator :SnippetBoundElement
        {

            public override string ConvertText(string input)
            {
                if (!int.TryParse(input, out var num))
                    return string.Empty;
                else
                    return $"[-{GetPointerMoveStr(num)}+{GetPointerMoveStr(-num)}]"; 
            }
        }
    }
}
