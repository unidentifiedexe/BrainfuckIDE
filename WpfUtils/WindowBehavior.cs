#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfUtils
{

    public class WindowBehavior
    {

        //static public ExecutedRoutedEventHandler WindowAction => new ExecutedRoutedEventHandler(WindowActionFunc);

        private static ExecutedRoutedEventHandler? _windowCloseEvent;
        static public ExecutedRoutedEventHandler WindowCloseEvent => _windowCloseEvent ??= GetEventHandler(WindowCommandType.CloseWindow);


        private static ExecutedRoutedEventHandler? _windowMinimizeEvent;
        static public ExecutedRoutedEventHandler WindowMinimizeEvent => _windowMinimizeEvent ??= GetEventHandler(WindowCommandType.MinimizeWindow);


        private static ExecutedRoutedEventHandler? _windowMaximizeEvent;
        static public ExecutedRoutedEventHandler WindowMaximizeEvent => _windowMaximizeEvent ??= GetEventHandler(WindowCommandType.MaximizeWindow);


        private static ExecutedRoutedEventHandler? _windowRestoreEvent;
        static public ExecutedRoutedEventHandler WindowRestoreEvent => _windowRestoreEvent ??= GetEventHandler(WindowCommandType.RestoreWindow);


        static ExecutedRoutedEventHandler GetEventHandler(WindowCommandType commandType)
        {
            return new ExecutedRoutedEventHandler((obj, t) => WindowActionFunc(obj, t, commandType));
        }

        static private void WindowActionFunc(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(e.Parameter is WindowCommandType commandType)) return;
            WindowActionFunc(sender, e, commandType);
        }
        static private void WindowActionFunc(object sender, ExecutedRoutedEventArgs e, WindowCommandType commandType)
        {

            Action<Window>? action = commandType switch
            {
                WindowCommandType.CloseWindow => new Action<Window>(SystemCommands.CloseWindow),
                WindowCommandType.MaximizeWindow => new Action<Window>(SystemCommands.MaximizeWindow),
                WindowCommandType.MinimizeWindow => new Action<Window>(SystemCommands.MinimizeWindow),
                WindowCommandType.RestoreWindow => new Action<Window>(SystemCommands.RestoreWindow),
                _ => null
            };

            if (action == null) return;

            var turget = sender;
            while (turget is FrameworkElement element)
            {
                if (element is Window) break;
                turget = element.Parent;
            }


            if (turget is Window window)
                action(window);

        }



    }

    public enum WindowCommandType
    {
        CloseWindow,
        MaximizeWindow,
        MinimizeWindow,
        RestoreWindow,
        ShowSystemMenu,
    }
}
