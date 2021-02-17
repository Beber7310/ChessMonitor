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
        readonly float compThresold = 0.95f;
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

        public void MovePiece(int screen, string szMove)
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

                s_screenX += scrn[screen].Bounds.X;
                s_screenY += scrn[screen].Bounds.Y;

                e_screenX += scrn[screen].Bounds.X;
                e_screenY += scrn[screen].Bounds.Y;


                MouseOperations.SetCursorPosition(s_screenX, s_screenY);
                System.Threading.Thread.Sleep(200);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                System.Threading.Thread.Sleep(200);

                for (int ii = 0; ii < 10; ii++)
                {
                    int x = ((ii * e_screenX) + ((10 - ii) * s_screenX)) / 10;
                    int y = ((ii * e_screenY) + ((10 - ii) * s_screenY)) / 10;

                    MouseOperations.SetCursorPosition(x, y);
                    System.Threading.Thread.Sleep(20);
                }

                MouseOperations.SetCursorPosition(e_screenX, e_screenY);
                System.Threading.Thread.Sleep(200);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);

                if (szMove.Length > 4)
                {
                    int p_screenX = 0, p_screenY = 0;
                    p_screenX = e_screenX;
                    // promotion!!!!!
                    if (szMove[4] == 'q')
                    {
                        p_screenY = e_screenY;
                    }
                    if (szMove[4] == 'k')
                    {
                        if (end_y == 8)
                            p_screenY = e_screenY + (chessboardCoord.Width * 1) / 8;
                        else
                            p_screenY = e_screenY - (chessboardCoord.Width * 1) / 8;
                    }
                    p_screenY = e_screenY + (chessboardCoord.Width * 1) / 8;
                    if (szMove[4] == 'r')
                    {
                        if (end_y == 8)
                            p_screenY = e_screenY + (chessboardCoord.Width * 2) / 8;
                        else
                            p_screenY = e_screenY - (chessboardCoord.Width * 2) / 8;
                    }
                    if (szMove[4] == 'b')
                    {
                        if (end_y == 8)
                            p_screenY = e_screenY + (chessboardCoord.Width * 3) / 8;
                        else
                            p_screenY = e_screenY - (chessboardCoord.Width * 3) / 8;
                    }
                    System.Threading.Thread.Sleep(400);
                    MouseOperations.SetCursorPosition(p_screenX, p_screenY);
                    System.Threading.Thread.Sleep(100);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    System.Threading.Thread.Sleep(100);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);

                }
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

            int offset = (coordX[1] - coordX[0]) / 8;
            int offsetY = 2;

            //ImageEmptyBlack
            filterCrop = new Crop(new Rectangle(coordX[0] + offset, coordY[5] + offsetY, (coordX[1] - coordX[0]) - 2 * offset, (coordY[6] - coordY[5]) - 2 * offsetY));
            ImageEmptyBlack = filterCrop.Apply(image);
            ImageEmptyBlack.Save("result_EmptyBlack.png");

            //ImageEmptyWhite
            filterCrop = new Crop(new Rectangle(coordX[1] + offset, coordY[5] + offsetY, (coordX[2] - coordX[1]) - 2 * offset, (coordY[6] - coordY[5]) - 2 * offsetY));
            ImageEmptyWhite = filterCrop.Apply(image);
            ImageEmptyWhite.Save("result_EmptyWhite.png");

            //Pawn
            filterCrop = new Crop(new Rectangle(coordX[0] + offset, coordY[6] + offsetY, (coordX[1] - coordX[0]) - 2 * offset, (coordY[7] - coordY[6]) - 2 * offsetY));
            ImageWhitePawn = filterCrop.Apply(image);
            ImageWhitePawn.Save("result_white_pawn.png");
            filterCrop = new Crop(new Rectangle(coordX[0] + offset, coordY[1] + offsetY, (coordX[1] - coordX[0]) - 2 * offset, (coordY[2] - coordY[1]) - 2 * offsetY));
            ImageBlackPawn = filterCrop.Apply(image);
            ImageBlackPawn.Save("result_black_pawn.png");

            //rook
            filterCrop = new Crop(new Rectangle(coordX[0] + offset, coordY[7] + offsetY, (coordX[1] - coordX[0]) - 2 * offset, (coordY[8] - coordY[7]) - 2 * offsetY));
            ImageWhiteRook = filterCrop.Apply(image);
            ImageWhiteRook.Save("result_white_rook.png");
            filterCrop = new Crop(new Rectangle(coordX[0] + offset, coordY[0] + offsetY, (coordX[1] - coordX[0]) - 2 * offset, (coordY[1] - coordY[0]) - 2 * offsetY));
            ImageBlackRook = filterCrop.Apply(image);
            ImageBlackRook.Save("result_black_rook.png");

            //Knight
            filterCrop = new Crop(new Rectangle(coordX[1] + offset, coordY[7] + offsetY, (coordX[2] - coordX[1]) - 2 * offset, (coordY[8] - coordY[7]) - 2 * offsetY));
            ImageWhiteKnight = filterCrop.Apply(image);
            ImageWhiteKnight.Save("result_white_knight.png");
            filterCrop = new Crop(new Rectangle(coordX[1] + offset, coordY[0] + offsetY, (coordX[2] - coordX[1]) - 2 * offset, (coordY[1] - coordY[0]) - 2 * offsetY));
            ImageBlackKnight = filterCrop.Apply(image);
            ImageBlackKnight.Save("result_black_knight.png");


            //bishop
            filterCrop = new Crop(new Rectangle(coordX[2] + offset, coordY[7] + offsetY, (coordX[3] - coordX[2]) - 2 * offset, (coordY[8] - coordY[7]) - 2 * offsetY));
            ImageWhiteBishop = filterCrop.Apply(image);
            ImageWhiteBishop.Save("result_white_bishop.png");
            filterCrop = new Crop(new Rectangle(coordX[2] + offset, coordY[0] + offsetY, (coordX[3] - coordX[2]) - 2 * offset, (coordY[1] - coordY[0]) - 2 * offsetY));
            ImageBlackBishop = filterCrop.Apply(image);
            ImageBlackBishop.Save("result_black_bishop.png");

            //queen
            filterCrop = new Crop(new Rectangle(coordX[3] + offset, coordY[7] + offsetY, (coordX[4] - coordX[3]) - 2 * offset, (coordY[8] - coordY[7]) - 2 * offsetY));
            ImageWhiteQueen = filterCrop.Apply(image);
            ImageWhiteQueen.Save("result_white_queen.png");

            filterCrop = new Crop(new Rectangle(coordX[3] + offset, coordY[0] + offsetY, (coordX[4] - coordX[3]) - 2 * offset, (coordY[1] - coordY[0]) - 2 * offsetY));
            ImageBlackQueen = filterCrop.Apply(image);
            ImageBlackQueen.Save("result_black_queen.png");


            //King
            filterCrop = new Crop(new Rectangle(coordX[4] + offset, coordY[7] + offsetY, (coordX[5] - coordX[4]) - 2 * offset, (coordY[8] - coordY[7]) - 2 * offsetY));
            ImageWhiteKing = filterCrop.Apply(image);
            ImageWhiteKing.Save("result_white_king.png");

            filterCrop = new Crop(new Rectangle(coordX[4] + offset, coordY[0] + offsetY, (coordX[5] - coordX[4]) - 2 * offset, (coordY[1] - coordY[0]) - 2 * offsetY));
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
                        g.DrawPolygon(new Pen(Color.Red, 1.0f), Points.ToArray());

                    }
                }
            }

            coordX = filterPos(coordX);
            coordY = filterPos(coordY);

            if (coordX.Count > 2 && coordY.Count > 2)
                g.DrawRectangle(new Pen(Color.Green, 1.0f), coordX[0], coordY[0], coordX[coordX.Count - 1] - coordX[0], coordY[coordY.Count - 1] - coordY[0]);

            image.Save("debug.bmp");
        }

        public List<int> filterPos(List<int> coord)
        {
            int lastCoord = 0;
            int lastCoord1 = 0;
            int lastCoord2 = 0;

            int count = 0;

            List<int> coordRet = new List<int>();
            coord.Sort();
            coord.Add(0);
            for (int ii = 0; ii < coord.Count; ii++)
            {
                if (Math.Abs(coord[ii] - lastCoord) < 20)
                {
                    count++;
                }
                else if (count > 5)
                {
                    coordRet.Add(lastCoord2);
                    count = 0;
                }
                else
                {
                    count = 0;
                }

                lastCoord2 = lastCoord1;
                lastCoord1 = lastCoord;
                lastCoord = coord[ii];
            }

            return coordRet;
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
            filter.FillColor = new HSL(0, 0, 0.5);            // apply the filter
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
                        //ImageCrop.Save("result_" + (xx + 1) + "_" + (8 - yy) + ".png");

                        float lvl = 0.0f;
                        float ret = 0.0f;
                        parseAllPiece(board, xx, yy, ref ImageCrop, ref lvl, ref ret);

                    }
            }
            else
            {
                return false;
            }

            /*
            for (int ii = 0; ii < 8; ii++)
            {
                for (int jj = 0; jj < 8; jj++)
                {
                    Console.Write(board[jj + 8 * ii]);
                }
                Console.WriteLine(" ");
            }
            */
            return true;


        }

        void parseAllPiece(char[] board, int xx, int yy, ref Bitmap ImageCrop, ref float lvl, ref float ret)
        {
            isEmpty(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = ' ';
                if (ret > 0.999)
                    return;
            }


            board[8 * yy + xx] = ' ';

            isWhitePawn(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'P';
            }

            isBlackPawn(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'p';
            }
            isWhiteRook(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'R';
            }
            isBlackRook(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'r';
            }
            isWhiteKnight(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'N';
            }
            isBlackKnight(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'n';
            }
            isWhiteBeshop(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'B';
            }
            isBlackBeshop(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'b';
            }
            isWhiteQueen(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'Q';
            }
            isBlackQueen(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'q';
            }
            isWhiteKing(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'K';
            }
            isBlackKing(ref ImageCrop, ref ret);
            if (ret > lvl)
            {
                lvl = ret;
                board[8 * yy + xx] = 'k';
            }
        }
        #region detection piece

        private bool isWhiteKing(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteKing);
            lvl = matchings[0].Similarity;
            /* if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
             {
                 Console.WriteLine("image comp not clear");
             }*/

            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isBlackKing(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackKing);
            lvl = matchings[0].Similarity;
            /*if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
            {
                Console.WriteLine("image comp not clear");
            }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isWhiteQueen(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteQueen);
            lvl = matchings[0].Similarity;
            /* if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
             {
                 Console.WriteLine("image comp not clear");
             }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isBlackQueen(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackQueen);
            lvl = matchings[0].Similarity;
            /* if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
             {
                 Console.WriteLine("image comp not clear");
             }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isWhitePawn(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhitePawn);
            lvl = matchings[0].Similarity;
            /* if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
             {
                 Console.WriteLine("image comp not clear");
             }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isEmpty(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageEmptyWhite);
            lvl = matchings[0].Similarity;
            /*if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
            {
                Console.WriteLine("image comp not clear");
            }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }



        private bool isBlackPawn(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackPawn);
            lvl = matchings[0].Similarity;
            /*       if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
                   {
                       Console.WriteLine("image comp not clear");
                   }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }


        private bool isWhiteRook(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteRook);
            lvl = matchings[0].Similarity;
            /*if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
            {
                Console.WriteLine("image comp not clear");
            }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isBlackRook(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackRook);
            lvl = matchings[0].Similarity;
            /* if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
             {
                 Console.WriteLine("image comp not clear");
             }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isWhiteBeshop(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteBishop);
            lvl = matchings[0].Similarity;
            /* if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
             {
                 Console.WriteLine("image comp not clear");
             }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isBlackBeshop(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackBishop);
            lvl = matchings[0].Similarity;
            /*if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
            {
                Console.WriteLine("image comp not clear");
            }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isWhiteKnight(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageWhiteKnight);
            lvl = matchings[0].Similarity;
            /*if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
            {
                Console.WriteLine("image comp not clear");
            }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        private bool isBlackKnight(ref Bitmap ImageCrop, ref float lvl)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            TemplateMatch[] matchings = tm.ProcessImage(ImageCrop, ImageBlackKnight);
            lvl = matchings[0].Similarity;
            /*if ((matchings[0].Similarity < compThresold) && (matchings[0].Similarity > (compThresold - 0.1f)))
            {
                Console.WriteLine("image comp not clear");
            }*/
            if (matchings[0].Similarity > compThresold)
            {
                return true;
            }
            return false;
        }

        #endregion

    }


}
