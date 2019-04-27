using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckInterpreter
{
   public class EffectiveCharacters
    {

        static public IEnumerable<char> Characters => _nomalBrainfackCharctors;

        static private readonly string _nomalBrainfackCharctors = "+-><.,[]";

        /// <summary>
        /// ソースコードの不要な文字を削除したものを返す
        /// </summary>
        /// <param name="code"></param>
        /// <param name="additivEfectivechars"></param>
        /// <returns></returns>
        static string RemoveNonEffectiveChars(string code ,params char[] additivEfectivechars)
        {

            var effectivChars = Characters.Concat(additivEfectivechars).Distinct().ToList();

            var ret = code.Where(p => effectivChars.Contains(p)).ToArray();

            return new string(ret);
        }

    }
}
