using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DeepFryCore
{
    public class DeepFryUtility
    {
        #region Public Properties
        public string Path = string.Empty;
        #endregion

        #region Parameters
        private long encoderLevel = 0L;

        private int noisePercentage = 0;

        private int redScew = 0;
        private int blueScew = 0;
        private int greenScew = 0;
        #endregion

        #region Init
        public DeepFryUtility()
        {
        }

        public DeepFryUtility(string Path, ulong encoderLevel = 1L, uint redScew = 0, uint blueScew = 0, uint greenScew = 0, uint noisePercentage = 50)
        {
            if (string.IsNullOrEmpty(Path))
                throw new ArgumentException("Parameter '" + nameof(Path) + "' must not be empty.");

            if (encoderLevel > 100)
                throw new ArgumentException("Parameter '" + nameof(encoderLevel) + "' must be less than or equal to 100");

            if (redScew > 255)
                throw new ArgumentException("Parameter '" + nameof(redScew) + "' must be less than 255");

            if (blueScew > 255)
                throw new ArgumentException("Parameter '" + nameof(blueScew) + "' must be less than 255");

            if (greenScew > 255)
                throw new ArgumentException("Parameter '" + nameof(greenScew) + "' must be less than 255");

            if (noisePercentage > 255)
                throw new ArgumentException("Parameter '" + nameof(noisePercentage) + "' must be less than or equal to 100");


            this.Path = Path;
            this.encoderLevel = (long)encoderLevel;
            this.noisePercentage = (int)noisePercentage;
            this.redScew = (int)redScew;
            this.blueScew = (int)blueScew;
            this.greenScew = (int)greenScew;
        }
        #endregion

        #region Public Methods
        public Bitmap DeepFry()
        {
            if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
                throw new ArgumentException("'" + nameof(Path) + "' does not exist at path or path is Invalid.");

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

                            // If the pixel is some variation of black and white, make it a raindom color in the red spectrum. 
                            // This doesnt need to scew red, I just thought it looked better.
                            if (Math.Abs(pixel.R - pixel.G) <= 20 &&
                                Math.Abs(pixel.R - pixel.B) <= 20 &&
                                Math.Abs(pixel.G - pixel.B) <= 20)
                            {
                                Color newPixel = Color.FromArgb(rnd.Next(redScew, 255), rnd.Next(greenScew, 255), rnd.Next(blueScew, 255));

                                bitmap.SetPixel(x, y, newPixel);
                            }
                            // else make the transpose the colors so it isnt entirely random. (this is messed up on purpose)
                            else
                            {
                                Color newPixel = Color.FromArgb(pixel.G, pixel.B, pixel.R);

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

        public void Save(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new NullReferenceException("Parameter '" + nameof(bitmap) + "' does not exist at path or path is Invalid.");

            try
            {
                // Basic encoder setup
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                Encoder myEncoder = Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, encoderLevel);
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
        #endregion
    }
}
