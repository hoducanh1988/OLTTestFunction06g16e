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
using TestFunctionOlt60g16e.Functions;
using TestFunctionOlt60g16e.ViewModels;
using TestFunctionOlt60g16e.Views;

namespace TestFunctionOlt60g16e {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        #region variable

        TestAllUserControl ta = new TestAllUserControl();
        TestEachUserControl te = new TestEachUserControl();
        SettingUserControl se = new SettingUserControl();
        QueryLogUserControl ql = new QueryLogUserControl();
        UserGuideUserControl ug = new UserGuideUserControl();
        DesignDocumentUserControl dd = new DesignDocumentUserControl();
        AboutUserControl au = new AboutUserControl();

        #endregion

        public MainWindow() {
            InitializeComponent();
            this.grid_main.Children.Clear();
            this.grid_main.Children.Add(ta);
            this.DataContext = myGlobal.myMainView;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                string b_tag = (sender as Border).Tag == null ? "" : (sender as Border).Tag.ToString();
                switch (b_tag) {
                    case "dragmove": { this.DragMove(); break; }
                    case "minimize": { this.WindowState = WindowState.Minimized; break; }
                    case "maximize": {
                            if (this.WindowState == WindowState.Normal) this.WindowState = WindowState.Maximized;
                            else this.WindowState = WindowState.Normal;
                            break; 
                        }
                    case "close": { this.Close(); break; }
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                //set font weight label 
                foreach (var gd in this.spControl.Children) {
                    if (gd is Grid) {
                        foreach (var lb in (gd as Grid).Children) {
                            if (lb is Label) {
                                (lb as Label).FontWeight = FontWeights.Normal;
                                break;
                            }
                        }
                    }
                }
                foreach (var lb in (sender as Grid).Children) {
                    if (lb is Label) {
                        (lb as Label).FontWeight = FontWeights.SemiBold;
                        break;
                    }
                }

                //add user control
                this.grid_main.Children.Clear();
                switch ((sender as Grid).Tag.ToString().ToLower()) {
                    case "all": { this.grid_main.Children.Add(ta); break; }
                    case "each": { this.grid_main.Children.Add(te); break; }
                    case "setting": { this.grid_main.Children.Add(se); break; }
                    case "log": { this.grid_main.Children.Add(ql); break; }
                    case "help": { this.grid_main.Children.Add(ug); break; }
                    case "design": { this.grid_main.Children.Add(dd); break; }
                    case "about": { this.grid_main.Children.Add(au); break; }
                }
            }
        }
    }
}
