using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.IO;

namespace Vivify
{
    public partial class AdminTravelVerify : System.Web.UI.Page
    {
        private const string ConnectionStringName = "vivify";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["TotalClaimed"] = 0m;
                Session["TotalNonClaimed"] = 0m;

                // Check query string parameters first
                CheckQueryStringParameters();

                
                if (Session["TotalClaimed"] != null)
                    lblDisplayClaimed.Text = Convert.ToDecimal(Session["TotalClaimed"]).ToString("0.00");
                else
                    lblDisplayClaimed.Text = "0.00";

                if (Session["TotalNonClaimed"] != null)
                    lblDisplayNonClaimed.Text = Convert.ToDecimal(Session["TotalNonClaimed"]).ToString("0.00");
                else
                    lblDisplayNonClaimed.Text = "0.00";

                // Hide non-claimed after verification
                if (Session["IsVerified"] != null && (bool)Session["IsVerified"])
                {
                    lblDisplayNonClaimed.Visible = false;
                }
                else
                {
                    lblDisplayNonClaimed.Visible = true;
                }

                // Get parameters from session/query string and bind data
                BindTravelExpenseData();
            }
        }

        // Method to bind TravelExpenses data
        protected void BindTravelExpenseData()
        {
            try
            {
                string employeeName = Session["EmployeeName"]?.ToString();
                string travelDate = Session["TravelDate"]?.ToString();

                // Debug session values
                System.Diagnostics.Debug.WriteLine($"DEBUG - Session values - EmployeeName: '{employeeName}', TravelDate: '{travelDate}'");

                // Check all session values
                System.Diagnostics.Debug.WriteLine("DEBUG - All Session values:");
                foreach (string key in Session.Keys)
                {
                    System.Diagnostics.Debug.WriteLine($"  {key}: {Session[key]}");
                }

                // Alternative: Get from query string if session is empty
                if (string.IsNullOrEmpty(employeeName))
                    employeeName = Request.QueryString["employeeName"];
                if (string.IsNullOrEmpty(travelDate))
                    travelDate = Request.QueryString["travelDate"];

                System.Diagnostics.Debug.WriteLine($"DEBUG - After query string check - EmployeeName: '{employeeName}', TravelDate: '{travelDate}'");

                if (!string.IsNullOrEmpty(employeeName) && !string.IsNullOrEmpty(travelDate))
                {
                    BindTravelExpenseGridView(employeeName, travelDate);
                }
                else
                {
                    lblMessage.Text = "Employee name or travel date not specified in session.";
                    lblMessage.Visible = true;
                    System.Diagnostics.Debug.WriteLine($"DEBUG - Missing parameters - EmployeeName: '{employeeName}', TravelDate: '{travelDate}'");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading data: " + ex.Message;
                lblMessage.Visible = true;
                System.Diagnostics.Debug.WriteLine($"DEBUG - Error in BindTravelExpenseData: {ex.Message}");
            }
        }

        // Method to bind ALL TravelExpenses data for the employee and date
        // Method to bind ALL TravelExpenses data for the employee and date
        protected void BindTravelExpenseGridView(string employeeName, string travelDate)
        {
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                // Debug: Print the parameters being used
                System.Diagnostics.Debug.WriteLine($"DEBUG - Searching for: EmployeeName='{employeeName}', TravelDate='{travelDate}'");

                // More flexible query to handle different date formats
                // In BindTravelExpenseGridView: ALLOW Status = 2 AND 3
                string qry = @"
SELECT 
    te.Id,
    te.EmployeeId,
    e.FirstName + ' ' + e.LastName AS EmployeeName,
    b.BranchName,
    te.TransportType,
    FORMAT(te.Date, 'dd/MMM/yyyy') AS TravelDate,
    te.FromPlace,
    te.ToPlace,
    te.Amount,
    FORMAT(te.FromTime, 'HH:mm') AS FromTime,
    FORMAT(te.ToTime, 'HH:mm') AS ToTime,
    te.Particulars,
    te.WBS,
    te.SAP,
    te.ReportingManager,
    te.Status,
    te.RefreshAmnt,
    ISNULL(te.IsClaimable, 1) AS IsClaimable,
    ISNULL(te.ClaimedAmount, te.Amount) AS ClaimedAmount,
    ISNULL(te.NonClaimedAmount, 0) AS NonClaimedAmount
FROM 
    TravelExpenses te
INNER JOIN Employees e ON te.EmployeeId = e.EmployeeId
INNER JOIN Branch b ON te.BranchId = b.BranchId
WHERE 
    (e.FirstName + ' ' + e.LastName = @EmployeeName)
    AND (CONVERT(date, te.Date) = CONVERT(date, @TravelDate)
         OR FORMAT(te.Date, 'dd/MMM/yyyy') = @TravelDate
         OR FORMAT(te.Date, 'dd-MMM-yyyy') = @TravelDate)
    AND te.Status IN (2, 3)  -- ✅ Allow editing of verified (3) and pending (2)
ORDER BY 
    te.Id";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                    cmd.Parameters.AddWithValue("@TravelDate", travelDate);

                    try
                    {
                        con.Open();
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            // Debug: Check what data was returned
                            System.Diagnostics.Debug.WriteLine($"DEBUG - Found {dt.Rows.Count} records in database");

                            if (dt.Rows.Count > 0)
                            {
                                // Debug: Show first row data
                                DataRow firstRow = dt.Rows[0];
                                System.Diagnostics.Debug.WriteLine($"DEBUG - First row - Employee: {firstRow["EmployeeName"]}, Date: {firstRow["TravelDate"]}, Amount: {firstRow["Amount"]}, RefreshAmnt: {firstRow["RefreshAmnt"]}");

                                ExpenseGridView.DataSource = dt;
                                ExpenseGridView.DataBind();
                                UpdateTotalAmounts();

                                lblMessage.Text = $"Found {dt.Rows.Count} expense records for verification.";
                                lblMessage.Visible = true;
                            }
                            else
                            {
                                ExpenseGridView.DataSource = null;
                                ExpenseGridView.DataBind();

                                // More detailed error message
                                lblMessage.Text = $"No travel expenses found for Employee: {employeeName} on Date: {travelDate}. " +
                                                "Possible reasons: Expenses are already verified, no expenses submitted, or date format mismatch.";
                                lblMessage.Visible = true;

                                // Debug information
                                System.Diagnostics.Debug.WriteLine($"DEBUG - No records found for: EmployeeName='{employeeName}', TravelDate='{travelDate}'");
                                System.Diagnostics.Debug.WriteLine($"DEBUG - Query executed: {qry}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"DEBUG - SQL Error: {ex.Message}");
                        lblMessage.Text = $"Database error: {ex.Message}";
                        lblMessage.Visible = true;
                    }
                }
            }
        }

        protected void CheckQueryStringParameters()
        {
            // Check if we have query string parameters as backup
            if (Session["EmployeeName"] == null && Request.QueryString["employeeName"] != null)
            {
                Session["EmployeeName"] = Request.QueryString["employeeName"];
            }

            if (Session["TravelDate"] == null && Request.QueryString["travelDate"] != null)
            {
                Session["TravelDate"] = Request.QueryString["travelDate"];
            }

            System.Diagnostics.Debug.WriteLine($"DEBUG - After query string check - Session EmployeeName: '{Session["EmployeeName"]}', TravelDate: '{Session["TravelDate"]}'");
        }

        protected void ExpenseGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                // === Conveyance Amount Edit/Save ===
                if (e.CommandName == "EditRow")
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = ExpenseGridView.Rows[rowIndex];
                    TextBox amountTextBox = (TextBox)row.FindControl("txtConveyanceAmount");
                    Button editButton = (Button)row.FindControl("btnEditConveyance");

                    if (amountTextBox != null && editButton != null)
                    {
                        if (editButton.Text == "Edit")
                        {
                            amountTextBox.ReadOnly = false;
                            amountTextBox.CssClass = "amount-input editing";
                            editButton.Text = "Save";
                            editButton.CssClass = "btn-edit saving";
                        }
                        else
                        {
                            amountTextBox.ReadOnly = true;
                            amountTextBox.CssClass = "amount-input";
                            editButton.Text = "Edit";
                            editButton.CssClass = "btn-edit";

                            if (decimal.TryParse(amountTextBox.Text, out decimal newAmount))
                            {
                                amountTextBox.Attributes["data-original-value"] = newAmount.ToString("0.00");
                                lblMessage.Text = "Conveyance amount updated locally. Click Submit to save.";
                                lblMessage.Visible = true;
                            }
                            else
                            {
                                lblMessage.Text = "Invalid conveyance amount.";
                                lblMessage.Visible = true;
                                amountTextBox.Text = amountTextBox.Attributes["data-original-value"] ?? "0.00";
                            }
                        }
                    }
                }
                // === Refreshment Amount Edit/Save ===
                else if (e.CommandName == "EditRefresh")
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = ExpenseGridView.Rows[rowIndex];
                    TextBox txtRefresh = (TextBox)row.FindControl("txtRefreshAmount");
                    Button btnEdit = (Button)row.FindControl("btnEditRefresh");

                    if (txtRefresh != null && btnEdit != null)
                    {
                        if (btnEdit.Text == "Edit")
                        {
                            txtRefresh.ReadOnly = false;
                            txtRefresh.CssClass = "amount-input editing";
                            btnEdit.Text = "Save";
                            btnEdit.CssClass = "btn-edit saving";
                        }
                        else
                        {
                            txtRefresh.ReadOnly = true;
                            txtRefresh.CssClass = "amount-input";
                            btnEdit.Text = "Edit";
                            btnEdit.CssClass = "btn-edit";

                            if (decimal.TryParse(txtRefresh.Text, out decimal newRefreshAmt))
                            {
                                txtRefresh.Attributes["data-original-value"] = newRefreshAmt.ToString("0.00");
                                lblMessage.Text = "Refreshment amount updated locally. Click Submit to save.";
                                lblMessage.Visible = true;
                            }
                            else
                            {
                                lblMessage.Text = "Invalid refreshment amount.";
                                lblMessage.Visible = true;
                                txtRefresh.Text = txtRefresh.Attributes["data-original-value"] ?? "0.00";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
                lblMessage.Visible = true;
                System.Diagnostics.Debug.WriteLine($"Error in RowCommand: {ex.Message}");
            }
        }
        protected void chkClaimable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                GridViewRow row = (GridViewRow)checkBox.NamingContainer;

                if (row != null && ExpenseGridView.DataKeys[row.RowIndex] != null)
                {
                    int expenseId = Convert.ToInt32(ExpenseGridView.DataKeys[row.RowIndex].Value);
                    TextBox amountTextBox = (TextBox)row.FindControl("txtConveyanceAmount");

                    if (amountTextBox != null && !string.IsNullOrEmpty(amountTextBox.Text))
                    {
                        decimal amount = decimal.Parse(amountTextBox.Text);
                        UpdateClaimableStatusInDatabase(expenseId, checkBox.Checked, amount);
                        UpdateTotalAmounts();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in chkClaimable_CheckedChanged: {ex.Message}");
            }
        }

        private void UpdateClaimableStatusInDatabase(int expenseId, bool isClaimable, decimal amount)
        {
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string updateQuery = @"
                UPDATE TravelExpenses 
                SET IsClaimable = @IsClaimable,
                    ClaimedAmount = @ClaimedAmount,
                    NonClaimedAmount = @NonClaimedAmount,
                    UpdatedDate = GETDATE()
                WHERE Id = @ExpenseId";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    cmd.Parameters.AddWithValue("@IsClaimable", isClaimable);
                    cmd.Parameters.AddWithValue("@ClaimedAmount", isClaimable ? amount : 0);
                    cmd.Parameters.AddWithValue("@NonClaimedAmount", isClaimable ? 0 : amount);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateTotalAmounts()
        {
            decimal totalClaimed = 0;
            decimal totalNonClaimed = 0;

            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            if (Session["EmployeeName"] != null && Session["TravelDate"] != null)
            {
                string employeeName = Session["EmployeeName"].ToString();
                string travelDate = Session["TravelDate"].ToString();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    string qry = @"
            SELECT 
                SUM(ISNULL(ClaimedAmount, 0)) as TotalClaimed,
                SUM(ISNULL(NonClaimedAmount, 0)) as TotalNonClaimed
            FROM TravelExpenses te
            INNER JOIN Employees e ON te.EmployeeId = e.EmployeeId
            WHERE e.FirstName + ' ' + e.LastName = @EmployeeName
            AND (FORMAT(te.Date, 'dd/MMM/yyyy') = @TravelDate 
                 OR CONVERT(date, te.Date) = CONVERT(date, @TravelDate))";

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                        cmd.Parameters.AddWithValue("@TravelDate", travelDate);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalClaimed = reader["TotalClaimed"] != DBNull.Value ? Convert.ToDecimal(reader["TotalClaimed"]) : 0;
                                totalNonClaimed = reader["TotalNonClaimed"] != DBNull.Value ? Convert.ToDecimal(reader["TotalNonClaimed"]) : 0;
                            }
                        }
                    }
                }
            }

            // ✅ Always show claimed amount
            if (lblDisplayClaimed != null)
                lblDisplayClaimed.Text = totalClaimed.ToString("0.00");

            // ✅ Show/hide non-claimed based on verification status
            if (lblDisplayNonClaimed != null)
            {
                if (Session["IsVerified"] != null && (bool)Session["IsVerified"])
                {
                    lblDisplayNonClaimed.Visible = false; // 👈 HIDE after submit
                }
                else
                {
                    lblDisplayNonClaimed.Visible = true;
                    lblDisplayNonClaimed.Text = totalNonClaimed.ToString("0.00");
                }
            }

            Session["TotalClaimed"] = totalClaimed;
            Session["TotalNonClaimed"] = totalNonClaimed;
        }

        private void UpdateAmountInDatabase(int expenseId, decimal newAmount)
        {
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // First get the current claimable status
                string getStatusQuery = "SELECT IsClaimable FROM TravelExpenses WHERE Id = @ExpenseId";
                bool isClaimable = false;

                using (SqlCommand getCmd = new SqlCommand(getStatusQuery, con))
                {
                    getCmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    object result = getCmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        isClaimable = Convert.ToBoolean(result);
                    }
                }

                // Update the amount and recalculate claimed/non-claimed amounts
                string updateQuery = @"
                UPDATE TravelExpenses 
                SET Amount = @Amount,
                    ClaimedAmount = @ClaimedAmount,
                    NonClaimedAmount = @NonClaimedAmount,
                    UpdatedDate = GETDATE()
                WHERE Id = @ExpenseId";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    cmd.Parameters.AddWithValue("@Amount", newAmount);
                    cmd.Parameters.AddWithValue("@ClaimedAmount", isClaimable ? newAmount : 0);
                    cmd.Parameters.AddWithValue("@NonClaimedAmount", isClaimable ? 0 : newAmount);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    // ALWAYS process all rows (even if no change detected)
                    foreach (GridViewRow row in ExpenseGridView.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            int expenseId = Convert.ToInt32(ExpenseGridView.DataKeys[row.RowIndex].Value);
                            CheckBox chkClaimable = (CheckBox)row.FindControl("chkConveyanceClaimable");
                            TextBox txtAmount = (TextBox)row.FindControl("txtConveyanceAmount");
                            TextBox txtRefresh = (TextBox)row.FindControl("txtRefreshAmount");

                            // Update Refreshment Amount
                            if (txtRefresh != null && decimal.TryParse(txtRefresh.Text, out decimal refreshAmt))
                            {
                                UpdateRefreshAmountInDatabase(expenseId, refreshAmt);
                            }

                            // Update Conveyance + Claimable
                            if (txtAmount != null && decimal.TryParse(txtAmount.Text, out decimal amount))
                            {
                                UpdateClaimableStatusInDatabase(expenseId, chkClaimable?.Checked ?? true, amount);
                            }
                        }
                    }

                    // ALWAYS mark as verified (Status = 3) on Submit
                    if (Session["EmployeeName"] != null && Session["TravelDate"] != null)
                    {
                        string employeeName = Session["EmployeeName"].ToString();
                        string travelDate = Session["TravelDate"].ToString();

                        string updateStatusQuery = @"
                UPDATE TravelExpenses 
                SET Status = 3, UpdatedDate = GETDATE()
                FROM TravelExpenses te
                INNER JOIN Employees e ON te.EmployeeId = e.EmployeeId
                WHERE e.FirstName + ' ' + e.LastName = @EmployeeName
                AND (FORMAT(te.Date, 'dd/MMM/yyyy') = @TravelDate 
                     OR CONVERT(date, te.Date) = CONVERT(date, @TravelDate))";

                        using (SqlCommand cmd = new SqlCommand(updateStatusQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                            cmd.Parameters.AddWithValue("@TravelDate", travelDate);
                            cmd.ExecuteNonQuery();
                        }

                        Session["IsVerified"] = true;
                        UpdateTotalAmounts();

                        ClientScript.RegisterStartupScript(this.GetType(), "success",
                            "alert('Changes saved and marked as verified!');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error",
                    $"alert('Error: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        private void UpdateRefreshAmountInDatabase(int expenseId, decimal refreshAmount)
        {
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "UPDATE TravelExpenses SET RefreshAmnt = @RefreshAmnt, UpdatedDate = GETDATE() WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@Id", expenseId);
                    cmd.Parameters.AddWithValue("@RefreshAmnt", refreshAmount);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            DataTable dtExport = new DataTable();

            // Add columns based on your TravelExpenses GridView
            dtExport.Columns.Add("Employee Name");
            dtExport.Columns.Add("Branch");
            dtExport.Columns.Add("Transport Type");
            dtExport.Columns.Add("Travel Date");
            dtExport.Columns.Add("From Place");
            dtExport.Columns.Add("To Place");
            dtExport.Columns.Add("From Time");
            dtExport.Columns.Add("To Time");
            dtExport.Columns.Add("Particulars");
            dtExport.Columns.Add("WBS");
            dtExport.Columns.Add("SAP");
            dtExport.Columns.Add("Reporting Manager");
            dtExport.Columns.Add("Refreshment Amount");
            dtExport.Columns.Add("Amount");
            dtExport.Columns.Add("Claimable");
            dtExport.Columns.Add("Status");

            // Populate DataTable from ExpenseGridView
            foreach (GridViewRow row in ExpenseGridView.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    DataRow dataRow = dtExport.NewRow();
                    dataRow["Employee Name"] = row.Cells[1].Text; // Adjust index based on your columns
                    dataRow["Branch"] = row.Cells[2].Text;
                    dataRow["Transport Type"] = row.Cells[3].Text;
                    dataRow["Travel Date"] = row.Cells[4].Text;
                    dataRow["From Place"] = row.Cells[5].Text;
                    dataRow["To Place"] = row.Cells[6].Text;
                    dataRow["From Time"] = row.Cells[7].Text;
                    dataRow["To Time"] = row.Cells[8].Text;
                    dataRow["Particulars"] = row.Cells[9].Text;
                    dataRow["WBS"] = row.Cells[10].Text;
                    dataRow["SAP"] = row.Cells[11].Text;
                    dataRow["Reporting Manager"] = row.Cells[12].Text;

                    // Refreshment Amount
                    Label lblRefreshAmount = (Label)row.FindControl("lblRefreshAmount");
                    dataRow["Refreshment Amount"] = lblRefreshAmount?.Text ?? "0.00";

                    // Amount
                    TextBox amountTextBox = (TextBox)row.FindControl("txtConveyanceAmount");
                    dataRow["Amount"] = amountTextBox?.Text ?? "0";

                    // Claimable status
                    CheckBox claimableCheckBox = (CheckBox)row.FindControl("chkConveyanceClaimable");
                    dataRow["Claimable"] = claimableCheckBox?.Checked ?? false ? "Yes" : "No";

                    dataRow["Status"] = "Verified";

                    dtExport.Rows.Add(dataRow);
                }
            }

            // Add summary row with session values
            DataRow summaryRow = dtExport.NewRow();
            summaryRow["Employee Name"] = "TOTALS:";

            decimal claimedTotal = Session["TotalClaimed"] != null ? Convert.ToDecimal(Session["TotalClaimed"]) : 0;
            decimal nonClaimedTotal = Session["TotalNonClaimed"] != null ? Convert.ToDecimal(Session["TotalNonClaimed"]) : 0;

            summaryRow["Amount"] = claimedTotal.ToString("0.00");
            summaryRow["Claimable"] = "Claimed: " + claimedTotal.ToString("0.00") + " | Non-Claimed: " + nonClaimedTotal.ToString("0.00");
            dtExport.Rows.Add(summaryRow);

            // Export to Excel
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Travel Expense Report");
                worksheet.Cells["A1"].LoadFromDataTable(dtExport, true);

                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=Travel_Expense_Report.xlsx");
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
        }
        protected void ExpenseGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Ensure data attributes are set properly
                TextBox amountTextBox = (TextBox)e.Row.FindControl("txtConveyanceAmount");
                if (amountTextBox != null && !string.IsNullOrEmpty(amountTextBox.Text))
                {
                    try
                    {
                        decimal amount = decimal.Parse(amountTextBox.Text);
                        amountTextBox.Attributes["data-original-value"] = amount.ToString("0.00");
                    }
                    catch
                    {
                        // If parsing fails, use the text as is
                        amountTextBox.Attributes["data-original-value"] = amountTextBox.Text;
                    }
                }
            }
        }

        protected void btncancel_Click(object sender, EventArgs e)
        {
            // Clear totals
            if (lblDisplayClaimed != null)
                lblDisplayClaimed.Text = "0.00";

            if (lblDisplayNonClaimed != null)
                lblDisplayNonClaimed.Text = "0.00";

            Session["TotalClaimed"] = 0;
            Session["TotalNonClaimed"] = 0;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("TravelExpensePage.aspx");
        }

        // Add a refresh button method if needed
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            BindTravelExpenseData();
        }
    }
}