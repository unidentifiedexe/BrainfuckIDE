using Snippets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.SnippetSeeds
{
    interface ISnippetSeed
    {

        public string Name { get; }

        public ISnippet CreateSnippet();
    }
}
