using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
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

            var byteVal = (byte)value;
            var letter = Helper.TryGetHeaderStringTrimed(byteVal, out var txt) ? txt : $"'{(char)byteVal}'";
            var showstring = $"0x{byteVal.ToString("x2")} | {letter}";

            return showstring;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ??= new ByteToCharStringConverter();
        }

        static class Helper
        {
            static private Dictionary<byte, string> _dic = new Dictionary<byte, string>()
            {
                [0] = "NUL（null文字）",
                [1] = "SOH（ヘッダ開始）",
                [2] = "STX（テキスト開始）",
                [3] = "ETX（テキスト終了）",
                [4] = "EOT（転送終了）",
                [5] = "ENQ（照会）",
                [6] = "ACK（受信確認）",
                [7] = "BEL（警告）",
                [8] = "BS（後退）",
                [9] = "HT（水平タブ）",
                [10] = "LF（改行）",
                [11] = "VT（垂直タブ）",
                [12] = "FF（改頁）",
                [13] = "CR（復帰）",
                [14] = "SO（シフトアウト）",
                [15] = "SI（シフトイン）",
                [16] = "DLE（データリンクエスケー プ）",
                [17] = "DC1（装置制御１）",
                [18] = "DC2（装置制御２）",
                [19] = "DC3（装置制御３）",
                [20] = "DC4（装置制御４）",
                [21] = "NAK（受信失敗）",
                [22] = "SYN（同期）",
                [23] = "ETB（転送ブロック終了）",
                [24] = "CAN（キャンセル）",
                [25] = "EM（メディア終了）",
                [26] = "SUB（置換）",
                [27] = "ESC（エスケープ）",
                [28] = "FS（フォーム区切り）",
                [29] = "GS（グループ区切り）",
                [30] = "RS（レコード区切り）",
                [31] = "US（ユニット区切り）",
                [127] = "DEL（削除）",
            };


            public static bool TryGetHeaderString(byte key, out string data)
            {
                return _dic.TryGetValue(key, out data);
            }


            public static bool TryGetHeaderStringTrimed(byte key, out string data)
            {
                var ret = _dic.TryGetValue(key, out data);
                if (ret)
                    data = Regex.Replace(data, @"(（|\().*?(）|\))", "");
                return ret;
            }
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

        private static bool TryParseHexNumber(string str, out byte res)
        {
            res = 0;
            if (str.Length < 3) return false;
            var match = Regex.Match(str, "(?<=^0?x).*");
            if (!match.Success) return false;
            var temp = match.Value;
            return byte.TryParse(temp, System.Globalization.NumberStyles.AllowHexSpecifier, null, out res);
        }
    }

    //値の一致を表示状態に変換するコンバータ  
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class EqualsToVisibleConveter : MarkupExtension, IValueConverter
    {

        private EqualsToVisibleConveter _converter;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return parameter == null ? Visibility.Visible : Visibility.Collapsed;
            }
            return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        //使わないよ  
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ??= new EqualsToVisibleConveter();
        }
    }
}
