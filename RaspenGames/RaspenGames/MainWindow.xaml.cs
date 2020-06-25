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
        private static Photo photo;
        private static byte[] Key;
        private static byte[] IV;

        public MainWindow()
        {
            InitializeComponent();        
        }
 
        private byte[] encrypt(byte[] data)
        {
            using (var aes = Aes.Create())
            {
                aes.BlockSize = Library.BlockSize;
                aes.KeySize = Library.KeySize;
                aes.Padding = PaddingMode.Zeros;
                aes.GenerateKey();
                aes.GenerateIV();
                Key = aes.Key;
                IV = aes.IV;
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return Library.PerformCryptography(data, encryptor);
                }
            }
        }

        private void decrypt()
        {
            var pixels = Library.ReadPixels(photo);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pixels.Length; i++)
            {
                sb.Append(pixels[i].ToString());
                if (i != pixels.Length - 1)
                {
                    sb.Append(',');
                }
            }
            textBoxOutPut.Text = sb.ToString();
            sb.Clear();
            sb.Append("Key : ");
            for (int i = 0; i < Key.Length; i++)
            {
                sb.Append(Key[i].ToString("X"));
                if (i != Key.Length - 1)
                    sb.Append(',');
            }
            sb.Append('\n');
            sb.Append("IV : \n");
            for (int i = 0; i < IV.Length; i++)
            {
                sb.Append(IV[i].ToString("X"));
                if (i != IV.Length - 1)
                    sb.Append(',');
            }
            textBoxKey.Text = sb.ToString();
        }

        private void Draw(Pixel[] pixels)
        {
            if (pixels.Length < 7)
            {
                photo = new Photo(pixels.Length, 1);
                for (int i = 0; i < pixels.Length; i++)
                {
                    photo[i, 0] = pixels[i];
                }
            }
            else
            {
                double side = Math.Sqrt(pixels.Length);
                int counter = 0;
                photo = new Photo((int)Math.Ceiling(side), (int)Math.Round(side));
                for (int i = 0; i < photo.width; i++)
                {
                    for (int j = 0; j < photo.height; j++)
                    {
                        if (pixels.Length > counter)
                        {
                            photo[i, j] = pixels[counter];
                        }
                        else
                        {
                            photo[i, j] = new Pixel(255, 255, 255);
                        }
                        counter++;
                    }
                }
            }
            image.Source = Library.BitmapToBitmapImage(Library.Photo2Bitmap(photo));
        }

        private void Decide_Click(object sender, RoutedEventArgs e)
        {
            var mass = Library.toByte(textBoxInput.Text);
            mass = encrypt(mass);
            var pixels = Library.toPixels(mass);
            Draw(pixels);
            decrypt();
            DecondingWindow decondingWindow = new DecondingWindow(photo);
            decondingWindow.Show();
        }
    }
}
