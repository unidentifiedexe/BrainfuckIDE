using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;

namespace BrainfuckIDE.Editor.Snippets
{
    class TestSnippet : ISnippet
    {
        readonly Snippet _snippet;

        public TestSnippet()
        {
            var pos = new SnippetReplaceableTextElement { Text = "1" };
            _snippet = new Snippet
            {
                Elements =
                {
                     new SnippetTextElement { Text = "//Copy to " },
                     pos,
                     new SnippetTextElement { Text = "\n" },
                     new SnippetTextElement { Text = "[->+<]>[-<+>" },
                     new SnippetRepeatBoundElement{ RepeatText = ">", TargetElement = pos},
                     new SnippetTextElement { Text = "+" },
                     new SnippetRepeatBoundElement{ RepeatText = "<", TargetElement = pos},
                     new SnippetTextElement { Text = "]" },

                }
            };
        }

        public string Shortcut => "for";

        public string Title => "for";

        public void Insert(TextArea textArea)
        {
            _snippet.Insert(textArea);
        }
    }
}
