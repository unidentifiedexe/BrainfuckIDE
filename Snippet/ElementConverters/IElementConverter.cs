using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.ElementConverters
{
    public interface IElementConverter
    {
        public string Name { get; }
        public string Convert(string text);
             
    }
}
