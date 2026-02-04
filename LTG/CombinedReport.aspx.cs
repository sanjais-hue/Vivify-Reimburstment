using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGrease.Activities;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using System.Globalization;
using OfficeOpenXml.Style;
using ClosedXML.Excel;
using System.Web;


namespace Vivify
{
    public partial class CombinedReport : Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRegions(); // Load all regions initially
                LoadBranches(""); // Load all branches initially
            }
        }

        // Load all regions when the page loads
        private void LoadRegions()
        {
            ddlRegion.Items.Clear();

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

            // Bind the actual regions
            if (regionTable.Rows.Count > 0)
            {
                ddlRegion.DataSource = regionTable;
                ddlRegion.DataTextField = "Region";
                ddlRegion.DataValueField = "Region";
                ddlRegion.DataBind();
            }

            // ✅ Now add "All" at the top
            ddlRegion.Items.Insert(0, new ListItem("All", "All"));

            // ✅ Set "All" as the default selected value
            ddlRegion.SelectedValue = "All";
        }        // Load branches based on the selected region
        protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBranches(ddlRegion.SelectedValue);
        }

        private void LoadBranches(string regionName)
        {
            ddlBranch.Items.Clear();
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = "SELECT DISTINCT BranchName FROM Branch";

            if (!string.IsNullOrEmpty(regionName) && regionName != "All")
            {
                query += " WHERE RegionId IN (SELECT RegionId FROM Region WHERE Region = @Region)";
            }

            DataTable branchTable = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
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

            if (branchTable.Rows.Count > 0)
            {
                ddlBranch.DataSource = branchTable;
                ddlBranch.DataTextField = "BranchName";
                ddlBranch.DataValueField = "BranchName";
                ddlBranch.DataBind();
            }
            ddlBranch.Items.Insert(0, new ListItem("All", "All"));
        }
        // Fetch Region based on selected Branch
        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlBranch.SelectedValue != "All")
            {
                GetRegionForBranch(ddlBranch.SelectedValue);
            }
        }

        private void GetRegionForBranch(string branchName)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"SELECT DISTINCT R.Region 
                         FROM Region R 
                         INNER JOIN Branch B ON R.RegionId = B.RegionId 
                         WHERE B.BranchName = @BranchName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@BranchName", branchName);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        ddlRegion.SelectedValue = result.ToString();
                    }
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime fromDate, toDate;
            lblError.Visible = false;

            if (!DateTime.TryParse(txtFromDate.Text, out fromDate) ||
                !DateTime.TryParse(txtToDate.Text, out toDate))
            {
                lblError.Text = "Please enter valid dates.";
                lblError.Visible = true;
                return;
            }

            if (fromDate > toDate)
            {
                lblError.Text = "The 'From Date' must be earlier than or equal to the 'To Date'.";
                lblError.Visible = true;
                return;
            }

            // ✅ CORRECT WAY: Convert "All" to null for both Region and Branch
            string selectedRegion = (ddlRegion.SelectedValue == "All") ? null : ddlRegion.SelectedValue;
            string selectedBranch = (ddlBranch.SelectedValue == "All") ? null : ddlBranch.SelectedValue;

            try
            {
                // ✅ Pass null (not "")
                DataTable expenseData = LoadExpenseData(selectedRegion, selectedBranch, fromDate, toDate);
                DataTable expenseCategories = LoadExpenseCategories(selectedRegion, selectedBranch, fromDate, toDate);
                DataTable smoData = LoadSmoData(selectedRegion, selectedBranch, fromDate, toDate);
                DataTable soData = LoadSoData(selectedRegion, selectedBranch, fromDate, toDate);
                DataTable departmentTotals = GetDepartmentTotals(selectedRegion, selectedBranch, fromDate, toDate);
                DataTable localTourSummary = GetLocalTourTotals(selectedRegion, selectedBranch, fromDate, toDate);

                if (expenseData.Rows.Count > 0 || expenseCategories.Rows.Count > 0 ||
                    smoData.Rows.Count > 0 || soData.Rows.Count > 0 ||
                    departmentTotals.Rows.Count > 0 || localTourSummary.Rows.Count > 0)
                {
                    BindGridView(gvExpenseReport, expenseData);
                    BindGridView(gvExpenseCategories, expenseCategories);
                    BindGridView(gvSmoReport, smoData);
                    BindGridView(gvSoReport, soData);
                    BindGridView(gvDepartmentTotals, departmentTotals);
                    BindGridView(gvLocalTour, localTourSummary);
                }
                else
                {
                    lblError.Text = "No data found for the selected filters.";
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error fetching data: " + ex.Message;
                lblError.Visible = true;
            }
        }
        private void BindGridView(GridView gridView, DataTable data)
        {
            gridView.DataSource = data;
            gridView.DataBind();
        }




        private DataTable LoadExpenseData(string region, string branch, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
    SELECT 
        e.FirstName AS EngineerName,
        ISNULL(localData.LocalExpenses, 0) AS LocalExpenses,
        ISNULL(tourData.TourExpenses, 0) AS TourExpenses,
        ISNULL(refreshData.RefreshmentAmount, 0) AS RefreshmentAmount,
        ISNULL(localData.LocalExpenses, 0) + 
        ISNULL(tourData.TourExpenses, 0) + 
        ISNULL(refreshData.RefreshmentAmount, 0) AS OverallExpenses
    FROM Employees e
    INNER JOIN Branch br ON e.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId

    LEFT JOIN (
        SELECT s.EmployeeId,
            SUM(ISNULL(c.ClaimedAmount, 0)) + 
            SUM(ISNULL(f.ClaimedAmount, 0)) + 
            SUM(ISNULL(o.ClaimedAmount, 0)) + 
            SUM(ISNULL(m.ClaimedAmount, 0)) AS LocalExpenses
        FROM Services s
        LEFT JOIN Conveyance c ON s.ServiceId = c.ServiceId AND c.ExpenseType = 'local'
        LEFT JOIN Food f ON s.ServiceId = f.ServiceId AND f.ExpenseType = 'local'
        LEFT JOIN Others o ON s.ServiceId = o.ServiceId AND o.ExpenseType = 'local'
        LEFT JOIN Miscellaneous m ON s.ServiceId = m.ServiceId AND m.ExpenseType = 'local'
        WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate
        GROUP BY s.EmployeeId
    ) localData ON e.EmployeeId = localData.EmployeeId

    LEFT JOIN (
        SELECT s.EmployeeId,
            SUM(ISNULL(c.ClaimedAmount, 0)) + 
            SUM(ISNULL(f.ClaimedAmount, 0)) + 
            SUM(ISNULL(l.ClaimedAmount, 0)) + 
            SUM(ISNULL(m.ClaimedAmount, 0)) AS TourExpenses
        FROM Services s
        LEFT JOIN Conveyance c ON s.ServiceId = c.ServiceId AND c.ExpenseType = 'tour'
        LEFT JOIN Food f ON s.ServiceId = f.ServiceId AND f.ExpenseType = 'tour'
        LEFT JOIN Lodging l ON s.ServiceId = l.ServiceId AND l.ExpenseType = 'tour'
        LEFT JOIN Miscellaneous m ON s.ServiceId = m.ServiceId AND m.ExpenseType = 'tour'
        WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate
        GROUP BY s.EmployeeId
    ) tourData ON e.EmployeeId = tourData.EmployeeId

    LEFT JOIN (
        SELECT EmployeeId, SUM(ISNULL(RefreshAmount, 0)) AS RefreshmentAmount
        FROM Refreshment
        WHERE FromDate >= @FromDate AND ToDate <= @ToDate
        GROUP BY EmployeeId
    ) refreshData ON e.EmployeeId = refreshData.EmployeeId

    -- ✅ CRITICAL: Filter out all-zero rows HERE
    WHERE (@Region IS NULL OR r.Region = @Region)
      AND (@BranchName IS NULL OR e.BranchName = @BranchName)
      AND (
          ISNULL(localData.LocalExpenses, 0) > 0 
          OR ISNULL(tourData.TourExpenses, 0) > 0 
          OR ISNULL(refreshData.RefreshmentAmount, 0) > 0
      )

    ORDER BY e.FirstName;
";
            DataTable dataTable = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Region", string.IsNullOrEmpty(region) ? (object)DBNull.Value : region);
                cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(branch) ? (object)DBNull.Value : branch);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dataTable);
                }
            }

            // Add Grand Total Row
            if (dataTable.Rows.Count > 0)
            {
                DataRow totalRow = dataTable.NewRow();
                totalRow["EngineerName"] = "Total";
                totalRow["LocalExpenses"] = dataTable.AsEnumerable().Sum(r => r.Field<decimal>("LocalExpenses"));
                totalRow["TourExpenses"] = dataTable.AsEnumerable().Sum(r => r.Field<decimal>("TourExpenses"));
                totalRow["RefreshmentAmount"] = dataTable.AsEnumerable().Sum(r => r.Field<decimal>("RefreshmentAmount"));
                totalRow["OverallExpenses"] = dataTable.AsEnumerable().Sum(r => r.Field<decimal>("OverallExpenses"));
                dataTable.Rows.Add(totalRow);
            }

            return dataTable;
        }
        private DataTable LoadSmoData(string region, string branch, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            string query = @"
WITH CombinedData AS (
    SELECT conv.SmoNo, conv.SoNo, ISNULL(conv.ClaimedAmount, 0) AS ClaimedAmount
    FROM Conveyance conv
    INNER JOIN Services srv ON conv.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)

    UNION ALL

    SELECT food.SmoNo, food.SoNo, ISNULL(food.ClaimedAmount, 0)
    FROM Food food
    INNER JOIN Services srv ON food.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)

    UNION ALL

    SELECT lodg.SmoNo, lodg.SoNo, ISNULL(lodg.ClaimedAmount, 0)
    FROM Lodging lodg
    INNER JOIN Services srv ON lodg.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)

    UNION ALL

    SELECT misc.SmoNo, misc.SoNo, ISNULL(misc.ClaimedAmount, 0)
    FROM Miscellaneous misc
    INNER JOIN Services srv ON misc.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)

    UNION ALL

    SELECT oth.SmoNo, oth.SoNo, ISNULL(oth.ClaimedAmount, 0)
    FROM Others oth
    INNER JOIN Services srv ON oth.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)
)
SELECT 
    SmoNo,
    SUM(ClaimedAmount) AS TotalClaimedAmount
