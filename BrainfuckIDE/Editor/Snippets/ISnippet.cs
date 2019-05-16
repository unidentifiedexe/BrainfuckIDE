using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets
{
    /// <summary>
    /// スニペット挿入に必要な情報の提供をサポートします
    /// </summary>
    interface ISnippet
    {
        string Shortcut { get; }

        string Title { get; }

        void Insert(TextArea textArea);
    }
}
