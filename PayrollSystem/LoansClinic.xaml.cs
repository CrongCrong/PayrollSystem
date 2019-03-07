using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for LoansClinic.xaml
    /// </summary>
    public partial class LoansClinic : MetroWindow
    {
        public LoansClinic()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters = new List<string>();
        List<ClinicLoanModel> lstClinicLoans = new List<ClinicLoanModel>();
        List<ClinicLoanModel> lstPLoans = new List<ClinicLoanModel>();


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lstPLoans = getClinicLoansPending();
            loadEmployeesList();
            dgvClinicLoan.ItemsSource = loadDatagridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            ClinicLoanModel enkModd = dgvClinicLoan.SelectedItem as ClinicLoanModel;
            if (enkModd != null)
            {
                ClinicLoansDetails emlDet = new ClinicLoansDetails(enkModd.EmpID);
                emlDet.ShowDialog();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                saveRecord();
                clearFields();
                dgvClinicLoan.ItemsSource = loadDatagridDetails();
                await this.ShowMessageAsync("RECORD SAVED", "Record successfully saved.");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            btnSave.Visibility = Visibility.Visible;
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private void txtloan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private List<ClinicLoanModel> loadDatagridDetails()
        {
            conDB = new ConnectionDB();

            ClinicLoanModel clinicLoan = new ClinicLoanModel();

            queryString = "SELECT tblloansclinic.ID, empID, concat(firstname, ' ', lastname) as fullname, " +
                "sum(loans) as loans, loandate FROM (tblloansclinic INNER JOIN tblemployees ON " +
                " tblloansclinic.empID = tblemployees.ID) " +
                "WHERE tblloansclinic.isDeleted = 0 AND tblemployees.isDeleted = 0 GROUP BY tblloansclinic.empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            double isAddedbyAdmin = 0;
            double current = 0;
            while (reader.Read())
            {
                clinicLoan.ID = reader["ID"].ToString();
                clinicLoan.EmpID = reader["empID"].ToString();
                clinicLoan.FullName = reader["fullname"].ToString();
                clinicLoan.Loan = reader["loans"].ToString();
                clinicLoan.TotalLoan = Convert.ToDouble(clinicLoan.Loan).ToString("0.##");
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                clinicLoan.LoanDate = dte.ToShortDateString();
                current = 0;

                foreach (ClinicLoanModel p in lstPLoans)
                {
                    if (p.EmpID.Equals(clinicLoan.EmpID))
                    {
                        double lo = Convert.ToDouble(clinicLoan.Loan);
                        double dblPending = Convert.ToDouble(p.Loan);
                        clinicLoan.PendingBalance = (lo - dblPending).ToString();
                        clinicLoan.Loan = (lo - dblPending).ToString();
                        clinicLoan.TotalLoan = ((lo - dblPending)).ToString();
                        current += Convert.ToDouble(clinicLoan.TotalLoan);
                    }

                }
                isAddedbyAdmin += current;

                lstClinicLoans.Add(clinicLoan);

                clinicLoan = new ClinicLoanModel();
            }
            lblTotalClinic.Content = "Total: " + isAddedbyAdmin.ToString("N0");
            conDB.closeConnection();
            return lstClinicLoans;
        }

        private void loadEmployeesList()
        {
            conDB = new ConnectionDB();
            List<EmployeeModel> lstEmployees = new List<EmployeeModel>();
            EmployeeModel employee = new EmployeeModel();

            cmbEmployees.Items.Clear();

            queryString = "SELECT tblemployees.ID, employeeID, firstname, lastname, status, incomeperday, companyID, description FROM " +
                "(tblemployees INNER JOIN tblcompany ON tblemployees.companyID = tblcompany.ID) " +
                "WHERE tblemployees.isDeleted = 0 ORDER BY lastname ASC";

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

        private List<ClinicLoanModel> getClinicLoansPending()
        {
            List<ClinicLoanModel> lstD = new List<ClinicLoanModel>();
            ClinicLoanModel peys = new ClinicLoanModel();

            conDB = new ConnectionDB();

            queryString = "SELECT empID, sum(clinicloan) as clinicpending FROM tblpayroll WHERE tblpayroll.isDeleted = 0 GROUP BY empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                peys.EmpID = reader["empID"].ToString();
                peys.Loan = (string.IsNullOrEmpty(reader["clinicpending"].ToString())) ? "0" : reader["clinicpending"].ToString();
                lstD.Add(peys);
                peys = new ClinicLoanModel();
            }
            
            conDB.closeConnection();
            return lstD;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO tblloansclinic (empID, loans, loandate, isDeleted) " +
                "VALUES (?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbEmployees.SelectedValue.ToString());
            parameters.Add(txtloan.Text);
            DateTime date = DateTime.Parse(dateLoanClinic.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("SAVE PEY RECORD: " + "EMPLOYEE ID: " + cmbEmployees.SelectedValue.ToString() +
                "| LOAN: " + txtloan.Text + " |DATE: " + dateLoanClinic.Text);
        }

        private async Task<bool> checkFields()
        {
            bool bl = false;

            if (string.IsNullOrEmpty(dateLoanClinic.Text))
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

        private bool ifValidFloatingNumber(string txtField)
        {
            bool ifOKay = false;

            ifOKay = Regex.IsMatch(txtField, "^[+-]?([0-9]*[.])?[0-9]+$");

            return ifOKay;
        }

        private void clearFields()
        {
            dateLoanClinic.Text = "";
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

    }
}
