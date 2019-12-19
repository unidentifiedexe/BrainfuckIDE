#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    /// <summary>
    /// メモリを逐次追加していくラッパークラス
    /// </summary>
    public partial class Memory
    {
        /// <summary> メモリ </summary>
        private readonly DataAddableMemory _memory = new DataAddableMemory();

        /// <summary> 現在のメモリの位置 </summary>
        private int _position = 0;

        /// <summary> 現在のメモリの位置を取得する </summary>
        public int Position => _position;

        /// <summary> 現在のメモリが指し示す値を取得する </summary>
        public byte CurrentValue
        {
            get => _memory[_position];
            set => _memory[_position] = value;
        }

        /// <summary> メモリポインタをインクリメントする </summary>
        /// <exception cref="OverflowException">ポインタが有効値を超えた</exception>
        public void MoveNext()
        {
            checked
            {
                _position++;
                _memory.CheckMemory(_position);
            }
        }

        /// <summary> メモリポインタをデクリメントする </summary>
        /// <exception cref="IndexOutOfRangeException">ポインタが負の値をとった</exception>
        public void MovePrev()
        {
            _position--;
            if (_position < 0)
                throw new IndexOutOfRangeException();
        }

        /// <summary> メモリの値をインクリメントする </summary>
        public void CurrentIncrement()
        {
            unchecked
            {
                _memory[_position]++;
            }
        }

        /// <summary> メモリの値をデクリメントする </summary>
        public void CurrentDecrement()
        {
            unchecked
            {
                _memory[_position]--;
            }
        }

        private EditableMemoryWrapper? _editableMemoryWrapper = null;

        /// <summary> デバッグ用の書き換え可能のメモリラッパーを取得します </summary>
        /// <returns></returns>
        public IEditableMemory GetEditableMemory()
        {
            if(_editableMemoryWrapper == null)
                _editableMemoryWrapper = new EditableMemoryWrapper(this);
            else
                _editableMemoryWrapper.ReadFeomParent();
            return _editableMemoryWrapper;

        }


        /// <summary> 自動でメモリ領域を追加するメモリ </summary>
        private class DataAddableMemory : IEnumerable<byte>
        {
            private readonly List<byte> _memory = new List<byte>() { 0 };

            private IReadOnlyList<byte> _readOnlyMemory = null!;

            /// <summary> 読み取り専用のメモリリストを返す </summary>
            public IReadOnlyList<byte> Memory => _readOnlyMemory ??= _memory.AsReadOnly();


            public byte this[int index]
            {
                get
                {
                    CheckMemory(index);
                    return _memory[index];
                }
                set
                {
                    CheckMemory(index);
                    _memory[index] = value;
                }
            }

            /// <summary>
            /// 指定indexが既に存在しているかを確認し、存在していなかれば、そこまでメモリを拡張する
            /// </summary>
            /// <param name="index"></param>
            public void CheckMemory(int index)
            {
                if (_memory.Count <= index)
                {
                    var add = index - _memory.Count + 1;
                    _memory.AddRange(new byte[add]);
                }
            }

            public IEnumerator<byte> GetEnumerator()
            {
                return _memory.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

        }



    }
}
