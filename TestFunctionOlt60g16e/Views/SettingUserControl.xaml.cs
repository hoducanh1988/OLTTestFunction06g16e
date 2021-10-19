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
using TestFunctionOlt60g16e.ViewModels;
using TestFunctionOlt60g16e.Functions;

namespace TestFunctionOlt60g16e.Views {
    /// <summary>
    /// Interaction logic for SettingUserControl.xaml
    /// </summary>
    public partial class SettingUserControl : UserControl {
        public SettingUserControl() {
            InitializeComponent();
            this.DataContext =  myGlobal.mySetting;
        }
    }
}
