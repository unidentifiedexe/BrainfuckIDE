using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace BrainfuckIDE.Controls
{

    class ByteToCharStringConverter : MarkupExtension, IValueConverter
    {
        private ByteToCharStringConverter _converter;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is byte)) throw new ArgumentException("Is not byte type.", nameof(value));

            return $"'{(char)(byte)value}'";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ??= new ByteToCharStringConverter();
        }

    }

    class ByteStringInteractiveConverter : MarkupExtension, IValueConverter
    {
        private ByteStringInteractiveConverter _converter;


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is byte)) throw new ArgumentException("byte型じゃないよ。", nameof(value));

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = (value as string) ?? throw new ArgumentException("string型じゃないよ。", nameof(value));

            if (byte.TryParse(str, out var val))
                return val;
            else if (TryParseHexNumber(str, out var hexVal))
                return hexVal;
            else if (str.Length == 1)
                return (byte)(str[0]);
            else if (str.Length == 3 && str[0] == str[2] && str[0] == '\'')
                return (byte)(str[1]);
            else
                return new object();
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ??= new ByteStringInteractiveConverter();
        }

        private static bool TryParseHexNumber(string str,out byte res)
        {
            res = 0;
            if (str.Length < 3) return false;
            if (str[0] != '0' || str[1] != 'x') return false;
            var temp = str.Remove(0, 2);
            return byte.TryParse(temp, System.Globalization.NumberStyles.AllowHexSpecifier, null, out res);
        }
    }

}
