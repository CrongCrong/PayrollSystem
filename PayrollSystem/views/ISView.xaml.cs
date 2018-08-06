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
    /// Interaction logic for ISView.xaml
    /// </summary>
    public partial class ISView : UserControl
    {
        public ISView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        MahApps.Metro.Controls.MetroWindow window;
        List<IntactSavingsModel> lstSavingsToAdd;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //dgvIS.ItemsSource = loadDatagridDetails();\
            lstSavingsToAdd = getSavingsToAdd();
            dgvIS.ItemsSource = getIntactSavingsRecord();
            loadEmployeesList();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<IntactSavingsModel> getIntactSavingsRecord()
        {
            List<IntactSavingsModel> lstIntactSavings = new List<IntactSavingsModel>();
            IntactSavingsModel intactsavings = new IntactSavingsModel();

            conDB = new ConnectionDB();

            queryString = "SELECT tblintactsavings.empid, sum(existingis) as existingis, concat(lastname,' , ', firstname) as fullname," +
                " dateadded FROM (dbfhpayroll.tblintactsavings INNER JOIN dbfhpayroll.tblemployees ON " +
                "tblintactsavings.empID = tblemployees.ID) WHERE dbfhpayroll.tblintactsavings.isDeleted = 0 GROUP BY tblintactsavings.empid";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                intactsavings.empID = reader["empID"].ToString();
                intactsavings.FullName = reader["fullname"].ToString();
                double current = Convert.ToDouble(reader["existingIS"].ToString());
                
                foreach(IntactSavingsModel ism in lstSavingsToAdd)
                {
                    if (intactsavings.empID.Equals(ism.empID))
                    {
                        current = current + Convert.ToDouble(ism.SavingsToAdd);
                    }
                }

                DateTime dte = DateTime.Parse(reader["dateadded"].ToString());
                intactsavings.DateAdded = dte.ToShortDateString();
                intactsavings.CurrentIS = current.ToString();
                lstIntactSavings.Add(intactsavings);
                intactsavings = new IntactSavingsModel();
            }
            conDB.closeConnection();
            return lstIntactSavings;
        }

        private List<IntactSavingsModel> getSavingsToAdd()
        {
            List<IntactSavingsModel> lstIntactSavings = new List<IntactSavingsModel>();
            IntactSavingsModel intactsavings = new IntactSavingsModel();

            conDB = new ConnectionDB();

            queryString = "SELECT empID, sum(isavings) as isavings FROM dbfhpayroll.tblpayroll WHERE tblpayroll.isDeleted = 0 GROUP BY empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                intactsavings.empID = reader["empID"].ToString();
                intactsavings.SavingsToAdd  = reader["isavings"].ToString();
                lstIntactSavings.Add(intactsavings);
                intactsavings = new IntactSavingsModel();
            }
            conDB.closeConnection();
            return lstIntactSavings;
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
            }else if (string.IsNullOrEmpty(txtCurrentIS.Text) || !ifValidFloatingNumber(txtCurrentIS.Text))
            {
                await window.ShowMessageAsync("IS", "Please input valid Intact Savings value.");
            }else if (cmbEmployees.SelectedItem == null)
            {
                await window.ShowMessageAsync("Employees", "Please select an employee.");
            }else
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

            queryString = "INSERT INTO dbfhpayroll.tblintactsavings (empID, existingIS, dateadded, isDeleted) VALUES (?,?,?,0)";
            EmployeeModel emoe = cmbEmployees.SelectedItem as EmployeeModel;

            parameters = new List<string>();
            parameters.Add(cmbEmployees.SelectedValue.ToString());
            parameters.Add(txtCurrentIS.Text);
            DateTime date = DateTime.Parse(dateIS.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("SAVE IS RECORD : EMPLOYEE ID: " +  emoe.ID + " CURRENT IS: " + txtCurrentIS.Text);
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
                dgvIS.ItemsSource = getIntactSavingsRecord();
                await window.ShowMessageAsync("RECORD SAVED", "Record successfully saved.");
                clearFields();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
             
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            IntactSavingsModel intact = dgvIS.SelectedItem as IntactSavingsModel;

            if(intact != null)
            {
                IntactSavingsDetails isDetails = new IntactSavingsDetails(intact.empID);
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
