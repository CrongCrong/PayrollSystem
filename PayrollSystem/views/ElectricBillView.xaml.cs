﻿using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PayrollSystem.views
{
    /// <summary>
    /// Interaction logic for ElectricBillView.xaml
    /// </summary>
    public partial class ElectricBillView : UserControl
    {
        public ElectricBillView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        MahApps.Metro.Controls.MetroWindow window;
        List<ElectricBillModel> lstElecBillToAdd;
        List<string> parameters;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadEmployeesList();
            lstElecBillToAdd = getElecBillToAdd();
            dgvElecBill.ItemsSource = getElecBillRecord();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<ElectricBillModel> getElecBillRecord()
        {
            List<ElectricBillModel> lstElecBill = new List<ElectricBillModel>();
            ElectricBillModel elecBill = new ElectricBillModel();

            conDB = new ConnectionDB();

            queryString = "SELECT tblelecbill.empid, sum(elecbill) as electricbill, concat(lastname,' , ', firstname) as fullname, " +
                "dateadded FROM (tblelecbill INNER JOIN tblemployees ON " +
                "tblelecbill.empID = tblemployees.ID) WHERE tblelecbill.isDeleted = 0 GROUP BY tblelecbill.empid";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            double isAddedByAdmin = 0;
            while (reader.Read())
            {
                elecBill.empID = reader["empID"].ToString();
                elecBill.FullName = reader["fullname"].ToString();
                double current = Convert.ToDouble(reader["electricbill"].ToString());

                foreach (ElectricBillModel ism in lstElecBillToAdd)
                {
                    if (elecBill.empID.Equals(ism.empID))
                    {
                        current = current - Convert.ToDouble(ism.ElecBillToAdd);
                    }
                }
                isAddedByAdmin += current;
                DateTime dte = DateTime.Parse(reader["dateadded"].ToString());
                elecBill.DateAdded = dte.ToShortDateString();
                elecBill.CurrentElecBill = current.ToString();
                lstElecBill.Add(elecBill);
                elecBill = new ElectricBillModel();
            }
            lblTotalIS.Content = "Total: " + isAddedByAdmin.ToString("N0");
            conDB.closeConnection();
            return lstElecBill;
        }

        private List<ElectricBillModel> getElecBillToAdd()
        {
            List<ElectricBillModel> lstElecBill = new List<ElectricBillModel>();
            ElectricBillModel eBill = new ElectricBillModel();

            conDB = new ConnectionDB();

            queryString = "SELECT empID, sum(electricbill) as electricbill FROM tblpayroll WHERE tblpayroll.isDeleted = 0 GROUP BY empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                eBill.empID = reader["empID"].ToString();
                eBill.ElecBillToAdd = reader["electricbill"].ToString();
                lstElecBill.Add(eBill);
                eBill = new ElectricBillModel();
            }
            conDB.closeConnection();
            return lstElecBill;
        }

        private void loadEmployeesList()
        {
            conDB = new ConnectionDB();
            List<EmployeeModel> lstEmployees = new List<EmployeeModel>();
            EmployeeModel employee = new EmployeeModel();

            cmbEmployees.Items.Clear();

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
                cmbEmployees.Items.Add(employee);
                employee = new EmployeeModel();
            }
            conDB.closeConnection();
        }

        private async Task<bool> checkFields()
        {
            bool bl = false;
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            if (string.IsNullOrEmpty(dateIS.Text))
            {
                await window.ShowMessageAsync("DATE", "Please select date.");
            }
            else if (string.IsNullOrEmpty(txtElecBill.Text) || !ifValidFloatingNumber(txtElecBill.Text))
            {
                await window.ShowMessageAsync("Electric Bill", "Please input valid Electric bill value.");
            }
            else if (cmbEmployees.SelectedItem == null)
            {
                await window.ShowMessageAsync("Employees", "Please select an employee.");
            }
            else
            {
                bl = true;
            }

            return bl;
        }

        private void clearFields()
        {
            dateIS.Text = "";
            cmbEmployees.SelectedItem = null;
            txtElecBill.Text = "";
        }

        private void saveSSSRecord()
        {
            conDB = new ConnectionDB();

            queryString = "INSERT INTO tblelecbill (empID, elecbill, dateadded, isDeleted) VALUES (?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbEmployees.SelectedValue.ToString());
            parameters.Add(txtElecBill.Text);
            DateTime date = DateTime.Parse(dateIS.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            ElectricBillModel eMod = dgvElecBill.SelectedItem as ElectricBillModel;

            if(eMod != null)
            {
                ElecBillDetails eDet = new ElecBillDetails(eMod.empID);
                eDet.ShowDialog();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                saveSSSRecord();
                dgvElecBill.ItemsSource = getElecBillRecord();
                await window.ShowMessageAsync("RECORD SAVED", "Record successfully saved.");
                clearFields();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtElecBill_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "." || e.Text == "-"))
            {
                e.Handled = true;
            }
        }

        private bool ifValidFloatingNumber(string txtField)
        {
            bool ifOKay = false;

            ifOKay = Regex.IsMatch(txtField, "^[+-]?([0-9]*[.])?[0-9]+$");

            return ifOKay;
        }
    }
}
