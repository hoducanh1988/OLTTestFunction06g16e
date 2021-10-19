using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFunctionOlt60g16e.Models;

namespace TestFunctionOlt60g16e.ViewModels {
    public class MainWindowViewModel {

        public MainWindowViewModel() {
            _mainview = new MainWindowModel();
        }

        //binding setting name
        MainWindowModel _mainview;
        public MainWindowModel MainView {
            get { return _mainview; }
        }

    }
}
