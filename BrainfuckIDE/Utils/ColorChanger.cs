using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BrainfuckIDE.Utils
{
    static class ColorChanger
    {
        static public Color AddAlpha(this Color color,byte a)
        {
            return Color.FromArgb((byte)(a * color.A / 255), color.R, color.G, color.B);
        }
    }
}
