using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TestFunctionOlt60g16e.Models;
using TestFunctionOlt60g16e.ViewModels;
using System.Diagnostics;
using System.IO;
using UtilityPack.IO;
using TestFunctionOlt60g16e.Views;

namespace TestFunctionOlt60g16e.Functions {

    public class Excute {

        SettingModel Setting = null;
        TestingModel Testing = null;
        string dir_log = $"{AppDomain.CurrentDomain.BaseDirectory}Log\\{DateTime.Now.ToString("yyyy-MM-dd")}";

        public Excute() {
            if (Directory.Exists(dir_log) == false) Directory.CreateDirectory(dir_log);
            Setting = myGlobal.mySetting.Setting;
            Testing = myGlobal.myTesting.Testing;
        }

        public bool RunTest() {
            bool r = true;
            try {
                Testing.Clear();
                Testing.Waiting();

                //upgrade firmware ---------------------------------//1
                if (Setting.isUpgradeFirmware) {
                    r = upgradeFirmware();
                    if (!r) goto END;
                }
                //config mode GE -----------------------------------//2
                if (Setting.isConfigGE) {
                    r = configUplinkModeGE();
                    if (!r) goto END;
                }
                //restore mode GE ----------------------------------//3
                if (Setting.isRestoreGE) {
                    r = restoreUplinkModeGE();
                    if (!r) goto END;
                }
                //test traffic pon 1-2-3-4 & uplink 1-2-3-4 --------//4
                if (Setting.isTestTrafficPon1234Uplink1234) {
                    r = testTrafficModeGE_pon1234_uplink1234();
                    if (!r) goto END;
                }
                //test traffic pon 5-6-7-8 & uplink 1-2-3-4 --------//5
                if (Setting.isTestTrafficPon5678Uplink1234) {
                    r = testTrafficModeGE_pon5678_uplink1234();
                    if (!r) goto END;
                }
                //test traffic pon 9-10-11-12 & uplink 5-6-7-8 -----//6
                if (Setting.isTestTrafficPon9101112Uplink5678) {
                    r = testTrafficModeGE_pon9101112_uplink5678();
                    if (!r) goto END;
                }
                //test traffic pon 13-14-15-16 & uplink 5-6-7-8 ----//7
                if (Setting.isTestTrafficPon13141516Uplink5678) {
                    r = testTrafficModeGE_pon13141516_uplink5678();
                    if (!r) goto END;
                }
                //config mode XGE ----------------------------------//8
                if (Setting.isConfigXGE) {
                    r = configUplinkModeXGE();
                    if (!r) goto END;
                }
                //restore mode XGE ---------------------------------//9
                if (Setting.isRestoreXGE) {
                    r = restoreUplinkModeXGE();
                    if (!r) goto END;
                }
                //test traffic -------------------------------------//10
                if (Setting.isTestTrafficXGE) {
                    r = testTrafficModeXGE();
                    if (!r) goto END;
                }
                //factory reset ------------------------------------//11
                if (Setting.isFactoryReset) {
                    r = factoryResetOlt();
                    if (!r) goto END;
                }

            }
            catch { r = false; goto END; }

        END:
            if (r) Testing.passCount = (int.Parse(Testing.passCount) + 1).ToString();
            else Testing.failCount = (int.Parse(Testing.failCount) + 1).ToString();
            Testing.totalCount = (int.Parse(Testing.totalCount) + 1).ToString();
            Testing.errorRate = ((int.Parse(Testing.failCount) * 100) / int.Parse(Testing.totalCount)).ToString();

            bool ___ = r ? Testing.Passed() : Testing.Failed();
            Testing.logSystem += "\n++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
            Testing.logSystem += $"> Total result: {Testing.totalResult}\n";
            Testing.logSystem += $"> Total time: {Testing.totalTime} Sec\n";
            Testing.saveStatistic();
            saveLogDetail();
            saveLogTotal();
            return r;
        }

