#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public TextImputOnlyInitDataViewModel TextImputDataViewModel { get; }
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
            var interpritor = _interpreter ??= new Interpreter(EditrVM.SourceCode) { BreakPoints = EditrVM.GetBreakPoints() };

            EditrVM.IsReadOnly = true;
            await Task.Run(() => interpritor.ExecuteNextCode(runType));

            EditrVM.IsReadOnly = false;
            StopInterpretorRunEvent?.Invoke(interpritor);
            UpdateDebuggerState();

        }


        private void UpdateDebuggerState()
        {
            EditrVM.RunnningPosition = _interpreter?.Position ?? Place.Empty;

            MemoryVM.SetMemory(_interpreter?.GetEditableMemory());
        }

    }
}
