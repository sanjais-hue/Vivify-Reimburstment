using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Configuration;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class TravelReport : System.Web.UI.Page
    {
       

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set default dates to current month
                DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                txtFromDate.Text = firstDayOfMonth.ToString("yyyy-MM-dd");
                txtToDate.Text = lastDayOfMonth.ToString("yyyy-MM-dd");

                LoadRegions();
                LoadBranches();
                LoadEmployeeNames();

                // Load initial data for current month
                LoadData("", "", "", firstDayOfMonth, lastDayOfMonth);
            }
        }

        private void LoadRegions()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = "SELECT RegionId, Region FROM Region ORDER BY Region";

            DataTable regionsTable = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(regionsTable);
                    }
                }
            }

            ddlRegion.DataSource = regionsTable;
            ddlRegion.DataTextField = "Region";
            ddlRegion.DataValueField = "RegionId";
            ddlRegion.DataBind();

            ddlRegion.Items.Insert(0, new ListItem("All Regions", ""));
        }

        private void LoadBranches(string regionId = null)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"SELECT DISTINCT BranchName 
                     FROM Branch 
                     WHERE (@RegionId IS NULL OR RegionId = @RegionId) 
                     ORDER BY BranchName";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (string.IsNullOrEmpty(regionId))
                    cmd.Parameters.AddWithValue("@RegionId", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@RegionId", regionId);

                con.Open();
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(dt);

                ddlBranch.DataSource = dt;
                ddlBranch.DataTextField = "BranchName";
                ddlBranch.DataValueField = "BranchName";
                ddlBranch.DataBind();
            }

            ddlBranch.Items.Insert(0, new ListItem("All Branches", ""));
        }

        protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRegionId = ddlRegion.SelectedValue;
            LoadBranches(selectedRegionId);
            LoadEmployeeNames();
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBranch = ddlBranch.SelectedValue;
            LoadEmployeeNames(selectedBranch);
        }

        private void LoadEmployeeNames(string branchName = null)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"SELECT DISTINCT emp.FirstName + ' ' + ISNULL(emp.LastName, '') as FullName, emp.EmployeeId
                     FROM Employees emp
                     INNER JOIN Branch b ON emp.BranchName = b.BranchName
                     WHERE (@BranchName IS NULL OR emp.BranchName = @BranchName)
                       AND emp.FirstName IS NOT NULL 
                       AND LTRIM(RTRIM(emp.FirstName)) != ''
                     ORDER BY FullName";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (string.IsNullOrEmpty(branchName))
                    cmd.Parameters.AddWithValue("@BranchName", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@BranchName", branchName);

                con.Open();
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(dt);

                ddlEmployee.DataSource = dt;
                ddlEmployee.DataTextField = "FullName";
                ddlEmployee.DataValueField = "EmployeeId";
                ddlEmployee.DataBind();
            }

            ddlEmployee.Items.Insert(0, new ListItem("All Employees", ""));
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedRegionId = ddlRegion.SelectedValue;
            string selectedBranch = ddlBranch.SelectedValue;
            string selectedEmployeeId = ddlEmployee.SelectedValue;
            DateTime fromDate, toDate;

            bool fromDateValid = DateTime.TryParse(txtFromDate.Text, out fromDate);
            bool toDateValid = DateTime.TryParse(txtToDate.Text, out toDate);

            lblError.Visible = false;

            if (fromDateValid && toDateValid)
            {
                if (fromDate <= toDate)
                {
                    LoadData(selectedRegionId, selectedBranch, selectedEmployeeId, fromDate, toDate);
                }
                else
                {
                    lblError.Text = "From Date cannot be later than To Date.";
                    lblError.Visible = true;
                }
            }
            else
            {
                lblError.Text = "Please enter valid dates.";
                lblError.Visible = true;
            }
        }

        private void LoadData(string regionId, string branchName, string employeeId, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"
SELECT 
    te.Date,
    te.FromPlace,
    te.ToPlace,
    te.TransportType,
    te.ClaimedAmount,
    te.RefreshAmnt,
    te.Particulars,
    te.IsClaimable,
    emp.FirstName + ' ' + ISNULL(emp.LastName, '') AS EmployeeName,
    emp.EmployeeId,
    b.BranchName,
    r.Region
FROM [ReImbursement].[dbo].[TravelExpenses] te
INNER JOIN Employees emp ON te.EmployeeId = emp.EmployeeId
INNER JOIN Branch b ON emp.BranchName = b.BranchName
INNER JOIN Region r ON b.RegionId = r.RegionId
WHERE te.Date BETWEEN @FromDate AND @ToDate
  AND (@RegionId IS NULL OR r.RegionId = @RegionId)
  AND (@BranchName IS NULL OR b.BranchName = @BranchName)
  AND (@EmployeeId IS NULL OR emp.EmployeeId = @EmployeeId)
  AND te.IsClaimable = 1
ORDER BY te.Date, emp.FirstName";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandTimeout = 120;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@RegionId", string.IsNullOrEmpty(regionId) ? (object)DBNull.Value : regionId);
                    cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(branchName) ? (object)DBNull.Value : branchName);
                    cmd.Parameters.AddWithValue("@EmployeeId", string.IsNullOrEmpty(employeeId) ? (object)DBNull.Value : employeeId);

                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // NO TOTAL ROW ADDED HERE - This section should be completely empty
                    // Just bind the data directly without any modifications

                    gvReport.DataSource = dt;
                    gvReport.DataBind();
                }
            }
        }



        protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // No need to check for "TOTAL" row since we're not adding it anymore
                // Only handle non-claimable rows

                // Color coding based on IsClaimable status
                object isClaimableObj = rowView["IsClaimable"];
                if (isClaimableObj != DBNull.Value && isClaimableObj != null)
                {
                    bool isClaimable = Convert.ToBoolean(isClaimableObj);
                    if (!isClaimable)
                    {
                        e.Row.CssClass = "non-claimable-row";
                        e.Row.ToolTip = "Non-Claimable Expense";
                    }
                }
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }


        private void ExportToExcel()
        {
            string selectedRegionId = ddlRegion.SelectedValue;
            string selectedBranch = ddlBranch.SelectedValue;
            string selectedEmployeeId = ddlEmployee.SelectedValue;
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);

            DataTable dtExport = GetDataForExport();

            Response.Clear();
            Response.Buffer = true;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("content-disposition", "attachment;filename=Smiths_Travel_Expense_Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "utf-8";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            // Create Smiths Detection Systems header
            hw.Write("<table border='0' cellpadding='2' cellspacing='0' width='100%' style='font-family:Arial;font-size:12px;'>");

            // Company Header (Rows 1-3)
            hw.Write("<tr><td colspan='7' style='font-size:22px;font-weight:bold;text-align:center;'>SMITHS DETECTION SYSTEMS PVT LTD</td></tr>");
            hw.Write("<tr><td colspan='7' style='font-size:16px;text-align:center;'>Business Travel by own vehicle or public transport in INDIA</td></tr>");
            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");

            // Empty rows for spacing (Rows 4-6)
            hw.Write("<tr><td colspan='7' style='height:5px;'></td></tr>");
            hw.Write("<tr><td colspan='7' style='height:5px;'></td></tr>");
            hw.Write("<tr><td colspan='7' style='height:5px;'></td></tr>");

            // Get employee details for the header
            string employeeName = " ";
            string personnelNo = "";
            string reportingManager = "";
            string costCentre = "";

            if (!string.IsNullOrEmpty(selectedEmployeeId) && dtExport.Rows.Count > 0)
            {
                DataRow firstRow = dtExport.Rows[0];
                employeeName = firstRow["EmployeeName"].ToString();
            }

            // Employee Details (Rows 7-11)
            hw.Write("<tr>");
            hw.Write("<td style='font-weight:bold;width:120px;'>SAP Personnel No.</td>");
            hw.Write("<td style='width:80px;'></td>");
            hw.Write("<td style='width:100px;'>" + personnelNo + "</td>");
            hw.Write("<td colspan='4'></td>");
            hw.Write("</tr>");

            hw.Write("<tr>");
            hw.Write("<td style='font-weight:bold;'>Employee Name</td>");
            hw.Write("<td></td>");
            hw.Write("<td>" + employeeName + "</td>");
            hw.Write("<td colspan='4'></td>");
            hw.Write("</tr>");

            hw.Write("<tr>");
            hw.Write("<td style='font-weight:bold;'>Reporting Manager</td>");
            hw.Write("<td></td>");
            hw.Write("<td>" + reportingManager + "</td>");
            hw.Write("<td colspan='4'></td>");
            hw.Write("</tr>");

            hw.Write("<tr>");
            hw.Write("<td style='font-weight:bold;'>Cost Centre / WBS</td>");
            hw.Write("<td></td>");
            hw.Write("<td>" + costCentre + "</td>");
            hw.Write("<td colspan='4'></td>");
            hw.Write("</tr>");

            hw.Write("<tr>");
            hw.Write("<td style='font-weight:bold;'>Currency</td>");
            hw.Write("<td></td>");
            hw.Write("<td>INR</td>");
            hw.Write("<td colspan='4'></td>");
            hw.Write("</tr>");

            // Empty row for spacing (Row 12)
            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");

            // Table Header (Row 13) - YELLOW BACKGROUND with 14px font
            hw.Write("<tr>");
            hw.Write("<td style='border:1px solid #000;padding:3px;width:80px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>Date</td>");
            hw.Write("<td colspan='2' style='border:1px solid #000;padding:3px;width:200px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>Journey</td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;width:150px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>Travel Mode (Own/Auto/Bus/Metro)</td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;width:80px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>Dist. (km) / Amnt</td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;width:80px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>Refreshments</td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;width:150px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>Purpose of Travel</td>");
            hw.Write("</tr>");

            // Sub-header for Journey (Row 14) - YELLOW BACKGROUND with 14px font
            hw.Write("<tr>");
            hw.Write("<td style='border:1px solid #000;padding:3px;background-color:#FFFF00;font-weight:bold;font-size:14px;'></td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;background-color:#FFFF00;font-weight:bold;font-size:14px;'>From</td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;background-color:#FFFF00;font-weight:bold;font-size:14px;'>To</td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;background-color:#FFFF00;font-weight:bold;font-size:14px;'></td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;background-color:#FFFF00;font-weight:bold;font-size:14px;'></td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;background-color:#FFFF00;font-weight:bold;font-size:14px;'></td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;background-color:#FFFF00;font-weight:bold;font-size:14px;'></td>");
            hw.Write("</tr>");

            // Data Rows - Calculate totals
            decimal totalMileage = 0;  // Total distance in km for own vehicle
            decimal totalRefreshments = 0;
            decimal totalPublicTransport = 0;

            foreach (DataRow row in dtExport.Rows)
            {
                string currentDate = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                string fromPlace = row["FromPlace"].ToString();
                string toPlace = row["ToPlace"].ToString();
                string travelMode = row["TransportType"].ToString();
                decimal amount = Convert.ToDecimal(row["ClaimedAmount"]);
                decimal refreshments = Convert.ToDecimal(row["RefreshAmnt"]);
                string purpose = row["Particulars"].ToString();

                // **CORRECTED LOGIC: Calculate totals based on transport type**
                if (travelMode.ToLower().Contains("own") || travelMode.ToLower().Contains("vehicle"))
                {
                    // For own vehicle, amount represents distance in km
                    totalMileage += amount;
                }
                else
                {
                    // For public transport, amount represents money
                    totalPublicTransport += amount;
                }

                totalRefreshments += refreshments;

                // Data row
                hw.Write("<tr>");
                hw.Write("<td style='border:1px solid #000;padding:3px;'>" + currentDate + "</td>");
                hw.Write("<td style='border:1px solid #000;padding:3px;'>" + fromPlace + "</td>");
                hw.Write("<td style='border:1px solid #000;padding:3px;'>" + toPlace + "</td>");
                hw.Write("<td style='border:1px solid #000;padding:3px;'>" + travelMode + "</td>");

                // Display appropriate value based on transport type
                if (travelMode.ToLower().Contains("own") || travelMode.ToLower().Contains("vehicle"))
                {
                    hw.Write("<td style='border:1px solid #000;padding:3px;text-align:right;'>" + amount.ToString("N0") + " km</td>");
                }
                else
                {
                    hw.Write("<td style='border:1px solid #000;padding:3px;text-align:right;'>₹" + amount.ToString("N0") + "</td>");
                }

                hw.Write("<td style='border:1px solid #000;padding:3px;text-align:right;'>₹" + refreshments.ToString("N0") + "</td>");
                hw.Write("<td style='border:1px solid #000;padding:3px;'>" + purpose + "</td>");
                hw.Write("</tr>");
            }

            // **CORRECTED: Calculate mileage claim amount**
            decimal mileageClaimAmount = totalMileage * 10; // ₹10 per km
            decimal finalClaimAmount = mileageClaimAmount + totalRefreshments + totalPublicTransport;

            // Summary Section - GREY BACKGROUND
            hw.Write("<tr style='background-color:#D3D3D3;'>");
            hw.Write("<td style='font-weight:bold;'>Total Mileage</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;text-align:right;'>" + totalMileage.ToString("N0") + " km</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("</tr>");

            hw.Write("<tr style='background-color:#D3D3D3;'>");
            hw.Write("<td style='font-weight:bold;'>Mileage Claim Amount @ INR 10/Km.</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;text-align:right;'>₹" + mileageClaimAmount.ToString("N0") + "</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("</tr>");

            hw.Write("<tr style='background-color:#D3D3D3;'>");
            hw.Write("<td style='font-weight:bold;'>Travel by Public Transport</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;text-align:right;'>₹" + totalPublicTransport.ToString("N0") + "</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("</tr>");

            hw.Write("<tr style='background-color:#D3D3D3;'>");
            hw.Write("<td style='font-weight:bold;'>Refreshments</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;text-align:right;'>₹" + totalRefreshments.ToString("N0") + "</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("</tr>");

            // Empty rows
            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");
            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");

            // ERP/Toll/Parking Section (keep existing code for this section)
            hw.Write("<tr>");
            hw.Write("<td colspan='2' style='font-weight:bold;'>2.  ERP / Toll / Parking Claim</td>");
            hw.Write("<td colspan='5'></td>");
            hw.Write("</tr>");

            hw.Write("<tr><td colspan='7' style='height:5px;'></td></tr>");

            // ERP Table Header
            hw.Write("<tr>");
            hw.Write("<td style='border:1px solid #000;padding:3px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>Date</td>");
            hw.Write("<td colspan='3' style='border:1px solid #000;padding:3px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>ERP Gantry / Toll / Parking Location</td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>Amount</td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;font-weight:bold;background-color:#FFFF00;font-size:14px;'>Purpose of Travel</td>");
            hw.Write("<td style='border:1px solid #000;padding:3px;font-weight:bold;background-color:#FFFF00;font-size:14px;'></td>");
            hw.Write("</tr>");

            // Empty ERP rows
            for (int i = 0; i < 7; i++)
            {
                hw.Write("<tr>");
                hw.Write("<td style='border:1px solid #000;padding:3px;'></td>");
                hw.Write("<td colspan='3' style='border:1px solid #000;padding:3px;'></td>");
                hw.Write("<td style='border:1px solid #000;padding:3px;'></td>");
                hw.Write("<td style='border:1px solid #000;padding:3px;'></td>");
                hw.Write("<td style='border:1px solid #000;padding:3px;'></td>");
                hw.Write("</tr>");
            }

            // ERP Total
            hw.Write("<tr>");
            hw.Write("<td style='font-weight:bold;'>ERP Reimbursement Claimed</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;text-align:right;'>0</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("</tr>");

            // Empty rows
            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");
            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");

            // Total Reimbursement Section
            hw.Write("<tr>");
            hw.Write("<td colspan='2' style='font-weight:bold;'>3.  Total Reimbursement</td>");
            hw.Write("<td colspan='5'></td>");
            hw.Write("</tr>");

            hw.Write("<tr><td colspan='7' style='height:5px;'></td></tr>");

            hw.Write("<tr>");
            hw.Write("<td style='font-weight:bold;'>Final Claim Amount</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;text-align:right;'>₹" + finalClaimAmount.ToString("N0") + "</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("</tr>");

            // Empty rows
            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");
            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");
            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");

            // Approval Section
            hw.Write("<tr>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;'>Requested By:</td>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;'>Approved By:</td>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;'>Date:</td>");
            hw.Write("</tr>");

            hw.Write("<tr>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td>" + employeeName + "</td>");
            hw.Write("<td></td>");
            hw.Write("<td>" + reportingManager + "</td>");
            hw.Write("<td></td>");
            hw.Write("<td>" + DateTime.Now.ToString("yyyy-MM-dd") + "</td>");
            hw.Write("</tr>");

            hw.Write("<tr><td colspan='7' style='height:15px;'></td></tr>");

            hw.Write("<tr>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;'>Name</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("</tr>");

            hw.Write("<tr><td colspan='7' style='height:10px;'></td></tr>");

            hw.Write("<tr>");
            hw.Write("<td></td>");
            hw.Write("<td style='font-weight:bold;'>Signature</td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("<td></td>");
            hw.Write("</tr>");

            hw.Write("</table>");

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
        private DataTable GetDataForExport()
        {
            string selectedRegionId = ddlRegion.SelectedValue;
            string selectedBranch = ddlBranch.SelectedValue;
            string selectedEmployeeId = ddlEmployee.SelectedValue;
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                // Use the EXACT SAME QUERY as LoadData() method
                string query = @"
SELECT 
    te.Date,
    te.FromPlace,
    te.ToPlace,
    te.TransportType,
    te.ClaimedAmount,
    te.RefreshAmnt,
    te.Particulars,
    te.IsClaimable,
    emp.FirstName + ' ' + ISNULL(emp.LastName, '') AS EmployeeName,
    emp.EmployeeId,
    b.BranchName,
    r.Region
FROM [ReImbursement].[dbo].[TravelExpenses] te
INNER JOIN Employees emp ON te.EmployeeId = emp.EmployeeId
INNER JOIN Branch b ON emp.BranchName = b.BranchName
INNER JOIN Region r ON b.RegionId = r.RegionId
WHERE te.Date BETWEEN @FromDate AND @ToDate
  AND (@RegionId IS NULL OR r.RegionId = @RegionId)
  AND (@BranchName IS NULL OR b.BranchName = @BranchName)
  AND (@EmployeeId IS NULL OR emp.EmployeeId = @EmployeeId)
  AND te.IsClaimable = 1
ORDER BY te.Date, emp.FirstName";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@RegionId", string.IsNullOrEmpty(selectedRegionId) ? (object)DBNull.Value : selectedRegionId);
                    cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(selectedBranch) ? (object)DBNull.Value : selectedBranch);
                    cmd.Parameters.AddWithValue("@EmployeeId", string.IsNullOrEmpty(selectedEmployeeId) ? (object)DBNull.Value : selectedEmployeeId);

                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required to avoid the runtime error
        }
    }
}