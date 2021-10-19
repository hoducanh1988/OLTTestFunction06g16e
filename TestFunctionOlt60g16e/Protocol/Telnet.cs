using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestFunctionOlt60g16e.Protocol {
    public class Telnet<T> : OLTAProtocol where T : class, new() {

        TcpClient client;
        enum Verbs { WILL = 251, WONT = 252, DO = 253, DONT = 254, IAC = 255 }
        enum Options { SGA = 3 }
        public string host;
        public int port;
        T testing = null;

        public Telnet(string port_name, string baud_rate) : base(port_name, baud_rate) { }

        public Telnet(string host_name, int port, T t) : base(host_name, port) {
            this.host = host_name;
            this.port = port;
            this.testing = t;
        }

        private void configTCP() {
            // Don't allow another process to bind to this port.
            this.client.ExclusiveAddressUse = false;
            // sets the amount of time to linger after closing, using the LingerOption public property.
            this.client.LingerState = new LingerOption(false, 0);
            // Sends data immediately upon calling NetworkStream.Write.
            this.client.NoDelay = true;
            // Sets the receive buffer size using the ReceiveBufferSize public property.
            this.client.ReceiveBufferSize = 1024;
            // Sets the receive time out using the ReceiveTimeout public property.
            this.client.ReceiveTimeout = 5000;
            // Sets the send buffer size using the SendBufferSize public property.
            this.client.SendBufferSize = 1024;
            // sets the send time out using the SendTimeout public property.
            this.client.SendTimeout = 5000;
        }

        public override bool Close() {
            if (this.client != null && IsConnected == true) this.client.Close();
            return true;
        }

        public override bool Open() {
            IsConnected = false;
            this.client = new TcpClient();
            this.configTCP();
            try {
                IsConnected = this.client.ConnectAsync(host, port).Wait(3000);
            }
            catch { IsConnected = false; }
            return IsConnected;
        }

        public override string Query(string cmd, int sleep_ms) {
            this.WriteLine(cmd);
            Thread.Sleep(sleep_ms);
            string data = this.Read();
            return data;
        }

        public override bool Query(string cmd, int timeout_sec, params string[] expectations) {
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
            }
            catch { goto END; }

        END:
            return r;
        }

        public override string Query(string cmd, int timeout_sec, int retry_time, params string[] expectations) {
            string data = "";
            bool r = false;
            int c = 0;

        LOOP:
            c++;
            try {
                int delay_ms = 100;
                int max_count = (timeout_sec * 1000) / delay_ms;
                int count = 0;
                this.WriteLine(cmd);


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
            }
            catch { goto END; }

        END:
            if (r == false) {
                if (c < retry_time) goto LOOP;
            }
            return data;
        }


        public override string Read() {
            if (!this.client.Connected) return null;
            StringBuilder sb = new StringBuilder();
            do {
                ParseTelnet(sb);
                System.Threading.Thread.Sleep(300);
            } while (this.client.Available > 0);

            string data = sb.ToString();
            PropertyInfo p = this.testing.GetType().GetProperty("logTelnet");
            string s = (string)p.GetValue(this.testing, null);
            s += data;
            p.SetValue(this.testing, s, null);
            return data;
        }

        public override bool Write(string cmd) {
            try {
                if (!IsConnected) return false;
                Byte[] sendBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF")); //convert string to array[bytes]
                this.client.GetStream().Write(sendBytes, 0, sendBytes.Length);
                return true;
            }
            catch { return false; }

        }

        public override bool WriteLine(string cmd) {
            return this.Write(cmd + "\n");
        }

        void ParseTelnet(StringBuilder sb) {
            while (this.client.Available > 0) {
                int input = this.client.GetStream().ReadByte();
                switch (input) {
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = this.client.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb) {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = this.client.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                this.client.GetStream().WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA)
                                    this.client.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    this.client.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                this.client.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
            }
        }

    }
}
