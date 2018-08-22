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
        List<PayrollModel> lstPayrollModelReport;
        string strCompanyName = "";
        string strDatePeriod = "";

        public ReportForm(List<PayrollModel> pm)
        {
            lstpayrollModel = pm;
            InitializeComponent();
        }

        public ReportForm(List<PayrollModel> pm_report, string strCmp, string strDtePeriod)
        {
            lstPayrollModelReport = pm_report;
            strCompanyName = strCmp;
            strDatePeriod = strDtePeriod;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

            if (lstpayrollModel != null)
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
            if (lstPayrollModelReport != null)
            {
                reportViewer.Reset();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                LocalReport localReport = reportViewer.LocalReport;
                List<ReportParameter> paramList = new List<ReportParameter>();


                ReportDataSource rds;
                ReportParameter param = new ReportParameter("CompanyName");
                param.Values.Add(strCompanyName);
                paramList.Add(param);

                param = new ReportParameter("DatePeriod");
                param.Values.Add(strDatePeriod);
                paramList.Add(param);

                localReport.ReportPath = "reports/MonthlyReport.rdlc";
                rds = new ReportDataSource("DataSet1", lstPayrollModelReport);
                this.reportViewer.LocalReport.SetParameters(paramList.ToArray());


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
}
