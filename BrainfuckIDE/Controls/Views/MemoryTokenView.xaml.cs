using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrainfuckIDE.Controls.Views
{
    /// <summary>
    /// MemoryTokenView.xaml の相互作用ロジック
    /// </summary>
    public partial class MemoryTokenView : UserControl
    {
        public MemoryTokenView()
        {
            InitializeComponent();
        }

        public bool IsReadOnly
        {
            get => _mainTextBox.IsReadOnly;
            set => _mainTextBox.IsReadOnly = value;
        }
    }
}
