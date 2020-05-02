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
        public char[] boardRef = new char[64];
        public void init()
        {
            szUciMove = "";

        }

        public void newboard()
        {
            Connect("127.0.0.1", "position startpos");
            szUciMove = "";
            boardRef = ("rnbqkbnrpppppppp                                PPPPPPPPRNBQKBNR").ToCharArray();
        }

        public string Compute()
        {
            if (szUciMove.Length > 0)
            {
                Connect("127.0.0.1", "position startpos moves " + szUciMove);
            }
            else
            {
                Connect("127.0.0.1", "position startpos");
            }

            string res = Connect("127.0.0.1", "go");
            string[] spl = res.Split(' ');

            if (spl[0] == "bestmove")
            {
                updateUCImove(spl[1]);
                ComputeBoardFromMove(ref boardRef, spl[1]);
                Console.WriteLine(res);
                return spl[1];
            }
            return "";
        }

        public void updateUCImove(string move)
        {
            szUciMove += move + " ";
        }

        public bool compareBoard(char[] boardRef1, char[] boardRef2)
        {
            if (boardRef1.Length != boardRef2.Length)
                return false;
            for (int ii = 0; ii < boardRef1.Length; ii++)
                if (boardRef1[ii] != boardRef2[ii])
                    return false;

            return true;
        }
        public string ComputeMoveFromBoard(char[] boardRef, char[] boardCurrent)
        {
            List<int> diff = new List<int>();
            string str;
            int start, end;


            for (int ii = 0; ii < 64; ii++)
            {
                if (boardRef[ii] != boardCurrent[ii])
                {
                    diff.Add(ii);
                }
            }




            if ((diff.Count < 2) || (diff.Count > 4))
                return "";


            if (diff.Count == 2)
            {
                if (boardCurrent[diff[0]] == ' ')
                {
                    start = diff[0];
                    end = diff[1];
                }
                else
                {
                    start = diff[1];
                    end = diff[0];
                }

                str = "";
                str += Encoding.ASCII.GetString(new[] { (byte)((start % 8) + 'a') });
                str += Encoding.ASCII.GetString(new[] { (byte)('8' - (start / 8)) });
                str += Encoding.ASCII.GetString(new[] { (byte)((end % 8) + 'a') });
                str += Encoding.ASCII.GetString(new[] { (byte)('8' - (end / 8)) });

                return str;
            }

            if (diff.Count == 4)
            {
                // petit rocque white
                if ((boardRef[60] == 'K') && (boardRef[63] == 'R') && (boardCurrent[63] == ' ') && (boardCurrent[61] == 'R'))
                {
                    return "e1g1";
                }

                // grand rocque white

                if ((boardRef[60] == 'K') && (boardRef[56] == 'R') && (boardCurrent[56] == ' ') && (boardCurrent[59] == 'R'))
                {
                    return "e1c1";
                }

                // petit rocque black                
                if ((boardRef[4] == 'k') && (boardRef[7] == 'r') && (boardCurrent[7] == ' ') && (boardCurrent[5] == 'r'))
                {
                    return "e8g8";
                }

                // grand rocque black
                if ((boardRef[4] == 'k') && (boardRef[0] == 'r') && (boardCurrent[0] == ' ') && (boardCurrent[3] == 'r'))
                {
                    return "e8c8";
                }

            }

            return "";


        }

        public void ComputeBoardFromMove(ref char[] board, string szMove)
        {
            int start_x = szMove[0] - 'a';
            int start_y = szMove[1] - '1';
            int end_x = szMove[2] - 'a';
            int end_y = szMove[3] - '1';

            int s, e;

            s = start_x + 8 * (7 - start_y);
            s = start_x + 8 * (7 - start_y);

            e = end_x + 8 * (7 - end_y);
            e = end_x + 8 * (7 - end_y);

            board[e] = board[s];
            board[s] = ' ';


            // petit rocque white
            if (szMove == "e1g1")
            {
                if ((board[60] == 'K') && (board[63] == 'R'))
                {
                    board[63] = ' ';
                    board[61] = 'R';
                }
            }


            // grand rocque white
            if (szMove == "e1c1")
            {
                if ((board[60] == 'K') && (board[56] == 'R'))
                {
                    board[56] = ' ';
                    board[59] = 'R';
                }
            }

            // petit rocque black
            if (szMove == "e8g8")
            {
                if ((board[4] == 'k') && (board[7] == 'r'))
                {
                    board[7] = ' ';
                    board[5] = 'r';
                }
            }

            // grand rocque black
            if (szMove == "e8c8")
            {
                if ((board[4] == 'k') && (board[0] == 'r'))
                {
                    board[0] = ' ';
                    board[3] = 'r';
                }
            }



            for (int ii = 0; ii < 8; ii++)
            {
                for (int jj = 0; jj < 8; jj++)
                {
                    Console.Write(board[jj + 8 * ii]);
                }
                Console.WriteLine(" ");
            }
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
