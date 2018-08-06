using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

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
                menuSettings.IsEnabled = false;
            }
        }
    }
}
