using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets
{
    class BasicCompletionData : ICompletionData
    {

        public System.Windows.Media.ImageSource Image => null;
        public string Text { get; private set; }

        // 以下のプロパティはリスト項目としてテキスト以外を設定する場合に使うことになる
        public object Content { get { return this.Text; } }
        public object Description { get { return "Description for " + this.Text; } }

        public double Priority => 0;

        public BasicCompletionData(string text)
        {
            this.Text = text;
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
