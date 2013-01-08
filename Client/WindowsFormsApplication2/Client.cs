using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;





namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private TcpClient client;
        private IPEndPoint serverEndPoint;
        private byte[] buffer;
        //private byte[] buffer2;
        private ASCIIEncoding encoder;
        //private ASCIIEncoding encoder2;
        private NetworkStream clientStream;
        private bool connected = false;
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        private void reconnectoserver()
        {
            if (client != null)
            try
            {
                client = new TcpClient();
                serverEndPoint = new IPEndPoint(IPAddress.Parse(Program.ip), 3000);

                client.Connect(serverEndPoint);

                clientStream = client.GetStream();

                encoder = new ASCIIEncoding();
                buffer = encoder.GetBytes("Name: " + Program.myName);

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
                label1.Text = "Connected to server";

                connected = true;
            }
            catch
            {
                //MessageBox.Show("Server Unavailable");
                Application.Exit();
            }
        }
            
        
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient();

                serverEndPoint = new IPEndPoint(IPAddress.Parse(Program.ip), 3000);

                client.Connect(serverEndPoint);

                clientStream = client.GetStream();

                encoder = new ASCIIEncoding();
                buffer = encoder.GetBytes("Name: "+Program.myName);

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
                connected = true;
            }
            catch
            {
                //MessageBox.Show("Server Unavailable");
                Application.Exit();
            }
            
        }
        private void disconnectfromServer()
        {
            try
            {
                buffer = encoder.GetBytes(Program.myName + ":Logging Off");
                clientStream.Write(buffer, 0, buffer.Length);
                //encoder2 = new ASCIIEncoding();
                //buffer2 = encoder2.GetBytes("Rahuls Compuer logged off");
                //clientStream.Write(buffer2, 0, buffer.Length);
                clientStream.Flush();
                clientStream.Close();
                client.Close();
                label1.Text="Disconnected from server";
                connected = false;
            }
            catch
            {
                MessageBox.Show("Server Unavailable");
                Application.Exit();
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            try
            {
                buffer = encoder.GetBytes(Program.myName+":Logging Off");
                clientStream.Write(buffer, 0, buffer.Length);
                //encoder2 = new ASCIIEncoding();
                //buffer2 = encoder2.GetBytes("Rahuls Compuer logged off");
                //clientStream.Write(buffer2, 0, buffer.Length);
                clientStream.Flush();
                client.Close();
            }
            catch
            {
                MessageBox.Show("Server Unavailable");
               Application.Exit();
            }

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //this.Hide();
        }
        private void tmrIdle_Tick(object sender, EventArgs e)
        {

            // Get the system uptime
            int systemUptime = Environment.TickCount;
           // The tick at which the last input was recorded
            int LastInputTicks = 0;
            // The number of ticks that passed since last input
           int IdleTicks = 0;
            // Set the struct
            LASTINPUTINFO LastInputInfo = new LASTINPUTINFO();
            LastInputInfo.cbSize = (uint)Marshal.SizeOf(LastInputInfo);
            LastInputInfo.dwTime = 0;
            // If we have a value from the function
            if (GetLastInputInfo(ref LastInputInfo))
            {
                // Get the number of ticks at the point when the last activity was seen
                LastInputTicks = (int)LastInputInfo.dwTime;
                // Number of idle ticks = system uptime ticks - number of ticks at last input
                IdleTicks = systemUptime - LastInputTicks;
            }
            // Set the labels; divide by 1000 to transform the milliseconds to seconds
            //lblSystemUptime.Text = Convert.ToString(systemUptime / 1000) + " seconds";
            label3.Text = Convert.ToString(IdleTicks / 1000) + " seconds" ;
            if (IdleTicks / 1000==120   && connected)
            {
                disconnectfromServer();
            }
            else if (IdleTicks / 1000 == 0&& !connected)
            {
                reconnectoserver();
            }

                
           // label4.Text = "At second " + Convert.ToString(LastInputTicks / 1000);
        }

       
    }
}
internal struct LASTINPUTINFO
{
    public uint cbSize;

    public uint dwTime;
}
