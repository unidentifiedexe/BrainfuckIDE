using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BrainfuckIDE.Editor
{
    /// <summary> コード上に配置される色を管理します </summary>
    static class EditorColoers
    {
        static public Color Background { get; } = (Color)ColorConverter.ConvertFromString("#FF1E1E1E");
        static public Color Foreground { get; } = (Color)ColorConverter.ConvertFromString("#FFF1F1F1");
    }
}
