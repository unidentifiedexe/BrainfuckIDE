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
            get { return (bool)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // 依存関係プロパティ
        public static readonly DependencyProperty SourceProperty =
          DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(MemoryTokenView),
            new PropertyMetadata(false, new PropertyChangedCallback(OnSourceChanged))
          );

        // 依存関係プロパティに値がセットされたときに呼び出されるメソッド
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MemoryTokenView tb) tb.IsReadOnly = (bool)e.NewValue;
            // ……後述……
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
