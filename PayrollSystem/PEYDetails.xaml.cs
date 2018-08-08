using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Windows;

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

            queryString = "SELECT tblemployees.ID, concat(lastname,', ', firstname) as fullname, loandate, " +
                "loans FROM (tblemployees INNER JOIN tblloanspey ON " +
                "tblemployees.ID = tblloanspey.empID) " +
                "WHERE tblemployees.isDeleted = 0 AND tblloanspey.isDeleted = 0 " +
                "AND tblemployees.ID = ?";

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
            queryString = "SELECT empID, pey, concat(lastname,', ', firstname) as fullname, tblpayroll.startdate" +
                " FROM (tblpayroll INNER JOIN tblemployees ON " +
                "tblpayroll.empID = tblemployees.ID)" +
                " WHERE tblpayroll.isDeleted = 0 AND empID = ? AND pey > 0";

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
