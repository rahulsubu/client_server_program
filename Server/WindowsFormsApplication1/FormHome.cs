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
using System.Threading;
using System.IO;
using System.Xml;
using Npgsql;
using System.Data.SqlClient;
using NpgsqlTypes;
namespace WindowsFormsApplication1
{
   
        
    public partial class FormHome : Form
    {
        public static int clientCount;
        public static string message2;
        
        
        public FormHome()
        {
            InitializeComponent();
            label4.Text = clientCount.ToString();
            
        }
        delegate void SetTextCallback(String Clientname,bool state);
        public  void updatefile(String Clientname,bool state)
        {
            //write file write code here. 
            if (this.label4.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(updatefile);
                this.Invoke(d, new object[] {Clientname,state  });
            }
            else
            {
                this.label4.Text = clientCount.ToString();
            
            //label4.Text = clientCount.ToString();
            StreamWriter logfile = File.AppendText("log.txt");
            if(state)
            logfile.Write(clientCount.ToString() + " " + DateTime.Now.ToString() + " ... "+Clientname+" Logged off"+ "\r\n");
            else
                logfile.Write(clientCount.ToString() + " " + DateTime.Now.ToString() + " ... " + Clientname + " Logged in"+"\r\n");

            //logfile.WriteLine(Clientname);
            logfile.Close();
                //Write to database
                string temp1=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string temp2 = Clientname + " Logged off";
                string temp3 = Clientname + " Logged in";
                using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User Id=postgres;Password=therock;Database=postgres;"))
                {
                    IDbCommand dbcmd;
                    IDataReader reader;
                    conn.Open();

                    NpgsqlCommand command = new NpgsqlCommand("INSERT INTO log (n_pplactive, time_stamp, comp_active) VALUES (:n_pplactive, :time_stamp, :comp_active);", conn);
                    command.Parameters.Add(new NpgsqlParameter("n_pplactive", DbType.Int32));
                    command.Parameters.Add(new NpgsqlParameter("time_stamp", DbType.String));
                    command.Parameters.Add(new NpgsqlParameter("comp_active", DbType.String));
                    command.Parameters[0].Value = clientCount;
                    command.Parameters[1].Value = temp1;
                    if (state)
                        command.Parameters[2].Value = temp2;
                    else
                        command.Parameters[2].Value = temp3;
                    dbcmd = conn.CreateCommand();
                    reader = command.ExecuteReader();
                    reader.Close();
                    conn.Close();
                }

                
                
                        }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            serverObject.tcpListener.Stop();
            Application.Exit();
            
        }
        private Server serverObject;
        private void FormHome_Load(object sender, EventArgs e)
        {
             serverObject = new Server();

        }

       
    }
    class Server
    {
        public TcpListener tcpListener;
        private Thread listenThread;
        public bool exitThis;
        public Server()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 3000);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            exitThis = false;
            this.listenThread.Start();
        }
       
        
        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (exitThis!=true)
            {
                //blocks until a client has connected to the server
                try
                {
                    TcpClient client = this.tcpListener.AcceptTcpClient();
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                }
                catch
                {
                    return;
                }
                //create a thread to handle communication 
                //with connected client
                
            }
        }
        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();
            
            FormHome.clientCount++;
            
            byte[] message = new byte[4096];
            
            int bytesRead;
            //bytesRead = clientStream.Read(message, 0, 4096);
            //FormHome.message2 = encoder.GetString(message, 0, bytesRead);
            
            String Name = "NoName";
            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);

                }
                catch
                {
                    //a socket error has occured
                    break;
                }
                String recMessage = encoder.GetString(message, 0, bytesRead);
                if (recMessage != "")
                {
                    if (recMessage.Substring(0, 5) == "Name:")
                    {
                        Name = (recMessage.Substring(5, recMessage.Length-5)).Trim();
                        Program.myFormHome.updatefile(Name, false);
                        StreamWriter myFile = File.AppendText(Name + "Logfile.xml");
                        myFile.Write("<Logging On>" + DateTime.Now.ToString() + "</Logging On>"+"\n");
                        myFile.Close();



                    }


                    //message has successfully been received
                    //ASCIIEncoding encoder = new ASCIIEncoding();
                    //MessageBox.Show(encoder.GetString(message, 0, bytesRead));

                    if (recMessage == Name + ":Logging off")
                    {
                        StreamWriter myFile = File.AppendText(Name + "Logfile.xml");
                        myFile.Write("<Logging Off>" + DateTime.Now.ToString() +"</Logging Off>" +"\n");
                        myFile.Close();
                        break;
                    }
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    StreamWriter myFile = File.AppendText(Name + "Logfile.xml");
                    myFile.Write("<Logging Off>" + DateTime.Now.ToString() + "</Logging Off>"+"\n");
                    myFile.Close();
                    break;
                }
            }
            FormHome.clientCount--;
            //ASCIIEncoding encoder2 = new ASCIIEncoding();
            //FormHome.message2 = encoder.GetString(message, 0, bytesRead);
            Program.myFormHome.updatefile(Name,true);
            tcpClient.Close();
        }
    }   
}
