using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class TravelExpensePage : System.Web.UI.Page
    {
        // On Page Load, bind the GridView and the Branch dropdown list
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRegions();
                LoadBranches();
                LoadEmployeeNames();

                // Set default dates if not set
                if (string.IsNullOrEmpty(txtFromDate.Text))
                {
                    txtFromDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                }
                if (string.IsNullOrEmpty(txtToDate.Text))
                {
                    txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                }

                // Check if Session contains filter values
                if (Session["FromDate"] != null && Session["ToDate"] != null)
                {
                    string selectedRegion = Session["Region"]?.ToString() ?? "All";
                    string selectedBranch = Session["Branch"]?.ToString() ?? "All";
                    string selectedEmployee = Session["Employee"]?.ToString() ?? "All";
                    DateTime fromDate = (DateTime)Session["FromDate"];
                    DateTime toDate = (DateTime)Session["ToDate"];

                    ddlRegion.SelectedValue = selectedRegion;
                    ddlBranch.SelectedValue = selectedBranch;
                    ddlEmployee.SelectedValue = selectedEmployee;
                    txtFromDate.Text = fromDate.ToString("yyyy-MM-dd");
                    txtToDate.Text = toDate.ToString("yyyy-MM-dd");

                    BindGridView(selectedRegion, selectedBranch, selectedEmployee, fromDate, toDate);
                }
                else
                {
                    // Initial load with default filters
                    DateTime fromDate = DateTime.Now.AddMonths(-1);
                    DateTime toDate = DateTime.Now;
                    // Initial load
                    BindGridView("All", "All", "All", DateTime.Now.AddMonths(-1), DateTime.Now);
                }
            }
        }

        // Add this method to fix the compilation error
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnVerify = (Button)e.Row.FindControl("btnVerify");
                if (btnVerify != null)
                {
                    // Get status from data
                    DataRowView rowView = (DataRowView)e.Row.DataItem;
                    int maxStatus = Convert.ToInt32(rowView["MaxStatus"]);

                    if (maxStatus == 3)
                    {
                        btnVerify.Text = "Verified";
                        btnVerify.Enabled = true;
                        btnVerify.CssClass = "btn btn-primary custom-button center-button verified-btn";
                    }
                    else // Status = 1 or others (assume pending)
                    {
                        btnVerify.Text = "Click to Verify";
                        btnVerify.Enabled = true;
                        btnVerify.CssClass = "btn btn-primary custom-button center-button";
                    }
                }
            }
        }
        // Load all regions when the page loads
        private void LoadRegions()
        {
            ddlRegion.Items.Clear();
            ddlRegion.Items.Insert(0, new ListItem("Select a region", ""));
            ddlRegion.Items.Insert(1, new ListItem("All", "All"));

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = "SELECT DISTINCT Region FROM Region ORDER BY Region";

            DataTable regionTable = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(regionTable);
                    }
                }
            }

            if (regionTable.Rows.Count > 0)
            {
                ddlRegion.DataSource = regionTable;
                ddlRegion.DataTextField = "Region";
                ddlRegion.DataValueField = "Region";
                ddlRegion.DataBind();
            }
        }

        // Load branches based on the selected region
        protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBranches(ddlRegion.SelectedValue);
            ApplyFilters();
        }

        private void LoadBranches(string regionName = null)
        {
            ddlBranch.Items.Clear();
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            string query = @"SELECT DISTINCT b.BranchName 
                           FROM Branch b
                           INNER JOIN Region r ON b.RegionId = r.RegionId
                           WHERE 1=1";

            if (!string.IsNullOrEmpty(regionName) && regionName != "All")
            {
                query += " AND r.Region = @Region";
            }

            query += " ORDER BY b.BranchName";

            DataTable branchTable = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(regionName) && regionName != "All")
                    {
                        cmd.Parameters.AddWithValue("@Region", regionName);
                    }
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(branchTable);
                    }
                }
            }

            if (branchTable.Rows.Count > 0)
            {
                ddlBranch.DataSource = branchTable;
                ddlBranch.DataTextField = "BranchName";
                ddlBranch.DataValueField = "BranchName";
                ddlBranch.DataBind();
            }

            // Always insert "All" at top after binding
            ddlBranch.Items.Insert(0, new ListItem("All", "All"));
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBranch = ddlBranch.SelectedValue;
            LoadEmployeeNames(selectedBranch);
            ApplyFilters();
        }

        private void LoadEmployeeNames(string branchName = null)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"SELECT DISTINCT FirstName + ' ' + LastName AS FullName 
                   FROM Employees 
                   WHERE (@BranchName = 'All' OR BranchName = @BranchName) 
                   AND FirstName IS NOT NULL 
                   ORDER BY FullName";  // Changed from FirstName to FullName

            DataTable employeeTable = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(branchName) ? "All" : branchName);

                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(employeeTable);
                    }
                }
            }

            ddlEmployee.DataSource = employeeTable;
            ddlEmployee.DataTextField = "FullName";
            ddlEmployee.DataValueField = "FullName";
            ddlEmployee.DataBind();

            // Insert "All" at the beginning
            ddlEmployee.Items.Insert(0, new ListItem("All", "All"));
        }

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            DateTime fromDate, toDate;
            if (DateTime.TryParse(txtFromDate.Text, out fromDate) &&
                DateTime.TryParse(txtToDate.Text, out toDate))
            {
                string region = ddlRegion.SelectedValue;
                string branch = ddlBranch.SelectedValue;
                string employee = ddlEmployee.SelectedValue;

                BindGridView(region, branch, employee, fromDate, toDate);
            }
        }

        private void BindGridView(string regionName, string branchName, string employeeName, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string qry = @"
SELECT 
    MIN(te.Id) as Id,
    e.FirstName + ' ' + e.LastName AS EmployeeName,
    b.BranchName,
    FORMAT(te.Date, 'dd-MMM-yyyy') AS TravelDate,
    MAX(te.Status) as MaxStatus
FROM 
    TravelExpenses te
INNER JOIN Employees e ON te.EmployeeId = e.EmployeeId
INNER JOIN Branch b ON te.BranchId = b.BranchId
INNER JOIN Region r ON b.RegionId = r.RegionId
WHERE 
    te.Status IN (1, 3)
    AND (@Region = 'All' OR r.Region = @Region)
    AND (@BranchName = 'All' OR b.BranchName = @BranchName)
    AND (@EmployeeName = 'All' OR (e.FirstName + ' ' + e.LastName) = @EmployeeName)
    AND (te.Date BETWEEN @FromDate AND @ToDate)
GROUP BY 
    e.FirstName + ' ' + e.LastName, 
    b.BranchName, 
    te.Date
ORDER BY 
    te.Date DESC, EmployeeName";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@Region", regionName);
                    cmd.Parameters.AddWithValue("@BranchName", branchName);
                    cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                }
            }

            GridView1.Visible = GridView1.Rows.Count > 0;
            lblError.Visible = !GridView1.Visible;
            if (!GridView1.Visible)
                lblError.Text = "No records found for the selected criteria.";
        }
        private DataTable ApplyClientSideGrouping(DataTable sourceTable)
        {
            if (sourceTable.Rows.Count == 0)
                return sourceTable;

            DataTable groupedTable = sourceTable.Clone();

            // Use Dictionary to track unique employee-date combinations
            var uniqueCombinations = new Dictionary<string, DataRow>();

            foreach (DataRow row in sourceTable.Rows)
            {
                string employeeName = row["EmployeeName"].ToString();
                string travelDate = row["TravelDate"].ToString();
                string key = $"{employeeName}|{travelDate}";

                if (!uniqueCombinations.ContainsKey(key))
                {
                    uniqueCombinations[key] = row;
                }
                else
                {
                    // If duplicate found, keep the one with lower ID (or modify as needed)
                    int currentId = Convert.ToInt32(row["Id"]);
                    int existingId = Convert.ToInt32(uniqueCombinations[key]["Id"]);

                    if (currentId < existingId)
                    {
                        uniqueCombinations[key] = row;
                    }
                }
            }

            // Add unique rows to the result table
            foreach (var row in uniqueCombinations.Values)
            {
                groupedTable.ImportRow(row);
            }

            return groupedTable;
        }
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (DateTime.TryParse(txtFromDate.Text, out DateTime fromDate) &&
                DateTime.TryParse(txtToDate.Text, out DateTime toDate))
            {
                Session["Region"] = ddlRegion.SelectedValue;
                Session["Branch"] = ddlBranch.SelectedValue;
                Session["Employee"] = ddlEmployee.SelectedValue;
                Session["FromDate"] = fromDate;
                Session["ToDate"] = toDate;

                BindGridView(
                    ddlRegion.SelectedValue,
                    ddlBranch.SelectedValue,
                    ddlEmployee.SelectedValue,
                    fromDate,
                    toDate
                );
            }
            else
            {
                lblError.Text = "Please enter valid dates.";
                lblError.Visible = true;
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Verify")
            {
                string[] commandArgs = e.CommandArgument.ToString().Split('|');
                int travelExpenseId = Convert.ToInt32(commandArgs[0]);
                string employeeName = commandArgs[1];
                string travelDate = commandArgs[2];

                // Store in session
                Session["TravelExpenseId"] = travelExpenseId;
                Session["EmployeeName"] = employeeName;
                Session["TravelDate"] = travelDate;

                // Redirect to verification page
                Response.Redirect("AdminTravelVerify.aspx");
            }
        }
        // Other methods that you might not need for travel expenses
        private void FetchFromDate(int serviceId)
        {
            // Not needed for travel expenses
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Handle submission logic if needed
        }

        private void FetchExpenseTotals(int serviceId)
        {
            // Not needed for travel expenses
        }
    }
}