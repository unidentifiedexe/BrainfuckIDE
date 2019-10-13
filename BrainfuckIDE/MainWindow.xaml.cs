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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] Commands = Environment.GetCommandLineArgs();
            var files = Commands.Skip(1).Where(p => File.Exists(p)).ToArray();
            if (files.Any())
            {
                LoadFiles(files);
            }
            else
            {
                var tempFiles = Filer.TemporaryFileMaker.GetUnManagedTemporaryFiles();
                if (tempFiles.Any())
                {
                    var res = MessageBox.Show("正しく終了されなかったファイルがありますが開きますか？", "", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        LoadFiles(tempFiles);
                    }
                }
            }


            void LoadFiles(IEnumerable<string> pathes)
            {
                bool isFirst = true;
                foreach (var path in pathes)
                {
                    if (isFirst)
                    {
                        GetDataContext()?.EditrVM.Load(path);
                        isFirst = false;
                    }
                    else
                    {
                        Process.Start(Filer.LocalEnvironmental.CurrentProcess.MainModule.FileName, path);
                    }
                }
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
