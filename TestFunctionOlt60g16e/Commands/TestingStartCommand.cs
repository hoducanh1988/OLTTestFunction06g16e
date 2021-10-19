using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using TestFunctionOlt60g16e.ViewModels;
using TestFunctionOlt60g16e.Functions;
using TestFunctionOlt60g16e.Models;
using System.Windows.Threading;
using UtilityPack.Converter;

namespace TestFunctionOlt60g16e.Commands {
    public class TestingStartCommand : ICommand {

        private TestingViewModel _tvm;
        public TestingStartCommand(TestingViewModel tvm) {
            _tvm = tvm;
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
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            int count = 0;
            timer.Tick += (s, e) => {
                count++;
                _tvm.Testing.totalTime = myConverter.intToTimeSpan(count * 500);
            };

            Thread t = new Thread(new ThreadStart(() => {
                timer.Start();
                Excute ex = new Excute();
                bool r = ex.RunTest();
                timer.Stop();
            }));
            t.IsBackground = true;
            t.Start();

        }

        #endregion

    }
}
