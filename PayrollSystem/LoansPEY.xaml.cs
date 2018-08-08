using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for LoansPEY.xaml
    /// </summary>
    public partial class LoansPEY : MetroWindow
    {
        public LoansPEY()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        List<PEYModel> lstPendingPEY;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadEmployeesList();
            lstPendingPEY = getPEYPending();
            dgvPEY.ItemsSource = loadDatagridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<PEYModel> loadDatagridDetails()
        {
            conDB = new ConnectionDB();
            List<PEYModel> lstPEY = new List<PEYModel>();
            PEYModel peyMod = new PEYModel();

            queryString = "SELECT tblloanspey.ID, empID, concat(firstname, ' ', lastname) as fullname, " +
                "sum(loans) as loans, loandate FROM (tblloanspey INNER JOIN tblemployees ON " +
                " tblloanspey.empID = tblemployees.ID) " +
                "WHERE tblloanspey.isDeleted = 0 GROUP BY tblloanspey.empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                peyMod.ID = reader["ID"].ToString();
                peyMod.EmpID = reader["empID"].ToString();
                peyMod.FullName = reader["fullname"].ToString();
                peyMod.Loan = reader["loans"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                peyMod.LoanDate = dte.ToShortDateString();

                foreach (PEYModel p in lstPendingPEY)
                {
                    if (p.EmpID.Equals(peyMod.EmpID))
                    {
                        double lo = Convert.ToDouble(peyMod.Loan);
                        double dblPending = Convert.ToDouble(p.Loan);
                        peyMod.PendingBalance = (lo - dblPending).ToString();
                        peyMod.Loan = (lo - dblPending).ToString();
                    }

                }

                lstPEY.Add(peyMod);
                peyMod = new PEYModel();
            }

            conDB.closeConnection();
            return lstPEY;
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

        private List<PEYModel> getPEYPending()
        {
            List<PEYModel> lstPeyPending = new List<PEYModel>();
            PEYModel peys = new PEYModel();

            conDB = new ConnectionDB();

            queryString = "SELECT empID, sum(pey) as peypending FROM tblpayroll WHERE tblpayroll.isDeleted = 0 GROUP BY empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                peys.EmpID = reader["empID"].ToString();
                peys.Loan = reader["peypending"].ToString();
                lstPeyPending.Add(peys);
                peys = new PEYModel();
            }

            conDB.closeConnection();
            return lstPeyPending;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO tblloanspey (empID, loans, loandate, isDeleted) " +
                "VALUES (?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbEmployees.SelectedValue.ToString());
            parameters.Add(txtloan.Text);
            DateTime date = DateTime.Parse(datePEL.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("SAVE PEY RECORD: " + "EMPLOYEE ID: " + cmbEmployees.SelectedValue.ToString() + 
                "| LOAN: " + txtloan.Text + " |DATE: " + datePEL.Text);
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            PEYModel enkModd = dgvPEY.SelectedItem as PEYModel;

            if (enkModd != null)
            {
                PEYDetails emlDet = new PEYDetails(enkModd.EmpID);
                emlDet.ShowDialog();
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
                saveRecord();
                clearFields();
                dgvPEY.ItemsSource = loadDatagridDetails();
                await this.ShowMessageAsync("RECORD SAVED", "Record successfully saved.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            btnSave.Visibility = Visibility.Visible;
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private async Task<bool> checkFields()
        {
            bool bl = false;

            if (string.IsNullOrEmpty(datePEL.Text))
            {
                await this.ShowMessageAsync("DATE", "Please select date.");
            }
            else if (string.IsNullOrEmpty(txtloan.Text) || !ifValidFloatingNumber(txtloan.Text))
            {
                await this.ShowMessageAsync("LOAN", "Please input valid Loan value.");
            }
            else if (cmbEmployees.SelectedItem == null)
            {
                await this.ShowMessageAsync("Employees", "Please select an employee.");
            }
            else
            {
                bl = true;
            }

            return bl;
        }

        private void clearFields()
        {
            datePEL.Text = "";
            cmbEmployees.SelectedItem = null;
            txtloan.Text = "";
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

        private void txtloan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
