using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TrySerialSocket
{
    public class SychronousSocketServerForSerialPort
    {
        private Socket _listenerSocket;
        private SerialPort _serialPort;

        public static void StartListening(Socket listenerSocket, SerialPort serialPort)
        {
            try
            {
                listenerSocket.Bind(ipEndPoint);
                listenerSocket.Listen(10);

                while (true)
                {
                    Socket handlerSocket = listenerSocket.Accept();

                    byte[] socketBuffer = new byte[255];
                    int byteCount = handlerSocket.Receive(socketBuffer, 0, socketBuffer.Length, SocketFlags.None);
                    serialPort.Write(socketBuffer, 0, byteCount);

                    // get response from serial port
                    byte[] serialBuffer = new byte[512];
                    int serialBufferLength = 0;

                    try
                    {
                        do
                        {
                            serialBuffer[serialBufferLength] = (byte)serialPort.ReadByte();
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
