#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfUtils
{

    public interface ICanExecuteChangedRaiseableCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }


    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
    public class Command : ICommand, ICanExecuteChangedRaiseableCommand
    {

        private readonly Action _action;

        private readonly Func<bool>? _canExecute;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        /// <summary>特定の処理を実行するコマンドを生成します</summary>
        /// <param name="action">実行する処理</param>
        public Command(Action action)
        {
            _action = action;
            _canExecute = null;
        }


        /// <summary>特定の処理を実行するコマンドを生成します</summary>
        /// <param name="action">実行する処理</param>
        /// <param name="canExecute"></param>
        public Command(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }


        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }

    public class Command<T> : ICommand, ICanExecuteChangedRaiseableCommand
    {

        private readonly Action<T> _action;

        private readonly Func<bool>? _canExecute;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        /// <summary>特定の処理を実行するコマンドを生成します</summary>
        /// <param name="action">実行する処理</param>
        public Command(Action<T> action)
        {
            _action = action;
            _canExecute = null;
        }


        /// <summary>特定の処理を実行するコマンドを生成します</summary>
        /// <param name="action">実行する処理</param>
        /// <param name="canExecute"></param>
        public Command(Action<T> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }

    public class AsyncCommand : IAsyncCommand, ICanExecuteChangedRaiseableCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;

        public AsyncCommand(
            Func<Task> execute,
            Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }


        #region Explicit implementations

        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
        #endregion
    }



    public class AsyncCommand<T> : IAsyncCommand, ICanExecuteChangedRaiseableCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<T,Task> _execute;
        private readonly Func<bool>? _canExecute;

        public AsyncCommand(
            Func<T,Task> execute,
            Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();
                    await _execute((T)parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }


        #region Explicit implementations

        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
        #endregion
    }


}