FROM CombinedData
WHERE SmoNo IS NOT NULL AND LTRIM(RTRIM(SmoNo)) <> ''
GROUP BY SmoNo
HAVING SUM(ClaimedAmount) > 0
ORDER BY SmoNo;
";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrEmpty(region) ? (object)DBNull.Value : region);
                    cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(branch) ? (object)DBNull.Value : branch);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            // Add Total Row
            DataRow totalRow = dt.NewRow();
            totalRow["SmoNo"] = "Total";
            totalRow["TotalClaimedAmount"] = dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalClaimedAmount"));
            totalRow["TotalClaimedAmount"] = Math.Round(Convert.ToDecimal(totalRow["TotalClaimedAmount"]), 2);
            dt.Rows.Add(totalRow);

            return dt;
        }

        private DataTable LoadSoData(string region, string branch, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            string query = @"
WITH CombinedData AS (
    SELECT conv.SmoNo, conv.SoNo, ISNULL(conv.ClaimedAmount, 0) AS ClaimedAmount
    FROM Conveyance conv
    INNER JOIN Services srv ON conv.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)

    UNION ALL

    SELECT food.SmoNo, food.SoNo, ISNULL(food.ClaimedAmount, 0)
    FROM Food food
    INNER JOIN Services srv ON food.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)

    UNION ALL

    SELECT lodg.SmoNo, lodg.SoNo, ISNULL(lodg.ClaimedAmount, 0)
    FROM Lodging lodg
    INNER JOIN Services srv ON lodg.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)

    UNION ALL

    SELECT misc.SmoNo, misc.SoNo, ISNULL(misc.ClaimedAmount, 0)
    FROM Miscellaneous misc
    INNER JOIN Services srv ON misc.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)

    UNION ALL

    SELECT oth.SmoNo, oth.SoNo, ISNULL(oth.ClaimedAmount, 0)
    FROM Others oth
    INNER JOIN Services srv ON oth.ServiceId = srv.ServiceId
    INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
    INNER JOIN Branch br ON emp.BranchName = br.BranchName
    INNER JOIN Region r ON br.RegionId = r.RegionId
    WHERE srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate
      AND (@BranchName IS NULL OR emp.BranchName = @BranchName)
      AND (@Region IS NULL OR r.Region = @Region)
)
SELECT 
    SoNo,
    SUM(ClaimedAmount) AS TotalClaimedAmount