        bool waitOltRebootComplete(int timeout_sec) {
            int count = 0;
            bool r = false;

        //wait dut can't ping
        RE_1:
            count++;
            Testing.logSystem += $"{count}..";
            Thread.Sleep(1000);
            r = Utility.pingNetwork(Setting.oltIP);
            if (r) {
                if (count < timeout_sec) goto RE_1;
                else return false;
            }

            //wait dut ping ok
            count = 0;
        RE_2:
            count++;
            Testing.logSystem += $"{count}..";
            Thread.Sleep(1000);
            r = Utility.pingNetwork(Setting.oltIP);
            if (!r) {
                if (count < timeout_sec) goto RE_2;
                else return false;
            }
            else {
                OLT<TestingModel, SettingModel> olt = null;
                r = this.loginToOlt(ref olt, 3);
                if (!r) {
                    if (count < timeout_sec) goto RE_2;
                    else return false;
                }
                olt.Close();
            }
            Testing.logSystem += "\n";
            return r;
        }

        bool loginToOlt(ref OLT<TestingModel, SettingModel> olt, int retry_time) {
            int count = 0;
            bool r = false;
            Testing.logSystem += $"...Login vào olt = ";
        RE:
            count++;
            olt = new OLT<TestingModel, SettingModel>(myGlobal.myTesting.Testing, Setting);
            r = olt.Login();
            Testing.logSystem += $"[{count}-{r}] ";
            if (!r) {
                if (count < retry_time) goto RE;
            }
            Testing.logSystem += "\n";
            return r;
        }

        bool changeTeleATTConfig(string script_test_traffic) {
            try {
                string cfg_file = "TeleATT.cfg";
                if (File.Exists(Setting.teleATTFile) == false) return false;
                string[] buffer = Setting.teleATTFile.Split('\\');
                string cfg_path = Setting.teleATTFile.Replace(buffer[buffer.Length - 1], cfg_file);

                Testing.logSystem += $"...{cfg_path}\n";
                Testing.logSystem += $"...{script_test_traffic}\n";

                buffer = File.ReadAllLines(cfg_path);
                List<string> listText = new List<string>();
                foreach (var b in buffer) {
                    if (b.Contains("<!--")) continue;
                    if (b.Contains("<AutoLoadEnabled>")) {
                        listText.Add("  <AutoLoadEnabled>True</AutoLoadEnabled>");
                    }
                    else if (b.Contains("<LoadFilePath>")) {
                        listText.Add($"  <LoadFilePath>{script_test_traffic}</LoadFilePath>");
                    }
                    else listText.Add(b);
                }
                File.WriteAllLines(cfg_path, listText.ToArray());
                return true;

            }
            catch { return false; }
        }

        bool openTeleATTexe() {
            try {
                Testing.logSystem += $"...{Setting.teleATTFile}\n";
                if (File.Exists(Setting.teleATTFile) == false) return false;
                Process.Start(Setting.teleATTFile);
                Thread.Sleep(Setting.delayOpenTeleATT * 1000);

                int count = 0;
                bool r = false;
            RE:
                count++;
                r = WindowProcess.isProcessRunning("teleatt");
                if (!r) {
                    if (count < 10) {
                        Thread.Sleep(1000);
                        goto RE;
                    }
                }

                return r;
            }
            catch { return false; }
        }

        bool closeTeleATTexe() {
            bool r = false;
            r = WindowProcess.isProcessRunning("teleatt");
            if (!r) return true;
            r = WindowProcess.killAllProcessByName("teleatt");

            int count = 0;
            r = false;
        RE:
            count++;
            r = WindowProcess.isProcessRunning("teleatt");
            if (r) {
                if (count < 5) {
                    Thread.Sleep(1000);
                    goto RE;
                }
            }

            return !r;
        }

