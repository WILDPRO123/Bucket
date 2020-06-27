using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RaspenGames
{
    public static class Library
    {

        public static readonly int KeySize = 256;
        public static readonly int BlockSize = 128;
        

        private static byte getByteFromChar(char a)
        {
            switch (a)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'a':
                case 'A':
                    return 0x0A;
                case 'b':
                case 'B':
                    return 0x0B;
                case 'c':
                case 'C':
                    return 0x0C;
                case 'd':
                case 'D':
                    return 0x0D;
                case 'e':
                case 'E':
                    return 0x0E;
                case 'f':
                case 'F':
                    return 0x0F;
            }
            MessageBox.Show($"Неправильный адрес(символа {a:X} не существует в шестнадцатиричной системе)");
            return 0;
        }

        private static byte getByteFromString(string a)
        {
            byte b;
            if (a.Length < 2)
            {
                a = '0' + a;
            }

            if (a.Length == 2)
            {
                b = (byte)(getByteFromChar(a[0]) << 4);
                b += getByteFromChar(a[1]);
                return b;
            }

            else
                throw new ArgumentException("Проверьте правильность введения чисел для строки адресс");
        }

        public static byte[] fromStringToByte(string text)
        {
            var line = text.Split(new char[] { ',',' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] list = new byte[line.Length];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = getByteFromString(line[i]);
            }
            return list;
        }

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
                        pixels[Length] = new Pixel(list[list.Length - 1], 255, 255);
                        break;
                    case 2:
                        pixels[Length] = new Pixel(list[list.Length - 2], list[list.Length - 1], 255);
                        break;
                }
            }
            return pixels;
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
            length = (int)Math.Ceiling((double)mass.Count / 8);           
            byte[] bytemass = new byte[length];                       
            var counter = 0;
            var scounter = length-1;
            byte metabyte = 0;
            while (mass.Count > 0)
            {
                if(mass.Last()==true)
                {
                    metabyte += (byte)(1 << counter);
                }    
                if(counter == 7)
                {
                    counter = -1;                   
                    bytemass[scounter] = metabyte;
                    scounter--;
                    metabyte = 0;
                }
                mass.RemoveAt(mass.Count-1);
                counter++;
            }
            if (metabyte != 0)
                bytemass[0] = metabyte;
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
