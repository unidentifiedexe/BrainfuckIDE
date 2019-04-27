using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckInterpreter
{
    class RunnningCode
    {
        //private readonly string _sourceCode;
        private readonly char[] _trimedSourceCode;

        private readonly Dictionary<int, Place> _mapToOriginCode = new Dictionary<int, Place>();
        private readonly Dictionary<Place, int> _mapToTrimed = new Dictionary<Place, int>();

        /// <summary>
        /// 指定位置でのコードの文字を返す。終端の場合は Char.MinValue を返す。
        /// </summary>
        /// <param name="index">トリムされたコードの位置</param>
        /// <returns></returns>
        public char this[int index]
        {
            get
            {
                if (index >= _trimedSourceCode.Length)
                    return Char.MinValue;
                else
                    return _trimedSourceCode[index];
            }
        }

        /// <summary> ソースコードの長さを返す </summary>
        public int Length => _trimedSourceCode.Length;

        /// <summary> ソースコードの長さを返す </summary>
        public long LongLength => _trimedSourceCode.LongLength;

        /// <summary>
        /// 元コードの位置より,トリムされたコードでの位置を得る
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Place GetOriginPlaceFromTrimedCode(int index)
        {
            return _mapToOriginCode[index];
        }

        /// <summary>
        /// トリムされたコードより，元のコードでの位置を得る
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetOriginPlaceFromTrimedCode(Place index)
        {
            return _mapToTrimed[index];
        }

        /// <summary> 列挙子を取得する </summary>
        /// <returns></returns>
        public IEnumerator<char> GetEnumerator() => _trimedSourceCode.AsEnumerable().GetEnumerator();

        /// <summary> 不要な文字が削除されたコードを取得する </summary>
        /// <returns></returns>
        public string GetTrimedCode()
        {
            return new string(_trimedSourceCode);
        }

        /// <summary> ソースコードよりインスタンスを生成する </summary>
        /// <param name="sourceCode"></param>
        public RunnningCode(string sourceCode)
        {
            //_sourceCode = sourceCode;
            var trimedCahrs = new List<char>(sourceCode.Length);
            Place originPlace = new Place();
            int trimedPlace = 0;

            foreach (var letter in sourceCode)
            {
                if (letter == '\n')
                {
                    originPlace.Colomun = 0;
                    originPlace.Line++;
                }
                else if (letter == '\r')
                {
                    //無視
                }
                else
                {
                    if (EffectiveCharacters.Characters.Contains(letter))
                    {
                        _mapToOriginCode.Add(trimedPlace, originPlace);
                        _mapToTrimed.Add(originPlace, trimedPlace);
                        trimedCahrs.Add(letter);
                        trimedPlace++;
                    }
                    originPlace.Colomun++;
                }
            }

            _trimedSourceCode = trimedCahrs.ToArray();
        }
    }

    public struct Place
    {
        public int Line { get; internal set; }
        public int Colomun { get; internal set; }

        public Place(int line, int colomun)
        {
            Line = line;
            Colomun = colomun;
        }


        static public Place Empty { get; } = new Place(-1, -1);

        public bool IsEmpty() => IsEmpty(this);
        static public bool IsEmpty(Place place)
        {
            return place.Line < 0 || place.Colomun < 0;
        }

    }
}
