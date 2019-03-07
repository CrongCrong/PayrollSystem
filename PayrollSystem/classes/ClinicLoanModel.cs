using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollSystem.classes
{
    public class ClinicLoanModel
    {

        public string ID { get; set; }

        public string EmpID { get; set; }

        public string FullName { get; set; }

        public string Loan { get; set; }

        public string LoanDate { get; set; }

        public string PendingBalance { get; set; }

        public bool isActive { get; set; }

        public string Remarks { get; set; }

        public string Interest { get; set; }

        public string TotalLoan { get; set; }

    }
}
