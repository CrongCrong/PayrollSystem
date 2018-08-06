using MahApps.Metro.Controls.Dialogs;
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
    /// Interaction logic for SSSLoanView.xaml
    /// </summary>
    public partial class SSSLoanView : UserControl
    {
        public SSSLoanView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        MahApps.Metro.Controls.MetroWindow window;
        List<SSSLoanModel> lstSSSLoanToAdd;
        List<string> parameters;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadEmployeesList();
            lstSSSLoanToAdd = getSSSToAdd();
            dgvSSSLoan.ItemsSource = getSSSRecord();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<SSSLoanModel> getSSSRecord()
        {
            List<SSSLoanModel> lstSSS = new List<SSSLoanModel>();
            SSSLoanModel sss = new SSSLoanModel();

            conDB = new ConnectionDB();

            queryString = "SELECT tblsssloan.empid, sum(sssloan) as existingsss, concat(lastname,' , ', firstname) as fullname, " +
                "dateadded FROM (dbfhpayroll.tblsssloan INNER JOIN dbfhpayroll.tblemployees ON " +
                "tblsssloan.empID = tblemployees.ID) WHERE dbfhpayroll.tblsssloan.isDeleted = 0 GROUP BY tblsssloan.empid";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                sss.empID = reader["empID"].ToString();
                sss.FullName = reader["fullname"].ToString();
                double current = Convert.ToDouble(reader["existingsss"].ToString());

                foreach (SSSLoanModel ism in lstSSSLoanToAdd)
                {
                    if (sss.empID.Equals(ism.empID))
                    {
                        current = current - Convert.ToDouble(ism.SSStoAdd);
                    }
                }

                DateTime dte = DateTime.Parse(reader["dateadded"].ToString());
                sss.DateAdded = dte.ToShortDateString();
                sss.CurrentSSSLoan = current.ToString();
                lstSSS.Add(sss);
                sss = new SSSLoanModel();
            }
            conDB.closeConnection();
            return lstSSS;
        }

        private List<SSSLoanModel> getSSSToAdd()
        {
            List<SSSLoanModel> listSSS = new List<SSSLoanModel>();
            SSSLoanModel sss = new SSSLoanModel();

            conDB = new ConnectionDB();

            queryString = "SELECT empID, sum(sssloan) as sssloan FROM dbfhpayroll.tblpayroll WHERE tblpayroll.isDeleted = 0 GROUP BY empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                sss.empID = reader["empID"].ToString();
                sss.SSStoAdd = reader["sssloan"].ToString();
                listSSS.Add(sss);
                sss = new SSSLoanModel();
            }
            conDB.closeConnection();
            return listSSS;
        }

        private void loadEmployeesList()
        {
            conDB = new ConnectionDB();
            List<EmployeeModel> lstEmployees = new List<EmployeeModel>();
            EmployeeModel employee = new EmployeeModel();

            cmbEmployees.Items.Clear();

            queryString = "SELECT dbfhpayroll.tblemployees.ID, employeeID, firstname, lastname, status, incomeperday, companyID, description FROM " +
                "(dbfhpayroll.tblemployees INNER JOIN dbfhpayroll.tblcompany ON tblemployees.companyID = tblcompany.ID) " +
                "WHERE dbfhpayroll.tblemployees.isDeleted = 0";

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
            else if (string.IsNullOrEmpty(txtSSSLoan.Text) || !ifValidFloatingNumber(txtSSSLoan.Text))
            {
                await window.ShowMessageAsync("SSS", "Please input valid SSS loan value.");
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
            txtSSSLoan.Text = "";
        }

        private void saveSSSRecord()
        {
            conDB = new ConnectionDB();

            queryString = "INSERT INTO dbfhpayroll.tblsssloan (empID, sssloan, dateadded, isDeleted) VALUES (?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbEmployees.SelectedValue.ToString());
            parameters.Add(txtSSSLoan.Text);
            DateTime date = DateTime.Parse(dateIS.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            SSSLoanModel sLo = dgvSSSLoan.SelectedItem as SSSLoanModel;

            if (sLo != null)
            {
                SSSLoanDetails sloD = new SSSLoanDetails(sLo.empID);
                sloD.ShowDialog();
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
                dgvSSSLoan.ItemsSource = getSSSRecord();
                await window.ShowMessageAsync("RECORD SAVED", "Record successfully saved.");
                clearFields();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

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

        private void txtSSSLoan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
