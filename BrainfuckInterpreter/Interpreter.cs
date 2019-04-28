#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckInterpreter
{
    public class Interpreter
    {
        private RunnningCodePointer _sourceCode;
        private readonly Memory _memory;
        private bool _hasStopReqest = false;
        private Place[] _breakPoints = new Place[0];

        public IEnumerable<Place>? BreakPoints
        {
            get => _breakPoints;
            set => _breakPoints = value?.ToArray() ?? new Place[0];
        }


        /// <summary> 実行ステートを取得する </summary>
        public RunnningState State { get; private set; }

        /// <summary> 異常終了したときの例外を取得する。異常終了をしていない場合はnullを返す。 </summary>
        public Exception? Exception { get; private set; }

        public Interpreter(string code)
        {

            _sourceCode = new RunnningCodePointer(code);
            _memory = new Memory();
        }

        public bool TryForceUpdateSourceCode(string code, Place currentPos)
        {
            if (State != RunnningState.Pause) return false;
            _sourceCode = new RunnningCodePointer(code, currentPos);
            return true;
        }

        public bool TryForceUpdateRunnningPosition(Place currentPos)
        {
            if (State != RunnningState.Pause) return false;
            _sourceCode.FourceUodatePosition(currentPos);
            return true;
        }

        /// <summary> 次に実行される場所を取得する </summary>
        public Place Position => _sourceCode.Position;

        public IEditableMemory GetEditableMemory()
        {
            return _memory.GetEditableMemory();
        }

        ///// <summary> コードを実行する </summary>
        //public void ExecuteNextCode()
        //{
        //    ExecuteNextCode(false);
        //}

        ///// <summary> コードを実行する </summary>
        //public void ExecuteNextStepCode()
        //{
        //    ExecuteNextCode(true);
        //}

        /// <summary> 実行の停止を要求します </summary>
        public void NoticeStopReqest()
        {
            _hasStopReqest = true;
        }
        

        /// <summary>
        /// コードを実行する
        /// </summary>
        /// <param name="isOnlyOneCode">1ステップのみ実行するかどうか</param>
        public void ExecuteNextCode(RunType runType)
        {
            if (State == RunnningState.Finished) return;
            if (State == RunnningState.ErrorStoped) return;
            if (State == RunnningState.Runnning) return;
            _hasStopReqest = false;
            State = RunnningState.Runnning;
            Exception = null;
            try
            {
                while (!_hasStopReqest)
                {
                    var letter = _sourceCode.Current;
                    if (letter == char.MaxValue) throw new Exception("コードポインタの位置がおかしい");
                    else if (letter == '+') _memory.CurrentIncrement();
                    else if (letter == '-') _memory.CurrentDecrement();
                    else if (letter == '>') _memory.MoveNext();
                    else if (letter == '<') _memory.MovePrev();
                    else if (letter == '[') { if (_memory.CurrentValue == 0) _sourceCode.Jamp(); }
                    else if (letter == ']') { if (_memory.CurrentValue != 0) _sourceCode.Jamp(); }
                    else if (letter == '.') WhiteCharMethod(_memory.CurrentValue);
                    else if (letter == ',')
                    {
                        if (!ReadCharMethod(out var let)) break;
                        _memory.CurrentValue = let;
                    }

                    if (!_sourceCode.MoveNext()) break;
                    if (runType == RunType.OneStep) break;
                    if (_breakPoints.Contains(_sourceCode.Position)) break;
                }
                if (_sourceCode.IsFinished)
                    State = RunnningState.Finished;
                else
                    State = RunnningState.Pause;
            }
            catch (Exception exe)
            {
                Exception = exe;
                State = RunnningState.ErrorStoped;
                Console.WriteLine(exe.Message);
            }
        }


        public delegate bool ReadCharDelegate(out byte letter);
        public delegate void WhiteCharDelegate(byte letter);

        public ReadCharDelegate? TryReadChar { get; set; }
        public WhiteCharDelegate? WriteChar { get; set; }

        /// <summary> 文字の読み取りが完了した時に発火するイベント </summary>
        public event EventHandler? CharRead;

        /// <summary> 文字の書き出しが完了した時に発火するイベント </summary>
        public event EventHandler? CharWriten;

        private bool ReadCharMethod(out byte letter)
        {
            var tryReadChar = TryReadChar ?? throw new NullReferenceException(nameof(TryReadChar));
            while (!tryReadChar(out letter))
            {
                if (_hasStopReqest)
                    return false;
            }
            CharRead?.Invoke(this, new EventArgs());
            return true;
        }

        private void WhiteCharMethod(byte letter)
        {
            var writeChar = WriteChar;
            if (writeChar != null)
            {
                writeChar(letter);
            }
            else
            {
                Console.Write((char)letter);
            }
            CharWriten?.Invoke(this, new EventArgs());
            return;
        }


        public bool IsStopped()
        {
            return
                State == RunnningState.ErrorStoped ||
                State == RunnningState.Finished;
        }
    }

    public enum RunnningState
    {
        Ready,
        Runnning,
        Pause,
        Finished,
        ErrorStoped,
    }
}
