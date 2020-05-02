using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Resources;

namespace RaspenGames
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private int boolToByte(bool a) 
        {
            if (a)
                return 1;
            return 0;
        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        private byte[] encrypt(byte[] data)
        {        
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.GenerateIV();
                aes.GenerateKey();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, encryptor);
                }
            }
        }

        private byte[] toByte(string text)
        {
            List<bool> mass = new List<bool>();
            for(int i = 0;i<text.Length;i++)
            {
                if (text[i] == '0')
                    mass.Add(false);
                else
                    mass.Add(true);
            }
            int length;
            length = mass.Count / 8;
            if(mass.Count%8!=0)
            {
                length++;
            }
            byte[] bytemass = new byte[length];
            int counter=0;
            int scouneter = 0;
            for(int i = 0;i<bytemass.Length;i++)
            {
                bytemass[i] = 0;
            }
            
            while(mass.Count!=0)
            {
                if(counter!=8)
                {
                    bytemass[scouneter] +=(byte)(boolToByte(mass.ElementAt(0))<<8-counter - 1);
                    mass.RemoveAt(0);
                }
                else if(counter == 8)
                {
                    counter = -1;
                    scouneter++;
                }
                counter++;
            }
            return bytemass;
        }

        private Pixel[] toPixels(byte[] list) 
        {
            var Length = list.Length/3;
            int ost = list.Length % 3;
            
            Pixel[] pixels = new Pixel[Length];

            int counter=0;
         
            for (int i = 0; i < Length; i++)
            {
                pixels[counter] = new Pixel(list[i*3], list[i*3 + 1], list[i*3 + 2]);
                counter++;
            }
            if(ost != 0)
            {
                switch(ost)
                {
                    case 1:
                        pixels[Length - 1] = new Pixel(list[list.Length-1]);
                        break;
                    case 2:
                        pixels[Length - 1] = new Pixel(list[list.Length - 2], list[list.Length - 1]);
                        break;
                }
            }
            return pixels;
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

        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
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
      

        private void decrypt() 
        {
                      
        }

        private void Draw(Pixel[] pixels) 
        {
            
           
            Photo photo = new Photo(pixels.Length, 1);
            for(int i = 0;i<pixels.Length;i++)
            {
                photo[i, 0] = pixels[i];
            }                   
            Photo2Bitmap(photo).Save(Resources.Source.AbsoluteUri + "image.png",ImageFormat.Png);       
            image.Source = new BitmapImage(new Uri("image.png", UriKind.Relative));     
        }

        private void Decide_Click(object sender, RoutedEventArgs e)
        {
            var mass = toByte(textBoxInput.Text); 
            mass = encrypt(mass);
            var pixels = toPixels(mass);
            Draw(pixels);
            decrypt();
        }
    }
}
