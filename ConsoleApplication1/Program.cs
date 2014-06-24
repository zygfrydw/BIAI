using System;
using System.Drawing;
using System.Dynamic;
using System.IO;

namespace GenerateFonts
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateNoisedFonts();
            //var gen = new GenerateFonts();
            //gen.Generate();
        }

        private static void GenerateNoisedFonts()
        {
            const int noisePropability = 5;
            const string fontName = "generated";
            var directoryInfo = new DirectoryInfo(@"..\..\Fonts\" + fontName);
            for (int i = 0; i <= 20; i++)
            {
                var outDirectoryName = string.Format("..\\..\\GrayColor\\level{0}\\{1}{2}", noisePropability, fontName, i);
                var outDirectory = new DirectoryInfo(outDirectoryName);
                outDirectory.Create();
                foreach (var file in directoryInfo.GetFiles("*.bmp"))
                {
                    AddNoiseToImage(file.FullName, outDirectory.FullName + "\\" + file.Name, noisePropability);
                }
            }
        }

        private static void AddNoiseToImage(string @from, string to, int noisePropability)
        {
            Bitmap bmp = (Bitmap) Image.FromFile(@from);
            LockBitmap lockBitmap = new LockBitmap(bmp);
            lockBitmap.LockBits();

            Random rand = new Random();
            for (int y = 0; y < lockBitmap.Height; y++)
            {
                for (int x = 0; x < lockBitmap.Width; x++)
                {
                    var pixel = lockBitmap.GetPixel(x, y);
                    var influence = rand.Next(0, noisePropability) == 0 ? 1 : 0;
                    var newColor = GetGrayColor(pixel, influence);
                    lockBitmap.SetPixel(x, y, newColor);
                }
            }
            lockBitmap.UnlockBits();
            bmp.Save(to);
        }

        private static Color GetColorColor(Color pixel, int influence)
        {
            Random rand = new Random();
            var a = Clamp(pixel.A, 0, 255);
            var r = Clamp(pixel.R + (int) (rand.Next(-255, 255)*influence), 0, 255);
            var g = Clamp(pixel.G + (int) (rand.Next(-255, 255)*influence), 0, 255);
            var b = Clamp(pixel.B + (int) (rand.Next(-255, 255)*influence), 0, 255);
            var newColor = Color.FromArgb(a, r, g, b);
            return newColor;
        }
        private static Color GetGrayColor(Color pixel, int influence)
        {
            Random rand = new Random();
            var a = Clamp(pixel.A, 0, 255);
            var color = rand.Next(-255, 255) * influence;
            var r = Clamp(pixel.R + color, 0, 255);
            var g = Clamp(pixel.G + color, 0, 255);
            var b = Clamp(pixel.B + color, 0, 255);
            var newColor = Color.FromArgb(a, r, g, b);
            return newColor;
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }

    
}
