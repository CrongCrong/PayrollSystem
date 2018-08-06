using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollSystem.classes
{
    public class ISAPModel
    {

        public string ID { get; set; }

        public string empID { get; set; }

        public string CurrentISAP { get; set; }

        public string DateAdded { get; set; }

        public string ISAPtoAdd { get; set; }

        public string FullName { get; set; }

        public string Remarks { get; set; }
    }
}
