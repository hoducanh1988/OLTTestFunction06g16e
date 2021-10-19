using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFunctionOlt60g16e.Models {

    public class DesignDocumentModel : INotifyPropertyChanged {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public DesignDocumentModel() {
            srsFile = AppDomain.CurrentDomain.BaseDirectory + "SRS.xps";
        }

        string _srs_file;
        public string srsFile {
            get { return _srs_file; }
            set {
                _srs_file = value;
                OnPropertyChanged(nameof(srsFile));
            }
        }
    }
}
