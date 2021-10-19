using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFunctionOlt60g16e.Models {
    public class SettingModel : INotifyPropertyChanged {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public SettingModel() {
            oltIP = "192.168.100.2";
            oltTelnetPort = 23;
            oltUser = "admin";
            oltPassword = "admin";

            firmwareFile = "";
            firmwareVersion = "";

            configGEFile = "";
            configXGEFile = "";

            teleATTFile = "";
            scriptTestTrafficGE = "";
            scriptTestTrafficXGE = "";
            timeoutTestTraffic = 90;
            timeoutOltReboot = 90;
            delayOpenTeleATT = 3;


            isUpgradeFirmware = true;
            isConfigGE = true;
            isRestoreGE = true;
            isTestTrafficPon1234Uplink1234 = true;
            isTestTrafficPon5678Uplink1234 = true;
            isTestTrafficPon9101112Uplink5678 = true;
            isTestTrafficPon13141516Uplink5678 = true;
            isConfigXGE = true;
            isRestoreXGE = true;
            isTestTrafficXGE = true;
            isFactoryReset = true;
        }

        bool _is_config_xge;
        public bool isConfigXGE {
            get { return _is_config_xge; }
            set {
                _is_config_xge = value;
                OnPropertyChanged(nameof(isConfigXGE));
            }
        }
        bool _is_restore_xge;
        public bool isRestoreXGE {
            get { return _is_restore_xge; }
            set {
                _is_restore_xge = value;
                OnPropertyChanged(nameof(isRestoreXGE));
            }
        }
        bool _is_testtraffic_xge;
        public bool isTestTrafficXGE {
            get { return _is_testtraffic_xge; }
            set {
                _is_testtraffic_xge = value;
                OnPropertyChanged(nameof(isTestTrafficXGE));
            }
        }
        bool _is_factory_reset;
        public bool isFactoryReset {
            get { return _is_factory_reset; }
            set {
                _is_factory_reset = value;
                OnPropertyChanged(nameof(isFactoryReset));
            }
        }
        bool _is_testtraffic_pon5678_uplink1234;
        public bool isTestTrafficPon5678Uplink1234 {
            get { return _is_testtraffic_pon5678_uplink1234; }
            set {
                _is_testtraffic_pon5678_uplink1234 = value;
                OnPropertyChanged(nameof(isTestTrafficPon5678Uplink1234));
            }
        }
        bool _is_testtraffic_pon9101112_uplink5678;
        public bool isTestTrafficPon9101112Uplink5678 {
            get { return _is_testtraffic_pon9101112_uplink5678; }
            set {
                _is_testtraffic_pon9101112_uplink5678 = value;
                OnPropertyChanged(nameof(isTestTrafficPon9101112Uplink5678));
            }
        }
        bool _is_testtraffic_pon13141516_uplink5678;
        public bool isTestTrafficPon13141516Uplink5678 {
            get { return _is_testtraffic_pon13141516_uplink5678; }
            set {
                _is_testtraffic_pon13141516_uplink5678 = value;
                OnPropertyChanged(nameof(isTestTrafficPon13141516Uplink5678));
            }
        }
        bool _is_testtraffic_pon1234_uplink1234;
        public bool isTestTrafficPon1234Uplink1234 {
            get { return _is_testtraffic_pon1234_uplink1234; }
            set {
                _is_testtraffic_pon1234_uplink1234 = value;
                OnPropertyChanged(nameof(isTestTrafficPon1234Uplink1234));
            }
        }
        bool _is_upgrade_firmware;
        public bool isUpgradeFirmware {
            get { return _is_upgrade_firmware; }
            set {
                _is_upgrade_firmware = value;
                OnPropertyChanged(nameof(isUpgradeFirmware));
            }
        }
        bool _is_config_ge;
        public bool isConfigGE {
            get { return _is_config_ge; }
            set {
                _is_config_ge = value;
                OnPropertyChanged(nameof(isConfigGE));
            }
        }
        bool _is_restore_ge;
        public bool isRestoreGE {
            get { return _is_restore_ge; }
            set {
                _is_restore_ge = value;
                OnPropertyChanged(nameof(isRestoreGE));
            }
        }
        int _delay_open_teleatt;
        public int delayOpenTeleATT {
            get { return _delay_open_teleatt; }
            set {
                _delay_open_teleatt = value;
                OnPropertyChanged(nameof(delayOpenTeleATT));
            }
        }
        int _timeout_olt_reboot;
        public int timeoutOltReboot {
            get { return _timeout_olt_reboot; }
            set {
                _timeout_olt_reboot = value;
                OnPropertyChanged(nameof(timeoutOltReboot));
            }
        }
        string _olt_ip;
        public string oltIP {
            get { return _olt_ip; }
            set {
                _olt_ip = value;
                OnPropertyChanged(nameof(oltIP));
            }
        }
        int _olt_telnet_port;
        public int oltTelnetPort {
            get { return _olt_telnet_port; }
            set {
                _olt_telnet_port = value;
                OnPropertyChanged(nameof(oltTelnetPort));
            }
        }
        string _olt_user;
        public string oltUser {
            get { return _olt_user; }
            set {
                _olt_user = value;
                OnPropertyChanged(nameof(oltUser));
            }
        }
        string _olt_password;
        public string oltPassword {
            get { return _olt_password; }
            set {
                _olt_password = value;
                OnPropertyChanged(nameof(oltPassword));
            }
        }
        string _firmware_file;
        public string firmwareFile {
            get { return _firmware_file; }
            set {
                _firmware_file = value;
                OnPropertyChanged(nameof(firmwareFile));
            }
        }
        string _firmware_version;
        public string firmwareVersion {
            get { return _firmware_version; }
            set {
                _firmware_version = value;
                OnPropertyChanged(nameof(firmwareVersion));
            }
        }
        string _config_ge_file;
        public string configGEFile {
            get { return _config_ge_file; }
            set {
                _config_ge_file = value;
                OnPropertyChanged(nameof(configGEFile));
            }
        }
        string _config_xge_file;
        public string configXGEFile {
            get { return _config_xge_file; }
            set {
                _config_xge_file = value;
                OnPropertyChanged(nameof(configXGEFile));
            }
        }
        string _teleatt_file;
        public string teleATTFile {
            get { return _teleatt_file; }
            set {
                _teleatt_file = value;
                OnPropertyChanged(nameof(teleATTFile));
            }
        }
        string _script_test_traffic_ge;
        public string scriptTestTrafficGE {
            get { return _script_test_traffic_ge; }
            set {
                _script_test_traffic_ge = value;
                OnPropertyChanged(nameof(scriptTestTrafficGE));
            }
        }
        string _script_test_traffic_xge;
        public string scriptTestTrafficXGE {
            get { return _script_test_traffic_xge; }
            set {
                _script_test_traffic_xge = value;
                OnPropertyChanged(nameof(scriptTestTrafficXGE));
            }
        }
        int _timeout_test_traffic;
        public int timeoutTestTraffic {
            get { return _timeout_test_traffic; }
            set {
                _timeout_test_traffic = value;
                OnPropertyChanged(nameof(timeoutTestTraffic));
            }
        }
    }
}
