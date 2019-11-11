#nullable enable
using ICSharpCode.AvalonEdit.Snippets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.ElementConverters
{
    [Serializable]
    class ConverterBoundElement : SnippetBoundElement
    {
        private readonly IEnumerable<IElementConverter> _converters;

        public ConverterBoundElement(IEnumerable<IElementConverter> converters)
        {
            _converters = converters?.ToArray() ?? throw new ArgumentNullException(nameof(converters));
        }

        public override string ConvertText(string input)
        {
            return _converters.Aggregate(input, (acm, conv) => conv.Convert(acm));
        }
    }
}