FROM CombinedData
WHERE SoNo IS NOT NULL AND LTRIM(RTRIM(SoNo)) <> ''
GROUP BY SoNo
HAVING SUM(ClaimedAmount) > 0
ORDER BY SoNo;
";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrEmpty(region) ? (object)DBNull.Value : region);
                    cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(branch) ? (object)DBNull.Value : branch);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            // Add Total Row
            DataRow totalRow = dt.NewRow();
            totalRow["SoNo"] = "Total";
            totalRow["TotalClaimedAmount"] = dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalClaimedAmount"));
            totalRow["TotalClaimedAmount"] = Math.Round(Convert.ToDecimal(totalRow["TotalClaimedAmount"]), 2);
            dt.Rows.Add(totalRow);

            return dt;
        }

        //protected void gvSmoSoReport_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        // Check if this is the "Total" row
        //        if (e.Row.Cells[0].Text == "Total")
        //        {
        //            // Apply bold style to all cells in the "Total" row
        //            foreach (TableCell cell in e.Row.Cells)
        //            {
        //                cell.Font.Bold = true;
        //            }
        //        }
        //    }
        //}

        protected void gvLocalTour_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get LocalExpenses and TourExpenses values directly from the database (no rounding)
                decimal localExpenses = Convert.ToDecimal(e.Row.Cells[1].Text);
                decimal tourExpenses = Convert.ToDecimal(e.Row.Cells[2].Text);

                // Get the TotalExpense which is sum of Local and Tour Expenses
                decimal totalAmount = localExpenses + tourExpenses;

                // Display Local and Tour expenses exactly without rounding
                e.Row.Cells[1].Text = localExpenses.ToString("0.00", CultureInfo.InvariantCulture);
                e.Row.Cells[2].Text = tourExpenses.ToString("0.00", CultureInfo.InvariantCulture);

                // Round the total sum only when necessary (e.g., .50 should round up)
                if (totalAmount % 1 == 0.50m)
                {
                    totalAmount = Math.Ceiling(totalAmount);  // Rounds up if it's .50
                }

                // Display the TotalExpense
                e.Row.Cells[3].Text = totalAmount.ToString("0.00", CultureInfo.InvariantCulture);
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                // Ensure no rounding on footer (total row) values.
                decimal totalLocal = 0;
                decimal totalTour = 0;

                // Sum the values from the data rows
                foreach (GridViewRow row in gvLocalTour.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        totalLocal += Convert.ToDecimal(row.Cells[1].Text);
                        totalTour += Convert.ToDecimal(row.Cells[2].Text);
                    }
                }

                decimal grandTotal = totalLocal + totalTour;

                // Apply rounding on grand total (if necessary)
                if (grandTotal % 1 == 0.50m)
                {
                    grandTotal = Math.Ceiling(grandTotal);
                }

                // Set the footer values for total row
                e.Row.Cells[1].Text = totalLocal.ToString("0.00", CultureInfo.InvariantCulture);
                e.Row.Cells[2].Text = totalTour.ToString("0.00", CultureInfo.InvariantCulture);
                e.Row.Cells[3].Text = grandTotal.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }

        protected DataTable GetLocalTourTotals(string region, string branchName, DateTime fromDate, DateTime toDate)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            string query = @"
