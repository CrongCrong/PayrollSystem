using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PayrollSystem.views
{
    /// <summary>
    /// Interaction logic for UsersView.xaml
    /// </summary>
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
        }

        MahApps.Metro.Controls.MetroWindow window;
        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        string recordID = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvUsers.ItemsSource = loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
            lblPin.Visibility = Visibility.Hidden;
            txtPin.Visibility = Visibility.Hidden;
        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            UserDisplay us = dgvUsers.SelectedItem as UserDisplay;
            
            if (us != null)
            {
                dgvUsers.IsEnabled = false;
                btnUpdate.Visibility = Visibility.Visible;
                txtPassword.Password = "";
                txtVerifyPassword.Password = "";
                recordID = us.ID; 
                txtName.IsEnabled = false;
                txtName.Text = us.Name;
                txtUsername.Text = us.Username;
                txtPassword.Password = us.Password;
                txtVerifyPassword.Password = us.Password;
                if (us.isAdmin)
                {
                    chkAdmin.IsChecked = true;
                    txtPin.Text = us.PIN;
                }
                else
                {
                    chkAdmin.IsChecked = false;
                }
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            bool y = false;
            if (x)
            {
                y = await checkifPasswordSame();
                if (y)
                {
                    updateRecord(recordID);
                    clearFields();
                    await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                    dgvUsers.ItemsSource = loadDataGridDetails();
                    dgvUsers.IsEnabled = true;
                    btnUpdate.Visibility = Visibility.Hidden;
                }
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            bool y = false;
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            if (x)
            {
                y = await checkifPasswordSame();
                if (y)
                {
                    saveRecord();
                    clearFields();
                    await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                    dgvUsers.ItemsSource = loadDataGridDetails();
                }

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            dgvUsers.IsEnabled = true;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            lblPin.Visibility = Visibility.Visible;
            txtPin.Visibility = Visibility.Visible;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            lblPin.Visibility = Visibility.Hidden;
            txtPin.Visibility = Visibility.Hidden;
        }

        private void clearFields()
        {
            txtName.Text = "";
            txtUsername.Text = "";
            txtPassword.Password = "";
            txtVerifyPassword.Password = "";
            chkAdmin.IsChecked = false;
            txtPin.Text = "";
            lblPin.Visibility = Visibility.Hidden;
            txtPin.Visibility = Visibility.Hidden;
        }

        private List<UserDisplay> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            UserDisplay userDisp = new UserDisplay();
            List<UserDisplay> lstUserDisp = new List<UserDisplay>();

            queryString = "SELECT ID, name, username, cast(aes_decrypt(dbfhpayroll.tbluser.password, ?)as CHAR) as pas, isAdmin, isViewing, pincode FROM " +
                "dbfhpayroll.tbluser WHERE isDeleted = 0";
            parameters = new List<string>();
            parameters.Add("sp3ctrum");

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                userDisp.ID = reader["ID"].ToString();
                userDisp.Name = reader["name"].ToString();
                userDisp.Username = reader["username"].ToString();
                userDisp.Password = reader["pas"].ToString();
                string i = reader["isAdmin"].ToString();
                if (i.Equals("1"))
                {
                    userDisp.isAdmin = true;
                    userDisp.Admin = i;
                }
                else
                {
                    userDisp.isAdmin = false;
                    userDisp.Admin = i;
                }

                i = reader["isViewing"].ToString();
                if (i.Equals("1"))
                {
                    userDisp.isViewing = true;
                    userDisp.Admin = i;
                }
                else
                {
                    userDisp.isViewing = false;
                    userDisp.Admin = i;
                }
                userDisp.PIN = reader["pincode"].ToString();
                lstUserDisp.Add(userDisp);
                userDisp = new UserDisplay();
            }

            return lstUserDisp;
        }

        private async Task<bool> checkFields()
        {
            bool ifCorrect = false;

            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            if (string.IsNullOrEmpty(txtName.Text))
            {
                await window.ShowMessageAsync("NAME", "Please type name!");
            }
            else if (string.IsNullOrEmpty(txtUsername.Text))
            {
                await window.ShowMessageAsync("USERNAME", "Please type username!");
            }
            else if (string.IsNullOrEmpty(txtPassword.Password) && string.IsNullOrEmpty(txtVerifyPassword.Password))
            {
                await window.ShowMessageAsync("PASSWORD", "Please type password!");
            }
            else if (chkAdmin.IsChecked.Value && string.IsNullOrEmpty(txtPin.Text))
            {
                await window.ShowMessageAsync("PIN", "Please enter PIN!");
            }
            else
            {
                ifCorrect = true;
            }

            return ifCorrect;
        }

        private async Task<bool> checkifPasswordSame()
        {
            bool ifSame = false;
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            if (txtPassword.Password.Equals(txtVerifyPassword.Password))
            {
                ifSame = true;
            }
            else
            {
                await window.ShowMessageAsync("PASSWORD", "Password mismatch!");
            }

            return ifSame;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            queryString = "INSERT INTO dbfhpayroll.tbluser (name, username, password, isAdmin, isViewing, pincode, isDeleted) " +
                "VALUES (?,?, aes_encrypt(?,?),?,?,?,0)";

            parameters = new List<string>();

            parameters.Add(txtName.Text);
            parameters.Add(txtUsername.Text);
            parameters.Add(txtPassword.Password);
            parameters.Add("sp3ctrum");
            if (chkAdmin.IsChecked.Value)
            {
                parameters.Add("1");
                parameters.Add("1");
                parameters.Add(txtPin.Text);
            }
            else
            {
                parameters.Add("0");
                parameters.Add("1");
                parameters.Add("NULL");
            }

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void updateRecord(string eID)
        {
            conDB = new ConnectionDB();

            queryString = "UPDATE dbfhpayroll.tbluser SET username = ?, password = aes_encrypt(?,?), isAdmin = ?," +
                " isViewing = ?, pincode  = ? WHERE ID = ?";

            parameters = new List<string>();
            parameters.Add(txtUsername.Text);
            parameters.Add(txtPassword.Password);
            parameters.Add("sp3ctrum");

            if (chkAdmin.IsChecked.Value)
            {
                parameters.Add("1");
                parameters.Add("1");
                parameters.Add(txtPin.Text);
            }
            else
            {
                parameters.Add("0");
                parameters.Add("1");
                parameters.Add("NULL");
            }

            parameters.Add(eID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection(); 
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void txtPin_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
