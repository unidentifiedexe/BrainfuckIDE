using ICSharpCode.AvalonEdit.Snippets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
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
