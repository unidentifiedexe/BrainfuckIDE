using ICSharpCode.AvalonEdit.Snippets;
using Snippets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor.Snippets.BasicBfSnippets
{
    class MultiByteVariable2
    {

        public static IEnumerable<ISnippet> GetAllSnippets()
        {
            var type = typeof(MultiByteVariable2);
            return
                 type.GetProperties(BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public)
                     .Where(x => x.PropertyType == typeof(ISnippet))
                     .Select(x => x.GetValue(type, null)).OfType<ISnippet>();
        }

        #region ReadNum

        public static ISnippet ReadNum { get; }
          = new ReginedSnippets("Multi.ReadNum", "Multi_ReadNum", GetReadNumByStrs());

        private static IEnumerable<string> GetReadNumByStrs()
        {
            yield return "+>+>+>+>+> +>+> //桁数";
            yield return ">+[-<";
            yield return "  ,+[-----------[----------------------";
            yield return "    [--------------<-[+<-]>[[-<+>]>]+<]";
            yield return "  ]]>";
            yield return "]";
            yield return "<<-[+<-]+>[->]<";
        }

        #endregion

        #region ReverseSign

        public static ISnippet ReverseSign { get; }
          = new ReginedSnippets($"Multi.{nameof(ReverseSign)}", $"Multi_{nameof(ReverseSign)}", GetReverseSignByStrs());

        private static IEnumerable<string> GetReverseSignByStrs()
        {
            yield return "[-[>[+++++++++>]>]+<]>->[<+[+++++++++>]>>]<<<";
            yield return "[ >++++++++++< -[->-<] <]";
            yield return ">>[[-<+>]>]<<";
        }

        #endregion

        #region Abs

        public static ISnippet Abs { get; }
          = new ReginedSnippets($"Multi.{nameof(Abs)}", $"Multi_{nameof(Abs)}", GetAbsByStrs());

        private static IEnumerable<string> GetAbsByStrs()
        {
            yield return "[<]>";
            yield return "-[+";
            yield return "  [>]<";
            yield return "  [-[>[+++++++++>]>]+<]>-<<";
            yield return "  [ >++++++++++< -[->-<] <]";
            yield return "  >>[[-<+>]>]<<";
            yield return "  [<]>[-]";
            yield return "]+";
            yield return "[>]<";
        }

        #endregion

        #region IsTrue

        public static ISnippet IsTrue { get; }
          = new ReginedSnippets($"Multi.{nameof(IsTrue)}", $"Multi_{nameof(IsTrue)}", GetIsTrueByStrs());

        private static IEnumerable<string> GetIsTrueByStrs()
        {
            yield return "// note 非ゼロ判定します";
            yield return "//start  ___{ a*}___";
            yield return "//resilt ___{ a*}_x_";
            yield return "//x : 0 if 0  1 if not 0";
            yield return "[-[+[>]>]+<] <[[<]<]>>[>]<";
        }

        #endregion

        #region WriteNum

        public static ISnippet WriteNum { get; }
          = new ReginedSnippets($"Multi.{nameof(WriteNum)}", $"Multi_{nameof(WriteNum)}", GetWriteNumByStrs());

        private static IEnumerable<string> GetWriteNumByStrs()
        {
            yield return "+[<]>[-[+[>]]<+>>] ++++++[-<-------->]<+<--> [[<]>[+>]<]<[<]>[.>]";
            yield return ">++++++[-<++++++++>]<- [[<]>[->]<]<[<]<[[->+<]<]>>[>]<　//数値復帰部";
        }

        #endregion

        #region Incriment

        public static ISnippet Incriment { get; }
          = new ReginedSnippets($"Multi.{nameof(Incriment)}", $"Multi_{nameof(Incriment)}", ">+[+++++++++<[->->+<<]+>[[-]<->]<]>>[+[-<<+>>]]>[[[-]<<+>>]>]<<");

        #endregion

        #region Decriment

        public static ISnippet Decriment { get; }
          = new ReginedSnippets($"Multi.{nameof(Decriment)}", $"Multi_{nameof(Decriment)}", "[-[>[+++++++++>]>]+<]>-<< >>>[<+[+++++++++>]>>]<<<");

        #endregion

        #region Decriment If Puls

        public static ISnippet DecrimentIfPuls { get; }
          = new ReginedSnippets($"Multi.{nameof(DecrimentIfPuls)}", $"Multi_{nameof(DecrimentIfPuls)}", "[-[>[+++++++++>]>]+<]>-<< >>>[<+[>]>>]<<<");

        #endregion

        #region Comparate

        public static ISnippet Comparate { get; }
          = new ReginedSnippets($"Multi.{nameof(Comparate)}", $"Multi_{nameof(Comparate)}", GetComparateByStrs());

        private static IEnumerable<string> GetComparateByStrs()
        {
            yield return "//start  ___{ a }___{ b*}___";
            yield return "//resilt ___{ a }_x_{ b*}___";
            yield return "//notice 桁数が同じである必要があります";
            yield return "//x : 0 if same / 1 if a lt b / 2 if a gt b";
            yield return "#region メイン比較部";
            yield return "[[->>+<<]<]";
            yield return "<<<[<]>";
            yield return "[";
            yield return "  -<<+>>";
            yield return "  [>[>]>-<<[<]<+>>-]";
            yield return "  >[>]>>>[>]>>";
            yield return "  -<<+>>";
            yield return "  [-<<+[<]<+>>[>]>]";
            yield return "  <<[<]<";
            yield return "  [";
            yield return "    <->>+<";
            yield return "    -[-[-[-[-[-[-[-[-[-[[+]>+<]]]]]]]]]]";
            yield return "  ]";
            yield return "  <+";
            yield return "  [- <[<]]>";
            yield return "]";
            yield return ">[<]";
            yield return ">[-<+>]<";
            yield return "#endregion";
            yield return "#region 後処理部";
            yield return "<<[<]<<[[->>+<<]<]>>>[>]>>>[>]>>[[-<<+>>]>]<<<";
            yield return "#endregion";

        }

        #endregion

        #region AddToPrev

        public static ISnippet AddToPrev { get; }
            = new MultiInputMultiByteVariableSnipetts($"Multi.{nameof(AddToPrev)}", $"Multi_{nameof(AddToPrev)}", GetAddToPrevByStrs(), GetAddToPrevCommens());

        private static IEnumerable<string> GetAddToPrevByStrs()
        {
            yield return "[";
            yield return "  [<]{p}<[<]<<[->>+<<]>>[>]>{n}[>]>+<<-";
            yield return "  [ [<]{p}<[<]>+[>]>{n}[>] >+<<- ] <";
            yield return "]";
            yield return "{p}<[<]<<[<]>";
            yield return "[[-<+>]>]>>[[-<<<+>>>]>]<<<<";
            yield return "[ ";
            yield return "  [->+<[->+<[->+<[->+<[->+<[->+<[->+<[->+<[->+<[->+<[ ";
            yield return "    [->+<]>----------< <[+>]>[<]";
            yield return "  ]]]]]]]]]]]<";
            yield return "]";
            yield return ">>[>]>>>{n}>>[[-<<+>>]>]<<<";
        }

        private static IEnumerable<string> GetAddToPrevCommens()
        {
            yield return "//start  ___{ a }___{ b*}___";
            yield return "//resilt ___{res}___{ b*}___";
        }

        #endregion

        #region AddToNext 

        public static ISnippet AddToNext { get; }
            = new MultiInputMultiByteVariableSnipetts($"Multi.{nameof(AddToNext)}", $"Multi_{nameof(AddToNext)}", GetAddToNextByStrs(), GetAddToNextCommens());

        private static IEnumerable<string> GetAddToNextByStrs()
        {
            yield return "[";
            yield return "  -";
            yield return "  [->>+[>]>{n}[>]<+[<]{p}<[<]< ]";
            yield return "  >>+[>]>{n}[>]<";
            yield return "  [->+<[->+<[->+<[->+<[->+<[->+<[->+<[->+<[->+<[->+<[ ";
            yield return "    [->+<]>----------< <[+>]>[<]";
            yield return "  ]]]]]]]]]]]";
            yield return "  <[<]{p}<[<]<<";
            yield return "]";
            yield return ">>>[[-<<+>>]>]>{n}[>]<";
            yield return "[";
            yield return "  [->+<[->+<[->+<[->+<[->+<[->+<[->+<[->+<[->+<[->+<[ ";
            yield return "    [->+<]>----------< <[+>]>[<]";
            yield return "  ]]]]]]]]]]]";
            yield return "  <";
            yield return "]";
            yield return ">>[[-<+>]>]<<[<]{p}<<<";
        }

        private static IEnumerable<string> GetAddToNextCommens()
        {

            yield return "//start  ___{ a*}___{ b*}___";
            yield return "//resilt ___{ a*}___{res}___";
            yield return "//notice b の桁が a 以上である必要があります";
        }

        #endregion

        #region SubFromNext

        public static ISnippet SubFromNext { get; }
            = new MultiInputMultiByteVariableSnipetts($"Multi.{nameof(SubFromNext)}", $"Multi_{nameof(SubFromNext)}", GetSubFromNextByStrs(), GetSubFromNextCommens());

        private static IEnumerable<string> GetSubFromNextByStrs()
        {
            yield return "> >>>{n}<++>{p}<<< <";
            yield return "[<]>[[-<<+>>]>]";
            yield return "<<<";
            yield return "[";
            yield return "  -[ ->>+[>]>>>{n}[>]< [-[<]] <[>++++++++++< [-[<]]< ]>{p}<<<[<]< ]";
            yield return "  >>+[>]>>>{n}[>]<";
            yield return "  [->+<]<[<]{p}<<<[<]<<";
            yield return "]";
            yield return ">>>[>]>>>{n}<[-]>[>]>[[-<+>]>]<<";
            yield return "[<]{p}<<<";
        }

        private static IEnumerable<string> GetSubFromNextCommens()
        {
            yield return "//start  ___{ a*}___{ b }___";
            yield return "//resilt ___{ a*}___{res}___";
            yield return "//notice b の桁が a 以上である必要があります。";
            yield return "//       アンダーフロー処理済み";
        }

        #endregion

        #region SubFromPrev

        public static ISnippet SubFromPrev { get; }
            = new MultiInputMultiByteVariableSnipetts($"Multi.{nameof(SubFromPrev)}", $"Multi_{nameof(SubFromPrev)}", GetSubFromPrevByStrs(), GetSubFromPrevCommens());

        private static IEnumerable<string> GetSubFromPrevByStrs()
        {
            yield return "[<]{p}<<<[<]++[[-<+>]>]";
            yield return ">>>{n}[>]<";
            yield return "[";
            yield return "  ->>+<<";
            yield return "  [ [<]{p}<<<[<]<[-[>[+++++++++>]>[>]>]+<] >-< >>>{n}[>]>+<<- ]";
            yield return "  <[<]{p}<<<[<]<[->+<]>[>]>>>{n}[>]<";
            yield return "]";
            yield return "{p}<<<[<]<[[->+<]<]>>[-]>[>]>>>{n}>>[[-<<+>>]>]<<";



        }

        private static IEnumerable<string> GetSubFromPrevCommens()
        {
            yield return "//start  ___{ a }___{ b*}___";
            yield return "//resilt ___{res}___{ b*}___";
            yield return "//notice a の桁が b 以上である必要があります。";
            yield return "//       アンダーフロー処理済み";
        }

        #endregion

        public static ISnippet Test { get; } =
            TestSnippets.TestSnippet;




        #region インナー関数

        class ReginedSnippets : NonArgumentSnippet
        {
            public ReginedSnippets(string shortcut, string title, IEnumerable<string> texts) : base(shortcut, title, texts)
            {
                InitSnipet();
            }

            public ReginedSnippets(string shortcut, string title, string text) : base(shortcut, title, text)
            {
                InitSnipet();
            }


            private void InitSnipet()
            {
                _snippet = GetWrapedSnipet(Title, _snippet);
            }

            static private Snippet GetWrapedSnipet(string title, Snippet innner)
            {
                return new Snippet()
                {
                    Elements =
                    {

                    new SnippetTextElement() { Text = $"#region {title}\n" },
                     innner,
                    new SnippetTextElement() { Text = $"\n#endregion\n" },
                    }
                };
            }
        }

        class MultiInputMultiByteVariableSnipetts : SnippetBase
        {

            public override string Title { get; protected set; }
            public override string Shortcut { get; protected set; }

            public MultiInputMultiByteVariableSnipetts(string shortcut, string title, IEnumerable<string> strs, IEnumerable<string> notices)
            {
                Title = title;
                Shortcut = shortcut;

                var (header, innner) = GetInnnerSnipetts(strs);
                _snippet = new Snippet
                {
                    Elements =
                    {
                        new SnippetTextElement() { Text = $"#region {title} " },
                        header ,
                        new SnippetTextElement() { Text = $"\n" },
                        GetCommentsSnippet(notices),
                        innner ,
                        new SnippetTextElement() { Text = $"\n#endregion\n" },
                    }
                };
            }

            private Snippet GetCommentsSnippet(IEnumerable<string> notices)
            {
                var ret = new Snippet();
                foreach (var item in notices)
                {
                    ret.Elements.Add(new SnippetTextElement() { Text = $"{item}\n" });
                }
                return ret;
            }

            private (SnippetReplaceableTextElement Header, Snippet Innner) GetInnnerSnipetts(IEnumerable<string> strs)
            {
                var conv = string.Join("\n", strs);

                var pos = new SnippetReplaceableTextElement { Text = "0" };
                var map = new Dictionary<string, Func<SnippetBoundElement>>
                {
                    ["p"] = ()=> new PrevMoveStrGeneretor() { TargetElement = pos },
                    ["n"] = ()=> new NextMoveStrGeneretor() { TargetElement = pos },
                };
                var ret = new Snippet();
                foreach (var item in ConvertToElement(conv, map))
                {
                    ret.Elements.Add(item);
                }
                return (Header: pos, Innner: ret);
            }


            private static IEnumerable<SnippetElement> ConvertToElement(string str, Dictionary<string, Func<SnippetBoundElement>> map)
            {
                Regex reg = new Regex("{.+?}");
                var matches = reg.Matches(str).OfType<Match>().OrderBy(p => p.Index);

                var startPos = 0;

                foreach (var item in matches)
                {
                    var trimd = item.Value.Substring(1, item.Value.Length - 2);

                    if (!map.TryGetValue(trimd, out var elementGetor))
                        continue;

                    if (item.Index != startPos)
                        yield return new SnippetTextElement() { Text = str.Substring(startPos, item.Index - startPos) };

                    yield return elementGetor();

                    startPos = item.Index + item.Length;
                }
                var left = str.Substring(startPos);

                if (left.Length > 0) yield return new SnippetTextElement() { Text = left };
            }
            private class NextMoveStrGeneretor : RepeatStringGenerator
            {
                public NextMoveStrGeneretor()
                    : base(" {", "[>]>>>", "} ")
                {

                }
            }
            private class PrevMoveStrGeneretor : RepeatStringGenerator
            {
                public PrevMoveStrGeneretor()
                    : base(" {", "<<<[<]", "} ")
                {

                }
            }
        }

        #endregion
    }
}
