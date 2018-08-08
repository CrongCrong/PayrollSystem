using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for EMLDetails.xaml
    /// </summary>
    public partial class EMLDetails : MetroWindow
    {
        public EMLDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<EMLModel> lstEML;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";

        public EMLDetails(string eID)
        {
            employeeID = eID;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getEMLRecord();
            getPayrollPELRecord();
            dgvEML.ItemsSource = lstEML;
        }

        private void getEMLRecord()
        {
            conDB = new ConnectionDB();
            lstEML = new List<EMLModel>();
            EMLModel emlMod = new EMLModel();

            queryString = "SELECT tblemployees.ID, concat(lastname,', ', firstname) as fullname, loandate, " +
                "loans FROM (tblemployees INNER JOIN tblloanseml ON " +
                "tblemployees.ID = tblloanseml.empID) " +
                "WHERE tblemployees.isDeleted = 0 AND tblloanseml.isDeleted = 0 " +
                "AND tblemployees.ID = ?";

            parameters = new List<string>();
            parameters.Add(employeeID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                emlMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                emlMod.LoanDate = dte.ToShortDateString();
                emlMod.Loan = reader["loans"].ToString();
                emlMod.Remarks = "MANUAL-ADMIN";
                lstEML.Add(emlMod);
                emlMod = new EMLModel();
            }

            conDB.closeConnection();
        }

        private void getPayrollPELRecord()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT empID, eml, concat(lastname,', ', firstname) as fullname, tblpayroll.startdate" +
                " FROM (tblpayroll INNER JOIN tblemployees ON tblpayroll.empID = tblemployees.ID)" +
                " WHERE tblpayroll.isDeleted = 0 AND empID = ? AND eml > 0";

            parameters = new List<string>();
            parameters.Add(employeeID);

            EMLModel emlMod = new EMLModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                emlMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                emlMod.LoanDate = dte.ToShortDateString();
                emlMod.Loan = reader["eml"].ToString();
                emlMod.Remarks = "AUTO-PAYSLIP";
                lstEML.Add(emlMod);
                emlMod = new EMLModel();
            }

            conDB.closeConnection();
        }
    }
}