        bool getTeleATTresult(int timeout_sec) {
            bool r = false;
            int count = 0, max = timeout_sec;
            int c = 0;

            try {
            RE:
                count++;
                var ws = WindowHandleHelper.GetOpenWindows();
                var w = ws.Where(x => x.Value.ToLower().Contains("teleatt") || x.Value.ToLower().Contains("version choose")).FirstOrDefault();
                r = w.Key == null || w.Value == null;
                if (r) {
                    c++;
                    if (c > 3) return false;
                    if (count < max) {
                        Thread.Sleep(1000);
                        goto RE;
                    }
                    else return false;
                }

                c = 0;
                if (w.Value.ToLower().Contains("version choose")) {
                    if (count < max) {
                        Thread.Sleep(1000);
                        goto RE;
                    }
                    else return false;
                }

                WindowHandleInfo wi = new WindowHandleInfo(w.Key);
                var handles = wi.GetAllChildHandles();
                string result = wi.GetWindowTitle(handles[8]);
                Testing.logSystem += $"...[{count}]{handles[8]}, {result}\n";
                r = result.ToLower().Contains("pass") || result.ToLower().Contains("fail");
                if (!r) {
                    if (count < max) {
                        Thread.Sleep(1000);
                        goto RE;
                    }
                    else return false;
                }

                return result.ToLower().Contains("pass");
            }
            catch { return false; }
        }

        bool upgradeFirmware() {
            bool r = false;
            OLT<TestingModel, SettingModel> olt = null;
            myGlobal.myTesting.Testing.upgradeResult = "Waiting...";
            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Nâng cấp firmware:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                //login -------------------------------------------//1
                r = this.loginToOlt(ref olt, 3);
                if (!r) goto END;

                //check firmware version
                string fw_ver = olt.getFirmwareVersion();
                Testing.logSystem += $"...Đọc version firmware, hiện tại: {fw_ver}\n";
                Testing.logSystem += $"...So sánh với tiêu chuẩn: {Setting.firmwareVersion}\n";
                r = Setting.firmwareVersion.ToUpper().Equals(fw_ver.ToUpper());
                Testing.logSystem += r ? "...Đúng thông tin version\n" : "...Sai thông tin version\n";
                if (r) goto END;

                //open tftpd64 app

                string fn = Setting.firmwareFile;
                if (fn.Contains("\\") == false) { r = false; goto END; }
                string[] buffer = fn.Split(new string[] { "\\" }, StringSplitOptions.None);
                string fp = fn.Replace(buffer[buffer.Length - 1], "");
                r = Utility.openTftpd64(fp);
                Testing.logSystem += $"...Mở file tftpd64: {fp}\n";

                //download firmware
                Testing.logSystem += $"...Download file firmware: {Setting.firmwareFile}: ";
                int count = 0;
            RE:
                count++;
                r = olt.downloadFirmware();
                Testing.logSystem += $"[{count}-{r}] ";
                if (!r) { if (count < 3) { Thread.Sleep(1000); goto RE; } }
                Testing.logSystem += "\n";
                if (!r) goto END;

                //close tftpd64 app
                r = Utility.closeTftpd64();
                Testing.logSystem += $"...Đóng file tftpd64\n";

                //reboot olt
                Testing.logSystem += $"...Khởi động lại OLT\n";
                r = olt.Reboot();
                if (!r) goto END;

                //wait olt reboot complete
                Testing.logSystem += $"...Chờ OLT khởi động xong\n";
                r = waitOltRebootComplete(Setting.timeoutOltReboot);

            }
            catch { r = false; goto END; }

        END:
            if (olt != null) olt.Close();
            Testing.logSystem += $"...Kết quả nâng cấp firmware : {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.upgradeResult = r ? "Passed" : "Failed";
            return r;

        }

