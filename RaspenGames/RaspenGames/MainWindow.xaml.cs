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
using RaspenGames.Additional;

namespace RaspenGames
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Photo _photo;

        private static byte[] Key;

        private static byte[] IV;

        private static byte[] ParseIntToByteArray(int value)
        {
            var result = new byte[4];

            var counter = 3;

            while (value > 0)
            {
                result[counter] = (byte)value;
                value >>= 8;
                counter--;
            }
            return result;
        }

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
            var pixels = Library.ReadPixels(_photo);
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

        private static Photo MakePhoto(Pixel[] pixels)
        {
            Photo photo;

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
                for (int i = 0; i < photo.Width; i++)
                {
                    for (int j = 0; j < photo.Height; j++)
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

            return photo;
        }

        private void Draw(Pixel[] pixels)
        {
            _photo = MakePhoto(pixels);
            image.Source = Library.BitmapToBitmapImage(Library.Photo2Bitmap(_photo));
        }

        private void Decide_Click(object sender, RoutedEventArgs e)
        {
            var bytes = false;
            for (int i = 0; i < textBoxInput.Text.Length; i++)
                if (textBoxInput.Text[i] != '0' && textBoxInput.Text[i] != '1')
                {
                    bytes = true;
                }
            byte[] mass;
            if (bytes)
                mass = Library.fromStringToByte(textBoxInput.Text);
            else
                mass = Library.toByte(textBoxInput.Text);
            mass = encrypt(mass);
            var pixels = Library.toPixels(mass);
            Draw(pixels);
            decrypt();
            ButtonSaveFile.IsEnabled = true;
        }

        private static DefaultDialogService _dialog = new DefaultDialogService();

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            if(_dialog.OpenFileDialog())
            {
                new DecodingWindow(MakePhoto(Library.toPixels(File.ReadAllBytes(_dialog.File)))).Show();               
            }
        }

        private void ButtonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (_dialog.SaveFileDialog())
            {
                var data = _photo.Serialize();
                
                using (var bw = new BinaryWriter(new FileStream(_dialog.File, FileMode.Create, FileAccess.Write)))
                {
                    foreach (var array in data)
                        bw.Write(array);
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] array;
            CheckBoxFirst.Visibility = Visibility.Visible;
            CheckBoxSecond.Visibility = Visibility.Visible;
            if(_dialog.OpenFileDialog())
            {
                array = File.ReadAllBytes(_dialog.File);
                CheckBoxFirst.IsChecked = true;
                if(_dialog.OpenFileDialog())
                {
                    CheckBoxSecond.IsChecked = true;
                    if(array.SequenceEqual(File.ReadAllBytes(_dialog.File)))
                    {
                        MessageBox.Show("Картинки идентичны.");
                    }
                    else
                        MessageBox.Show("Картинки разные.");
                }
            }
            CheckBoxFirst.Visibility = Visibility.Hidden;
            CheckBoxSecond.Visibility = Visibility.Hidden;
        }
    }
}
