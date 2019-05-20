using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cps
{
    public partial class Form1 : Form
    {
        List<PictureBox> pb;
        List<PictureBox> wards;
        Hospital hospital;
        Dictionary<Ward, PictureBox> dwpb;
        List<PictureBox> pbp;
        Dictionary<Patient, PictureBox> dppb;
        Dictionary<Patient, Ward> dpw;
        Dictionary<Doctor, PictureBox> ddpb;
        Dictionary<Patient, Doctor> dpd;
        Dictionary<Doctor, Ward> ddw;
        List<Patient> recoveredPatients;
        int cnt;
        int timePass = 0;
        int totalWaitingTime = 0;
        int totalWaitingTimeInWard = 0;
        int maxiInQ = -1;
        int maxiInWard = -1;
        int patientsInQ = 0;
        int patientsInWard = 0;
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            pb = (this.Controls.OfType<PictureBox>()).ToList<PictureBox>();
            wards = (pb.Where(p => p.Size.Height == 70)).ToList<PictureBox>();
            pbp = new List<PictureBox>();
            dppb = new Dictionary<Patient, PictureBox>();
            dpw = new Dictionary<Patient, Ward>();
            
            hospital = new Hospital();
            dwpb = hospital.setDictWards(wards);
            ddpb = new Dictionary<Doctor, PictureBox>();
            dpd = new Dictionary<Patient, Doctor>();
            ddw = new Dictionary<Doctor, Ward>();
            recoveredPatients = new List<Patient>();

            getDocsFromHospital();
            cnt = 0;
            patientsInQ = hospital.Patients.Count();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox26_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox26_Paint(object sender, PaintEventArgs e)
        {
        }


        private void addPatientPictureBox()
        {
            PictureBox newPicBox = new PictureBox();
            newPicBox.Location = pictureBox26.Location;
            newPicBox.Size = pictureBox26.Size;
            newPicBox.Image = Patient.TestImg;
            this.Controls.Add(newPicBox);
            newPicBox.BringToFront();
            this.addDeasesLevelToPB(newPicBox, hospital.Patients.Peek());
            dppb.Add(hospital.Patients.Dequeue(), newPicBox);
        }


        private void getDocsFromHospital()
        {
            foreach(Doctor d in hospital.Doctors)
            {
                addDoctorPictureBox(d);
            }
        }

        private void addDoctorPictureBox(Doctor d)
        {
            PictureBox newPicBox = new PictureBox();
            newPicBox.Location = pictureBox28.Location;
            newPicBox.Size = pictureBox28.Size;
            newPicBox.Image = Doctor.TestImg;
            this.Controls.Add(newPicBox);
            newPicBox.BringToFront();
            ddpb.Add(d, newPicBox);

        }

        private void addDeasesLevelToPB(PictureBox picBox, Patient pat)
        {
            Label l = new Label();
            l.Location = new Point(0, 0);
            l.BackColor = Color.Black;
            l.ForeColor = Color.White;
            l.Size = new Size(15, 15);
            l.Text = pat.DeasesLevel.ToString();
            picBox.Controls.Add(l);
            l.BringToFront();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (
                hospital.Patients.Count > 0 && 
                (dppb.Where(p => p.Key.CurrentState == Patient.State.InQueue)).Count() == 0 && 
                (dwpb.Where(f => f.Key.Busy == false)).Count() > 0)
            {
                hospital.Patients.Peek().Time = 0;
                //Console.WriteLine(hospital.Patients.Peek());
                addPatientPictureBox();
                cnt++;

            }
            else if(hospital.Patients.Count <= 0)
            {
                hospital.addRandomPatients();
                this.patientsInQ += hospital.Patients.Count();
            }
            else
            {
                foreach(Patient p in hospital.Patients)
                {
                    p.Time += timer1.Interval;
                    totalWaitingTime += timer1.Interval;
                    if(p.Time > maxiInQ)
                    {
                        maxiInQ = p.Time;
                    }
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (dppb.Count > 0)
            {
                foreach (KeyValuePair<Patient, PictureBox> ppb in dppb)
                {
                    if (ppb.Key.CurrentState == Patient.State.InQueue)
                    {
                        
                        Point pos = ppb.Key.move(ppb.Value.Location.X, ppb.Value.Location.Y, pictureBox24.Location.X + 5, pictureBox24.Location.Y - 5);
                        ppb.Value.Location = new Point(ppb.Value.Location.X + pos.X, ppb.Value.Location.Y + pos.Y);
                        if (Math.Abs(ppb.Value.Location.X - pictureBox24.Location.X) < 7 &&  Math.Abs(ppb.Value.Location.Y - pictureBox24.Location.Y) < 7)
                        {
                            ppb.Key.CurrentState = Patient.State.InReception;

                            foreach (KeyValuePair<Ward, PictureBox> wpb in dwpb)
                            {
                                if (!wpb.Key.Busy)
                                {
                                    dpw.Add(ppb.Key, wpb.Key);
                                    wpb.Key.Busy = true;
                                    break;           
                                }
                            }
                        }
                       
                        
                    }
                    else if(ppb.Key.CurrentState == Patient.State.InReception)
                    {
                        if (ppb.Key.Time < 1000)
                        {
                            ppb.Key.Time += 10;
                        }
                        else
                        {
                            if (dpw.ContainsKey(ppb.Key))
                            {

                                ppb.Key.TimeInWard += timer2.Interval;
                                PictureBox curWard = dwpb[dpw[ppb.Key]];
                                if (curWard.Location.Y > ppb.Value.Location.Y)
                                {
                                    Point pos = ppb.Key.move(ppb.Value.Location.X, ppb.Value.Location.Y, curWard.Location.X + 5, curWard.Location.Y + 5);
                                    ppb.Value.Location = new Point(ppb.Value.Location.X + pos.X, ppb.Value.Location.Y + pos.Y);
                                }
                                else if (curWard.Location.Y <= ppb.Value.Location.Y)
                                {
                                    Point pos = ppb.Key.move(ppb.Value.Location.X, ppb.Value.Location.Y, curWard.Location.X + 5, curWard.Location.Y - 5);
                                    ppb.Value.Location = new Point(ppb.Value.Location.X + pos.X, ppb.Value.Location.Y + pos.Y);
                                }

                                if (Math.Abs(ppb.Value.Location.X - curWard.Location.X) < 7 && Math.Abs(ppb.Value.Location.Y - curWard.Location.Y) < 7)
                                {
                                    foreach (KeyValuePair<Doctor, PictureBox> dpb in ddpb)
                                    {
                                        if (!dpb.Key.Busy && 
                                            dppb.Where(kv => kv.Key.CurrentState == Patient.State.InReception).All(kv => ppb.Key.DeasesLevel >= kv.Key.DeasesLevel))
                                        {
                                            ppb.Key.CurrentState = Patient.State.Recovering;
                                            ppb.Key.Time = 0;
                                            dpd.Add(ppb.Key, dpb.Key);
                                            ddw.Add(dpb.Key, dpw[ppb.Key]);
                                            dpb.Key.Busy = true;

                                            break;
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else if(ppb.Key.CurrentState == Patient.State.Recovering )
                    {
                        if (dpd.Keys.Contains(ppb.Key))
                        {
                            ppb.Key.Time += 10;
                        }
                        if (ppb.Key.checkIfRecovered())
                        {
                            if (dpd.Keys.Contains(ppb.Key))
                            {
                                ppb.Key.TimeInWard = 0;
                                dpd[ppb.Key].Busy = false;
                                if (ddw.Keys.Contains(dpd[ppb.Key]))
                                {
                                    ddw.Remove(dpd[ppb.Key]);
                                }
                                dpd.Remove(ppb.Key);
                            }
                            if (dpw.Keys.Contains(ppb.Key))
                            {
                                dpw[ppb.Key].Busy = false;
                                dpw.Remove(ppb.Key);
                            }
                            if (pictureBox25.Location.Y > ppb.Value.Location.Y)
                            {
                                Point pos = ppb.Key.move(ppb.Value.Location.X, ppb.Value.Location.Y, pictureBox25.Location.X + 5, pictureBox25.Location.Y + 5);
                                ppb.Value.Location = new Point(ppb.Value.Location.X + pos.X, ppb.Value.Location.Y + pos.Y);
                            }
                            else if (pictureBox25.Location.Y <= ppb.Value.Location.Y)
                            {
                                Point pos = ppb.Key.move(ppb.Value.Location.X, ppb.Value.Location.Y, pictureBox25.Location.X + 5, pictureBox25.Location.Y - 5);
                                ppb.Value.Location = new Point(ppb.Value.Location.X + pos.X, ppb.Value.Location.Y + pos.Y);
                            }
                            if (Math.Abs(ppb.Value.Location.X - pictureBox25.Location.X) < 7 && Math.Abs(ppb.Value.Location.Y - pictureBox25.Location.Y) < 7)
                            {
                                recoveredPatients.Add(ppb.Key);
                                ppb.Key.CurrentState = Patient.State.Recovered;
                            }

                        }
                    }
                }
                var toRemoveDict = dppb.Where(kv => kv.Key.CurrentState == Patient.State.Recovered).ToArray();
                foreach (var temp in toRemoveDict)
                {
                    dppb.Remove(temp.Key);
                }
                //toRemoveDict = null;
                //Console.WriteLine(dppb.Count());
            }
            if((dwpb.Where(f => f.Key.Busy == false)).Count() > 0)
            {
                foreach(KeyValuePair<Doctor, PictureBox> dpb in ddpb)
                {
                    if(dpb.Key.Busy == true)
                    {
                        PictureBox curWard = dwpb[ddw[dpb.Key]];
                        if (curWard.Location.Y > dpb.Value.Location.Y)
                        {
                            Point pos = dpb.Key.move(dpb.Value.Location.X, dpb.Value.Location.Y, curWard.Location.X + 45, curWard.Location.Y + 5);
                            dpb.Value.Location = new Point(dpb.Value.Location.X + pos.X, dpb.Value.Location.Y + pos.Y);
                        }
                        else if (curWard.Location.Y <= dpb.Value.Location.Y)
                        {
                            Point pos = dpb.Key.move(dpb.Value.Location.X, dpb.Value.Location.Y, curWard.Location.X + 5, curWard.Location.Y - 5);
                            dpb.Value.Location = new Point(dpb.Value.Location.X + pos.X, dpb.Value.Location.Y + pos.Y);
                        }
                    }
                    else if(dpb.Key.Busy == false)
                    {
                        if (pictureBox28.Location.Y > dpb.Value.Location.Y)
                        {
                            Point pos = dpb.Key.move(dpb.Value.Location.X, dpb.Value.Location.Y, pictureBox28.Location.X, pictureBox28.Location.Y + 5);
                            dpb.Value.Location = new Point(dpb.Value.Location.X + pos.X, dpb.Value.Location.Y + pos.Y);
                        }
                        else if (pictureBox28.Location.Y <= dpb.Value.Location.Y)
                        {
                            Point pos = dpb.Key.move(dpb.Value.Location.X, dpb.Value.Location.Y, pictureBox28.Location.X, pictureBox28.Location.Y - 5);
                            dpb.Value.Location = new Point(dpb.Value.Location.X + pos.X, dpb.Value.Location.Y + pos.Y);
                        }
                    }
                }

            }

            foreach (KeyValuePair<Patient, Ward> pw in dpw)
            {
                if (!dpd.Keys.Contains(pw.Key))
                {
                    totalWaitingTimeInWard += timer2.Interval;
                    if (pw.Key.TimeInWard > maxiInWard)
                    {
                        maxiInWard = pw.Key.TimeInWard;
                    }
                }
            }
            patientsInWard = dpw.Where(pw => !dpd.Keys.Contains(pw.Key)).Count();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs != null) MessageBox.Show(string.Format("X: {0} Y: {1}", mouseEventArgs.X, mouseEventArgs.Y));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer2.Enabled = true;
            timer2.Interval = 10;
            timer1.Enabled = true;
            timer1.Interval = 100;
            timer3.Enabled = true;
            timer3.Interval = 1000;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //label10.Text = ((this.patientsInQ) / (totalWaitingTime / 1000)).ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timer2.Interval = (int)(trackBar1.Value * 10) + 10;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timePass += timer3.Interval;
            label5.Text = (timePass / 1000).ToString();
            label8.Text = recoveredPatients.Count.ToString();
            label7.Text = hospital.Patients.Count.ToString();
            label6.Text = cnt.ToString();
            //label10.Text = ((totalWaitingTime / 1000) / (timePass / 1000)).ToString();
            label10.Text = ((totalWaitingTime / 1000) / (patientsInQ)).ToString();
            label11.Text = (totalWaitingTime / 1000).ToString();
            label13.Text = (maxiInQ / 1000).ToString();
            label17.Text = ((totalWaitingTimeInWard / 1000) / (dpw.Where(pw => !dpd.Keys.Contains(pw.Key)).Count())).ToString();
            label15.Text = (totalWaitingTimeInWard / 1000).ToString();
            label19.Text = (maxiInWard / 1000).ToString();

        }
    }
}
