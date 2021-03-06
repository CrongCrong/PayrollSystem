﻿using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PayrollSystem.views
{
    /// <summary>
    /// Interaction logic for PayrollView.xaml
    /// </summary>
    /// 
    public partial class PayrollView : UserControl
    {
        public PayrollView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        string employeeID = "";
        MahApps.Metro.Controls.MetroWindow window;

        //MahApps.Metro.Controls.MetroWindow window;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvEmployees.ItemsSource = loadDataGridDetails();
            loadCompanyList();
            loadEmployeesList();
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            cmbSearchCompany.IsEnabled = false;
            cmbSearchEmployee.IsEnabled = false;
        }

        private List<PayrollModel> searchEmployees()
        {
            conDB = new ConnectionDB();
            List<PayrollModel> lstSearching = new List<PayrollModel>();
            PayrollModel payroll = new PayrollModel();

            queryString = "SELECT tblpayroll.ID, empID, tblpayroll.startdate, enddate, sss, philhealth, pagibig, " +
                "sssloan, isap, isavings, pey, pel, grl, eml, electricbill, cashadvance, " +
                "absent, lates, undertime, others, remarks, firstname, lastname, netpay, workdays, trips, othours, ottotal, " +
                "allowance, commission, particularothers, deductionothers, totalpel, totaleml, totalgrl, totalpey, " +
                "totalelectbill, totalsssloan, totalis, totalisap, clinicloan, totalclinicloan FROM (tblpayroll INNER JOIN " +
                "tblemployees ON tblpayroll.empID = tblemployees.ID) WHERE tblpayroll.isDeleted = 0 " +
                "AND tblemployees.isDeleted = 0 ";

            parameters = new List<string>();
           
            if (chkByDate.IsChecked.Value && (!string.IsNullOrEmpty(searchDateFrom.Text) && 
                !string.IsNullOrEmpty(searchDateTo.Text)))
            {
                queryString += " AND (tblpayroll.startdate BETWEEN ? AND ? AND enddate BETWEEN ? AND ?)";
                DateTime sdate = DateTime.Parse(searchDateFrom.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
                sdate = DateTime.Parse(searchDateTo.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);

                sdate = DateTime.Parse(searchDateFrom.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
                sdate = DateTime.Parse(searchDateTo.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            }
            if (chkByCompany.IsChecked.Value && cmbSearchCompany.SelectedItem != null)
            {
                queryString += " AND (tblemployees.companyID = ?)";
                parameters.Add(cmbSearchCompany.SelectedValue.ToString());
            }
            if (chkByEmployee.IsChecked.Value && cmbSearchEmployee.SelectedItem != null)
            {
                queryString += " AND (empID =?)";
                parameters.Add(cmbSearchEmployee.SelectedValue.ToString());
            }

            queryString += " ORDER BY tblpayroll.startdate DESC";
         
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                payroll.ID = reader["ID"].ToString();
                payroll.EmployeeID = reader["empID"].ToString();

                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                payroll.StartDate = dte.ToShortDateString();

                dte = DateTime.Parse(reader["enddate"].ToString());
                payroll.EndDate = dte.ToShortDateString();

                payroll.SSS = reader["sss"].ToString();
                payroll.PhilHealth = reader["philhealth"].ToString();
                payroll.PagIbig = reader["pagibig"].ToString();
                payroll.SSSLoan = reader["sssloan"].ToString();
                payroll.ISAP = reader["isap"].ToString();
                payroll.IS = reader["isavings"].ToString();
                payroll.PEY = reader["pey"].ToString();
                payroll.PEL = reader["pel"].ToString();
                payroll.GRL = reader["grl"].ToString();
                payroll.EML = reader["eml"].ToString();
                payroll.ElecBill = reader["electricbill"].ToString();
                payroll.CashAdvance = reader["cashadvance"].ToString();
                payroll.Absent = reader["absent"].ToString();
                payroll.Lates = reader["lates"].ToString();
                payroll.Undertime = reader["undertime"].ToString();
                payroll.Others = reader["others"].ToString();
                payroll.Remarks = reader["remarks"].ToString();
                payroll.NetPay = reader["netpay"].ToString();
                string firstName = reader["firstname"].ToString();
                string lastName = reader["lastname"].ToString();
                payroll.FullName = lastName + ", " + firstName;
                payroll.WorkDays = reader["workdays"].ToString();
                payroll.Trips = reader["trips"].ToString();
                payroll.OTHours = reader["othours"].ToString();
                payroll.OTTotal = reader["ottotal"].ToString();
                payroll.Allowance = reader["allowance"].ToString();
                payroll.Commission = reader["commission"].ToString();
                payroll.ParticularOthers = reader["particularothers"].ToString();
                payroll.DeductionOthers = reader["deductionothers"].ToString();
                payroll.TotalPEL = reader["totalpel"].ToString();
                payroll.TotalEML = reader["totaleml"].ToString();
                payroll.TotalGRL = reader["totalgrl"].ToString();
                payroll.TotalPEY = reader["totalpey"].ToString();
                payroll.TotalElectBill = reader["totalelectbill"].ToString();
                payroll.TotalSSSLoan = reader["totalsssloan"].ToString();
                payroll.TotalIS = reader["totalis"].ToString();
                payroll.TotalISAP = reader["totalisap"].ToString();
                payroll.ClinicLoan = reader["clinicloan"].ToString();
                payroll.TotalClinicLoan = reader["totalclinicloan"].ToString();

                lstSearching.Add(payroll);
                payroll = new PayrollModel();
            }
            conDB.closeConnection();
            return lstSearching;
        }

        private void loadEmployeesList()
        {
            conDB = new ConnectionDB();
            List<EmployeeModel> lstEmployees = new List<EmployeeModel>();
            EmployeeModel employee = new EmployeeModel();
            cmbSearchEmployee.Items.Clear();

            queryString = "SELECT tblemployees.ID, employeeID, firstname, lastname, status, incomeperday, companyID, description FROM " +
                "(tblemployees INNER JOIN tblcompany ON tblemployees.companyID = tblcompany.ID) " +
                "WHERE tblemployees.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                employee.ID = reader["ID"].ToString();
                employee.EmployeeID = reader["employeeID"].ToString();
                employee.FirstName = reader["firstname"].ToString();
                employee.LastName = reader["lastname"].ToString();
                employee.Status = reader["status"].ToString();
                employee.Wage = reader["incomeperday"].ToString();
                employee.Company = reader["description"].ToString();
                employee.CompanyID = reader["companyID"].ToString();
                employee.FullName = employee.LastName + ", " + employee.FirstName;
                cmbSearchEmployee.Items.Add(employee);
                employee = new EmployeeModel();
            }
            conDB.closeConnection();

        }

        private void loadCompanyList()
        {
            conDB = new ConnectionDB();
            cmbSearchCompany.Items.Clear();
            queryString = "SELECT ID, companyname, description FROM tblcompany WHERE isDeleted = 0";
            CompanyModel company = new CompanyModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                company.ID = reader["ID"].ToString();
                company.CompanyName = reader["companyname"].ToString();
                company.Description = reader["description"].ToString();
                cmbSearchCompany.Items.Add(company);
                company = new CompanyModel();
            }
            conDB.closeConnection();
        }

        private void deleteRecord(string rID)
        {
            conDB = new ConnectionDB();

            queryString = "UPDATE tblpayroll SET isDeleted = 1 WHERE ID = ?";

            parameters = new List<string>();

            parameters.Add(rID);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
           
        }

        private List<PayrollModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<PayrollModel> lstPayroll = new List<PayrollModel>();
            PayrollModel payroll = new PayrollModel();

            queryString = "SELECT tblpayroll.ID, empID, tblpayroll.startdate, enddate, sss, philhealth, pagibig, " +
                "sssloan, isap, isavings, pey, pel, grl, eml, electricbill, cashadvance, " +
                "absent, lates, undertime, others, remarks, firstname, lastname, netpay, workdays, trips, othours, ottotal, " +
                "allowance, commission, particularothers, deductionothers, totalpel, totaleml, totalgrl, totalpey, " +
                "totalelectbill, totalsssloan, totalis, totalisap, clinicloan, totalclinicloan FROM (tblpayroll INNER JOIN " +
                "tblemployees ON tblpayroll.empID = tblemployees.ID) WHERE tblpayroll.isDeleted = 0 " +
                "AND tblemployees.isDeleted = 0 ORDER BY startdate DESC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                payroll.ID = reader["ID"].ToString();
                payroll.EmployeeID = reader["empID"].ToString();

                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                payroll.StartDate = dte.ToShortDateString();

                dte = DateTime.Parse(reader["enddate"].ToString());
                payroll.EndDate = dte.ToShortDateString();

                payroll.SSS = reader["sss"].ToString();
                payroll.PhilHealth = reader["philhealth"].ToString();
                payroll.PagIbig = reader["pagibig"].ToString();
                payroll.SSSLoan = reader["sssloan"].ToString();
                payroll.ISAP = reader["isap"].ToString();
                payroll.IS = reader["isavings"].ToString();
                payroll.PEY = reader["pey"].ToString();
                payroll.PEL = reader["pel"].ToString();
                payroll.GRL = reader["grl"].ToString();
                payroll.EML = reader["eml"].ToString();
                payroll.ElecBill = reader["electricbill"].ToString();
                payroll.CashAdvance = reader["cashadvance"].ToString();
                payroll.Absent = reader["absent"].ToString();
                payroll.Lates = reader["lates"].ToString();
                payroll.Undertime = reader["undertime"].ToString();
                payroll.Others = reader["others"].ToString();
                payroll.Remarks = reader["remarks"].ToString();
                payroll.NetPay = reader["netpay"].ToString();
                string firstName = reader["firstname"].ToString();
                string lastName = reader["lastname"].ToString();
                payroll.FullName = lastName + ", " + firstName;
                payroll.WorkDays = reader["workdays"].ToString();
                payroll.Trips = reader["trips"].ToString();
                payroll.OTHours = reader["othours"].ToString();
                payroll.OTTotal = reader["ottotal"].ToString();
                payroll.Allowance = reader["allowance"].ToString();
                payroll.Commission = reader["commission"].ToString();
                payroll.ParticularOthers = reader["particularothers"].ToString();
                payroll.DeductionOthers = reader["deductionothers"].ToString();
                payroll.TotalPEL = reader["totalpel"].ToString();
                payroll.TotalEML = reader["totaleml"].ToString();
                payroll.TotalGRL = reader["totalgrl"].ToString();
                payroll.TotalPEY = reader["totalpey"].ToString();
                payroll.TotalElectBill = reader["totalelectbill"].ToString();
                payroll.TotalSSSLoan = reader["totalsssloan"].ToString();
                payroll.TotalIS = reader["totalis"].ToString();
                payroll.TotalISAP = reader["totalisap"].ToString();
                payroll.TotalClinicLoan = reader["totalclinicloan"].ToString();
                payroll.ClinicLoan = reader["clinicloan"].ToString();
                lstPayroll.Add(payroll);
                payroll = new PayrollModel();
            }
            conDB.closeConnection();
            return lstPayroll;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgvEmployees.ItemsSource = searchEmployees();
        }

        private void btnAddPayroll_Click(object sender, RoutedEventArgs e)
        {
            PayrollDetailsWindow payrollDetails = new PayrollDetailsWindow(this);
            payrollDetails.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            PayrollModel pm = dgvEmployees.SelectedItem as PayrollModel;

            if(pm != null)
            {
                PayrollDetailsWindow pdw = new PayrollDetailsWindow(this, pm);
                pdw.ShowDialog();
            }
        }

        private void chkByEmployee_Checked(object sender, RoutedEventArgs e)
        {
            cmbSearchEmployee.IsEnabled = true;
        }

        private void chkByCompany_Checked(object sender, RoutedEventArgs e)
        {
            cmbSearchCompany.IsEnabled = true;
        }

        private void chkByDate_Checked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = true;
            searchDateTo.IsEnabled = true;
        }

        private void chkByEmployee_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbSearchEmployee.IsEnabled = false;
            cmbSearchEmployee.SelectedItem = null;
        }

        private void chkByCompany_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbSearchCompany.IsEnabled = false;
            cmbSearchCompany.SelectedItem = null;
        }

        private void chkByDate_Unchecked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            searchDateFrom.Text = "";
            searchDateTo.Text = "";

        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            PayrollModel pm = dgvEmployees.SelectedItem as PayrollModel;

            if (pm != null)
            {
                MessageDialogResult result = await window.ShowMessageAsync("Delete Payroll", "Are you sure you want to delete payroll?", MessageDialogStyle.AffirmativeAndNegative);

                if (result.Equals(MessageDialogResult.Affirmative))
                {
                    deleteRecord(pm.ID);
                    dgvEmployees.ItemsSource = loadDataGridDetails();
                    await window.ShowMessageAsync("Delete Record", "Record deleted successfully!");
                    conDB.writeLogFile("DELETE EMPLOYEE PAYROLL:  ID: " + pm.ID);
                }
            }
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            PayrollReport pr = new PayrollReport();
            pr.ShowDialog();
        }
    }
}
