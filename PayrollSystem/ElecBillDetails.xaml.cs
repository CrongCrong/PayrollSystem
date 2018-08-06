﻿using MahApps.Metro.Controls;
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
    /// Interaction logic for ElecBillDetails.xaml
    /// </summary>
    public partial class ElecBillDetails : MetroWindow
    {
        public ElecBillDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<ElectricBillModel> lstElecBill;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";

        public ElecBillDetails(string eID)
        {
            employeeID = eID;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getElecBillRecord();
            getPayrollElecBillRecord();
            dgvElecBill.ItemsSource = lstElecBill;
        }

        private void getElecBillRecord()
        {
            conDB = new ConnectionDB();
            lstElecBill = new List<ElectricBillModel>();
            ElectricBillModel elecBill = new ElectricBillModel();

            queryString = "SELECT dbfhpayroll.tblemployees.ID, concat(lastname,', ', firstname) as fullname, dateadded, " +
                "elecbill FROM (dbfhpayroll.tblemployees INNER JOIN dbfhpayroll.tblelecbill ON " +
                "dbfhpayroll.tblemployees.ID = dbfhpayroll.tblelecbill.empID) " +
                "WHERE dbfhpayroll.tblemployees.isDeleted = 0 AND dbfhpayroll.tblelecbill.isDeleted = 0 " +
                "AND dbfhpayroll.tblemployees.ID = ?";

            parameters = new List<string>();
            parameters.Add(employeeID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                elecBill.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["dateadded"].ToString());
                elecBill.DateAdded = dte.ToShortDateString();
                elecBill.CurrentElecBill = reader["elecbill"].ToString();
                elecBill.Remarks = "MANUAL-ADMIN";
                lstElecBill.Add(elecBill);
                elecBill = new ElectricBillModel();
            }

            conDB.closeConnection();
        }

        private void getPayrollElecBillRecord()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT empID, electricbill, concat(lastname,', ', firstname) as fullname, dbfhpayroll.tblpayroll.startdate" +
                " FROM (dbfhpayroll.tblpayroll INNER JOIN dbfhpayroll.tblemployees ON " +
                "dbfhpayroll.tblpayroll.empID = dbfhpayroll.tblemployees.ID)" +
                " WHERE dbfhpayroll.tblpayroll.isDeleted = 0 AND empID = ? AND electricbill > 0";

            parameters = new List<string>();
            parameters.Add(employeeID);

            ElectricBillModel elecBill = new ElectricBillModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                elecBill.FullName = reader["fullname"].ToString();
                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                elecBill.DateAdded = dte.ToShortDateString();
                elecBill.CurrentElecBill = reader["electricbill"].ToString();
                elecBill.Remarks = "AUTO-PAYSLIP";
                lstElecBill.Add(elecBill);
                elecBill = new ElectricBillModel();
            }

            conDB.closeConnection();
        }
    }
}


