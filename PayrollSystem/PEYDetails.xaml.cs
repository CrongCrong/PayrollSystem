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
    /// Interaction logic for PEYDetails.xaml
    /// </summary>
    public partial class PEYDetails : MetroWindow
    {
        public PEYDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<PEYModel> lstPEY;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";

        public PEYDetails(string eID)
        {
            employeeID = eID;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getPELRecord();
            getPayrollPELRecord();
            dgvPEY.ItemsSource = lstPEY;
        }

        private void getPELRecord()
        {
            conDB = new ConnectionDB();
            lstPEY = new List<PEYModel>();
            PEYModel peyMod = new PEYModel();

            queryString = "SELECT dbfhpayroll.tblemployees.ID, concat(lastname,', ', firstname) as fullname, loandate, " +
                "loans FROM (dbfhpayroll.tblemployees INNER JOIN dbfhpayroll.tblloanspey ON " +
                "dbfhpayroll.tblemployees.ID = dbfhpayroll.tblloanspey.empID) " +
                "WHERE dbfhpayroll.tblemployees.isDeleted = 0 AND dbfhpayroll.tblloanspey.isDeleted = 0 " +
                "AND dbfhpayroll.tblemployees.ID = ?";

            parameters = new List<string>();
            parameters.Add(employeeID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                peyMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                peyMod.LoanDate = dte.ToShortDateString();
                peyMod.Loan = reader["loans"].ToString();
                peyMod.Remarks = "MANUAL-ADMIN";
                lstPEY.Add(peyMod);
                peyMod = new PEYModel();
            }

            conDB.closeConnection();
        }

        private void getPayrollPELRecord()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT empID, pey, concat(lastname,', ', firstname) as fullname, dbfhpayroll.tblpayroll.startdate" +
                " FROM (dbfhpayroll.tblpayroll INNER JOIN dbfhpayroll.tblemployees ON " +
                "dbfhpayroll.tblpayroll.empID = dbfhpayroll.tblemployees.ID)" +
                " WHERE dbfhpayroll.tblpayroll.isDeleted = 0 AND empID = ? AND pey > 0";

            parameters = new List<string>();
            parameters.Add(employeeID);

            PEYModel peyMod = new PEYModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                peyMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                peyMod.LoanDate = dte.ToShortDateString();
                peyMod.Loan = reader["pey"].ToString();
                peyMod.Remarks = "AUTO-PAYSLIP";
                lstPEY.Add(peyMod);
                peyMod = new PEYModel();
            }

            conDB.closeConnection();
        }
    }
}
