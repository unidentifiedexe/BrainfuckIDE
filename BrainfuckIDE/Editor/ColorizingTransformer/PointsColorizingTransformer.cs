using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BrainfuckIDE.Editor.ColorizingTransformer
{
    class PointsColorizingTransformer : ColorizingTransformerBase
    {
        private readonly MultiRangeColorizer _colorizer;

        public PointsColorizingTransformer(Color color)
        {
            _colorizer = new MultiRangeColorizer(color);
        }

        public IEnumerable<int> Points
        {
            get => _colorizer.Positions;
            set
            {
                Clear();
                if (value != null)
                    _colorizer.Positions.AddRange(value);
            }
        }

        public void Clear() => _colorizer.Positions.Clear();

        protected override IEnumerable<RangeColorizer> GetRangeColorizers()
        {
            yield return _colorizer;
        }
    }
}
