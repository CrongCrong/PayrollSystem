using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollSystem.classes
{
    public class ElectricBillModel
    {

        public string ID { get; set; }

        public string empID { get; set; }

        public string CurrentElecBill { get; set; }

        public string DateAdded { get; set; }

        public string ElecBillToAdd { get; set; }

        public string FullName { get; set; }

        public string Remarks { get; set; }

    }
}
