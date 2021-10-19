using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TestFunctionOlt60g16e.Protocol;

namespace TestFunctionOlt60g16e.Functions {

    public class OLT<T, S> where T : class, new() where S : class, new() {

        Telnet<T> olt = null;
        T testing = null;
        S setting = null;
        bool isConnected = false;

        public OLT(T t, S s) {
            this.testing = t;
            this.setting = s;
        }

        public bool Login() {
            bool r = false;
            string data = "";
            try {
                string ip = (string)setting.GetType().GetProperty("oltIP").GetValue(setting, null);
                int port = (int)setting.GetType().GetProperty("oltTelnetPort").GetValue(setting, null);
                string user = (string)setting.GetType().GetProperty("oltUser").GetValue(setting, null);
                string pass = (string)setting.GetType().GetProperty("oltPassword").GetValue(setting, null);

                //kiểm tra tham số setting có hợp lệ hay không
                r = string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port.ToString()) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass);
                if (r == true) return false;

                //mở cổng COM OLT
                olt = new Telnet<T>(ip, port, this.testing);
                isConnected = olt.Open();
                if (!isConnected) goto END;

                //gửi lệnh enter
                r = false;
                int status = 0;
                data = olt.Query("", 100);
                switch (data.ToLower()) {
                    case string a when a.Contains("password:"): { status = olt.Query("", 3, "username:") ? 0 : -1; break; }
                    case string b when b.Contains("username:"): { break; }
                    case string c when c.Contains("gpon>:"): { status = 1; break; }
                    default: { status = -1; break; }
                }
                if (status == -1) goto END;
                if (status == 1) { r = true; goto END; }

                //gui thong tin user name
                status = olt.Query(user, 3, "password:") ? 0 : -1;
                if (status == -1) goto END;

                //gui thong tin password
                status = olt.Query(pass, 3, "gpon>") ? 0 : -1;
                if (status == -1) goto END;

                //gui thong tin enable
                status = olt.Query("enable", 3, "gpon#") ? 1 : -1;
                r = status == 1;
                goto END;
            }
            catch { goto END; }