WITH LocalTourTotals AS (
    SELECT 
        ExpenseCategory,
        SUM(CAST(ClaimedAmount AS DECIMAL(18, 2))) AS TotalAmount
    FROM (
        -- Conveyance table (Local and Tour ExpenseType)
        SELECT 
            CASE 
                WHEN conv.ExpenseType = 'Local' THEN 'Local'
                WHEN conv.ExpenseType = 'Tour' THEN 'Tour'
            END AS ExpenseCategory,
            ISNULL(conv.ClaimedAmount, 0) AS ClaimedAmount
        FROM 
            Conveyance conv
        INNER JOIN 
            Services s ON conv.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        LEFT JOIN 
            Branch br ON e.BranchName = br.BranchName
        LEFT JOIN 
            Region r ON br.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)
            AND (conv.ExpenseType = 'Local' OR conv.ExpenseType = 'Tour')

        UNION ALL

        -- Food table (Local and Tour ExpenseType)
        SELECT 
            CASE 
                WHEN food.ExpenseType = 'Local' THEN 'Local'
                WHEN food.ExpenseType = 'Tour' THEN 'Tour'
            END AS ExpenseCategory,
            ISNULL(food.ClaimedAmount, 0) AS ClaimedAmount
        FROM 
            Food food
        INNER JOIN 
            Services s ON food.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        LEFT JOIN 
            Branch br ON e.BranchName = br.BranchName
        LEFT JOIN 
            Region r ON br.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)
            AND (food.ExpenseType = 'Local' OR food.ExpenseType = 'Tour')

        UNION ALL

        -- Others table (Only Local expense type for Others)
        SELECT 
            'Local' AS ExpenseCategory,  
            ISNULL(other.ClaimedAmount, 0) AS ClaimedAmount
        FROM 
            Others other
        INNER JOIN 
            Services s ON other.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        LEFT JOIN 
            Branch br ON e.BranchName = br.BranchName
        LEFT JOIN 
            Region r ON br.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)
            AND other.ExpenseType = 'Local' 

        UNION ALL

        -- Lodging table (Only Tour expense type for Lodging)
        SELECT 
            'Tour' AS ExpenseCategory,  
            ISNULL(lodge.ClaimedAmount, 0) AS ClaimedAmount
        FROM 
            Lodging lodge
        INNER JOIN 
            Services s ON lodge.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        LEFT JOIN 
            Branch br ON e.BranchName = br.BranchName
        LEFT JOIN 
            Region r ON br.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)
            AND lodge.ExpenseType = 'Tour' 

        UNION ALL

        -- Miscellaneous table (Local and Tour ExpenseType)
        SELECT 
            CASE 
                WHEN misc.ExpenseType = 'Local' THEN 'Local'
                WHEN misc.ExpenseType = 'Tour' THEN 'Tour'
            END AS ExpenseCategory,
            ISNULL(misc.ClaimedAmount, 0) AS ClaimedAmount
        FROM 
            Miscellaneous misc
        INNER JOIN 
            Services s ON misc.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        LEFT JOIN 
            Branch br ON e.BranchName = br.BranchName
        LEFT JOIN 
            Region r ON br.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)
            AND (misc.ExpenseType = 'Local' OR misc.ExpenseType = 'Tour')
    ) AS AllExpenses
    GROUP BY 
        ExpenseCategory
), 

