using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaspenGames.Additional
{
    public interface IDialogService
    {
        string[] FilePath { get; set; }   // путь к выбранному файлу

        bool OpenDirectoryDialog();  // открытие файла

        bool OpenFileDialog();
    }

    public class DefaultDialogService
    {
        public string File { get; set; }

        public bool SaveFileDialog()
        {
            var folderBrowser = new Microsoft.Win32.SaveFileDialog() { AddExtension = true, DefaultExt = ".bin", FileName = ".bin",Filter = "Binary file(.bin)|*.bin" };
            
            

            bool result = folderBrowser.ShowDialog() ?? false;
            
            if (result)
            {
                File = folderBrowser.FileName;
                return true;
            }
            return false;
        }

        public bool OpenFileDialog()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            var result = openFileDialog.ShowDialog();
            
            if (result != null && (bool)result)
            {
                File = openFileDialog.FileName;
                return true;
            }
            return false;
        }
    }

}
