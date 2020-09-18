using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace TCPConcurrentServer1
{

    public class EchoServices
    {
        private  TcpClient _serverSocket;
        private TcpListener _welcomingSocket;
        private FileStream _fileStream;
        String uri = @"C:/temp"; // Root path C://temp/index.html
        public EchoServices(TcpClient serverSocket , TcpListener welcomingSocket)
        {
            _serverSocket = serverSocket;
            _welcomingSocket = welcomingSocket;
        }
        public void DoIt()
        {
            using (Stream ns = _serverSocket.GetStream())
            {
                StreamWriter sw = new StreamWriter(new BufferedStream(ns));
                sw.AutoFlush = true;
                StreamReader sr = new StreamReader(ns);

                // Read data from Client
                var request = sr.ReadLine();
                // Check message is not null 
                while (request != null || request != "")
                {
                    // Get /index.html HTTP1.1
                    // [0] Get
                    //  [1]/index.html
                    // [2] HTTP1.1
                    var brokenMessages = request.Split(' ');
                    if (brokenMessages.Length != 0)
                    {
                        // check first Message
                        // Get
                        var firstMessage = brokenMessages[0];
                        // calls when client send STOP message 
                        if (firstMessage.Equals("stop"))
                        {
                            Console.WriteLine("its mean you want to stop the server");
                            while (_welcomingSocket.Pending())
                            {
                                Thread.Sleep(100);
                            }
                            Console.WriteLine("Finally Server is Stopped");
                            _welcomingSocket.Stop();                           
                            break;
                        }
                        // calls when client send GET /index.html HTTP1.1
                        else if (firstMessage.Equals("GET") && brokenMessages.Length == 3)
                        {
                            string fileName = brokenMessages[1];
                            string protocol = brokenMessages[2];
                            // HTTP response header 200-> http response status
                            sw.Write("HTTP/1.1 200 OK\r\n");
                            sw.Write("Content-Type: text/html\r\n");
                            sw.Write("Connection: close\r\n");
                            sw.Write("\r\n"); //Only to Browser marks data are coming
                            sw.Write("Hello client\r\n");
                            sw.Write("Requested file: " + fileName + "\r\n");
                             // C://temp/index.html
                            uri = uri + fileName;
                            // path of resource name
                            _fileStream = new FileStream(uri, FileMode.Open, FileAccess.Read);
                            StreamReader fileReader = new StreamReader(_fileStream);
                            while (!fileReader.EndOfStream)
                            {
                                string s = fileReader.ReadLine();
                                Console.WriteLine(s);
                                sw.Write(s + "\r\n");
                            }
                            sw.Write("\r\n");
                            sw.Flush();
                            sw.BaseStream.Flush();
                            //sw.Close();
                        }
                        // calls this when client send general message 
                        else
                        {
                            Console.WriteLine("Client message:" + request);
                            var respond = request.ToLower();
                            sw.WriteLine(respond);
                        }
                        // server is ready to listen again
                        request = sr.ReadLine();
                    }
                }
               
            }
            
        }

    }
}
