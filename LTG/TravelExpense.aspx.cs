using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class TravelExpense : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bool isNewForm = !string.IsNullOrEmpty(Request.QueryString["new"]) &&
                                 Request.QueryString["new"].ToLower() == "true";

                int employeeId = GetEmployeeIdFromCookies();
                if (employeeId == -1)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                if (isNewForm)
                {
                    // 🚫 DO NOT clear hdnRefreshSubmitDate — leave it empty
                    txtFromDate.Text = string.Empty;
                    txtWBSNo.Text = string.Empty;
                    txtSAPNo.Text = string.Empty;
                    txtReportingManager.Text = string.Empty;
                    ddlTransportType.SelectedIndex = 0;
                    ViewState["EditingId"] = null;
                    hdnExpenseId.Value = string.Empty;

                    // Show empty grid
                    GridViewTravelExpenses.DataSource = null;
                    GridViewTravelExpenses.DataBind();

                    // Optional: show message
                    return; // ⚠️ Exit early — skip all loading logic
                }

                // === Normal mode: load existing data (your original logic below) ===
                string empIdStr = Request.QueryString["empId"];
                string latestDateStr = Request.QueryString["latestDate"];
                DateTime? latestDate = null;

                if (!string.IsNullOrEmpty(empIdStr) && int.TryParse(empIdStr, out int eid))
                {
                    employeeId = eid;
                    if (!string.IsNullOrEmpty(latestDateStr) && DateTime.TryParse(latestDateStr, out DateTime ld))
                    {
                        latestDate = ld.Date;
                    }
                }
                else
                {
                    employeeId = GetEmployeeIdFromCookies();
                    if (employeeId == -1)
                    {
                        Response.Redirect("~/Login.aspx");
                        return;
                    }
                    string requestedDateStr = Request.QueryString["date"];
                    if (!string.IsNullOrEmpty(requestedDateStr) && DateTime.TryParse(requestedDateStr, out DateTime parsedDate))
                    {
                        latestDate = parsedDate.Date;
                    }
                    else
                    {
                        try
                        {
                            latestDate = GetMostRecentExpenseDate(employeeId);
                        }
                        catch
                        {
                            latestDate = DateTime.Now.Date;
                        }
                    }
                }

                if (latestDate.HasValue)
                {
                    hdnRefreshSubmitDate.Value = latestDate.Value.ToString("yyyy-MM-dd");
                    decimal currentRefresh = GetCurrentRefreshAmount(employeeId, latestDate.Value);
                    hdnCurrentRefreshAmount.Value = currentRefresh.ToString("F2");
                    LoadLatestExpenseMetadata(employeeId, latestDate);
                }
                else
                {
                    DateTime fallbackDate = DateTime.Now.Date;
                    hdnRefreshSubmitDate.Value = fallbackDate.ToString("yyyy-MM-dd");
                    LoadLatestExpenseMetadata(employeeId, fallbackDate);
                }

                if (DateTime.TryParse(hdnRefreshSubmitDate.Value, out DateTime dateToLoad))
                {
                    if (IsExpenseVerified(employeeId, dateToLoad))
                    {
                        LockForm();
                        ShowAlert("This expense is already verified. No further changes allowed.", "info");
                    }
                    else
                    {
                        LoadTravelExpensesByDate(employeeId, dateToLoad);
                    }
                }
                else
                {
                    DateTime fallbackDate = DateTime.Now.Date;
                    hdnRefreshSubmitDate.Value = fallbackDate.ToString("yyyy-MM-dd");
                    LoadTravelExpensesByDate(employeeId, fallbackDate);
                }
            }
        }

        private void LockForm()
        {
            ddlTransportType.Enabled = false;
            txtFromDate.Enabled = false;
            txtFromPlace.Enabled = false;
            txtToPlace.Enabled = false;
            txtAmountConveyance.Enabled = false;
            txtFromTime.Enabled = false;
            txtToTime.Enabled = false;
            txtWBSNo.Enabled = false;
            txtSAPNo.Enabled = false;
            txtReportingManager.Enabled = false;
            txtParticularsConveyance.Enabled = false;

            btnSubmit.Visible = false;
            btnSave.Visible = false;
            btnCancel.Visible = false;
        }
        private bool IsExpenseVerified(int employeeId, DateTime expenseDate)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
                SELECT COUNT(*) 
                FROM TravelExpenses 
                WHERE EmployeeId = @EmployeeId 
                  AND CAST(Date AS DATE) = @ExpenseDate
                  AND Status = 3";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@ExpenseDate", expenseDate.Date);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        private void LoadLatestExpenseMetadata(int employeeId, DateTime? expenseDate)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
                SELECT TOP 1 
                    Date, WBS, SAP, ReportingManager
                FROM TravelExpenses 
                WHERE EmployeeId = @EmployeeId 
                  AND (@ExpenseDate IS NULL OR CAST(Date AS DATE) = @ExpenseDate)
                ORDER BY CreatedDate DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@ExpenseDate", expenseDate.HasValue ? (object)expenseDate.Value.Date : DBNull.Value);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["Date"] != DBNull.Value)
                                txtFromDate.Text = Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd");
                            txtWBSNo.Text = reader["WBS"] as string ?? "";
                            txtSAPNo.Text = reader["SAP"] as string ?? "";
                            txtReportingManager.Text = reader["ReportingManager"] as string ?? "";
                        }
                        else if (expenseDate.HasValue)
                        {
                            txtFromDate.Text = expenseDate.Value.ToString("yyyy-MM-dd");
                        }
                    }
                }
            }
        }

        private DateTime GetMostRecentExpenseDate(int employeeId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT TOP 1 CAST(Date AS DATE) AS ExpenseDate
                    FROM TravelExpenses 
                    WHERE EmployeeId = @EmployeeId 
                    ORDER BY Date DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDateTime(result) : DateTime.Now.Date;
                }
            }
        }

        private void LoadTravelExpensesByDate(int employeeId, DateTime expenseDate)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
                SELECT 
                    Id, Date, TransportType, FromPlace, ToPlace, Amount, 
                    FromTime, ToTime, Particulars, CreatedDate,
                    WBS AS WBSNo,
                    SAP AS SAPNo,
                    ReportingManager
                FROM TravelExpenses 
                WHERE EmployeeId = @EmployeeId 
                  AND CAST(Date AS DATE) = @ExpenseDate
                  AND TransportType != 'Refreshment'
                ORDER BY CreatedDate DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@ExpenseDate", expenseDate.Date);
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        GridViewTravelExpenses.DataSource = dt;
                        GridViewTravelExpenses.DataBind();
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int employeeId = GetEmployeeIdFromCookies();
            if (employeeId == -1)
            {
                ShowAlert("Employee ID not found in cookies.", "error");
                return;
            }

            if (string.IsNullOrEmpty(hdnRefreshSubmitDate.Value) ||
                !DateTime.TryParse(hdnRefreshSubmitDate.Value, out DateTime currentDate))
            {
                ShowAlert("Invalid or missing expense date.", "error");
                return;
            }

            if (IsExpenseVerified(employeeId, currentDate))
            {
                ShowAlert("This expense is already verified. No further changes allowed.", "error");
                return;
            }

            try
            {
                string createdBy = GetFirstNameFromCookies();
                int branchId = 1;
                string transportType = ddlTransportType.SelectedValue;
                string date = txtFromDate.Text;
                string fromPlace = txtFromPlace.Text;
                string toPlace = txtToPlace.Text;
                string amountText = txtAmountConveyance.Text;
                string fromTime = txtFromTime.Text;
                string toTime = txtToTime.Text;
                string particulars = txtParticularsConveyance.Text;

                if (string.IsNullOrEmpty(transportType) || transportType == "-- Select Transport --")
                {
                    ShowAlert("Select Transport Type.", "error");
                    return;
                }
                if (string.IsNullOrEmpty(date))
                {
                    ShowAlert("Select Date.", "error");
                    return;
                }
                if (string.IsNullOrEmpty(amountText) || !decimal.TryParse(amountText, out decimal amount))
                {
                    ShowAlert("Enter valid Amount.", "error");
                    return;
                }
                if (!DateTime.TryParse(date, out DateTime parsedDate))
                {
                    ShowAlert("Invalid Date.", "error");
                    return;
                }

                bool hasFromTime = TimeSpan.TryParse(fromTime, out TimeSpan ft);
                bool hasToTime = TimeSpan.TryParse(toTime, out TimeSpan tt);

                int? editingId = ViewState["EditingId"] as int?;
                int finalId;

                if (editingId.HasValue)
                {
                    UpdateExpense(editingId.Value, employeeId, branchId, transportType, parsedDate, fromPlace, toPlace, amount,
                                  hasFromTime ? ft : (TimeSpan?)null, hasToTime ? tt : (TimeSpan?)null, particulars, createdBy);
                    finalId = editingId.Value;
                }
                else
                {
                    finalId = InsertExpense(employeeId, branchId, transportType, parsedDate, fromPlace, toPlace, amount,
                                            hasFromTime ? ft : (TimeSpan?)null, hasToTime ? tt : (TimeSpan?)null, particulars, createdBy);
                    ViewState["EditingId"] = finalId;
                }

                hdnExpenseId.Value = finalId.ToString();
                ClearForm();

                LoadTravelExpensesByDate(employeeId, currentDate);
                ShowAlert("Travel expense saved successfully!", "success");
            }
            catch (Exception ex)
            {
                ShowAlert($"Error: {ex.Message}", "error");
            }
        }

        private int InsertExpense(int employeeId, int branchId, string transportType, DateTime date, string fromPlace, string toPlace,
                                  decimal amount, TimeSpan? fromTime, TimeSpan? toTime, string particulars, string createdBy)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = @"
                            INSERT INTO TravelExpenses (
                                EmployeeId, BranchId, TransportType, Date, FromPlace, ToPlace,
                                Amount, FromTime, ToTime, Particulars, WBS, SAP, ReportingManager,
                                RefreshAmnt, Status, CreatedDate, CreatedBy
                            ) VALUES (
                                @EmployeeId, @BranchId, @TransportType, @Date, @FromPlace, @ToPlace,
                                @Amount, @FromTime, @ToTime, @Particulars, @WBS, @SAP, @ReportingManager,
                                @RefreshAmnt, @Status, @CreatedDate, @CreatedBy
                            );
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                            cmd.Parameters.AddWithValue("@BranchId", branchId);
                            cmd.Parameters.AddWithValue("@TransportType", transportType);
                            cmd.Parameters.AddWithValue("@Date", date);
                            cmd.Parameters.AddWithValue("@FromPlace", fromPlace);
                            cmd.Parameters.AddWithValue("@ToPlace", toPlace);
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@FromTime", fromTime.HasValue ? (object)fromTime.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@ToTime", toTime.HasValue ? (object)toTime.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                            cmd.Parameters.AddWithValue("@WBS", GetWBS() ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SAP", GetSAP() ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ReportingManager", GetReportingManager() ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@RefreshAmnt", (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Status", 0);
                            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? (object)DBNull.Value);

                            object result = cmd.ExecuteScalar();
                            int newId = Convert.ToInt32(result);
                            transaction.Commit();
                            return newId;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void UpdateExpense(int id, int employeeId, int branchId, string transportType, DateTime date, string fromPlace, string toPlace,
                                   decimal amount, TimeSpan? fromTime, TimeSpan? toTime, string particulars, string createdBy)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    UPDATE TravelExpenses SET
                        TransportType = @TransportType,
                        Date = @Date,
                        FromPlace = @FromPlace,
                        ToPlace = @ToPlace,
                        Amount = @Amount,
                        FromTime = @FromTime,
                        ToTime = @ToTime,
                        Particulars = @Particulars,
                        WBS = @WBS,
                        SAP = @SAP,
                        ReportingManager = @ReportingManager,
                        RefreshAmnt = @RefreshAmnt,
                        Status = @Status,
                        CreatedDate = @CreatedDate,
                        CreatedBy = @CreatedBy
                    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@TransportType", transportType);
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@FromPlace", fromPlace);
                    cmd.Parameters.AddWithValue("@ToPlace", toPlace);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@FromTime", fromTime.HasValue ? (object)fromTime.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToTime", toTime.HasValue ? (object)toTime.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                    cmd.Parameters.AddWithValue("@WBS", GetWBS() ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SAP", GetSAP() ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReportingManager", GetReportingManager() ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RefreshAmnt", (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", 0);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string GetWBS() => txtWBSNo.Text.Trim();
        private string GetSAP() => txtSAPNo.Text.Trim();
        private string GetReportingManager() => string.IsNullOrWhiteSpace(txtReportingManager.Text) ? null : txtReportingManager.Text.Trim();

        protected void btnFinalSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                int employeeId = GetEmployeeIdFromCookies();
                if (employeeId == -1)
                {
                    ShowAlert("Employee ID not found.", "error");
                    return;
                }

                if (string.IsNullOrEmpty(hdnRefreshSubmitDate.Value) ||
                    !DateTime.TryParse(hdnRefreshSubmitDate.Value, out DateTime refreshDate))
                {
                    ShowAlert("Invalid date for refreshment.", "error");
                    return;
                }

                if (IsExpenseVerified(employeeId, refreshDate))
                {
                    ShowAlert("This expense is already verified. No further changes allowed.", "error");
                    return;
                }

                string refreshAmountStr = hdnRefreshAmount.Value;
                if (string.IsNullOrEmpty(refreshAmountStr) || !decimal.TryParse(refreshAmountStr, out decimal refreshAmount))
                {
                    ShowAlert("Invalid refreshment amount.", "error");
                    return;
                }

                SaveRefreshmentAmount(employeeId, refreshDate, refreshAmount);
                SubmitExpensesWithRefreshment(employeeId, refreshDate);

                ShowAlert("Expenses submitted successfully with refreshment amount!", "success");
                hdnRefreshAmount.Value = string.Empty;
                LoadTravelExpensesByDate(employeeId, refreshDate);
            }
            catch (InvalidOperationException ex)
            {
                ShowAlert(ex.Message, "error");
            }
            catch (Exception ex)
            {
                ShowAlert($"Error: {ex.Message}", "error");
            }
        }

        private void SaveRefreshmentAmount(int employeeId, DateTime expenseDate, decimal amount)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string wbs = GetWBS();
            string sap = GetSAP();
            string reportingManager = GetReportingManager();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string findLatestQuery = @"
                    SELECT TOP 1 Id
                    FROM TravelExpenses 
                    WHERE EmployeeId = @EmployeeId 
                      AND CAST(Date AS DATE) = @ExpenseDate
                      AND TransportType != 'Refreshment'
                    ORDER BY CreatedDate DESC";

                SqlCommand findCmd = new SqlCommand(findLatestQuery, conn);
                findCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                findCmd.Parameters.AddWithValue("@ExpenseDate", expenseDate.Date);
                object result = findCmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    int latestId = Convert.ToInt32(result);
                    string updateQuery = @"
                        UPDATE TravelExpenses 
    SET RefreshAmnt = @RefreshAmount,  -- ✅ REPLACE instead of ADD
                            WBS = @WBS,
                            SAP = @SAP,
                            ReportingManager = @ReportingManager,
                            Status = 1,
                            UpdatedDate = GETDATE(),
                            UpdatedBy = @CreatedBy
                        WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RefreshAmount", amount);
                        cmd.Parameters.AddWithValue("@WBS", string.IsNullOrEmpty(wbs) ? (object)DBNull.Value : wbs);
                        cmd.Parameters.AddWithValue("@SAP", string.IsNullOrEmpty(sap) ? (object)DBNull.Value : sap);
                        cmd.Parameters.AddWithValue("@ReportingManager", reportingManager ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", GetFirstNameFromCookies() ?? "System");
                        cmd.Parameters.AddWithValue("@Id", latestId);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    throw new InvalidOperationException("No travel expense record found for this date. Please add a travel expense before submitting refreshment.");
                }
            }
        }

        private void SubmitExpensesWithRefreshment(int employeeId, DateTime expenseDate)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = @"
                    UPDATE TravelExpenses 
                    SET Status = 1 
                    WHERE EmployeeId = @EmployeeId 
                      AND CAST(Date AS DATE) = @ExpenseDate
                      AND Status = 0";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@ExpenseDate", expenseDate.Date);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private int GetEmployeeBranchId(int employeeId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT BranchId FROM Employees WHERE EmployeeId = @EmpId", conn))
                {
                    cmd.Parameters.AddWithValue("@EmpId", employeeId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 1;
                }
            }
        }

        private void ShowAlert(string message, string type)
        {
            string script = $"<script type=\"text/javascript\">showAlert('{message.Replace("'", "\\'")}', '{type}');</script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showalert", script);
        }

        private string GetFirstNameFromCookies()
        {
            HttpCookie firstNameCookie = Request.Cookies["FirstName"];
            return firstNameCookie?.Value;
        }

        private int GetEmployeeIdFromCookies()
        {
            HttpCookie employeeIdCookie = Request.Cookies["EmployeeId"];
            if (employeeIdCookie != null && int.TryParse(employeeIdCookie.Value, out int employeeId))
            {
                return employeeId;
            }
            ShowAlert("Employee ID not found.", "error");
            return -1;
        }

        protected string FormatTime(object timeValue)
        {
            if (timeValue == null || timeValue == DBNull.Value)
                return string.Empty;
            if (timeValue is string str)
                return str.Length >= 5 ? str.Substring(0, 5) : str;
            if (timeValue is TimeSpan ts)
                return ts.ToString(@"hh\:mm");
            return timeValue.ToString();
        }

        protected void GridViewTravelExpenses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int employeeId = GetEmployeeIdFromCookies();
            DateTime currentDate = DateTime.Parse(hdnRefreshSubmitDate.Value);
            if (IsExpenseVerified(employeeId, currentDate))
            {
                ShowAlert("This expense is already verified. No further changes allowed.", "error");
                return;
            }

            if (e.CommandName == "EditRecord")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                LoadExpenseForEditing(id);
            }
            else if (e.CommandName == "DeleteRecord")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                DeleteExpense(id);
            }
        }

        private void LoadExpenseForEditing(int expenseId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
                SELECT 
                    TransportType, Date, FromPlace, ToPlace, Amount, FromTime, ToTime, Particulars,
                    WBS, SAP, ReportingManager
                FROM TravelExpenses 
                WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", expenseId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ddlTransportType.SelectedValue = reader["TransportType"].ToString();
                            txtFromDate.Text = Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd");
                            txtFromPlace.Text = reader["FromPlace"].ToString();
                            txtToPlace.Text = reader["ToPlace"].ToString();
                            txtAmountConveyance.Text = reader["Amount"].ToString();

                            if (reader["FromTime"] != DBNull.Value)
                                txtFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                            if (reader["ToTime"] != DBNull.Value)
                                txtToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");

                            txtParticularsConveyance.Text = reader["Particulars"].ToString();
                            txtWBSNo.Text = reader["WBS"] as string ?? "";
                            txtSAPNo.Text = reader["SAP"] as string ?? "";
                            txtReportingManager.Text = reader["ReportingManager"] as string ?? "";

                            ViewState["EditingId"] = expenseId;
                            ScriptManager.RegisterStartupScript(this, GetType(), "showDetails", "toggleTransportFields();", true);
                        }
                    }
                }
            }
        }

        private void DeleteExpense(int id)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM TravelExpenses WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowAlert("Record deleted successfully.", "success");
                int empId = GetEmployeeIdFromCookies();
                if (empId != -1)
                {
                    DateTime currentDate = DateTime.Parse(hdnRefreshSubmitDate.Value);
                    LoadTravelExpensesByDate(empId, currentDate);
                }
            }
            catch (Exception ex)
            {
                ShowAlert($"Error deleting record: {ex.Message}", "error");
            }
        }

        private void ClearForm()
        {
            ddlTransportType.SelectedIndex = 0;
            txtFromDate.Text = string.Empty;
            txtFromPlace.Text = string.Empty;
            txtToPlace.Text = string.Empty;
            txtAmountConveyance.Text = string.Empty;
            txtFromTime.Text = string.Empty;
            txtToTime.Text = string.Empty;
            txtParticularsConveyance.Text = string.Empty;
            txtWBSNo.Text = string.Empty;
            txtSAPNo.Text = string.Empty;
            txtReportingManager.Text = string.Empty;
            ViewState["EditingId"] = null;
            hdnExpenseId.Value = string.Empty;
            // 👉 DO NOT clear hdnRefreshSubmitDate
        }

        private void RebindGridView()
        {
            int employeeId = GetEmployeeIdFromCookies();
            if (employeeId != -1)
            {
                DateTime currentDate = DateTime.Parse(hdnRefreshSubmitDate.Value);
                LoadTravelExpensesByDate(employeeId, currentDate);
            }
            else
            {
                GridViewTravelExpenses.DataSource = null;
                GridViewTravelExpenses.DataBind();
            }
        }
        private decimal GetCurrentRefreshAmount(int employeeId, DateTime expenseDate)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
        SELECT ISNULL(MAX(RefreshAmnt), 0)
        FROM TravelExpenses
        WHERE EmployeeId = @EmployeeId 
          AND CAST(Date AS DATE) = @ExpenseDate
          AND RefreshAmnt IS NOT NULL";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@ExpenseDate", expenseDate.Date);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
                }
            }
        }
        protected void GridViewTravelExpenses_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int employeeId = GetEmployeeIdFromCookies();
            DateTime currentDate = DateTime.Parse(hdnRefreshSubmitDate.Value);
            if (IsExpenseVerified(employeeId, currentDate))
            {
                ShowAlert("This expense is already verified. No further changes allowed.", "error");
                GridViewTravelExpenses.EditIndex = -1;
                RebindGridView();
                return;
            }

            try
            {
                int id = Convert.ToInt32(GridViewTravelExpenses.DataKeys[e.RowIndex].Value);
                GridViewRow row = GridViewTravelExpenses.Rows[e.RowIndex];
                TextBox txtDate = (TextBox)row.FindControl("txtEditDate");
                DropDownList ddlTransport = (DropDownList)row.FindControl("ddlEditTransportType");
                TextBox txtFrom = (TextBox)row.FindControl("txtEditFromPlace");
                TextBox txtTo = (TextBox)row.FindControl("txtEditToPlace");
                TextBox txtAmount = (TextBox)row.FindControl("txtEditAmount");
                TextBox txtFromTime = (TextBox)row.FindControl("txtEditFromTime");
                TextBox txtToTime = (TextBox)row.FindControl("txtEditToTime");
                TextBox txtParticulars = (TextBox)row.FindControl("txtEditParticulars");

                if (!DateTime.TryParse(txtDate.Text, out DateTime date))
                    throw new Exception("Invalid Date");
                if (!decimal.TryParse(txtAmount.Text, out decimal amount))
                    throw new Exception("Invalid Amount");
                if (!TimeSpan.TryParse(txtFromTime.Text, out TimeSpan fromTime))
                    throw new Exception("Invalid From Time");
                if (!TimeSpan.TryParse(txtToTime.Text, out TimeSpan toTime))
                    throw new Exception("Invalid To Time");

                string connStr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE TravelExpenses 
                        SET Date = @Date, TransportType = @TransportType, FromPlace = @FromPlace, 
                            ToPlace = @ToPlace, Amount = @Amount, FromTime = @FromTime, 
                            ToTime = @ToTime, Particulars = @Particulars
                        WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Date", date);
                        cmd.Parameters.AddWithValue("@TransportType", ddlTransport.SelectedValue);
                        cmd.Parameters.AddWithValue("@FromPlace", txtFrom.Text);
                        cmd.Parameters.AddWithValue("@ToPlace", txtTo.Text);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@FromTime", fromTime);
                        cmd.Parameters.AddWithValue("@ToTime", toTime);
                        cmd.Parameters.AddWithValue("@Particulars", txtParticulars.Text ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                GridViewTravelExpenses.EditIndex = -1;
                LoadTravelExpensesByDate(employeeId, currentDate);
                ShowAlert("Record updated successfully.", "success");
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating: " + ex.Message, "error");
            }
        }
    }
}