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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace BrainfuckIDE.Editor.Snippets
{
    /// <summary>
    /// The code completion window.
    /// </summary>
    public class CompletionWindow : CompletionWindowBase
    {
        readonly CompletionList _completionList = new CompletionList();
        protected ToolTip _toolTip = new ToolTip();


        public Brush ToolTipBackground { get => _toolTip.Background; set => _toolTip.Background = value; } 
        public Brush ToolTipForeground { get => _toolTip.Foreground; set => _toolTip.Foreground = value; }
        public double ToolTipOpacity { get => _toolTip.Opacity; set => _toolTip.Opacity = value; }


        /// <summary>
        /// Gets the completion list used in this completion window.
        /// </summary>
        public CompletionList CompletionList => _completionList;

        /// <summary>
        /// Creates a new code completion window.
        /// </summary>
        public CompletionWindow(TextArea textArea) : base(textArea)
        {
            // keep height automatic
            this.CloseAutomatically = true;
            this.SizeToContent = SizeToContent.Height;
            this.MaxHeight = 300;
            this.Width = 175;
            this.Content = _completionList;
            // prevent user from resizing window to 0x0
            this.MinHeight = 15;
            this.MinWidth = 30;

            _toolTip.PlacementTarget = this;
            _toolTip.Placement = PlacementMode.Right;
            _toolTip.Closed += ToolTip_Closed;
            this.CompletionList.SelectionChanged += CompletionList_SelectionChanged1;

            AttachEvents();

        }

        private void CompletionList_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (this.CompletionList.ListBox.HasItems)
            {
                this._toolTip.Visibility = 
                this.Visibility = Visibility.Visible;
                if (this.CompletionList.ListBox.SelectedIndex == -1)
                    this.CompletionList.ListBox.SelectedIndex = 0;
            }
            else
            {
                this._toolTip.Visibility =
                this.Visibility = Visibility.Hidden;
            }
        }

        #region ToolTip handling
        void ToolTip_Closed(object sender, RoutedEventArgs e)
        {
            // Clear content after tooltip is closed.
            // We cannot clear is immediately when setting IsOpen=false
            // because the tooltip uses an animation for closing.
            if (_toolTip != null)
                _toolTip.Content = null;
        }

        void CompletionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = _completionList.SelectedItem;
            if (item == null)
                return;
            object description = item.Description;
            if (description != null)
            {
                if (description is string descriptionText)
                {
                    _toolTip.Content = new TextBlock
                    {
                        Text = descriptionText,
                        TextWrapping = TextWrapping.Wrap
                    };
                }
                else
                {
                    _toolTip.Content = description;
                }
                _toolTip.IsOpen = true;
            }
            else
            {
                _toolTip.IsOpen = false;
            }
        }
        #endregion

        void CompletionList_InsertionRequested(object sender, EventArgs e)
        {
            Close();
            // The window must close before Complete() is called.
            // If the Complete callback pushes stacked input handlers, we don't want to pop those when the CC window closes.
            var item = _completionList.SelectedItem;
            if (item != null)
                item.Complete(this.TextArea, new AnchorSegment(this.TextArea.Document, this.StartOffset, this.EndOffset - this.StartOffset), e);
        }

        void AttachEvents()
        {
            this._completionList.InsertionRequested += CompletionList_InsertionRequested;
            this._completionList.SelectionChanged += CompletionList_SelectionChanged;
            this.TextArea.Caret.PositionChanged += CaretPositionChanged;
            this.TextArea.MouseWheel += TextArea_MouseWheel;
            this.TextArea.PreviewTextInput += TextArea_PreviewTextInput;
        }

        /// <inheritdoc/>
        protected override void DetachEvents()
        {
            this._completionList.InsertionRequested -= CompletionList_InsertionRequested;
            this._completionList.SelectionChanged -= CompletionList_SelectionChanged;
            this.TextArea.Caret.PositionChanged -= CaretPositionChanged;
            this.TextArea.MouseWheel -= TextArea_MouseWheel;
            this.TextArea.PreviewTextInput -= TextArea_PreviewTextInput;
            base.DetachEvents();
        }

        /// <inheritdoc/>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_toolTip != null)
            {
                _toolTip.IsOpen = false;
                _toolTip = null;
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                _completionList.HandleKey(e);
            }
        }

        void TextArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent,
                                       new TextCompositionEventArgs(e.Device, e.TextComposition));
        }

        void TextArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = RaiseEventPair(GetScrollEventTarget(),
                                       PreviewMouseWheelEvent, MouseWheelEvent,
                                       new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
        }

        UIElement GetScrollEventTarget()
        {
            if (_completionList == null)
                return this;
            return _completionList.ScrollViewer ?? _completionList.ListBox ?? (UIElement)_completionList;
        }

        /// <summary>
        /// Gets/Sets whether the completion window should close automatically.
        /// The default value is true.
        /// </summary>
        public bool CloseAutomatically { get; set; }

        /// <inheritdoc/>
        protected override bool CloseOnFocusLost
        {
            get { return this.CloseAutomatically; }
        }

        /// <summary>
        /// When this flag is set, code completion closes if the caret moves to the
        /// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
        /// but not in dot-completion.
        /// Has no effect if CloseAutomatically is false.
        /// </summary>
        public bool CloseWhenCaretAtBeginning { get; set; }

        void CaretPositionChanged(object sender, EventArgs e)
        {
            int offset = this.TextArea.Caret.Offset;
            if (offset == this.StartOffset)
            {
                if (CloseAutomatically && CloseWhenCaretAtBeginning)
                {
                    Close();
                }
                else
                {
                    _completionList.SelectItem(string.Empty);
                }
                return;
            }
            if (offset < this.StartOffset || offset > this.EndOffset)
            {
                if (CloseAutomatically)
                {
                    Close();
                }
            }
            else
            {
                TextDocument document = this.TextArea.Document;
                if (document != null)
                {
                    _completionList.SelectItem(document.GetText(this.StartOffset, offset - this.StartOffset));
                }
            }
        }
    }
}
