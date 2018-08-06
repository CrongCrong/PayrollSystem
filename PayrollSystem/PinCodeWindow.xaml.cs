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
    /// Interaction logic for PinCodeWindow.xaml
    /// </summary>
    public partial class PinCodeWindow : MetroWindow
    {
        public PinCodeWindow()
        {
            InitializeComponent();
        }

        PayrollDetailsWindow payrollDetails;

        public PinCodeWindow(PayrollDetailsWindow pdw)
        {
            payrollDetails = pdw;
            InitializeComponent();
        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

            

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (UserModel.PIN.Equals(passwordBox.Password))
            {
                if (payrollDetails != null)
                {
                    payrollDetails.txtWage.Visibility = Visibility.Visible;
                    payrollDetails.hideWageBox.Visibility = Visibility.Hidden;
                    payrollDetails.btnHidePassword.Visibility = Visibility.Visible;
                    payrollDetails.btnShowPassword.Visibility = Visibility.Hidden;
                    this.Close();
                }
            }
        }
    }
}
