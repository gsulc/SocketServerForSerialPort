using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TrySerialSocket
{
    public class SocketServerForSerialPort : Socket
    {
        SerialPort _serialPort;
        Socket b;

        public SocketServerForSerialPort(SocketInformation socketInformation)
            : base(socketInformation)
        {

        }
    }
}
