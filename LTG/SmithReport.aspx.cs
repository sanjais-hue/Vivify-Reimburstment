using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.Linq;

namespace Vivify
{
    public partial class SmithReport : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRegions();
                LoadBranches("All"); // Load all branches initially

                // Set default dates: last 1 month
                if (string.IsNullOrEmpty(txtFromDate.Text))
                {
                    txtFromDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                }
                if (string.IsNullOrEmpty(txtToDate.Text))
                {
                    txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                }
            }
        }

        private void LoadRegions()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = "SELECT DISTINCT Region FROM Region WHERE Region IS NOT NULL ORDER BY Region";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            ddlRegion.DataSource = dt;
            ddlRegion.DataTextField = "Region";
            ddlRegion.DataValueField = "Region";
            ddlRegion.DataBind();
            ddlRegion.Items.Insert(0, new ListItem("All", "All"));
        }

        protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string region = ddlRegion.SelectedValue;
            if (!string.IsNullOrEmpty(region))
            {
                LoadBranches(region);
            }
            else
            {
                LoadBranches("All");
            }
            // Do NOT auto-filter here — wait for user to click "Search"
        }

        private void LoadBranches(string regionFilter)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query;

            if (regionFilter == "All" || string.IsNullOrEmpty(regionFilter))
            {
                query = "SELECT DISTINCT BranchName FROM Branch ORDER BY BranchName";
            }
            else
            {
                query = @"
            SELECT DISTINCT b.BranchName 
            FROM Branch b
            INNER JOIN Region r ON b.RegionId = r.RegionId
            WHERE r.Region = @Region
            ORDER BY b.BranchName";
            }

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (regionFilter != "All" && !string.IsNullOrEmpty(regionFilter))
                {
                    cmd.Parameters.AddWithValue("@Region", regionFilter);
                }
                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            ddlBranch.DataSource = dt;
            ddlBranch.DataTextField = "BranchName";
            ddlBranch.DataValueField = "BranchName";
            ddlBranch.DataBind();
            ddlBranch.Items.Insert(0, new ListItem("All", "All")); // Or "-- Select Branch --"
        }
        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: You can leave this empty if no real-time action is needed.
            // But it must exist to avoid CS1061 error.
        }
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedRegion = ddlRegion.SelectedValue;   // e.g., "All" or "North"
            string selectedBranch = ddlBranch.SelectedValue;   // e.g., "All" or "Chennai"

            DateTime fromDate, toDate;
            if (!DateTime.TryParse(txtFromDate.Text, out fromDate) ||
                !DateTime.TryParse(txtToDate.Text, out toDate))
            {
                lblError.Text = "Please enter valid dates.";
                lblError.Visible = true;
                return;
            }
            if (fromDate > toDate)
            {
                lblError.Text = "From Date must be earlier than or equal to To Date.";
                lblError.Visible = true;
                return;
            }

            try
            {
                DataTable smoSoData = LoadSmoSoData(selectedRegion, selectedBranch, fromDate, toDate);
                DataTable departmentTotals = GetDepartmentTotals(selectedRegion, selectedBranch, fromDate, toDate);
                DataTable overallTotals = GetOverallTotals(selectedRegion, selectedBranch, fromDate, toDate);

                bool hasData = smoSoData.Rows.Count > 0 ||
                               departmentTotals.Rows.Count > 0 ||
                               overallTotals.Rows.Count > 0;

                if (hasData)
                {
                    gvSmoSoReport.DataSource = smoSoData;
                    gvSmoSoReport.DataBind();
                    gvDepartmentTotals.DataSource = departmentTotals;
                    gvDepartmentTotals.DataBind();
                    gvOverallTotals.DataSource = overallTotals;
                    gvOverallTotals.DataBind();
                }
                else
                {
                    lblError.Text = "No data found for the selected filters.";
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void gvDepartmentTotals_DataBound(object sender, EventArgs e)
        {
            if (gvDepartmentTotals.Rows.Count > 0)
            {
                string previousMonth = string.Empty;
                int currentRowSpan = 1;

                // Variable to calculate the total WithoutGSTAmount
                decimal totalWithoutGSTAmount = 0;

                // Iterate through the rows of the GridView
                for (int i = 0; i < gvDepartmentTotals.Rows.Count; i++)
                {
                    // Get the current row's Month value
                    TableCell currentMonthCell = gvDepartmentTotals.Rows[i].Cells[1]; // Index 1 corresponds to the Month column
                    string currentMonth = currentMonthCell.Text;

                    // Parse the "Without GST Amount" column value and add to the total
                    if (decimal.TryParse(gvDepartmentTotals.Rows[i].Cells[8].Text.Replace(",", ""), out decimal withoutGSTAmount))
                    {
                        totalWithoutGSTAmount += withoutGSTAmount;
                    }

                    // Handle rowspan logic for the Month column
                    if (currentMonth == previousMonth)
                    {
                        // If the current Month matches the previous one, increment the rowspan
                        currentRowSpan++;
                        currentMonthCell.Visible = false; // Hide the current cell
                    }
                    else
                    {
                        // If the current Month is different from the previous one, apply the rowspan to the previous group
                        if (i > 0 && currentRowSpan > 1)
                        {
                            gvDepartmentTotals.Rows[i - currentRowSpan].Cells[1].RowSpan = currentRowSpan;
                        }
                        // Reset for the new group
                        currentRowSpan = 1;
                        previousMonth = currentMonth;
                    }
                }

                // Apply rowspan to the last group
                if (currentRowSpan > 1)
                {
                    gvDepartmentTotals.Rows[gvDepartmentTotals.Rows.Count - currentRowSpan].Cells[1].RowSpan = currentRowSpan;
                }

                // Add a header row above the existing header
                GridViewRow topHeaderRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                //string topHeaderMessage = $"Local & Tour Expenses of Southern Region";
                //TableCell topHeaderCell = new TableCell
                //{
                //    ColumnSpan = gvDepartmentTotals.Columns.Count, // Span across all columns
                //    Text = topHeaderMessage,
                //    CssClass = "top-header-cell" // Optional: Add CSS class for styling
                //};
                //topHeaderRow.Cells.Add(topHeaderCell);
                //gvDepartmentTotals.Controls[0].Controls.AddAt(0, topHeaderRow); // Insert at the top

                // Add a footer row to display the custom message and total
                GridViewRow footerRow = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);

                // Create cells for the footer row
                string customMessage = $"Total"; // Custom message for the footer
                TableCell messageCell = new TableCell { ColumnSpan = 8, Text = customMessage }; // Span across 7 columns (adjust as needed)
                TableCell totalCell = new TableCell { Text = totalWithoutGSTAmount.ToString("N2") }; // Total Without GST Amount formatted to 2 decimal places

                // Add cells to the footer row
                footerRow.Cells.Add(messageCell); // Custom message spans 7 columns
                footerRow.Cells.Add(totalCell);   // Total Without GST Amount

                // Add the footer row to the GridView
                gvDepartmentTotals.Controls[0].Controls.Add(footerRow);
            }
        }
        private DataTable GetDepartmentTotals(string region, string branchName, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            // Build dynamic parts
            string mainBranchJoinAndFilter = "";
            string refreshJoinAndFilter = "";
            string awardJoinAndFilter = "";

            if (branchName != "All")
            {
                // Filter by branch only
                mainBranchJoinAndFilter = "AND b.BranchName = @BranchName";
                refreshJoinAndFilter = "AND e.BranchName = @BranchName";
                awardJoinAndFilter = "AND b.BranchName = @BranchName";
            }
            else if (region != "All")
            {
                // Join with Region and filter
                mainBranchJoinAndFilter = @"
            INNER JOIN Region rMain ON b.RegionID = rMain.RegionID
            AND rMain.Region = @Region";

                // For Refresh: Employees → Branch → Region
                refreshJoinAndFilter = @"
            INNER JOIN Branch brEmp ON e.BranchID = brEmp.BranchID
            INNER JOIN Region rEmp ON brEmp.RegionID = rEmp.RegionID
            AND rEmp.Region = @Region";

                // For Award: Branch → Region
                awardJoinAndFilter = @"
            INNER JOIN Region rAward ON b.RegionID = rAward.RegionID
            AND rAward.Region = @Region";
            }

            string query = $@"
WITH AggregatedConveyance AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalConveyance FROM Conveyance GROUP BY ServiceID
),
AggregatedFood AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalFood FROM Food GROUP BY ServiceID
),
AggregatedMiscellaneous AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalMiscellaneous FROM Miscellaneous GROUP BY ServiceID
),
AggregatedLodging AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalLodging FROM Lodging GROUP BY ServiceID
),
AggregatedOthers AS (
    SELECT ServiceID, SUM(ISNULL(ClaimedAmount, 0)) AS TotalOthers FROM Others GROUP BY ServiceID
),
DepartmentExpenses AS (
    SELECT 
        b.BranchName,
        s.Department,
        s.ServiceID,
        ISNULL(ac.TotalConveyance, 0) AS TotalConveyance,
        ISNULL(af.TotalFood, 0) AS TotalFood,
        ISNULL(am.TotalMiscellaneous, 0) AS TotalMiscellaneous,
        ISNULL(al.TotalLodging, 0) AS TotalLodging,
        ISNULL(ao.TotalOthers, 0) AS TotalOthers
    FROM Services s
    INNER JOIN Branch b ON s.BranchID = b.BranchID
    {mainBranchJoinAndFilter}
    LEFT JOIN AggregatedConveyance ac ON s.ServiceID = ac.ServiceID
    LEFT JOIN AggregatedFood af ON s.ServiceID = af.ServiceID
    LEFT JOIN AggregatedMiscellaneous am ON s.ServiceID = am.ServiceID
    LEFT JOIN AggregatedLodging al ON s.ServiceID = al.ServiceID
    LEFT JOIN AggregatedOthers ao ON s.ServiceID = ao.ServiceID
    WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate
),
RefreshExpenses AS (
    SELECT 
        e.BranchName,
        SUM(ISNULL(r.RefreshAmount, 0)) AS RefreshTotal
    FROM Refreshment r
    INNER JOIN Employees e ON r.EmployeeID = e.EmployeeID
    {(branchName != "All" ? refreshJoinAndFilter : refreshJoinAndFilter)}
    WHERE r.FromDate >= @FromDate AND r.ToDate <= @ToDate
    GROUP BY e.BranchName
),
AwardExpenses AS (
    SELECT 
        b.BranchName,
        SUM(ISNULL(a.Amount, 0)) AS AwardTotal
    FROM Award a
    INNER JOIN Services s ON a.ServiceID = s.ServiceID
    INNER JOIN Branch b ON s.BranchID = b.BranchID
    {(branchName != "All" ? "" : awardJoinAndFilter)}
    WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate
    {(branchName != "All" ? awardJoinAndFilter : "")}
    GROUP BY b.BranchName
)
SELECT 
    ROW_NUMBER() OVER (ORDER BY de.BranchName) AS SNo,
    CASE 
        WHEN FORMAT(@FromDate, 'yyyyMM') = FORMAT(@ToDate, 'yyyyMM') THEN FORMAT(@FromDate, 'MMM-yy')
        ELSE CONCAT(FORMAT(@FromDate, 'MMM-yy'), ' - ', FORMAT(@ToDate, 'MMM-yy'))
    END AS Month,
    de.BranchName,
    SUM(CASE WHEN de.Department = 'Service' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) + ISNULL(re.RefreshTotal, 0) AS ServiceTotal,
    SUM(CASE WHEN de.Department = 'Sales' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) AS SalesTotal,
    ISNULL(ae.AwardTotal, 0) AS AwardTotal,
    SUM(CASE WHEN de.Department = 'Service' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) 
        + SUM(CASE WHEN de.Department = 'Sales' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) 
        + ISNULL(re.RefreshTotal, 0) + ISNULL(ae.AwardTotal, 0) AS OverallTotal,
    ISNULL((SUM(CASE WHEN de.Department = 'Service' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) 
        + SUM(CASE WHEN de.Department = 'Sales' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) 
        + ISNULL(re.RefreshTotal, 0)) * 0.05, 0) AS HandlingCharge,
    (SUM(CASE WHEN de.Department = 'Service' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) 
        + SUM(CASE WHEN de.Department = 'Sales' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) 
        + ISNULL(re.RefreshTotal, 0) + ISNULL(ae.AwardTotal, 0)) 
    + ISNULL((SUM(CASE WHEN de.Department = 'Service' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) 
        + SUM(CASE WHEN de.Department = 'Sales' THEN de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers ELSE 0 END) 
        + ISNULL(re.RefreshTotal, 0)) * 0.05, 0) AS WithoutGSTAmount
