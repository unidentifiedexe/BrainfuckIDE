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

        private async Task Run(RunType runType)
        {
            if(_interpreter == null || _interpreter.IsStopped())
            {
                try
                {
                    _interpreter = new Interpreter(EditrVM.SourceCode) { BreakPoints = EditrVM.GetBreakPoints() };

                    _interpreter.TryReadChar += TextImputDataVM.GetTextSender().TryGetNextChar;
                    _interpreter.WriteChar += ResultTextVM.WriteChar;
                    ResultTextVM.Clear();
                }
                catch(Exception exe)
                {
                    MessageBox.Show(exe.Message);
                    return;
                }
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
