using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.ElementConverters
{
    [Serializable]
    class DirectionFlipConverter : IElementConverter
    {
        public string Name { get; set; }

        public string Convert(string text)
        {
            var arr = text.Select(Converter).ToArray();

            return new string(arr);

            char Converter(char letter)
            {
                return letter switch
                {
                    '>' => '<',
                    '<' => '>',
                    char x => x,
                };
            }


        }
    }
}
