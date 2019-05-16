#nullable enable

using ICSharpCode.AvalonEdit.Snippets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets
{
    class SnippetRepeatBoundElement : SnippetBoundElement
    {
        public string RepeatText { get; set; } = string.Empty;


        public override string ConvertText(string input)
        {
            if (!int.TryParse(input, out var num))
                return string.Empty;

            num = NumberConverter?.Invoke(num) ?? num;
            return string.Join("", Enumerable.Repeat(RepeatText, num));
        }

        public Func<int, int>? NumberConverter { get; set; }
    }
}
