#nullable enable
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
    class PointerMoveSnippet : ISnippet
    {

        static private PointerMoveSnippet? _instance;
        private readonly Snippet _snippet;

        static public ISnippet Instance => _instance ??= new PointerMoveSnippet();



        public PointerMoveSnippet()
        {
            var pos = new SnippetReplaceableTextElement { Text = ">" };
            _snippet = new Snippet
            {
                Elements =
                {
                     pos,
                     new SnippetTextElement(){  Text = " "},
                     new PointerMoveSnipetGenerator(){ TargetElement = pos } ,
                }
            };
        }



        public string Shortcut => "mp";

        public string Title => "Pointer move";

        public virtual void Insert(TextArea textArea)
        {
            _snippet?.Insert(textArea);
        }
        public class PointerMoveSnipetGenerator : SnippetBoundElement
        {

            public override string ConvertText(string input)
            {
                var rev =
                    input.Reverse().Select(p => p switch
                    {
                        '>' => '<',
                        '<' => '>',
                        _ => ' ',
                    }).Where(p => p != ' ');

                var ret = new string(rev.ToArray());
                ret = Refacter.ToSimpleCode.Get(ret,p=>p);
                return ret;
            }
        }
    }
}
