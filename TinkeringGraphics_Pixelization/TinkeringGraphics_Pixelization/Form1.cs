using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace TinkeringGraphics_Pixelization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

         // Finds an average color of a specified area of a bitmap
        public static Color GetAverageColor(Bitmap bmp, int startX, int startY, int maxW, int maxH)
        {
            Color p;
            int totalR = 0, totalG = 0, totalB = 0, avgR, avgG, avgB, numberOfPixels = (maxW - startX) * (maxH - startY);
            for (int x = startX; x < maxW; x++)
                for (int y = startY; y < maxH; y++)
                {
                    p = bmp.GetPixel(x, y);
                    totalR += p.R;
                    totalG += p.G;
                    totalB += p.B;

                }

            avgR = totalR / numberOfPixels;
            avgG = totalG / numberOfPixels;
            avgB = totalB / numberOfPixels;

            return Color.FromArgb(150, avgR, avgG, avgB);
        }

        public static Bitmap Collage(Bitmap bmp, int startX, int startY, int maxW, int maxH, Color p)
        {
            /* setting all the pixels in the zone to the color of the middle one
                in a nested loop */
            for (int x = startX; x < maxW; x++)
                for (int y = startY; y < maxH; y++)
                    bmp.SetPixel(x, y, p);
            return bmp;
        }

        public static Bitmap Pixelization(Bitmap bmp, int numberOfPixels) //based on our previous experiments implementing the Posterization algorithm
        {
            int intensityX = bmp.Width / numberOfPixels; //number of zones for X coordinates
            int intensityY = bmp.Height / numberOfPixels; //number of zones for Y coordinates
            
            Color p;

            /*  Iteration in a nested loop over all the stages we have
             *  the stages are based on the number of pixels we want in each of them
                and the width and height of the picture are divided into the intensity variables
                we are letting it run one more time than the intensity so it finishes the last rows
                that don't have the full stage size*/
            for (int stageY = 1; stageY <= intensityY+1; stageY++) {      
                for (int stageX = 1; stageX <= intensityX+1; stageX++)
                {
                    int zoneW = numberOfPixels * stageX; //width of the zone we are working with
                    int zoneH = numberOfPixels * stageY; //height of the zone we are working with

                    if (zoneW > bmp.Width)  //in case we are at the end of the picture width
                        zoneW = bmp.Width - 1;

                    if (zoneH > bmp.Height)  //in case we are at the end of the picture height
                        zoneH = bmp.Height - 1;

                    int startX = numberOfPixels * (stageX - 1); 
                    int startY = numberOfPixels * (stageY - 1); //moving the starting point 

                    p = GetAverageColor(bmp, startX, startY, zoneW, zoneH); //getting average color of the given area
                    bmp = Collage(bmp, startX, startY, zoneW, zoneH, p); //setting the given area with the average color
                }
            }
            return bmp;
        }

        /*
         * Blending algorithm with alpha values and adjustable ratio
         */
        public static Bitmap Blending(Bitmap OrigBmp, double ratio)
        {

            Bitmap PixBmp = new Bitmap(OrigBmp);
            PixBmp = Pixelization(PixBmp, 50); //to avoid problems with referencing
            Color PixClr, OrigClr;
            double newR, newG, newB;

            // Checking if we can work with the ratio - it needs to be 0.0 < ratio < 1.0
            if (ratio > 1 || ratio < 0)
            {
                Console.WriteLine("Ratio of the blending must be between 0 and 1");
                return null;
            }
            
            /*
             * Nested for loop that will blend the two images together 
             */
            for (int x = 0; x < OrigBmp.Width; x++)
                for (int y = 0; y < OrigBmp.Height; y++)
                {
                    PixClr = PixBmp.GetPixel(x, y);
                    OrigClr = OrigBmp.GetPixel(x, y);
                    newR = PixClr.R * ratio + OrigClr.R * (1 - ratio);
                    newG = PixClr.G * ratio + OrigClr.G * (1 - ratio);
                    newB = PixClr.B * ratio + OrigClr.B * (1 - ratio);
                    OrigBmp.SetPixel(x, y, Color.FromArgb(255, (int)newR, (int)newG, (int)newB));
                }
            return OrigBmp;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string filename = "test1.jpg"; //Picture which must be located at TinkeringGraphics_Pixelization\TinkeringGraphics_Pixelization\bin\Debug

            string path = Path.Combine(Environment.CurrentDirectory + Path.DirectorySeparatorChar + filename);
            Bitmap image = new Bitmap(path);

            Image.Image = Blending(image, 0.5); //call of the method takes the image in Bitmap and the number of pixels we want in one block + the ratio of blending
            Image.Image.Save($"pixelated{filename}"); 

        }
    }
}