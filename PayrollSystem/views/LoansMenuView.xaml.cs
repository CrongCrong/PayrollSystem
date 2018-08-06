using System.Windows;
using System.Windows.Controls;

namespace PayrollSystem.views
{
    /// <summary>
    /// Interaction logic for LoansMenuView.xaml
    /// </summary>
    public partial class LoansMenuView : UserControl
    {
        public LoansMenuView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnPel_Click(object sender, RoutedEventArgs e)
        {
            LoansPEL lp = new LoansPEL();
            lp.ShowDialog();
        }

        private void btnGrl_Click(object sender, RoutedEventArgs e)
        {
            LoansGRL gr = new LoansGRL();
            gr.ShowDialog();
        }

        private void btnEml_Click(object sender, RoutedEventArgs e)
        {
            LoansEML em = new LoansEML();
            em.ShowDialog();
        }

        private void btnPey_Click(object sender, RoutedEventArgs e)
        {
            LoansPEY pe = new LoansPEY();
            pe.ShowDialog();
        }
    }
}
