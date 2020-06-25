using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RaspenGames
{
    /// <summary>
    /// Логика взаимодействия для DecondingWindow.xaml
    /// </summary>
    public partial class DecondingWindow : Window
    {
        
        

        private byte[] data;

        private byte[] result;

        public DecondingWindow(Photo photo)
        {
            InitializeComponent();
            var pixels = Library.ReadPixels(photo);
            List<byte> mass = new List<byte>();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].Red != 0 || pixels[i].Green != 0 || pixels[i].Blue != 0)
                {
                    mass.Add(pixels[i].Red);
                    mass.Add(pixels[i].Green);
                    mass.Add(pixels[i].Blue);
                }
            }
            while (mass[mass.Count - 1] == 0)
                mass.RemoveAt(mass.Count-1);
            data = new byte[mass.Count];
            for(int i = 0; i < mass.Count; i++)
            {
                data[i] = mass[i];
            }
            Image1.Source = Library.BitmapToBitmapImage(Library.Photo2Bitmap(photo));
        }

        

        private string byteToBoolStr(byte a)
        {
            StringBuilder sb = new StringBuilder();
            while(a>0)
            {
                sb.Append(a % 2);
                a /= 2;
            }
            var length = sb.Length;
            for(int i = sb.Length-1;i>=0;i--)
            {
                sb.Append(sb[i]);
            }
            sb.Remove(0, length);
            if (sb.Length == 0)
                return "0";
            return sb.ToString();
        }

       

        private void Decide_Click(object sender, RoutedEventArgs e)
        {
            byte[] key = Library.fromStringToByte(textBoxKey.Text);
            byte[] IV = Library.fromStringToByte(textBoxIV.Text);
            Aes aes = Aes.Create();
            aes.KeySize = Library.KeySize;
            aes.BlockSize = Library.BlockSize;
            aes.Padding = PaddingMode.Zeros;
            using (var decryptor = aes.CreateDecryptor(key, IV))
            {
                result = Library.PerformCryptography(data, decryptor);
            }
            StringBuilder sb = new StringBuilder();
            for (int i = result.Length-1; i >=0; i--)
            {
                sb.Append(byteToBoolStr(result[i]));
            }
            textBoxResult.Text = sb.ToString();
        }
    }
}
