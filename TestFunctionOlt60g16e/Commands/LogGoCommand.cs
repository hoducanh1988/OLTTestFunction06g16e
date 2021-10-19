using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestFunctionOlt60g16e.ViewModels;
using System.Diagnostics;

namespace TestFunctionOlt60g16e.Commands {

    public class LogGoCommand : ICommand {
        private LogViewModel _lvm;
        public LogGoCommand(LogViewModel lvm) {
            _lvm = lvm;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        //enable button save setting
        public bool CanExecute(object parameter) {
            return true;
        }

        //save setting
        public void Execute(object parameter) {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }

        #endregion

    }
}
