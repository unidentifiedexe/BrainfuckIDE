using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refacter
{
    public class CodeConverter
    {
        /// <summary> ポインタの向きを逆にした文字列に変換します </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PointerInverse(string str)
        {
            var array = str.ToCharArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] switch
                {
                    '>' => '<',
                    '<' => '>',
                    char c => c,
                };
            }

            return new string(array);
        }

        /// <summary>
        /// 不要な文字を削除したコードを返します
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSimplyCode(string str)
            => ToSimpleCode.Get(str, EffectiveCharacters.RemoveNonEffectiveChars);
    }
}
