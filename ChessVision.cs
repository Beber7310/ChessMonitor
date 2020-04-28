using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;

namespace ChessMonitor
{
    class ChessVision
    {

        public ChessVision()
        {
            loadCalibratePiece();
        }

        private int ThresholdChessboard = 200;

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

        private Rectangle chessboardCoord;

        

        public bool ChessboardRead(Bitmap captureBitmap, ref char[] board)
        {
            bool res = false;
            List<int> coordX = new List<int>();
            List<int> coordY = new List<int>();



            var watch = System.Diagnostics.Stopwatch.StartNew();
            {
                FindChessBoard(captureBitmap, ref coordX, ref coordY);
            }
            watch.Stop();
            Console.WriteLine("FindChessBoard  execution time:" + watch.ElapsedMilliseconds);


            if ((coordX.Count == 9) && (coordY.Count == 9))
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
                {
                    removeBackground(ref captureBitmap);
                    AnalysePiece(captureBitmap, coordX, coordY, ref board);
                }
                watch.Stop();
                Console.WriteLine("AnalysePiece execution time:" + watch.ElapsedMilliseconds);

                chessboardCoord = new Rectangle(coordX[0], coordY[0], coordX[8] - coordX[0], coordY[8] - coordY[0]);
                res = true;
            }
            else
            {
                res = false;
                Console.WriteLine("Unable to find the chessboard");
            }

            return res;
        }

        public bool ChessboardCalibrate(Bitmap captureBitmap)
        {
            List<int> coordX = new List<int>();
            List<int> coordY = new List<int>();

            FindChessBoard(captureBitmap, ref coordX, ref coordY);
            if ((coordX.Count == 9) && (coordY.Count == 9))
            {
                removeBackground(ref captureBitmap);
                CalibratePiece(captureBitmap, coordX, coordY);
                return true;
            }
            return false;
        }

        public void MovePiece(string szMove)
        {
            int start_x, start_y, end_x, end_y;

                 
            int sx, sy, ex, ey;

            start_x = szMove[0] - 'a';
            start_y = szMove[1] - '1';
            end_x = szMove[2] - 'a';
            end_y = szMove[3] - '1';


            int s_screenX, s_screenY;
            int e_screenX, e_screenY;

            if (chessboardCoord != null)
            {
                s_screenX = chessboardCoord.Left + (chessboardCoord.Width / 16) + (chessboardCoord.Width * start_x) / 8;
                s_screenY = chessboardCoord.Bottom - (chessboardCoord.Height / 16) - (chessboardCoord.Height * start_y) / 8;

                e_screenX = chessboardCoord.Left + (chessboardCoord.Width / 16) + (chessboardCoord.Width * end_x) / 8;
                e_screenY = chessboardCoord.Bottom - (chessboardCoord.Height / 16) - (chessboardCoord.Height * end_y) / 8;

                System.Windows.Forms.Screen[] scrn = System.Windows.Forms.Screen.AllScreens;

                s_screenX += scrn[0].Bounds.X;
                s_screenY += scrn[0].Bounds.Y;

                e_screenX += scrn[0].Bounds.X;
                e_screenY += scrn[0].Bounds.Y;


                MouseOperations.SetCursorPosition(s_screenX, s_screenY);
                System.Threading.Thread.Sleep(200);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                System.Threading.Thread.Sleep(200);
                MouseOperations.SetCursorPosition((e_screenX + s_screenX) / 2, (e_screenY + e_screenY) / 2);
                System.Threading.Thread.Sleep(200);
                MouseOperations.SetCursorPosition(e_screenX, e_screenY);
                System.Threading.Thread.Sleep(200);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            }
            else
            {
                //error
            }
        }

        private void loadCalibratePiece()
        {
            try
            {
                ImageEmptyBlack = AForge.Imaging.Image.FromFile("result_EmptyBlack.png");
                ImageEmptyWhite = AForge.Imaging.Image.FromFile("result_EmptyWhite.png");

                ImageWhitePawn = AForge.Imaging.Image.FromFile("result_white_pawn.png");
                ImageBlackPawn = AForge.Imaging.Image.FromFile("result_black_pawn.png");

                //rook          
                ImageWhiteRook = AForge.Imaging.Image.FromFile("result_white_rook.png");
                ImageBlackRook = AForge.Imaging.Image.FromFile("result_black_rook.png");

                //Knight           
                ImageWhiteKnight = AForge.Imaging.Image.FromFile("result_white_knight.png");
                ImageBlackKnight = AForge.Imaging.Image.FromFile("result_black_knight.png");

                //bishop            
                ImageWhiteBishop = AForge.Imaging.Image.FromFile("result_white_bishop.png");
                ImageBlackBishop = AForge.Imaging.Image.FromFile("result_black_bishop.png");

                //queen            
                ImageWhiteQueen = AForge.Imaging.Image.FromFile("result_white_queen.png");
                ImageBlackQueen = AForge.Imaging.Image.FromFile("result_black_queen.png");

                //King            
                ImageWhiteKing = AForge.Imaging.Image.FromFile("result_white_king.png");
                ImageBlackKing = AForge.Imaging.Image.FromFile("result_black_king.png");
            }
            catch (System.IO.FileNotFoundException)
            {

            }
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
            filterCrop = new Crop(new Rectangle(coordX[0] + 1, coordY[6] + 1, (coordX[1] - coordX[0]) - 2, (coordY[7] - coordY[6]) - 2));
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

        private void FindChessBoard(Bitmap image, ref List<int> coordX, ref List<int> coordY)
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

            coordX = FindBoundary(coordX);
            coordY = FindBoundary(coordY);


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

        private bool AnalysePiece(Bitmap image, List<int> coordX, List<int> coordY, ref char[] board)
        {


            if ((coordX.Count == 9) && (coordY.Count == 9))
            {
                for (int xx = 0; xx < coordX.Count - 1; xx++)
                    for (int yy = 0; yy < coordY.Count - 1; yy++)
                    {
                        // create filter
                        Crop filterCrop = new Crop(new Rectangle(coordX[xx], coordY[yy], (coordX[xx + 1] - coordX[xx]), (coordY[yy + 1] - coordY[yy])));
                        // apply the filter
                        Bitmap ImageCrop = filterCrop.Apply(image);
                        ImageCrop.Save("result_" + (xx + 1) + "_" + (8 - yy) + ".png");

                        if (isEmpty(ref ImageCrop))
                        {
                            board[8 * yy + xx] = ' ';
                        }
                        else if (isWhitePawn(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'P';
                        }
                        else if (isBlackPawn(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'p';
                        }
                        else if (isWhiteRook(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'R';
                        }
                        else if (isBlackRook(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'r';
                        }
                        else if (isWhiteKnight(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'N';
                        }
                        else if (isBlackKnight(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'n';
                        }
                        else if (isWhiteBeshop(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'B';
                        }
                        else if (isBlackBeshop(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'b';
                        }
                        else if (isWhiteQueen(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'Q';
                        }
                        else if (isBlackQueen(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'q';
                        }
                        else if (isWhiteKing(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'K';
                        }
                        else if (isBlackKing(ref ImageCrop))
                        {
                            board[8 * yy + xx] = 'k';
                        }
                    }
            }
            else
            {
                return false;
            }


            for (int ii = 0; ii < 8; ii++)
            {
                for (int jj = 0; jj < 8; jj++)
                {
                    Console.Write(board[jj + 8 * ii]);
                }
                Console.WriteLine(" ");
            }
            return true;
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

        private bool isEmpty(ref Bitmap ImageCrop)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageEmptyWhite);
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

    }


}
