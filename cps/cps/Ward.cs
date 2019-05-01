using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cps
{
    class Ward
    {
        public int Id { get; set; }
        public bool Busy { get; set; }

        public Ward() { }

        public Ward(int id)
        {
            this.Id = id;
            this.Busy = false;
        }
    }
}
