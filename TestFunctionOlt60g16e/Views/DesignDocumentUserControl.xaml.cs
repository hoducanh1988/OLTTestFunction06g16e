using System;
using System.Collections.Generic;
using System.IO;
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

namespace TestFunctionOlt60g16e.Views {
    /// <summary>
    /// Interaction logic for DesignDocumentUserControl.xaml
    /// </summary>
    public partial class DesignDocumentUserControl : UserControl {
        public DesignDocumentUserControl() {
            InitializeComponent();
            var dd = new DesignDocumentViewModel();
            string srs_file = dd.Design.srsFile;
            if (File.Exists(srs_file)) {
                XpsDocument xpsDocument = new XpsDocument(srs_file, System.IO.FileAccess.Read);
                FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                docViewer.Document = fds;
            }
        }
    }
}
