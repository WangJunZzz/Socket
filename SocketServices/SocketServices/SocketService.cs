using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServices
{
    class SocketService
    {
        private static byte[] result = new byte[1024];
        private static int port = 9999;//端口
        static Socket serverSocket;//服务

        public void Start()
        {
            //服务器ip地址
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            // socket的构造函数进行服务注册
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 绑定端口号
            serverSocket.Bind(new IPEndPoint(ip, port));
            // 设定最多10个排队连接
            serverSocket.Listen(10);
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            Thread th = new Thread(new ThreadStart(Accept));
            //while (true)
            //{
            //    Socket accept = serverSocket.Accept();
            //    string receiveData = Accept(accept, 5000);
            //    Console.WriteLine("接收客户端{0}消息.", receiveData);

            //}

            th.Start();

        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="timeout"></param>
        private void Accept()
        {
            while (true)
            {
                Socket accept = serverSocket.Accept();

                string result = string.Empty;

                List<byte> data = new List<byte>();
                byte[] buffer = new byte[1024];
                int length = 0;
                try
                {
                    while ((length = accept.Receive(buffer)) > 0)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            data.Add(buffer[i]);
                        }
                        if (length < buffer.Length)
                        {
                            break;
                        }
                    }
                    if (data.Count > 0)
                    {
                        result = Encoding.UTF8.GetString(data.ToArray(), 0, data.Count);
                    }
                    Console.WriteLine("接收客户端{0}消息.", result);

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private string Send(string host, int port, string data)
        {
            string result = string.Empty;
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(host, port);
            client.Send(Encoding.Default.GetBytes(data));
           // result = Accept(client, 5000 * 2);
            DestroySocket(client);
            Console.WriteLine("receive:" + result);
            return result;
        }

        /// <summary>
        /// 销毁Socket对象
        /// </summary>
        /// <param name="socket"></param>
        private static void DestroySocket(Socket socket)
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
        }
    }
}
