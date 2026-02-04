using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Configuration;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class Reportform : Page
    {
        private string previousEmployeeName = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRegions();
                LoadBranches();
                LoadEmployeeNames();
            }
        }

        private void LoadRegions()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = "SELECT RegionId, Region FROM Region ORDER BY Region";  // Updated query for your Region table

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
            ddlRegion.DataTextField = "Region";  // Display the Region name
            ddlRegion.DataValueField = "RegionId";  // Use RegionId as value
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
            LoadBranches(selectedRegionId);  // Load branches for the selected region
            LoadEmployeeNames();  // Refresh employees based on new branch filter
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBranch = ddlBranch.SelectedValue;
            LoadEmployeeNames(selectedBranch);  // Load employees for the selected branch
        }

        private void LoadEmployeeNames(string branchName = null)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"SELECT DISTINCT FirstName 
                     FROM Employees 
                     WHERE (@BranchName IS NULL OR BranchName = @BranchName)
                       AND FirstName IS NOT NULL 
                       AND LTRIM(RTRIM(FirstName)) != ''
                     ORDER BY FirstName";

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
                ddlEmployee.DataTextField = "FirstName";
                ddlEmployee.DataValueField = "FirstName";
                ddlEmployee.DataBind();
            }

            ddlEmployee.Items.Insert(0, new ListItem("All Employees", ""));
        }
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedRegionId = ddlRegion.SelectedValue;
            string selectedBranch = ddlBranch.SelectedValue;
            string selectedEmployeeName = ddlEmployee.SelectedValue;
            DateTime fromDate, toDate;

            bool fromDateValid = DateTime.TryParse(txtFromDate.Text, out fromDate);
            bool toDateValid = DateTime.TryParse(txtToDate.Text, out toDate);

            // Clear previous error message
            lblError.Visible = false;

            if (fromDateValid && toDateValid)
            {
                if (fromDate <= toDate)
                {
                    LoadData(selectedRegionId, selectedBranch, selectedEmployeeName, fromDate, toDate);
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

        private void LoadData(string regionId, string branchName, string employeeName, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"
       SELECT 
    emp.FirstName AS Eng_Name,
    expDetails.ExpenseType AS Tour_Local,
    expDetails.Date AS Date,
    expDetails.FromTime AS From_Time,
    expDetails.ToTime AS To_Time,
    expDetails.Particulars AS Particulars,
    expDetails.Distance AS Distance,
    expDetails.TransportType AS Transport,
    expDetails.Conveyance,
    expDetails.Lodging,
    expDetails.Food,
    expDetails.Others,
    expDetails.Miscellaneous,
    expDetails.SoNo AS SO_Number,
    s.Department AS Department,
    s.ServiceType AS Nature_of_Work,
    expDetails.SmoNo AS SMO,
    expDetails.RefNo AS Document_Reference,
    expDetails.Remarks AS Remarks
FROM 
(
    -- Conveyance
    SELECT 
        ServiceId, 
        ExpenseType, 
        Date, 
        FromTime, 
        ToTime, 
        Particulars, 
        Distance, 
        TransportType,
        ClaimedAmount AS Conveyance, 
        NULL AS Lodging, 
        NULL AS Food, 
        NULL AS Others, 
        NULL AS Miscellaneous,
        SoNo, 
        SmoNo, 
        RefNo, 
        Remarks
    FROM Conveyance
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND isclaimable = 1  -- Add condition for isclaimable
    UNION ALL
    -- Food
    SELECT 
        ServiceId, 
        ExpenseType, 
        Date, 
        FromTime, 
        ToTime, 
        Particulars, 
        NULL AS Distance, 
        NULL AS TransportType,
        NULL AS Conveyance, 
        NULL AS Lodging, 
        ClaimedAmount AS Food, 
        NULL AS Others, 
        NULL AS Miscellaneous,
        SoNo, 
        SmoNo, 
        RefNo, 
        Remarks
    FROM Food
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND isclaimable = 1  -- Add condition for isclaimable
    UNION ALL
    -- Lodging
    SELECT 
        ServiceId, 
        ExpenseType, 
        Date, 
        FromTime, 
        ToTime, 
        Particulars, 
        NULL AS Distance, 
        NULL AS TransportType,
        NULL AS Conveyance, 
        ClaimedAmount AS Lodging, 
        NULL AS Food, 
        NULL AS Others, 
        NULL AS Miscellaneous,
        SoNo, 
        SmoNo, 
        RefNo, 
        Remarks
    FROM Lodging
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND isclaimable = 1  -- Add condition for isclaimable
    UNION ALL
    -- Others
    SELECT 
        ServiceId, 
        ExpenseType, 
        Date, 
        FromTime, 
        ToTime, 
        Particulars, 
        NULL AS Distance, 
        NULL AS TransportType,
        NULL AS Conveyance, 
        NULL AS Lodging, 
        NULL AS Food, 
        ClaimedAmount AS Others, 
        NULL AS Miscellaneous,
        SoNo, 
        SmoNo, 
        RefNo, 
        Remarks
    FROM Others
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND isclaimable = 1  -- Add condition for isclaimable
    UNION ALL
    -- Miscellaneous
    SELECT 
        ServiceId, 
        ExpenseType, 
        Date, 
        FromTime, 
        ToTime, 
        Particulars, 
        NULL AS Distance, 
        NULL AS TransportType,
        NULL AS Conveyance, 
        NULL AS Lodging, 
        NULL AS Food, 
        NULL AS Others, 
        ClaimedAmount AS Miscellaneous,
        SoNo, 
        SmoNo, 
        RefNo, 
        Remarks
    FROM Miscellaneous
    WHERE Date BETWEEN @FromDate AND @ToDate
      AND isclaimable = 1  -- Add condition for isclaimable
) AS expDetails
LEFT JOIN Services s ON expDetails.ServiceId = s.ServiceId
LEFT JOIN Employees emp ON s.EmployeeId = emp.EmployeeId
LEFT JOIN Expense e ON expDetails.ServiceId = e.ServiceId
LEFT JOIN Branch b ON emp.BranchName = b.BranchName
WHERE (@RegionId IS NULL OR b.RegionId = @RegionId)
  AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
  AND (@EmployeeName IS NULL OR emp.FirstName LIKE '%' + @EmployeeName + '%')
  AND e.StatusId = 3  -- Existing condition
ORDER BY emp.FirstName, expDetails.Date, expDetails.FromTime";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandTimeout = 120;
                    // Add query parameters
                    if (string.IsNullOrEmpty(regionId))
                    {
                        cmd.Parameters.AddWithValue("@RegionId", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@RegionId", regionId);
                    }
                    cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(branchName) ? (object)DBNull.Value : branchName);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@EmployeeName", string.IsNullOrEmpty(employeeName) ? (object)DBNull.Value : employeeName);

                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Add Total column to DataTable
                    dt.Columns.Add("Total", typeof(decimal));
                    decimal totalConveyance = 0, totalFood = 0, totalOthers = 0, totalLodging = 0, totalMiscellaneous = 0, overallTotal = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        decimal conveyance = row["Conveyance"] != DBNull.Value ? Convert.ToDecimal(row["Conveyance"]) : 0;
                        decimal food = row["Food"] != DBNull.Value ? Convert.ToDecimal(row["Food"]) : 0;
                        decimal others = row["Others"] != DBNull.Value ? Convert.ToDecimal(row["Others"]) : 0;
                        decimal lodging = row["Lodging"] != DBNull.Value ? Convert.ToDecimal(row["Lodging"]) : 0;
                        decimal miscellaneous = row["Miscellaneous"] != DBNull.Value ? Convert.ToDecimal(row["Miscellaneous"]) : 0;
                        decimal rowTotal = conveyance + food + others + lodging + miscellaneous;
                        row["Conveyance"] = conveyance;
                        row["Food"] = food;
                        row["Others"] = others;
                        row["Lodging"] = lodging;
                        row["Miscellaneous"] = miscellaneous;
                        row["Total"] = rowTotal;
                        totalConveyance += conveyance;
                        totalFood += food;
                        totalOthers += others;
                        totalLodging += lodging;
                        totalMiscellaneous += miscellaneous;
                        overallTotal += rowTotal;
                    }

                    if (dt.Rows.Count > 0)
                    {
                        DataRow totalsRow = dt.NewRow();
                        totalsRow["Eng_Name"] = "Total";
                        totalsRow["Conveyance"] = totalConveyance;
                        totalsRow["Food"] = totalFood;
                        totalsRow["Others"] = totalOthers;
                        totalsRow["Lodging"] = totalLodging;
                        totalsRow["Miscellaneous"] = totalMiscellaneous;
                        totalsRow["Total"] = overallTotal;
                        dt.Rows.Add(totalsRow);
                    }

                    gvReport.DataSource = dt;
                    gvReport.DataBind();
                }
            }
        }

        protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string currentEmployeeName = DataBinder.Eval(e.Row.DataItem, "Eng_Name")?.ToString();

                if (!string.Equals(previousEmployeeName, currentEmployeeName, StringComparison.OrdinalIgnoreCase))
                {
                    previousEmployeeName = currentEmployeeName;

                    GridViewRow employeeRow = new GridViewRow(-1, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                    TableCell headerCell = new TableCell
                    {
                        ColumnSpan = gvReport.Columns.Count,
                        CssClass = "employee-header",
                        Text = $"<strong>Employee: {currentEmployeeName}</strong> - Department: {DataBinder.Eval(e.Row.DataItem, "Department")}"
                    };
                    employeeRow.Cells.Add(headerCell);
                    employeeRow.CssClass = "employee-header-row";

                    gvReport.Controls[0].Controls.AddAt(e.Row.RowIndex, employeeRow);
                }
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            ExportToExcel(gvReport);
        }

        private void ExportToExcel(GridView gridView)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=ExpenseReport.xls");
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            gridView.AllowPaging = false;
            gridView.RenderControl(hw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required to avoid the runtime error "Control 'GridView1' of type 'GridView' must be placed inside a form tag with runat=server."
        }
    }
}