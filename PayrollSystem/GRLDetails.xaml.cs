using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for GRLDetails.xaml
    /// </summary>
    public partial class GRLDetails : MetroWindow
    {
        public GRLDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<GRLModel> lstGRL;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";

        public GRLDetails(string eID)
        {
            employeeID = eID;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getGRLRecord();
            getPayrollGRLRecord();
            dgvPEL.ItemsSource = lstGRL;
        }

        private void getGRLRecord()
        {
            conDB = new ConnectionDB();
            lstGRL = new List<GRLModel>();
            GRLModel grlMod = new GRLModel();

            queryString = "SELECT dbfhpayroll.tblemployees.ID, concat(lastname,', ', firstname) as fullname, loandate, " +
                "loans FROM (dbfhpayroll.tblemployees INNER JOIN dbfhpayroll.tblloansgrl ON " +
                "dbfhpayroll.tblemployees.ID = dbfhpayroll.tblloansgrl.empID) " +
                "WHERE dbfhpayroll.tblemployees.isDeleted = 0 AND dbfhpayroll.tblloansgrl.isDeleted = 0 " +
                "AND dbfhpayroll.tblemployees.ID = ?";

            parameters = new List<string>();
            parameters.Add(employeeID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                grlMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                grlMod.LoanDate = dte.ToShortDateString();
                grlMod.Loan = reader["loans"].ToString();
                grlMod.Remarks = "MANUAL-ADMIN";
                lstGRL.Add(grlMod);
                grlMod = new GRLModel();
            }

            conDB.closeConnection();
        }

        private void getPayrollGRLRecord()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT empID, grl, concat(lastname,', ', firstname) as fullname, dbfhpayroll.tblpayroll.startdate" +
                " FROM (dbfhpayroll.tblpayroll INNER JOIN dbfhpayroll.tblemployees ON dbfhpayroll.tblpayroll.empID = dbfhpayroll.tblemployees.ID)" +
                " WHERE dbfhpayroll.tblpayroll.isDeleted = 0 AND empID = ? AND grl > 0";

            parameters = new List<string>();
            parameters.Add(employeeID);

            GRLModel pelMod = new GRLModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                pelMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                pelMod.LoanDate = dte.ToShortDateString();
                pelMod.Loan = reader["grl"].ToString();
                pelMod.Remarks = "AUTO-PAYSLIP";
                lstGRL.Add(pelMod);
                pelMod = new GRLModel();
            }

            conDB.closeConnection();
        }
    }
}
