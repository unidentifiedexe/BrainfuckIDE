using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.JsonLoder
{
    //    public class SnippetInfo
    //    {
    //        public MlutiLineString Comment { get; set; }

    //        public string Header { get; set; }

    //        public string Name { get; set; }


    //        public CodeNode[] Nodes { get; set; }


    //        public MlutiLineString Code { get; set; }

    //    }

    //    /// <summary> 指定回数を繰り返すコードの生成の情報を格納します </summary>
    //    public class IntRepeater : CodeNode
    //    {

    //    }

    //    public class CodeNode
    //    {
    //        public string Header { get; set; }

    //        public string Code { get; set; }

    //    }

    public class MlutiLineString
    {
        public string[] InnnerArray { get; set; }

        public static explicit operator MlutiLineString(string[] vs)
        {
            return new MlutiLineString() { InnnerArray = (string[])vs.Clone() };
        }

        public static explicit operator MlutiLineString(string text)
        {
            return new MlutiLineString() { InnnerArray = SplitByNewLine(text) };
        }

        private static string[] SplitByNewLine(string str)
        {
            return str.Replace("\r\n", "\n").Split('\r', '\n');
        }
    }
}
