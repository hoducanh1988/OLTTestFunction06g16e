using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestFunctionOlt60g16e.Commands;
using TestFunctionOlt60g16e.Models;

namespace TestFunctionOlt60g16e.ViewModels {
    public class LogViewModel {

        public LogViewModel() {
            _log = new LogModel();

            //set command
            GoCommand = new LogGoCommand(this);
        }

        //binding setting name
        LogModel _log;
        public LogModel Log {
            get { return _log; }
        }
        //command
        public ICommand GoCommand {
            get;
            private set;
        }

    }
}
