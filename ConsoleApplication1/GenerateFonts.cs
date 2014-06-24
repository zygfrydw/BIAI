using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateFonts
{
    public class GenerateFonts
    {
        class LetterToPrint
        {
            public string Name;
            public int[] Bits;
        }

        private LetterToPrint[] letters = new[]
        {
            new LetterToPrint(){Name = "A", Bits = new []{0, 1, 1, 1, 0,
                                                          1, 0, 0, 0, 1, 
                                                          1, 0, 0, 0, 1, 
                                                          1, 1, 1, 1, 1, 
                                                          1, 0, 0, 0, 1, 
                                                          1, 0, 0, 0, 1,
                                                          1, 0, 0, 0, 1}},
            new LetterToPrint(){Name = "B", Bits = new []{1, 1, 1, 1, 0, 
                                                          1, 0, 0, 0, 1, 
                                                          1, 0, 0, 0, 1, 
                                                          1, 1, 1, 1, 0, 
                                                          1, 0, 0, 0, 1, 
                                                          1, 0, 0, 0, 1, 
                                                          1, 1, 1, 1, 0}},
            new LetterToPrint(){Name = "C", Bits = new []{0, 1, 1, 1, 0, 
                                                          1, 0, 0, 0, 1, 
                                                          1, 0, 0, 0, 0, 
                                                          1, 0, 0, 0, 0, 
                                                          1, 0, 0, 0, 0, 
                                                          1, 0, 0, 0, 1, 
                                                          0, 1, 1, 1, 0}},
            new LetterToPrint(){Name = "D", Bits = new []{1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0}},
            new LetterToPrint(){Name = "E", Bits = new []{1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1}},
            new LetterToPrint(){Name = "F", Bits = new []{1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0}},
            new LetterToPrint(){Name = "G", Bits = new []{0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0}},
            new LetterToPrint(){Name = "H", Bits = new []{1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1}},
            new LetterToPrint(){Name = "I", Bits = new []{0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0}},
            new LetterToPrint(){Name = "J", Bits = new []{1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0}},
            new LetterToPrint(){Name = "K", Bits = new []{1, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1}},
            new LetterToPrint(){Name = "L", Bits = new []{1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1}},
            new LetterToPrint(){Name = "M", Bits = new []{1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1}},
            new LetterToPrint(){Name = "N", Bits = new []{1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1}},
            new LetterToPrint(){Name = "O", Bits = new []{0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0}},
            new LetterToPrint(){Name = "P", Bits = new []{1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0}},
            new LetterToPrint(){Name = "Q", Bits = new []{0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1}},
            new LetterToPrint(){Name = "R", Bits = new []{1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1}},
            new LetterToPrint(){Name = "S", Bits = new []{0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0}},
            new LetterToPrint(){Name = "T", Bits = new []{1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0}},
            new LetterToPrint(){Name = "U", Bits = new []{1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0}},
            new LetterToPrint(){Name = "V", Bits = new []{1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0}},
            new LetterToPrint(){Name = "W", Bits = new []{1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0}},
            new LetterToPrint(){Name = "X", Bits = new []{1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1}},
            new LetterToPrint(){Name = "Y", Bits = new []{1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0}},
            new LetterToPrint(){Name = "Z", Bits = new []{1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1}}
        };
        public void Generate()
        {
            int maxX = 5;
            int maxY = 7;

            foreach (var letter in letters)
            {
                Bitmap bmp = new Bitmap(maxX, maxY);
                LockBitmap lockBitmap = new LockBitmap(bmp);
                lockBitmap.LockBits();
                var outDirectory = new DirectoryInfo(@"..\..\GeneratedFonts");
                outDirectory.Create();
                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        var index = maxX*y + x;
                        var color = (1 - letter.Bits[index]) * 255;
                        var newColor = Color.FromArgb(255, color, color, color);
                        lockBitmap.SetPixel(x, y, newColor);
                    }
                }
                lockBitmap.UnlockBits();
                bmp.Save(@"..\..\GeneratedFonts\" + letter.Name + ".bmp");
            }
        }
    }
}
