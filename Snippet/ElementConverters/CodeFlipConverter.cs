using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.ElementConverters
{
    [Serializable]
    class CodeFlipConverter : IElementConverter
    {
        public string Name { get; }

        public string Convert(string text)
        {
            var arr = text.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}
