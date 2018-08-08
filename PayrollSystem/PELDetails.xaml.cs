using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for PELDetails.xaml
    /// </summary>
    public partial class PELDetails : MetroWindow
    {
        public PELDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<PELModel> lstPEL;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";

        public PELDetails(string eID)
        {
            employeeID = eID;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getPELRecord();
            getPayrollPELRecord();
            dgvPEL.ItemsSource = lstPEL;
        }

        private void getPELRecord()
        {
            conDB = new ConnectionDB();
            lstPEL = new List<PELModel>();
            PELModel pelMod = new PELModel();

            queryString = "SELECT dbfhproll.tblemployees.ID, concat(lastname,', ', firstname) as fullname, loandate, " +
                "loans, interest FROM (tblemployees INNER JOIN tblloanspel ON " +
                "tblemployees.ID = tblloanspel.empID) " +
                "WHERE tblemployees.isDeleted = 0 AND tblloanspel.isDeleted = 0 " +
                "AND tblemployees.ID = ?";

            parameters = new List<string>();
            parameters.Add(employeeID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                pelMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                pelMod.LoanDate = dte.ToShortDateString();
                pelMod.Loan = reader["loans"].ToString();
                pelMod.Interest = reader["interest"].ToString();
                pelMod.Remarks = "MANUAL-ADMIN";
                lstPEL.Add(pelMod);
                pelMod = new PELModel();
            }

            conDB.closeConnection();
        }

        private void getPayrollPELRecord()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT empID, pel, concat(lastname,', ', firstname) as fullname, tblpayroll.startdate" +
                " FROM (tblpayroll INNER JOIN tblemployees ON " +
                "tblpayroll.empID = tblemployees.ID)" +
                " WHERE tblpayroll.isDeleted = 0 AND empID = ? AND pel > 0";

            parameters = new List<string>();
            parameters.Add(employeeID);

            PELModel pelMod = new PELModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                pelMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                pelMod.LoanDate = dte.ToShortDateString();
                pelMod.Loan = reader["pel"].ToString();
                pelMod.Remarks = "AUTO-PAYSLIP";
                lstPEL.Add(pelMod);
                pelMod = new PELModel();
            }

            conDB.closeConnection();
        }
    }
}
