using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace BrainfuckIDE
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        //private void Window_Minimize(object sender, ExecutedRoutedEventArgs e) 
        //    => SystemCommands.MinimizeWindow(this);

        //private void Window_Close(object sender, ExecutedRoutedEventArgs e)
        //{
        //    SystemCommands.CloseWindow(this);
        //}

        //private void Window_Restore(object sender, ExecutedRoutedEventArgs e) => SystemCommands.RestoreWindow(this);

        //private void Window_Maximize(object sender, ExecutedRoutedEventArgs e) => SystemCommands.MaximizeWindow(this);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] Commands = System.Environment.GetCommandLineArgs();
            var files = Commands.Skip(1).Where(p => System.IO.File.Exists(p)).ToArray();
            if (files.Any())
            {
                LoadFile(files.First());
            }
            else
            {
                var current = Process.GetCurrentProcess();
                if(Process.GetProcessesByName(current.ProcessName).Length == 1)
                {
                    var tempFiles = Filer.FileSaverAndLoader.GetTemporaryFiles();
                    if (tempFiles.Any())
                    {
                        var res = MessageBox.Show("正しく終了されなかったファイルがありますが開きますか？", "", MessageBoxButton.YesNo);
                        if(res == MessageBoxResult.Yes)
                        {
                            LoadFile(tempFiles.First());
                            foreach (var item in tempFiles.Skip(1))
                            {
                                Process.Start(current.MainModule.FileName, item);
                            }
                        }
                    }
                }
            }


            void LoadFile(string path)
            {
                GetDataContext()?.EditrVM.Load(path);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var dataContext = GetDataContext();
            if (!dataContext.IsFileSaved)
            {
                var res = MessageBox.Show("ファイルが保存されていません。保存しますか？", "", MessageBoxButton.YesNoCancel);

                if (res == MessageBoxResult.Yes)
                    dataContext.Save();
                else if (res == MessageBoxResult.No)
                    dataContext.EditrVM.FileSaverViewModel.ForceDeleteTemporaryFile();
                else if (res == MessageBoxResult.Cancel)
                    e.Cancel = true;
            }
        }

        private Controls.ViewModels.InterpreterRunningViewModel GetDataContext()
        {
            return (this.DataContext as Controls.ViewModels.InterpreterRunningViewModel);
        }
    }

}