RefreshTotals AS (
    SELECT 
        'Refreshment' AS ExpenseCategory,
        SUM(CAST(ISNULL(refresh.RefreshAmount, 0) AS DECIMAL(18, 2))) AS TotalAmount
    FROM 
        Refreshment refresh
    LEFT JOIN 
            Employees e ON refresh.EmployeeId = e.EmployeeId
        LEFT JOIN 
            Branch br ON e.BranchName = br.BranchName
        LEFT JOIN 
            Region r ON br.RegionId = r.RegionId
    WHERE 
        refresh.FromDate >= @FromDate 
        AND refresh.ToDate <= @ToDate
        AND (@BranchName IS NULL OR e.BranchName = @BranchName)
        AND (@Region IS NULL OR r.Region = @Region)
), 

TotalSum AS (
    SELECT 
        'Total' AS ExpenseCategory,
        SUM(TotalAmount) AS TotalAmount
    FROM (
        SELECT TotalAmount FROM LocalTourTotals
        UNION ALL
        SELECT TotalAmount FROM RefreshTotals
    ) AS CombinedTotals
)

-- Final selection
SELECT * FROM LocalTourTotals
UNION ALL 
SELECT * FROM RefreshTotals
UNION ALL 
SELECT * FROM TotalSum;
";

            DataTable resultTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@Region", string.IsNullOrEmpty(region) ? (object)DBNull.Value : region);
                    command.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(branchName) ? (object)DBNull.Value : branchName);
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);

                    connection.Open();
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(resultTable);
                    }
                }
            }

            return resultTable;
        }

        private DataTable LoadExpenseCategories(string region, string branchName, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
    WITH ExpenseDetails AS (
        -- Conveyance expenses
        SELECT 
            'Conveyance' AS ExpenseCategory,
            SUM(ISNULL(conv.ClaimedAmount, 0)) AS TotalAmount
        FROM 
            Conveyance conv
        INNER JOIN 
            Services s ON conv.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        INNER JOIN 
            Branch b ON e.BranchName = b.BranchName
        INNER JOIN 
            Region r ON b.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)

        UNION ALL

        -- Food expenses
        SELECT 
            'Food' AS ExpenseCategory,
            SUM(ISNULL(food.ClaimedAmount, 0)) AS TotalAmount
        FROM 
            Food food
        INNER JOIN 
            Services s ON food.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        INNER JOIN 
            Branch b ON e.BranchName = b.BranchName
        INNER JOIN 
            Region r ON b.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)

        UNION ALL

        -- Others expenses (only 'Local')
        SELECT 
            'Others' AS ExpenseCategory,
            SUM(ISNULL(other.ClaimedAmount, 0)) AS TotalAmount
        FROM 
            Others other
        INNER JOIN 
            Services s ON other.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        INNER JOIN 
            Branch b ON e.BranchName = b.BranchName
        INNER JOIN 
            Region r ON b.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)

        UNION ALL

        -- Lodging expenses (only 'Tour')
        SELECT 
            'Lodging' AS ExpenseCategory,
            SUM(ISNULL(lodge.ClaimedAmount, 0)) AS TotalAmount
        FROM 
            Lodging lodge
        INNER JOIN 
            Services s ON lodge.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        INNER JOIN 
            Branch b ON e.BranchName = b.BranchName
        INNER JOIN 
            Region r ON b.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)

        UNION ALL

        -- Miscellaneous expenses
        SELECT 
            'Miscellaneous' AS ExpenseCategory,
            SUM(ISNULL(misc.ClaimedAmount, 0)) AS TotalAmount
        FROM 
            Miscellaneous misc
        INNER JOIN 
            Services s ON misc.ServiceId = s.ServiceId
        INNER JOIN 
            Employees e ON s.EmployeeId = e.EmployeeId
        INNER JOIN 
            Branch b ON e.BranchName = b.BranchName
        INNER JOIN 
            Region r ON b.RegionId = r.RegionId
        WHERE 
            s.FromDate >= @FromDate 
            AND s.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)
    ),

    RefreshTotals AS (
        -- Refreshment expenses
        SELECT 
            'Refreshment' AS ExpenseCategory,
            SUM(ISNULL(refresh.RefreshAmount, 0)) AS TotalAmount
        FROM 
            Refreshment refresh
        LEFT JOIN 
            Employees e ON refresh.EmployeeId = e.EmployeeId
        LEFT JOIN 
            Branch b ON e.BranchName = b.BranchName
        LEFT JOIN 
            Region r ON b.RegionId = r.RegionId
        WHERE 
            refresh.FromDate >= @FromDate 
            AND refresh.ToDate <= @ToDate
            AND (@BranchName IS NULL OR e.BranchName = @BranchName)
            AND (@Region IS NULL OR r.Region = @Region)
    ),

    GrandTotal AS (
        -- Calculate the grand total
        SELECT 
            'Total' AS ExpenseCategory,
            SUM(TotalAmount) AS TotalAmount
        FROM (
            SELECT TotalAmount FROM ExpenseDetails
            UNION ALL
            SELECT TotalAmount FROM RefreshTotals
        ) AS CombinedTotals
    )

    -- Final output
    SELECT * FROM ExpenseDetails
    UNION ALL
    SELECT * FROM RefreshTotals
    UNION ALL
    SELECT * FROM GrandTotal;
