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
    class Patient
    {
        public enum State { InQueue, InReception, Recovering, Recovered };
        public int Id { get; set; }
        public int DeasesLevel { get; set; }
        public State CurrentState { get; set; }
        public Image Img { get; set; }
        public static string imgpath = @"patient.png";
        public static Image TestImg = Image.FromFile(imgpath);
        public int Time { get; set; }
        public int TimeInWard { get; set; }
        public bool Moving { get; set; }

        public Patient() { }

        public Patient(int id, int deasesLevel)
        {
            this.Id = id;
            this.DeasesLevel = deasesLevel;
            this.CurrentState = State.InQueue;
            this.Img = Image.FromFile(imgpath);
            this.Time = 0;
            this.TimeInWard = 0;
            this.Moving = false;
        }

        public Point move(int x1, int y1, int x2, int y2)
        {
            int nextX = (int)(x2 - x1) / 10;
            int nextY = (int)(y2 - y1) / 10;

            return new Point(nextX, nextY);
        }

        public bool checkIfRecovered()
        {
            if (this.Time > 50000 && this.DeasesLevel == 5) return true;
            if (this.Time > 40000 && this.DeasesLevel == 4) return true;
            if (this.Time > 30000 && this.DeasesLevel == 3) return true;
            if (this.Time > 20000 && this.DeasesLevel == 2) return true;
            if (this.Time > 10000 && this.DeasesLevel == 1) return true;
            return false;
        }

        public override string ToString()
        {
            return this.DeasesLevel.ToString();
        }
    }
}
