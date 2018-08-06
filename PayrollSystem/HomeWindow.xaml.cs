using MahApps.Metro.Controls;
using PayrollSystem.classes;
using System.Windows;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : MetroWindow
    {
        public HomeWindow()
        {
            InitializeComponent();
        }

        MainWindow main;

        public HomeWindow(MainWindow mw)
        {
            main = mw;
            InitializeComponent();
        }

        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            this.HamburgerControl.Content = e.ClickedItem;
            this.HamburgerControl.IsPaneOpen = false;

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.Show();
            main.txtUsername.Text = "";
            main.txtPassword.Password = "";
            UserModel.Username = "";
            UserModel.Password = "";
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserModel.isViewing && !UserModel.isAdmin)
            {
                menuEmployees.IsEnabled = false;
                menuPayroll.IsEnabled = false;
                menuIS.IsEnabled = false;
                menuISAP.IsEnabled = false;
                menuLoans.IsEnabled = false;
                menuSSSLoan.IsEnabled = false;
                menuElecBill.IsEnabled = false;
                menuSettings.IsEnabled = false;
            }
        }
    }
}
