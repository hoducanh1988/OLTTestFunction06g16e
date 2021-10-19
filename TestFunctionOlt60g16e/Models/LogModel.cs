using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFunctionOlt60g16e.Models {
    public class LogModel : INotifyPropertyChanged {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public LogModel() {
            isSelectLogFile = false;
            isSelectSettingFile = false;
            isSelectUserGuide = false;
            isSelectDesignDocument = false;
        }

        bool _is_select_log_file;
        public bool isSelectLogFile {
            get { return _is_select_log_file; }
            set {
                _is_select_log_file = value;
                OnPropertyChanged(nameof(isSelectLogFile));
            }
        }
        bool _is_select_setting_file;
        public bool isSelectSettingFile {
            get { return _is_select_setting_file; }
            set {
                _is_select_setting_file = value;
                OnPropertyChanged(nameof(isSelectSettingFile));
            }
        }
        bool _is_select_user_guide;
        public bool isSelectUserGuide {
            get { return _is_select_user_guide; }
            set {
                _is_select_user_guide = value;
                OnPropertyChanged(nameof(isSelectUserGuide));
            }
        }
        bool _is_select_design_document;
        public bool isSelectDesignDocument {
            get { return _is_select_design_document; }
            set {
                _is_select_design_document = value;
                OnPropertyChanged(nameof(isSelectDesignDocument));
            }
        }
    }
}
