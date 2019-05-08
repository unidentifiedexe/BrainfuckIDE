using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsUtility.Enumerable;

namespace Refacter
{
    public class ToSimpleCode
    {
        static public string Get(string sourceCode , Func<string,IEnumerable<char>> removeNonEfectivChar)
        {

            var codes = removeNonEfectivChar(sourceCode).SplitIf((p, n) => (p != n)).Select(p => p.ToArray());

            return new string(GetMarged(codes).SelectMany().ToArray());
        }


        abstract class CodeSeed
        {
            public static CodeSeed GetSeed(char[] data)
            {
                var header = data[0];
                switch (header)
                {
                    case '<':
                    case '>':
                    case '+':
                    case '-': return new CanMargeCodeSeed(data);
                    case '[':
                    case ']':
                    case ',':
                    case '.': return new DefaultCodeSeed(data);
                    default:
                        return null;
                }
            }


            public abstract CodeType Type { get; }
            public abstract bool HasCode { get; }
            public abstract IEnumerable<char> ToCode();

            public bool TryMarge(CodeSeed b, out CodeSeed ret)
            {
                ret = null;
                if (!(this is CanMargeCodeSeed am))
                    return false;
                if (!(b is CanMargeCodeSeed bm))
                    return false;
                if (am.Type != bm.Type)
                    return false;

                ret = CanMargeCodeSeed.Marge(am, bm);
                return true;
            }

            private class DefaultCodeSeed : CodeSeed
            {
                public override CodeType Type => CodeType.None;

                public override bool HasCode => _code.Length != 0;

                private string _code;

                public DefaultCodeSeed(char[] code)
                {
                    _code = new string(code);
                }
                public override IEnumerable<char> ToCode()
                {
                    return _code;
                }
            }

            private class CanMargeCodeSeed : CodeSeed
            {
                char _header;
                int _length;


                public override CodeType Type { get; }

                public override bool HasCode => _length != 0;

                public CanMargeCodeSeed(char[] code)
                {
                    _length = code.Length;
                    _header = code[0];

                    Type = _header switch
                    {
                        '<' => CodeType.MovePointerSeed,
                        '>' => CodeType.MovePointerSeed,
                        '+' => CodeType.ValueChangeSeed,
                        '-' => CodeType.ValueChangeSeed,
                        _ => CodeType.None,
                    };
                }
                private CanMargeCodeSeed(CanMargeCodeSeed baseSeed, int length)
                {
                    Type = baseSeed.Type;
                    _header = baseSeed._header;
                    _length = length;
                }

                static public CanMargeCodeSeed Marge(CanMargeCodeSeed a, CanMargeCodeSeed b)
                {
                    if (a.Type != b.Type) throw new ArgumentNullException();

                    if (a._header == b._header) return new CanMargeCodeSeed(a, a._length + b._length);
                    else if (a._length > b._length) return new CanMargeCodeSeed(a, a._length - b._length);
                    else return new CanMargeCodeSeed(b, b._length - a._length);
                }

                public override IEnumerable<char> ToCode()
                {
                    return Enumerable.Repeat(_header, _length);
                }
            }
        }
        static IEnumerable<IEnumerable<char>> GetMarged(IEnumerable<char[]> source)
        {
            CodeSeed prev = null;

            foreach (var item in source)
            {
                var current = CodeSeed.GetSeed(item);
                if (current == null) continue;
                if (prev == null)
                {
                    prev = current;
                }
                else if (prev != null)
                {
                    if (prev.TryMarge(current, out var marged))
                        prev = marged;
                    else
                    {
                        yield return prev.ToCode();
                        prev = current;
                    }

                }

            }
            yield return prev.ToCode();
        }


        public enum CodeType
        {

            None = 0,
            MovePointerSeed = 1,
            ValueChangeSeed = 2,
            InputSeed = 3,
            OutputSeed = 4,
            LoopSeed = 5,
        }
    }

}
