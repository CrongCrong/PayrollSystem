using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using PayrollSystem.classes;
using System.Collections.Generic;
using System.Windows;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for ReportForm.xaml
    /// </summary>
    public partial class ReportForm : MetroWindow
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        List<PayrollModel> lstpayrollModel;

        public ReportForm(List<PayrollModel> pm)
        {
            lstpayrollModel = pm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            reportViewer.Reset();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer.LocalReport;


            ReportDataSource rds;

            localReport.ReportPath = "reports/PaySlipRev.rdlc";
            rds = new ReportDataSource("DataSet1", lstpayrollModel);


            //System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
            //ps.Landscape = true;
            //ps.PaperSize = new System.Drawing.Printing.PaperSize("Letter", 850, 1170);
            ////ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
            //ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.Letter;
            //reportViewer.SetPageSettings(ps);

            reportViewer.LocalReport.DataSources.Add(rds);

            reportViewer.RefreshReport();
        }
    }
}
