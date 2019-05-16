using BrainfuckIDE.Editor.Snippets.BasicBfSnippets;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BrainfuckIDE.Editor.Snippets
{
    static class CompletionWindowCreator
    {
        public static CompletionWindow Creato(TextArea textArea)
        {
            var snippets = new[]{
                new [] { MoveSnippet.Instance, PointerMoveSnippet.Instance },
                OperationSnipets.GetAllSnippets(),
                StandardInOutSnippets.GetAllSnippets(),
                 ArrayOperetorSnippets.GetAllSnippets(),
            }.SelectMany(p => p);

            return Creato(textArea, snippets);
        }


        public static CompletionWindow Creato( TextArea textArea, IEnumerable<ISnippet> snippets)
        {

            var window = new CompletionWindow(textArea);
            window.WindowStyle = WindowStyle.None;
            window.AllowsTransparency = true;
            window.BorderThickness = new Thickness(0);
            var border = (textArea.Parent as FrameworkElement).Parent as Border;
            window.CompletionList.Background = border.Background;
            window.CompletionList.Foreground = textArea.Foreground;
            window.CompletionList.ListBox.Background = border.Background;
            window.CompletionList.ListBox.Foreground = textArea.Foreground;


            IList<ICompletionData> data = window.CompletionList.CompletionData;
            foreach (var item in snippets)
                data.Add(new SnippetCompletionData (item));

            return window;
        }



        private class SnippetCompletionData : ICompletionData
        {
            public ImageSource Image => null;

            public string Text => _snippet.Shortcut;


            private ISnippet _snippet;

            public object Content => _snippet.Shortcut;

            public object Description => _snippet.Title;

            public double Priority => 0;


            public SnippetCompletionData(ISnippet content)
            {
                _snippet = content;
            }

            public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Remove(completionSegment);
                _snippet.Insert(textArea);

            }
        }
    }
}
