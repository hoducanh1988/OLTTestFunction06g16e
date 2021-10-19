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

namespace TestFunctionOlt60g16e.Views {
    /// <summary>
    /// Interaction logic for TestEachUserControl.xaml
    /// </summary>
    public partial class TestEachUserControl : UserControl {
        public TestEachUserControl() {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                MessageBox.Show("Function is being updated.", "Test Each", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
