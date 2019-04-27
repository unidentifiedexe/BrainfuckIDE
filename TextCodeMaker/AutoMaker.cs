using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodeMaker
{
    class AutoMaker
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
            var charArray = str.ToCharArray();
            return new string(Enumerable.Repeat(charArray, count).SelectMany(p => p).ToArray());
        }
        static private string RepeatString(char letter, int count)
        {
            return new string(Enumerable.Repeat(letter, count).ToArray());
        }
    }
}
