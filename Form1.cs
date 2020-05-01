

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;



using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using ChessMonitor;

namespace ChessMonitor
{
    public partial class Form1 : Form
    {


        private Button button1;
        private Button button2;
        private GroupBox groupBox1;
        private CheckBox checkBoxCalibrationDone;
        private CheckBox checkBoxChessboardFound;


        private GroupBox groupBox2;
        private TextBox textBoxMoussePos;
        ChessVision vision;
        uciInterface uci;
        private Button button3;
        private Button button4;
        private TextBox tbMove;
        private Timer Timer_100ms;
        private Timer Timer_500ms;

        private bool GameOngoing;
        private bool GameWaitOpponent;


        string timerDetectedMove;

        public Form1()
        {
            InitializeComponent();
            Timer_100ms = new Timer();
            Timer_100ms.Interval = 100;
            Timer_100ms.Enabled = true;
            Timer_100ms.Tick += new System.EventHandler(timer1_Tick);

            Timer_500ms = new Timer();
            Timer_500ms.Interval = 500;
            Timer_500ms.Enabled = true;
            Timer_500ms.Tick += new System.EventHandler(timer500_Tick);

            vision = new ChessVision();
            uci = new uciInterface();

            uci.init();

            GameOngoing = false;
            GameWaitOpponent = false;
        }

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            bool found = false;
            Bitmap captureBitmap = CaptureMyScreen();
            captureBitmap.Save("capture.png");
            char[] board = new char[64];

            found = vision.ChessboardRead(captureBitmap, ref board);

            if (found)
            {

                checkBoxChessboardFound.Checked = true;
                uci.newboard();

            }
            else
            {
                checkBoxChessboardFound.Checked = false;
            }
        }

        private void buttonCalibrate_Click(object sender, EventArgs e)
        {
            Bitmap captureBitmap = CaptureMyScreen();
            captureBitmap.Save("capture.png");
            checkBoxCalibrationDone.Checked = vision.ChessboardCalibrate(captureBitmap);
            if (checkBoxCalibrationDone.Checked)
            {
                uci.newboard();
            }

        }



        private Bitmap CaptureMyScreen()
        {
            try
            {
                //Creating a new Bitmap object
                Bitmap captureBitmap = new Bitmap(1920, 1080, PixelFormat.Format32bppArgb);

                //Creating a Rectangle object which will capture our Current Screen
                Rectangle captureRectangle = Screen.AllScreens[0].Bounds;

                //Creating a New Graphics Object
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);

                //Copying Image from The Screen
                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);

                //Saving the Image File (I am here Saving it in My E drive).
                //captureBitmap.Save(@"E:\Capture.jpg", ImageFormat.Jpeg);
                // FindChessBoard(captureBitmap);
                //Displaying the Successfull Result

                //MessageBox.Show("Screen Captured");