        END:
            //đóng cổng COM OLT
            if (olt != null && isConnected == true && r == false) olt.Close();
            return r;
        }

        public bool Close() {
            if (olt != null && isConnected == true) olt.Close();
            return true;
        }

        public string getFirmwareVersion() {
            if (!isConnected) return null;
            string data = olt.Query("show version", 1, 1, "svn                 :").ToLower();
            if (data.Contains("svn                 :") && data.Contains("\n") == true) {
                string[] buffer = data.Split('\n');
                string fw_ver = "";
                foreach (var b in buffer) {
                    if (b.Contains("svn")) {
                        fw_ver = b.Split(new string[] { ":" }, StringSplitOptions.None)[1];
                        break;
                    }
                }
                return fw_ver.Replace("\n", "").Replace("\r", "").Trim();
            }
            else return null;
        }

        public bool downloadFirmware() {
            if (!isConnected) return false;

            //get ip address from local pc
            string[] buffer = Utility.getLocalIP();
            string ip = buffer.Where(x => x.Contains("192.168.100.")).FirstOrDefault();
            if (string.IsNullOrEmpty(ip) || string.IsNullOrWhiteSpace(ip)) return false;

            //get firmware file
            string fp = (string)setting.GetType().GetProperty("firmwareFile").GetValue(setting, null);
            if (fp.Contains("\\") == false) return false;
            buffer = fp.Split(new string[] { "\\" }, StringSplitOptions.None);
            string fn = buffer[buffer.Length - 1];
            if (string.IsNullOrEmpty(fn) || string.IsNullOrWhiteSpace(fn)) return false;


            //download config file
            string data = olt.Query($"upgrade tftp {ip} {fn}", 30, 1, "GPON#");
            bool r = data.Contains("Finished, Please reboot to use new system.");

            return r;
        }

        public bool Reboot() {
            if (!isConnected) return false;
            string data = olt.Query("reboot", 1, 1, "Are you sure reboot [y/n]");
            bool r = data.Contains("Are you sure reboot [y/n]");
            if (!r) return false;

            data = olt.Query("y", 1, 1, "Unsaved configuration will be lost. Save now? [y/n]");
            r = data.Contains("Unsaved configuration will be lost. Save now? [y/n]");
            if (!r) return false;

            //data = olt.Query("y", 1, 1, "Rebooting system...");
            data = olt.Query("n", 1, 1, "Rebooting system...");
            r = data.Contains("Rebooting system...");

            return r;
        }

        public bool factoryReset() {
            if (!isConnected) return false;
            bool r = false;
            
            string data = olt.Query("reboot config default", 10, 1, "Are you sure reboot [y/n]");
            r = data.Contains("Are you sure reboot [y/n]");
            if (!r) return false;

            data = olt.Query("y", 10, 3, "Rebooting system by config default...");
            r = data.Contains("Rebooting system by config default...");
            return r;
        }

        public string getGEUplinkMode() {
            if (!isConnected) return null;
            string data = olt.Query("show running interface ge1/1", 3, 1, "!");
            return data;
        }

        public string getXGEUplinkMode() {
            if (!isConnected) return null;
            string data = olt.Query("show running interface xe1/1", 3, 1, "!");
            return data;
        }

        public bool downloadConfig() {
            if (!isConnected) return false;

            //get ip address from local pc
            string[] buffer = Utility.getLocalIP();
            string ip = buffer.Where(x => x.Contains("192.168.100.")).FirstOrDefault();
            if (string.IsNullOrEmpty(ip) || string.IsNullOrWhiteSpace(ip)) return false;

            //download config file
            string data = olt.Query($"tftp config get {ip}", 10, 1, "GPON#");
            bool r = data.Contains("success!!!");

            return r;
        }

        public bool setGEMode() {
            if (!isConnected) return false;
            bool r = false;

            string data = olt.Query("configure terminal", 3, 1, "GPON(config)#");
            r = data.Contains("GPON(config)#");
            if (!r) return false;

            data = olt.Query("uplink-mode ge", 20, 1, "Are you sure to set uplink mode[y/n]");
            r = data.Contains("Are you sure to set uplink mode[y/n]");
            if (!r) return false;
            if (data.Contains("GPON(config)#")) return true;

            data = olt.Query("y", 20, 1, "Unsaved configuration will be lost. Save now? [y/n]");
            r = data.Contains("Unsaved configuration will be lost. Save now? [y/n]");
            if (!r) return false;

            data = olt.Query("y", 10, 1, "Rebooting system...");
            r = data.Contains("Rebooting system...");

            return r;
        }

        public bool setXGEMode() {
            if (!isConnected) return false;
            bool r = false;

            string data = olt.Query("configure terminal", 3, 1, "GPON(config)#");
            r = data.Contains("GPON(config)#");
            if (!r) return false;

            data = olt.Query("uplink-mode xe", 20, 1, "Are you sure to set uplink mode[y/n]");
            r = data.Contains("Are you sure to set uplink mode[y/n]");
            if (!r) return false;
            if (data.Contains("GPON(config)#")) return true;

            data = olt.Query("y", 20, 1, "Unsaved configuration will be lost. Save now? [y/n]");
            r = data.Contains("Unsaved configuration will be lost. Save now? [y/n]");
            if (!r) return false;

            data = olt.Query("y", 10, 1, "[OK]");
            r = data.Contains("[OK]");

            return r;
        }

        public bool setFanLevel() {
            if (!isConnected) return false;
            bool r = false;

            string data = olt.Query("configure terminal", 3, 1, "GPON(config)#");
            r = data.Contains("GPON(config)#");
            if (!r) return false;

            //fan speed-level 1
            data = olt.Query("fan speed-level 1", 3, 1, "GPON(config)#");
            r = data.Contains("GPON(config)#");

            return r;
        }

    }
}
