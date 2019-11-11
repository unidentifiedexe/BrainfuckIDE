using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Snippets.ElementConverters;
using Snippets.RepleaceElements;
using Snippets.SnippetSeeds;

namespace Snippets
{
    public class TestSnippets
    {
        public static ISnippet TestSnippet =>
            _testSnippetBase.CreateSnippet();

        private static readonly ConverterSnippetSeed _testSnippetBase = Test();
        private static IEnumerable<string> TestCodeByStrs()
        {
            yield return "#region {pos}";
            yield return "[<]{pos:prev}<<<[<]++[[-<+>]>]";
            yield return ">>>{pos:next}[>]<";
            yield return "[";
            yield return "  ->>+<<";
            yield return "  [ [<]{pos:prev}<<<[<]<[-[>[+++++++++>]>[>]>]+<] >-< >>>{pos:next}[>]>+<<- ]";
            yield return "  <[<]{pos:prev}<<<[<]<[->+<]>[>]>>>{pos:next}[>]<";
            yield return "]";
            yield return "{pos:prev}<<<[<]<[[->+<]<]>>[-]>[>]>>>{pos:next}>>[[-<<+>>]>]<<";
            yield return "#endregion";



        }


        private static ConverterSnippetSeed Test()
        {
            var assembly = typeof(TestSnippets).Assembly;
            var knownTypes = assembly.DefinedTypes
                .Where(p => p.GetCustomAttributes(typeof(DataContractAttribute), false) != null)
                .ToArray();
            //JsonHelper.Json.Save(_testSnippetBase, "hoge.json", knownTypes);
            var str = new StreamReader("hoge.json").ReadToEnd();
            var ezer = new JsonHelper.WellKnownTypeDeserializer(knownTypes);
            var  x = ezer.Parse(str);
            return x as ConverterSnippetSeed;
        }
    }
}
