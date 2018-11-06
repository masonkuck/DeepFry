using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace DeepFryCore
{
    public class DeepFryUtility
    {
        #region Public Properties
        public string Path = string.Empty;
        #endregion

        #region Parameters

        #endregion

        #region Init
        public DeepFryUtility()
        {
        }

        public DeepFryUtility(string Path)
        {
            if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
                throw new ArgumentException("'" + nameof(Path) + "' does not exist at path or path is Invalid.");

            this.Path = Path;
        }
        #endregion

        #region Public Methods
        public Bitmap DeepFry(uint RedScew = 0, uint BlueScew = 0, uint GreenScew = 0, uint NoisePercentage = 50)
        {
            if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
                throw new ArgumentException("'" + nameof(Path) + "' does not exist at path or path is Invalid.");

            if (RedScew > 255)
                throw new ArgumentException("Parameter '" + nameof(RedScew) + "' must be less than 255");

            if (BlueScew > 255)
                throw new ArgumentException("Parameter '" + nameof(BlueScew) + "' must be less than 255");

            if (GreenScew > 255)
                throw new ArgumentException("Parameter '" + nameof(GreenScew) + "' must be less than 255");

            if (NoisePercentage > 255)
                throw new ArgumentException("Parameter '" + nameof(NoisePercentage) + "' must be less than or equal to 100");


            int noisePercentage = (int)NoisePercentage;
            int redScew = (int)RedScew;
            int blueScew = (int)BlueScew;
            int greenScew = (int)GreenScew;

            try
            {
                string path = Path;

                Random rnd = new Random();
                Bitmap bitmap = new Bitmap(path);
                for (int y = 0; y < bitmap.Height; y++)
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        // Randomly chooses pixels to make grainy
                        int next = rnd.Next(0, 100);
                        if (next >= noisePercentage)
                        {
                            Color pixel = bitmap.GetPixel(x, y);

                            // If the pixel is some variation of black and white (ish), make it a raindom color scewed by the users input. 
                            if (Math.Abs(pixel.R - pixel.G) <= 20 &&
                                Math.Abs(pixel.R - pixel.B) <= 20 &&
                                Math.Abs(pixel.G - pixel.B) <= 20)
                            {
                                int redRand = rnd.Next(redScew, 255);
                                int greenRand = rnd.Next(greenScew, 255);
                                int blueRand = rnd.Next(blueScew, 255);

                                Color newPixel = Color.FromArgb(redRand, greenRand, blueRand);

                                bitmap.SetPixel(x, y, newPixel);
                            }
                            // else make the transpose the colors randomly
                            else
                            {
                                List<int> colors = new List<int> { pixel.R, pixel.G, pixel.B };

                                Shuffle(colors);

                                Color newPixel = Color.FromArgb(colors[0], colors[1], colors[2]);

                                bitmap.SetPixel(x, y, newPixel);
                            }
                        }
                    }

                byte[] imageByteArray = ImageToByte(bitmap);
                MemoryStream stream = new MemoryStream(imageByteArray);

                return new Bitmap(stream);

            }
            catch (Exception ex)
            {
                // error logging? probably not
                throw ex;
            }
        }

        /// <summary>
        /// THIS IS SUPER WIP, I AM TESTING THINGS SORRY
        /// </summary>
        /// <param name="CircleX"></param>
        /// <param name="CircleY"></param>
        /// <param name="Radius"></param>
        /// <returns></returns>
        public Bitmap WIPDistort(uint CircleX, uint CircleY, uint Radius)
        {
            Bitmap bitmap = new Bitmap(Path);
            Bitmap bitmapBlank = new Bitmap(bitmap.Width, bitmap.Height);

            List<DistortionPoint> pixels = GetPointsInCircle(bitmap, (int)CircleX, (int)CircleY, (int)Radius).OrderBy(x => x.DistanceToOrigin).ToList();
            //pixels = pixels.Where(x => x.DistanceToOrigin > 50).ToList();


            double stepDownRate = pixels.Count / Radius;
            double maxDistance = pixels.Max(x => x.DistanceToOrigin);
            foreach (DistortionPoint point in pixels)
            {
                bool left = false, up = false;
                if (point.XDistance < 0)
                    left = true;

                if (point.YDistance > 0)
                    up = true;

                int factor = (int)Radius / 10;

                if (left)
                {
                    if (up)
                    {
                        bitmap.SetPixel(point.X + factor, point.Y - factor, bitmap.GetPixel(point.X, point.Y));
                        bitmapBlank.SetPixel(point.X + factor, point.Y - factor, bitmap.GetPixel(point.X, point.Y));
                    }
                    else
                    {

                        bitmap.SetPixel(point.X + factor, point.Y + factor, bitmap.GetPixel(point.X, point.Y));
                        bitmapBlank.SetPixel(point.X + factor, point.Y + factor, bitmap.GetPixel(point.X, point.Y));
                    }
                }
                else
                {
                    if (up)
                    {
                        bitmap.SetPixel(point.X - factor, point.Y - factor, bitmap.GetPixel(point.X, point.Y));
                        bitmapBlank.SetPixel(point.X - factor, point.Y - factor, bitmap.GetPixel(point.X, point.Y));
                    }
                    else
                    {
                        bitmap.SetPixel(point.X - factor, point.Y + factor, bitmap.GetPixel(point.X, point.Y));
                        bitmapBlank.SetPixel(point.X - factor, point.Y + factor, bitmap.GetPixel(point.X, point.Y));
                    }
                }

            }

            return bitmap;
        }

        public void Save(Bitmap bitmap, uint EncoderLevel)
        {
            if (bitmap == null)
                throw new NullReferenceException("Parameter '" + nameof(bitmap) + "' does not exist at path or path is Invalid.");

            if (EncoderLevel > 100)
                throw new ArgumentException("Parameter '" + nameof(EncoderLevel) + "' must be less than or equal to 100");

            try
            {
                // Basic encoder setup
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                Encoder myEncoder = Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (int)EncoderLevel);
                myEncoderParameters.Param[0] = myEncoderParameter;

                bitmap.Save(GetRandomPath(), jpgEncoder, myEncoderParameters);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region Private Methods
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private string GetRandomPath()
        {
            string fileName = Path.Substring(0, Path.LastIndexOf("."));
            string extension = Path.Substring(Path.LastIndexOf("."));

            return fileName + Guid.NewGuid() + extension;
        }

        private static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private Random rng = new Random();
        private void Shuffle(List<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        // Get all pixels within a defined cicle.
        // (pointX - cicleX)^2 + (pointY - cicleY)^2 <= radiusSquard
        // basically if the distance of a pixed to the center of the circle is less than or equal 
        // to the radius squared (area of the circle) then it is included in the list
        private List<DistortionPoint> GetPointsInCircle(Bitmap bitmap, int CircleX, int CircleY, int Radius)
        {
            List<DistortionPoint> points = new List<DistortionPoint>();

            double radiusSquared = Math.Pow(Radius, 2);
            for (int ImageX = 0; ImageX < bitmap.Width; ImageX++)
            {
                for (int ImageY = 0; ImageY < bitmap.Height; ImageY++)
                {
                    double dx = ImageX - CircleX;
                    double dy = ImageY - CircleY;
                    double distanceSquared = Math.Pow(dx, 2) + Math.Pow(dy, 2);

                    if (distanceSquared <= radiusSquared)
                    {
                        points.Add(new DistortionPoint(ImageX, ImageY, CircleX, CircleY));
                    }
                }
            }

            return points;
        }
        #endregion
    }
}
