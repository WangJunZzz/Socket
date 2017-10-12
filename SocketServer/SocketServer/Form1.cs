using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //关闭对文本框的非法线程操作检查
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        Thread threadWatch = null;// 负责监听客户端的线程
        Socket socketWatch = null;// 负责监听客户端的套接字
        Socket clientConnection = null;// 负责和客户端通信的套接字
        private void btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ipAddress.Text.ToString()))
            {
                MessageBox.Show("监听ip地址不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(port.Text.ToString()))
            {
                MessageBox.Show("监听端口不能为空！");
                return;
            }
            // 定义一个套接字用于监听客户端发来的消息，包含三个参数（ipv4寻址协议，流式连接，tcp协议）
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 服务端发送消息需要一个ip地址和端口号
            IPAddress ip = IPAddress.Parse(ipAddress.Text.Trim());
            // 把ip地址和端口号绑定在网路节点endport上
            IPEndPoint endPort = new IPEndPoint(ip, int.Parse(port.Text.Trim()));
         
            // 监听绑定的网路节点
            socketWatch.Bind(endPort);
            // 将套接字的监听队列长度设置限制为0，0表示无限
            socketWatch.Listen(0);
            // 创建一个监听线程
            threadWatch = new Thread(WatchConnecting);
            threadWatch.IsBackground = true;
            threadWatch.Start();
            chatContent.AppendText("成功启动监听！ip："+ip+"，端口："+port.Text.Trim()+"\r\n");

        }

        /// <summary>
        ///  监听客户端发来的请求
        /// </summary>
        private void WatchConnecting()
        {
            //持续不断监听客户端发来的请求
            while (true)
            {
                clientConnection = socketWatch.Accept();
                chatContent.AppendText("客户端连接成功！"+"\r\n");
                // 创建一个通信线程
                ParameterizedThreadStart pts = new ParameterizedThreadStart(acceptMsg);
                Thread thr = new Thread(pts);
                thr.IsBackground = true;
                thr.Start(clientConnection);
            }
        }

        /// <summary>
        ///  接收客户端发来的消息
        /// </summary>
        /// <param name="socket">客户端套接字对象</param>
        private void acceptMsg(object socket)
        {
            Socket socketServer = socket as Socket;
            while (true)
            {
                //创建一个内存缓冲区 其大小为1024*1024字节  即1M
                byte[] recMsg = new byte[1024 * 1024];
                //将接收到的信息存入到内存缓冲区,并返回其字节数组的长度
                int length = socketServer.Receive(recMsg);
                //将机器接受到的字节数组转换为人可以读懂的字符串
                string msg = Encoding.UTF8.GetString(recMsg,0,length);
                chatContent.AppendText("客户端("+GetCurrentTime()+"):"+msg+"\r\n");
            }
        }
        /// <summary>
        ///  发送消息到客户端
        /// </summary>
        /// <param name="msg"></param>
        private void serverSendMsg(string msg)
        {
            byte[] sendMsg = Encoding.UTF8.GetBytes(msg);
            clientConnection.Send(sendMsg);
            chatContent.AppendText("服务端("+GetCurrentTime()+"):"+msg+"\r\n");
        }

        /// <summary>
        /// 获取当前系统时间的方法
        /// </summary>
        /// <returns>当前时间</returns>
        private DateTime GetCurrentTime()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            return currentTime;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serverSendMsg(replyContent.Text.Trim());
        }
    }
}
