using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace cps
{
    class Doctor
    {
        public int Id { get; set; }
        public bool Busy { get; set; }
        public Image Img { get; set; }
        public static string imgpath = @"doctor.png";
        public static Image TestImg = Image.FromFile(imgpath);

        public Doctor() { }

        public Doctor(int id)
        {
            this.Id = id;
            this.Busy = false;
            this.Img = Image.FromFile(imgpath);

        }

        public Point move(int x1, int y1, int x2, int y2)
        {
            int nextX = (int)(x2 - x1) / 10;
            int nextY = (int)(y2 - y1) / 10;

            return new Point(nextX, nextY);
        }
    }
}
