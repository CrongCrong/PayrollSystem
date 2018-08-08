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
    /// Interaction logic for LoansPEL.xaml
    /// </summary>
    public partial class LoansPEL : MetroWindow
    {
        public LoansPEL()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        List<PELModel> lstPendingPEL;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadEmployeesList();
            lstPendingPEL = getPELPending();
            dgvPEL.ItemsSource = loadDatagridDetails();
            btnUpdate.Visibility = Visibility.Hidden;

        }

        private List<PELModel> loadDatagridDetails()
        {
            conDB = new ConnectionDB();
            List<PELModel> lstPEL = new List<PELModel>();
            PELModel pelMod = new PELModel();

            queryString = "SELECT tblloanspel.ID, empID, concat(firstname, ' ', lastname) as fullname, " +
                "sum(loans) as loans, loandate, sum(interest) as interest FROM (tblloanspel INNER JOIN tblemployees ON " +
                " tblloanspel.empID = tblemployees.ID) " +
                "WHERE tblloanspel.isDeleted = 0 GROUP BY tblloanspel.empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                pelMod.ID = reader["ID"].ToString();
                pelMod.EmpID = reader["empID"].ToString();
                pelMod.FullName = reader["fullname"].ToString();
                pelMod.Loan = (string.IsNullOrEmpty(reader["loans"].ToString())) ?
                    "0" : reader["loans"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                pelMod.Interest = (!string.IsNullOrEmpty(reader["interest"].ToString())) ?
                    reader["interest"].ToString() : "0";
                pelMod.TotalLoan = (Convert.ToDouble(pelMod.Loan) +
                    Convert.ToDouble(pelMod.Interest)).ToString();
                pelMod.LoanDate = dte.ToShortDateString();

                foreach (PELModel p in lstPendingPEL)
                {
                    if (p.EmpID.Equals(pelMod.EmpID))
                    {
                        double lo = Convert.ToDouble(pelMod.Loan);
                        double dblPending = Convert.ToDouble(p.Loan);
                        double dblInterests = Convert.ToDouble(pelMod.Interest);
                        pelMod.PendingBalance = (lo - dblPending).ToString();
                        pelMod.Loan = (lo - dblPending).ToString();
                        pelMod.TotalLoan = ((lo - dblPending) + dblInterests).ToString();
                    }
                }

                lstPEL.Add(pelMod);
                pelMod = new PELModel();
            }

            conDB.closeConnection();
            return lstPEL;
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

        private List<PELModel> getPELPending()
        {
            List<PELModel> lstPelPending = new List<PELModel>();
            PELModel pels = new PELModel();

            conDB = new ConnectionDB();

            queryString = "SELECT empID, sum(pel) as pelpending FROM tblpayroll WHERE tblpayroll.isDeleted = 0 GROUP BY empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                pels.EmpID = reader["empID"].ToString();
                pels.Loan = reader["pelpending"].ToString();
                lstPelPending.Add(pels);
                pels = new PELModel();
            }

            conDB.closeConnection();
            return lstPelPending;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO tblloanspel (empID, loans, loandate, interest, isDeleted) " +
                "VALUES (?,?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbEmployees.SelectedValue.ToString());
            parameters.Add(txtloan.Text);
            DateTime date = DateTime.Parse(datePEL.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add(txtinterest.Text);
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("SAVE PEL RECORD: " + "EMPLOYEE ID: " + cmbEmployees.SelectedValue.ToString() + " |LOAN: " +
                txtloan.Text + " |DATE: " + txtloan.Text);
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            PELModel pelMod = dgvPEL.SelectedItem as PELModel;

            if (pelMod != null)
            {
                PELDetails pelDet = new PELDetails(pelMod.EmpID);
                pelDet.ShowDialog();
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
                dgvPEL.ItemsSource = loadDatagridDetails();
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
            else if(string.IsNullOrEmpty(txtinterest.Text) || !ifValidFloatingNumber(txtinterest.Text))
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

        private double getInterestPEL(string pLoan)
        {
            double dblloan = 0;
            double dblgetInterest = Convert.ToDouble(pLoan);

            if(dblgetInterest <= 1999)
            {
                dblloan = 0;
            }
            else if (dblgetInterest >= 2000 && dblgetInterest <= 4000)
            {
                dblloan = Math.Round(dblgetInterest * (4 / 100d));
            }
            else if (dblgetInterest >= 4001 && dblgetInterest <= 6000)
            {
                dblloan = Math.Round(dblgetInterest * (4.5 / 100d));
            }
            else if (dblgetInterest >= 6001 && dblgetInterest <= 8000)
            {
                dblloan = Math.Round(dblgetInterest * (5 / 100d));
            }
            else if (dblgetInterest >= 8001 && dblgetInterest <= 10000)
            {
                dblloan = Math.Round(dblgetInterest * (5.5 / 100d));
            }
            else if (dblgetInterest >= 10001 && dblgetInterest <= 15000)
            {
                dblloan = Math.Round(dblgetInterest * (6 / 100d));
            }
            else if (dblgetInterest >= 15001 && dblgetInterest <= 20000)
            {
                dblloan = Math.Round(dblgetInterest * (7 / 100d));
            }
            else if (dblgetInterest >= 20001 && dblgetInterest <= 30000)
            {
                dblloan = Math.Round(dblgetInterest * (8 / 100d));
            }

            return dblloan;
        }

        private void txtinterest_MouseEnter(object sender, MouseEventArgs e)
        {
            double dblLoan = (!string.IsNullOrEmpty(txtloan.Text) ? Convert.ToDouble(txtloan.Text) : Convert.ToDouble("0"));
            double dbInt = getInterestPEL(dblLoan.ToString());

            txtinterest.Text = dbInt.ToString();
        }

        private void txtinterest_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
