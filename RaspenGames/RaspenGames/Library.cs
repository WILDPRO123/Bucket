using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RaspenGames
{
    public static class Library
    {
        public static Pixel[] toPixels(byte[] list)
        {
            int Length = (int)Math.Ceiling((double)list.Length / 3);
            int ost = list.Length % 3;
            Pixel[] pixels;
            pixels = new Pixel[Length];

            int counter = 0;

            if (ost != 0)
                Length--;

            for (int i = 0; i < Length; i++)
            {
                pixels[counter] = new Pixel(list[i * 3], list[i * 3 + 1], list[i * 3 + 2]);
                counter++;
            }
            if (ost != 0)
            {
                switch (ost)
                {
                    case 1:
                        pixels[Length] = new Pixel(list[list.Length - 1], 0, 0);
                        break;
                    case 2:
                        pixels[Length] = new Pixel(list[list.Length - 2], list[list.Length - 1], 0);
                        break;
                }
            }
            return pixels;
        }


        private static int boolToByte(bool a)
        {
            if (a)
                return 1;
            return 0;
        }

        public static byte[] toByte(string text)
        {
            List<bool> mass = new List<bool>();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '0')
                    mass.Add(false);
                else
                    mass.Add(true);
            }
            int length;
            length = mass.Count / 8;
            if (mass.Count % 8 != 0)
            {
                length++;
            }
            byte[] bytemass = new byte[length];
            int counter = 0;
            int scouneter = 0;
            for (int i = 0; i < bytemass.Length; i++)
            {
                bytemass[i] = 0;
            }

            while (mass.Count != 0)
            {
                if (counter != 8)
                {
                    bytemass[scouneter] += (byte)(boolToByte(mass.ElementAt(0)) << 8 - counter - 1);
                    mass.RemoveAt(0);
                }
                else if (counter == 8)
                {
                    counter = -1;
                    scouneter++;
                }
                counter++;
            }
            return bytemass;
        }

        public static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        public static Bitmap Photo2Bitmap(Photo photo)
        {
            var bmp = new Bitmap(photo.width, photo.height);
            for (int x = 0; x < bmp.Width; x++)
                for (int y = 0; y < bmp.Height; y++)
                    bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(
                       photo[x, y].Red,
                       photo[x, y].Green,
                        photo[x, y].Blue));
            return bmp;
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
        public static Pixel[] ReadPixels(Photo photo)
        {
            Pixel[] pixels = new Pixel[photo.height * photo.width];
            int counter = 0;
            for (int i = 0; i < photo.width; i++)
            {
                for (int j = 0; j < photo.height; j++)
                {
                    pixels[counter] = photo[i, j];
                    counter++;
                }
            }
            return pixels;
        }
    }
}
