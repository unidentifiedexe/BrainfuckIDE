using JsonHelper.Attributes;
using Snippets.ElementConverters;
using Snippets.RepleaceElements;
using Snippets.SnippetSeeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Snippets
{
    class SnippetList : IPriority
    {

        [DefaultCollectionType(typeof(IRepleaceElement[]))]
        public IEnumerable<IRepleaceElement> Elements { get; set; }

        [DefaultCollectionType(typeof(IElementConverter[]))]
        public IEnumerable<IElementConverter> Converters { get; set; }

        [DefaultCollectionType(typeof(ISnippetSeed[]))]
        public IEnumerable<ISnippetSeed> Snippets { get; set; }

        [DataMember]
        public int Priority { get; set; }

        public ISnippet[] GetSnipetts()
        {
            if (Snippets == null) return Array.Empty<ISnippet>();

            return Snippets.Select(GestSnippet).Where(p => p != null).ToArray();
            
            ISnippet GestSnippet(ISnippetSeed seed)
            {
                return seed switch
                {
                    IAddableSnippetSeed addable => addable.CreateSnippet(Elements, Converters),
                    ISnippetSeed baseic => baseic.CreateSnippet(),
                };
            }
        }
    }
}
