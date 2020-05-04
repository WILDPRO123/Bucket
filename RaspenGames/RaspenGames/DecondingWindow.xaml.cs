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
        }

        private byte[] fromStringToByte(string text)
        {
            var line = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] list = new byte[line.Length];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = getByteFromString(line[i]);
            }
            return list;
        }

        private void Decide_Click(object sender, RoutedEventArgs e)
        {
            byte[] key = fromStringToByte(textBoxKey.Text);
            byte[] IV = fromStringToByte(textBoxIV.Text);
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = IV;
            aes.KeySize = aes.Key.Length;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.Zeros;
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                result = Library.PerformCryptography(data, decryptor);
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(i.ToString("X"));
                if (i != result.Length - 1)
                    sb.Append(',');
            }
            textBoxResult.Text = sb.ToString();
        }
    }
}
