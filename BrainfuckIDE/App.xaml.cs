using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BrainfuckIDE
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        static public void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
