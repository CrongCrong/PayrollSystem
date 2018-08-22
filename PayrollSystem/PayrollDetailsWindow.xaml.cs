using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PayrollSystem.classes;
using PayrollSystem.views;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PayrollSystem
{
    /// <summary>
    /// Interaction logic for PayrollDetailsWindow.xaml
    /// </summary>
    public partial class PayrollDetailsWindow : MetroWindow
    {
        public PayrollDetailsWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        string recordID = "";
        PayrollView payRollView;
        PayrollModel payrollModel;
        List<PayrollModel> LstPayrollModelPayslip;
        EmployeeModel selectedEmployee;

        public PayrollDetailsWindow(PayrollView pv)
        {
            payRollView = pv;
            InitializeComponent();
        }

        public PayrollDetailsWindow(PayrollView pv, PayrollModel pm)
        {
            payrollModel = pm;
            payRollView = pv;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnHidePassword.Visibility = Visibility.Hidden;
            txtWage.Visibility = Visibility.Hidden;

            loadEmployeesList();
            EnableDisableValueControls(false);
            if (payrollModel != null)
            {
                foreach (EmployeeModel ee in cmbEmployees.Items)
                {
                    if (ee.ID.Equals(payrollModel.EmployeeID))
                    {
                        cmbEmployees.SelectedItem = ee;
                    }
                }
                recordID = payrollModel.ID;
                //DateTime dte = DateTime.Parse(payrollModel.StartDate);
                //startDate.Text = dte.ToShortDateString();

                //dte = DateTime.Parse(payrollModel.EndDate);
                //endDate.Text = dte.ToShortDateString();
                //txtWage.Text = payrollModel.
                startDate.Text = payrollModel.StartDate;
                endDate.Text = payrollModel.EndDate;
                txtWorkdays.Text = payrollModel.WorkDays;
                txtSSS.Text = payrollModel.SSS;
                txtphilhealth.Text = payrollModel.PhilHealth;
                txtpagibig.Text = payrollModel.PagIbig;
                txtsssloan.Text = payrollModel.SSSLoan;
                txtisap.Text = payrollModel.ISAP;
                txtis.Text = payrollModel.IS;
                txtca.Text = payrollModel.CashAdvance;
                txtOthers.Text = payrollModel.Others;
                txtpel.Text = payrollModel.PEL;
                txteml.Text = payrollModel.EML;
                txtgrl.Text = payrollModel.GRL;
                txtpey.Text = payrollModel.PEY;
                txtelecbill.Text = payrollModel.ElecBill;
                txtabsent.Text = payrollModel.Absent;
                txtLates.Text = payrollModel.Lates;
                txtUndertime.Text = payrollModel.Undertime;
                txtTrips.Text = payrollModel.Trips;
                txtOvertimehours.Text = payrollModel.OTHours;
                txtOvertimetotal.Text = payrollModel.OTTotal;
                txtAllowance.Text = payrollModel.Allowance;
                txtCommission.Text = payrollModel.Commission;
                txtNotes.Text = payrollModel.Remarks;
                txtothersdeduct1.Text = payrollModel.DeductionOthers;
                txtnotesdeduct1.Text = payrollModel.ParticularOthers;
                //txtOvertimehours.Text = payrollModel.OTHours;
                //txtOvertimetotal.Text = payrollModel.OTTotal;
                lblNetPay.Content = payrollModel.NetPay;

                lblTotalPEL.Content = (!string.IsNullOrEmpty(payrollModel.TotalPEL) ? 
                    Convert.ToDouble(payrollModel.TotalPEL).ToString("N0") : "0");
                lblTotalEML.Content = (!string.IsNullOrEmpty(payrollModel.TotalEML) ?
                    Convert.ToDouble(payrollModel.TotalEML).ToString("N0") : "0");
                lblTotalGRL.Content = (!string.IsNullOrEmpty(payrollModel.TotalGRL) ?
                    Convert.ToDouble(payrollModel.TotalGRL).ToString("N0") : "0");
                lblTotalPEY.Content = (!string.IsNullOrEmpty(payrollModel.TotalPEY) ?
                    Convert.ToDouble(payrollModel.TotalPEY).ToString("N0") : "0");
                lblTotalISAP.Content = (!string.IsNullOrEmpty(payrollModel.TotalISAP) ?
                    Convert.ToDouble(payrollModel.TotalISAP).ToString("N0") : "0");
                lblTotalIS.Content = (!string.IsNullOrEmpty(payrollModel.TotalIS) ?
                    Convert.ToDouble(payrollModel.TotalIS).ToString("N0") : "0");
                lblTotalElectBill.Content = (!string.IsNullOrEmpty(payrollModel.TotalElectBill) ?
                    Convert.ToDouble(payrollModel.TotalElectBill).ToString("N0") : "0");
                lblTotalSSSLoan.Content = (!string.IsNullOrEmpty(payrollModel.TotalSSSLoan) ?
                    Convert.ToDouble(payrollModel.TotalSSSLoan).ToString("N0") : "0");

                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
                fillModelForReport();
            }
        }

        private void cmbEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedEmployee = cmbEmployees.SelectedItem as EmployeeModel;
            if (selectedEmployee != null)
            {
                clearFields();
                recordID = "";
                txtWage.Text = selectedEmployee.Wage;
                hideWageBox.Password = selectedEmployee.Wage;

                txtRatehr.Text = (Convert.ToDouble(selectedEmployee.Wage) / 8).ToString();
                txtSSS.Text = selectedEmployee.SSS;
                txtphilhealth.Text = selectedEmployee.PhilHealth;
                txtpagibig.Text = selectedEmployee.Pagibig;
                txtsssloan.Text = selectedEmployee.SSSLoan;
                txtpel.Text = selectedEmployee.PEL;
                txteml.Text = selectedEmployee.EML;
                txtgrl.Text = selectedEmployee.GRL;
                txtpey.Text = selectedEmployee.PEY;
                txtelecbill.Text = selectedEmployee.ElecBill;
                txtAllowance.Text = selectedEmployee.Allowance;
                txtWorkdays.Text = selectedEmployee.RegWorkingDays;
                EnableDisableValueControls(true);
                btnUpdate.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
            }
        }

        private List<PayrollModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<PayrollModel> lstPayroll = new List<PayrollModel>();
            PayrollModel payroll = new PayrollModel();

            queryString = "SELECT tblpayroll.ID, empID, tblpayroll.startdate, enddate, sss, philhealth, pagibig, " +
                "sssloan, isap, isavings, pey, pel, grl, eml, electricbill, cashadvance, " +
                "absent, lates, undertime, others, remarks, firstname, lastname, netpay, workdays, trips, othours, ottotal, " +
                "allowance, commission, particularothers, deductionothers, totalpel, totaleml, totalgrl, totalpey, " +
                "totalelectbill, totalsssloan, totalis, totalisap FROM (tblpayroll INNER JOIN " +
                "tblemployees ON tblpayroll.empID = tblemployees.ID) WHERE tblpayroll.isDeleted = 0 " +
                "AND tblemployees.isDeleted = 0 ORDER BY startdate DESC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                payroll.ID = reader["ID"].ToString();
                payroll.EmployeeID = reader["empID"].ToString();

                DateTime dte = DateTime.Parse(reader["startdate"].ToString());
                payroll.StartDate = dte.ToShortDateString();

                dte = DateTime.Parse(reader["enddate"].ToString());
                payroll.EndDate = dte.ToShortDateString();

                payroll.SSS = reader["sss"].ToString();
                payroll.PhilHealth = reader["philhealth"].ToString();
                payroll.PagIbig = reader["pagibig"].ToString();
                payroll.SSSLoan = reader["sssloan"].ToString();
                payroll.ISAP = reader["isap"].ToString();
                payroll.IS = reader["isavings"].ToString();
                payroll.PEY = reader["pey"].ToString();
                payroll.PEL = reader["pel"].ToString();
                payroll.GRL = reader["grl"].ToString();
                payroll.EML = reader["eml"].ToString();
                payroll.ElecBill = reader["electricbill"].ToString();
                payroll.CashAdvance = reader["cashadvance"].ToString();
                payroll.Absent = reader["absent"].ToString();
                payroll.Lates = reader["lates"].ToString();
                payroll.Undertime = reader["undertime"].ToString();
                payroll.Others = reader["others"].ToString();
                payroll.Remarks = reader["remarks"].ToString();
                payroll.NetPay = reader["netpay"].ToString();
                string firstName = reader["firstname"].ToString();
                string lastName = reader["lastname"].ToString();
                payroll.FullName = lastName + ", " + firstName;
                payroll.WorkDays = reader["workdays"].ToString();
                payroll.Trips = reader["trips"].ToString();
                payroll.OTHours = reader["othours"].ToString();
                payroll.OTTotal = reader["ottotal"].ToString();
                payroll.Allowance = reader["allowance"].ToString();
                payroll.Commission = reader["commission"].ToString();
                payroll.ParticularOthers = reader["particularothers"].ToString();
                payroll.DeductionOthers = reader["deductionothers"].ToString();
                payroll.TotalPEL = reader["totalpel"].ToString();
                payroll.TotalEML = reader["totaleml"].ToString();
                payroll.TotalGRL = reader["totalgrl"].ToString();
                payroll.TotalPEY = reader["totalpey"].ToString();
                payroll.TotalElectBill = reader["totalelectbill"].ToString();
                payroll.TotalSSSLoan = reader["totalsssloan"].ToString();
                payroll.TotalIS = reader["totalis"].ToString();
                payroll.TotalISAP = reader["totalisap"].ToString();
                lstPayroll.Add(payroll);
                payroll = new PayrollModel();

            }
            conDB.closeConnection();
            return lstPayroll;
        }

        private void fillModelForReport()
        {
            LstPayrollModelPayslip = new List<PayrollModel>();
            PayrollModel payrollModelPayslip = new PayrollModel();
            EmployeeModel ee = cmbEmployees.SelectedItem as EmployeeModel;

            payrollModelPayslip.StartDate = startDate.Text + " - " + endDate.Text;
            payrollModelPayslip.FullName = ee.FullName;
            payrollModelPayslip.BasicPay = Convert.ToDouble(txtWage.Text).ToString("N0");
            payrollModelPayslip.WorkDays = txtWorkdays.Text;
            payrollModelPayslip.OTTotal = Convert.ToDouble(txtOvertimetotal.Text).ToString("N0");
            payrollModelPayslip.Allowance = Convert.ToDouble(txtAllowance.Text).ToString("N0");
            payrollModelPayslip.Commission = Convert.ToDouble(txtCommission.Text).ToString("N0");
            payrollModelPayslip.ParticularOthers = Convert.ToDouble(txtnotesdeduct1.Text).ToString("N0");
            payrollModelPayslip.Remarks = txtNotes.Text;
            payrollModelPayslip.CashAdvance = Convert.ToDouble(txtca.Text).ToString("N0");
            payrollModelPayslip.PEL = Convert.ToDouble(txtpel.Text).ToString("N0");
            payrollModelPayslip.EML = Convert.ToDouble(txteml.Text).ToString("N0");
            payrollModelPayslip.GRL = Convert.ToDouble(txtgrl.Text).ToString("N0");
            payrollModelPayslip.PEY = Convert.ToDouble(txtpey.Text).ToString("N0");
            payrollModelPayslip.ElecBill = Convert.ToDouble(txtelecbill.Text).ToString("N0");
            payrollModelPayslip.Absent = Convert.ToDouble(txtabsent.Text).ToString("N0");
            payrollModelPayslip.Lates = Convert.ToDouble(txtLates.Text).ToString("N0");
            payrollModelPayslip.Undertime = Convert.ToDouble(txtUndertime.Text).ToString("N0");
            payrollModelPayslip.Trips = Convert.ToDouble(txtTrips.Text).ToString("N0");
            payrollModelPayslip.OTHours = txtOvertimehours.Text;
            payrollModelPayslip.OTTotal = Convert.ToDouble(txtOvertimetotal.Text).ToString("N0");
            payrollModelPayslip.DeductionOthers = Convert.ToDouble(txtothersdeduct1.Text).ToString("N0");
            payrollModelPayslip.Others = txtOthers.Text;
            payrollModelPayslip.IS = Convert.ToDouble(txtis.Text).ToString("N0");
            payrollModelPayslip.ISAP = Convert.ToDouble(txtisap.Text).ToString("N0");
            payrollModelPayslip.SSS = Convert.ToDouble(txtSSS.Text).ToString("N0");
            payrollModelPayslip.PhilHealth = Convert.ToDouble(txtphilhealth.Text).ToString("N0");
            payrollModelPayslip.NetPay = Convert.ToDouble(lblNetPay.Content.ToString()).ToString("N0");
            payrollModelPayslip.PagIbig = Convert.ToDouble(txtpagibig.Text).ToString("N0");
            payrollModelPayslip.SSSLoan = Convert.ToDouble(txtsssloan.Text).ToString("N0");

            payrollModelPayslip.GrossSalary = ((Convert.ToDouble(payrollModelPayslip.BasicPay) *
                Convert.ToDouble(payrollModelPayslip.WorkDays)) + Convert.ToDouble(payrollModelPayslip.OTTotal)
                + Convert.ToDouble(payrollModelPayslip.Allowance) + Convert.ToDouble(payrollModelPayslip.Commission)
                + Convert.ToDouble(payrollModelPayslip.Trips) + Convert.ToDouble(payrollModelPayslip.ParticularOthers)).ToString("N0");

            double dblAbsent = Convert.ToDouble(payrollModelPayslip.Absent);
            double dblLate = Convert.ToDouble(payrollModelPayslip.Lates);
            double dblUndertime = Convert.ToDouble(payrollModelPayslip.Undertime);
            double dblCA = Convert.ToDouble(payrollModelPayslip.CashAdvance);
            double dblPEL = Convert.ToDouble(payrollModelPayslip.PEL);
            double dblEML = Convert.ToDouble(payrollModelPayslip.EML);
            double dblGRL = Convert.ToDouble(payrollModelPayslip.GRL);
            double dblPEY = Convert.ToDouble(payrollModelPayslip.PEY);
            double dblISAP = Convert.ToDouble(payrollModelPayslip.ISAP);
            double dblIS = Convert.ToDouble(payrollModelPayslip.IS);
            double dblElect = Convert.ToDouble(payrollModelPayslip.ElecBill);
            double dblSSS = Convert.ToDouble(payrollModelPayslip.SSS);
            double dblSSSLoan = Convert.ToDouble(payrollModelPayslip.SSSLoan);
            double dblPagibig = Convert.ToDouble(payrollModelPayslip.PagIbig);
            double dblPhilhealth = Convert.ToDouble(payrollModelPayslip.PhilHealth);
            double dblOthersDeduction = Convert.ToDouble(payrollModelPayslip.DeductionOthers);

            payrollModelPayslip.TotalDeductions = (dblAbsent + dblLate + dblUndertime + dblCA + dblPEL + dblEML +
                dblGRL + dblPEY + dblIS + dblISAP + dblElect + dblSSS + dblSSSLoan + dblPagibig + dblPhilhealth +
                dblOthersDeduction).ToString("N0");

            payrollModelPayslip.TotalPEL = (!string.IsNullOrEmpty(lblTotalPEL.Content.ToString())) ? lblTotalPEL.Content.ToString() : "0";
            payrollModelPayslip.TotalEML = (!string.IsNullOrEmpty(lblTotalEML.Content.ToString())) ? lblTotalEML.Content.ToString() : "0";
            payrollModelPayslip.TotalGRL = (!string.IsNullOrEmpty(lblTotalGRL.Content.ToString())) ? lblTotalGRL.Content.ToString() : "0";
            payrollModelPayslip.TotalPEY = (!string.IsNullOrEmpty(lblTotalPEY.Content.ToString())) ? lblTotalPEY.Content.ToString() : "0";
            payrollModelPayslip.TotalElectBill = (!string.IsNullOrEmpty(lblTotalElectBill.Content.ToString())) ? lblTotalElectBill.Content.ToString() : "0";
            payrollModelPayslip.TotalSSSLoan = (!string.IsNullOrEmpty(lblTotalSSSLoan.Content.ToString())) ? lblTotalSSSLoan.Content.ToString() : "0";
            payrollModelPayslip.TotalIS = (!string.IsNullOrEmpty(lblTotalIS.Content.ToString())) ? lblTotalIS.Content.ToString() : "0";
            payrollModelPayslip.TotalISAP = (!string.IsNullOrEmpty(lblTotalISAP.Content.ToString())) ? lblTotalISAP.Content.ToString() : "0";
            LstPayrollModelPayslip.Add(payrollModelPayslip);

            payrollModelPayslip = new PayrollModel();
        }

        private void loadEmployeesList()
        {
            conDB = new ConnectionDB();
            List<EmployeeModel> lstEmployees = new List<EmployeeModel>();
            EmployeeModel employee = new EmployeeModel();
            cmbEmployees.Items.Clear();

            queryString = "SELECT tblemployees.ID, employeeID, firstname, lastname, status, incomeperday, " +
                "companyID, empsss, empphilhealth, emppagibig, empsssloan, emppel, empeml, empgrl, emppey," +
                " empelecbill, empallowance, description, regworkingdays FROM " +
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
                employee.SSS = reader["empsss"].ToString();
                employee.PhilHealth = reader["empphilhealth"].ToString();
                employee.Pagibig = reader["emppagibig"].ToString();
                employee.SSSLoan = reader["empsssloan"].ToString();
                employee.PEL = reader["emppel"].ToString();
                employee.EML = reader["empeml"].ToString();
                employee.GRL = reader["empgrl"].ToString();
                employee.PEY = reader["emppey"].ToString();
                employee.ElecBill = reader["empelecbill"].ToString();
                employee.Allowance = reader["empallowance"].ToString();
                employee.RegWorkingDays = reader["regworkingdays"].ToString();
                cmbEmployees.Items.Add(employee);
                employee = new EmployeeModel();
            }
            conDB.closeConnection();

        }

        private void savePayrollRecord()
        {
            string eeIDD = cmbEmployees.SelectedValue.ToString();
            //PEL
            double dblTempPEL = getEmployeeLoanPEL(eeIDD);
            double dblTempPEL2 = getToBeSubtractedPEL(eeIDD);
            //EML
            double dblTempEML = getEmployeeLoanEML(eeIDD);
            double dblTempEML2 = getToBeSubtractedEML(eeIDD);
            //GRL
            double dblTempGRL = getEmployeeLoanGRL(eeIDD);
            double dbltempGRL2 = getToBeSubtractedGRL(eeIDD);
            //PEY
            double dblTempPEY = getEmployeeLoanPEY(eeIDD);
            double dblTempPEY2 = getToBeSubtractedPEY(eeIDD);
            //ELECT BILL
            double dblTempElecBill = getEmployeeLoanElecBill(eeIDD);
            double dblTempElecBill2 = getToBeSubtractedElecBill(eeIDD);
            //SSS LOAN
            double dblTempSSSLoan = getEmployeeLoanSSSLoan(eeIDD);
            double dblTempSSSLoan2 = getToBeSubtractedSSSLoan(eeIDD);
            //IS
            double dblTempIS = getEmployeeTotalIS(eeIDD);
            double dblTempIS2 = getToBeAddedTotalIS(eeIDD);
            //ISAP
            double dblTempISAP = getEmployeeTotalISAP(eeIDD);
            double dblTempISAP2 = getToBeAddedTotalISAP(eeIDD);

            conDB = new ConnectionDB();

            queryString = "INSERT INTO tblpayroll (empID, startdate, enddate, sss, philhealth, pagibig, " +
                " sssloan, isap, isavings, pey, pel, grl, eml, electricbill, cashadvance, absent, " +
                "lates, undertime, others, remarks, netpay, ratehr, workdays, trips, othours, ottotal, " +
                "allowance, commission, particularothers, deductionothers, totalpel, totaleml, totalgrl, " +
                "totalpey, totalelectbill, totalsssloan, totalis, totalisap, isDeleted) VALUES " +
                "(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,0)";

            List<string> paramsave = new List<string>();

            paramsave.Add(cmbEmployees.SelectedValue.ToString());
            DateTime sdate = DateTime.Parse(startDate.Text);
            paramsave.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            sdate = DateTime.Parse(endDate.Text);
            paramsave.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            paramsave.Add(txtSSS.Text);
            paramsave.Add(txtphilhealth.Text);
            paramsave.Add(txtpagibig.Text);
            //parameters.Add(txtwithtax.Text);
            paramsave.Add(txtsssloan.Text);
            paramsave.Add(txtisap.Text);
            paramsave.Add(txtis.Text);
            paramsave.Add(txtpey.Text);
            paramsave.Add(txtpel.Text);
            paramsave.Add(txtgrl.Text);
            paramsave.Add(txteml.Text);
            paramsave.Add(txtelecbill.Text);
            paramsave.Add(txtca.Text);
            paramsave.Add(txtabsent.Text);
            paramsave.Add(txtLates.Text);
            paramsave.Add(txtUndertime.Text);

            paramsave.Add((!string.IsNullOrEmpty(txtOthers.Text)) ? txtOthers.Text : "0");
            paramsave.Add((!string.IsNullOrEmpty(txtNotes.Text)) ? txtNotes.Text : "0");

            paramsave.Add(lblNetPay.Content.ToString());
            paramsave.Add(txtRatehr.Text);
            paramsave.Add(txtWorkdays.Text);
            paramsave.Add(txtTrips.Text);
            paramsave.Add(txtOvertimehours.Text);
            paramsave.Add(txtOvertimetotal.Text);
            paramsave.Add(txtAllowance.Text);
            paramsave.Add(txtCommission.Text);

            paramsave.Add(txtnotesdeduct1.Text);
            paramsave.Add(txtothersdeduct1.Text);


            dblTempPEL = (dblTempPEL - dblTempPEL2) - Convert.ToDouble(txtpel.Text);
            paramsave.Add(dblTempPEL.ToString());
            lblTotalPEL.Content = dblTempPEL.ToString("N0");

            dblTempEML = (dblTempEML - dblTempEML2) - Convert.ToDouble(txteml.Text);
            paramsave.Add(dblTempEML.ToString());
            lblTotalEML.Content = dblTempEML.ToString("N0");

            dblTempGRL = (dblTempGRL - dbltempGRL2) - Convert.ToDouble(txtgrl.Text);
            paramsave.Add(dblTempGRL.ToString());
            lblTotalGRL.Content = dblTempGRL.ToString("N0");

            dblTempPEY = (dblTempPEY - dblTempPEY2) - Convert.ToDouble(txtpey.Text);
            paramsave.Add(dblTempPEY.ToString());
            lblTotalPEY.Content = dblTempPEY.ToString("N0");

            dblTempElecBill = (dblTempElecBill - dblTempElecBill2) - Convert.ToDouble(txtelecbill.Text);
            paramsave.Add(dblTempElecBill.ToString());
            lblTotalElectBill.Content = dblTempElecBill.ToString("N0");

            dblTempSSSLoan = (dblTempSSSLoan - dblTempSSSLoan2) - Convert.ToDouble(txtsssloan.Text);
            paramsave.Add(dblTempSSSLoan.ToString());
            lblTotalSSSLoan.Content = dblTempSSSLoan.ToString("N0");

            dblTempIS = (dblTempIS + dblTempIS2) + Convert.ToDouble(txtis.Text);
            paramsave.Add(dblTempIS.ToString());
            lblTotalIS.Content = dblTempIS.ToString("N0");

            dblTempISAP = (dblTempISAP + dblTempISAP2) + Convert.ToDouble(txtisap.Text);
            paramsave.Add(dblTempISAP.ToString());
            lblTotalISAP.Content = dblTempISAP.ToString("N0");

            conDB.AddRecordToDatabase(queryString, paramsave);
            conDB.closeConnection();

            conDB.writeLogFile("SAVE PAYROLL RECORD: EMPLOYEE ID: " + cmbEmployees.SelectedValue.ToString() + "| DATE FROM: " +
                startDate.Text + "| DATE TO: " + endDate.Text + "NET PAY: " + lblNetPay.Content);

            MySqlDataReader reader = conDB.getSelectConnection("select ID from tblpayroll order by ID desc limit 1", null);

            while (reader.Read())
            {
                recordID = reader["ID"].ToString();
            }

            conDB.closeConnection();
        }

        private void updatePayRollRecord()
        {
            conDB = new ConnectionDB();

            queryString = "UPDATE tblpayroll SET startdate = ?, enddate = ?, sss = ?, philhealth = ?, pagibig = ?, " +
                "sssloan = ?, isap = ?, isavings = ?, pey = ?, pel = ?, grl = ?, eml = ?, electricbill = ?, absent = ?, " +
                "lates = ?, undertime = ?, others = ?, remarks = ?, netpay = ?, ratehr = ?, workdays = ?, trips = ?," +
                "othours = ?, ottotal = ?, allowance = ?, commission = ?, " +
                "cashadvance = ?, particularothers = ?, deductionothers = ?, totalpel = ?, totaleml = ?, " +
                "totalgrl = ?, totalpey = ?, totalelectbill = ?, totalsssloan = ?, totalis = ?, " +
                "totalisap = ? WHERE ID = ?";

            parameters = new List<string>();

            DateTime sdate = DateTime.Parse(startDate.Text);
            parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);

            sdate = DateTime.Parse(endDate.Text);
            parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            parameters.Add(txtSSS.Text);
            parameters.Add(txtphilhealth.Text);
            parameters.Add(txtpagibig.Text);
            //parameters.Add(txtwithtax.Text);
            parameters.Add(txtsssloan.Text);
            parameters.Add(txtisap.Text);
            parameters.Add(txtis.Text);
            parameters.Add(txtpey.Text);
            parameters.Add(txtpel.Text);
            parameters.Add(txtgrl.Text);
            parameters.Add(txteml.Text);
            parameters.Add(txtelecbill.Text);
            parameters.Add(txtabsent.Text);
            parameters.Add(txtLates.Text);
            parameters.Add(txtUndertime.Text);
            parameters.Add(txtOthers.Text);
            parameters.Add(txtNotes.Text);
            parameters.Add(lblNetPay.Content.ToString());
            parameters.Add(txtRatehr.Text);
            parameters.Add(txtWorkdays.Text);
            parameters.Add(txtTrips.Text);
            parameters.Add(txtOvertimehours.Text);
            parameters.Add(txtOvertimetotal.Text);
            parameters.Add(txtAllowance.Text);
            parameters.Add(txtCommission.Text);
            parameters.Add(txtca.Text);
            parameters.Add(txtnotesdeduct1.Text);
            parameters.Add(txtothersdeduct1.Text);

            double updatePEL = 0;
            double updateEML = 0;
            double updateGRL = 0;
            double updatePEY = 0;
            double updateElecBill = 0;
            double updateSSSLoan = 0;
            double updateIS = 0;
            double updateISAP = 0;

            //PEL
            updatePEL = setUpdatedValuesForLoans(Convert.ToDouble(txtpel.Text),
                Convert.ToDouble(payrollModel.PEL), Convert.ToDouble(payrollModel.TotalPEL));       
            parameters.Add(updatePEL.ToString());
            lblTotalPEL.Content = updatePEL.ToString("N0");

            //EML
            updateEML = setUpdatedValuesForLoans(Convert.ToDouble(txteml.Text),
                Convert.ToDouble(payrollModel.EML), Convert.ToDouble(payrollModel.TotalEML));
            parameters.Add(updateEML.ToString());
            lblTotalEML.Content = updateEML.ToString("N0");

            //GRL
            updateGRL = setUpdatedValuesForLoans(Convert.ToDouble(txtgrl.Text),
                Convert.ToDouble(payrollModel.GRL), Convert.ToDouble(payrollModel.TotalGRL));
            parameters.Add(updateGRL.ToString());
            lblTotalGRL.Content = updateGRL.ToString("N0");

            //PEY
            updatePEY = setUpdatedValuesForLoans(Convert.ToDouble(txtpey.Text),
                Convert.ToDouble(payrollModel.PEY), Convert.ToDouble(payrollModel.TotalPEY));
            parameters.Add(updatePEY.ToString());
            lblTotalPEY.Content = updatePEY.ToString("N0");

            //Elec Bill
            updateElecBill = setUpdatedValuesForLoans(Convert.ToDouble(txtelecbill.Text),
                Convert.ToDouble(payrollModel.ElecBill), Convert.ToDouble(payrollModel.TotalElectBill));
            parameters.Add(updateElecBill.ToString());
            lblTotalElectBill.Content = updateElecBill.ToString("N0");

            //SSS Loan
            updateSSSLoan = setUpdatedValuesForLoans(Convert.ToDouble(txtsssloan.Text),
                 Convert.ToDouble(payrollModel.SSSLoan), Convert.ToDouble(payrollModel.TotalSSSLoan));
            parameters.Add(updateSSSLoan.ToString());
            lblTotalSSSLoan.Content = updateSSSLoan.ToString("N0");

            //IS
            updateIS = setUpdatedValuesForIntactSavings(Convert.ToDouble(txtis.Text),
                 Convert.ToDouble(payrollModel.IS), Convert.ToDouble(payrollModel.TotalIS));
            parameters.Add(updateIS.ToString());
            lblTotalIS.Content = updateIS.ToString("N0");

            //ISAP
            updateISAP = setUpdatedValuesForIntactSavings(Convert.ToDouble(txtisap.Text),
                 Convert.ToDouble(payrollModel.ISAP), Convert.ToDouble(payrollModel.TotalISAP));
            parameters.Add(updateISAP.ToString());
            lblTotalISAP.Content = updateISAP.ToString("N0");

            parameters.Add(recordID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private string computeDeductions()
        {
            double x = 0.0;
            double particulars = 0.0;

            txtSSS.Text = (!string.IsNullOrEmpty(txtSSS.Text)) ? txtSSS.Text : "0";
            txtphilhealth.Text = (!string.IsNullOrEmpty(txtphilhealth.Text)) ? txtphilhealth.Text : "0";
            txtpagibig.Text = (!string.IsNullOrEmpty(txtpagibig.Text)) ? txtpagibig.Text : "0";

            txtnotesdeduct1.Text = (!string.IsNullOrEmpty(txtnotesdeduct1.Text)) ? txtnotesdeduct1.Text : "0";
            txtothersdeduct1.Text = (!string.IsNullOrEmpty(txtothersdeduct1.Text)) ? txtothersdeduct1.Text : "0";
            //txtwithtax.Text = (!string.IsNullOrEmpty(txtwithtax.Text)) ? txtwithtax.Text : "0";
            txtsssloan.Text = (!string.IsNullOrEmpty(txtsssloan.Text)) ? txtsssloan.Text : "0";
            txtisap.Text = (!string.IsNullOrEmpty(txtisap.Text)) ? txtisap.Text : "0";
            txtis.Text = (!string.IsNullOrEmpty(txtis.Text)) ? txtis.Text : "0";
            txtpey.Text = (!string.IsNullOrEmpty(txtpey.Text)) ? txtpey.Text : "0";
            txtpel.Text = (!string.IsNullOrEmpty(txtpel.Text)) ? txtpel.Text : "0";
            txtgrl.Text = (!string.IsNullOrEmpty(txtgrl.Text)) ? txtgrl.Text : "0";
            txteml.Text = (!string.IsNullOrEmpty(txteml.Text)) ? txteml.Text : "0";
            txtelecbill.Text = (!string.IsNullOrEmpty(txtelecbill.Text)) ? txtelecbill.Text : "0";
            txtca.Text = (!string.IsNullOrEmpty(txtca.Text)) ? txtca.Text : "0";
            txtabsent.Text = (!string.IsNullOrEmpty(txtabsent.Text)) ? txtabsent.Text : "0";
            txtLates.Text = (!string.IsNullOrEmpty(txtLates.Text)) ? txtLates.Text : "0";
            txtUndertime.Text = (!string.IsNullOrEmpty(txtUndertime.Text)) ? txtUndertime.Text : "0";
            txtTrips.Text = (!string.IsNullOrEmpty(txtTrips.Text)) ? txtTrips.Text : "0";
            txtOvertimehours.Text = (!string.IsNullOrEmpty(txtOvertimehours.Text)) ? txtOvertimehours.Text : "0";
            txtAllowance.Text = (!string.IsNullOrEmpty(txtAllowance.Text)) ? txtAllowance.Text : "0";
            txtCommission.Text = (!string.IsNullOrEmpty(txtCommission.Text)) ? txtCommission.Text : "0";

            x = Convert.ToDouble(txtSSS.Text) + Convert.ToDouble(txtphilhealth.Text) + Convert.ToDouble(txtpagibig.Text)
                + Convert.ToDouble(txtsssloan.Text) + Convert.ToDouble(txtisap.Text)
                + Convert.ToDouble(txtis.Text) + Convert.ToDouble(txtpey.Text) + Convert.ToDouble(txtpel.Text) +
                Convert.ToDouble(txtgrl.Text) + Convert.ToDouble(txteml.Text) + Convert.ToDouble(txtelecbill.Text) +
                Convert.ToDouble(txtca.Text) + Convert.ToDouble(txtabsent.Text) +
                +Convert.ToDouble(txtLates.Text) + Convert.ToDouble(txtUndertime.Text) +
                Convert.ToDouble(txtothersdeduct1.Text);

            particulars = Convert.ToDouble(txtTrips.Text) + Convert.ToDouble(txtAllowance.Text)
                + Convert.ToDouble(txtCommission.Text) + Convert.ToDouble(txtnotesdeduct1.Text)
                + (Convert.ToDouble(txtOvertimehours.Text) * Convert.ToDouble(txtRatehr.Text));

            txtOvertimetotal.Text = (Convert.ToDouble(txtOvertimehours.Text) * Convert.ToDouble(txtRatehr.Text)).ToString();

            x = (Convert.ToDouble(txtWage.Text) * (Convert.ToDouble(txtWorkdays.Text))) - Convert.ToDouble(x);

            x = x + particulars;

            return x.ToString("N0");
        }

        private void clearFields()
        {
            startDate.Text = "";
            endDate.Text = "";

            txtWage.Text = "";
            txtRatehr.Text = "";
            txtWorkdays.Text = "";

            txtSSS.Text = "0";
            txtphilhealth.Text = "0";
            txtpagibig.Text = "0";
            //txtwithtax.Text = "0";
            txtsssloan.Text = "0";
            txtisap.Text = "0";
            txtis.Text = "0";
            txtOthers.Text = "";

            txtpey.Text = "0";
            txtpel.Text = "0";
            txtgrl.Text = "0";
            txteml.Text = "0";
            txtelecbill.Text = "0";
            txtca.Text = "0";
            txtabsent.Text = "0";
            txtLates.Text = "0";
            txtUndertime.Text = "0";

            txtTrips.Text = "0";
            txtOvertimehours.Text = "0";
            txtOvertimetotal.Text = "0";
            txtAllowance.Text = "0";
            txtCommission.Text = "0";
            txtNotes.Text = "0";

            txtnotesdeduct1.Text = "0";
            txtothersdeduct1.Text = "0";

            lblNetPay.Content = "";

        }

        private async void btnCompute_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            if (x)
            {
                lblNetPay.Content = computeDeductions();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            if (x)
            {
                updatePayRollRecord();
                fillModelForReport();
                await this.ShowMessageAsync("UPDATE RECORD", "Record updated successfully.");
                payRollView.dgvEmployees.ItemsSource = loadDataGridDetails();
                //clearFields();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            if (x)
            {
                lblNetPay.Content = computeDeductions();
                savePayrollRecord();
                fillModelForReport();
                await this.ShowMessageAsync("SAVE RECORD", "Record saved successfully.");
                payRollView.dgvEmployees.ItemsSource = loadDataGridDetails();
                //clearFields();
                //btnSave.Visibility = Visibility.Hidden;
                //btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            if (string.IsNullOrEmpty(startDate.Text))
            {
                await this.ShowMessageAsync("Date From", "Please select date.");
            }
            else if (string.IsNullOrEmpty(endDate.Text))
            {
                await this.ShowMessageAsync("Date To", "Please select date.");
            }
            else if (cmbEmployees.SelectedItem == null)
            {
                await this.ShowMessageAsync("Employees", "Please select employee.");
            }
            else if (string.IsNullOrEmpty(txtWage.Text) || !ifValidFloatingNumber(txtWage.Text))
            {
                await this.ShowMessageAsync("Wage", "Please input wage or input not valid.");
            }
            else if (string.IsNullOrEmpty(txtRatehr.Text) || !ifValidFloatingNumber(txtRatehr.Text))
            {
                await this.ShowMessageAsync("Rate/HR", "Please input rate per hour.");
            }
            else if (string.IsNullOrEmpty(txtWorkdays.Text))
            {
                await this.ShowMessageAsync("Work days", "Please input work days.");
            }
            else if (string.IsNullOrEmpty(txtSSS.Text) || !ifValidFloatingNumber(txtSSS.Text))
            {
                await this.ShowMessageAsync("SSS", "Please input SSS value.");
            }
            else if (string.IsNullOrEmpty(txtphilhealth.Text) || !ifValidFloatingNumber(txtphilhealth.Text))
            {
                await this.ShowMessageAsync("PhilHealth", "Please input PhilHealth value.");
            }
            else if (string.IsNullOrEmpty(txtpagibig.Text) || !ifValidFloatingNumber(txtpagibig.Text))
            {
                await this.ShowMessageAsync("Pag-Ibig", "Please input Pag-ibig value.");
            }
            else if (string.IsNullOrEmpty(txtsssloan.Text) || !ifValidFloatingNumber(txtsssloan.Text))
            {
                await this.ShowMessageAsync("SSS Loan", "Please input SSS value.");
            }
            else if (string.IsNullOrEmpty(txtisap.Text) || !ifValidFloatingNumber(txtisap.Text))
            {
                await this.ShowMessageAsync("ISAP", "Please input ISAP value.");
            }
            else if (string.IsNullOrEmpty(txtis.Text) || !ifValidFloatingNumber(txtis.Text))
            {
                await this.ShowMessageAsync("IS", "Please input IS value.");
            }
            else if (string.IsNullOrEmpty(txtca.Text) || !ifValidFloatingNumber(txtca.Text))
            {
                await this.ShowMessageAsync("Cash Advance", "Please input C.A. value.");
            }
            else if (string.IsNullOrEmpty(txtpel.Text) || !ifValidFloatingNumber(txtpel.Text))
            {
                await this.ShowMessageAsync("PEL", "Please input PEL value.");
            }
            else if (string.IsNullOrEmpty(txteml.Text) || !ifValidFloatingNumber(txteml.Text))
            {
                await this.ShowMessageAsync("EML", "Please input EML value.");
            }
            else if (string.IsNullOrEmpty(txtgrl.Text) || !ifValidFloatingNumber(txtgrl.Text))
            {
                await this.ShowMessageAsync("GRL", "Please input GRL value.");
            }
            else if (string.IsNullOrEmpty(txtpey.Text) || !ifValidFloatingNumber(txtpey.Text))
            {
                await this.ShowMessageAsync("PEY", "Please input PEY value.");
            }
            else if (string.IsNullOrEmpty(txtelecbill.Text) || !ifValidFloatingNumber(txtelecbill.Text))
            {
                await this.ShowMessageAsync("Elec. Bill", "Please input Elec. Bill.");
            }
            else if (string.IsNullOrEmpty(txtabsent.Text) || !ifValidFloatingNumber(txtabsent.Text))
            {
                await this.ShowMessageAsync("Absent", "Please input absent value.");
            }
            else if (string.IsNullOrEmpty(txtLates.Text) || !ifValidFloatingNumber(txtLates.Text))
            {
                await this.ShowMessageAsync("Lates", "Please input lates value.");
            }
            else if (string.IsNullOrEmpty(txtUndertime.Text) || !ifValidFloatingNumber(txtUndertime.Text))
            {
                await this.ShowMessageAsync("Undertime", "Please iput undertime value.");
            }
            else if (string.IsNullOrEmpty(txtTrips.Text) || !ifValidFloatingNumber(txtTrips.Text))
            {
                await this.ShowMessageAsync("Trips", "Please input trips value.");
            }
            else if (string.IsNullOrEmpty(txtOvertimehours.Text) || !ifValidFloatingNumber(txtOvertimehours.Text))
            {
                await this.ShowMessageAsync("Overtime", "Please input overtime value.");
            }
            else if (string.IsNullOrEmpty(txtCommission.Text) || !ifValidFloatingNumber(txtCommission.Text))
            {
                await this.ShowMessageAsync("Commission", "Please input commission value.");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "." || e.Text == "-"))
            {
                e.Handled = true;
            }
        }

        private bool ifValidFloatingNumber(string txtField)
        {
            bool ifOKay = false;

            ifOKay = Regex.IsMatch(txtField, "^[+-]?([0-9]*[.])?[0-9]+$");

            return ifOKay;
        }

        #region ControlsUI
        private void txtSSS_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtphilhealth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtpagibig_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtsssloan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtisap_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtis_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtca_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtpel_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txteml_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtgrl_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtpey_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtelecbill_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtabsent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtLates_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtUndertime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtTrips_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtOvertimehours_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtAllowance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtCommission_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtothersdeduct1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtnotesdeduct1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void EnableDisableValueControls(bool en)
        {
            txtSSS.IsEnabled = en;
            txtphilhealth.IsEnabled = en;
            txtpagibig.IsEnabled = en;
            txtsssloan.IsEnabled = en;
            txtpel.IsEnabled = en;
            txteml.IsEnabled = en;
            txtgrl.IsEnabled = en;
            txtpey.IsEnabled = en;
            txtAllowance.IsEnabled = en;

            chkSSS.IsChecked = en;
            chkPhilHealth.IsChecked = en;
            chkPagibig.IsChecked = en;
            chkSSSLoan.IsChecked = en;
            chkPEL.IsChecked = en;
            chkEML.IsChecked = en;
            chkGRL.IsChecked = en;
            chkPEY.IsChecked = en;
            chkAllowance.IsChecked = en;
        }

        private async void chkSSS_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtSSS.IsEnabled = true;
                if (payrollModel != null)
                {
                    txtSSS.Text = payrollModel.SSS;
                }
                else
                {
                    txtSSS.Text = selectedEmployee.SSS;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("PAYROLL", "Please select employee first.");

            }

        }

        private void chkSSS_Unchecked(object sender, RoutedEventArgs e)
        {
            txtSSS.IsEnabled = false;
            txtSSS.Text = "0";
        }

        private async void chkPhilHealth_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtphilhealth.IsEnabled = true;
                if (payrollModel != null)
                {
                    txtphilhealth.Text = payrollModel.PhilHealth;
                }
                else
                {
                    txtphilhealth.Text = selectedEmployee.PhilHealth;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("PAYROLL", "Please select employee first.");
            }


        }

        private void chkPhilHealth_Unchecked(object sender, RoutedEventArgs e)
        {
            txtphilhealth.IsEnabled = false;
            txtphilhealth.Text = "0";
        }

        private async void chkPagibig_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtpagibig.IsEnabled = true;
                if (payrollModel != null)
                {
                    txtpagibig.Text = payrollModel.PagIbig;
                }
                else
                {
                    txtpagibig.Text = selectedEmployee.Pagibig;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("PAYROLL", "Please select employee first.");
            }


        }

        private void chkPagibig_Unchecked(object sender, RoutedEventArgs e)
        {
            txtpagibig.IsEnabled = false;
            txtpagibig.Text = "0";
        }

        private async void chkSSSLoan_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtsssloan.IsEnabled = true;
                if (payrollModel != null)
                {
                    txtsssloan.Text = payrollModel.SSSLoan;
                }
                else
                {
                    txtsssloan.Text = selectedEmployee.SSSLoan;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("PAYROLL", "Please select employee first.");
            }


        }

        private void chkSSSLoan_Unchecked(object sender, RoutedEventArgs e)
        {
            txtsssloan.IsEnabled = false;
            txtsssloan.Text = "0";
        }

        private async void chkPEL_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtpel.IsEnabled = true;
                if (payrollModel != null)
                {
                    txtpel.Text = payrollModel.PEL;
                }
                else
                {
                    txtpel.Text = selectedEmployee.PEL;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("PAYROLL", "Please select employee first.");
            }


        }

        private void chkPEL_Unchecked(object sender, RoutedEventArgs e)
        {
            txtpel.IsEnabled = false;
            txtpel.Text = "0";
        }

        private async void chkEML_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txteml.IsEnabled = true;
                if (payrollModel != null)
                {
                    txteml.Text = payrollModel.EML;
                }
                else
                {
                    txteml.Text = selectedEmployee.EML;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("PAYROLL", "Please select employee first.");
            }


        }

        private void chkEML_Unchecked(object sender, RoutedEventArgs e)
        {
            txteml.IsEnabled = false;
            txteml.Text = "0";
        }

        private async void chkGRL_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtgrl.IsEnabled = true;
                if (payrollModel != null)
                {
                    txtgrl.Text = payrollModel.GRL;
                }
                else
                {
                    txtgrl.Text = selectedEmployee.GRL;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("PAYROLL", "Please select employee first.");
            }
        }

        private void chkGRL_Unchecked(object sender, RoutedEventArgs e)
        {
            txtgrl.IsEnabled = false;
            txtgrl.Text = "0";
        }

        private async void chkPEY_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtpey.IsEnabled = true;
                if (payrollModel != null)
                {
                    txtpey.Text = payrollModel.PEY;
                }
                else
                {
                    txtpey.Text = selectedEmployee.PEY;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("PAYROLL", "Please select employee first.");
            }
        }

        private void chkPEY_Unchecked(object sender, RoutedEventArgs e)
        {
            txtpey.IsEnabled = false;
            txtpey.Text = "0";
        }

        private async void chkAllowance_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtAllowance.IsEnabled = true;
                if (payrollModel != null)
                {
                    txtAllowance.Text = payrollModel.Allowance;
                }
                else
                {
                    txtAllowance.Text = selectedEmployee.Allowance;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("PAYROLL", "Please select employee first.");
            }


        }

        private void chkAllowance_Unchecked(object sender, RoutedEventArgs e)
        {
            txtAllowance.IsEnabled = false;
            txtAllowance.Text = "0";
        }

        private async void btnPrintPaySlip_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            if (x)
            {
                lblNetPay.Content = computeDeductions();
                ReportForm report = new ReportForm(LstPayrollModelPayslip);
                report.ShowDialog();
            }
            
        }

        private void btnEnterPin_Click(object sender, RoutedEventArgs e)
        {
            PinCodeWindow pinCode = new PinCodeWindow();
            pinCode.ShowDialog();
        }

        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            PinCodeWindow pinCode = new PinCodeWindow(this);
            pinCode.ShowDialog();
        }

        private void btnHidePassword_Click(object sender, RoutedEventArgs e)
        {
            btnShowPassword.Visibility = Visibility.Visible;
            btnHidePassword.Visibility = Visibility.Hidden;
            hideWageBox.Visibility = Visibility.Visible;
        }
        #endregion

        private double getEmployeeLoanPEL(string eID)
        {
            conDB = new ConnectionDB();
            double dblLoan = 0;
            double dblInterest = 0;
            queryString = "SELECT tblloanspel.ID, empID, concat(firstname, ' ', lastname) as fullname, " +
                "sum(loans) as loans, sum(interest) as interest FROM(tblloanspel " +
                "INNER JOIN tblemployees ON tblloanspel.empID = " +
                "tblemployees.ID) WHERE tblloanspel.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblLoan = (!string.IsNullOrEmpty(reader["loans"].ToString())) ?
                     Convert.ToDouble(reader["loans"].ToString()) : 0;

                dblInterest = (!string.IsNullOrEmpty(reader["interest"].ToString())) ?
                     Convert.ToDouble(reader["interest"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblLoan;
        }

        private double getToBeSubtractedPEL(string eID)
        {
            conDB = new ConnectionDB();
            double dblPending = 0;
            queryString = "SELECT empID, sum(pel) as pelpending FROM tblpayroll WHERE tblpayroll.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblPending = (!string.IsNullOrEmpty(reader["pelpending"].ToString())) ?
                     Convert.ToDouble(reader["pelpending"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblPending;
        }

        private double getEmployeeLoanEML(string eID)
        {
            conDB = new ConnectionDB();
            double dblLoan = 0;
            double dblInterest = 0;
            queryString = "SELECT tblloanseml.ID, empID, concat(firstname, ' ', lastname) as fullname, " +
                "sum(loans) as loans, loandate, sum(interest) as interest FROM (tblloanseml INNER JOIN tblemployees ON " +
                " tblloanseml.empID = tblemployees.ID) " +
                "WHERE tblloanseml.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblLoan = (!string.IsNullOrEmpty(reader["loans"].ToString())) ?
                     Convert.ToDouble(reader["loans"].ToString()) : 0;

                dblInterest = (!string.IsNullOrEmpty(reader["interest"].ToString())) ?
                     Convert.ToDouble(reader["interest"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblLoan;
        }

        private double getToBeSubtractedEML(string eID)
        {
            conDB = new ConnectionDB();
            double dblPending = 0;
            queryString = "SELECT empID, sum(eml) as emlpending FROM tblpayroll WHERE tblpayroll.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblPending = (!string.IsNullOrEmpty(reader["emlpending"].ToString())) ?
                     Convert.ToDouble(reader["emlpending"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblPending;
        }

        private double getEmployeeLoanGRL(string eID)
        {
            conDB = new ConnectionDB();
            double dblLoan = 0;
            queryString = "SELECT tblloansgrl.ID, empID, concat(firstname, ' ', lastname) as fullname, " +
                "sum(loans) as loans, loandate FROM (tblloansgrl INNER JOIN tblemployees ON " +
                " tblloansgrl.empID = tblemployees.ID) " +
                "WHERE tblloansgrl.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblLoan = (!string.IsNullOrEmpty(reader["loans"].ToString())) ?
                     Convert.ToDouble(reader["loans"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblLoan;
        }

        private double getToBeSubtractedGRL(string eID)
        {
            conDB = new ConnectionDB();
            double dblPending = 0;
            queryString = "SELECT empID, sum(grl) as grlpending FROM tblpayroll WHERE tblpayroll.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblPending = (!string.IsNullOrEmpty(reader["grlpending"].ToString())) ?
                     Convert.ToDouble(reader["grlpending"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblPending;
        }

        private double getEmployeeLoanPEY(string eID)
        {
            conDB = new ConnectionDB();
            double dblLoan = 0;
            queryString = "SELECT tblloanspey.ID, empID, concat(firstname, ' ', lastname) as fullname, " +
                "sum(loans) as loans, loandate FROM (tblloanspey INNER JOIN tblemployees ON " +
                " tblloanspey.empID = tblemployees.ID) " +
                "WHERE tblloanspey.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblLoan = (!string.IsNullOrEmpty(reader["loans"].ToString())) ?
                     Convert.ToDouble(reader["loans"].ToString()) : 0;


            }

            conDB.closeConnection();
            return dblLoan;
        }

        private double getToBeSubtractedPEY(string eID)
        {
            conDB = new ConnectionDB();
            double dblPending = 0;
            queryString = "SELECT empID, sum(pey) as peypending FROM tblpayroll WHERE tblpayroll.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblPending = (!string.IsNullOrEmpty(reader["peypending"].ToString())) ?
                     Convert.ToDouble(reader["peypending"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblPending;
        }

        private double getEmployeeLoanElecBill(string eID)
        {
            conDB = new ConnectionDB();
            double dblLoan = 0;
            queryString = "SELECT tblelecbill.empid, sum(elecbill) as electricbill, concat(lastname,' , ', firstname) as fullname, " +
                "dateadded FROM (tblelecbill INNER JOIN tblemployees ON " +
                "tblelecbill.empID = tblemployees.ID) WHERE tblelecbill.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblLoan = (!string.IsNullOrEmpty(reader["electricbill"].ToString())) ?
                     Convert.ToDouble(reader["electricbill"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblLoan;
        }

        private double getToBeSubtractedElecBill(string eID)
        {
            conDB = new ConnectionDB();
            double dblPending = 0;
            queryString = "SELECT empID, sum(electricbill) as electricbill FROM tblpayroll WHERE tblpayroll.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblPending = (!string.IsNullOrEmpty(reader["electricbill"].ToString())) ?
                     Convert.ToDouble(reader["electricbill"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblPending;
        }

        private double getEmployeeLoanSSSLoan(string eID)
        {
            conDB = new ConnectionDB();
            double dblLoan = 0;
            queryString = "SELECT tblsssloan.empid, sum(sssloan) as existingsss, concat(lastname,' , ', firstname) as fullname, " +
                "dateadded FROM (tblsssloan INNER JOIN tblemployees ON " +
                "tblsssloan.empID = tblemployees.ID) WHERE tblsssloan.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblLoan = (!string.IsNullOrEmpty(reader["existingsss"].ToString())) ?
                     Convert.ToDouble(reader["existingsss"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblLoan;
        }

        private double getToBeSubtractedSSSLoan(string eID)
        {
            conDB = new ConnectionDB();
            double dblPending = 0;
            queryString = "SELECT empID, sum(sssloan) as sssloan FROM tblpayroll WHERE tblpayroll.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblPending = (!string.IsNullOrEmpty(reader["sssloan"].ToString())) ?
                     Convert.ToDouble(reader["sssloan"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblPending;
        }

        private double getEmployeeTotalIS(string eID)
        {
            conDB = new ConnectionDB();
            double dblLoan = 0;
            queryString = "SELECT tblintactsavings.empid, sum(existingis) as existingis, concat(lastname,' , ', firstname) as fullname," +
                " dateadded FROM (tblintactsavings INNER JOIN tblemployees ON " +
                "tblintactsavings.empID = tblemployees.ID) WHERE tblintactsavings.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblLoan = (!string.IsNullOrEmpty(reader["existingis"].ToString())) ?
                     Convert.ToDouble(reader["existingis"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblLoan;
        }

        private double getToBeAddedTotalIS(string eID)
        {
            conDB = new ConnectionDB();
            double dblPending = 0;
            queryString = "SELECT empID, sum(isavings) as isavings FROM tblpayroll WHERE tblpayroll.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblPending = (!string.IsNullOrEmpty(reader["isavings"].ToString())) ?
                     Convert.ToDouble(reader["isavings"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblPending;
        }

        private double getEmployeeTotalISAP(string eID)
        {
            conDB = new ConnectionDB();
            double dblLoan = 0;
            queryString = "SELECT tblisap.empid, sum(existingisap) as existingisap, concat(lastname,' , ', firstname) as fullname, " +
                "dateadded FROM (tblisap INNER JOIN tblemployees ON " +
                "tblisap.empID = tblemployees.ID) WHERE tblisap.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblLoan = (!string.IsNullOrEmpty(reader["existingisap"].ToString())) ?
                     Convert.ToDouble(reader["existingisap"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblLoan;
        }

        private double getToBeAddedTotalISAP(string eID)
        {
            conDB = new ConnectionDB();
            double dblPending = 0;
            queryString = "SELECT empID, sum(isap) as isap FROM tblpayroll WHERE tblpayroll.isDeleted = 0 AND empID = ?";

            parameters = new List<string>();
            parameters.Add(eID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblPending = (!string.IsNullOrEmpty(reader["isap"].ToString())) ?
                     Convert.ToDouble(reader["isap"].ToString()) : 0;
            }

            conDB.closeConnection();
            return dblPending;
        }

        private double setUpdatedValuesForIntactSavings(double fromTextControl, double fromClass, double origTotal)
        {
            double updatedVal = 0;
            double dblTemp = 0;
            if (Convert.ToDouble(fromTextControl) < Convert.ToDouble(fromClass))
            {
                dblTemp = fromClass - fromTextControl;
                updatedVal = origTotal - dblTemp;
            }
            else if (Convert.ToDouble(txtpel.Text) > Convert.ToDouble(payrollModel.PEL))
            {
                dblTemp = fromClass + dblTemp;
                updatedVal = origTotal + dblTemp;
            }
            else
            {
                updatedVal = origTotal;
            }

            return updatedVal;
        }

        private double setUpdatedValuesForLoans(double fromTextControl, double fromClass, double origTotal)
        {
            double updatedVal = 0;
            double dblTemp = 0;
            if (Convert.ToDouble(fromTextControl) < Convert.ToDouble(fromClass))
            {
                dblTemp = fromClass - fromTextControl;
                updatedVal = origTotal + dblTemp;
            }
            else if (Convert.ToDouble(txtpel.Text) > Convert.ToDouble(payrollModel.PEL))
            {
                dblTemp = fromClass + dblTemp;
                updatedVal = origTotal - dblTemp;
            }
            else
            {
                updatedVal = origTotal;
            }

            return updatedVal;
        }

    }
}
