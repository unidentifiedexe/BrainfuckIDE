#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BrainfuckIDE.Editor.Controls;
using BrainfuckInterpreter;
using WpfUtils;

namespace BrainfuckIDE.Controls.ViewModels
{
    class InterpreterRunningViewModel : ViewModelBase
    {
        public EditLayerViewModel EditrVM { get; } = new EditLayerViewModel();
        public ResultTextViewModel ResultTextVM { get; } = new ResultTextViewModel();
        public MemoryViewModel MemoryVM { get; } = new MemoryViewModel();

        public TextImputOnlyInitDataViewModel TextImputDataVM { get; }
            = new TextImputOnlyInitDataViewModel();

        private Interpreter? _interpreter;
        private Guid _sourceCodeHash = Guid.Empty;



        private AsyncCommand<RunType>? _runCommand = null;
        private Command? _stopCommand;
        private Command? _pauseCommand;

        public AsyncCommand<RunType> RunCommand => _runCommand ??= new AsyncCommand<RunType>(Run);

        public Command RaiseStopReqestCommand => _stopCommand ??= new Command(RaiseStopReqest);
        public Command RaisePauseReqestCommand => _pauseCommand ??= new Command(RaisePauseReqest);


        private void RaiseStopReqest()
        {
            RaisePauseReqest();
            _interpreter = null;
            UpdateDebuggerState();

        }
        private void RaisePauseReqest()
        {
            _interpreter?.NoticeStopReqest();
        }

        private event Action<Interpreter> StopInterpretorRunEvent;


        private bool InitializiedInterpretor()
        {
            var (hash, code) = EditrVM.SourceCode;
            try
            {
                var ret = true;
                if (_interpreter == null || _interpreter.IsStopped())
                {
                    _sourceCodeHash = hash;
                    _interpreter = new Interpreter(code) {};

                    _interpreter.TryReadChar += TextImputDataVM.GetTextSender().TryGetNextChar;
                    _interpreter.WriteChar += ResultTextVM.WriteChar;
                    ResultTextVM.Clear();
                }
                else
                {
                    if (_sourceCodeHash != hash)
                        ret &= _interpreter.TryForceUpdateSourceCode(code, EditrVM.RunnningPosition);
                    else
                        ret &= _interpreter.TryForceUpdateRunnningPosition(EditrVM.RunnningPosition);
                    MemoryVM.ReflectToInterpretor();
                }
                _interpreter.BreakPoints = EditrVM.GetBreakPoints();

                return ret;
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.Message);
                return false;
            }
            finally
            {
                _sourceCodeHash = hash;
            }
        }
        private async Task Run(RunType runType)
        {
            if (!InitializiedInterpretor() || _interpreter == null)
            {
                MessageBox.Show("Interpretorの作成に失敗しました");
                return;
            }
            var interpreter = _interpreter;

            EditrVM.IsReadOnly = true;
            MemoryVM.IsReadOnly = true;
            await Task.Run(() => interpreter.ExecuteNextCode(runType));

            EditrVM.IsReadOnly = false;
            MemoryVM.IsReadOnly = false;
            StopInterpretorRunEvent?.Invoke(interpreter);
            UpdateDebuggerState();

        }

        private void UpdateDebuggerState()
        {
            EditrVM.RunnningPosition = _interpreter?.Position ?? Place.Empty;

            MemoryVM.SetMemory(_interpreter?.GetEditableMemory());
        }

    }
}
