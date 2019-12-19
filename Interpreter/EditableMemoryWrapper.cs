#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{

    public partial class Memory
    {

        /// <summary> <see cref="Memory"/> を直接編集するためのラッパークラス </summary>
        class EditableMemoryWrapper : IEditableMemory
        {

            readonly Memory _parent;
            private EditableMemoryToken[] _data = null!;
            private int _currentIndex;

            int IReadOnlyCollection<EditableMemoryToken>.Count => _data.Length;

            EditableMemoryToken IReadOnlyList<EditableMemoryToken>.this[int index] => _data[index];


            public int CurrentIndex
            {
                get => _currentIndex;
                set
                {
                    if ((uint)value >= _data.Length)
                        throw new IndexOutOfRangeException(nameof(CurrentIndex));
                    _currentIndex = value;
                }
            }

            public EditableMemoryWrapper(Memory parent)
            {
                _parent = parent;
                ReadFeomParent();
            }

            internal void ReadFeomParent()
            {
                _data = _parent._memory.Select((p, i) => new EditableMemoryToken(i, p, i == _parent.Position)).ToArray();
                CurrentIndex = _parent.Position;
            }

            public void ReflectToParent()
            {
                foreach (var (i, val) in _data.Where(p => p.IsEdited))
                {
                    _parent._memory[i] = val;
                }
                _parent._position = CurrentIndex;
            }

            IEnumerator<EditableMemoryToken> IEnumerable<EditableMemoryToken>.GetEnumerator()
            {
                return _data.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _data.GetEnumerator();
            }
        }

    }


    public interface IEditableMemory : IReadOnlyList<EditableMemoryToken>
    {
        /// <summary> 変更を元データに反映します </summary>
        void ReflectToParent();


        public int CurrentIndex { get; }
    }


    /// <summary> メモリの各インデックスの状態のラッパークラス </summary>
    public class EditableMemoryToken
    {
        private byte _value;
        public byte Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    IsEdited = true;
                }
            }
        }

        public int Index { get; }
        public bool IsCurrent { get; }
        public bool IsEdited { get; private set; } = false;



        public EditableMemoryToken(int index, byte value, bool isCurrent)
        {
            Value = value;
            Index = index;
            IsCurrent = isCurrent;
        }

        public void Deconstruct(out int index, out byte value)
        {
            index = Index;
            value = Value;
        }
    }
}
