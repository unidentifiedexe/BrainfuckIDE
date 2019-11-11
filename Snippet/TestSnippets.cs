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
        public static ISnippet TestSnippet => Test();


        public static ISnippet[] GetSnippets()
        {
            SnippetLoader.Instance.Load(new[] { "hoge.json" });
            return SnippetLoader.Instance.LoadedSnippets.ToArray();
        }
        private static ISnippet Test()
        {
            throw new NotImplementedException();

            //var assembly = typeof(TestSnippets).Assembly;
            //var knownTypes = assembly.DefinedTypes.ToArray();
            ////JsonHelper.Json.Save(_testSnippetBase, "hoge.json", knownTypes);
            //var str = new StreamReader("hoge.json").ReadToEnd();
            //var ezer = new JsonHelper.WellKnownTypeDeserializer(knownTypes);
            //var  x = ezer.Parse(str);
            //return (x as SnippetList).GetSnipetts().First();
        }
    }
}
