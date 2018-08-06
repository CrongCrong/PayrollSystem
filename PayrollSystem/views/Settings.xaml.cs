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
    }
}
