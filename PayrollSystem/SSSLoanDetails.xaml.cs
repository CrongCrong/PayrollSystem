using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for SSSLoanDetails.xaml
    /// </summary>
    public partial class SSSLoanDetails : MetroWindow
    {
        public SSSLoanDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<SSSLoanModel> lstSSSLoan;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";

        public SSSLoanDetails(string eID)
        {
            employeeID = eID;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getSSSLoanRecord();
            getPayrollSSSLoanRecord();
            dgvSSSLoan.ItemsSource = lstSSSLoan;
        }

        private void getSSSLoanRecord()
        {
            conDB = new ConnectionDB();
            lstSSSLoan = new List<SSSLoanModel>();
            SSSLoanModel sssLoan = new SSSLoanModel();

            queryString = "SELECT tblemployees.ID, concat(lastname,', ', firstname) as fullname, dateadded, " +
                "sssloan FROM (tblemployees INNER JOIN tblsssloan ON " +
                "tblemployees.ID = tblsssloan.empID) " +
                "WHERE tblemployees.isDeleted = 0 AND tblsssloan.isDeleted = 0 " +
                "AND tblemployees.ID = ?";

            parameters = new List<string>();
            parameters.Add(employeeID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                sssLoan.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["dateadded"].ToString());
                sssLoan.DateAdded = dte.ToShortDateString();
                sssLoan.CurrentSSSLoan = reader["sssloan"].ToString();
                sssLoan.Remarks = "MANUAL-ADMIN";
                lstSSSLoan.Add(sssLoan);
                sssLoan = new SSSLoanModel();
            }

            conDB.closeConnection();
        }

        private void getPayrollSSSLoanRecord()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT empID, sssloan, concat(lastname,', ', firstname) as fullname, tblpayroll.startdate" +
                " FROM (tblpayroll INNER JOIN tblemployees ON " +
                "tblpayroll.empID = tblemployees.ID)" +
                " WHERE tblpayroll.isDeleted = 0 AND empID = ? AND sssloan > 0";

            parameters = new List<string>();
            parameters.Add(employeeID);

            SSSLoanModel sssLoan = new SSSLoanModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                sssLoan.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                sssLoan.DateAdded = dte.ToShortDateString();
                sssLoan.CurrentSSSLoan = reader["sssloan"].ToString();
                sssLoan.Remarks = "AUTO-PAYSLIP";
                lstSSSLoan.Add(sssLoan);
                sssLoan = new SSSLoanModel();
            }

            conDB.closeConnection();
        }
    }
}
