using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.IO;
using System.Diagnostics;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using NPOI.SS.Formula.Functions;
using System.Data.SqlTypes;
using System.Linq;

namespace Vivify
{
    public partial class AdminVerify : System.Web.UI.Page
    {
        private const string ConnectionStringName = "vivify";
        private const string ServiceIdSessionKey = "ServiceId";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["TotalClaimable"] = 0m;
                Session["TotalNonClaimable"] = 0m;

                // Display employee name from session
                if (Session["EmployeeName"] != null)
                {
                    lblEmployeeName.Text = "Employee: " + Session["EmployeeName"].ToString();
                }
                else
                {
                    lblEmployeeName.Text = "Employee: N/A";
                }

                if (Session[ServiceIdSessionKey] is int serviceId)
                {
                    BindExpenseGridView(serviceId);
                }
                else
                {
                    lblMessage.Text = "Service ID not found in session.";
                    lblMessage.Visible = true;
                }
            }
        }
        // Method to check if a specific attachment exists
        protected bool HasAttachment(object expenseId, string sourceTable, string columnName)
        {
            try
            {
                if (expenseId == null || expenseId == DBNull.Value || string.IsNullOrEmpty(sourceTable))
                    return false;

                int id = Convert.ToInt32(expenseId);
                string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
                int serviceId = Session[ServiceIdSessionKey] is int ? (int)Session[ServiceIdSessionKey] : 0;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = $@"
                SELECT {columnName} 
                FROM {sourceTable} 
                WHERE Id = @ExpenseId AND ServiceId = @ServiceId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExpenseId", id);
                        cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        con.Open();
                        object result = cmd.ExecuteScalar();

                        // Check if the result is not null and contains data
                        if (result != null && result != DBNull.Value)
                        {
                            byte[] fileData = (byte[])result;
                            return fileData.Length > 0;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking {columnName} attachment: " + ex.Message);
                return false;
            }
        }

        // Method to check if any attachment exists for an expense
        protected bool HasAnyAttachment(object expenseId, string sourceTable)
        {
            try
            {
                if (expenseId == null || expenseId == DBNull.Value || string.IsNullOrEmpty(sourceTable))
                    return false;

                int id = Convert.ToInt32(expenseId);
                string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
                int serviceId = Session[ServiceIdSessionKey] is int ? (int)Session[ServiceIdSessionKey] : 0;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    // Check common attachment columns for each table
                    string query = "";

                    if (sourceTable == "Lodging" || sourceTable == "Others")
                    {
                        query = $@"
                    SELECT 
                        CASE 
                            WHEN (Image IS NOT NULL AND DATALENGTH(Image) > 0) OR
                                 (ServiceReport IS NOT NULL AND DATALENGTH(ServiceReport) > 0) OR
                                 (ApprovalMail IS NOT NULL AND DATALENGTH(ApprovalMail) > 0) 
                            THEN 1 
                            ELSE 0 
                        END 
                    FROM {sourceTable} 
                    WHERE Id = @ExpenseId AND ServiceId = @ServiceId";
                    }
                    else if (sourceTable == "Conveyance" || sourceTable == "Miscellaneous")
                    {
                        query = $@"
                    SELECT 
                        CASE 
                            WHEN (Image IS NOT NULL AND DATALENGTH(Image) > 0) 
                            THEN 1 
                            ELSE 0 
                        END 
                    FROM {sourceTable} 
                    WHERE Id = @ExpenseId AND ServiceId = @ServiceId";
                    }
                    else
                    {
                        return false; // Food table doesn't have attachments
                    }

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExpenseId", id);
                        cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        return result != null && Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking any attachment: " + ex.Message);
                return false;
            }
        }
        protected void BindExpenseGridView(int serviceId)
        {
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string qry = @"
                SELECT 
                    C.Id AS ExpenseId,
                    'Conveyance' AS SourceTable,
                    c.ExpenseType,
                    FORMAT(c.Date, 'dd/MMM/yyyy') AS Date,
                    c.FromTime, 
                    c.ToTime, 
                    c.Particulars,
                    c.Distance,     
                    ISNULL(c.ClaimedAmount, c.Amount) AS Amount,
                    ISNULL(e.ClaimedAmount, 0) AS ClaimedAmount,
                    ISNULL(e.NonClaimedAmount, 0) AS NonClaimedAmount,
                    c.Remarks,
                    c.Date AS OrderDate,
ISNULL(c.IsClaimable, 0) AS IsClaimable,
                    c.Image AS Image -- Add this line for the Image column
                FROM 
                    Conveyance c
                LEFT JOIN Expense e ON e.ServiceId = c.ServiceId
                WHERE 
                    c.ServiceId = @ServiceId

                UNION ALL

                SELECT 
                    f.Id AS ExpenseId,
                    'Food' AS SourceTable,
                    f.ExpenseType, 
                    FORMAT(f.Date, 'dd/MMM/yyyy') AS Date,
                    f.FromTime, 
                    f.ToTime, 
                    f.Particulars,
                    NULL AS Distance,
                    ISNULL(f.ClaimedAmount, f.Amount) AS Amount,
                    ISNULL(e.ClaimedAmount, 0) AS ClaimedAmount,
                    ISNULL(e.NonClaimedAmount, 0) AS NonClaimedAmount,
                    f.Remarks,
                    f.Date AS OrderDate,
ISNULL(f.IsClaimable, 0) AS IsClaimable,
                    NULL AS Image -- Add this line for the Image column
                FROM 
                    Food f
                LEFT JOIN Expense e ON e.ServiceId = f.ServiceId
                WHERE 
                    f.ServiceId = @ServiceId

                UNION ALL

                SELECT 
                    o.Id AS ExpenseId,
                    'Others' AS SourceTable,
                    o.ExpenseType, 
                    FORMAT(o.Date, 'dd/MMM/yyyy') AS Date,
                    o.FromTime, 
                    o.ToTime, 
                    o.Particulars,
                    NULL AS Distance,
                    ISNULL(o.ClaimedAmount, o.Amount) AS Amount,
                    ISNULL(e.ClaimedAmount, 0) AS ClaimedAmount,
                    ISNULL(e.NonClaimedAmount, 0) AS NonClaimedAmount,
                    o.Remarks,
                    o.Date AS OrderDate,
 ISNULL(o.IsClaimable, 0) AS IsClaimable,
                    o.Image AS Image -- Add this line for the Image column
                FROM 
                    Others o
                LEFT JOIN Expense e ON e.ServiceId = o.ServiceId
                WHERE 
                    o.ServiceId = @ServiceId

                UNION ALL

                SELECT 
                    m.Id AS ExpenseId,
                    'Miscellaneous' AS SourceTable,
                    m.ExpenseType, 
                    FORMAT(m.Date, 'dd/MMM/yyyy') AS Date,
                    m.FromTime, 
                    m.ToTime, 
                    m.Particulars,
                    NULL AS Distance,
                    ISNULL(m.ClaimedAmount, m.Amount) AS Amount,
                    ISNULL(e.ClaimedAmount, 0) AS ClaimedAmount,
                    ISNULL(e.NonClaimedAmount, 0) AS NonClaimedAmount,
                    m.Remarks,
                    m.Date AS OrderDate,
ISNULL(m.IsClaimable, 0) AS IsClaimable,
                    m.Image AS Image -- Add this line for the Image column
                FROM 
                    Miscellaneous m
                LEFT JOIN Expense e ON e.ServiceId = m.ServiceId
                WHERE 
                    m.ServiceId = @ServiceId

                UNION ALL

                SELECT 
                    l.Id AS ExpenseId,
                    'Lodging' AS SourceTable,
                    l.ExpenseType, 
                    FORMAT(l.Date, 'dd/MMM/yyyy') AS Date,
                    l.FromTime, 
                    l.ToTime, 
                    l.Particulars,
                    NULL AS Distance,
                    ISNULL(l.ClaimedAmount, l.Amount) AS Amount,
                    ISNULL(e.ClaimedAmount, 0) AS ClaimedAmount,
                    ISNULL(e.NonClaimedAmount, 0) AS NonClaimedAmount,
                    l.Remarks,
                    l.Date AS OrderDate,
ISNULL(l.IsClaimable, 0) AS IsClaimable,
                    l.Image AS Image -- Add this line for the Image column
                FROM 
                    Lodging l
                LEFT JOIN Expense e ON e.ServiceId = l.ServiceId
                WHERE 
                    l.ServiceId = @ServiceId

                ORDER BY OrderDate DESC"; // Use the newly added OrderDate field for sorting

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        ExpenseGridView.DataSource = dt;
                        ExpenseGridView.DataBind();
                    }
                }
            }
        }
        protected string GetFriendlySourceName(string sourceTable)
        {
            switch (sourceTable)
            {
                case "Lodging":
                    return "Lodging";
                case "Conveyance":
                    return "Conveyance";
                case "Miscellaneous":
                    return "Miscellaneous";
                case "Food":
                    return "Food";
                case "Others":
                    return "Others";
                default:
                    return sourceTable; // Fallback to raw value if no match
            }
        }

        protected void ExpenseGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                // Get the CommandArgument which now contains "ExpenseId|SourceTable"
                string[] args = e.CommandArgument.ToString().Split('|');
                if (args.Length != 2)
                {
                    lblMessage.Text = "Invalid command arguments.";
                    lblMessage.Visible = true;
                    return;
                }

                int expenseId = Convert.ToInt32(args[0]);
                string sourceTable = args[1];
                string expenseType = string.Empty;
                byte[] pdfBytes = null;

                // Handle different commands based on CommandName
                switch (e.CommandName)
                {
                    case "ViewPDF":
                        // Check for Image in appropriate tables
                        if (new[] { "Lodging", "Others", "Conveyance", "Miscellaneous" }.Contains(sourceTable))
                        {
                            pdfBytes = GetPDFBytesFromDatabaseBySource(expenseId, sourceTable, "Image", out expenseType);
                        }
                        break;

                    case "ViewServiceReport":
                        // Check for ServiceReport in Others and Lodging
                        if (new[] { "Lodging", "Others" }.Contains(sourceTable))
                        {
                            pdfBytes = GetPDFBytesFromDatabaseBySource(expenseId, sourceTable, "ServiceReport", out expenseType);
                        }
                        break;

                    case "ViewApprovalMail":
                        // Check for ApprovalMail in Others and Lodging
                        if (new[] { "Lodging", "Others" }.Contains(sourceTable))
                        {
                            pdfBytes = GetPDFBytesFromDatabaseBySource(expenseId, sourceTable, "ApprovalMail", out expenseType);
                        }
                        break;

                    default:
                        lblMessage.Text = "Invalid command.";
                        lblMessage.Visible = true;
                        return;
                }

                // If a PDF is found, send it to the browser
                if (pdfBytes != null && pdfBytes.Length > 0)
                {
                    // Your existing PDF display code here
                    string base64 = Convert.ToBase64String(pdfBytes);
                    string script = $@"
                var byteCharacters = atob('{base64}');
                var byteNumbers = new Array(byteCharacters.length);
                for (var i = 0; i < byteCharacters.length; i++) {{
                    byteNumbers[i] = byteCharacters.charCodeAt(i);
                }}
                var byteArray = new Uint8Array(byteNumbers);
                var blob = new Blob([byteArray], {{ type: 'application/pdf' }});
                var url = URL.createObjectURL(blob);
                window.open(url, '_blank');";

                    ClientScript.RegisterStartupScript(this.GetType(), "OpenPDF", script, true);
                }
                else
                {
                    lblMessage.Text = $"No {e.CommandName.Replace("View", "")} found for this expense.";
                    lblMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"An error occurred: {ex.Message}";
                lblMessage.Visible = true;
            }
        }

        // Helper method to get PDF bytes by source table
        private byte[] GetPDFBytesFromDatabaseBySource(int expenseId, string sourceTable, string columnName, out string expenseType)
        {
            expenseType = sourceTable;
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            int serviceId = Session[ServiceIdSessionKey] is int ? (int)Session[ServiceIdSessionKey] : 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = $@"
            SELECT {columnName} 
            FROM {sourceTable} 
            WHERE Id = @ExpenseId AND ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? (byte[])result : null;
                }
            }
        }
        private byte[] GetPDFBytesFromDatabase(int expenseId, out string expenseType)
        {
            expenseType = "Conveyance"; // Default to Conveyance
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            int serviceId = Session[ServiceIdSessionKey] is int ? (int)Session[ServiceIdSessionKey] : 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"
                SELECT Image 
                FROM Conveyance 
                WHERE Id = @ExpenseId AND ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? (byte[])result : null;
                }
            }
        }

        private byte[] GetPDFBytesFromDatabaseForMiscellaneous(int expenseId, out string expenseType)
        {
            expenseType = "Miscellaneous";
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            int serviceId = Session[ServiceIdSessionKey] is int ? (int)Session[ServiceIdSessionKey] : 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"
                SELECT Image 
                FROM Miscellaneous 
                WHERE Id = @ExpenseId AND ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? (byte[])result : null;
                }
            }
        }

        private byte[] GetPDFBytesFromDatabaseForOthers(int expenseId, out string expenseType, string columnName = "Image")
        {
            expenseType = "Others"; // Default to Others
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            int serviceId = Session[ServiceIdSessionKey] is int ? (int)Session[ServiceIdSessionKey] : 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = $@"
                SELECT {columnName} 
                FROM Others 
                WHERE Id = @ExpenseId AND ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? (byte[])result : null;
                }
            }
        }

        private byte[] GetPDFBytesFromDatabaseForLodging(int expenseId, out string expenseType, string columnName = "Image")
        {
            expenseType = "Lodging"; // Default to Lodging
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            int serviceId = Session[ServiceIdSessionKey] is int ? (int)Session[ServiceIdSessionKey] : 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = $@"
                SELECT {columnName} 
                FROM Lodging 
                WHERE Id = @ExpenseId AND ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? (byte[])result : null;
                }
            }
        }

        // Renamed method for Claimed data export
        private void ExportClaimedToExcel(DataTable dtExport, string fileName)
        {
            // Your code to export claimed expenses
        }

        // Renamed method for NonClaimed data export
        private void ExportNonClaimedToExcel(DataTable dtExport, string fileName)
        {
            // Your code to export non-claimed expenses
        }
        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            // Ensure EPPlus LicenseContext is set
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // Create DataTables for Claimed and NonClaimed
            DataTable dtClaimed = new DataTable();
            DataTable dtNonClaimed = new DataTable();

            // Add columns to both DataTables (Claimed and NonClaimed), including the "Amount" column
            // Add columns to both DataTables (Claimed and NonClaimed), excluding the "ExpenseId" column
            foreach (DataControlField column in ExpenseGridView.Columns)
            {
                if (column.Visible && !(column is BoundField bf && bf.DataField == "ExpenseId")) // Exclude ExpenseId column
                {
                    if (column is BoundField)
                    {
                        var columnHeader = ((BoundField)column).HeaderText;

                        // Add columns to DataTables, excluding "ClaimedAmount" as per existing logic
                        if (columnHeader != "ClaimedAmount")
                        {
                            dtClaimed.Columns.Add(columnHeader);
                            dtNonClaimed.Columns.Add(columnHeader);
                        }
                    }
                    else if (column is TemplateField)
                    {
                        // TemplateField handling
                        dtClaimed.Columns.Add(column.HeaderText);
                        dtNonClaimed.Columns.Add(column.HeaderText);
                    }
                }
            }

            // Loop through GridView rows to get all column data
            foreach (GridViewRow row in ExpenseGridView.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    DataRow dataRowClaimed = dtClaimed.NewRow();
                    DataRow dataRowNonClaimed = dtNonClaimed.NewRow();

                    int columnIndex = 0; // Column index for DataRow population
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        if (row.Cells[i].Visible && ExpenseGridView.Columns[i].HeaderText != "Id") // Skip Id column
                        {
                            dataRowClaimed[columnIndex] = row.Cells[i].Text; // Add data from cells to DataTable
                            dataRowNonClaimed[columnIndex] = row.Cells[i].Text;
                            columnIndex++;
                        }
                    }

                    // Manually handle FromTime, ToTime, Amount, and Claimable logic
                    var fromTimeLabel = row.FindControl("lblFromTime") as Label;
                    var toTimeLabel = row.FindControl("lblToTime") as Label;
                    var amountTextBox = row.FindControl("txtConveyanceAmount") as TextBox;
                    var chkClaimable = row.FindControl("chkConveyanceClaimable") as CheckBox;

                    if (fromTimeLabel != null)
                    {
                        dataRowClaimed["From Time"] = fromTimeLabel.Text;
                        dataRowNonClaimed["From Time"] = fromTimeLabel.Text;
                    }

                    if (toTimeLabel != null)
                    {
                        dataRowClaimed["To Time"] = toTimeLabel.Text;
                        dataRowNonClaimed["To Time"] = toTimeLabel.Text;
                    }

                    if (amountTextBox != null)
                    {
                        if (chkClaimable != null && chkClaimable.Checked)
                        {
                            dataRowClaimed["Amount"] = amountTextBox.Text;
                            dtClaimed.Rows.Add(dataRowClaimed);
                        }
                        else
                        {
                            dataRowNonClaimed["Amount"] = amountTextBox.Text;
                            dtNonClaimed.Rows.Add(dataRowNonClaimed);
                        }
                    }
                }
            }


            // Now create Excel package and export data
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                // Create worksheet for Claimed data
                var worksheetClaimed = package.Workbook.Worksheets.Add("Claimed Expense Report");
                worksheetClaimed.Cells["A1"].LoadFromDataTable(dtClaimed, true);

                // Create worksheet for NonClaimed data
                var worksheetNonClaimed = package.Workbook.Worksheets.Add("NonClaimed Expense Report");
                worksheetNonClaimed.Cells["A1"].LoadFromDataTable(dtNonClaimed, true);

                // Create a memory stream to hold the Excel file data
                using (var stream = new MemoryStream())
                {
                    // Save the package to the memory stream
                    package.SaveAs(stream);

                    // Send the file to the browser
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=Expense_Report.xlsx");
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
        }


        private void ExportToExcel(DataTable dtExport, string fileName)
        {
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                // Create a worksheet for the data
                var worksheet = package.Workbook.Worksheets.Add("Expense Report");
                worksheet.Cells["A1"].LoadFromDataTable(dtExport, true);

                // Create a memory stream to hold the Excel file data
                using (var stream = new MemoryStream())
                {
                    // Save the package to the memory stream
                    package.SaveAs(stream);

                    // Send the file to the browser
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
        }

        private void ProcessExpense(DataTable dtClaimed, DataTable dtNonClaimed, CheckBox chkClaimable, string source, string expenseType, string date, string fromTime, string toTime, string particulars, string distance, string remarks, string amount, GridViewRow row)
        {
            TextBox amountTextBox = (TextBox)row.FindControl($"txt{source}Amount");
            amount = amountTextBox != null ? amountTextBox.Text : string.Empty;

            if (chkClaimable != null && chkClaimable.Checked)
            {
                DataRow claimedRow = dtClaimed.NewRow();
                claimedRow["Source"] = source;
                claimedRow["ExpenseType"] = expenseType;
                claimedRow["Date"] = date;
                claimedRow["FromTime"] = fromTime;
                claimedRow["ToTime"] = toTime;
                claimedRow["Particulars"] = particulars;
                claimedRow["Distance"] = distance;
                claimedRow["Remarks"] = remarks;
                claimedRow["Amount"] = amount;
                dtClaimed.Rows.Add(claimedRow);
            }
            else
            {
                if (!string.IsNullOrEmpty(amount) && decimal.TryParse(amount, out decimal parsedAmount) && parsedAmount > 0)
                {
                    DataRow nonClaimedRow = dtNonClaimed.NewRow();
                    nonClaimedRow["Source"] = source;
                    nonClaimedRow["ExpenseType"] = expenseType;
                    nonClaimedRow["Date"] = date;
                    nonClaimedRow["FromTime"] = fromTime;
                    nonClaimedRow["ToTime"] = toTime;
                    nonClaimedRow["Particulars"] = particulars;
                    nonClaimedRow["Distance"] = distance;
                    nonClaimedRow["Remarks"] = remarks;
                    nonClaimedRow["Amount"] = amount;
                    dtNonClaimed.Rows.Add(nonClaimedRow);
                }
            }
        }






        private DataTable GetExpenseData()
        {
            DataTable dt = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string qry = @"
      SELECT 
          'Conveyance' AS Source, 
          c.ExpenseType, 
          FORMAT(c.Date, 'dd/MMM/yyyy') AS Date,
          c.FromTime, 
          c.ToTime, 
          c.Particulars,  
          c.Distance,     
          c.Remarks,       
          c.Amount AS Amount
      FROM Conveyance c
      WHERE c.ServiceId = @ServiceId
      UNION ALL
      SELECT 
          'Food' AS Source,
          f.ExpenseType, 
          FORMAT(f.Date, 'dd/MMM/yyyy') AS Date,
          f.FromTime, 
          f.ToTime, 
          f.Particulars,  
          NULL AS Distance,     
          f.Remarks,       
          f.Amount AS Amount
      FROM Food f
      WHERE f.ServiceId = @ServiceId
      UNION ALL
      SELECT 
          'Others' AS Source,
          o.ExpenseType, 
          FORMAT(o.Date, 'dd/MMM/yyyy') AS Date,
          o.FromTime, 
          o.ToTime, 
          o.Particulars,  
          NULL AS Distance,     
          o.Remarks,       
          o.Amount AS Amount
      FROM Others o
      WHERE o.ServiceId = @ServiceId
      UNION ALL
      SELECT 
          'Miscellaneous' AS Source,
          m.ExpenseType, 
          FORMAT(m.Date, 'dd/MMM/yyyy') AS Date,
          m.FromTime, 
          m.ToTime, 
          m.Particulars,  
          NULL AS Distance,     
          m.Remarks,       
          m.Amount AS Amount
      FROM Miscellaneous m
      WHERE m.ServiceId = @ServiceId
      UNION ALL
      SELECT 
          'Lodging' AS Source,
          l.ExpenseType, 
          FORMAT(l.Date, 'dd/MMM/yyyy') AS Date,
          l.FromTime, 
          l.ToTime, 
          l.Particulars,  
          NULL AS Distance,     
          l.Remarks,       
          l.Amount AS Amount
      FROM Lodging l
      WHERE l.ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", Convert.ToInt32(Session[ServiceIdSessionKey]));
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }
        //protected void ExpenseGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "EditRow")
        //    {
        //        int rowIndex = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = ExpenseGridView.Rows[rowIndex];
        //        ToggleTextBoxReadOnly(row);
        //    }
        //    }
        //}

        private void ToggleTextBoxReadOnly(GridViewRow row)
        {
            string[] textBoxIds = { "txtConveyanceAmount", "txtFoodAmount", "txtOthersAmount", "txtMiscellaneousAmount", "txtLodgingAmount" };
            foreach (var id in textBoxIds)
            {
                TextBox textBox = (TextBox)row.FindControl(id);
                if (textBox != null)
                {
                    textBox.ReadOnly = !textBox.ReadOnly; // Toggle ReadOnly state
                }
            }
        }
        protected void chkClaimable_CheckedChanged(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((CheckBox)sender).NamingContainer;
            UpdateClaimableStatus(row);
            UpdateTotalAmounts();
        }

        private void UpdateClaimableStatus(GridViewRow row)
        {
            string category = row.Cells[1]?.Text; // Assuming the category is in the second cell
            if (string.IsNullOrEmpty(category)) return;

            CheckBox checkBox = (CheckBox)row.FindControl($"chk{category}Claimable");
            decimal amount = GetAmountFromTextBox(row, $"txt{category}Amount");

            if (checkBox != null) // Ensure the checkbox is found
            {
                // Determine the database table and update amount based on the category
                string tableName = "";

                switch (category)
                {
                    case "Conveyance":
                        tableName = "Conveyance";
                        break;
                    case "Food":
                        tableName = "Food";
                        break;
                    case "Others":
                        tableName = "Others";
                        break;
                    case "Miscellaneous":
                        tableName = "Miscellaneous";
                        break;
                    case "Lodging":
                        tableName = "Lodging";
                        break;
                    default:
                        return; // Unknown category, exit the method
                }

                if (checkBox.Checked)
                {
                    UpdateClaimedAmounts(tableName, amount, true);
                }
                else
                {
                    UpdateClaimedAmounts(tableName, amount, false);
                }
            }
        }

        private void UpdateClaimedAmounts(string tableName, decimal amount, bool isClaimed)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string updateQuery = $@"
            UPDATE {tableName} 
            SET ClaimedAmount = @ClaimedAmount,
                NonClaimedAmount = @NonClaimedAmount
            WHERE ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", Convert.ToInt32(Session[ServiceIdSessionKey]));

                    // If claimed, set the ClaimedAmount and NonClaimedAmount should be 0.
                    // If not claimed, set the ClaimedAmount to 0 and NonClaimedAmount to the amount.
                    cmd.Parameters.AddWithValue("@ClaimedAmount", isClaimed ? amount : 0);
                    cmd.Parameters.AddWithValue("@NonClaimedAmount", isClaimed ? 0 : amount);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateTotalAmounts()
        {
            decimal totalClaimed = 0;
            decimal totalNonClaimed = 0;

            foreach (GridViewRow row in ExpenseGridView.Rows)
            {
                totalClaimed += CalculateTotalForRow(row);
            }

            foreach (GridViewRow row in ExpenseGridView.Rows)
            {
                string[] categories = { "Conveyance", "Food", "Others", "Miscellaneous", "Lodging" };
                foreach (var category in categories)
                {
                    CheckBox checkBox = (CheckBox)row.FindControl($"chk{category}Claimable");
                    decimal amount = GetAmountFromTextBox(row, $"txt{category}Amount");

                    // If not checked, this amount goes into the non-claimed total
                    if (checkBox != null && !checkBox.Checked)
                    {
                        totalNonClaimed += amount;
                    }
                }
            }

            txtTotalClaimedAmount.Text = totalClaimed.ToString("0.00");
            txtTotalNonClaimedAmount.Text = totalNonClaimed.ToString("0.00");
        }

        private decimal CalculateTotalForRow(GridViewRow row)
        {
            decimal total = 0;
            string[] categories = { "Conveyance", "Food", "Others", "Miscellaneous", "Lodging" };

            foreach (var category in categories)
            {
                total += UpdateTotalBasedOnCheckbox(row, $"chk{category}Claimable", GetAmountFromTextBox(row, $"txt{category}Amount"));
            }

            return total;
        }

        private decimal UpdateTotalBasedOnCheckbox(GridViewRow row, string checkBoxId, decimal amount)
        {
            CheckBox checkBox = (CheckBox)row.FindControl(checkBoxId);
            return checkBox != null && checkBox.Checked ? amount : 0;
        }


        private decimal GetAmountFromTextBox(GridViewRow row, string textBoxId)
        {
            TextBox textBox = (TextBox)row.FindControl(textBoxId);
            decimal.TryParse(textBox?.Text, out decimal amount);
            return amount;
        }

        protected void btnEditRow_Click(object sender, EventArgs e)
        {
            Button btnEdit = (Button)sender;
            GridViewRow row = (GridViewRow)btnEdit.NamingContainer;
            ToggleTextBoxReadOnly(row);
            btnEdit.Text = btnEdit.Text == "Edit" ? "Save" : "Edit";
            UpdateTotalAmounts();
        }

        private int GetCurrentEmployeeId()
        {
            // Retrieve the current EmployeeId from session or other relevant source
            // For example:
            return Session["EmployeeId"] is int empId ? empId : 0;  // Default to 0 if not found
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            decimal totalClaimedAmount = decimal.TryParse(txtTotalClaimedAmount.Text, out decimal claimedAmount) ? claimedAmount : 0;
                            decimal totalNonClaimedAmount = decimal.TryParse(txtTotalNonClaimedAmount.Text, out decimal nonClaimedAmount) ? nonClaimedAmount : 0;

                            // Update Expense table — always allow update
                            string updateExpenseQuery = @"
                    UPDATE Expense
                    SET ClaimedAmount = @ClaimedAmount,
                        NonClaimedAmount = @NonClaimedAmount,
                        StatusId = 3
                    WHERE ServiceId = @ServiceId";

                            using (SqlCommand cmd = new SqlCommand(updateExpenseQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClaimedAmount", totalClaimedAmount);
                                cmd.Parameters.AddWithValue("@NonClaimedAmount", totalNonClaimedAmount);
                                cmd.Parameters.AddWithValue("@ServiceId", Convert.ToInt32(Session[ServiceIdSessionKey]));
                                cmd.ExecuteNonQuery();
                            }

                            // Also update Services table
                            string updateServiceQuery = @"
                    UPDATE Services
                    SET StatusId = 3
                    WHERE ServiceId = @ServiceId";

                            using (SqlCommand cmd2 = new SqlCommand(updateServiceQuery, con, transaction))
                            {
                                cmd2.Parameters.AddWithValue("@ServiceId", Convert.ToInt32(Session[ServiceIdSessionKey]));
                                cmd2.ExecuteNonQuery();
                            }

                            StoreClaimedAndNonClaimedAmounts(con, transaction);
                            transaction.Commit();

                            string successScript = "alert('Amounts and status updated successfully!');";
                            ClientScript.RegisterStartupScript(this.GetType(), "alertSuccess", successScript, true);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            string errorScript = $"alert('An error occurred: {ex.Message}');";
                            ClientScript.RegisterStartupScript(this.GetType(), "alertError", errorScript, true);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string generalErrorScript = $"alert('An error occurred: {ex.Message}');";
                ClientScript.RegisterStartupScript(this.GetType(), "alertGeneralError", generalErrorScript, true);
            }
        }


        private void StoreClaimedAndNonClaimedAmounts(SqlConnection con, SqlTransaction transaction)
        {
            foreach (GridViewRow row in ExpenseGridView.Rows)
            {
                decimal claimedAmount = 0;
                decimal nonClaimedAmount = 0;

                CheckBox checkBox = (CheckBox)row.Cells[0].FindControl("chkConveyanceClaimable");
                var txtAmount = (TextBox)row.Cells[0].FindControl("txtConveyanceAmount");
                decimal amount = 0;
                if (txtAmount != null && txtAmount.Text != "")
                    amount = Convert.ToDecimal(txtAmount.Text);

                string id = row.Cells[0].Text; // Assuming the Id is in the first column
                string sourceTable = row.Cells[1].Text; // Assuming the source table is in the second column

                if (!int.TryParse(id, out int parsedId))
                {
                    lblMessage.Text = "Invalid Id value.";
                    lblMessage.Visible = true;
                    return;
                }

                if (checkBox != null)
                {
                    if (checkBox.Checked)
                    {
                        claimedAmount = amount;
                    }
                    else
                    {
                        nonClaimedAmount = amount;
                    }
                }

                PersistClaimableStatusAndAmounts(sourceTable, checkBox.Checked, claimedAmount, nonClaimedAmount, parsedId, con, transaction);
            }
        }

        private void PersistClaimableStatusAndAmounts(string sourceTable, bool isClaimable, decimal claimedAmount, decimal nonClaimedAmount, int id, SqlConnection con, SqlTransaction transaction)
        {
            string updateQuery = $@"
    UPDATE {sourceTable}
    SET IsClaimable = @IsClaimable,
        ClaimedAmount = @ClaimedAmount,
        NonClaimedAmount = @NonClaimedAmount
    WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(updateQuery, con, transaction))
            {
                cmd.Parameters.AddWithValue("@IsClaimable", isClaimable);
                cmd.Parameters.AddWithValue("@ClaimedAmount", claimedAmount != 0 ? (object)claimedAmount : DBNull.Value);
                cmd.Parameters.AddWithValue("@NonClaimedAmount", nonClaimedAmount != 0 ? (object)nonClaimedAmount : DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", id);

                cmd.ExecuteNonQuery();
            }
        }


        private void UpdateClaimableStatusInDatabase(string category, bool isClaimable, int id, SqlConnection con)
        {
            string updateQuery = $@"
    UPDATE {category} 
    SET IsClaimable = @IsClaimable 
    WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(updateQuery, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@IsClaimable", isClaimable ? 1 : 0);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateCategoryTable(string sourceTable, decimal claimedAmount, decimal nonClaimedAmount, int id, SqlConnection con)
        {
            string updateQuery = $@"
    UPDATE {sourceTable} 
    SET ClaimedAmount = @ClaimedAmount, 
        NonClaimedAmount = @NonClaimedAmount
    WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(updateQuery, con))
            {
                cmd.Parameters.AddWithValue("@ClaimedAmount", claimedAmount);
                cmd.Parameters.AddWithValue("@NonClaimedAmount", nonClaimedAmount);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private decimal GetTotalForCategory(string category)
        {
            decimal total = 0;

            foreach (GridViewRow row in ExpenseGridView.Rows)
            {
                total += GetAmountFromTextBox(row, $"txt{category}Amount");
            }

            return total;
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/AdminPage.aspx"); // Redirect to AdminPage
        }
        protected void btncancel_Click(object sender, EventArgs e)
        {
            // Iterate through all rows in the GridView


            // Optionally, clear totals if needed (not required for just checkbox unchecking)
            txtTotalClaimedAmount.Text = string.Empty;
            txtTotalNonClaimedAmount.Text = string.Empty;
        }



    }
}