                return captureBitmap;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }


        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxChessboardFound = new System.Windows.Forms.CheckBox();
            this.checkBoxCalibrationDone = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxMoussePos = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.tbMove = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(99, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 59);
            this.button1.TabIndex = 0;
            this.button1.Text = "Parse Chessboard";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(322, 116);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(83, 40);
            this.button2.TabIndex = 1;
            this.button2.Text = "Calibrate";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonCalibrate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.checkBoxChessboardFound);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.checkBoxCalibrationDone);
            this.groupBox1.Location = new System.Drawing.Point(12, 419);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(411, 162);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // checkBoxChessboardFound
            // 
            this.checkBoxChessboardFound.AutoSize = true;
            this.checkBoxChessboardFound.Location = new System.Drawing.Point(6, 42);
            this.checkBoxChessboardFound.Name = "checkBoxChessboardFound";
            this.checkBoxChessboardFound.Size = new System.Drawing.Size(112, 17);
            this.checkBoxChessboardFound.TabIndex = 1;
            this.checkBoxChessboardFound.Text = "Chessboard found";
            this.checkBoxChessboardFound.UseVisualStyleBackColor = true;
            // 
            // checkBoxCalibrationDone
            // 
            this.checkBoxCalibrationDone.AutoSize = true;
            this.checkBoxCalibrationDone.Location = new System.Drawing.Point(6, 19);
            this.checkBoxCalibrationDone.Name = "checkBoxCalibrationDone";
            this.checkBoxCalibrationDone.Size = new System.Drawing.Size(104, 17);
            this.checkBoxCalibrationDone.TabIndex = 0;
            this.checkBoxCalibrationDone.Text = "Calibration Done";
            this.checkBoxCalibrationDone.UseVisualStyleBackColor = true;
            this.checkBoxCalibrationDone.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxMoussePos);
            this.groupBox2.Location = new System.Drawing.Point(12, 100);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(388, 55);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mousse position";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // textBoxMoussePos
            // 
            this.textBoxMoussePos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMoussePos.Location = new System.Drawing.Point(14, 16);
            this.textBoxMoussePos.Name = "textBoxMoussePos";
            this.textBoxMoussePos.Size = new System.Drawing.Size(368, 20);
            this.textBoxMoussePos.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(322, 71);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(83, 39);
            this.button3.TabIndex = 2;
            this.button3.Text = "CheckBoard";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(322, 26);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(83, 39);
            this.button4.TabIndex = 3;
            this.button4.Text = "Compute move";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.GamePlay);
            // 
            // tbMove
            // 
            this.tbMove.Location = new System.Drawing.Point(15, 265);
            this.tbMove.Name = "tbMove";
            this.tbMove.Size = new System.Drawing.Size(408, 20);
            this.tbMove.TabIndex = 4;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(435, 593);
            this.Controls.Add(this.tbMove);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MouseOperations.MousePoint pos = MouseOperations.GetCursorPosition();
            textBoxMoussePos.Text = "X:" + System.Windows.Forms.Control.MousePosition.X + " Y:" + System.Windows.Forms.Control.MousePosition.Y;

        }




        private void button3_Click(object sender, EventArgs e)
        {
            bool found = false;
            Bitmap captureBitmap = CaptureMyScreen();
            captureBitmap.Save("capture.png");
            char[] board = new char[64];

            found = vision.ChessboardRead(captureBitmap, ref board);

            if (found)
            {
                checkBoxChessboardFound.Checked = true;
            }
            else
            {
                checkBoxChessboardFound.Checked = false;
            }
        }

        private void GamePlay(object sender, EventArgs e)
        {
            GameOngoing = true;
            GameWaitOpponent = false;


            bool found = false;
            Bitmap captureBitmap = CaptureMyScreen();
            captureBitmap.Save("capture.png");
            char[] boardRead = new char[64];
            char[] boardAfter = new char[64];

            found = vision.ChessboardRead(captureBitmap, ref boardRead);

            if (found)
            {
                checkBoxChessboardFound.Checked = true;
                uci.newboard();
            }
            else
            {
                checkBoxChessboardFound.Checked = false;
            }

            string move = uci.Compute();
            tbMove.Text = uci.szUciMove;
            vision.MovePiece(move);
            GameWaitOpponent = true;

        }


        private void timer500_Tick(object sender, EventArgs e)
        {
             string move2play;
            string lclDetectedMove;

            Bitmap captureBitmap;
            char[] boardRead = new char[64];


            if (GameWaitOpponent)
            {
                captureBitmap = CaptureMyScreen();

                if (!vision.ChessboardRead(captureBitmap, ref boardRead))
                {
                    Console.WriteLine("Board not found in timer 500");
                }
                else
                {
                    lclDetectedMove = uci.ComputeMoveFromBoard(uci.boardRef, boardRead);
                    if ((lclDetectedMove == timerDetectedMove) && (timerDetectedMove.Length>0))
                    {
                        Console.WriteLine("Found move!!!!! " + timerDetectedMove);

                        uci.ComputeBoardFromMove(ref uci.boardRef, timerDetectedMove);
                        uci.updateUCImove(timerDetectedMove);

                        move2play = uci.Compute();
                        tbMove.Text = uci.szUciMove;
                        vision.MovePiece(move2play);
                        GameWaitOpponent = true;
                        timerDetectedMove = "";
                    }
                    else
                    {
                        timerDetectedMove = lclDetectedMove;
                    }
                }
            }
            
        }







    }
}
