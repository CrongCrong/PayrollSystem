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
    /// Interaction logic for EmployeesViews.xaml
    /// </summary>
    public partial class EmployeesViews : UserControl
    {
        public EmployeesViews()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        String queryString = "";
        List<string> parameters;
        string recordToEditID = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            progressRing.Visibility = Visibility.Hidden;
            btnUpdate.Visibility = Visibility.Hidden;
            loadCompanyList();
            loadEmployeeStatus();
            hideWageBox.Visibility = Visibility.Hidden;
            btnHidePassword.Visibility = Visibility.Hidden;
            btnShowPassword.Visibility = Visibility.Hidden;
            dgvEmployees.ItemsSource = loadDataGridDetails();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EmployeeModel emp = dgvEmployees.SelectedItem as EmployeeModel;

            if(emp != null)
            {
                dgvEmployees.IsEnabled = false;
                recordToEditID = emp.ID;
                txtEmployeeID.Text = emp.EmployeeID;
                txtFirstName.Text = emp.FirstName;
                txtLastName.Text = emp.LastName;

                txtWage.Text = emp.Wage;
                hideWageBox.Password = emp.Wage;

                startDate.Text = emp.StartDate;
                txtSSS.Text = emp.SSS;
                txtPhilHealth.Text = emp.PhilHealth;
                txtPagibig.Text = emp.Pagibig;
                txtSSSLoan.Text = emp.SSSLoan;
                txtPEL.Text = emp.PEL;
                txtEML.Text = emp.EML;
                txtGRL.Text = emp.GRL;
                txtPEY.Text = emp.PEY;
                txtElecBill.Text = emp.ElecBill;
                txtAllowance.Text = emp.Allowance;

                foreach(CompanyModel cp in cmbCompany.Items)
                {
                    if (cp.ID.Equals(emp.CompanyID))
                    {
                        cmbCompany.SelectedItem = cp;
                    }
                }

                foreach(EmployeeStatusModel em in cmbStatus.Items)
                {
                    if (em.ID.Equals(emp.Status))
                    {
                        cmbStatus.SelectedItem = em;
                    }
                }

                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
                hideWageBox.Visibility = Visibility.Visible;
                btnShowPassword.Visibility = Visibility.Visible;
                txtFirstName.IsEnabled = false;
                txtLastName.IsEnabled = false;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (x)
            {
                progressRing.Visibility = Visibility.Visible;
                progressRing.IsActive = true;

                saveEmployeeRecord();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                clearFields();
                dgvEmployees.ItemsSource = loadDataGridDetails();
                progressRing.IsActive = false;
                progressRing.Visibility = Visibility.Hidden;
            }

        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (x)
            {
                updateEmployeesRecord(recordToEditID);
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                clearFields();
                dgvEmployees.ItemsSource = loadDataGridDetails();
                dgvEmployees.IsEnabled = true;
                btnUpdate.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            hideWageBox.Visibility = Visibility.Hidden;
            txtFirstName.IsEnabled = true;
            txtLastName.IsEnabled = true;
            dgvEmployees.IsEnabled = true;
            btnShowPassword.Visibility = Visibility.Hidden;
            
        }

        private async Task<bool> checkFields()
        {
            bool bl = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;


            if (string.IsNullOrEmpty(txtEmployeeID.Text))
            {
                await window.ShowMessageAsync("Employee ID", "Please provide employee ID.");
            }
            else if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                await window.ShowMessageAsync("First name", "Please provide first name.");
            }else if (string.IsNullOrEmpty(txtLastName.Text))
            {
                await window.ShowMessageAsync("Last name", "Please provide last name.");
            }else if(cmbCompany.SelectedItem == null)
            {
                await window.ShowMessageAsync("Company", "Please select company.");
            }else if (string.IsNullOrEmpty(txtWage.Text))
            {
                await window.ShowMessageAsync("Wage", "Please provide wage.");
            }else if (string.IsNullOrEmpty(startDate.Text))
            {
                await window.ShowMessageAsync("Date", "Please select date.");
            }else if(cmbStatus.SelectedItem == null)
            {
                await window.ShowMessageAsync("Status", "Please select employee status.");
            }else if (string.IsNullOrEmpty(txtSSS.Text) || !ifValidFloatingNumber(txtSSS.Text))
            {
                await window.ShowMessageAsync("SSS", "Please provide valid SSS value.");
            }
            else if (string.IsNullOrEmpty(txtPhilHealth.Text) || !ifValidFloatingNumber(txtPhilHealth.Text))
            {
                await window.ShowMessageAsync("Phil. Health", "Please provide valid Phil. Healh value.");
            }
            else if (string.IsNullOrEmpty(txtPagibig.Text) || !ifValidFloatingNumber(txtPagibig.Text))
            {
                await window.ShowMessageAsync("Pag-ibig", "Please provide valid Pag-ibig value.");
            }
            else if (string.IsNullOrEmpty(txtSSSLoan.Text) || !ifValidFloatingNumber(txtSSSLoan.Text))
            {
                await window.ShowMessageAsync("SSS Loan", "Please provide valid SSS Loan value.");
            }
            else if (string.IsNullOrEmpty(txtPEL.Text) || !ifValidFloatingNumber(txtPEL.Text))
            {
                await window.ShowMessageAsync("PEL", "Please provide valid PEL value.");
            }
            else if (string.IsNullOrEmpty(txtEML.Text) || !ifValidFloatingNumber(txtEML.Text))
            {
                await window.ShowMessageAsync("EML", "Please provide valid EML value.");
            }
            else if (string.IsNullOrEmpty(txtGRL.Text) || !ifValidFloatingNumber(txtGRL.Text))
            {
                await window.ShowMessageAsync("GRL", "Please provide valid GRL value.");
            }
            else if (string.IsNullOrEmpty(txtPEY.Text) || !ifValidFloatingNumber(txtPEY.Text))
            {
                await window.ShowMessageAsync("PEY", "Please provide valid PEY value.");
            }
            else if (string.IsNullOrEmpty(txtElecBill.Text) || !ifValidFloatingNumber(txtElecBill.Text))
            {
                await window.ShowMessageAsync("Elec. bill", "Please provide valid Elec. bill value.");
            }
            else if (string.IsNullOrEmpty(txtAllowance.Text) || !ifValidFloatingNumber(txtAllowance.Text))
            {
                await window.ShowMessageAsync("Allowance", "Please provide valid Allowance value.");
            }
            else
            {
                bl = true;
            }

            return bl;
        }

        private void clearFields()
        {
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtWage.Text = "";
            startDate.Text = "";
            txtSSS.Text = "";
            txtPhilHealth.Text = "";
            txtPagibig.Text = "";
            txtSSSLoan.Text = "";
            txtPEL.Text = "";
            txtEML.Text = "";
            txtGRL.Text = "";
            txtPEY.Text = "";
            txtElecBill.Text = "";
            txtAllowance.Text = "";
            cmbStatus.SelectedItem = null;
            cmbCompany.SelectedItem = null;
        }

        private void loadCompanyList()
        {
            conDB = new ConnectionDB();
            cmbCompany.Items.Clear();
            queryString = "SELECT ID, companyname, description FROM tblcompany WHERE isDeleted = 0";
            CompanyModel company = new CompanyModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                company.ID = reader["ID"].ToString();
                company.CompanyName = reader["companyname"].ToString();
                company.Description = reader["description"].ToString();
                cmbCompany.Items.Add(company);
                company = new CompanyModel();
            }
            conDB.closeConnection();           
        }

        private void loadEmployeeStatus()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT ID, status, description FROM tblempstatus WHERE isDeleted = 0";
            EmployeeStatusModel empStatus = new EmployeeStatusModel();
            cmbStatus.Items.Clear();
            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                empStatus.ID = reader["ID"].ToString();
                empStatus.Status = reader["status"].ToString();
                empStatus.Description = reader["description"].ToString();
                cmbStatus.Items.Add(empStatus);
                empStatus = new EmployeeStatusModel();
               
            }
            conDB.closeConnection();
        }

        private List<EmployeeModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<EmployeeModel> lstEmployees = new List<EmployeeModel>();
            EmployeeModel employee = new EmployeeModel();

            queryString = "SELECT tblemployees.ID, employeeID, firstname, lastname, status, " +
                "incomeperday, companyID, description, startdate, empsss, empphilhealth, emppagibig, empsssloan, " +
                "emppel, empeml, empgrl, emppey, empelecbill, empallowance FROM " +
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
                employee.Status = reader["status"].ToString();
                string dd = !string.IsNullOrEmpty(reader["startdate"].ToString()) ? reader["startdate"].ToString() : DateTime.Now.ToString();
                DateTime dte = DateTime.Parse(dd);
                employee.StartDate = dte.ToShortDateString();
                employee.SSS = reader["empsss"].ToString();
                employee.PhilHealth = reader["empphilhealth"].ToString();
                employee.Pagibig = reader["emppagibig"].ToString();
                employee.SSSLoan = reader["empsssloan"].ToString();
                employee.PEL = reader["emppel"].ToString();
                employee.EML = reader["empeml"].ToString();
                employee.GRL = reader["empgrl"].ToString();
                employee.PEY = reader["emppey"].ToString();
                employee.ElecBill = reader["empelecbill"].ToString();
                employee.Allowance = reader["empallowance"].ToString();
                lstEmployees.Add(employee);
                employee = new EmployeeModel();

            }
            conDB.closeConnection();

            return lstEmployees;
        }

        private void saveEmployeeRecord()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO tblemployees (employeeID, firstname, lastname, incomeperday, " +
                " companyID, startdate, status, empsss, empphilhealth, emppagibig, empsssloan, emppel, empeml, " +
                "empgrl, emppey, empelecbill, empallowance, isDeleted) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(txtEmployeeID.Text);
            parameters.Add(txtFirstName.Text);
            parameters.Add(txtLastName.Text);
            parameters.Add(txtWage.Text);
            parameters.Add(cmbCompany.SelectedValue.ToString());
            DateTime sdate = DateTime.Parse(startDate.Text);
            parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            parameters.Add(cmbStatus.SelectedValue.ToString());
            parameters.Add(txtSSS.Text);
            parameters.Add(txtPhilHealth.Text);
            parameters.Add(txtPagibig.Text);
            parameters.Add(txtSSSLoan.Text);
            parameters.Add(txtPEL.Text);
            parameters.Add(txtEML.Text);
            parameters.Add(txtGRL.Text);
            parameters.Add(txtPEY.Text);
            parameters.Add(txtElecBill.Text);
            parameters.Add(txtAllowance.Text);
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.writeLogFile("SAVE EMPLOYEE RECORD: " + "Employee name: " + txtFirstName.Text + " " 
                + txtLastName.Text);
        }

        private void updateEmployeesRecord(string ID)
        {
            conDB = new ConnectionDB();

            queryString = "UPDATE tblemployees SET employeeID = ?, firstname = ?, lastname = ?, incomeperday = ?, " +
                "companyID = ?, startdate = ?, status = ?, empsss = ?, empphilhealth = ?, emppagibig = ?, empsssloan = ?, " +
                "emppel = ?, empeml = ?, empgrl = ?, emppey = ?, empelecbill = ?, empallowance = ? WHERE ID = ?";

            parameters = new List<string>();

            parameters.Add(txtEmployeeID.Text);
            parameters.Add(txtFirstName.Text);
            parameters.Add(txtLastName.Text);
            parameters.Add(txtWage.Text);
            parameters.Add(cmbCompany.SelectedValue.ToString());
            DateTime sdate = DateTime.Parse(startDate.Text);
            parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            parameters.Add(cmbStatus.SelectedValue.ToString());
            parameters.Add(txtSSS.Text);
            parameters.Add(txtPhilHealth.Text);
            parameters.Add(txtPagibig.Text);
            parameters.Add(txtSSSLoan.Text);
            parameters.Add(txtPEL.Text);
            parameters.Add(txtEML.Text);
            parameters.Add(txtGRL.Text);
            parameters.Add(txtPEY.Text);
            parameters.Add(txtElecBill.Text);
            parameters.Add(txtAllowance.Text);
            parameters.Add(ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("UPDATE EMPLOYEE RECORD: EMPLOYEE: " + txtFirstName.Text + " " + txtLastName.Text);
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void txtWage_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void deleteRecord(string eID)
        {
            conDB = new ConnectionDB();

            queryString = "UPDATE tblemployees SET isDeleted = 1 WHERE ID = ?";
            parameters = new List<string>();
            parameters.Add(eID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("DELETE EMPLOYEE RECORD: EMPLOYEE DB ID: " + eID);

        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            EmployeeModel emMod = dgvEmployees.SelectedItem as EmployeeModel;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (emMod != null)
            {
                MessageDialogResult result = await window.ShowMessageAsync("Delete Record", "Are you sure you want to delete record?", MessageDialogStyle.AffirmativeAndNegative);

                if (result.Equals(MessageDialogResult.Affirmative))
                {
                    deleteRecord(emMod.ID);
                    dgvEmployees.ItemsSource = loadDataGridDetails();
                    await window.ShowMessageAsync("Delete Record", "Record deleted successfully!");
                }
            }
        }

        private bool ifValidFloatingNumber(string txtField)
        {
            bool ifOKay = false;

            ifOKay = Regex.IsMatch(txtField, "^[+-]?([0-9]*[.])?[0-9]+$");

            return ifOKay;
        }

        private void txtSSS_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtPhilHealth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtPagibig_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtSSSLoan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtPEL_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtEML_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtGRL_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtPEY_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtElecBill_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtAllowance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            PinCodeWindow pinCode = new PinCodeWindow(this);
            pinCode.ShowDialog();
        }

        private void btnHidePassword_Click(object sender, RoutedEventArgs e)
        {
            btnShowPassword.Visibility = Visibility.Visible;
            btnHidePassword.Visibility = Visibility.Hidden;
            hideWageBox.Visibility = Visibility.Visible;
        }
    }
}
