using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Filer
{
    static class SnippetFileGetor
    {
        const string _snippetFileExtention = ".json";

        public static string[] GetSnipetFilePathes()
        {
            if (!Directory.Exists(LocalEnvironmental.SnippetDirectory))
                return Array.Empty<string>();
            return Directory.GetFiles(LocalEnvironmental.SnippetDirectory, @$"*{_snippetFileExtention}", SearchOption.AllDirectories);
        }
    }
}