";

            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Correctly handle NULL for optional filters
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);
                        cmd.Parameters.AddWithValue("@Region", string.IsNullOrEmpty(region) ? (object)DBNull.Value : region);
                        cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(branchName) ? (object)DBNull.Value : branchName); // ✅ Fixed: Convert "" to NULL

                        con.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Consider logging to a file or error display instead of Console in web app
                System.Diagnostics.Debug.WriteLine("Error in LoadExpenseCategories: " + ex.Message);
            }

            return dt;
        }
        private DataTable GetDepartmentTotals(string region, string branchName, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
WITH AggregatedConveyance AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalConveyance
    FROM Conveyance
    GROUP BY ServiceID
),
AggregatedFood AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalFood
    FROM Food
    GROUP BY ServiceID
),
AggregatedMiscellaneous AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalMiscellaneous
    FROM Miscellaneous
    GROUP BY ServiceID
),
AggregatedLodging AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalLodging
    FROM Lodging
    GROUP BY ServiceID
),
AggregatedOthers AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalOthers
    FROM Others
    GROUP BY ServiceID
),

-- Step 2: Service Expenses
ServiceExpenses AS (
    SELECT 
        'Service' AS Department,
        SUM(ISNULL(ac.TotalConveyance, 0)) AS TotalConveyance,
        SUM(ISNULL(af.TotalFood, 0)) AS TotalFood,
        SUM(ISNULL(am.TotalMiscellaneous, 0)) AS TotalMiscellaneous,
        SUM(ISNULL(al.TotalLodging, 0)) AS TotalLodging,
        SUM(ISNULL(ao.TotalOthers, 0)) AS TotalOthers
    FROM Services s
    LEFT JOIN AggregatedConveyance ac ON s.ServiceID = ac.ServiceID
    LEFT JOIN AggregatedFood af ON s.ServiceID = af.ServiceID
    LEFT JOIN AggregatedMiscellaneous am ON s.ServiceID = am.ServiceID
    LEFT JOIN AggregatedLodging al ON s.ServiceID = al.ServiceID
    LEFT JOIN AggregatedOthers ao ON s.ServiceID = ao.ServiceID
    LEFT JOIN Employees e ON s.EmployeeID = e.EmployeeID
    LEFT JOIN Branch br ON e.BranchName = br.BranchName
    LEFT JOIN Region r ON br.RegionId = r.RegionId
    WHERE s.Department = 'Service'
    AND s.FromDate >= @FromDate AND s.ToDate <= @ToDate
    AND (@BranchName IS NULL OR e.BranchName = @BranchName)
    AND (@Region IS NULL OR r.Region = @Region)
),

