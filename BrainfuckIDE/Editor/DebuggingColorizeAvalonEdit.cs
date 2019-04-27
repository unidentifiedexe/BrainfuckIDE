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

namespace BrainfuckIDE.Editor
{
    class DebuggingColorizeAvalonEdit: DocumentColorizingTransformer
    {

        private readonly SingleRangeColorizer _runnningPosition = new SingleRangeColorizer(Colors.Yellow);
        private readonly MultiRangeColorizer _breakPoints = new MultiRangeColorizer(Colors.Red);


        public int RunnningPosition
        {
            get => _runnningPosition.Positiion;
            set => _runnningPosition.Positiion = value;
        }

        public List<int> BreakPoints => _breakPoints.Positions;

        private IEnumerable<RangeColorizer> GetRangeColorizers()
        {
            yield return _breakPoints;
            yield return _runnningPosition;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            int lineLen = line.Length;
            foreach (var colorizer in GetRangeColorizers())
            {
                var positions =
                    colorizer.RangePositions
                    .Where(p => p >= lineStartOffset && p < lineStartOffset + lineLen);
                foreach (var pos in positions)
                {
                    base.ChangeLinePart(pos, pos + 1, colorizer.Applyer);
                }
            }
        }


        private class SingleRangeColorizer : RangeColorizer
        {
            public int Positiion { get; set; } = -1;
            public override IEnumerable<int> RangePositions
            {
                get
                {
                    if (Positiion >= 0) yield return Positiion;
                }
            }
            public SingleRangeColorizer(Color color) : base(color) { }
        }
        private class MultiRangeColorizer :RangeColorizer
        {

            public List<int> Positions { get; } = new List<int>();

            public override IEnumerable<int> RangePositions => Positions;


            public MultiRangeColorizer(Color color) : base(color) { }
        }
        private abstract class RangeColorizer
        {
            private readonly ISyntaxHighliteApplyer _applyer;
            public Action<VisualLineElement> Applyer => _applyer.Applyer;

            public abstract IEnumerable<int> RangePositions { get; }


            public RangeColorizer(Color color)
            {
                _applyer = new BackgroundSyntaxHighliteApplyer(color);
            }
        }

        private interface ISyntaxHighliteApplyer
        {
            void Applyer(VisualLineElement element);
        }
        private class BackgroundSyntaxHighliteApplyer : ISyntaxHighliteApplyer
        {
            private readonly Brush _brush;

            public BackgroundSyntaxHighliteApplyer(Color color)
            {
                _brush = new SolidColorBrush(color);
            }


            public void Applyer(VisualLineElement element)
            {
                element.BackgroundBrush = _brush;
            }
        }
    }


}
