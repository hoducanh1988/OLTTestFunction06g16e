using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestFunctionOlt60g16e.Commands;
using TestFunctionOlt60g16e.Models;
using TestFunctionOlt60g16e.Functions;

namespace TestFunctionOlt60g16e.ViewModels {
    public class TestingViewModel {

        public TestingViewModel() {
            _testing = new TestingModel();

            //set command
            StartCommand = new TestingStartCommand(this);
        }

        //binding testing
        TestingModel _testing;
        public TestingModel Testing {
            get { return _testing; }
        }

        //binding setting
        public SettingModel Setting {
            get { return myGlobal.mySetting.Setting; }
        }

        //command
        public ICommand StartCommand {
            get;
            private set;
        }
    }
}
