#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.CodeAnalysis
{
    public class NestRefactor
    {

        private static readonly NestRefactor _basicInstance = new NestRefactor();

        public static string NestString => _basicInstance._nestString.NestString;

        public static string NestStr { get; internal set; }

        static public string ConvertNestStringFromTab(string str)
            => _basicInstance._nestString.ConvertNestStringFromTab(str);
        static public IEnumerable<string> Refact(IEnumerable<string> strs)
        {
            return _basicInstance.NestRefact(strs);
        }
        static public IEnumerable<string> Refact(IEnumerable<string> strs, int nest)
        {
            return _basicInstance.NestRefact(strs, nest);
        }

        private class NestStringMap
        {
            public string NestString { get; }

            private readonly List<string> _nestStringList = new List<string>() { string.Empty };


            public string this [int nest ] => GetNestString(nest);

            public NestStringMap(string nestString)
            {
                if (nestString.Length <= 0)
                    throw new ArgumentException($"{nameof(nestString)}は長さが非負である必要があります");
                NestString = nestString;
            }

            public string GetNestString(int nest)
            {
                if (nest <= 0) return string.Empty;
                while (nest >= _nestStringList.Count)
                    AddNextNestString();
                return _nestStringList[nest];
            }

            private void AddNextNestString()
            {
                _nestStringList.Add(_nestStringList.Last() + NestString);
            }

            public string GetNestedString( string str, int nest)
            {
                return $"{GetNestString(nest)}{str}";
            }


            public int GetFirstNest(string text) => GetFirstNest(text, NestString);
            private static int GetFirstNest(string text, string nestText)
            {
                int pos = 0;
                while (pos < text.Length)
                {
                    var next = text.IndexOf(nestText, pos);
                    if (next != pos)
                        break;
                    pos += +nestText.Length;
                }
                return pos / nestText.Length;
            }


            public string ConvertNestStringFromTab(string str)
            {
                return str.Replace("\t", NestString);
            }
        }

        private class NestDiffInfo
        {

            public int NestDiff { get; }

            public int NestDiffMin { get; }

            public string Text { get; }

            public NestDiffInfo(string text)
            {
                Text = text;
                (NestDiffMin, NestDiff) = CheckNestDiff(text);
            }

            private static (int min, int sum) CheckNestDiff(string str)
            {
                var min = int.MaxValue;
                var sum = 0;

                foreach (var item in str.Select(CheckNestDiffFromChar))
                {
                    sum += item;
                    if (sum< min)
                        min = sum;
                }

                return (min, sum);
            }

            private static int CheckNestDiffFromChar(char letter)
            {
                return letter switch
                {
                    '[' => 1,
                    ']' => -1,
                    _ => 0,
                };
            }

        }

        private readonly struct NestInfo
        {
            public NestInfo(int nest, string text)
            {
                Nest = nest;
                Text = text;
            }

            public int Nest { get; }

            public string Text { get; }
        }

        private readonly NestStringMap _nestString = new NestStringMap("  ");


        private NestRefactor()
        {

        }



        IEnumerable<string> NestRefact(IEnumerable<string> strs)
        {
            var nest = _nestString.GetFirstNest(strs.FirstOrDefault() ?? string.Empty);
            return NestRefact(strs, nest);
        }
        IEnumerable<string> NestRefact(IEnumerable<string> strs,int nest)
        {
            return Itr().ToArray();


            IEnumerable<string> Itr()
            {
                foreach (var item in GetNestInfos(strs))
                {
                    yield return _nestString.GetNestedString(item.Text, Math.Max(nest + item.Nest, 0));
                }
            }
        }

        static IEnumerable<NestInfo> GetNestInfos(IEnumerable<string> strs)
        {

            var stack = new Stack<NestDiffInfo>();
            var lineNestDic = new Dictionary<NestDiffInfo, int>();
            var nest = 0;
            foreach (var item in strs.Select(p => new NestDiffInfo(p.TrimStart())))
            {
                if (item.NestDiffMin >= 0)
                {
                    yield return new NestInfo(nest, item.Text);
                    if (item.NestDiff > 0)
                    {
                        AddNest(item, item.NestDiff);

                        lineNestDic.Add(item, nest);
                        nest++;
                    }
                }
                else //if(item.NestDiffMin < 0)
                {
                    var popCount = -item.NestDiffMin;

                    var target = RemoveNest(-item.NestDiffMin);
                    if (target != null)
                        nest = lineNestDic[target];
                    else
                        nest = Math.Min(nest - 1, -1);

                    yield return new NestInfo(nest, item.Text);
                    var add = item.NestDiff - item.NestDiffMin;

                    if (add > 0)
                    {
                        AddNest(item, add);
                        lineNestDic.Add(item, nest);
                        nest++;
                    }
                }
            }

            void AddNest(NestDiffInfo info, int nest)
            {
                if (nest < 0) return;
                for (int i = 0; i < nest; i++)
                    stack.Push(info);
            }

            NestDiffInfo? RemoveNest( int nest)
            {
                if (nest < 1) throw new ArgumentException($"{nameof(nest)}は非負である必要があります");

                for (int i = 0; i < Math.Min(nest - 1, stack.Count); i++)
                    _ = stack.Pop();
                if (stack.Any())
                    return stack.Pop();
                else return null;
            }

        }

    }
}
