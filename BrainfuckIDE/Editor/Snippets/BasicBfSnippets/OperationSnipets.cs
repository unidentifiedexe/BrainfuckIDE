using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
    static class OperationSnipets
    {
        public static IEnumerable<ISnippet> GetAllSnippets()
        {
            yield return Multiplication;
            yield return Division;
            yield return Mod;
            yield return Min;
            yield return Max;
            yield return GreaterThan;
            yield return Not;
        }

        /// <summary> 積算を行うスニペットを取得します </summary>
        public static ISnippet Multiplication { get; }
            = new NonArgumentSnippet("Multi", "Multiplication", "[->[->+>+<<]>[-<+>]<<]");


        /// <summary> 除算を行うスニペットを取得します </summary>
        public static ISnippet Division { get; }
           = new NonArgumentSnippet("Division", "Division", "[->->+<[>]>[[-<+>]>>>+<]<<<<]  >[-]>[-<+>]>>>[-<<<<<+>>>>>]<<<<<");



        /// <summary> 剰余算を行うスニペットを取得します </summary>
        public static ISnippet Mod { get; }
           = new NonArgumentSnippet("Mod", "Modulo", "[->->+<[>]>[[-<+>]>>>+<]<<<<]  >[-]>[-<+>]>>>[-<<<<<+>>>>>]<<<<<");


        public static ISnippet Min { get; }
          = new NonArgumentSnippet("Min", "Tow args Min", ">>>>+[<+<<<[>>>+<]>>[<]<[>>+<]>[<]>---[[+]<+>]+<[->-<]>>->+<<[-<<<->->>>+<]>]>-<<<<[-]<[-]");


        public static ISnippet Max { get; }
          = new NonArgumentSnippet("Max", "Tow args Max", ">>>>+[<+<<<[>>>+<]>>[<]<[>>+<]>[<]>---[[+]<+>]+<[->-<]>>->+<<[-<<<->->>>+<]>]>-[-<<<<<+>>>>>]<<<<[-<+>]<");


        public static ISnippet GreaterThan { get; }
          = new NonArgumentSnippet("Gt", "Greater than", "[[->]<[<]>]<<");


        public static ISnippet Not { get; }
          = new NonArgumentSnippet("Not", "Not", ">[-]+<[[-]>-<]>");

    }
}