        bool configUplinkModeGE() {
            bool r = false;
            OLT<TestingModel, SettingModel> olt = null;
            myGlobal.myTesting.Testing.configGEResult = "Waiting...";

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Cấu hình uplink mode GE:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                r = this.loginToOlt(ref olt, 3);
                if (!r) goto END;

                //check ge mode
                Testing.logSystem += $"...Đọc thông tin interface : ";
                string data = olt.getGEUplinkMode();
                Testing.logSystem += $"{data}\n";
                string[] buffer = data.Split(new string[] { "interface ge1/1" }, StringSplitOptions.None);
                r = buffer.Length >= 3;
                if (r) goto END;

                //chuyen sang mode GE
                Testing.logSystem += $"...Chuyển interface sang mode GE : ";
                r = olt.setGEMode();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //wait olt reboot complete
                Testing.logSystem += $"...Chờ OLT khởi động xong\n";
                r = waitOltRebootComplete(Setting.timeoutOltReboot);

            }
            catch {
                r = false;
                goto END;
            }

        END:
            if (olt != null) olt.Close();
            Testing.logSystem += $"...Kết quả cấu hình uplink mode GE : {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.configGEResult = r ? "Passed" : "Failed";
            return r;
        }

        bool restoreUplinkModeGE() {
            bool r = false;
            OLT<TestingModel, SettingModel> olt = null;
            myGlobal.myTesting.Testing.restoreGEResult = "Waiting...";

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Restore file cấu hình uplink mode GE:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                r = this.loginToOlt(ref olt, 3);
                if (!r) goto END;

                //open tftpd64 app
                string gn = Setting.configGEFile;
                if (gn.Contains("\\") == false) { r = false; goto END; }
                string[] buffer = gn.Split(new string[] { "\\" }, StringSplitOptions.None);
                string gp = gn.Replace(buffer[buffer.Length - 1], "");
                r = Utility.openTftpd64(gp);
                Testing.logSystem += $"...Mở file tftpd64: {gp}\n";

                //download ge config
                Testing.logSystem += $"...Download file config mode ge: {Setting.configGEFile}: ";
                int count = 0;
            RE:
                count++;
                r = olt.downloadConfig();
                Testing.logSystem += $"[{count}-{r}] ";
                if (!r) { if (count < 3) { Thread.Sleep(1000); goto RE; } }
                Testing.logSystem += "\n";
                if (!r) return false;

                //close tftpd64 app
                r = Utility.closeTftpd64();
                Testing.logSystem += $"...Đóng file tftpd64\n";

                //reboot olt
                Testing.logSystem += $"...Khởi động lại OLT\n";
                r = olt.Reboot();
                if (!r) goto END;

                //wait olt reboot complete
                Testing.logSystem += $"...Chờ OLT khởi động xong\n";
                r = waitOltRebootComplete(Setting.timeoutOltReboot);


            }
            catch { r = false; goto END; }

        END:
            if (olt != null) olt.Close();
            Testing.logSystem += $"...Kết quả restore file cấu hình mode GE : {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.restoreGEResult = r ? "Passed" : "Failed";
            return r;
        }

        bool configUplinkModeXGE() {
            bool r = false;
            OLT<TestingModel, SettingModel> olt = null;
            myGlobal.myTesting.Testing.configXGEResult = "Waiting...";

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Cấu hình uplink mode XGE:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                r = this.loginToOlt(ref olt, 3);
                if (!r) goto END;

                //check ge mode
                Testing.logSystem += $"...Đọc thông tin interface : ";
                string data = olt.getXGEUplinkMode();
                Testing.logSystem += $"{data}\n";
                string[] buffer = data.Split(new string[] { "interface xe1/1" }, StringSplitOptions.None);
                r = buffer.Length >= 3;
                if (r) goto END;

                //chuyen sang mode XGE
                Testing.logSystem += $"...Chuyển interface sang mode XGE : ";
                r = olt.setXGEMode();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //wait olt reboot complete
                Testing.logSystem += $"...Chờ OLT khởi động xong\n";
                r = waitOltRebootComplete(Setting.timeoutOltReboot);

            }
            catch {
                r = false;
                goto END;
            }

        END:
            if (olt != null) olt.Close();
            Testing.logSystem += $"...Kết quả cấu hình uplink mode XGE : {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.configXGEResult = r ? "Passed" : "Failed";
            return r;
        }

        bool restoreUplinkModeXGE() {
            bool r = false;
            OLT<TestingModel, SettingModel> olt = null;
            myGlobal.myTesting.Testing.restoreXGEResult = "Waiting...";

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Restore file cấu hình uplink mode XGE:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                r = this.loginToOlt(ref olt, 3);
                if (!r) goto END;

                //open tftpd64 app
                string xn = Setting.configXGEFile;
                if (xn.Contains("\\") == false) { r = false; goto END; }
                string[] buffer = xn.Split(new string[] { "\\" }, StringSplitOptions.None);
                string xp = xn.Replace(buffer[buffer.Length - 1], "");
                r = Utility.openTftpd64(xp);
                Testing.logSystem += $"...Mở file tftpd64: {xp}\n";

                //download ge config
                Testing.logSystem += $"...Download file config mode xge: {Setting.configXGEFile}: ";
                int count = 0;
            RE:
                count++;
                r = olt.downloadConfig();
                Testing.logSystem += $"[{count}-{r}] ";
                if (!r) { if (count < 3) { Thread.Sleep(1000); goto RE; } }
                Testing.logSystem += "\n";
                if (!r) return false;

                //close tftpd64 app
                Testing.logSystem += $"...Đóng file tftpd64\n";
                r = Utility.closeTftpd64();

                //reboot olt
                Testing.logSystem += $"...Khởi động lại OLT\n";
                r = olt.Reboot();
                if (!r) goto END;

                //wait olt reboot complete
                Testing.logSystem += $"...Chờ OLT khởi động xong\n";
                r = waitOltRebootComplete(Setting.timeoutOltReboot);


            }
            catch { r = false; goto END; }

        END:
            if (olt != null) olt.Close();
            Testing.logSystem += $"...Kết quả restore file cấu hình mode XGE : {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.restoreXGEResult = r ? "Passed" : "Failed";
            return r;
        }

        bool factoryResetOlt() {
            bool r = false;
            OLT<TestingModel, SettingModel> olt = null;
            myGlobal.myTesting.Testing.factoryResetResult = "Waiting...";

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Factory reset OLT:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                r = this.loginToOlt(ref olt, 3);
                if (!r) goto END;

                //factory reset olt
                Testing.logSystem += "...Factory reset OLT\n";
                r = olt.factoryReset();
                if (!r) goto END;

                //wait olt reboot complete
                Testing.logSystem += $"...Chờ OLT khởi động xong\n";
                r = waitOltRebootComplete(Setting.timeoutOltReboot);

            }
            catch { r = false; goto END; }

        END:
            if (olt != null) olt.Close();
            Testing.logSystem += $"...Kết quả factory reset olt : {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.factoryResetResult = r ? "Passed" : "Failed";
            return r;
        }

        bool testTrafficModeGE_pon1234_uplink1234() {
            bool r = false;
            myGlobal.myMainView.MainView.Opacity = 0.3;
            myGlobal.myTesting.Testing.pon1234uplink1234Result = "Waiting...";

            App.Current.Dispatcher.Invoke(new Action(() => {
                DialogTestTrafficPon1234Uplink1234 dlg = new DialogTestTrafficPon1234Uplink1234();
                dlg.ShowDialog();
            }));

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Test traffic mode GE pon port 1-2-3-4 và uplink port 1-2-3-4:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                //1. close tele att.exe
                Testing.logSystem += $"...Close file teleATT.exe: ";
                r = closeTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //2. change TeleATT.cfg
                Testing.logSystem += $"...Change file TeleATT.cfg: ";
                r = changeTeleATTConfig(Setting.scriptTestTrafficGE);
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //3. open teleATT.exe
                Testing.logSystem += $"...Reopen file TeleATT.exe: ";
                r = openTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //3. check result
                Testing.logSystem += $"...Check result test: \n";
                r = getTeleATTresult(Setting.timeoutTestTraffic);
                Testing.logSystem += $"{r}\n";
            }
            catch { r = false; goto END; }

        END:
            myGlobal.myMainView.MainView.Opacity = 1;
            closeTeleATTexe();
            Testing.logSystem += $"...Kết quả test traffic mode GE pon port 1-2-3-4 và uplink port 1-2-3-4: {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.pon1234uplink1234Result = r ? "Passed" : "Failed";
            return r;
        }

        bool testTrafficModeGE_pon5678_uplink1234() {
            bool r = false;
            myGlobal.myMainView.MainView.Opacity = 0.3;
            myGlobal.myTesting.Testing.pon5678uplink1234Result = "Waiting...";

            App.Current.Dispatcher.Invoke(new Action(() => {
                DialogTestTrafficPon5678Uplink1234 dlg = new DialogTestTrafficPon5678Uplink1234();
                dlg.ShowDialog();
            }));

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Test traffic mode GE pon port 5-6-7-8 và uplink port 1-2-3-4:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                //1. close tele att.exe
                Testing.logSystem += $"...Close file teleATT.exe: ";
                r = closeTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //2. change TeleATT.cfg
                Testing.logSystem += $"...Change file TeleATT.cfg: ";
                r = changeTeleATTConfig(Setting.scriptTestTrafficGE);
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //3. open teleATT.exe
                Testing.logSystem += $"...Reopen file TeleATT.exe: ";
                r = openTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //4. check result
                Testing.logSystem += $"...Check result test: \n";
                r = getTeleATTresult(Setting.timeoutTestTraffic);
                Testing.logSystem += $"{r}\n";
            }
            catch { r = false; goto END; }

        END:
            myGlobal.myMainView.MainView.Opacity = 1;
            Testing.logSystem += $"...Kết quả test traffic mode GE pon port 5-6-7-8 và uplink port 1-2-3-4: {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.pon5678uplink1234Result = r ? "Passed" : "Failed";
            closeTeleATTexe();
            return r;
        }

        bool testTrafficModeGE_pon9101112_uplink5678() {
            bool r = false;
            myGlobal.myMainView.MainView.Opacity = 0.3;
            myGlobal.myTesting.Testing.pon9101112uplink5678Result = "Waiting...";

            App.Current.Dispatcher.Invoke(new Action(() => {
                DialogTestTrafficPon9101112Uplink5678 dlg = new DialogTestTrafficPon9101112Uplink5678();
                dlg.ShowDialog();
            }));

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Test traffic mode GE pon port 9-10-11-12 và uplink port 5-6-7-8:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                //1. close tele att.exe
                Testing.logSystem += $"...Close file teleATT.exe: ";
                r = closeTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //2. change TeleATT.cfg
                Testing.logSystem += $"...Change file TeleATT.cfg: ";
                r = changeTeleATTConfig(Setting.scriptTestTrafficGE);
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //3. open teleATT.exe
                Testing.logSystem += $"...Reopen file TeleATT.exe: ";
                r = openTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //4. check result
                Testing.logSystem += $"...Check result test: \n";
                r = getTeleATTresult(Setting.timeoutTestTraffic);
                Testing.logSystem += $"{r}\n";
            }
            catch { r = false; goto END; }

        END:
            myGlobal.myMainView.MainView.Opacity = 1;
            Testing.logSystem += $"...Kết quả test traffic mode GE pon port 9-10-11-12 và uplink port 5-6-7-8: {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.pon9101112uplink5678Result = r ? "Passed" : "Failed";
            closeTeleATTexe();
            return r;
        }

        bool testTrafficModeGE_pon13141516_uplink5678() {
            bool r = false;
            myGlobal.myMainView.MainView.Opacity = 0.3;
            myGlobal.myTesting.Testing.pon13141516uplink5678Result = "Waiting...";

            App.Current.Dispatcher.Invoke(new Action(() => {
                DialogTestTrafficPon13141516Uplink5678 dlg = new DialogTestTrafficPon13141516Uplink5678();
                dlg.ShowDialog();
            }));

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Test traffic mode GE pon port 13-14-15-16 và uplink port 5-6-7-8:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                //1. close tele att.exe
                Testing.logSystem += $"...Close file teleATT.exe: ";
                r = closeTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //2. change TeleATT.cfg
                Testing.logSystem += $"...Change file TeleATT.cfg: ";
                r = changeTeleATTConfig(Setting.scriptTestTrafficGE);
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //3. open teleATT.exe
                Testing.logSystem += $"...Reopen file TeleATT.exe: ";
                r = openTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //4. check result
                Testing.logSystem += $"...Check result test: \n";
                r = getTeleATTresult(Setting.timeoutTestTraffic);
                Testing.logSystem += $"{r}\n";
            }
            catch { r = false; goto END; }

        END:
            myGlobal.myMainView.MainView.Opacity = 1;
            Testing.logSystem += $"...Kết quả test traffic mode GE pon port 13-14-15-16 và uplink port 5-6-7-8: {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.pon13141516uplink5678Result = r ? "Passed" : "Failed";
            closeTeleATTexe();
            return r;
        }

        bool testTrafficModeXGE() {
            bool r = false;
            myGlobal.myMainView.MainView.Opacity = 0.3;
            myGlobal.myTesting.Testing.trafficResult = "Waiting...";

            App.Current.Dispatcher.Invoke(new Action(() => {
                DialogTestTrafficXGE dlg = new DialogTestTrafficXGE();
                dlg.ShowDialog();
            }));

            Testing.logSystem += $">Date time: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss ffff")}\n";
            Testing.logSystem += ">Test traffic mode XGE:\n";
            Testing.logSystem += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            try {
                //1. close tele att.exe
                Testing.logSystem += $"...Close file teleATT.exe: ";
                r = closeTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //2. change TeleATT.cfg
                Testing.logSystem += $"...Change file TeleATT.cfg: ";
                r = changeTeleATTConfig(Setting.scriptTestTrafficXGE);
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //3. open teleATT.exe
                Testing.logSystem += $"...Reopen file TeleATT.exe: ";
                r = openTeleATTexe();
                Testing.logSystem += $"{r}\n";
                if (!r) goto END;

                //4. check result
                Testing.logSystem += $"...Check result test: \n";
                r = getTeleATTresult(Setting.timeoutTestTraffic);
                Testing.logSystem += $"{r}\n";
            }
            catch { r = false; goto END; }

        END:
            myGlobal.myMainView.MainView.Opacity = 1;
            Testing.logSystem += $"...Kết quả test traffic mode XGE: {r}\n";
            Testing.logSystem += "...\n";
            myGlobal.myTesting.Testing.trafficResult = r ? "Passed" : "Failed";
            closeTeleATTexe();
            return r;
        }

        bool saveLogTotal() {
            string f = $"{dir_log}\\Total.csv";
            string title = "DateTime,ID,TotalTime,TotalResult,UpgradeFW,ConfigGE,RestoreGE,1234_1234,5678_1234,9101112_5678,13141516_5678,ConfigXGE,RestoreXGE,TrafficXGE,FactoryReset";
            string data = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                                         DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                         Testing.ID,
                                         Testing.totalTime,
                                         Testing.totalResult,
                                         Testing.upgradeResult,
                                         Testing.configGEResult,
                                         Testing.restoreGEResult,
                                         Testing.pon1234uplink1234Result,
                                         Testing.pon5678uplink1234Result,
                                         Testing.pon9101112uplink5678Result,
                                         Testing.pon13141516uplink5678Result,
                                         Testing.configXGEResult,
                                         Testing.restoreXGEResult,
                                         Testing.trafficResult,
                                         Testing.factoryResetResult);

            if (File.Exists(f) == false) {
                StreamWriter sw = new StreamWriter(f, true, Encoding.Unicode);
                sw.WriteLine(title);
                sw.WriteLine(data);
                sw.Close();
            }
            else {
                StreamWriter sw = new StreamWriter(f, true, Encoding.Unicode);
                sw.WriteLine(data);
                sw.Close();
            }
            return true;
        }

        bool saveLogDetail() {
            string f = $"{dir_log}\\{Testing.ID}_{DateTime.Now.ToString("HHmmss")}_{Testing.totalResult}.txt";
            StreamWriter sw = new StreamWriter(f, true);
            sw.WriteLine(settingToString());
            sw.WriteLine(itemToString());
            sw.WriteLine(systemToString());
            sw.WriteLine(telnetToString());
            sw.Dispose();
            return true;
        }

        string settingToString() {
            var ps = Setting.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            string data = "Setting Information\n";
            data += "*********************************************************************\n";
            foreach (var p in ps) {
                string name = p.Name;
                var value = p.GetValue(Setting, null);
                data += $"{name}={value}\n";
            }
            return data;
        }

        string itemToString() {
            string data = "Log Item\n";
            data += "*********************************************************************\n";
            data += $"ID:{Testing.ID}\n";
            data += $"Total time:{Testing.totalTime}\n";
            data += $"Total result:{Testing.totalResult}\n";
            data += $"Pass:{Testing.passCount} - Fail:{Testing.failCount} - Total:{Testing.totalCount} => Error rate:{Testing.errorRate} %\n";
            data += $"{"Nâng cấp firmware:".PadLeft(56, ' ')} {Testing.upgradeResult.PadLeft(10, ' ')}\n";
            data += $"{ "Cấu hình uplink mode GE:".PadLeft(56, ' ')} {Testing.configGEResult.PadLeft(10, ' ')}\n";
            data += $"{"Restore file cấu hình uplink mode GE:".PadLeft(56, ' ')} {Testing.restoreGEResult.PadLeft(10, ' ')}\n";
            data += $"{"Test traffic port pon 1-2-3-4 & port uplink 1-2-3-4:".PadLeft(56, ' ')} {Testing.pon1234uplink1234Result.PadLeft(10, ' ')}\n";
            data += $"{"Test traffic port pon 5-6-7-8 & port uplink 1-2-3-4:".PadLeft(56, ' ')} {Testing.pon5678uplink1234Result.PadLeft(10, ' ')}\n";
            data += $"{"Test traffic port pon 9-10-11-12 & port uplink 5-6-7-8:".PadLeft(56, ' ')} {Testing.pon9101112uplink5678Result.PadLeft(10, ' ')}\n";
            data += $"{"Test traffic port pon 13-14-15-16 & port uplink 5-6-7-8:".PadLeft(56, ' ')} {Testing.pon13141516uplink5678Result.PadLeft(10, ' ')}\n";
            data += $"{"Cấu hình uplink mode XGE:".PadLeft(56, ' ')} {Testing.configXGEResult.PadLeft(10, ' ')}\n";
            data += $"{"Restore file cấu hình uplink mode XGE:".PadLeft(56, ' ')} {Testing.restoreXGEResult.PadLeft(10, ' ')}\n";
            data += $"{"Test traffic XGE:".PadLeft(56, ' ')} {Testing.trafficResult.PadLeft(10, ' ')}\n";
            data += $"{"Factory reset:".PadLeft(56, ' ')} {Testing.factoryResetResult.PadLeft(10, ' ')}\n";
            return data;
        }

        string systemToString() {
            string data = "Log System\n";
            data += "*********************************************************************\n";
            data += Testing.logSystem;
            return data;
        }

        string telnetToString() {
            string data = "Log Telnet\n";
            data += "*********************************************************************\n";
            data += Testing.logTelnet;
            return data;
        }

    }
}
