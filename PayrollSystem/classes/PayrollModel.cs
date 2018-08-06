using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollSystem.classes
{
    public class PayrollModel
    {
        public string ID { get; set; }

        public string EmployeeID { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string SSS { get; set; }

        public string PhilHealth { get; set; }

        public string PagIbig { get; set; }

        public string WithholdingTax { get; set; }

        public string SSSLoan { get; set; }

        public string ISAP { get; set; }

        public string IS { get; set; }

        public string PEY { get; set; }

        public string PEL { get; set; }

        public string GRL { get; set; }

        public string EML { get; set; }

        public string ElecBill { get; set; }

        public string CashAdvance { get; set; }

        public string Absent { get; set; }

        public string Lates { get; set; }

        public string Undertime { get; set; }

        public string Others { get; set; }

        public string Remarks { get; set; }

        public string RateHr { get; set; }

        public string FullName { get; set; }

        public string NetPay { get; set; }

        public string WorkDays { get; set; }

        public string Trips { get; set; }

        public string OTHours { get; set; }

        public string OTTotal { get; set; }

        public string Allowance { get; set; }

        public string Commission { get; set; }

        public string BasicPay { get; set; }

        public string Wage { get; set; }

        public string TotalISAP { get; set; }

        public string TotalIS { get; set; }

        public string TotalLoans { get; set; }

        public string ParticularOthers { get; set; }

        public string DeductionOthers { get; set; }

        public string GrossSalary { get; set; }

        public string TotalDeductions { get; set; }

        public string TotalPEL { get; set; }

        public string TotalEML { get; set; }

        public string TotalElectBill { get; set; }

        public string TotalGRL { get; set; }

        public string TotalPEY { get; set; }

        public string TotalSSSLoan { get; set; }
    }
}
