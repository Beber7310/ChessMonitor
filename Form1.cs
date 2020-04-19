

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
 


using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;


namespace ScreenCaptureDemo
{
    public partial class Form1 : Form
    {
        private Button button1;
        private Button button2;
        
        private int ThresholdChessboard = 200;        
        private int ThresholdPiece = 200;

        private Bitmap ImageEmptyBlack;
        private Bitmap ImageEmptyWhite;

        private Bitmap ImageWhitePawn;
        private Bitmap ImageWhiteRook;
        private Bitmap ImageWhiteKnight;
        private Bitmap ImageWhiteBishop;
        private Bitmap ImageWhiteQueen;
        private Bitmap ImageWhiteKing;

        private Bitmap ImageBlackPawn;
        private Bitmap ImageBlackRook;
        private Bitmap ImageBlackKnight;
        private Bitmap ImageBlackBishop;
        private Bitmap ImageBlackQueen;
        private Bitmap ImageBlackKing;


        public Form1()
        {
            InitializeComponent();
        } 

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            List<int> coordX = new List<int>();
            List<int> coordY = new List<int>();

            Bitmap captureBitmap=CaptureMyScreen();
            captureBitmap.Save("capture.png");
            FindChessBoard(captureBitmap, ref coordX, ref coordY);
            if ((coordX.Count == 9) && (coordY.Count == 9))
            {
                removeBackground(ref captureBitmap);
                AnalysePiece(captureBitmap, coordX, coordY);
            }
        }

        private void buttonCalibrate_Click(object sender, EventArgs e)
        {
            List<int> coordX = new List<int>();
            List<int> coordY = new List<int>();

            Bitmap captureBitmap = CaptureMyScreen();
            FindChessBoard(captureBitmap, ref coordX, ref coordY);
            if ((coordX.Count == 9) && (coordY.Count == 9))
            {
                removeBackground(ref captureBitmap);
                CalibratePiece(captureBitmap, coordX, coordY);
            }
        }

        private void removeBackground(ref Bitmap image)
        {
            // create filter
            HSLFiltering filter = new HSLFiltering();
            // set color ranges to keep
            filter.Hue = new IntRange(91, 42);
            filter.Saturation = new DoubleRange(0, 1);
            filter.Luminance = new DoubleRange(0, 1);
            // apply the filter
            filter.ApplyInPlace(image);

            Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
            // apply the filter
            image = filterG.Apply(image);

        }
        
