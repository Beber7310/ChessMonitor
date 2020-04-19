

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
        ChessVision vision;



        public Form1()
        {
            InitializeComponent();

            vision=new ChessVision();
        } 

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            bool found = false;
            Bitmap captureBitmap=CaptureMyScreen();
            captureBitmap.Save("capture.png");
            found=vision.ChessboardRead(captureBitmap);
            
            if(!found)

            if(found)
                vision.MovePiece(1, 0,2,2);
        }

        private void buttonCalibrate_Click(object sender, EventArgs e)
        {            
            Bitmap captureBitmap = CaptureMyScreen();
            captureBitmap.Save("capture.png");
            vision.ChessboardCalibrate(captureBitmap);
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
            this.checkBoxCalibrationDone = new System.Windows.Forms.CheckBox();
            this.checkBoxChessboardFound = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
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
            this.button2.Location = new System.Drawing.Point(183, 19);
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
            this.groupBox1.Controls.Add(this.checkBoxChessboardFound);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.checkBoxCalibrationDone);
            this.groupBox1.Location = new System.Drawing.Point(12, 375);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 77);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
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
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(300, 464);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
