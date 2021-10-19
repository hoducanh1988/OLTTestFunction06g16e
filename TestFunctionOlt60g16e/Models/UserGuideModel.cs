using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFunctionOlt60g16e.Models {
    public class UserGuideModel : INotifyPropertyChanged {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public UserGuideModel() {
            helpFile = AppDomain.CurrentDomain.BaseDirectory + "Help.xps";
        }

        string _help_file;
        public string helpFile {
            get { return _help_file; }
            set {
                _help_file = value;
                OnPropertyChanged(nameof(helpFile));
            }
        }


    }
}
