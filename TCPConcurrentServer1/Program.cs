using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPConcurrentServer1
{
    class Program
    {
        private static TcpListener _welcomingSocket;
        private static TcpClient _serverSocket;
        static void Main(string[] args)
        {
            try
            {
                var ip = IPAddress.Parse("127.0.0.1");
                // you have to reserved the server port
                // You always define port number service here in server 
                _welcomingSocket = new TcpListener(ip, 6789);
                Console.WriteLine("Server is ready to listen incoming client request");
                _welcomingSocket.Start();
                using (_serverSocket = _welcomingSocket.AcceptTcpClient())
                {
                    Console.WriteLine("Client IP" + ((IPEndPoint)_serverSocket.Client.RemoteEndPoint).Address);
                    EchoServices service = new EchoServices(_serverSocket, _welcomingSocket);
                    // method is now a multi threaded 
                    // server recieved multiple message from different clients at the same same time

                    while (true)
                    {
                        Task.Run(() => service.DoIt());
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _welcomingSocket.Stop();
            }

        }
    }
}