        private void CalibratePiece(Bitmap image, List<int> coordX, List<int> coordY)
        {
            Crop filterCrop;
             
            //ImageEmptyBlack
            filterCrop = new Crop(new Rectangle(coordX[0] + 1, coordY[5] + 1, (coordX[1] - coordX[0]) - 2, (coordY[6] - coordY[5]) - 2));
            ImageEmptyBlack = filterCrop.Apply(image);
            ImageEmptyBlack.Save("result_EmptyBlack.png");

            //ImageEmptyWhite
            filterCrop = new Crop(new Rectangle(coordX[1] + 1, coordY[5] + 1, (coordX[2] - coordX[1]) - 2, (coordY[6] - coordY[5]) - 2));
            ImageEmptyWhite = filterCrop.Apply(image);
            ImageEmptyWhite.Save("result_EmptyWhite.png");

            //Pawn
            filterCrop = new Crop(new Rectangle(coordX[0]+1, coordY[6]+1, (coordX[1]- coordX[0])-2, (coordY[7]- coordY[6])-2));
            ImageWhitePawn = filterCrop.Apply(image);
            ImageWhitePawn.Save("result_white_pawn.png");
            filterCrop = new Crop(new Rectangle(coordX[0] + 1, coordY[1], (coordX[1] - coordX[0]) - 2, (coordY[2] - coordY[1]) - 2));
            ImageBlackPawn = filterCrop.Apply(image);
            ImageBlackPawn.Save("result_black_pawn.png");

            //rook
            filterCrop = new Crop(new Rectangle(coordX[0] + 1, coordY[7] + 1, (coordX[1] - coordX[0]) - 2, (coordY[8] - coordY[7]) - 2));
            ImageWhiteRook = filterCrop.Apply(image);
            ImageWhiteRook.Save("result_white_rook.png");
            filterCrop = new Crop(new Rectangle(coordX[0] + 1, coordY[0], (coordX[1] - coordX[0]) - 2, (coordY[1] - coordY[0]) - 2));
            ImageBlackRook = filterCrop.Apply(image);
            ImageBlackRook.Save("result_black_rook.png");

            //Knight
            filterCrop = new Crop(new Rectangle(coordX[1] + 1, coordY[7] + 1, (coordX[2] - coordX[1]) - 2, (coordY[8] - coordY[7]) - 2));
            ImageWhiteKnight = filterCrop.Apply(image);
            ImageWhiteKnight.Save("result_white_knight.png");
            filterCrop = new Crop(new Rectangle(coordX[1] + 1, coordY[0] + 1, (coordX[2] - coordX[1]) - 2, (coordY[1] - coordY[0]) - 2));
            ImageBlackKnight = filterCrop.Apply(image);
            ImageBlackKnight.Save("result_black_knight.png");


            //bishop
            filterCrop = new Crop(new Rectangle(coordX[2] + 1, coordY[7] + 1, (coordX[3] - coordX[2]) - 2, (coordY[8] - coordY[7]) - 2));
            ImageWhiteBishop = filterCrop.Apply(image);
            ImageWhiteBishop.Save("result_white_bishop.png");
            filterCrop = new Crop(new Rectangle(coordX[2] + 1, coordY[0] + 1, (coordX[3] - coordX[2]) - 2, (coordY[1] - coordY[0]) - 2));
            ImageBlackBishop = filterCrop.Apply(image);
            ImageBlackBishop.Save("result_black_bishop.png");

            //queen
            filterCrop = new Crop(new Rectangle(coordX[3] + 1, coordY[7] + 1, (coordX[4] - coordX[3]) - 2, (coordY[8] - coordY[7]) - 2));
            ImageWhiteQueen = filterCrop.Apply(image);
            ImageWhiteQueen.Save("result_white_queen.png");

            filterCrop = new Crop(new Rectangle(coordX[3] + 1, coordY[0] + 1, (coordX[4] - coordX[3]) - 2, (coordY[1] - coordY[0]) - 2));
            ImageBlackQueen = filterCrop.Apply(image);
            ImageBlackQueen.Save("result_black_queen.png");


            //King
            filterCrop = new Crop(new Rectangle(coordX[4] + 1, coordY[7] + 1, (coordX[5] - coordX[4]) - 2, (coordY[8] - coordY[7]) - 2));
            ImageWhiteKing = filterCrop.Apply(image);
            ImageWhiteKing.Save("result_white_king.png");
            
            filterCrop = new Crop(new Rectangle(coordX[4] + 1, coordY[0] + 1, (coordX[5] - coordX[4]) - 2, (coordY[1] - coordY[0]) - 2));
            ImageBlackKing = filterCrop.Apply(image);
            ImageBlackKing.Save("result_black_king.png");

        }

        private void AnalysePiece(Bitmap image, List<int> coordX,  List<int> coordY)
        {
            char[] board = new char[64];
            
            if ((coordX.Count == 9) && (coordY.Count == 9))
            {
                for (int xx = 0; xx < coordX.Count - 1; xx++)
                    for (int yy = 0; yy < coordY.Count - 1; yy++)
                    {
                        // create filter
                        Crop filterCrop = new Crop(new Rectangle(coordX[xx] , coordY[yy], (coordX[xx + 1] - coordX[xx]), (coordY[yy + 1] - coordY[yy]) ));
                        // apply the filter
                        Bitmap ImageCrop = filterCrop.Apply(image);
                        ImageCrop.Save("result_" + (xx + 1) + "_" + (8 - yy) + ".png");

                        if(isWhitePawn(ref ImageCrop)){
                            board[8*yy+xx]='P';
                        }
                        else if(isBlackPawn(ref ImageCrop)){
                            board[8 * yy + xx] = 'p';
                        }
                        else if (isWhiteRook(ref ImageCrop)){
                            board[8 * yy + xx] = 'R';
                        }
                        else if (isBlackRook(ref ImageCrop)){
                            board[8 * yy + xx] = 'r';
                        }
                        else if (isWhiteKnight(ref ImageCrop)){
                            board[8 * yy + xx] = 'K';
                        }
                        else if (isBlackKnight(ref ImageCrop)){
                            board[8 * yy + xx] = 'k';
                        }
                        else if (isWhiteBeshop(ref ImageCrop)){
                            board[8 * yy + xx] = 'B';
                        }
                        else if (isBlackBeshop(ref ImageCrop)){
                            board[8 * yy + xx] = 'b';
                        }
                        else if (isWhiteQueen(ref ImageCrop)){
                            board[8 * yy + xx] = 'Q';
                        }
                        else if (isBlackQueen(ref ImageCrop)){
                            board[8 * yy + xx] = 'q';
                        }
                        else if (isWhiteKing(ref ImageCrop)){
                            board[8 * yy + xx] = 'V';
                        }
                        else if (isBlackKing(ref ImageCrop)){
                            board[8 * yy + xx] = 'v';
                        }
                    }
            }
        }

