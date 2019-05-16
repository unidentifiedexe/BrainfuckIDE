using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
    static class SnippetUtils
    {
        public static string GetPointerMoveStr(int num)
        {
            if (num == 0)
                return string.Empty;
            else if (num < 0)
                return CharRepeater('<', -num);
            else
                return CharRepeater('>', num);
        }


        public static string CharRepeater(char letter, int num)
        {
            return string.Join("", Enumerable.Repeat(letter, num));
        }



    }
}
