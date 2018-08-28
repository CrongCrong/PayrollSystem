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
        ParametersReportModel paramReports;

        public ReportForm(List<PayrollModel> pm)
        {
            lstpayrollModel = pm;
            InitializeComponent();
        }

        public ReportForm(List<PayrollModel> pm_report, string strCmp, string strDtePeriod, ParametersReportModel prm)
        {
            lstPayrollModelReport = pm_report;
            strCompanyName = strCmp;
            strDatePeriod = strDtePeriod;
            paramReports = prm;
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

                param = new ReportParameter("TotalOT");
                param.Values.Add(paramReports.TotalOT.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalAllowance");
                param.Values.Add(paramReports.TotalAllowance.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalCommission");
                param.Values.Add(paramReports.TotalCommission.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalGross");
                param.Values.Add(paramReports.TotalGross.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalAbsent");
                param.Values.Add(paramReports.TotalAbsent.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalLates");
                param.Values.Add(paramReports.TotalLates.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalUndertime");
                param.Values.Add(paramReports.TotalUndertime.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalCashAdvance");
                param.Values.Add(paramReports.TotalCashAdvance.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalGRL");
                param.Values.Add(paramReports.TotalGRL.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalPEL");
                param.Values.Add(paramReports.TotalPEL.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalEML");
                param.Values.Add(paramReports.TotalEML.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalPEY");
                param.Values.Add(paramReports.TotalPEY.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalISAP");
                param.Values.Add(paramReports.TotalISAP.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalIS");
                param.Values.Add(paramReports.TotalIS.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalElectBill");
                param.Values.Add(paramReports.TotalElectBill.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalSSSLoan");
                param.Values.Add(paramReports.TotalSSSLoan.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalDeductionOthers");
                param.Values.Add(paramReports.TotalDeductionOthers.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalSalary");
                param.Values.Add(paramReports.TotalSalary.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalParticularOthers");
                param.Values.Add(paramReports.TotalParticularOthers.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalSSS");
                param.Values.Add(paramReports.TotalSSS.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalPagibig");
                param.Values.Add(paramReports.TotalPagibig.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalPhilHealth");
                param.Values.Add(paramReports.TotalPhilHealth.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalDeductions");
                param.Values.Add(paramReports.TotalDeductions.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalPayroll");
                param.Values.Add(paramReports.TotalPayroll.ToString("N0"));
                paramList.Add(param);

                param = new ReportParameter("TotalNetDeductions");
                param.Values.Add(paramReports.TotalNetDeductions.ToString("N0"));
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
