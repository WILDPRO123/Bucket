using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspenGames
{
    public class Pixel
    {
       public byte Red;
       public byte Green;
       public byte Blue;

        public Pixel() { }
        public Pixel(byte Red) { this.Red = Red; Green = 0; Blue = 0; }
        public Pixel(byte Red,byte Green) { this.Red = Red; this.Green = Green; Blue = 0; }

        public Pixel(byte Red,byte Green,byte Blue) 
        {
            this.Red = Red;
            this.Green = Green;
            this.Blue = Blue;
        }
        public override string ToString()
        {
            return $"{Red:X},{Green:X},{Blue:X}";
        }
    }
}
