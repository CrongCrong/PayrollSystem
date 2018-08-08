using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private bool ifLoginSuccess()
        {
            bool x = false;

            conDB = new ConnectionDB();
            string tempStrAdmin = "";
            string tempStrViewing = "";

            string queryString = "SELECT tbluser.ID, username, aes_decrypt(tbluser.password, ?) as pas, isAdmin, isViewing, " +
                "pincode FROM tbluser WHERE aes_decrypt(tbluser.password, ?) = ? AND username = ? AND tbluser.isDeleted = 0";

            List<string> parameters = new List<string>();
            parameters.Add("sp3ctrum");
            parameters.Add("sp3ctrum");
            parameters.Add(txtPassword.Password);
            parameters.Add(txtUsername.Text);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                UserModel.Username = reader["username"].ToString();
                UserModel.Password = reader["pas"].ToString();
                UserModel.PIN = reader["pincode"].ToString();
                tempStrAdmin = reader["isAdmin"].ToString();
                tempStrViewing = reader["isViewing"].ToString();
                x = true;
                conDB.writeLogFile("Logged in: username: " + UserModel.Username);
            }

            if (tempStrAdmin.Equals("1"))
            {
                UserModel.isAdmin = true;
            }else
            {
                UserModel.isAdmin = false;
            }

            if (tempStrViewing.Equals("1"))
            {
                UserModel.isViewing = true;
            }else
            {
                UserModel.isViewing = false;
            }
            
            return x;
        }

        private void btnLOgin_Click(object sender, RoutedEventArgs e)
        {
            if (ifLoginSuccess())
            {
                HomeWindow home = new HomeWindow(this);
                home.Show();
                this.Hide();
            }
        }

        private async void txtPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return && e.Key == Key.Enter)
            {
                if (ifLoginSuccess())
                {            
                    HomeWindow home = new HomeWindow(this);
                    home.Show();
                    this.Hide();
                }
                else
                {
                    await this.ShowMessageAsync("LOG IN", "Incorrect username/password. Please try again!");
                }
            }
        }

    }
}
