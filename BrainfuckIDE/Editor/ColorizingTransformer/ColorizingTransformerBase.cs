using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BrainfuckIDE.Editor.ColorizingTransformer
{
    abstract class ColorizingTransformerBase : DocumentColorizingTransformer
    {

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


        protected abstract IEnumerable<RangeColorizer> GetRangeColorizers();

        protected class SingleRangeColorizer : RangeColorizer
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
        protected class MultiRangeColorizer : RangeColorizer
        {

            public List<int> Positions { get; } = new List<int>();

            public override IEnumerable<int> RangePositions => Positions;


            public MultiRangeColorizer(Color color) : base(color) { }
        }
        protected abstract class RangeColorizer
        {
            private readonly ISyntaxHighliteApplyer _applyer;
            public Action<VisualLineElement> Applyer => _applyer.Applyer;

            public abstract IEnumerable<int> RangePositions { get; }


            public RangeColorizer(Color color)
            {
                _applyer = new BackgroundSyntaxHighliteApplyer(color);
            }
        }

        protected interface ISyntaxHighliteApplyer
        {
            void Applyer(VisualLineElement element);
        }
        protected class BackgroundSyntaxHighliteApplyer : ISyntaxHighliteApplyer
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
