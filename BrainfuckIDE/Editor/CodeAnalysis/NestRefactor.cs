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

        static public IEnumerable<string> Refact(IEnumerable<string> strs, int nestSpaceNum)
        {
            return _basicInstance.NestRefact(strs, nestSpaceNum);
        }
        static public IEnumerable<string> Refact(IEnumerable<string> strs, int firstSpaceNum, int nestSpaceNum)
        {
            return _basicInstance.NestRefact(strs, firstSpaceNum, nestSpaceNum);
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
                    if (sum < min)
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


        private NestRefactor()
        {

        }



        IEnumerable<string> NestRefact(IEnumerable<string> strs, int nestSpaceNum)
        {
            var nest = SpaceStringMap.CountFrontSpace(strs.FirstOrDefault() ?? string.Empty);
            return NestRefact(strs, nest, nestSpaceNum);
        }
        IEnumerable<string> NestRefact(IEnumerable<string> strs, int firstSpaceNum, int nestSpaceNum)
        {
            return Itr().ToArray();

            IEnumerable<string> Itr()
            {
                var isFirst = true;
                foreach (var item in GetNestInfos(strs))
                {
                    if (isFirst)
                    {
                        firstSpaceNum -= item.Nest * nestSpaceNum;
                        isFirst = false;
                    }
                    yield return SpaceStringMap.GetNestedString(item.Text, Math.Max(firstSpaceNum + item.Nest * nestSpaceNum, 0));

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

            NestDiffInfo? RemoveNest(int nest)
            {
                if (nest < 1) throw new ArgumentException($"{nameof(nest)}は非負である必要があります");
                var max = Math.Min(nest - 1, stack.Count);
                for (int i = 0; i < max; i++)
                    _ = stack.Pop();
                if (stack.Any())
                    return stack.Pop();
                else return null;
            }

        }

    }

}
