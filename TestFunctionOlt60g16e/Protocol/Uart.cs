using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using System.Reflection;

namespace TestFunctionOlt60g16e.Protocol {

    public class Uart<T> : OLTAProtocol where T : class, new() {

        SerialPort serialport = null;
        string portName = "";
        string baudRate = "";
        T testing = null;

        public Uart(string port_name, string baud_rate, T t) : base(port_name, baud_rate) {
            this.portName = port_name;
            this.baudRate = baud_rate;
            this.testing = t;
        }

        public override bool Open() {
            int count = 0;
            bool result = false;
        REP:
            count++;
            try {
                this.serialport = new SerialPort();
                this.serialport.PortName = portName;
                this.serialport.BaudRate = int.Parse(baudRate);
                this.serialport.DataBits = 8;
                this.serialport.Parity = Parity.None;
                this.serialport.StopBits = StopBits.One;
                this.serialport.Open();
                result = serialport.IsOpen;
            }
            catch { result = false; }
            if (!result) { if (count < 3) { Thread.Sleep(100); goto REP; } }
            return result;
        }

        public override bool Close() {
            try {
                if (this.serialport != null) this.serialport.Close();
                return true;
            }
            catch {
                return false;
            }
        }

        public override bool Write(string cmd) {
            try {
                this.serialport.Write(cmd);
                return true;
            }
            catch { return false; }
        }

        public override bool WriteLine(string cmd) {
            try {
                this.Write($"{cmd}\r\n");
                return true;
            }
            catch { return false; }
        }

        public override string Query(string cmd, int sleep_ms) {
            this.WriteLine(cmd);
            Thread.Sleep(sleep_ms);
            string data = this.Read();
            return data;
        }

        public override bool Query (string cmd, int timeout_sec, params string[] expectations) {
            bool r = false;
            try {
                int delay_ms = 100;
                int max_count = (timeout_sec * 1000) / delay_ms;
                int count = 0;
                this.WriteLine(cmd);

                string data = "";
            RE:
                count++;
                Thread.Sleep(delay_ms);
                data += this.Read();
                
                //check null or empty
                if (string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data)) {
                    if (count < max_count) goto RE;
                    else goto END;
                }

                //check contain expectation
                foreach (var expectation in expectations) {
                    if (data.ToLower().Contains(expectation.ToLower())) {
                        r = true;
                        break;
                    }
                }
                if (!r) {
                    if (count < max_count) goto RE;
                    else goto END;
                }
            } catch { goto END;  }

        END:
            return r;
        }

        public override string Read() {
            try {
                string data = this.serialport.ReadExisting();
                PropertyInfo p = this.testing.GetType().GetProperty("logUart");
                string s = (string)p.GetValue(this.testing, null);
                s += data;
                p.SetValue(this.testing, s, null);
                return data;
            }
            catch { return null; }
        }

        public override string Query(string cmd, int timeout_sec, int retry_time, params string[] expectations) {
            throw new NotImplementedException();
        }
    }
}
