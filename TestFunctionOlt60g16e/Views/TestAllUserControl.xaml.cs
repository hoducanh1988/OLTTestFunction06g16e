using System;
using System.Windows.Controls;
using TestFunctionOlt60g16e.Functions;
using System.Threading;

namespace TestFunctionOlt60g16e.Views {
    /// <summary>
    /// Interaction logic for TestAllUserControl.xaml
    /// </summary>
    public partial class TestAllUserControl : UserControl {


        public TestAllUserControl() {
            InitializeComponent();
            this.DataContext = myGlobal.myTesting;

            Thread t = new Thread(new ThreadStart(() => { 
                RE:
                if (myGlobal.myTesting.Testing.totalResult == "Waiting...") {
                    App.Current.Dispatcher.Invoke(new Action(() => {
                        this.scrl_logsystem.ScrollToEnd();
                        this.scrl_logtelnet.ScrollToEnd();
                    }));
                }
                Thread.Sleep(1000);
                goto RE;
            }));
            t.IsBackground = true;
            t.Start();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length == 0) tb.Focus();
        }
    }
}
