using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace TrySerialSocket
{
    // not really a wrapper, but a Tcp Server that passes data onto the serial port.
    // SynchronousTcpServerForSerialPort
    public class SynchronousTcpServerForSerialPort //: IDisposable
    {
        private SerialPort _serialPort;
        private Socket _listenerSocket;

        // Incoming data from the client.  
        public string data = null; // doesn't seem right at all

        public SynchronousTcpServerForSerialPort(string hostname, int port)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(hostname);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
        }

        public SynchronousTcpServerForSerialPort(IPAddress ipAddress, int port)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            _listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenerSocket.Bind(ipEndPoint);
            _listenerSocket.Listen(10);
        }

        public void StartListening(IPAddress ipAddress, int port)
        {
            byte[] buffer = new Byte[1024];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            _listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                _listenerSocket.Bind(ipEndPoint);
                _listenerSocket.Listen(10);

                while (true)
                {
                    Socket handlerSocket = _listenerSocket.Accept();

                    byte[] socketBuffer = new byte[255];
                    int byteCount = handlerSocket.Receive(socketBuffer, 0, socketBuffer.Length, SocketFlags.None);
                    _serialPort.Write(socketBuffer, 0, byteCount);

                    // get response from serial port
                    byte[] serialBuffer = new byte[512];
                    int serialBufferLength = 0;

                    try
                    {
                        do
                        {
                            serialBuffer[serialBufferLength] = (byte)_serialPort.ReadByte();
                            ++serialBufferLength;
                        } while (true);
                    }
                    catch (TimeoutException)
                    {
                        // Expected exception indicating all data was recieved -- a yucky way to program!
                        handlerSocket.Send(serialBuffer, 0, serialBufferLength, SocketFlags.None);
                    }
                    finally
                    {
                        handlerSocket.Shutdown(SocketShutdown.Both);
                        handlerSocket.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
