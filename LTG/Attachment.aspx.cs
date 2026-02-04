using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vivify
{
    public partial class Attachment : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBranches();
                LoadExpenseTypes();
            }
        }

        private void LoadBranches()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("SELECT BranchId, BranchName FROM Branch", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlBranch.DataSource = reader;
                ddlBranch.DataTextField = "BranchName";
                ddlBranch.DataValueField = "BranchId";
                ddlBranch.DataBind();
            }
        }

        private void LoadExpenseTypes()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                string query = @"
                    SELECT DISTINCT ExpenseType FROM (
                        SELECT ExpenseType FROM Conveyance 
                        UNION ALL 
                        SELECT ExpenseType FROM Others 
                        UNION ALL 
                        SELECT ExpenseType FROM Lodging 
                        UNION ALL 
                        SELECT ExpenseType FROM Miscellaneous
                    ) AS AllExpenses";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlExpenseType.DataSource = reader;
                ddlExpenseType.DataTextField = "ExpenseType";
                ddlExpenseType.DataValueField = "ExpenseType";
                ddlExpenseType.DataBind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);
            int branchId = int.Parse(ddlBranch.SelectedValue);
            string selectedExpenseType = ddlExpenseType.SelectedValue;

            var serviceData = GetServiceData(fromDate, toDate, branchId);
            if (serviceData.ServiceId == 0)
            {
                // Handle the case where no service data is found (e.g., show an error message)
                return;
            }

            var expenseData = GetExpenseData(serviceData.ServiceId, selectedExpenseType);
            var workOrderData = GetWorkOrderImages(serviceData.ServiceId, selectedExpenseType);
            var refreshmentData = GetRefreshmentData(serviceData.ServiceId);
            var approvalMailData = GetApprovalMailImages(serviceData.ServiceId, selectedExpenseType, ddlBranch.SelectedItem.Text);


            GeneratePDF(serviceData, expenseData, workOrderData, refreshmentData, approvalMailData, fromDate, toDate, selectedExpenseType);
        }

        private (int ServiceId, string EmployeeName, string BranchName, DateTime FromDate, DateTime ToDate) GetServiceData(DateTime fromDate, DateTime toDate, int branchId)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT s.ServiceId, e.FirstName, b.BranchName, s.FromDate, s.ToDate 
                    FROM Services s
                    JOIN Employees e ON s.EmployeeId = e.EmployeeId
                    JOIN Branch b ON s.BranchId = b.BranchId
                    WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate AND s.BranchId = @BranchId", conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@BranchId", branchId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (
                            ServiceId: reader.GetInt32(0),
                            EmployeeName: reader.GetString(1),
                            BranchName: reader.GetString(2),
                            FromDate: reader.GetDateTime(3),
                            ToDate: reader.GetDateTime(4)
                        );
                    }
                }
            }
            return (0, "", "", DateTime.MinValue, DateTime.MinValue);
        }

        private DataTable GetExpenseData(int serviceId, string expenseType)
        {
            DataTable expenseTable = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                string query = @"
                    SELECT ExpenseType, ClaimedAmount, Image, IsClaimable 
                    FROM (
                        SELECT ExpenseType, ClaimedAmount, Image, IsClaimable FROM Conveyance WHERE ServiceId = @ServiceId
                        UNION ALL
                        SELECT ExpenseType, ClaimedAmount, Image, IsClaimable FROM Others WHERE ServiceId = @ServiceId
                        UNION ALL
                        SELECT ExpenseType, ClaimedAmount, Image, IsClaimable FROM Lodging WHERE ServiceId = @ServiceId
                        UNION ALL
                        SELECT ExpenseType, ClaimedAmount, Image, IsClaimable FROM Miscellaneous WHERE ServiceId = @ServiceId
                    ) AS AllExpenses
                    WHERE IsClaimable = 1 AND (ExpenseType = @ExpenseType OR @ExpenseType IS NULL)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                cmd.Parameters.AddWithValue("@ExpenseType", string.IsNullOrEmpty(expenseType) ? (object)DBNull.Value : expenseType);
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(expenseTable);
            }
            return expenseTable;
        }

        private DataTable GetWorkOrderImages(int serviceId, string expenseType)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                string query = string.Empty;

                // Determine the table and query based on ExpenseType
                if (expenseType == "Local") // ExpenseType "Local" corresponds to the "Others" table
                {
                    query = @"
                SELECT ServiceReport
                FROM Others
                WHERE ServiceId = @ServiceId AND IsClaimable = 1";
                }
                else if (expenseType == "Tour") // ExpenseType "Tour" corresponds to the "Lodging" table
                {
                    query = @"
                SELECT ServiceReport
                FROM Lodging
                WHERE ServiceId = @ServiceId AND IsClaimable = 1";
                }
                else
                {
                    // If no specific expense type or an unsupported one, we can return an empty DataTable
                    return new DataTable();
                }

                // Execute the SQL command
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ServiceId", serviceId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        private DataTable GetRefreshmentData(int serviceId)
        {
            DataTable refreshmentTable = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                string query = @"
                    SELECT Image
                    FROM Refreshment
                    WHERE ServiceId = @ServiceId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(refreshmentTable);
            }
            return refreshmentTable;
        }

        private DataTable GetApprovalMailImages(int serviceId, string expenseType, string branchName)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                string query = string.Empty;

                if (expenseType == "Local")
                {
                    query = @"
            SELECT ApprovalMail
            FROM Others o
            INNER JOIN Services s ON o.ServiceId = s.ServiceId
            INNER JOIN Employees e ON s.EmployeeId = e.EmployeeId
            WHERE o.ServiceId = @ServiceId 
            AND e.BranchName = @BranchName
            AND o.IsClaimable = 1";
                }
                else if (expenseType == "Tour")
                {
                    query = @"
            SELECT ApprovalMail
            FROM Lodging l
            INNER JOIN Services s ON l.ServiceId = s.ServiceId
            INNER JOIN Employees e ON s.EmployeeId = e.EmployeeId
            WHERE l.ServiceId = @ServiceId 
            AND e.BranchName = @BranchName
            AND l.IsClaimable = 1";
                }
                else
                {
                    return new DataTable();
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                cmd.Parameters.AddWithValue("@BranchName", branchName);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        private void GeneratePDF(
            (int ServiceId, string EmployeeName, string BranchName, DateTime FromDate, DateTime ToDate) serviceData,
            DataTable expenseData, DataTable workOrderData, DataTable refreshmentData, DataTable approvalMailData,
            DateTime fromDate, DateTime toDate, string selectedExpenseType)
        {
            Document document = new Document();
            MemoryStream ms = new MemoryStream();
            PdfWriter.GetInstance(document, ms);

            document.Open();

            // Add Branch Information and Date Range
            string serviceInfo = $"Branch: {serviceData.BranchName}\nFrom: {serviceData.FromDate:yyyy-MM-dd}\nTo: {serviceData.ToDate:yyyy-MM-dd}\nExpense Type: {selectedExpenseType}";
            document.Add(new Paragraph(serviceInfo));

            // Add Expense Type Header
            document.Add(new Paragraph($"Expense Type: {selectedExpenseType}"));

            // Add Images for Expenses (Local or Tour)
            if (expenseData.Rows.Count > 0)
            {
                foreach (DataRow row in expenseData.Rows)
                {
                    // Add image if it exists for the current expense
                    byte[] imageBytes = row["Image"] as byte[];

                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        try
                        {
                            // Attempt to load the image from the byte array
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imageBytes);
                            img.ScaleToFit(500, 500); // Adjust the size of the image
                            img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                            document.Add(img);
                        }
                        catch (Exception ex)
                        {
                            // If an error occurs, add an error message to the PDF
                            document.Add(new Paragraph($"Error loading image: {ex.Message}"));
                        }
                    }
                }
            }

            // Optionally, Add Work Order Images if they exist
            if (workOrderData.Rows.Count > 0)
            {
                foreach (DataRow row in workOrderData.Rows)
                {
                    byte[] imageBytes = row["ServiceReport"] as byte[];

                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        try
                        {
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imageBytes);
                            img.ScaleToFit(500, 500); // Adjust the size of the image
                            img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                            document.Add(img);
                        }
                        catch (Exception ex)
                        {
                            document.Add(new Paragraph($"Error loading work order image: {ex.Message}"));
                        }
                    }
                }
            }

            // Optionally, Add Refreshment Images if they exist
            if (refreshmentData.Rows.Count > 0)
            {
                foreach (DataRow row in refreshmentData.Rows)
                {
                    byte[] imageBytes = row["Image"] as byte[];

                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        try
                        {
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imageBytes);
                            img.ScaleToFit(500, 500); // Adjust the size of the image
                            img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                            document.Add(img);
                        }
                        catch (Exception ex)
                        {
                            document.Add(new Paragraph($"Error loading refreshment image: {ex.Message}"));
                        }
                    }
                }
            }

            // Optionally, Add Approval Mail Images if they exist
            if (approvalMailData.Rows.Count > 0)
            {
                foreach (DataRow row in approvalMailData.Rows)
                {
                    byte[] imageBytes = row["ApprovalMail"] as byte[];

                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        try
                        {
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imageBytes);
                            img.ScaleToFit(500, 500); // Adjust the size of the image
                            img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                            document.Add(img);
                        }
                        catch (Exception ex)
                        {
                            document.Add(new Paragraph($"Error loading approval mail image: {ex.Message}"));
                        }
                    }
                }
            }

            // Close the document to complete the PDF generation
            document.Close();

            // Set up the response for downloading the file
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment;filename=Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }

    }
}
