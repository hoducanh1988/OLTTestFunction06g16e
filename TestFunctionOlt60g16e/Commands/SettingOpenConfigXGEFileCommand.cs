using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestFunctionOlt60g16e.ViewModels;

namespace TestFunctionOlt60g16e.Commands {
    public class SettingOpenConfigXGEFileCommand : ICommand {

        private SettingViewModel _svm;
        public SettingOpenConfigXGEFileCommand(SettingViewModel svm) {
            _svm = svm;
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
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "*.conf|*.conf";
            if (dlg.ShowDialog() == true) {
                _svm.Setting.configXGEFile = dlg.FileName;
            }
        }

        #endregion

    }
}
