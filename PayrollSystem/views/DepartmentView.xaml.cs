using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PayrollSystem.views
{
    /// <summary>
    /// Interaction logic for DepartmentView.xaml
    /// </summary>
    public partial class DepartmentView : UserControl
    {
        public DepartmentView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string recordIDtoEdit = "";
        MahApps.Metro.Controls.MetroWindow window;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            dgvDepartments.ItemsSource = loadDataGridDetails();
        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            CompanyModel comp = dgvDepartments.SelectedItem as CompanyModel;

            if(comp != null)
            {
                recordIDtoEdit = comp.ID;
                txtCompanyName.Text = comp.CompanyName;
                txtDescription.Text = comp.Description;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
                dgvDepartments.IsEnabled = false;
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (x)
            {
                updateCompany(recordIDtoEdit);
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                dgvDepartments.ItemsSource = loadDataGridDetails();
                dgvDepartments.Items.Refresh();
                clearFields();
                dgvDepartments.IsEnabled = true;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (x)
            {
                saveCompany();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                dgvDepartments.ItemsSource = loadDataGridDetails();
                dgvDepartments.Items.Refresh();
                clearFields();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            dgvDepartments.IsEnabled = true;
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private void clearFields()
        {
            txtCompanyName.Text = "";
            txtDescription.Text = "";
        }

        private async Task<bool> checkFields()
        {
            bool bl = false;
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtCompanyName.Text))
            {
                await window.ShowMessageAsync("COMPANY NAME", "Please provide company name.");
            }else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await window.ShowMessageAsync("Description", "Please provide description");
            }else
            {
                bl = true;
            }
            return bl;
        }

        private List<CompanyModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            CompanyModel company = new CompanyModel();
            List<CompanyModel> lstCompany = new List<CompanyModel>();
            string queryString = "SELECT ID, companyname, description FROM tblcompany WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                company.ID = reader["ID"].ToString();
                company.CompanyName = reader["companyname"].ToString();
                company.Description = reader["description"].ToString();
                lstCompany.Add(company);
                company = new CompanyModel();
            }
            conDB.closeConnection();
            return lstCompany;
        }

        private void saveCompany()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO tblcompany (companyname, description, isDeleted) " +
                "VALUES (?,?,0)";

            List<string> parameters = new List<string>();
            parameters.Add(txtCompanyName.Text);
            parameters.Add(txtDescription.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.writeLogFile("SAVE COMPANY RECORD: " + txtCompanyName.Text + " |DESCRIPTION: " + txtDescription.Text);
            conDB.closeConnection();
        }

        private void updateCompany(string strID)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE tblcompany SET companyname = ?, description = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(txtCompanyName.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(strID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }
    }
}
