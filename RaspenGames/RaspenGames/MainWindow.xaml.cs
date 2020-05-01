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

            if (list.Length % 3 != 0)
                Length++;
            

            Pixel[] pixels = new Pixel[Length];
            int counter=0;
         
            for (int i = 0; i < Length-1; i++)
            {
                pixels[counter] = new Pixel(list[i], list[i + 1], list[i + 2]);
                i += 2;
                counter++;
            }
            return pixels;
        }

        private void decrypt() 
        {
           // Colobutton.Background
        }

        private void Decide_Click(object sender, RoutedEventArgs e)
        {
            var mass = toByte(textBoxInput.Text); 
            mass = encrypt(mass);
            var pixels = toPixels(mass);
          
            var p = pixels[0];
            button.Background = new SolidColorBrush(Color.FromRgb(p.Red,p.Green,p.Blue));
            decrypt();
        }
    }
}
