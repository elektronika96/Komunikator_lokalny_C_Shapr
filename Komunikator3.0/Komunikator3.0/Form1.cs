using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;

namespace Komunikator3._0
{
    public partial class Form1 : Form
    {

        Socket sck;
        EndPoint epLocal, epRemote;
        private SerialPort port;

        //inicjacja portu
        private void init()
        {
            port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            port.BaudRate = 9600;
            port.Open();
        }
        public Form1()
        {
            InitializeComponent();
            init();

            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            textBox_server_ip.Text = GetLocalIP();
            textBox_client_ip.Text = GetLocalIP();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(textBox_server_ip.Text), Convert.ToInt32(textBox_server_port.Text));
                sck.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(textBox_client_ip.Text), Convert.ToInt32(textBox_client_port.Text));
                sck.Connect(epRemote);

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

                button_start.Text = "Connected";
                button_start.Enabled = false;

                button_send.Enabled = true;

                textBox_message.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] recivedData = new byte[1464];
                    recivedData = (byte[])aResult.AsyncState;

                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string recivedMessage = eEncoding.GetString(recivedData);
                    listBox_log.Items.Add("Friend: " + recivedMessage);
                    port.Write("ledon\n");
                    
                }

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(textBox_message.Text);

                sck.Send(msg);

                listBox_log.Items.Add("You: "+textBox_message.Text);
                textBox_message.Clear();
                port.Write("ledoff\n");

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }

        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";

        }
    }
}
