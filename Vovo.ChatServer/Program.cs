using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Vovo.ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse("192.168.1.36"), 2012);

            server.Start();
            Console.WriteLine("Server has started on 192.168.100.100:100. {0}Waiting for a connection...", Environment.NewLine);

                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("A client connected.");


            NetworkStream stream = client.GetStream();

            //enter to an infinite cycle to be able to handle every change in stream
            while (true)
            {
                while (!stream.DataAvailable);

                Byte[] bytes = new Byte[client.Available];

                stream.Read(bytes, 0, bytes.Length);
            }
        }
    }
}
