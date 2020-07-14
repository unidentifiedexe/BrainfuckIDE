﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{

    /// <summary> ソースコードの実行ポインタを管理するクラス </summary>
    class RunnningCodePointer
    {
        /// <summary> ソースコードが終了したかどうか </summary>
        public bool IsFinished = false;

        /// <summary> ソースコードの現在位置を取得する </summary>
        /// <remarks> 実行中でない場合初期位置(0,0)を返す</remarks>
        public Place Position
        {
            get
            {
                if (IsFinished || _runningPointer == -1)
                    return Place.Empty;
                else
                    return _sourceCode.GetOriginPlaceFromTrimedCode(_runningPointer);
            }
        }

        /// <summary> 現在実行中のポインタのトリムコードでの位置 </summary>
        private int _runningPointer = 0;

        /// <summary> ソースコード </summary>
        private readonly RunnningCode _sourceCode;

        /// <summary> ジャンパー </summary>
        private readonly Janper _janper;

        /// <summary> 現在のポインタが指し示すソースコードの文字を取得する。失敗した場合はChar.MinValueを返す。 </summary>
        public char Current
        {
            get
            {
                if (_runningPointer == -1)
                    return char.MaxValue;
                else if (IsFinished)
                    return char.MaxValue;
                else
                    return _sourceCode[_runningPointer];
            }
        }

        /// <summary> ソースコードよりインスタンスの作成 </summary>
        /// <param name="sourceCode">ソースコード</param>
        public RunnningCodePointer(string sourceCode)
        {
            _sourceCode = new RunnningCode(sourceCode);
            _janper = new Janper(_sourceCode);
        }


        /// <summary> ソースコードよりインスタンスの作成 </summary>
        /// <param name="sourceCode">ソースコード</param>
        public RunnningCodePointer(string sourceCode, Place currentPos)
        {
            _sourceCode = new RunnningCode(sourceCode);
            _janper = new Janper(_sourceCode);
            if(!currentPos.IsEmpty())
                _runningPointer = _sourceCode.GetTrimedCodePlaceFromOrigin(currentPos);
        }

        /// <summary> ポインタを一つ移動させる </summary>
        /// <remarks> 
        /// </remarks>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (IsFinished) return false;
            _runningPointer++;
            //System.Diagnostics.Debug.WriteLine($"Next => [{_runnningPointer}] = {_sourceCode[_runnningPointer]}");
            if (_sourceCode[_runningPointer] == Char.MinValue)
            {
                IsFinished = true;
                return false;
            }

            return true;
        }

        /// <summary> [,]でのジャンプを行うする。 </summary>
        public void Jamp()
        {
            if (Current == '[' || Current == ']')
                _runningPointer = _janper[_runningPointer];
        }

        public void FourceUodatePosition(Place place)
        {
            if (!place.IsEmpty())
                _runningPointer = _sourceCode.GetTrimedCodePlaceFromOrigin(place);
        }

        /// <summary> []によるコードの移動をサポートする </summary>
        private class Janper
        {
            private readonly Dictionary<int, int> _janpMap = new Dictionary<int, int>();

            public Janper(RunnningCode code)
            {
                Stack<int> stack = new Stack<int>();

                for (int index = 0; index < code.Length; index++)
                {
                    var letter = code[index];

                    if (letter == '[')
                    {
                        stack.Push(index);
                    }
                    else if (letter == ']')
                    {
                        if (!stack.Any()) throw new Exception("\']\'に対応する\'[\'が存在しません");
                        var firstIndex = stack.Pop();
                        _janpMap.Add(firstIndex, index);
                        _janpMap.Add(index, firstIndex);
                    }
                }
                if (stack.Any()) throw new Exception("\'[\'に対応する\']\'が存在しません");


            }

            public int this[int index]
            {
                get
                {
                    if (_janpMap.ContainsKey(index))
                        return _janpMap[index];
                    else
                        return index;
                }
            }
        }


    }
}
