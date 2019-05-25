#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
    abstract class SnippetBase : ISnippet
    {
        public abstract string Shortcut { get; protected set; }

        public abstract string Title { get; protected set; }

        protected Snippet? _snippet = null;

        public virtual void Insert(TextArea textArea)
        {
            _snippet?.Insert(textArea);
        }
    }
}
