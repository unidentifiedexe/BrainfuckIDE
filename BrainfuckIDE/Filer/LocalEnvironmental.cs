#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Filer
{
    static class LocalEnvironmental
    {

        static public string TemporaryDirectory => "./.temp/";

        static public string SnippetDirectory => "./Snippets/";

        private static Process? _currentProcess;

        static public Process CurrentProcess => _currentProcess ??= Process.GetCurrentProcess();


        public static string Delimiter => "\n";
    }
}
