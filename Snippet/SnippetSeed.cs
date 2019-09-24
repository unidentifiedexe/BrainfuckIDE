using Snippet.JsonLoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Snippet
{
    class SnippetSeed
    {

        private IEnumerable<string> _comment;

        public string Header { get;}

        public string Name { get; }


        private IEnumerable<string> _code;
        public SnippetSeed(SnippetInfo snippetInfo)
        {

        }

        private object CheckNode(IEnumerable<CodeNode> codeNodes)
        {
            var hoge = codeNodes.Select(p => new CodeNodeHelper(p)).ToArray();
            var headers = hoge.Select(p => p.Header).ToArray();

            foreach (var item in hoge)
                item.ParentUnioner(headers);

        }


        private class CodeNodeHelper
        {
            private string[] _parent;

            public string Header { get; }

            public IEnumerable<string> Parent => _parent;

            public CodeNode Innner { get; }

            public CodeNodeHelper(CodeNode innner)
            {
                Innner = innner;
                Header = innner.Header.Trim();
                _parent = GetParens(innner);
            }

            public void ParentUnioner(IEnumerable<string> filter)
            {
                _parent = _parent.Union(filter).ToArray();
            }
            private static string[] GetParens(CodeNode node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                if (node.Code == null) return Array.Empty<string>();

                Regex reg = new Regex(@"(?<={\s*).+?(?=\s*})");

                return reg.Matches(node.Code).OfType<Match>().Select(p => p.Value).Distinct().ToArray();
            }
        }

        
    }
}
