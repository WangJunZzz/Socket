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

namespace SocketClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //关闭对文本框的非法线程操作检查
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        // 创建一个客户端套接字
        Socket clientSocket = null;
        // 创建一个监听服务端的线程
        Thread threadServer = null;
        private void btn_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
            IPAddress ip = IPAddress.Parse(ipAddress.Text.Trim());
            IPEndPoint endpoint = new IPEndPoint(ip, int.Parse(port.Text.Trim()));
         
            try
            {   //这里客户端套接字连接到网络节点(服务端)用的方法是Connect 而不是Bind
                clientSocket.Connect(endpoint);
            }
            catch 
            {
                chatContent.AppendText("连接失败！");
               
            }
            
            // 创建一个线程监听服务端发来的消息
            threadServer = new Thread(recMsg);
            threadServer.IsBackground = true;
            threadServer.Start();
        }

        /// <summary>
        ///  接收服务端发来的消息
        /// </summary>
        private void recMsg() {

            while (true) //持续监听服务端发来的消息
            {
                //定义一个1M的内存缓冲区 用于临时性存储接收到的信息
                byte[] arrRecMsg = new byte[1024 * 1024];
                int length = 0;
                try
                {
                    //将客户端套接字接收到的数据存入内存缓冲区, 并获取其长度
                     length = clientSocket.Receive(arrRecMsg);
                }
                catch 
                {
                    return;
                   
                }
             
                //将套接字获取到的字节数组转换为人可以看懂的字符串
                string strRecMsg = Encoding.UTF8.GetString(arrRecMsg, 0, length);
                //将发送的信息追加到聊天内容文本框中
                chatContent.AppendText("服务端(" + GetCurrentTime() + "):" + strRecMsg + "\r\n");
            }
        }

        /// <summary>
        /// 发送消息到服务端
        /// </summary>
        /// <param name="msg"></param>
        private void clientSendMsg(string msg)
        {
            byte[] sendMsg = Encoding.UTF8.GetBytes(msg);
            clientSocket.Send(sendMsg);
            chatContent.AppendText("客户端(" + GetCurrentTime() + "):" + msg + "\r\n");
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
            clientSendMsg(replyContent.Text.Trim());
        }
    }
}
