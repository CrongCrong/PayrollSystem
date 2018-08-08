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
    /// Interaction logic for LoansEML.xaml
    /// </summary>
    public partial class LoansEML : MetroWindow
    {
        public LoansEML()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        List<EMLModel> lstPendingEML;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadEmployeesList();
            lstPendingEML = getEMLPending();
            dgvEML.ItemsSource = loadDatagridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<EMLModel> loadDatagridDetails()
        {
            conDB = new ConnectionDB();
            List<EMLModel> lstEML = new List<EMLModel>();
            EMLModel emlMode = new EMLModel();

            queryString = "SELECT tblloanseml.ID, empID, concat(firstname, ' ', lastname) as fullname, " +
                "sum(loans) as loans, loandate, sum(interest) as interest FROM (tblloanseml INNER JOIN tblemployees ON " +
                " tblloanseml.empID = tblemployees.ID) " +
                "WHERE tblloanseml.isDeleted = 0 GROUP BY tblloanseml.empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                emlMode.ID = reader["ID"].ToString();
                emlMode.EmpID = reader["empID"].ToString();
                emlMode.FullName = reader["fullname"].ToString();
                emlMode.Loan = (string.IsNullOrEmpty(reader["loans"].ToString())) ?
                    "0" : reader["loans"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                emlMode.Interest = (!string.IsNullOrEmpty(reader["interest"].ToString())) ?
                    reader["interest"].ToString() : "0";
                emlMode.TotalLoan = (Convert.ToDouble(emlMode.Loan) +
                    Convert.ToDouble(emlMode.Interest)).ToString();
                emlMode.LoanDate = dte.ToShortDateString();
                foreach (EMLModel p in lstPendingEML)
                {
                    if (p.EmpID.Equals(emlMode.EmpID))
                    {
                        double lo = Convert.ToDouble(emlMode.Loan);
                        double dblPending = Convert.ToDouble(p.Loan);
                        double dblInterests = Convert.ToDouble(emlMode.Interest);
                        emlMode.PendingBalance = (lo - dblPending).ToString();
                        emlMode.Loan = (lo - dblPending).ToString();
                        emlMode.TotalLoan = ((lo - dblPending) + dblInterests).ToString();
                    }
                    
                }

                lstEML.Add(emlMode);
                emlMode = new EMLModel();
            }

            conDB.closeConnection();
            return lstEML;
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

        private List<EMLModel> getEMLPending()
        {
            List<EMLModel> lstEmlPending = new List<EMLModel>();
            EMLModel emls = new EMLModel();

            conDB = new ConnectionDB();

            queryString = "SELECT empID, sum(eml) as emlpending FROM tblpayroll WHERE tblpayroll.isDeleted = 0 GROUP BY empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                emls.EmpID = reader["empID"].ToString();
                emls.Loan = reader["emlpending"].ToString();
                lstEmlPending.Add(emls);
                emls = new EMLModel();
            }

            conDB.closeConnection();
            return lstEmlPending;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO tblloanseml (empID, loans, loandate, interest, isDeleted) " +
                "VALUES (?,?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbEmployees.SelectedValue.ToString());
            parameters.Add(txtloan.Text);
            DateTime date = DateTime.Parse(datePEL.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add(txtInterest.Text);
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("SAVE EML RECORD: " + " EMPLOYEE ID: " + cmbEmployees.SelectedValue.ToString() +
                " |LOAN :" + txtloan.Text + " |DATE: " + datePEL.Text);
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            EMLModel enkModd = dgvEML.SelectedItem as EMLModel;

            if (enkModd != null)
            {
                EMLDetails emlDet = new EMLDetails(enkModd.EmpID);
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
                dgvEML.ItemsSource = loadDatagridDetails();
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
            else if (string.IsNullOrEmpty(txtInterest.Text) || !ifValidFloatingNumber(txtInterest.Text))
            {
                await this.ShowMessageAsync("Interest", "Please input valid Interest value.");
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

        private void txtloan_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private double getInterestEML(string pLoan)
        {
            double dblloan = 0;
            double dblgetInterest = Convert.ToDouble(pLoan);

            if (dblgetInterest == 2000)
            {
                dblloan = Math.Round(dblgetInterest * (3 / 100d));
            }
            else if (dblgetInterest == 3000)
            {
                dblloan = Math.Round(dblgetInterest * (4 / 100d));
            }
            else if (dblgetInterest == 4000)
            {
                dblloan = Math.Round(dblgetInterest * (5 / 100d));
            }
            else if (dblgetInterest == 5000 )
            {
                dblloan = Math.Round(dblgetInterest * (6 / 100d));
            }
            

            return dblloan;
        }

        private void txtInterest_MouseEnter(object sender, MouseEventArgs e)
        {
            double dblLoan = (!string.IsNullOrEmpty(txtloan.Text) ? Convert.ToDouble(txtloan.Text) : Convert.ToDouble("0"));
            double dbInt = getInterestEML(dblLoan.ToString());

            txtInterest.Text = dbInt.ToString();
        }

        private void txtInterest_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
