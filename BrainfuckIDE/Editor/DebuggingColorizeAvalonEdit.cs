#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace BrainfuckIDE.Editor.ColorizingTransformer
{
    class DebuggingColorizingTransformer : ColorizingTransformerBase
    {
        private readonly SingleRangeColorizer _runnningPosition = new SingleRangeColorizer(Colors.Yellow);
        private readonly MultiRangeColorizer _breakPoints = new MultiRangeColorizer(Colors.Red);



        public int RunnningPosition
        {
            get => _runnningPosition.Positiion;
            set => _runnningPosition.Positiion = value;
        }

        public List<int> BreakPoints => _breakPoints.Positions;

        protected override IEnumerable<PointsColorizer> GetPointsColorizers()
        {
            yield return _breakPoints;
            yield return _runnningPosition;
        }


    }

}
