using PayrollSystem.classes;
using System.Windows;
using System.Windows.Controls;

namespace PayrollSystem.views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        DepartmentView _deptView = new DepartmentView();
        EmpStatusView _empStatus = new EmpStatusView();
        UsersView _users = new UsersView();

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            content.Content = _deptView;
        }

        private void content_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserModel.isAdmin)
            {
                menuCompany.IsEnabled = false;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            content.Content = _empStatus;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            content.Content = _users;
        }
    }
}
