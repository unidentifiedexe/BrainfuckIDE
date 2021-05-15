using CsUtility.Enumerable;
using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refacter
{
    public class ToCLang
    {

        public static string ToCLangCode(string str)
            => string.Join(Environment.NewLine, Get(str, EffectiveCharacters.RemoveNonEffectiveChars));
        static  IEnumerable<string> Get(string sourceCode, Func<string, IEnumerable<char>> removeNonEfectivChar)
        {

            var codes = ToSimpleCode.Get(sourceCode, removeNonEfectivChar).SplitIf((p, n) => (p != n)).Select(p => p.ToArray());
            return ToCLangHelper(ToSimpleCode.GetMarged(codes));
        }

        private static IEnumerable<string> ToCLangHelper(IEnumerable<ToSimpleCode.CodeSeed> codeSeeds)
        {
            foreach(var item in Header())
            {
                yield return item;
            }
            var codeLines = codeSeeds.SelectMany(GetClangCode);
            foreach (var line in codeLines)
            {
                yield return line;
            }

            foreach (var item in Fotter())
            {
                yield return item;
            }
        }

        private static IEnumerable<string> GetClangCode(ToSimpleCode.CodeSeed seed)
        {
            if (seed == null || seed.HasCode == false)
                yield break;

            switch (seed.Type)
            {
                case ToSimpleCode.CodeType.None:
                    yield break;
                case ToSimpleCode.CodeType.MovePointerSeed:
                    {
                        var code = seed.ToCode().ToArray();
                        var f = code.First();
                        if (f == '>')
                            yield return $"ptr += {code.Length};";
                        else if (f == '<')
                            yield return $"ptr -= {code.Length};";
                    } break;
                case ToSimpleCode.CodeType.ValueChangeSeed:
                    {
                        var code = seed.ToCode().ToArray();
                        var f = code.First();
                        if (f == '+')
                            yield return $"(*ptr) += {code.Length};";
                        else if (f == '-')
                            yield return $"(*ptr) -= {code.Length};";
                    }
                    break;
                case ToSimpleCode.CodeType.InputSeed:
                    {
                        foreach (var letter in seed.ToCode())
                            yield return "*ptr = getchar();";
                    }
                    break;
                case ToSimpleCode.CodeType.OutputSeed:
                    {
                        foreach (var letter in seed.ToCode())
                            yield return "*ptr =  putchar(*ptr);";
                    }
                    break;
                case ToSimpleCode.CodeType.LoopSeed:
                    {
                        foreach (var letter in seed.ToCode())
                        {
                            if(letter == '[')
                                yield return "while(*ptr){";
                            else
                                yield return "}";
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private static IEnumerable<string> Header()
        {
            yield return "#include <stdio.h>";
            yield return "char memory[50000];";
            yield return "char* ptr = &memory[0];";
            yield return "int main ()";
            yield return "{";
        }

        private static IEnumerable<string> Fotter()
        {
            yield return "}";
        }

        private static string AddNest(string baseStr,int nest, int tabLength)
        {
            var nestStr = new string(Enumerable.Repeat(' ', tabLength * nest).ToArray());
            return nestStr + baseStr;
        }
    }
}
