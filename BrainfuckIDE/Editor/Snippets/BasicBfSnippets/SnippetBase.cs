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

    public class RepeatStringGenerator : SnippetBoundElement
    {
        private readonly string _statString;
        private readonly string _seedStr;
        private readonly string _endString;

        public RepeatStringGenerator(string seedString)
            : this(string.Empty, seedString, string.Empty)
        {
            _seedStr = seedString;
        }
        public RepeatStringGenerator(string startString, string seedString, string endString)
        {
            _statString = startString;
            _seedStr = seedString;
            _endString = endString;
        }

        public override string ConvertText(string input)
        {
            if (!int.TryParse(input, out var num) || num < 0)
                return "error";
            if (num == 0) return string.Empty;
            return string.Join("", Enumerable.Repeat(_seedStr, num).Prepend(_statString).Append(_endString));
        }
    }
}
