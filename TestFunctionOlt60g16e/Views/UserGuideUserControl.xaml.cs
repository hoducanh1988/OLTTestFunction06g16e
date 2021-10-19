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
using System.Windows.Xps.Packaging;
using TestFunctionOlt60g16e.ViewModels;
using System.IO;

namespace TestFunctionOlt60g16e.Views {
    /// <summary>
    /// Interaction logic for UserGuideUserControl.xaml
    /// </summary>
    public partial class UserGuideUserControl : UserControl {
        public UserGuideUserControl() {
            InitializeComponent();
            var ug = new UserGuideViewModel();
            string help_file = ug.Guide.helpFile;
            if (File.Exists(help_file)) {
                XpsDocument xpsDocument = new XpsDocument(help_file, System.IO.FileAccess.Read);
                FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                docViewer.Document = fds;
            }
            
        }
    }
}
