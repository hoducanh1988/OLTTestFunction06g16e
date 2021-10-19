using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFunctionOlt60g16e.Models {
    public class MainWindowModel : INotifyPropertyChanged {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public MainWindowModel() {
            Opacity = 1;
        }

        double _opacity;
        public double Opacity {
            get { return _opacity; }
            set {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }

    }
}
