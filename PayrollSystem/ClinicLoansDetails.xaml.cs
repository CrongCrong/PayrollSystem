using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for ClinicLoansDetails.xaml
    /// </summary>
    public partial class ClinicLoansDetails : MetroWindow
    {
        public ClinicLoansDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<ClinicLoanModel> lstLoansClinic;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";

        public ClinicLoansDetails(string empID)
        {
            employeeID = empID;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getClinicLoansRecord();
            getPayrollClinicLoanRecord();
            dgvEML.ItemsSource = lstLoansClinic;
        }

        private void getClinicLoansRecord()
        {
            conDB = new ConnectionDB();
            lstLoansClinic = new List<ClinicLoanModel>();
            ClinicLoanModel clinicLoan = new ClinicLoanModel();

            queryString = "SELECT tblemployees.ID, concat(lastname,', ', firstname) as fullname, loandate, " +
                "loans FROM (tblemployees INNER JOIN tblloansclinic ON " +
                "tblemployees.ID = tblloansclinic.empID) " +
                "WHERE tblemployees.isDeleted = 0 AND tblloansclinic.isDeleted = 0 " +
                "AND tblemployees.ID = ?";

            parameters = new List<string>();
            parameters.Add(employeeID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                clinicLoan.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                clinicLoan.LoanDate = dte.ToShortDateString();
                clinicLoan.Loan = reader["loans"].ToString();
                clinicLoan.Remarks = "MANUAL-ADMIN";
                lstLoansClinic.Add(clinicLoan);
                clinicLoan = new ClinicLoanModel();
            }

            conDB.closeConnection();
        }

        private void getPayrollClinicLoanRecord()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT empID, clinicloan, concat(lastname,', ', firstname) as fullname, tblpayroll.startdate" +
                " FROM (tblpayroll INNER JOIN tblemployees ON tblpayroll.empID = tblemployees.ID)" +
                " WHERE tblpayroll.isDeleted = 0 AND empID = ? AND clinicloan > 0";

            parameters = new List<string>();
            parameters.Add(employeeID);

            ClinicLoanModel emlMod = new ClinicLoanModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                emlMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                emlMod.LoanDate = dte.ToShortDateString();
                emlMod.Loan = reader["clinicloan"].ToString();
                emlMod.Remarks = "AUTO-PAYSLIP";
                lstLoansClinic.Add(emlMod);
                emlMod = new ClinicLoanModel();
            }

            conDB.closeConnection();
        }
    }
}
