using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFunctionOlt60g16e.Protocol {
    public abstract class OLTAProtocol {

        public bool IsConnected { get; set; }
        public OLTAProtocol(string port_name, string baud_rate) { }
        public OLTAProtocol(string host_name, int port) { }

        public abstract bool Open();
        public abstract bool Close();
        public abstract bool Write(string cmd);
        public abstract bool WriteLine(string cmd);
        public abstract string Query(string cmd, int sleep_ms);
        public abstract bool Query(string cmd, int timeout_sec, params string[] expectations);
        public abstract string Query(string cmd, int timeout_sec, int retry_time, params string[] expectations);
        public abstract string Read();
    }
}
