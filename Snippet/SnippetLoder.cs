using JsonHelper;
using Snippets.SnippetSeeds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snippets
{
    public class SnippetLoader
    {
        private static readonly Type[] _knownType = typeof(SnippetLoader).Assembly.DefinedTypes.ToArray();

        private static readonly WellKnownTypeDeserializer _deserializer =
            new WellKnownTypeDeserializer(_knownType);


        private static ISnippet[] LoadSnipetts(IEnumerable<string> path)
        {
            return 
                path?
                .Select(LoadJson)
                .OrderByDescending(p => (p as IPriority)?.Priority ?? 0)
                .SelectMany(LoadSnipettsFrom).ToArray()
                ?? Array.Empty<ISnippet>();
        }
        private static ISnippet[] LoadSnipettsFrom(object obj)
        {
            return obj switch
            {
                ISnippetSeed snipetts => new[] { snipetts.CreateSnippet() },
                SnippetList list => list.GetSnipetts(),
                _ => Array.Empty<ISnippet>(),
            };
        }
        private static object LoadJson(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
                using var s = File.OpenRead(path);
                return _deserializer.Parse(s);
            }
            catch
            {
                return null;
            }
        }


        private static SnippetLoader _instance;
        public static SnippetLoader Instance => _instance ??= new SnippetLoader();



        private ISnippet[] _loadedSnipet;
        public IEnumerable<ISnippet> LoadedSnippets => _loadedSnipet;

        public bool IsLoaded => _loadedSnipet != null;
        public void Load(IEnumerable<string> path)
        {
            _loadedSnipet = LoadSnipetts(path);
        }
    }
}
