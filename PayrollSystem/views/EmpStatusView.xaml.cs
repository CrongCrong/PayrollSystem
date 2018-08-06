using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PayrollSystem.views
{
    /// <summary>
    /// Interaction logic for EmpStatusView.xaml
    /// </summary>
    public partial class EmpStatusView : UserControl
    {
        public EmpStatusView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        MahApps.Metro.Controls.MetroWindow window;
        string queryString = "";
        List<string> parameters;
        string strRecordID = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            dgvEmpStatus.ItemsSource = loadDatagridDetails();
        }

        private List<EmployeeStatusModel> loadDatagridDetails()
        {
            conDB = new ConnectionDB();
            List<EmployeeStatusModel> lstEmpStatusModel = new List<EmployeeStatusModel>();
            EmployeeStatusModel emp = new EmployeeStatusModel();

            queryString = "SELECT ID, status, description FROM dbfhpayroll.tblempstatus WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                emp.ID = reader["ID"].ToString();
                emp.Status = reader["status"].ToString();
                emp.Description = reader["description"].ToString();
                lstEmpStatusModel.Add(emp);
                emp = new EmployeeStatusModel();
            }

            conDB.closeConnection();

            return lstEmpStatusModel;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            queryString = "INSERT INTO dbfhpayroll.tblempstatus (status, description, isDeleted) VALUES (?,?,0)";
            parameters = new List<string>();

            parameters.Add(txtEmpStatus.Text);
            parameters.Add(txtDescription.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("SAVE STATUS RECORD: " + txtEmpStatus.Text + " |DESCRIPTION: " + txtDescription.Text);
        }

        private void updateRecord(string recID)
        {
            conDB = new ConnectionDB();

            queryString = "UPDATE dbfhpayroll.tblempstatus SET status = ?, description = ? WHERE ID = ?";

            parameters = new List<string>();
            parameters.Add(txtEmpStatus.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(recID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void clearFields()
        {
            txtEmpStatus.Text = "";
            txtDescription.Text = "";
        }

        private async Task<bool> checkFields()
        {
            bool bl = false;
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtEmpStatus.Text))
            {
                await window.ShowMessageAsync("COMPANY NAME", "Please provide company name.");
            }
            else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await window.ShowMessageAsync("Description", "Please provide description");
            }
            else
            {
                bl = true;
            }

            return bl;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (x)
            {
                saveRecord();
                clearFields();
                dgvEmpStatus.ItemsSource = loadDatagridDetails();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully.");
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (x)
            {
                updateRecord(strRecordID);
                clearFields();
                dgvEmpStatus.ItemsSource = loadDatagridDetails();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully.");
                btnSave.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Hidden;
                //dgvEmpStatus.IsEnabled = true;

            }
        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            EmployeeStatusModel empStatus = dgvEmpStatus.SelectedItem as EmployeeStatusModel;

            if(empStatus != null)
            {
                strRecordID = empStatus.ID;
                txtEmpStatus.Text = empStatus.Status;
                txtDescription.Text = empStatus.Description;
                dgvEmpStatus.IsEnabled = false;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            dgvEmpStatus.IsEnabled = true;
            btnSave.Visibility = Visibility.Visible;
            btnUpdate.Visibility = Visibility.Hidden;
        }
    }
}
