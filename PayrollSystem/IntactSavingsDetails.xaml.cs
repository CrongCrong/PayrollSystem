using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for IntactSavingsDetails.xaml
    /// </summary>
    public partial class IntactSavingsDetails : MetroWindow
    {
        public IntactSavingsDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<IntactSavingsModel> lstIS;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";

        public IntactSavingsDetails(string sID)
        {
            employeeID = sID;
            InitializeComponent();

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getIntactSavingsRecord();
            getPayrollISRecord();
            dgvIS.ItemsSource = lstIS;
        }

        private void getIntactSavingsRecord()
        {
            conDB = new ConnectionDB();
            lstIS = new List<IntactSavingsModel>();
            IntactSavingsModel isMod = new IntactSavingsModel();

            queryString = "SELECT tblemployees.ID, concat(lastname,', ', firstname) as fullname, dateadded, " +
                "existingIS FROM (tblemployees INNER JOIN tblintactsavings ON " +
                "tblemployees.ID = tblintactsavings.empID) " +
                "WHERE tblemployees.isDeleted = 0 AND tblintactsavings.isDeleted = 0 AND tblemployees.ID = ?";

            parameters = new List<string>();
            parameters.Add(employeeID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                isMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["dateadded"].ToString());
                isMod.DateAdded = dte.ToShortDateString();
                isMod.CurrentIS = reader["existingIS"].ToString();
                isMod.Remarks = "MANUAL-ADMIN";
                lstIS.Add(isMod);
                isMod = new IntactSavingsModel();
            }

            conDB.closeConnection();
        }

        private void getPayrollISRecord()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT empID, isavings, concat(lastname,', ', firstname) as fullname, tblpayroll.startdate" +
                " FROM (tblpayroll INNER JOIN tblemployees ON tblpayroll.empID = tblemployees.ID)" +
                " WHERE tblpayroll.isDeleted = 0 AND empID = ? AND isavings > 0";

            parameters = new List<string>();
            parameters.Add(employeeID);

            IntactSavingsModel isMod = new IntactSavingsModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                isMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                isMod.DateAdded = dte.ToShortDateString();
                isMod.CurrentIS = reader["isavings"].ToString();
                isMod.Remarks = "AUTO-PAYSLIP";
                lstIS.Add(isMod);
                isMod = new IntactSavingsModel();
            }

            conDB.closeConnection();
        }

    }
}
