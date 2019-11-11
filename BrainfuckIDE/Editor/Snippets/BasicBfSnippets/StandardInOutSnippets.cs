using Snippets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
    static class StandardInOutSnippets
    {
        public static IEnumerable<ISnippet> GetAllSnippets()
        {
            yield return ReadAll;
            yield return ReadLine;
            yield return ReadNumsOnLine;
            yield return WriteNumber;
            yield return SkipLine;
        }


        public static ISnippet SkipLine { get; }
          = new NonArgumentSnippet("SkipLine", "Read line", ",[,----------]");

        public static ISnippet ReadAll { get; }
          = new NonArgumentSnippet("ReadAll", "Read all", ",+[->,+]");


        public static ISnippet ReadLine { get; }
          = new NonArgumentSnippet("ReadLine", "Read line", ",----------[++++++++++,----------]");


        public static ISnippet ReadNumsOnLine { get; }
          = new NonArgumentSnippet("ReadNums", "Read numbers on line", GetReadNumsOnLineStrs());


        private static IEnumerable<string> GetReadNumsOnLineStrs()
        {
            yield return ">>+";
            yield return "[-<,----- -----[ >++++[-<----->]<-- ";
            yield return "  [>++++[-<---->]<<[->>+<<]>>[-<<++++++++++>>]<[-<+>]>+<]";
            yield return "  +>[-<->]< [- /* */ > ]>+<";
            yield return "]>]<";
        }


        private static ISnippet WriteNumber { get; }
          = new NonArgumentSnippet("WriteNumber ", "Write number", GetWriteNumberStrs());


        private static IEnumerable<string> GetWriteNumberStrs()
        {
            yield return "[[->+<]>[>++++++++++<[->-[>+>>]>[+[-<+>]>+>>]<<<<<]>[-]>+[-<<+>>]>[-<<+>>]<<]<[<]>-<]";
            yield return ">+[>]>++++++[-<-------->]<+[[<]>[+>]<]<[.[-]<]";
        }
    }
}