FROM DepartmentExpenses de
LEFT JOIN RefreshExpenses re ON de.BranchName = re.BranchName
LEFT JOIN AwardExpenses ae ON de.BranchName = ae.BranchName
GROUP BY de.BranchName, re.RefreshTotal, ae.AwardTotal;";

            DataTable result = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                if (branchName != "All")
                    cmd.Parameters.AddWithValue("@BranchName", branchName);
                else if (region != "All")
                    cmd.Parameters.AddWithValue("@Region", region);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(result);
            }
            return result;
        }
        protected void gvOverallTotals_DataBound(object sender, EventArgs e)
        {
            if (gvOverallTotals.Rows.Count > 0)
            {
                decimal totalExpenseAmount = 0;
                decimal totalHandlingCharge = 0;
                decimal totalWithoutGST = 0;
                decimal totalGST = 0;
                decimal totalWithGST = 0;

                // Calculate totals and skip empty rows
                foreach (GridViewRow row in gvOverallTotals.Rows)
                {
                    // Parse cell values from the GridView cells
                    if (decimal.TryParse(row.Cells[2].Text.Replace(",", ""), out decimal expenseAmount) &&
                        decimal.TryParse(row.Cells[3].Text.Replace(",", ""), out decimal handlingCharge) &&
                        decimal.TryParse(row.Cells[4].Text.Replace(",", ""), out decimal withoutGST) &&
                        decimal.TryParse(row.Cells[5].Text.Replace(",", ""), out decimal gst) &&
                        decimal.TryParse(row.Cells[6].Text.Replace(",", ""), out decimal withGST))
                    {
                        // Skip rows where all values are zero
                        if (expenseAmount == 0 && handlingCharge == 0 && withoutGST == 0 && gst == 0 && withGST == 0)
                        {
                            continue; // Skip this row
                        }

                        // Add non-zero values to totals
                        totalExpenseAmount += expenseAmount;
                        totalHandlingCharge += handlingCharge;
                        totalWithoutGST += withoutGST;
                        totalGST += gst;
                        totalWithGST += withGST;
                    }
                }

                // Add footer row for grand totals
                GridViewRow footerRow = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);

                // Add cells for the footer row
                footerRow.Cells.Add(new TableCell { Text = "", ColumnSpan = 1 }); // SNo column (empty)
                footerRow.Cells.Add(new TableCell { Text = "Grand Total", ColumnSpan = 1 }); // Department column
                footerRow.Cells.Add(new TableCell { Text = totalExpenseAmount.ToString("N2") }); // Expense Amount
                footerRow.Cells.Add(new TableCell { Text = totalHandlingCharge.ToString("N2") }); // Handling Charge
                footerRow.Cells.Add(new TableCell { Text = totalWithoutGST.ToString("N2") }); // Total Without GST
                footerRow.Cells.Add(new TableCell { Text = totalGST.ToString("N2") }); // GST
                footerRow.Cells.Add(new TableCell { Text = totalWithGST.ToString("N2") }); // Total With GST

                // Apply styling to the footer row (optional)
                footerRow.Cells[0].CssClass = "footer-cell"; // Style for SNo column
                footerRow.Cells[1].CssClass = "footer-cell"; // Style for "Grand Total" label
                footerRow.Cells[2].CssClass = "footer-cell"; // Style for Expense Amount
                footerRow.Cells[3].CssClass = "footer-cell"; // Style for Handling Charge
                footerRow.Cells[4].CssClass = "footer-cell"; // Style for Total Without GST
                footerRow.Cells[5].CssClass = "footer-cell"; // Style for GST
                footerRow.Cells[6].CssClass = "footer-cell"; // Style for Total With GST

                // Add the footer row directly to the GridView
                gvOverallTotals.Controls[0].Controls.Add(footerRow);
            }
        }
        private DataTable GetOverallTotals(string region, string branchName, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            // Build dynamic filter clauses
            string branchFilter = "";
            string regionJoinAndFilter = "";
            string refreshBranchFilter = "";
            string refreshRegionFilter = "";
            string awardBranchFilter = "";
            string awardRegionFilter = "";

            if (branchName != "All")
            {
                branchFilter = "AND b.BranchName = @BranchName";
                refreshBranchFilter = "AND e.BranchName = @BranchName";
                awardBranchFilter = "AND b.BranchName = @BranchName";
            }
            else if (region != "All")
            {
                // Join with Region and filter by @Region
                regionJoinAndFilter = @"
            INNER JOIN Region r ON b.RegionID = r.RegionID";
                branchFilter = "AND r.Region = @Region";

                refreshRegionFilter = @"
            INNER JOIN Branch br ON e.BranchID = br.BranchID
            INNER JOIN Region rg ON br.RegionID = rg.RegionID
            AND rg.Region = @Region";

                awardRegionFilter = @"
            INNER JOIN Region rg ON b.RegionID = rg.RegionID
            AND rg.Region = @Region";
            }

            string query = $@"
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
DepartmentExpenses AS (
    SELECT 
        b.BranchName,
        s.Department,
        s.ServiceID,
        ISNULL(ac.TotalConveyance, 0) AS TotalConveyance,
        ISNULL(af.TotalFood, 0) AS TotalFood,
        ISNULL(am.TotalMiscellaneous, 0) AS TotalMiscellaneous,
        ISNULL(al.TotalLodging, 0) AS TotalLodging,
        ISNULL(ao.TotalOthers, 0) AS TotalOthers
    FROM Services s
    INNER JOIN Branch b ON s.BranchID = b.BranchID
    {regionJoinAndFilter}
    LEFT JOIN AggregatedConveyance ac ON s.ServiceID = ac.ServiceID
    LEFT JOIN AggregatedFood af ON s.ServiceID = af.ServiceID
    LEFT JOIN AggregatedMiscellaneous am ON s.ServiceID = am.ServiceID
    LEFT JOIN AggregatedLodging al ON s.ServiceID = al.ServiceID
    LEFT JOIN AggregatedOthers ao ON s.ServiceID = ao.ServiceID
    WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate
    {branchFilter}
),
RefreshExpenses AS (
    SELECT 
        e.BranchName,
        SUM(ISNULL(r.RefreshAmount, 0)) AS RefreshTotal
    FROM Refreshment r
    INNER JOIN Employees e ON r.EmployeeID = e.EmployeeID
    {(branchName != "All" ? refreshBranchFilter : refreshRegionFilter)}
    WHERE r.FromDate >= @FromDate AND r.ToDate <= @ToDate
    GROUP BY e.BranchName
),
AwardExpenses AS (
    SELECT 
        b.BranchName,
        SUM(ISNULL(a.Amount, 0)) AS AwardTotal
    FROM Award a
    INNER JOIN Services s ON a.ServiceID = s.ServiceID
    INNER JOIN Branch b ON s.BranchID = b.BranchID
    {(branchName != "All" ? "" : awardRegionFilter)}
    WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate
    {(branchName != "All" ? awardBranchFilter : "")}
    GROUP BY b.BranchName
)
SELECT 
    'Service' AS Department,
    SUM(de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) AS ExpenseAmount,
    SUM((de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) * 0.05) AS HandlingCharge,
    SUM((de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) * 1.05) AS TotalWithoutGST,
    SUM((de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) * 1.05 * 0.18) AS GST,
    SUM((de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) * 1.05 * 1.18) AS TotalWithGST
FROM DepartmentExpenses de
WHERE de.Department = 'Service'
HAVING SUM(de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) > 0

UNION ALL

SELECT 
    'Sales' AS Department,
    SUM(de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) AS ExpenseAmount,
    SUM((de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) * 0.05) AS HandlingCharge,
    SUM((de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) * 1.05) AS TotalWithoutGST,
    SUM((de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) * 1.05 * 0.18) AS GST,
    SUM((de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) * 1.05 * 1.18) AS TotalWithGST
FROM DepartmentExpenses de
WHERE de.Department = 'Sales'
HAVING SUM(de.TotalConveyance + de.TotalFood + de.TotalMiscellaneous + de.TotalLodging + de.TotalOthers) > 0

UNION ALL

SELECT 
    'Refreshment' AS Department,
    SUM(ISNULL(re.RefreshTotal, 0)) AS ExpenseAmount,
    SUM(ISNULL(re.RefreshTotal, 0) * 0.05) AS HandlingCharge,
    SUM(ISNULL(re.RefreshTotal, 0) * 1.05) AS TotalWithoutGST,
    SUM(ISNULL(re.RefreshTotal, 0) * 1.05 * 0.18) AS GST,
    SUM(ISNULL(re.RefreshTotal, 0) * 1.05 * 1.18) AS TotalWithGST
FROM RefreshExpenses re
HAVING SUM(ISNULL(re.RefreshTotal, 0)) > 0

UNION ALL

SELECT 
    'Award' AS Department,
    SUM(ISNULL(ae.AwardTotal, 0)) AS ExpenseAmount,
    0 AS HandlingCharge,
    SUM(ISNULL(ae.AwardTotal, 0)) AS TotalWithoutGST,
    SUM(ISNULL(ae.AwardTotal, 0)) * 0.18 AS GST,
    SUM(ISNULL(ae.AwardTotal, 0)) * 1.18 AS TotalWithGST
FROM AwardExpenses ae
HAVING SUM(ISNULL(ae.AwardTotal, 0)) > 0;";

            DataTable result = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                if (branchName != "All")
                {
                    cmd.Parameters.AddWithValue("@BranchName", branchName);
                }
                else if (region != "All")
                {
                    cmd.Parameters.AddWithValue("@Region", region);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(result);
                }
            }
            return result;
        }
        private DataTable LoadSmoSoData(string region, string branch, DateTime fromDate, DateTime toDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            // Build region filter condition if needed
            string regionFilterClause = "";
            if (region != "All" && branch == "All")
            {
                regionFilterClause = @"
    INNER JOIN Branch brMain ON emp.BranchID = brMain.BranchID
    INNER JOIN Region regMain ON brMain.RegionID = regMain.RegionID
    WHERE regMain.Region = @Region AND ";
            }
            else
            {
                regionFilterClause = "WHERE ";
            }
            string branchFilterClause = (branch != "All") ? "emp.BranchName = @BranchName AND " : "";
            string baseWhere = regionFilterClause + branchFilterClause +
                "srv.FromDate >= @FromDate AND srv.ToDate <= @ToDate";

            string query = $@"
WITH CombinedData AS (
     SELECT
         conv.SmoNo,
         conv.SoNo,
         emp.BranchName,
         SUM(CAST(ISNULL(conv.ClaimedAmount, 0) AS DECIMAL(18, 2))) AS TotalClaimedAmount,
         'Service' AS Department
     FROM Conveyance conv
     INNER JOIN Services srv ON conv.ServiceId = srv.ServiceId
     INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
     {baseWhere}
     GROUP BY conv.SmoNo, conv.SoNo, emp.BranchName

     UNION ALL

     SELECT
         food.SmoNo,
         food.SoNo,
         emp.BranchName,
         SUM(CAST(ISNULL(food.ClaimedAmount, 0) AS DECIMAL(18, 2))) AS TotalClaimedAmount,
         'Sales' AS Department
     FROM Food food
     INNER JOIN Services srv ON food.ServiceId = srv.ServiceId
     INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
     {baseWhere}
     GROUP BY food.SmoNo, food.SoNo, emp.BranchName

     UNION ALL

     SELECT
         lodg.SmoNo,
         lodg.SoNo,
         emp.BranchName,
         SUM(CAST(ISNULL(lodg.ClaimedAmount, 0) AS DECIMAL(18, 2))) AS TotalClaimedAmount,
         'Service' AS Department
     FROM Lodging lodg
     INNER JOIN Services srv ON lodg.ServiceId = srv.ServiceId
     INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
     {baseWhere}
     GROUP BY lodg.SmoNo, lodg.SoNo, emp.BranchName

     UNION ALL

     SELECT
         misc.SmoNo,
         misc.SoNo,
         emp.BranchName,
         SUM(CAST(ISNULL(misc.ClaimedAmount, 0) AS DECIMAL(18, 2))) AS TotalClaimedAmount,
         'Service' AS Department
     FROM Miscellaneous misc
     INNER JOIN Services srv ON misc.ServiceId = srv.ServiceId
     INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
     {baseWhere}
     GROUP BY misc.SmoNo, misc.SoNo, emp.BranchName

     UNION ALL

     SELECT
         oth.SmoNo,
         oth.SoNo,
         emp.BranchName,
         SUM(CAST(ISNULL(oth.ClaimedAmount, 0) AS DECIMAL(18, 2))) AS TotalClaimedAmount,
         'Sales' AS Department
     FROM Others oth
     INNER JOIN Services srv ON oth.ServiceId = srv.ServiceId
     INNER JOIN Employees emp ON emp.EmployeeId = srv.EmployeeId
     {baseWhere}
     GROUP BY oth.SmoNo, oth.SoNo, emp.BranchName
)
SELECT
    CAST(ROW_NUMBER() OVER (ORDER BY BranchName, SmoNo, SoNo) AS NVARCHAR(10)) AS SerialNo,
    COALESCE(SmoNo, '') + ' - ' + COALESCE(SoNo, '') AS SmoSo,
    BranchName,
    Department,
    SUM(TotalClaimedAmount) AS TotalClaimedAmount
FROM CombinedData
GROUP BY SmoNo, SoNo, BranchName, Department
HAVING SUM(TotalClaimedAmount) > 0
ORDER BY BranchName, SmoNo, SoNo;
";

            // Refresh query
            string refreshQueryBaseWhere = "";
            if (region != "All" && branch == "All")
            {
                refreshQueryBaseWhere = @"
    INNER JOIN Branch brRefresh ON emp.BranchID = brRefresh.BranchID
    INNER JOIN Region regRefresh ON brRefresh.RegionID = regRefresh.RegionID
    WHERE regRefresh.Region = @Region AND re.FromDate >= @FromDate AND re.ToDate <= @ToDate";
            }
            else
            {
                refreshQueryBaseWhere = "WHERE re.FromDate >= @FromDate AND re.ToDate <= @ToDate";
            }
            string refreshBranchFilter = (branch != "All") ? " AND emp.BranchName = @BranchName" : "";

            string refreshQuery = $@"
SELECT 
    emp.BranchName,
    'Refreshment' AS Department,
    SUM(ISNULL(re.RefreshAmount, 0)) AS TotalClaimedAmount
FROM Refreshment re
INNER JOIN Employees emp ON re.EmployeeId = emp.EmployeeId
{refreshQueryBaseWhere}{refreshBranchFilter}
GROUP BY emp.BranchName
HAVING SUM(ISNULL(re.RefreshAmount, 0)) > 0
ORDER BY emp.BranchName;
";

            DataTable smoSoDataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                if (region != "All" && branch == "All")
                    cmd.Parameters.AddWithValue("@Region", region);
                if (branch != "All")
                    cmd.Parameters.AddWithValue("@BranchName", branch);

                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(smoSoDataTable);
                }
            }

            DataTable refreshDataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(refreshQuery, con))
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                if (region != "All" && branch == "All")
                    cmd.Parameters.AddWithValue("@Region", region);
                if (branch != "All")
                    cmd.Parameters.AddWithValue("@BranchName", branch);

                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(refreshDataTable);
                }
            }

            foreach (DataRow refreshRow in refreshDataTable.Rows)
            {
                DataRow newRow = smoSoDataTable.NewRow();
                newRow["SmoSo"] = "Refreshment";
                newRow["BranchName"] = refreshRow["BranchName"];
                newRow["Department"] = refreshRow["Department"];
                newRow["TotalClaimedAmount"] = refreshRow["TotalClaimedAmount"];
                smoSoDataTable.Rows.Add(newRow);
            }

            if (smoSoDataTable.Rows.Count == 0) return smoSoDataTable;

            DataRow totalRow = smoSoDataTable.NewRow();
            totalRow["SmoSo"] = "Total";
            totalRow["BranchName"] = "";
            totalRow["Department"] = "";
            totalRow["TotalClaimedAmount"] = Math.Round(smoSoDataTable.AsEnumerable().Sum(row => row.Field<decimal>("TotalClaimedAmount")), 2);
            smoSoDataTable.Rows.Add(totalRow);

            var sortedRows = smoSoDataTable.AsEnumerable()
                .Where(row => row.Field<string>("SmoSo") != "Total")
                .OrderBy(row => row.Field<string>("BranchName"))
                .ThenBy(row => row.Field<string>("SmoSo"))
                .ToList();

            var finalRows = sortedRows.Concat(new[] { totalRow });
            DataTable sortedDataTable = smoSoDataTable.Clone();
            foreach (var row in finalRows) sortedDataTable.ImportRow(row);

            int serialNumber = 1;
            foreach (DataRow row in sortedDataTable.Rows)
            {
                row["SerialNo"] = row.Field<string>("SmoSo") == "Total" ? "" : (serialNumber++).ToString();
            }

            string monthYear = fromDate.ToString("MMM-yy") == toDate.ToString("MMM-yy")
                ? fromDate.ToString("MMM-yy")
                : $"{fromDate.ToString("MMM-yy")} - {toDate.ToString("MMM-yy")}";

            sortedDataTable.Columns.Add("HandlingCharge", typeof(decimal));
            sortedDataTable.Columns.Add("TotalWithoutGST", typeof(decimal));
            sortedDataTable.Columns.Add("GST", typeof(decimal));
            sortedDataTable.Columns.Add("OverallTotal", typeof(decimal));
            sortedDataTable.Columns.Add("MonthYear", typeof(string));

            foreach (DataRow row in sortedDataTable.Rows)
            {
                decimal total = row.Field<decimal>("TotalClaimedAmount");
                decimal hc = Math.Round(total * 0.05m, 2);
                decimal twg = Math.Round(total + hc, 2);
                decimal gst = Math.Round(twg * 0.18m, 2);
                row["HandlingCharge"] = hc;
                row["TotalWithoutGST"] = twg;
                row["GST"] = gst;
                row["OverallTotal"] = Math.Round(twg + gst, 2);
                row["MonthYear"] = row.Field<string>("SmoSo") == "Total" ? "" : monthYear;
            }

            DataTable finalDataTable = new DataTable();
            string[] columnNames = {
    "SerialNo", "MonthYear", "SmoSo", "BranchName", "Department",
    "TotalClaimedAmount", "HandlingCharge", "TotalWithoutGST", "GST", "OverallTotal"
};

            foreach (string col in columnNames)
            {
                finalDataTable.Columns.Add(col, sortedDataTable.Columns[col].DataType);
            }

            foreach (DataRow row in sortedDataTable.Rows)
            {
                var newRow = finalDataTable.NewRow();
                foreach (DataColumn col in finalDataTable.Columns)
                    newRow[col.ColumnName] = row[col.ColumnName];
                finalDataTable.Rows.Add(newRow);
            }

            return finalDataTable;
        }
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                // Parse the selected date range
                DateTime fromDate = DateTime.Parse(txtFromDate.Text);
                DateTime toDate = DateTime.Parse(txtToDate.Text);
                string selectedRegion = ddlRegion.SelectedValue;
                string selectedBranch = ddlBranch.SelectedValue;
                DataTable smoSoData = LoadSmoSoData(selectedRegion, selectedBranch, fromDate, toDate);
                DataTable departmentTotals = GetDepartmentTotals(selectedRegion, selectedBranch, fromDate, toDate);
                DataTable overallTotals = GetOverallTotals(selectedRegion, selectedBranch, fromDate, toDate);
                // Export all reports to a single Excel file
                ExportToExcel(smoSoData, departmentTotals, overallTotals, fromDate, toDate);
            }
            catch (FormatException)
            {
                lblError.Text = "Invalid date format. Please enter valid dates.";
                lblError.Visible = true;
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred: " + ex.Message;
                lblError.Visible = true;
            }
        }
        private void ExportToExcel(DataTable smoSoData, DataTable departmentTotals, DataTable overallTotals, DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Validate input data
                if ((smoSoData == null || smoSoData.Rows.Count == 0) &&
                    (departmentTotals == null || departmentTotals.Rows.Count == 0) &&
                    (overallTotals == null || overallTotals.Rows.Count == 0))
                {
                    lblError.Text = "No data available to export.";
                    lblError.Visible = true;
                    return;
                }

                // Set the license context for EPPlus
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                // Clear the response and set headers
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=CombinedReport.xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                using (ExcelPackage package = new ExcelPackage())
                {
                    // Create a single worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Combined Report");

                    int currentRow = 1; // Track the current row in the worksheet

                    // Generate dynamic title based on the selected date range
                    string dynamicTitle = $"Local & Tour Expenses of Southern Region for the Period of {FormatDateRange(fromDate, toDate)}";

                    // Add Dynamic Title
                    worksheet.Cells[currentRow, 1].Value = dynamicTitle; // Dynamic Title
                    worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                    worksheet.Cells[currentRow, 1].Style.Font.Size = 14;
                    worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[currentRow, 1, currentRow, smoSoData.Columns.Count].Merge = true; // Merge cells for title
                    currentRow += 2; // Leave two blank rows after the title

                    // Add SMO Report Data
                    if (smoSoData != null && smoSoData.Rows.Count > 0)
                    {
                        // Subtitle for SMO Report
                        //worksheet.Cells[currentRow, 1].Value = "SMO Report";
                        worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 1].Style.Font.Size = 12;
                        worksheet.Cells[currentRow, 1, currentRow, smoSoData.Columns.Count].Merge = true; // Merge cells for subtitle
                        currentRow++;

                        // Load SMO Report data
                        worksheet.Cells[currentRow, 1].LoadFromDataTable(smoSoData, true);

                        // Modify column headers
                        worksheet.Cells[currentRow, 1].Value = "S.No"; // Change "SerialNo" to "S.No"
                        worksheet.Cells[currentRow, 2].Value = "Month"; // Change "MonthYear" to "Month"
                        worksheet.Cells[currentRow, 7].Value = "Handling Charge (5%)"; // Change "HandlingCharge" to "Handling Charge (5%)"
                        worksheet.Cells[currentRow, 9].Value = "GST (18%)";
                        // Format Month column values
                        for (int i = currentRow + 1; i <= currentRow + smoSoData.Rows.Count; i++)
                        {
                            string monthYear = worksheet.Cells[i, 2].Text;
                            if (!string.IsNullOrEmpty(monthYear) && monthYear.Contains("-"))
                            {
                                string[] parts = monthYear.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 2 && parts[0] == parts[1])
                                {
                                    worksheet.Cells[i, 2].Value = parts[0]; // Display only the single month (e.g., Jan-24)
                                }
                            }
                        }

                        // Apply formatting to the header row
                        using (var range = worksheet.Cells[currentRow, 1, currentRow, smoSoData.Columns.Count])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                        }

                        currentRow += smoSoData.Rows.Count + 2; // Leave two blank rows after SMO Report
                    }
                    string dynamicTitle1 = $"Local & Tour Expenses of Southern Region for the Period of {FormatDateRange(fromDate, toDate)}";

                    // Add Dynamic Title
                    worksheet.Cells[currentRow, 1].Value = dynamicTitle1; // Dynamic Title
                    worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                    worksheet.Cells[currentRow, 1].Style.Font.Size = 14;
                    worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[currentRow, 1, currentRow, smoSoData.Columns.Count].Merge = true; // Merge cells for title
                    currentRow += 2; // Leave two blank rows after the title


                    // Add Department Totals Data
                    if (departmentTotals != null && departmentTotals.Rows.Count > 0)
                    {
                        // Subtitle for Department Totals
                        // worksheet.Cells[currentRow, 1].Value = "Department Totals";
                        worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 1].Style.Font.Size = 12;
                        worksheet.Cells[currentRow, 1, currentRow, departmentTotals.Columns.Count].Merge = true; // Merge cells for subtitle
                        currentRow++;

                        // Load Department Totals data
                        worksheet.Cells[currentRow, 1].LoadFromDataTable(departmentTotals, true);

                        // Modify column headers
                        worksheet.Cells[currentRow, 1].Value = "S.No"; // Change "SerialNo" to "S.No"
                        worksheet.Cells[currentRow, 2].Value = "Month"; // Change "Month" to "Month"
                        worksheet.Cells[currentRow, 7].Value = "Reimbursement Total";
                        worksheet.Cells[currentRow, 8].Value = "Handling Charge (5%)"; // Change "HandlingCharge" to "Handling Charge (5%)"
                                                                                       // worksheet.Cells[currentRow, 8].Value = "GST (18%)";
                                                                                       // Format Month column values
                        for (int i = currentRow + 1; i <= currentRow + departmentTotals.Rows.Count; i++)
                        {
                            string month = worksheet.Cells[i, 2].Text;
                            if (!string.IsNullOrEmpty(month) && month.Contains("-"))
                            {
                                string[] parts = month.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 2 && parts[0] == parts[1])
                                {
                                    worksheet.Cells[i, 2].Value = parts[0]; // Display only the single month (e.g., Jan-24)
                                }
                            }
                        }

                        // Apply formatting to the header row
                        using (var range = worksheet.Cells[currentRow, 1, currentRow, departmentTotals.Columns.Count])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                        }

                        // Add footer row for totals
                        int lastRow = currentRow + departmentTotals.Rows.Count + 1; // Leave one blank row before the footer
                        worksheet.Cells[lastRow, 1].Value = "Total Without GST";
                        worksheet.Cells[lastRow, departmentTotals.Columns.Count].Formula = $"SUM({worksheet.Cells[currentRow + 1, departmentTotals.Columns.Count].Address}:{worksheet.Cells[lastRow - 1, departmentTotals.Columns.Count].Address})";

                        currentRow += departmentTotals.Rows.Count + 3; // Leave three blank rows after Department Totals
                    }
                    if (overallTotals != null && overallTotals.Rows.Count > 0)
                    {
                        //worksheet.Cells[currentRow, 1].Value = "Overall Totals"; // Subtitle for Overall Totals
                        worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 1].Style.Font.Size = 12;
                        worksheet.Cells[currentRow, 1, currentRow, overallTotals.Columns.Count].Merge = true; // Merge cells for subtitle
                        currentRow++;

                        // Load Overall Totals data
                        worksheet.Cells[currentRow, 1].LoadFromDataTable(overallTotals, true);

                        // Apply formatting to the header row
                        using (var range = worksheet.Cells[currentRow, 1, currentRow, overallTotals.Columns.Count])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                        }

                        // Add footer row for grand totals
                        int lastRow = currentRow + overallTotals.Rows.Count + 1; // Leave one blank row before the footer
                        worksheet.Cells[lastRow, 1].Value = "Grand Total";
                        worksheet.Cells[lastRow, 2].Formula = $"SUM({worksheet.Cells[currentRow + 1, 2].Address}:{worksheet.Cells[lastRow - 1, 2].Address})"; // ExpenseAmount
                        worksheet.Cells[lastRow, 3].Formula = $"SUM({worksheet.Cells[currentRow + 1, 3].Address}:{worksheet.Cells[lastRow - 1, 3].Address})"; // HandlingCharge
                        worksheet.Cells[lastRow, 4].Formula = $"SUM({worksheet.Cells[currentRow + 1, 4].Address}:{worksheet.Cells[lastRow - 1, 4].Address})"; // TotalWithoutGST
                        worksheet.Cells[lastRow, 5].Formula = $"SUM({worksheet.Cells[currentRow + 1, 5].Address}:{worksheet.Cells[lastRow - 1, 5].Address})"; // GST
                        worksheet.Cells[lastRow, 6].Formula = $"SUM({worksheet.Cells[currentRow + 1, 6].Address}:{worksheet.Cells[lastRow - 1, 6].Address})"; // TotalWithGST

                        currentRow += overallTotals.Rows.Count + 3; // Leave three blank rows after Overall Totals
                    }


                    // Auto-fit columns for better readability
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Write the Excel file to the response
                    Response.BinaryWrite(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                lblError.Text = "Error exporting to Excel: " + ex.Message;
                lblError.Visible = true;
            }
            finally
            {
                Response.End(); // Ensure the response ends properly
            }
        }

        // Helper method to format the date range
        private string FormatDateRange(DateTime fromDate, DateTime toDate)
        {
            if (fromDate.ToString("MMM-yy") == toDate.ToString("MMM-yy"))
            {
                return fromDate.ToString("MMM-yy"); // Single month (e.g., Jan-24)
            }
            else
            {
                return $"{fromDate.ToString("MMM-yy")} - {toDate.ToString("MMM-yy")}"; // Full range (e.g., Dec-24 - Jan-25)
            }
        }

    }
}