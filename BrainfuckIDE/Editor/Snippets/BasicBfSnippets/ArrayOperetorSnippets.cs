using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
    static class ArrayOperetorSnippets
    {

        public static IEnumerable<ISnippet> GetAllSnippets()
        {
            yield return OrderBy;
            yield return OrderByDescending;
        }


        private static ISnippet OrderBy { get; }
          = new NonArgumentSnippet("Arr.OrderBy ", "Order By", GetOrderByStrs());


        private static IEnumerable<string> GetOrderByStrs()
        {
            yield return "//** Sort";
            yield return "<[";
            yield return "  [";
            yield return "    [->>>+>>+<< <<<]";
            yield return "    >[-<+>>+<]";
            yield return "    >[[->]<[<]>]<<";
            yield return "    [-<<->> >>>+<<<]";
            yield return "    <<<";
            yield return "  ]";
            yield return "  >[-<+>]";
            yield return "  >>>>> [ [-<<<<+>>>>]>]<< <<<<";
            yield return "]";
            yield return "<";
            yield return "[ [->+<]<]";
            yield return ">>[>]<";
            yield return "//**";
        }


        private static ISnippet OrderByDescending { get; }
          = new NonArgumentSnippet("Arr.OrderByDescending", "Order ByDescending", GetOrderByDisStrs());


        private static IEnumerable<string> GetOrderByDisStrs()
        {

            yield return "//** Sort";
            yield return "<[";
            yield return "  [";
            yield return "    [->>+>>>+<< <<<]";
            yield return "    >[-<+>>>+<<]";
            yield return "    >[[->]<[<]>]<<";
            yield return "    [-<<+>> >>>-<<<]";
            yield return "    <<<";
            yield return "  ]";
            yield return "  >[-<+>]";
            yield return "  >>>>> [ [-<<<<+>>>>]>]<< <<<<";
            yield return "]";
            yield return "<";
            yield return "[ [->+<]<]";
            yield return ">>[>]<";
            yield return "//**";
        }

        

    }
}
