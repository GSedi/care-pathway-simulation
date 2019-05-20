using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cps
{
    class Hospital
    {
        public List<Doctor> Doctors { get; set; }
        public List<Ward> Wards { get; set; }
        public Queue<Patient> Patients { get; set; }

        public Hospital()
        {
            this.Doctors = new List<Doctor>();
            this.Wards = new List<Ward>();
            this.Patients = new Queue<Patient>();
            this.initialize();
        }


        public void initialize()
        {
            for(int i = 1; i <= 4; i++)
            {
                this.Doctors.Add(new Doctor(i));
            }
            addRandomPatients();
            for(int i = 0; i < 20; i++)
            {
                this.Wards.Add(new Ward(i));
            }

           
        }

        public void addRandomPatients()
        {
            Random rnd = new Random();

            Queue<Patient> tempPatients = new Queue<Patient>();

            int lim = rnd.Next(1, 15);
            for(int i = 0; i < lim; i++ )
            {
                int id = rnd.Next(0, 1000);
                do
                {
                    id = rnd.Next(0, 1000);
                    
                } while (((this.Patients.Where(p => p.Id == id)).ToList<Patient>()).Count  > 0);

                //this.Patients.Enqueue(new Patient(id, rnd.Next(1, 5)));
                tempPatients.Enqueue(new Patient(id, rnd.Next(1, 5)));
            }
            this.Patients = new Queue<Patient>(tempPatients.OrderBy(q => q.DeasesLevel).Reverse());
            tempPatients.Clear();
        }

        public Dictionary<Ward, PictureBox> setDictWards(List<PictureBox> pbs)
        {
            Dictionary<Ward, PictureBox> dwpb = new Dictionary<Ward, PictureBox>();

            for (int i = 0; i < 20; i++)
            {
                dwpb.Add(this.Wards[i], pbs[i]);
            }

            return dwpb;
        }

    }
}
