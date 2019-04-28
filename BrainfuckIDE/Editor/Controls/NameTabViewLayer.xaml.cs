using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace BrainfuckIDE.Editor.Controls
{
    /// <summary>
    /// NameTabViewLayer.xaml の相互作用ロジック
    /// </summary>
    public partial class NameTabViewLayer : UserControl
    {
        public NameTabViewLayer()
        {
            InitializeComponent();
        }
    }

    class SaveStateConverter : MarkupExtension, IValueConverter
    {
        private SaveStateConverter _converter;


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool)) throw new ArgumentException("Is not bool Type", nameof(value));
            var isSaved = (bool)value;
            return isSaved ? " " : "●";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ??= new SaveStateConverter();
        }

    }


    internal sealed class DesignDummyTabViewModel : INameTabViewModel
    {
        public bool IsSaved => false;

        public string FileName => "Dummy.cs";

        public string FilePath => "Hoge/Dummy.cs";

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
    }


    public interface INameTabViewModel : INotifyPropertyChanged
    {
        bool IsSaved { get; }
        string FileName { get; }
        string FilePath { get; }

    }
}