-- Step 3: Sales Expenses
SalesExpenses AS (
    SELECT 
        'Sales' AS Department,
        SUM(ISNULL(ac.TotalConveyance, 0)) AS TotalConveyance,
        SUM(ISNULL(af.TotalFood, 0)) AS TotalFood,
        SUM(ISNULL(am.TotalMiscellaneous, 0)) AS TotalMiscellaneous,
        SUM(ISNULL(al.TotalLodging, 0)) AS TotalLodging,
        SUM(ISNULL(ao.TotalOthers, 0)) AS TotalOthers
    FROM Services s
    LEFT JOIN AggregatedConveyance ac ON s.ServiceID = ac.ServiceID
    LEFT JOIN AggregatedFood af ON s.ServiceID = af.ServiceID
    LEFT JOIN AggregatedMiscellaneous am ON s.ServiceID = am.ServiceID
    LEFT JOIN AggregatedLodging al ON s.ServiceID = al.ServiceID
    LEFT JOIN AggregatedOthers ao ON s.ServiceID = ao.ServiceID
    LEFT JOIN Employees e ON s.EmployeeID = e.EmployeeID
    LEFT JOIN Branch br ON e.BranchName = br.BranchName
    LEFT JOIN Region r ON br.RegionId = r.RegionId
    WHERE s.Department = 'Sales'
    AND s.FromDate >= @FromDate AND s.ToDate <= @ToDate
    AND (@BranchName IS NULL OR e.BranchName = @BranchName)
    AND (@Region IS NULL OR r.Region = @Region)
),

-- Step 4: Refreshment Expenses
RefreshExpenses AS (
    SELECT 
        'Refresh' AS Department,
        0 AS TotalConveyance,
        0 AS TotalFood,
        0 AS TotalMiscellaneous,
        0 AS TotalLodging,
        SUM(ISNULL(r.RefreshAmount, 0)) AS TotalOthers
    FROM Refreshment r
    LEFT JOIN Employees e ON r.EmployeeID = e.EmployeeID
    LEFT JOIN Branch br ON e.BranchName = br.BranchName
    LEFT JOIN Region re ON br.RegionId = re.RegionId
    WHERE r.FromDate >= @FromDate AND r.ToDate <= @ToDate
    AND (@BranchName IS NULL OR e.BranchName = @BranchName)
    AND (@Region IS NULL OR re.Region = @Region)
),

-- Step 5: Combine All Expenses
CombinedExpenses AS (
    SELECT * FROM ServiceExpenses
    UNION ALL
    SELECT * FROM SalesExpenses
    UNION ALL
    SELECT * FROM RefreshExpenses
)

-- Step 6: Department Totals and Grand Total
SELECT 
    Department,
    SUM(TotalConveyance) AS TotalConveyance,
    SUM(TotalFood) AS TotalFood,
    SUM(TotalOthers) AS TotalOthers,
    SUM(TotalMiscellaneous) AS TotalMiscellaneous,
    SUM(TotalLodging) AS TotalLodging,
    SUM(TotalConveyance + TotalFood + TotalOthers + TotalMiscellaneous + TotalLodging) AS TotalAmount
