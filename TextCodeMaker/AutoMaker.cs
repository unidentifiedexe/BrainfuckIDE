using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsUtility.Primitive;
using CsUtility.Enumerable;
namespace TextCodeMaker
{
    public static class AutoMaker
    {
        /// <summary>
        /// 末尾に\n\0を追加して、その文字の文字コードをメモリに格納するコードを生成する
        /// </summary>
        /// <returns></returns>
        static public string AutoMake(string str)
        {
            string shortestStr = null;
            for (int i = 5; i <= 30; i++)
            {
                var tempStr = AutoMake(str, i);
                Console.WriteLine($"[{i},{tempStr.Length}]");
                if (shortestStr == null || tempStr.Length < shortestStr.Length)
                    shortestStr = tempStr;
            }
            return shortestStr;
        }
        /// <summary>
        /// 末尾に\n\0を追加して、その文字の文字コードをメモリに格納するコードを生成する
        /// </summary>
        /// <returns></returns>
        static public string AutoMake(string str, int splitLength)
        {
            if(str.Any( p=>p> 0xff)) throw new Exception("実行不可能な文字が検出されました");

            var letters = (str + '\n').Select(p => (byte)p).ToArray();
            var retStr = "";
            retStr += RepeatString('>',letters.Length);//\0が入力されるポインタに移動
            retStr += RepeatString('+', splitLength); ; //\0が代入されるポインタに"splitLength"を代入
            retStr += "[";
            retStr += RepeatString('<', letters.Length);
            retStr += string.Join("", letters.Select(p => RepeatString('+', p / splitLength) + ">").ToArray());
            retStr += "-]";//元となるsplitLengthの倍数を入力する;
            retStr += string.Join("", letters.Reverse().SelectMany(p => "<" + RepeatString('+', p % splitLength)).ToArray());
            retStr += "[>]";//\0が代入されているポインタに移動
            return retStr;
        }

        static private string RepeatString(string str, int count)
        {
            return new string(Enumerable.Repeat(str, count).SelectMany(p => p).ToArray());
        }
        static private string RepeatString(char letter, int count)
        {
            return new string(Enumerable.Repeat(letter, count).ToArray());
        }

        static public string GetSimplyTextOutputer(string text)
        {
            const int baseDivider = 10;

            if (text.IsNullOrEmpty()) return string.Empty;
            var baseChars =
                text.AsEnumerable()
                .ZipVicinity((p, n) => (New: n, Deff: Math.Abs(n - p)))
                .Where(p => p.Deff >= 20).Select(p=>p.New)
                .Append(text[0])
                .Select(p => (p + (baseDivider / 2)) / baseDivider * baseDivider).ToArray();

            var retStrs = $"++++++++++[->"
                + string.Join(">", baseChars.Select(p => RepeatString('+', p/10)))
                + RepeatString('<', baseChars.Length)
                + "]>";

            IEnumerable<string> Itr()
            {
                var pos = 0;
                foreach (var item in text)
                {
                    int newPos = GetMinimusIndexes(baseChars, p => Math.Abs(p - item));
                    if (newPos > pos) yield return RepeatString('>', newPos - pos);
                    else if (newPos < pos) yield return RepeatString('<', pos - newPos);
                    pos = newPos;

                    if (item > baseChars[pos]) yield return RepeatString('+', item - baseChars[pos]);
                    else if (item < baseChars[pos]) yield return RepeatString('-', baseChars[pos] - item);
                    baseChars[pos] = item;

                    yield return ".";
                }
                yield return "[>]";
            }


            return retStrs + string.Join(string.Empty, Itr());

        }

        private static int GetMinimusIndexes<T>(IEnumerable<T> source, Func<T, int> resSelectrt)
        {
            return source.Select((p, i) => (I: i, V: p)).MinItem(p => resSelectrt(p.V)).I;
        }

    }
}
