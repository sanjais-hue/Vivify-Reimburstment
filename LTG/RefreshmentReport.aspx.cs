using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Configuration;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class RefreshmentReport : Page
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
            string query = @"
        SELECT BranchId, BranchName 
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
                ddlBranch.DataValueField = "BranchId"; // ✅ Use BranchId as value
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
            if (int.TryParse(ddlBranch.SelectedValue, out int branchId))
            {
                LoadEmployeeNames(branchId);
            }
            else
            {
                LoadEmployeeNames(null); // "All Branches" selected
            }
        }

        private void LoadEmployeeNames(int? branchId = null)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
        SELECT DISTINCT FirstName 
        FROM Employees 
        WHERE (@BranchId IS NULL OR BranchId = @BranchId)
          AND FirstName IS NOT NULL 
          AND LTRIM(RTRIM(FirstName)) != ''
        ORDER BY FirstName";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (branchId.HasValue)
                    cmd.Parameters.AddWithValue("@BranchId", branchId.Value);
                else
                    cmd.Parameters.AddWithValue("@BranchId", DBNull.Value);

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
            string selectedBranchId = ddlBranch.SelectedValue;
            string selectedEmployeeName = ddlEmployee.SelectedValue;
            DateTime fromDate, toDate;

            lblError.Visible = false;

            if (DateTime.TryParse(txtFromDate.Text, out fromDate) &&
                DateTime.TryParse(txtToDate.Text, out toDate))
            {
                if (fromDate <= toDate)
                {
                    DataTable dt = GetRefreshmentData(selectedRegionId, selectedBranchId, selectedEmployeeName, fromDate, toDate);
                    gvReport.DataSource = dt;
                    gvReport.DataBind();
                
                    lblError.Visible = true;
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


        private DataTable GetRefreshmentData(string regionId, string branchId, string employeeName, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"
            SELECT 
                e.FirstName AS Eng_Name,
                r.FromDate AS Date,
                '' AS FromTime,
                '' AS ToTime,
                'Refreshment' AS Particulars,
                '' AS Distance,
                '' AS Transport,
                '' AS Conveyance,
                '' AS Lodging,
                '' AS Food,
                '' AS TrainBus,
                '' AS Others,
                r.RefreshAmount AS Miscellaneous,
                r.RefreshAmount AS Total,
                'Refresh' AS Department,
                'Refresh' AS Nature_of_Work,
                'Refresh' AS SMO,
                '' AS Document_Reference
            FROM Refreshment r
            INNER JOIN Employees e ON r.EmployeeId = e.EmployeeId
            INNER JOIN Branch b ON e.BranchId = b.BranchId
            WHERE r.IsVerified = 1
              AND r.FromDate BETWEEN @FromDate AND @ToDate
              AND (@RegionId IS NULL OR b.RegionId = @RegionId)
              AND (@BranchId IS NULL OR b.BranchId = @BranchId)
              AND (@EmployeeName IS NULL OR e.FirstName LIKE '%' + @EmployeeName + '%')
            ORDER BY e.FirstName, r.FromDate";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@RegionId", string.IsNullOrEmpty(regionId) ? (object)DBNull.Value : regionId);

                    if (string.IsNullOrEmpty(branchId))
                        cmd.Parameters.AddWithValue("@BranchId", DBNull.Value);
                    else if (int.TryParse(branchId, out int parsedBranchId))
                        cmd.Parameters.AddWithValue("@BranchId", parsedBranchId);
                    else
                        return dt; // empty

                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@EmployeeName", string.IsNullOrEmpty(employeeName) ? (object)DBNull.Value : employeeName);

                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    // Add Total row
                    if (dt.Rows.Count > 0)
                    {
                        decimal total = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            total += Convert.ToDecimal(row["Miscellaneous"]);
                        }

                        DataRow totalRow = dt.NewRow();
                        totalRow["Eng_Name"] = "Total";
                        totalRow["Miscellaneous"] = total;
                        totalRow["Total"] = total;
                        dt.Rows.Add(totalRow);
                    }
                }
            }
            return dt;
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
            string selectedRegionId = ddlRegion.SelectedValue;
            string selectedBranchId = ddlBranch.SelectedValue;
            string selectedEmployeeName = ddlEmployee.SelectedValue;
            DateTime fromDate, toDate;

            if (!DateTime.TryParse(txtFromDate.Text, out fromDate) ||
                !DateTime.TryParse(txtToDate.Text, out toDate))
            {
                lblError.Text = "Please enter valid dates before exporting.";
                lblError.Visible = true;
                return;
            }

            DataTable dt = GetRefreshmentData(selectedRegionId, selectedBranchId, selectedEmployeeName, fromDate, toDate);

            if (dt.Rows.Count == 0)
            {
                lblError.Text = "No data to export.";
                lblError.Visible = true;
                return;
            }

            // Export to Excel
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=RefreshmentReport.xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    // Create a temporary GridView for export
                    GridView gvExport = new GridView();
                    gvExport.DataSource = dt;
                    gvExport.AutoGenerateColumns = false;

                    // Add columns exactly as in UI
                    gvExport.Columns.Add(new BoundField { DataField = "Eng_Name", HeaderText = "Employee Name" });
                    gvExport.Columns.Add(new BoundField { DataField = "Date", HeaderText = "Date", DataFormatString = "{0:dd-MMM-yyyy}" });
                    gvExport.Columns.Add(new BoundField { DataField = "FromTime", HeaderText = "From Time" });
                    gvExport.Columns.Add(new BoundField { DataField = "ToTime", HeaderText = "To Time" });
                    gvExport.Columns.Add(new BoundField { DataField = "Particulars", HeaderText = "Particulars" });
                    gvExport.Columns.Add(new BoundField { DataField = "Distance", HeaderText = "Distance" });
                    gvExport.Columns.Add(new BoundField { DataField = "Transport", HeaderText = "Mode of Transport" });
                    gvExport.Columns.Add(new BoundField { DataField = "Conveyance", HeaderText = "Conveyance" });
                    gvExport.Columns.Add(new BoundField { DataField = "Lodging", HeaderText = "Lodging" });
                    gvExport.Columns.Add(new BoundField { DataField = "Food", HeaderText = "Fooding Exp" });
                    gvExport.Columns.Add(new BoundField { DataField = "TrainBus", HeaderText = "Train/Bus Fair" });
                    gvExport.Columns.Add(new BoundField { DataField = "Others", HeaderText = "Others" });
                    gvExport.Columns.Add(new BoundField { DataField = "Miscellaneous", HeaderText = "Misc." });
                    gvExport.Columns.Add(new BoundField { DataField = "Total", HeaderText = "Total" });
                    gvExport.Columns.Add(new BoundField { DataField = "Department", HeaderText = "Department" });
                    gvExport.Columns.Add(new BoundField { DataField = "Nature_of_Work", HeaderText = "Nature of Work" });
                    gvExport.Columns.Add(new BoundField { DataField = "SMO", HeaderText = "SMO/SO/WBS" });
                    gvExport.Columns.Add(new BoundField { DataField = "Document_Reference", HeaderText = "Doc Ref" });

                    gvExport.RowDataBound += (s, ev) =>
                    {
                        if (ev.Row.RowType == DataControlRowType.Header || ev.Row.RowType == DataControlRowType.DataRow)
                        {
                            ev.Row.Font.Size = FontUnit.Point(11);
                            ev.Row.HorizontalAlign = HorizontalAlign.Center;
                        }
                    };

                    gvExport.DataBind();
                    gvExport.RenderControl(hw);

                    Response.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required to avoid the runtime error "Control 'GridView1' of type 'GridView' must be placed inside a form tag with runat=server."
        }
    }
}