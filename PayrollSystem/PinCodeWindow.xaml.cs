using MahApps.Metro.Controls;
using PayrollSystem.classes;
using PayrollSystem.views;
using System.Windows;

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
        EmployeesViews employeeViews;

        public PinCodeWindow(PayrollDetailsWindow pdw)
        {
            payrollDetails = pdw;
            InitializeComponent();
        }

        public PinCodeWindow(EmployeesViews ev)
        {
            employeeViews = ev;
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

                if(employeeViews != null)
                {
                    employeeViews.txtWage.Visibility = Visibility.Visible;
                    employeeViews.hideWageBox.Visibility = Visibility.Hidden;
                    employeeViews.btnHidePassword.Visibility = Visibility.Visible;
                    employeeViews.btnShowPassword.Visibility = Visibility.Hidden;
                    this.Close();
                }
            }

        }
    }
}
