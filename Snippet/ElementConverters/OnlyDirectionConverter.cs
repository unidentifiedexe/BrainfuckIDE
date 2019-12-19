using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.ElementConverters
{
    class OnlyDirectionConverter : IElementConverter
    {
        public string Name { get; set; }

        public string Convert(string text)
        {
            var arr = text.Select(p => Converter(p)).Where(p => p != '\0').ToArray();
            return new string(arr);

            static char Converter(char letter)
            {
                return letter switch
                {
                    '>' => '>',
                    '<' => '<',
                    _ => '\0',
                };
            }
        }
    }
}
