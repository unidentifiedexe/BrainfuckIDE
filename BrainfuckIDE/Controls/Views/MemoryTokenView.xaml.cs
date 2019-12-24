using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrainfuckIDE.Controls.Views
{
    /// <summary>
    /// MemoryTokenView.xaml の相互作用ロジック
    /// </summary>
    public partial class MemoryTokenView : UserControl
    {

        public static Brush NomalBackgroundBrush { get; } = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        public static Brush SelectedBackgroundBrush { get; } = new SolidColorBrush(Color.FromArgb(0xff, 0x20, 0x60, 0xC0));//FF275FBA

        public MemoryTokenView()
        {
            InitializeComponent();
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // 依存関係プロパティ
        public static readonly DependencyProperty IsReadOnlyProperty =
          DependencyProperty.Register(
            nameof(IsReadOnly), typeof(bool), typeof(MemoryTokenView),
            new PropertyMetadata(false/*, new PropertyChangedCallback(OnIsReadOnlyChanged)*/)
          );

        // 依存関係プロパティに値がセットされたときに呼び出されるメソッド
        //private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is MemoryTokenView tb) tb.IsReadOnly = (bool)e.NewValue;
        //}




        public bool IsEdditing
        {
            get { return (bool)GetValue(IsEdditingProperty); }
            private set { SetValue(IsEdditingPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsEdditingPropertyKey = DependencyProperty.RegisterReadOnly
            (nameof(IsEdditing), typeof(bool), typeof(MemoryTokenView), new PropertyMetadata(false, OnIsEdditingChnaged));
        
        // Using a DependencyProperty as the backing store for IsEdditing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEdditingProperty = IsEdditingPropertyKey.DependencyProperty;
            //DependencyProperty.Register(nameof(IsEdditing), typeof(bool), typeof(MemoryTokenView), new PropertyMetadata(false, OnIsEdditingChnaged));

        private static void OnIsEdditingChnaged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MemoryTokenView owner)) return;
            if (!(e.NewValue is bool val)) return;
            owner.SetMainTextVisible(val);
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IsEdditing = true;
        }

        private void SetMainTextVisible(bool isEditing)
        {
            _mainTextBlock.Visibility = BoolToVisibality(!isEditing);
            _mainTextBox.Visibility = BoolToVisibality(isEditing);
        }
        private static Visibility BoolToVisibality(bool boolean)
        {
            return boolean ? Visibility.Visible : Visibility.Hidden;
        }
        private void OnTextUpdated()
        {
            IsEdditing = false;
        }

        private void MainTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox textBox) || textBox != _mainTextBox) return;
            var be = textBox.GetBindingExpression(TextBox.TextProperty);
            if (e.Key == Key.Enter)
            {
                be?.UpdateSource();
                OnTextUpdated();
            }
            else if (e.Key == Key.Escape)
            {
                be?.UpdateTarget();
                OnTextUpdated();
            }
        }

        private void _mainTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox) || textBox != _mainTextBox) return;
            var be = textBox.GetBindingExpression(TextBox.TextProperty);
            be?.UpdateSource();
            OnTextUpdated();
        }
    }

    public class BackGroundBrushConverter : MarkupExtension, IValueConverter
    {
        private BackGroundBrushConverter _converter;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool)) throw new ArgumentException("Is not bool type.", nameof(value));

            return ((bool)value) ? MemoryTokenView.SelectedBackgroundBrush : MemoryTokenView.NomalBackgroundBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ??= new BackGroundBrushConverter();
        }

    }

}
