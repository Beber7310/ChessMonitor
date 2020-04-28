using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChessMonitor
{
    class uciInterface
    {
        public string szUciMove;
        public void init()
        {
            szUciMove = "";

        }

        public void newboard()
        {
            Connect("127.0.0.1", "position startpos");
            szUciMove = "";
        }

        public string Compute()
        {
            string res=Connect("127.0.0.1", "go");
            string[] spl = res.Split(' ');

            if (spl[0] == "bestmove")
            {
                szUciMove += spl[1];
                Console.WriteLine(res);
                return spl[1];
            }

            return "";
            
        }

        public void move(string move)
        {
            szUciMove += move + " ";
        }

       


 
        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        string Connect(String server, String message)
        {
            string result = "";

            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("[TCP]:" + message);

                tcpclnt.Connect(server, 13000);
               
                StreamWriter writer = new StreamWriter(tcpclnt.GetStream(), Encoding.ASCII);
                StreamReader reader = new StreamReader(tcpclnt.GetStream(), Encoding.ASCII);
                writer.AutoFlush = true;
                writer.WriteLine(message);

                bool cmddone = false;
                do
                {
                    string ret = reader.ReadLine();
                    if (ret == "tcp_end")
                        cmddone = true;
                    else
                        result = ret;
                }
                while (!cmddone);

                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }

            return result;

        }



    }
}
