using System;
using System.Windows.Media;
using System.Windows.Forms.VisualStyles;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace RaspenGames.Additional
{
    public class Photo
    {
        public int Width { get; set; }

        public int Height { get; set; }

        private Pixel[,] Data { get; set; }

        public byte[,,] Serialize()
        {
            var data = new byte[Width, Height, 3];

            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    data[i, j, 0] = Data[i, j].Red;
                    data[i, j, 1] = Data[i, j].Green;
                    data[i, j, 2] = Data[i, j].Blue;
                }
            }
            return data;
        }



        public Pixel this[int x, int y]
        {
            get
            {
                return Data[x, y];
            }
            set
            {
                Data[x, y] = value;
            }
        }

        public Photo(int width, int height)
        {
            Width = width;
            Height = height;
            Data = new Pixel[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Data[i, j] = new Pixel();
                }
            }
        }
    }
}