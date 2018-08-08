using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for ISAPDetails.xaml
    /// </summary>
    public partial class ISAPDetails : MetroWindow
    {
        public ISAPDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<ISAPModel> lstISAP;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";

        public ISAPDetails(string sID)
        {
            InitializeComponent();
            employeeID = sID;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getISAPRecord();
            getPayrollISAPRecord();
            dgvISAP.ItemsSource = lstISAP;
        }

        private void getISAPRecord()
        {
            conDB = new ConnectionDB();
            lstISAP = new List<ISAPModel>();
            ISAPModel isapMod = new ISAPModel();

            queryString = "SELECT tblemployees.ID, concat(lastname,', ', firstname) as fullname, dateadded, " +
                "existingisap FROM (tblemployees INNER JOIN tblisap ON " +
                "tblemployees.ID = tblisap.empID) " +
                "WHERE tblemployees.isDeleted = 0 AND tblisap.isDeleted = 0 AND tblemployees.ID = ?";

            parameters = new List<string>();
            parameters.Add(employeeID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                isapMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["dateadded"].ToString());
                isapMod.DateAdded = dte.ToShortDateString();
                isapMod.CurrentISAP = reader["existingisap"].ToString();
                isapMod.Remarks = "MANUAL-ADMIN";
                lstISAP.Add(isapMod);
                isapMod = new ISAPModel();
            }

            conDB.closeConnection();
        }

        private void getPayrollISAPRecord()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT empID, isap, concat(lastname,', ', firstname) as fullname, tblpayroll.startdate" +
                " FROM (tblpayroll INNER JOIN tblemployees ON tblpayroll.empID = tblemployees.ID)" +
                " WHERE tblpayroll.isDeleted = 0 AND empID = ? AND isap > 0";

            parameters = new List<string>();
            parameters.Add(employeeID);

            ISAPModel isapMod = new ISAPModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                isapMod.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                isapMod.DateAdded = dte.ToShortDateString();
                isapMod.CurrentISAP = reader["isap"].ToString();
                isapMod.Remarks = "AUTO-PAYSLIP";
                lstISAP.Add(isapMod);
                isapMod = new ISAPModel();
            }

            conDB.closeConnection();
        }

    }
}
