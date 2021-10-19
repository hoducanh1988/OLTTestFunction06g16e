using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestFunctionOlt60g16e.Models {
    public class TestingModel : INotifyPropertyChanged {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public TestingModel() {
            Init();
        }

        public void Init() {
            macAddress = "";
            ID = "";
            enableTextBox = true;

            upgradeResult = configGEResult = configXGEResult = restoreGEResult = restoreXGEResult = pon1234uplink1234Result = pon5678uplink1234Result = pon9101112uplink5678Result = pon13141516uplink5678Result = trafficResult = factoryResetResult = totalResult = "-";
            totalTime = "00:00:00";
            loadStatistic();
            logSystem = logUart = logTelnet = "";
        }

        public void Clear() {
            ID = macAddress;
            enableTextBox = false;
            upgradeResult = configGEResult = configXGEResult = restoreGEResult = restoreXGEResult = pon1234uplink1234Result = pon5678uplink1234Result = pon9101112uplink5678Result = pon13141516uplink5678Result = trafficResult = factoryResetResult = totalResult = "-";
            totalTime = "00:00:00";
            logSystem = logUart = logTelnet = "";
        }

        public void saveStatistic() {
            string data = $"passCount={passCount}\n";
            data += $"failCount={failCount}\n";
            data += $"totalCount={totalCount}\n";
            data += $"errorRate={errorRate}\n";
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "log.txt", data);
        }

        public void loadStatistic() {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "log.txt") == true) {
                string[] buffer = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "log.txt");
                passCount = buffer[0].Split('=')[1];
                failCount = buffer[1].Split('=')[1];
                totalCount = buffer[2].Split('=')[1];
                errorRate = buffer[3].Split('=')[1];
            }
            else {
                passCount = failCount = totalCount = errorRate = "0";
            }
        }


        public void Waiting() {
            enableTextBox = false;
            totalResult = "Waiting...";
        }

        public bool Passed() {
            enableTextBox = true;
            totalResult = "Passed";
            macAddress = "";
            return true;
        }

        public bool Failed() {
            enableTextBox = true;
            totalResult = "Failed";
            macAddress = "";
            return true;
        }

        string _mac_address;
        public string macAddress {
            get { return _mac_address; }
            set {
                _mac_address = value;
                OnPropertyChanged(nameof(macAddress));
            }
        }
        bool _enable_textbox;
        public bool enableTextBox {
            get { return _enable_textbox; }
            set {
                _enable_textbox = value;
                OnPropertyChanged(nameof(enableTextBox));
            }
        }

        string _id;
        public string ID {
            get { return _id; }
            set {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        string _upgrade_result;
        public string upgradeResult {
            get { return _upgrade_result; }
            set {
                _upgrade_result = value;
                OnPropertyChanged(nameof(upgradeResult));
            }
        }
        string _config_ge_result;
        public string configGEResult {
            get { return _config_ge_result; }
            set {
                _config_ge_result = value;
                OnPropertyChanged(nameof(configGEResult));
            }
        }
        string _restore_ge_result;
        public string restoreGEResult {
            get { return _restore_ge_result; }
            set {
                _restore_ge_result = value;
                OnPropertyChanged(nameof(restoreGEResult));
            }
        }
        string _pon1234_uplink1234_result;
        public string pon1234uplink1234Result {
            get { return _pon1234_uplink1234_result; }
            set {
                _pon1234_uplink1234_result = value;
                OnPropertyChanged(nameof(pon1234uplink1234Result));
            }
        }
        string _pon5678_uplink1234_result;
        public string pon5678uplink1234Result {
            get { return _pon5678_uplink1234_result; }
            set {
                _pon5678_uplink1234_result = value;
                OnPropertyChanged(nameof(pon5678uplink1234Result));
            }
        }
        string _pon9101112_uplink5678_result;
        public string pon9101112uplink5678Result {
            get { return _pon9101112_uplink5678_result; }
            set {
                _pon9101112_uplink5678_result = value;
                OnPropertyChanged(nameof(pon9101112uplink5678Result));
            }
        }
        string _pon13141516_uplink5678_result;
        public string pon13141516uplink5678Result {
            get { return _pon13141516_uplink5678_result; }
            set {
                _pon13141516_uplink5678_result = value;
                OnPropertyChanged(nameof(pon13141516uplink5678Result));
            }
        }
        string _config_xge_result;
        public string configXGEResult {
            get { return _config_xge_result; }
            set {
                _config_xge_result = value;
                OnPropertyChanged(nameof(configXGEResult));
            }
        }
        string _restore_xge_result;
        public string restoreXGEResult {
            get { return _restore_xge_result; }
            set {
                _restore_xge_result = value;
                OnPropertyChanged(nameof(restoreXGEResult));
            }
        }
        string _traffic_result;
        public string trafficResult {
            get { return _traffic_result; }
            set {
                _traffic_result = value;
                OnPropertyChanged(nameof(trafficResult));
            }
        }
        string _factory_reset_result;
        public string factoryResetResult {
            get { return _factory_reset_result; }
            set {
                _factory_reset_result = value;
                OnPropertyChanged(nameof(factoryResetResult));
            }
        }
        string _total_result;
        public string totalResult {
            get { return _total_result; }
            set {
                _total_result = value;
                OnPropertyChanged(nameof(totalResult));
            }
        }
        string _total_time;
        public string totalTime {
            get { return _total_time; }
            set {
                _total_time = value;
                OnPropertyChanged(nameof(totalTime));
            }
        }
        string _pass_count;
        public string passCount {
            get { return _pass_count; }
            set {
                _pass_count = value;
                OnPropertyChanged(nameof(passCount));
            }
        }
        string _fail_count;
        public string failCount {
            get { return _fail_count; }
            set {
                _fail_count = value;
                OnPropertyChanged(nameof(failCount));
            }
        }
        string _total_count;
        public string totalCount {
            get { return _total_count; }
            set {
                _total_count = value;
                OnPropertyChanged(nameof(totalCount));
            }
        }
        string _error_rate;
        public string errorRate {
            get { return _error_rate; }
            set {
                _error_rate = value;
                OnPropertyChanged(nameof(errorRate));
            }
        }
        string _log_system;
        public string logSystem {
            get { return _log_system; }
            set {
                _log_system = value;
                OnPropertyChanged(nameof(logSystem));
            }
        }
        string _log_uart;
        public string logUart {
            get { return _log_uart; }
            set {
                _log_uart = value;
                OnPropertyChanged(nameof(logUart));
            }
        }
        string _log_telnet;
        public string logTelnet {
            get { return _log_telnet; }
            set {
                _log_telnet = value;
                OnPropertyChanged(nameof(logTelnet));
            }
        }
    }
}
