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
    /// Interaction logic for ISAPView.xaml
    /// </summary>
    public partial class ISAPView : UserControl
    {
        public ISAPView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        MahApps.Metro.Controls.MetroWindow window;
        List<ISAPModel> lstISAPToAdd;
        List<string> parameters;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadEmployeesList();
            lstISAPToAdd = getISAPToADD();
            dgvISAP.ItemsSource = getISAPRecord();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<ISAPModel> getISAPRecord()
        {
            List<ISAPModel> lstisap = new List<ISAPModel>();
            ISAPModel isap = new ISAPModel();

            conDB = new ConnectionDB();

            queryString = "SELECT tblisap.empid, sum(existingisap) as existingisap, concat(lastname,' , ', firstname) as fullname, " +
                "dateadded FROM (dbfhpayroll.tblisap INNER JOIN dbfhpayroll.tblemployees ON " +
                "tblisap.empID = tblemployees.ID) WHERE dbfhpayroll.tblisap.isDeleted = 0 GROUP BY tblisap.empid";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                isap.empID = reader["empID"].ToString();
                isap.FullName = reader["fullname"].ToString();
                double current = Convert.ToDouble(reader["existingisap"].ToString());

                foreach (ISAPModel ism in lstISAPToAdd)
                {
                    if (isap.empID.Equals(ism.empID))
                    {
                        current = current + Convert.ToDouble(ism.ISAPtoAdd);
                    }
                }

                DateTime dte = DateTime.Parse(reader["dateadded"].ToString());
                isap.DateAdded = dte.ToShortDateString();
                isap.CurrentISAP = current.ToString();
                lstisap.Add(isap);
                isap = new ISAPModel();
            }
            conDB.closeConnection();
            return lstisap;
        }

        private List<ISAPModel> getISAPToADD()
        {
            List<ISAPModel> listISAP = new List<ISAPModel>();
            ISAPModel isaps = new ISAPModel();

            conDB = new ConnectionDB();

            queryString = "SELECT empID, sum(isap) as isap FROM dbfhpayroll.tblpayroll WHERE tblpayroll.isDeleted = 0 GROUP BY empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                isaps.empID = reader["empID"].ToString();
                isaps.ISAPtoAdd = reader["isap"].ToString();
                listISAP.Add(isaps);
                isaps = new ISAPModel();
            }
            conDB.closeConnection();
            return listISAP;
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
            else if (string.IsNullOrEmpty(txtCurrentIS.Text) || !ifValidFloatingNumber(txtCurrentIS.Text))
            {
                await window.ShowMessageAsync("IS", "Please input valid Intact Savings value.");
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
            txtCurrentIS.Text = "";
        }

        private void saveISRecord()
        {
            conDB = new ConnectionDB();

            queryString = "INSERT INTO dbfhpayroll.tblisap (empID, existingisap, dateadded, isDeleted) VALUES (?,?,?,0)";
            EmployeeModel emm = cmbEmployees.SelectedItem as EmployeeModel;

            parameters = new List<string>();
            parameters.Add(cmbEmployees.SelectedValue.ToString());
            parameters.Add(txtCurrentIS.Text);
            DateTime date = DateTime.Parse(dateIS.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("SAVE IS RECORD: " + "EMPLOYEE ID: " + emm.FirstName + " " + emm.LastName +
                "| CURRENT IS: " + txtCurrentIS.Text);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                saveISRecord();
                dgvISAP.ItemsSource = getISAPRecord();
                await window.ShowMessageAsync("RECORD SAVED", "Record successfully saved.");
                clearFields();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            ISAPModel intact = dgvISAP.SelectedItem as ISAPModel;

            if (intact != null)
            {
                ISAPDetails isDetails = new ISAPDetails(intact.empID);
                isDetails.ShowDialog();
            }
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

        private void txtCurrentIS_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
