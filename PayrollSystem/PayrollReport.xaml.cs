using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for PayrollReport.xaml
    /// </summary>
    public partial class PayrollReport : MetroWindow
    {
        public PayrollReport()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        ParametersReportModel paramReport = new ParametersReportModel();


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            cmbSearchCompany.IsEnabled = false;
            cmbSearchEmployee.IsEnabled = false;
            cmbMonths.IsEnabled = false;
            fillMonths();
            loadCompanyList();
            loadEmployeesList();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            CompanyModel cm = cmbSearchCompany.SelectedItem as CompanyModel;

            string cmpName = (cm != null) ? cm.CompanyName : "SPECTRUM GROUP OF COMPANIES";

            string dtePeriod = (cmbMonths.SelectedItem != null) ? cmbMonths.SelectedValue.ToString() + " " + DateTime.Now.Year : "NO MONTHS SELECTED";
            ReportForm rf = new ReportForm(getReport(), cmpName, dtePeriod, paramReport);
            rf.ShowDialog();
        }

        private List<PayrollModel> getReport()
        {
            PayrollModel pm = new PayrollModel();
            List<PayrollModel> lstPayrollReport = new List<PayrollModel>();
            double dblTGross = 0;

            conDB = new ConnectionDB();
            queryString = "SELECT tblpayroll.ID, empID, incomeperday, regworkingdays, tblpayroll.startdate, enddate, sum(sss) as tsss," +
                "sum(philhealth) as tphil, sum(pagibig) as tpagibig, sum(sssloan) as tsssloan," +
                " sum(isap) as tisap, sum(isavings) tis, sum(pey) as tpey, sum(pel) as tpel," +
                " sum(grl) as tgrl, sum(eml) as teml, sum(electricbill) as telecbill, " +
                "sum(cashadvance) as tca, sum(absent) as tabsent, sum(lates) as tlates, " +
                "sum(undertime) as tundertime, others, remarks, firstname, lastname, " +
                "sum(netpay) as tnetpay, workdays, sum(trips) as ttrips, othours, " +
                "sum(ottotal) as tottotal, sum(allowance) as tallowance, sum(commission) as tcommission, " +
                "sum(particularothers) as tparticularothers, sum(deductionothers) as tdeductionothers " +
                "FROM(tblpayroll INNER JOIN tblemployees ON tblpayroll.empID = tblemployees.ID) " +
                "WHERE tblpayroll.isDeleted = 0 AND tblemployees.isDeleted = 0";

            parameters = new List<string>();

            if (chkByDate.IsChecked.Value && (!string.IsNullOrEmpty(searchDateFrom.Text) &&
                !string.IsNullOrEmpty(searchDateTo.Text)))
            {
                queryString += " AND (tblpayroll.startdate BETWEEN ? AND ? AND enddate BETWEEN ? AND ?)";
                DateTime sdate = DateTime.Parse(searchDateFrom.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
                sdate = DateTime.Parse(searchDateTo.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);

                sdate = DateTime.Parse(searchDateFrom.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
                sdate = DateTime.Parse(searchDateTo.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            }
            if (chkByCompany.IsChecked.Value && cmbSearchCompany.SelectedItem != null)
            {
                queryString += " AND (tblemployees.companyID = ?)";
                parameters.Add(cmbSearchCompany.SelectedValue.ToString());
            }
            if (chkByEmployee.IsChecked.Value && cmbSearchEmployee.SelectedItem != null)
            {
                queryString += " AND (empID =?)";
                parameters.Add(cmbSearchEmployee.SelectedValue.ToString());
            }

            queryString += " GROUP BY tblpayroll.empID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                string firstName = reader["firstname"].ToString();
                string lastName = reader["lastname"].ToString();
                pm.FullName = lastName + ", " + firstName;
                double dblworkdays = (!string.IsNullOrEmpty(reader["regworkingdays"].ToString())) ? Convert.ToDouble(reader["regworkingdays"].ToString()) : 0;
                double dblWge = Convert.ToDouble(reader["incomeperday"].ToString());
                pm.WithholdingTax = "4";
                pm.SSS = reader["tsss"].ToString();
                pm.BasicPay = (dblworkdays * dblWge).ToString("N0");
                pm.PhilHealth = reader["tphil"].ToString();
                pm.PagIbig = reader["tpagibig"].ToString();
                pm.SSSLoan = reader["tsssloan"].ToString();
                pm.ISAP = reader["tisap"].ToString();
                pm.IS = reader["tis"].ToString();
                pm.PEY = reader["tpey"].ToString();
                pm.PEL = reader["tpel"].ToString();
                pm.GRL = reader["tgrl"].ToString();
                pm.EML = reader["teml"].ToString();
                pm.ElecBill = reader["telecbill"].ToString();
                pm.CashAdvance = reader["tca"].ToString();
                pm.Absent = reader["tabsent"].ToString();
                pm.Lates = reader["tlates"].ToString();
                pm.Undertime = reader["tundertime"].ToString();
                pm.NetPay = reader["tnetpay"].ToString();
                pm.Trips = reader["ttrips"].ToString();
                pm.OTTotal = reader["tottotal"].ToString();
                pm.Allowance = reader["tallowance"].ToString();
                pm.Commission = reader["tcommission"].ToString();
                pm.ParticularOthers = reader["tparticularothers"].ToString();
                pm.DeductionOthers = reader["tdeductionothers"].ToString();

                double x = ((dblworkdays * dblWge) * 4);

                //paramReport.TotalGross += paramReport.TotalGross + x;
                
                paramReport.TotalSalary += x;
                paramReport.TotalOT += Convert.ToDouble(pm.OTTotal);
                paramReport.TotalAllowance += Convert.ToDouble(pm.Allowance);
                paramReport.TotalCommission += Convert.ToDouble(pm.Commission);
                paramReport.TotalAbsent += Convert.ToDouble(pm.Absent);
                paramReport.TotalLates += Convert.ToDouble(pm.Lates);
                paramReport.TotalUndertime += Convert.ToDouble(pm.Undertime);
                paramReport.TotalCashAdvance += Convert.ToDouble(pm.CashAdvance);
                paramReport.TotalGRL += Convert.ToDouble(pm.GRL);
                paramReport.TotalPEL += Convert.ToDouble(pm.PEL);
                paramReport.TotalEML += Convert.ToDouble(pm.EML);
                paramReport.TotalPEY += Convert.ToDouble(pm.PEY);
                paramReport.TotalISAP += Convert.ToDouble(pm.ISAP);
                paramReport.TotalIS += Convert.ToDouble(pm.IS);
                paramReport.TotalElectBill += Convert.ToDouble(pm.ElecBill);
                paramReport.TotalSSSLoan += Convert.ToDouble(pm.SSSLoan);
                paramReport.TotalDeductionOthers += Convert.ToDouble(pm.DeductionOthers);
                paramReport.TotalParticularOthers += Convert.ToDouble(pm.ParticularOthers);
                paramReport.TotalSSS += Convert.ToDouble(pm.SSS);
                paramReport.TotalPagibig += Convert.ToDouble(pm.PagIbig);
                paramReport.TotalPhilHealth += Convert.ToDouble(pm.PhilHealth);

                pm.Wage = (x + Convert.ToDouble(pm.OTTotal) + Convert.ToDouble(pm.Allowance)
                    + Convert.ToDouble(pm.Commission) + Convert.ToDouble(pm.ParticularOthers)).ToString("N0");

                dblTGross = dblTGross + Convert.ToDouble(pm.Wage);
                paramReport.TotalGross = dblTGross;

                pm.TotalDeductions = (Convert.ToDouble(pm.Absent) + Convert.ToDouble(pm.Lates) + Convert.ToDouble(pm.Undertime) +
                    Convert.ToDouble(pm.CashAdvance) + Convert.ToDouble(pm.GRL) + Convert.ToDouble(pm.PEL) + Convert.ToDouble(pm.EML) +
                    Convert.ToDouble(pm.PEY) + Convert.ToDouble(pm.ISAP) + Convert.ToDouble(pm.IS) + Convert.ToDouble(pm.ElecBill) +
                    Convert.ToDouble(pm.SSSLoan) + Convert.ToDouble(pm.DeductionOthers)).ToString("N0");

                pm.NetPay = (Convert.ToDouble(pm.Wage) + Convert.ToDouble(pm.TotalDeductions)).ToString("N0");

                paramReport.TotalDeductions += Convert.ToDouble(pm.TotalDeductions);
                lstPayrollReport.Add(pm);
                pm = new PayrollModel();
            }
            paramReport.TotalNetDeductions  = paramReport.TotalDeductions + paramReport.TotalSSS +
                paramReport.TotalPhilHealth + paramReport.TotalPagibig;

            paramReport.TotalPayroll = paramReport.TotalGross - paramReport.TotalNetDeductions;
            return lstPayrollReport;
        }

        private void fillMonths()
        {
            foreach (string s in CultureInfo.InvariantCulture.DateTimeFormat.MonthNames)
            {
                cmbMonths.Items.Add(s);
            }
        }

        private void loadEmployeesList()
        {
            conDB = new ConnectionDB();
            List<EmployeeModel> lstEmployees = new List<EmployeeModel>();
            EmployeeModel employee = new EmployeeModel();
            cmbSearchEmployee.Items.Clear();

            queryString = "SELECT tblemployees.ID, employeeID, firstname, lastname, status, incomeperday, companyID, description FROM " +
                "(tblemployees INNER JOIN tblcompany ON tblemployees.companyID = tblcompany.ID) " +
                "WHERE tblemployees.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                employee.ID = reader["ID"].ToString();
                employee.EmployeeID = reader["employeeID"].ToString();
                employee.FirstName = reader["firstname"].ToString();
                employee.LastName = reader["lastname"].ToString();
                employee.Status = reader["status"].ToString();
                employee.Wage = reader["incomeperday"].ToString();
                employee.Company = reader["description"].ToString();
                employee.CompanyID = reader["companyID"].ToString();
                employee.FullName = employee.LastName + ", " + employee.FirstName;
                cmbSearchEmployee.Items.Add(employee);
                employee = new EmployeeModel();
            }
            conDB.closeConnection();

        }

        private void loadCompanyList()
        {
            conDB = new ConnectionDB();
            cmbSearchCompany.Items.Clear();
            queryString = "SELECT ID, companyname, description FROM tblcompany WHERE isDeleted = 0";
            CompanyModel company = new CompanyModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                company.ID = reader["ID"].ToString();
                company.CompanyName = reader["companyname"].ToString();
                company.Description = reader["description"].ToString();
                cmbSearchCompany.Items.Add(company);
                company = new CompanyModel();
            }
            conDB.closeConnection();
        }

        private void chkByDate_Checked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = true;
            searchDateTo.IsEnabled = true;
            cmbMonths.IsEnabled = true;
        }

        private void chkByDate_Unchecked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            cmbMonths.IsEnabled = false;
        }

        private void chkByCompany_Checked(object sender, RoutedEventArgs e)
        {
            cmbSearchCompany.IsEnabled = true;
        }

        private void chkByCompany_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbSearchCompany.IsEnabled = false;
            cmbSearchCompany.SelectedItem = null;
        }

        private void chkByEmployee_Checked(object sender, RoutedEventArgs e)
        {
            cmbSearchEmployee.IsEnabled = true;
        }

        private void chkByEmployee_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbSearchEmployee.IsEnabled = false;
            cmbSearchEmployee.SelectedItem = null;
        }
    }
}
