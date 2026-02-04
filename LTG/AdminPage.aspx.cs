using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace Vivify
{
    public partial class AdminPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRegions();
                LoadBranches(); // Load all branches initially
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
                    string selectedBranch = Session["SelectedBranch"]?.ToString();
                    string selectedEmployeeName = Session["SelectedEmployeeName"]?.ToString();
                    string selectedRegion = Session["SelectedRegion"]?.ToString();
                    DateTime fromDate = (DateTime)Session["FromDate"];
                    DateTime toDate = (DateTime)Session["ToDate"];

                    // Rebind the GridView with stored filter values
                    BindGridView(selectedBranch, selectedEmployeeName, fromDate, toDate);

                    // Restore dropdown selections and textboxes
                    if (!string.IsNullOrEmpty(selectedRegion))
                        ddlRegion.SelectedValue = selectedRegion;
                    if (!string.IsNullOrEmpty(selectedBranch))
                        ddlBranch.SelectedValue = selectedBranch;
                    if (!string.IsNullOrEmpty(selectedEmployeeName))
                        ddlEmployee.SelectedValue = selectedEmployeeName;
                    txtFromDate.Text = fromDate.ToString("yyyy-MM-dd");
                    txtToDate.Text = toDate.ToString("yyyy-MM-dd");
                }
                else
                {
                    // Initial load - don't show any data until user clicks Search
                    // Just set default dates
                    // GridView will be empty initially
                }
            }
        }

        // AJAX WebMethod for getting branches based on region
        [WebMethod]
        public static List<DropdownItem> GetBranches(string regionName)
        {
            List<DropdownItem> branches = new List<DropdownItem>();
            branches.Add(new DropdownItem { value = "All", text = "All" });

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

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(regionName) && regionName != "All")
                    {
                        cmd.Parameters.AddWithValue("@Region", regionName);
                    }
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            branches.Add(new DropdownItem
                            {
                                value = reader["BranchName"].ToString(),
                                text = reader["BranchName"].ToString()
                            });
                        }
                    }
                }
            }

            return branches;
        }

        // AJAX WebMethod for getting employees based on branch and region
        [WebMethod]
        public static List<DropdownItem> GetEmployees(string branchName, string regionName)
        {
            List<DropdownItem> employees = new List<DropdownItem>();
            employees.Add(new DropdownItem { value = "All", text = "All" });

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"SELECT DISTINCT e.FirstName 
                           FROM Employees e
                           INNER JOIN Branch b ON e.BranchName = b.BranchName
                           INNER JOIN Region r ON b.RegionId = r.RegionId
                           WHERE e.FirstName IS NOT NULL 
                           AND LEN(e.FirstName) > 0";

            if (!string.IsNullOrEmpty(branchName) && branchName != "All")
            {
                query += " AND e.BranchName = @BranchName";
            }

            if (!string.IsNullOrEmpty(regionName) && regionName != "All")
            {
                query += " AND r.Region = @Region";
            }

            query += " ORDER BY e.FirstName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(branchName) && branchName != "All")
                    {
                        cmd.Parameters.AddWithValue("@BranchName", branchName);
                    }
                    if (!string.IsNullOrEmpty(regionName) && regionName != "All")
                    {
                        cmd.Parameters.AddWithValue("@Region", regionName);
                    }

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string firstName = reader["FirstName"].ToString().Trim();
                            if (!string.IsNullOrEmpty(firstName))
                            {
                                employees.Add(new DropdownItem
                                {
                                    value = firstName,
                                    text = firstName
                                });
                            }
                        }
                    }
                }
            }

            return employees;
        }

        // Helper class for dropdown items
        public class DropdownItem
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnVerify = (Button)e.Row.FindControl("btnVerify");
                if (btnVerify != null)
                {
                    DataRowView rowView = (DataRowView)e.Row.DataItem;
                    var expenseStatus = rowView["ExpenseStatus"];

                    if (expenseStatus != DBNull.Value && expenseStatus.ToString() == "3")
                    {
                        btnVerify.Text = "Verified";
                        btnVerify.CssClass = "btn btn-primary custom-button center-button verified-btn";
                        btnVerify.Enabled = true; // Keep clickable
                        btnVerify.CommandName = "Verify"; // Same command name
                    }
                    else
                    {
                        btnVerify.Text = "Click here to Verify";
                        btnVerify.CssClass = "btn btn-primary custom-button center-button";
                        btnVerify.Enabled = true;
                        btnVerify.CommandName = "Verify";
                    }
                }
            }
        }

        private void LoadRegions()
        {
            ddlRegion.Items.Clear();
            ddlRegion.Items.Insert(0, new ListItem("All", "All"));

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
                foreach (DataRow row in regionTable.Rows)
                {
                    ddlRegion.Items.Add(new ListItem(row["Region"].ToString(), row["Region"].ToString()));
                }
            }
        }

        private void LoadBranches(string regionName = null)
        {
            ddlBranch.Items.Clear();
            ddlBranch.Items.Insert(0, new ListItem("All", "All"));

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
                foreach (DataRow row in branchTable.Rows)
                {
                    ddlBranch.Items.Add(new ListItem(row["BranchName"].ToString(), row["BranchName"].ToString()));
                }
            }
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBranch = ddlBranch.SelectedValue;

            // Store selected branch in session
            Session["SelectedBranch"] = selectedBranch;

            // Load employees based on selected branch
            LoadEmployeeNames(selectedBranch);
        }

        private void LoadEmployeeNames(string branchName = null)
        {
            ddlEmployee.Items.Clear();
            ddlEmployee.Items.Insert(0, new ListItem("All", "All"));

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"SELECT DISTINCT FirstName FROM Employees 
                   WHERE (@BranchName = 'All' OR BranchName = @BranchName) 
                   AND FirstName IS NOT NULL 
                   AND LEN(FirstName) > 0
                   ORDER BY FirstName";

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

            if (employeeTable.Rows.Count > 0)
            {
                foreach (DataRow row in employeeTable.Rows)
                {
                    string firstName = row["FirstName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(firstName))
                    {
                        ddlEmployee.Items.Add(new ListItem(firstName, firstName));
                    }
                }
            }
        }

        private bool ValidateEmployeeInBranchRegion(string employeeName, string branchName, string regionName)
        {
            if (employeeName == "All" || branchName == "All" || regionName == "All")
                return true;

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
        SELECT COUNT(*) 
        FROM Employees e
        INNER JOIN Branch b ON e.BranchName = b.BranchName
        INNER JOIN Region r ON b.RegionId = r.RegionId
        WHERE e.FirstName = @EmployeeName 
        AND e.BranchName = @BranchName
        AND r.Region = @Region";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                cmd.Parameters.AddWithValue("@BranchName", branchName);
                cmd.Parameters.AddWithValue("@Region", regionName);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void ApplyFilters()
        {
            DateTime fromDate, toDate;
            if (DateTime.TryParse(txtFromDate.Text, out fromDate) && DateTime.TryParse(txtToDate.Text, out toDate))
            {
                BindGridView(ddlBranch.SelectedValue, ddlEmployee.SelectedValue, fromDate, toDate);
            }
        }

        private void BindGridView(string branchName, string employeeName, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string qry = @"
      SELECT 
    s.ServiceId, 
    e.FirstName, 
    exp.ClaimedAmount, 
    s.Advance,
    FORMAT(s.FromDate, 'dd-MMM-yyyy') AS FormattedFromDate,
    ISNULL(exp.StatusId, s.StatusId) AS ExpenseStatus,   -- Use expense status if available, otherwise service status
    s.CustomerId,
    e.BranchName,
    r.Region
FROM 
    Services s
INNER JOIN 
    Employees e ON s.EmployeeId = e.EmployeeId
INNER JOIN 
    Branch b ON e.BranchName = b.BranchName
INNER JOIN 
    Region r ON b.RegionId = r.RegionId
LEFT JOIN 
    Expense exp ON s.ServiceId = exp.ServiceId
WHERE 
    (s.FromDate BETWEEN @FromDate AND @ToDate)
    AND (s.StatusId IN (2, 3))  -- Include both pending (2) and verified (3) statuses
    AND (@Region = 'All' OR r.Region = @Region)
    AND (@BranchName = 'All' OR e.BranchName = @BranchName)
    AND (@EmployeeName = 'All' OR e.FirstName = @EmployeeName)
ORDER BY 
    s.FromDate DESC";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    string selectedRegion = ddlRegion.SelectedValue;

                    cmd.Parameters.AddWithValue("@BranchName", branchName);
                    cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@Region", selectedRegion);

                    // Debug: Show what filters are being applied
                    System.Diagnostics.Debug.WriteLine($"Filters - Region: {selectedRegion}, Branch: {branchName}, Employee: {employeeName}, From: {fromDate}, To: {toDate}");

                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Debug: Show how many rows returned
                        System.Diagnostics.Debug.WriteLine($"Rows returned: {dt.Rows.Count}");

                        if (dt.Rows.Count > 0)
                        {
                            // Debug: Show first few rows data
                            for (int i = 0; i < Math.Min(3, dt.Rows.Count); i++)
                            {
                                DataRow row = dt.Rows[i];
                                System.Diagnostics.Debug.WriteLine($"Row {i}: Employee: {row["FirstName"]}, Branch: {row["BranchName"]}, Region: {row["Region"]}");
                            }
                        }

                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                }
            }

            GridView1.Visible = GridView1.Rows.Count > 0;
        }

        protected string GetCustomerNamesFromIds(string customerIds)
        {
            if (string.IsNullOrEmpty(customerIds))
                return string.Empty;

            List<string> names = new List<string>();
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            // Split the comma-separated IDs
            string[] ids = customerIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (ids.Length == 0)
                return string.Empty;

            // Create parameterized query
            var parameters = new List<string>();
            var sqlParams = new List<SqlParameter>();

            for (int i = 0; i < ids.Length; i++)
            {
                parameters.Add($"@id{i}");
                sqlParams.Add(new SqlParameter($"@id{i}", ids[i].Trim()));
            }

            string query = $"SELECT CustomerName FROM Customers WHERE CustomerId IN ({string.Join(",", parameters)})";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddRange(sqlParams.ToArray());

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        names.Add(reader["CustomerName"].ToString());
                    }
                }
            }

            return string.Join(", ", names);
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedBranch = ddlBranch.SelectedValue;
            string selectedEmployeeName = ddlEmployee.SelectedValue;
            string selectedRegion = ddlRegion.SelectedValue;
            DateTime fromDate, toDate;

            bool fromDateValid = DateTime.TryParse(txtFromDate.Text, out fromDate);
            bool toDateValid = DateTime.TryParse(txtToDate.Text, out toDate);

            if (fromDateValid && toDateValid)
            {
                // Validate if selected employee exists in selected branch and region
                if (selectedEmployeeName != "All" && selectedBranch != "All" && selectedRegion != "All")
                {
                    if (!ValidateEmployeeInBranchRegion(selectedEmployeeName, selectedBranch, selectedRegion))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "validationError",
                            "alert('Selected employee does not exist in the selected branch and region combination.');", true);
                        return;
                    }
                }

                // Store filter values in Session
                Session["SelectedRegion"] = selectedRegion;
                Session["SelectedBranch"] = selectedBranch;
                Session["SelectedEmployeeName"] = selectedEmployeeName;
                Session["FromDate"] = fromDate;
                Session["ToDate"] = toDate;

                // Bind the GridView with the current filters
                BindGridView(selectedBranch, selectedEmployeeName, fromDate, toDate);
            }
            else
            {
                // Show error message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dateError", "alert('Please enter valid From Date and To Date.');", true);
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Verify")
            {
                // Get the ServiceId from the CommandArgument
                int serviceId = Convert.ToInt32(e.CommandArgument);

                // Get the row that contains the button
                GridViewRow row = (GridViewRow)((Button)e.CommandSource).NamingContainer;

                // Get the employee name from the row
                string employeeName = row.Cells[0].Text; // First column contains FirstName

                // Store in session
                Session["ServiceId"] = serviceId;
                Session["EmployeeName"] = employeeName; // Add this line

                FetchExpenseTotals(serviceId);
                FetchFromDate(serviceId);
                Response.Redirect("AdminVerify.aspx");
            }
        }

        private void FetchFromDate(int serviceId)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string qry = "SELECT FORMAT(FromDate, 'dd-MMM-yyyy') FROM Services WHERE ServiceId = @ServiceId";
                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    // Store in session if needed
                    if (result != null)
                    {
                        Session["ServiceFromDate"] = result.ToString();
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Handle submission logic
        }

        private void FetchExpenseTotals(int serviceId)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string qry = @"
            SELECT 
                ConveyanceTotal,
                FoodTotal,
                MiscellaneousTotal,
                OthersTotal,
                LodgingTotal
            FROM 
                Expense WHERE ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Session["ConveyanceTotal"] = reader["ConveyanceTotal"];
                        Session["FoodTotal"] = reader["FoodTotal"];
                        Session["MiscellaneousTotal"] = reader["MiscellaneousTotal"];
                        Session["OthersTotal"] = reader["OthersTotal"];
                        Session["LodgingTotal"] = reader["LodgingTotal"];
                    }
                }
            }
        }
    }
}