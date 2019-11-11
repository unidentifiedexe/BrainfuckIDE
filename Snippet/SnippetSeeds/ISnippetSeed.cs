using Snippets;
using Snippets.ElementConverters;
using Snippets.RepleaceElements;
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

    interface IAddableSnippetSeed : ISnippetSeed
    {
        ISnippet CreateSnippet(IEnumerable<IRepleaceElement> additivElements, IEnumerable<IElementConverter> additivConverters);

    }
}