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
    /// Interaction logic for LoansGRL.xaml
    /// </summary>
    public partial class LoansGRL : MetroWindow
    {
        public LoansGRL()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        List<GRLModel> lstPendingGRL;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadEmployeesList();
            lstPendingGRL = getGRLPending();
            dgvGRL.ItemsSource = loadDatagridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<GRLModel> loadDatagridDetails()
        {
            conDB = new ConnectionDB();
            List<GRLModel> lstGRL = new List<GRLModel>();
            GRLModel grlMode = new GRLModel();

            queryString = "SELECT tblloansgrl.ID, empID, concat(firstname, ' ', lastname) as fullname, " +
                "sum(loans) as loans, loandate FROM (tblloansgrl INNER JOIN tblemployees ON " +
                " tblloansgrl.empID = tblemployees.ID) " +
                "WHERE tblloansgrl.isDeleted = 0 GROUP BY tblloansgrl.empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            double isAddedbyAdmin = 0;
            double current = 0;
            while (reader.Read())
            {
                grlMode.ID = reader["ID"].ToString();
                grlMode.EmpID = reader["empID"].ToString();
                grlMode.FullName = reader["fullname"].ToString();
                grlMode.Loan = reader["loans"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                grlMode.LoanDate = dte.ToShortDateString();

                current = 0;
                foreach (GRLModel p in lstPendingGRL)
                {
                    if (p.EmpID.Equals(grlMode.EmpID))
                    {
                        double lo = Convert.ToDouble(grlMode.Loan);
                        double dblPending = Convert.ToDouble(p.Loan);
                        grlMode.PendingBalance = (lo - dblPending).ToString();
                        grlMode.Loan = (lo - dblPending).ToString();
                        current += Convert.ToDouble(grlMode.Loan);
                    }
                }
                isAddedbyAdmin += current;
                lstGRL.Add(grlMode);
                grlMode = new GRLModel();
            }
            lblTotalIS.Content = "Total: " + isAddedbyAdmin.ToString("N0");
            conDB.closeConnection();
            return lstGRL;
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

        private List<GRLModel> getGRLPending()
        {
            List<GRLModel> lstGrlPending = new List<GRLModel>();
            GRLModel grls = new GRLModel();

            conDB = new ConnectionDB();

            queryString = "SELECT empID, sum(grl) as grlpending FROM tblpayroll WHERE tblpayroll.isDeleted = 0 GROUP BY empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                grls.EmpID = reader["empID"].ToString();
                grls.Loan = reader["grlpending"].ToString();
                lstGrlPending.Add(grls);
                grls = new GRLModel();
            }

            conDB.closeConnection();
            return lstGrlPending;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO tblloansgrl (empID, loans, loandate, isDeleted) " +
                "VALUES (?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbEmployees.SelectedValue.ToString());
            parameters.Add(txtloan.Text);
            DateTime date = DateTime.Parse(datePEL.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("SAVE GRL RECORD: " + " EMPLOYEE ID: " + cmbEmployees.SelectedValue.ToString() + " |LOAN: " +
                txtloan.Text + " |DATE: " + datePEL.Text);
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            GRLModel pelModd = dgvGRL.SelectedItem as GRLModel;

            if (pelModd != null)
            {
                GRLDetails grlDet = new GRLDetails(pelModd.EmpID);
                grlDet.ShowDialog();
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
                dgvGRL.ItemsSource = loadDatagridDetails();
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

        private void txtloan_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

    }
}