        #region detection piece
        

        private bool isWhiteKing(ref Bitmap ImageCrop)
        {            
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);            
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteKing);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;                
            }
            return false;
        }

        private bool isBlackKing(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackKing);
            if (matchings[0].Similarity > 0.95f)
            {
                return true;
            }
            return false;
        }

        private bool isWhiteQueen(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteQueen);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }

        private bool isBlackQueen(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackQueen);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }

        private bool isWhitePawn(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhitePawn);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }

        private bool isBlackPawn(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackPawn);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }


        private bool isWhiteRook(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteRook);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }

        private bool isBlackRook(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackRook);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }

        private bool isWhiteBeshop(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteBishop);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }

        private bool isBlackBeshop(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackBishop);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }

        private bool isWhiteKnight(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteKnight);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }

        private bool isBlackKnight(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackKnight);
            if (matchings[0].Similarity > 0.97f)
            {
                return true;
            }
            return false;
        }

        #endregion

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

        private void FindChessBoard(Bitmap image,ref List<int> coordX ,ref List<int> coordY)
        {
            // locating objects
            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 50;
            blobCounter.MinWidth = 50;

            blobCounter.MaxHeight = image.Height / 8;
            blobCounter.MaxWidth = image.Height / 8;
             
            Graphics g = Graphics.FromImage(image);

            // create grayscale filter (BT709)
            Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
            // apply the filter
            Bitmap imageWork = filterG.Apply(image);
            imageWork.Save("resultGrey.png");
            
            // create filter
            Threshold filterBinarisation = new Threshold(ThresholdChessboard);
            // apply the filter
            filterBinarisation.ApplyInPlace(imageWork);
            imageWork.Save("resultBinarisation.png");

            // create filter
            Erosion filterErosion = new Erosion();
            // apply the filter
            filterErosion.ApplyInPlace(imageWork);
            imageWork.Save("resultErosion.png");



            blobCounter.ProcessImage(imageWork);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // check for rectangles
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
 

            foreach (var blob in blobs)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);
                List<IntPoint> cornerPoints;

                // use the shape checker to extract the corner points
                if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints))
                {
                    // only do things if the corners form a rectangle
                    if (shapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Square)
                    {
                        // here i use the graphics class to draw an overlay, but you
                        // could also just use the cornerPoints list to calculate your
                        // x, y, width, height values.
                        List<Point> Points = new List<Point>();
                        foreach (var point in cornerPoints)
                        {
                            coordX.Add(point.X);
                            coordY.Add(point.Y);
                            Points.Add(new Point(point.X, point.Y));
                        }
                        //g.DrawPolygon(new Pen(Color.Red, 1.0f), Points.ToArray());

                    }
                }
            }

            coordX=FindBoundary(coordX);
            coordY=FindBoundary(coordY);
         
           
        }

        private List<int> FindBoundary(List<int> coords)
        {
            List<Point> Points = new List<Point>(); ; //x will be the coordinate and Y the number of occurence

            coords.Sort();

            foreach (var coord in coords)
            {
                Points.Add(new Point(coord, 1));
            }


            bool stop = false;

            while (!stop)
            {
                stop = true;
                for (int ii = 0; ii < Points.Count - 1; ii++)
                {
                    if (Math.Abs(Points[ii].X - Points[ii + 1].X) < 5)
                    {
                        Point tmp = new Point((Points[ii].X + Points[ii + 1].X) / 2, Points[ii].Y + Points[ii + 1].Y);
                        Points[ii] = tmp;
                        Points.RemoveAt(ii + 1);
                        stop = false;
                    }
                    
                }
            }

            List<int> res = new List<int>();

            foreach (var p in Points)
            {
                res.Add(p.X);
            }
            res.Sort();
            return res;
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(61, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 59);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(61, 124);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(122, 52);
            this.button2.TabIndex = 1;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonCalibrate_Click);
            
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.ResumeLayout(false);

        }
    }
}