FROM CombinedExpenses
GROUP BY Department

UNION ALL

-- Grand Total Row
SELECT 
    'Total' AS Department,
    SUM(TotalConveyance) AS TotalConveyance,
    SUM(TotalFood) AS TotalFood,
    SUM(TotalOthers) AS TotalOthers,
    SUM(TotalMiscellaneous) AS TotalMiscellaneous,
    SUM(TotalLodging) AS TotalLodging,
    SUM(TotalConveyance + TotalFood + TotalOthers + TotalMiscellaneous + TotalLodging) AS TotalAmount
FROM CombinedExpenses;
";

            DataTable result = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Add parameters for Region, Branch, FromDate, and ToDate
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrEmpty(region) ? (object)DBNull.Value : region);
                    cmd.Parameters.AddWithValue("@BranchName", string.IsNullOrEmpty(branchName) ? (object)DBNull.Value : branchName);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(result);
                    }
                }
            }
            return result;
        }


        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            string selectedRegion = ddlRegion.SelectedValue;
            string selectedBranch = ddlBranch.SelectedValue;
            DateTime fromDate, toDate;

            lblError.Visible = false; // Clear previous error messages

            bool fromDateValid = DateTime.TryParse(txtFromDate.Text, out fromDate);
            bool toDateValid = DateTime.TryParse(txtToDate.Text, out toDate);

            if (!fromDateValid || !toDateValid)
            {
                lblError.Text = "Please enter valid dates.";
                lblError.Visible = true;
                return;
            }

            if (fromDate > toDate)
            {
                lblError.Text = "The 'From Date' must be earlier than or equal to the 'To Date'.";
                lblError.Visible = true;
                return;
            }

            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Expense Report");

                    int currentRow = 1; // Start row for data entry

                    // Export each GridView's data into the same sheet
                    AddGridViewDataToWorksheet(worksheet, gvExpenseReport, "Expense Report", ref currentRow);
                    AddGridViewDataToWorksheet(worksheet, gvExpenseCategories, "Expense Categories", ref currentRow);
                    AddGridViewDataToWorksheet(worksheet, gvSmoReport, "SMO Report", ref currentRow);     // Updated
                    AddGridViewDataToWorksheet(worksheet, gvSoReport, "SO Report", ref currentRow);
                    AddGridViewDataToWorksheet(worksheet, gvDepartmentTotals, "Department Totals", ref currentRow);
                    AddGridViewDataToWorksheet(worksheet, gvLocalTour, "Local Tour Summary", ref currentRow);

                    // Auto-fit columns for better readability
                    worksheet.Columns().AdjustToContents();

                    // Send file to the browser
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=ExpenseReport.xlsx");

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error exporting to Excel: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private void AddGridViewDataToWorksheet(IXLWorksheet worksheet, GridView gridView, string sectionTitle, ref int currentRow)
        {
            if (gridView.Rows.Count > 0)
            {
                // Add section title
                worksheet.Cell(currentRow, 1).Value = sectionTitle;
                worksheet.Range(currentRow, 1, currentRow, gridView.Columns.Count).Merge(); // Merge title across columns
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.Yellow; // Set light blue background
                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                currentRow++;

                DataTable dt = ConvertGridViewToDataTable(gridView);

                // Insert headers
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = dt.Columns[i].ColumnName;
                    worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.NoColor;
                }

                currentRow++;

                // Insert data
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string cellValue = dt.Rows[i][j].ToString().Trim();
                        worksheet.Cell(currentRow + i, j + 1).Value = cellValue == "&nbsp;" ? "" : cellValue; // Remove &nbsp;
                        worksheet.Cell(currentRow + i, j + 1).Style.Fill.BackgroundColor = XLColor.NoColor;
                    }
                }

                // Move index to next section
                currentRow += dt.Rows.Count + 2; // Add spacing
            }
        }

        private DataTable ConvertGridViewToDataTable(GridView gridView)
        {
            DataTable dt = new DataTable();

            // Add column headers
            foreach (TableCell headerCell in gridView.HeaderRow.Cells)
            {
                dt.Columns.Add(headerCell.Text);
            }

            // Add row data
            foreach (GridViewRow row in gridView.Rows)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text.Trim();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}