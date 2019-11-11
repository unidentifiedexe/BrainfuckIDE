using Snippets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
    class MultiByteVariable
    {
        public static IEnumerable<ISnippet> GetAllSnippets()
        {
            yield return Incriment;
            yield return Decriment;
            yield return ReverseSign;
            yield return Abs;
            yield return WriteSimply;
            yield return ReadSimply;
            yield return Init;
        }

        /// <summary> マルチバイト変数のインクリメントスニペットを取得します </summary>
        public static ISnippet Incriment { get; }
            = new NonArgumentSnippet("Multi_Incriment", "MultiByte Incriment", "[[-<+>[[>>]>]]<[->+<]<]<  [[<<]<]>>>[>>]<<");


        /// <summary> マルチバイト変数のデクリメントスニペットを取得します </summary>
        public static ISnippet Decriment { get; }
            = new NonArgumentSnippet("Multi_Decriment", "MultiByte Decriment", ">>-<< [>>+<<<[->+[>>]]>[-[-<+>]]<<]<  >>[>+[>>]]<<");


        /// <summary> マルチバイト変数の符号変換のスニペットを取得します </summary>
        public static ISnippet ReverseSign { get; }
            = new NonArgumentSnippet("Multi_ReverseSign", "MultiByte Reverse Sign", ReverseSignStrings());

        private static IEnumerable<string> ReverseSignStrings()
        {
            yield return "[<[->>+<<]<]>>";
            yield return "[-[-<+>]>+[-<+>]>]<<";
            yield return "[[-<+>[[>>]>]]<[->+<]<]< [[<<]<]>>>[>>]<<";
        }



        /// <summary> マルチバイト変数の絶対値取得のスニペットを取得します </summary>
        public static ISnippet Abs { get; }
            = new NonArgumentSnippet("Multi_Abs", "MultiByte Abs", AbsStrings());

        private static IEnumerable<string> AbsStrings()
        {
            yield return "[<<]>";
            yield return "[";
            yield return "  >[>>]<<";
            yield return "  [<[->>+<<]<]";
            yield return "  >>";
            yield return "  [ -[-<+>]>+[-<+>]>]<<";
            yield return "  [[-<+>[[>>]>]]<[->+<]<]<  [[<<]<]>>";
            yield return "  [->+<]";
            yield return "]";
            yield return ">[>>]<<";
        }



        /// <summary> マルチバイト変数の数値出力のスニペットを取得します </summary>
        public static ISnippet WriteSimply { get; }
            = new NonArgumentSnippet("Multi_WriteSimply", "MultiByte WriteSimply", WriteSimplyStrings());

        private static IEnumerable<string> WriteSimplyStrings()
        {
            yield return "<+>[<<]>";
            yield return ">[<+>>[<<-<[<<]<]>]";
            yield return ">[->>]";
            yield return ">>>[<+>>>]";
            yield return "++++++++++[-<+++++>]<---";
            yield return "<<->>";
            yield return "[[+<<]>>[>>]<+<--]";
            yield return "<<[<<]>>[.>>]";
            yield return ">+[-<<<[<<]>>[->>]>]";
        }


        /// <summary> マルチバイト変数の読取のスニペットを取得します </summary>
        public static ISnippet ReadSimply { get; }
            = new NonArgumentSnippet("Multi_ReadSimply", "MultiByte ReadSimply", ReadSimplyStrings());

        private static IEnumerable<string> ReadSimplyStrings()
        {
            yield return ">>";
            yield return ">+> >+> >+> >+> >+> //桁数分移動(桁識別用の1を加算)";
            yield return ">+  //一時入力に移動";
            yield return "[-<";
            yield return "  ,----------";
            yield return "  [---------- ---------- --";
            yield return "    [-------- --------";
            yield return "      +//入力識別用に1足す";
            yield return "      [<<]//入力なされた最大の桁まで移動";
            yield return "      >>[[-<<+>>]>>]<<";
            yield return "      >+<";
            yield return "    ]";
            yield return "  ]";
            yield return "  >";
            yield return "]";
            yield return "<<";
            yield return "";
            yield return "[<[-<<]<] // 入力識別用の1を減算";
            yield return ">>[-<+>>>] //桁識別を数値部に移動";
            yield return "<<<";
            yield return "[- //識別用の数値を下ろす";
            yield return "  [-<+>]";
            yield return "  >++++++++++<<";
            yield return "  [->+>-<<]";
            yield return "  <";
            yield return "]";
            yield return ">>>[>>]<< 末尾に移動";
        }



        /// <summary> マルチバイト変数の初期化のスニペットを取得します </summary>
        public static ISnippet Init { get; }
            = new NonArgumentSnippet("Multi_Init", "MultiByte Init", InitStrings());

        private static IEnumerable<string> InitStrings()
        {
            yield return ">>";
            yield return ">++++++++++>";
            yield return ">++++++++++>";
            yield return ">++++++++++>";
            yield return ">++++++++++>";
            yield return ">++++++++++>";
            yield return ">>>";
        }
    }
}
