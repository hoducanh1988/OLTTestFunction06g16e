using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UtilityPack.IO;
using System.IO;

namespace TestFunctionOlt60g16e.Functions {
    public class Utility {

        public static string[] getLocalIP() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var listIP = new List<string>();
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    listIP.Add(ip.ToString());
                }
            }

            return listIP.ToArray();
        }

        public static bool pingNetwork(string ip) {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            // Use the default Ttl value which is 128, 
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted. 
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 60;

            try {
                PingReply reply = pingSender.Send(ip, timeout, buffer, options);
                if (reply.Status == IPStatus.Success) {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch {
                return false;
            }
        }

        public static bool openTftpd64(string root_path) {
            string file_config = $"{AppDomain.CurrentDomain.BaseDirectory}tftpd64\\tftpd32.ini";
            string[] buffer = File.ReadAllLines(file_config);
            List<string> listStr = new List<string>();
            foreach (var b in buffer) {
                if (b.Contains("BaseDirectory=")) {
                    string s = $"BaseDirectory={root_path}";
                    listStr.Add(s);
                }
                else {
                    listStr.Add(b);
                }
            }

            File.Delete(file_config);
            File.WriteAllLines(file_config, listStr.ToArray());
            WindowProcess.callBackProcess($"{AppDomain.CurrentDomain.BaseDirectory}tftpd64\\tftpd64.exe");
            Thread.Sleep(100);
            return WindowProcess.isProcessRunning("tftpd64");
        }

        public static bool closeTftpd64() {
            return WindowProcess.killAllProcessByName("tftpd64");
        }



    }
}
