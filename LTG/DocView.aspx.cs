using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web.UI;
using System.Drawing;
using System.Drawing.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Linq;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;
using static Vivify.Admin_Training_Assign;
using DocumentFormat.OpenXml.Office.Word;

namespace Vivify
{
    public partial class DocView : Page
    {
       
            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    LoadBranches();
                    LoadEmployeeNames(); // Initially load all employees
                }
            }

            private void LoadBranches()
            {
                string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
                string query = "SELECT DISTINCT BranchId, BranchName FROM Branch ORDER BY BranchName";

                DataTable branchesTable = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(branchesTable);
                        }
                    }
                }

                ddlBranch.DataSource = branchesTable;
                ddlBranch.DataTextField = "BranchName";
                ddlBranch.DataValueField = "BranchId"; // Use BranchId as ValueField
                ddlBranch.DataBind();

            ddlBranch.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Branch", "0"));

        }

            protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
            {
                int selectedBranchId = int.Parse(ddlBranch.SelectedValue);
                LoadEmployeeNames(selectedBranchId);  // Load employees for the selected branch
            }

            private void LoadEmployeeNames(int branchId = 0)
            {
                string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
                string query = "SELECT DISTINCT EmployeeId, FirstName FROM Employees WHERE (@BranchId = 0 OR BranchId = @BranchId) ORDER BY FirstName";

                DataTable employeeTable = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BranchId", branchId);

                        con.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(employeeTable);
                        }
                    }
                }

                ddlEmployee.DataSource = employeeTable;
                ddlEmployee.DataTextField = "FirstName";
                ddlEmployee.DataValueField = "EmployeeId"; // Use EmployeeId for ValueField
                ddlEmployee.DataBind();

            // Insert "All" at the beginning
            ddlEmployee.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Employee", "0"));

        }
        

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                // Parse input parameters
                DateTime fromDate = DateTime.Parse(txtFromDate.Text);
                DateTime toDate = DateTime.Parse(txtToDate.Text);
                int branchId = int.Parse(ddlBranch.SelectedValue);
                string branchName = ddlBranch.SelectedItem.Text;

                int employeeId = int.Parse(ddlEmployee.SelectedValue); // Capture selected employee
                string employeeName = ddlEmployee.SelectedItem.Text;

                // Fetch data
                var services = GetServiceData(fromDate, toDate, branchId, employeeId); // Pass employeeId
                DataTable refreshmentData = GetRefreshmentImages(fromDate, toDate, branchName, employeeName); // Pass employeeName

                // Define folder path
                string folderPath = GetFolderPath();

                // Ensure the folder exists and clear previous files
                PrepareFolder(folderPath);

                // Handle conditions
                if ((services == null || !services.Any()) && (refreshmentData == null || refreshmentData.Rows.Count == 0))
                {
                    ShowAlert("No attachment found.");
                    return;
                }

                if (services == null || !services.Any())
                {
                    HandleRefreshmentOnly(refreshmentData, folderPath);
                    return;
                }

                HandleFullReportGeneration(services, refreshmentData, fromDate, toDate, branchName, employeeName, folderPath);
            }
            catch (FormatException)
            {
                string script = "alert('Invalid input format. Please ensure correct date selection and dropdown values.');";
                ClientScript.RegisterStartupScript(this.GetType(), "InputError", script, true);
                return;
            }
            catch (Exception )
            {
                string script = "alert('An unexpected error occurred.');";
                ClientScript.RegisterStartupScript(this.GetType(), "GeneralError", script, true);
                return;
            }

        }

        private void HandleRefreshmentOnly(DataTable refreshmentData, string folderPath)
        {
            try
            {
                // Collect PDF data
                List<byte[]> pdfByteArrays = new List<byte[]>();

                // Add refreshment data to the PDF list
                AddPdfData(refreshmentData, 0, "Refreshment", pdfByteArrays);

                if (pdfByteArrays.Count == 0)
                {
                    string script = "alert('No valid refreshment images found.');";
                    ClientScript.RegisterStartupScript(this.Page.GetType(), "NoRefreshment", script, true);
                    return;
                }

                // Merge all PDFs
                string mergedPdfPath = Path.Combine(folderPath, "MergedDocument.pdf");
                MergePdfs(pdfByteArrays, mergedPdfPath);

                // Trigger download
                TriggerZIPDownload(mergedPdfPath);

                DisplayMessage("Downloading merged refreshment PDF.", isError: false);
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error while processing refreshment images: {ex.Message}", isError: true);
            }
        }
        private void HandleFullReportGeneration(List<(int, string, string, DateTime, DateTime)> services, DataTable refreshmentData, DateTime fromDate, DateTime toDate, string branchName, string employeeName, string folderPath)
        {
            try
            {
                DataTable expenseData = GetExpenseData(fromDate, toDate, branchName, employeeName);
                DataTable workOrderData = GetWorkOrderImages(fromDate, toDate, branchName, employeeName);
                DataTable approvalMailData = GetApprovalMailImages(fromDate, toDate, branchName, employeeName);

                // Check if there’s absolutely no data to generate PDF from
                if ((expenseData == null || expenseData.Rows.Count == 0) &&
                    (workOrderData == null || workOrderData.Rows.Count == 0) &&
                    (approvalMailData == null || approvalMailData.Rows.Count == 0) &&
                    (refreshmentData == null || refreshmentData.Rows.Count == 0))
                {
                    ShowAlert("No attachment found.");
                    return;
                }

                // Attempt PDF generation
                GeneratePDF(services, expenseData, workOrderData, approvalMailData, refreshmentData, fromDate, toDate);
                ShowAlert("PDF generation successful. Downloading PDF.");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("No PDF data to merge"))
            {
                ShowAlert("No attachment found.");
            }
            catch (Exception )
            {
                ShowAlert("An error occurred while processing attachments.");
            }
        }
        private void ShowAlert(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
        }

        private string GetFolderPath()
        {
            return Server.MapPath("~/DocReport");
        }

        private void PrepareFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
            Directory.CreateDirectory(folderPath);
        }
        private void DisplayMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = isError ? Color.Red : Color.Green;
            lblMessage.Visible = true;
        }

        private List<(int, string, string, DateTime, DateTime)> GetServiceData(DateTime fromDate, DateTime toDate, int branchId, int employeeId)
        {
            List<(int, string, string, DateTime, DateTime)> serviceList = new List<(int, string, string, DateTime, DateTime)>();
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"
                SELECT s.ServiceId, e.FirstName, b.BranchName, s.FromDate, s.ToDate 
                FROM Services s
                JOIN Employees e ON s.EmployeeId = e.EmployeeId
                JOIN Branch b ON s.BranchId = b.BranchId
                WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate AND s.BranchId = @BranchId AND s.EmployeeId = @EmployeeId", conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@BranchId", branchId);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        serviceList.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4)));
                    }
                }
            }

            return serviceList;
        }

        private DataTable GetApprovalMailImages(DateTime fromDate, DateTime toDate, string branchName,string employeeName)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                string query = @"
             SELECT 
                 o.ApprovalMail AS ImageData, 
                 o.ServiceId AS ServiceId, 
                 'ApprovalMail' AS ImageType
             FROM 
                 Others o
             INNER JOIN 
                 Services s ON o.ServiceId = s.ServiceId
             INNER JOIN 
                 Employees e ON s.EmployeeId = e.EmployeeId
             WHERE 
                 s.FromDate >= @FromDate AND s.ToDate <= @ToDate
                 AND e.BranchName = @BranchName
                  AND e.FirstName = @employeeName
                 AND o.IsClaimable = 1
             UNION ALL

             SELECT 
                 l.ApprovalMail AS ImageData, 
                 l.ServiceId AS ServiceId, 
                 'ApprovalMail' AS ImageType
             FROM 
                 Lodging l
             INNER JOIN 
                 Services s ON l.ServiceId = s.ServiceId
             INNER JOIN 
                 Employees e ON s.EmployeeId = e.EmployeeId
             WHERE 
                 s.FromDate >= @FromDate AND s.ToDate <= @ToDate
                 AND e.BranchName = @BranchName
                    AND e.FirstName = @employeeName
                 AND l.IsClaimable = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@BranchName", branchName);
                cmd.Parameters.AddWithValue("@employeeName", employeeName);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        private DataTable GetExpenseData(DateTime fromDate, DateTime toDate, string branchName,string employeeName)
        {
            DataTable expenseTable = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                string serviceQuery = @"
     SELECT s.ServiceId
     FROM Services s
     JOIN Employees e ON s.EmployeeId = e.EmployeeId
     WHERE s.FromDate >= @FromDate 
     AND s.ToDate <= @ToDate 
     AND e.BranchName = @BranchName
       AND e.FirstName = @employeeName
";
     
                SqlCommand serviceCmd = new SqlCommand(serviceQuery, conn);
                serviceCmd.Parameters.AddWithValue("@FromDate", fromDate);
                serviceCmd.Parameters.AddWithValue("@ToDate", toDate);
                serviceCmd.Parameters.AddWithValue("@BranchName", branchName);
                serviceCmd.Parameters.AddWithValue("@employeeName", employeeName);
                conn.Open();
                SqlDataReader serviceReader = serviceCmd.ExecuteReader();

                List<int> serviceIds = new List<int>();

                while (serviceReader.Read())
                {
                    serviceIds.Add(serviceReader.GetInt32(0));
                }

                serviceReader.Close();

                if (serviceIds.Count == 0)
                {
                    return expenseTable;
                }

                string query = @"
     SELECT ExpenseType, ClaimedAmount, ImageData, ServiceId, IsClaimable 
     FROM (
         SELECT 'Conveyance' AS ExpenseType, ExpenseType AS Type, ClaimedAmount, Image AS ImageData, ServiceId, IsClaimable 
         FROM Conveyance 
         WHERE ServiceId IN ({0})
         UNION ALL
         SELECT 'Others' AS ExpenseType, ExpenseType AS Type, ClaimedAmount, Image AS ImageData, ServiceId, IsClaimable 
         FROM Others 
         WHERE ServiceId IN ({0})
         UNION ALL
         SELECT 'Lodging' AS ExpenseType, ExpenseType AS Type, ClaimedAmount, Image AS ImageData, ServiceId, IsClaimable 
         FROM Lodging 
         WHERE ServiceId IN ({0})
         UNION ALL
         SELECT 'Miscellaneous' AS ExpenseType, ExpenseType AS Type, ClaimedAmount, Image AS ImageData, ServiceId, IsClaimable 
         FROM Miscellaneous 
         WHERE ServiceId IN ({0})
     ) AS AllExpenses
     WHERE IsClaimable = 1";

                var parameters = serviceIds.Select((serviceId, index) => "@ServiceId" + index).ToList();
                string formattedQuery = string.Format(query, string.Join(",", parameters));

                SqlCommand cmd = new SqlCommand(formattedQuery, conn);
                for (int i = 0; i < serviceIds.Count; i++)
                {
                    cmd.Parameters.AddWithValue("@ServiceId" + i, serviceIds[i]);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(expenseTable);
            }

            return expenseTable;
        }

        private DataTable GetWorkOrderImages(DateTime fromDate, DateTime toDate, string branchName,string employeeName)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                string query = @"
     SELECT o.ServiceReport AS ImageData, o.ServiceId
     FROM Others o
     INNER JOIN Services s ON o.ServiceId = s.ServiceId
     INNER JOIN Employees e ON s.EmployeeId = e.EmployeeId
     WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate
     AND e.BranchName = @BranchName
     AND e.FirstName = @employeeName
     AND o.IsClaimable = 1
     UNION ALL
     SELECT l.ServiceReport AS ImageData, l.ServiceId
     FROM Lodging l
     INNER JOIN Services s ON l.ServiceId = s.ServiceId
     INNER JOIN Employees e ON s.EmployeeId = e.EmployeeId
     WHERE s.FromDate >= @FromDate AND s.ToDate <= @ToDate
     AND e.BranchName = @BranchName
     AND e.FirstName = @employeeName
     AND l.IsClaimable = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@BranchName", branchName);
                cmd.Parameters.AddWithValue("@employeeName", employeeName);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
        private DataTable GetRefreshmentImages(DateTime fromDate, DateTime toDate, string branchName,string employeeName)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(constr))
            {
                string query = @"
            SELECT r.Image AS ImageData, r.EmployeeId AS EmployeeId, r.FromDate AS FromDate, r.ToDate AS ToDate, 'Refreshment' AS ImageType
            FROM Refreshment r
            INNER JOIN Employees e ON r.EmployeeId = e.EmployeeId
            WHERE r.FromDate <= @ToDate 
                AND r.ToDate >= @FromDate 
                AND e.BranchName = @BranchName AND e.FirstName = @employeeName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@BranchName", branchName);
                cmd.Parameters.AddWithValue("@employeeName", employeeName);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Validate ImageData before returning
                foreach (DataRow row in dt.Rows)
                {
                    byte[] imageData = row["ImageData"] as byte[];
                    if (imageData != null && !IsValidImageData(imageData) && !IsValidPdfData(imageData))
                    {
                        row["ImageData"] = DBNull.Value; // Mark as invalid
                    }
                }

                return dt;
            }
        }
        private void GeneratePDF(
     List<(int ServiceId, string EmployeeName, string BranchName, DateTime FromDate, DateTime ToDate)> services,
     DataTable expenseData,
     DataTable workOrderData,
     DataTable approvalMailData,
     DataTable refreshmentData,
     DateTime fromDate,
     DateTime toDate)
        {
            string folderPath = Server.MapPath("~/DocReport");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Reset counters
            _counters.Clear();

            // Collect PDF data
            List<byte[]> pdfByteArrays = new List<byte[]>();

            // Process approval and refreshment data first
            AddPdfData(approvalMailData, 0, "Approval", pdfByteArrays);
            AddPdfData(refreshmentData, 0, "Refreshment", pdfByteArrays);

            // Process expense data
            if (services != null && services.Any())
            {
                foreach (var service in services)
                {
                    AddPdfData(expenseData, service.ServiceId, "Expense", pdfByteArrays);
                }
            }

            // Process work order data
            if (services != null && services.Any())
            {
                foreach (var service in services)
                {
                    AddPdfData(workOrderData, service.ServiceId, "WorkOrder", pdfByteArrays);
                }
            }

            // Merge all PDFs
            string mergedPdfPath = Path.Combine(folderPath, "MergedDocument.pdf");
            MergePdfs(pdfByteArrays, mergedPdfPath);

            // Trigger download
            TriggerZIPDownload(mergedPdfPath);
        }
        private void ConvertImageToPdf(byte[] imageData, string filePath)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(imageData))
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
                using (Document document = new Document())
                using (PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create)))
                {
                    document.Open();
                    iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(imageData);

                    // Scale image to fit within page margins
                    pdfImage.ScaleToFit(
                        document.PageSize.Width - document.LeftMargin - document.RightMargin,
                        document.PageSize.Height - document.TopMargin - document.BottomMargin
                    );

                    document.Add(pdfImage);
                    document.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting image to PDF: {ex.Message}");
                throw; // Re-throw the exception to see the error in the calling method
            }
        }
        private void AddPdfData(DataTable data, int serviceId, string prefix, List<byte[]> pdfByteArrays)
        {
            if (data == null || !data.Columns.Contains("ImageData"))
            {
                Console.WriteLine($"No valid data found for prefix: {prefix}");
                return;
            }

            foreach (DataRow row in data.Rows)
            {
                bool isRefreshment = (prefix == "Refreshment");
                bool isApproval = (prefix == "Approval");
                bool isOther = !isRefreshment && !isApproval && data.Columns.Contains("ServiceId")
                               && row["ServiceId"] != DBNull.Value
                               && (int)row["ServiceId"] == serviceId;

                if (isRefreshment || isApproval || isOther)
                {
                    byte[] imageData = row["ImageData"] as byte[];
                    if (imageData != null && (IsValidImageData(imageData) || IsValidPdfData(imageData)))
                    {
                        try
                        {
                            string tempFilePath = Path.GetTempFileName();
                            string finalFilePath = Path.GetTempFileName();

                            if (IsPdfFormat(imageData))
                            {
                                File.WriteAllBytes(tempFilePath, imageData);
                            }
                            else
                            {
                                ConvertImageToPdf(imageData, tempFilePath);
                            }

                            string textCount = GetTextCount(prefix);
                            AddTextCountToFirstPage(tempFilePath, textCount);

                            byte[] modifiedPdfBytes = File.ReadAllBytes(tempFilePath);
                            pdfByteArrays.Add(modifiedPdfBytes);

                            Console.WriteLine($"Added PDF for {prefix} with text count: {textCount}");

                            File.Delete(tempFilePath);
                            File.Delete(finalFilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing PDF: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid image/PDF data for {prefix}");
                    }
                }
            }
        }
        private Dictionary<string, int> _counters = new Dictionary<string, int>();

        private string GetTextCount(string prefix)
        {
            string shortPrefix;
            switch (prefix)
            {
                case "Approval":
                case "Refreshment":
                    shortPrefix = "A";
                    break;
                case "Expense":
                    shortPrefix = "B";
                    break;
                case "WorkOrder":
                    shortPrefix = "W";
                    break;
                default:
                    shortPrefix = prefix;
                    break;
            }

            if (!_counters.ContainsKey(shortPrefix))
            {
                _counters[shortPrefix] = 0;
            }
            _counters[shortPrefix]++;

            return $"{shortPrefix}{_counters[shortPrefix]}";
        }
        private bool IsPdfFormat(byte[] fileData)
        {
            return fileData.Length > 4 && fileData[0] == 0x25 && fileData[1] == 0x50 && fileData[2] == 0x44 && fileData[3] == 0x46; // "%PDF"
        }
        private void AddTextCountToFirstPage(string filePath, string textCount)
        {
            try
            {
                // Create a temporary file to write the modified PDF
                string tempFilePath = Path.GetTempFileName();

                // Open the original PDF
                using (PdfReader reader = new PdfReader(filePath))
                using (FileStream fs = new FileStream(tempFilePath, FileMode.Create))
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {
                    // Log total pages
                    Console.WriteLine($"Total Pages: {reader.NumberOfPages}");

                    // Add text count to the first page
                    PdfContentByte content = stamper.GetOverContent(1);
                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    Font font = new Font(baseFont, 25, Font.BOLD, BaseColor.ORANGE);

                    ColumnText.ShowTextAligned(
                        content,
                        Element.ALIGN_LEFT,
                        new Phrase(textCount, font),
                        36, // x-coordinate (left margin)
                        PageSize.A4.Height - 36, // y-coordinate (top margin)
                        0 // rotation
                    );

                    // Log each page being processed
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        Console.WriteLine($"Processed Page {i}");
                    }
                }

                // Replace the original file with the modified one
                File.Delete(filePath);
                File.Move(tempFilePath, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding text to PDF: {ex.Message}");
                throw;
            }
        }
        private void MergePdfs(List<byte[]> pdfByteArrays, string outputFilePath)
        {
            if (pdfByteArrays == null || pdfByteArrays.Count == 0)
            {
                throw new InvalidOperationException("No PDF data to merge. Check if valid data was fetched and processed.");
            }

            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create))
            {
                Document doc = new Document();
                PdfCopy pdfCopy = new PdfCopy(doc, outputStream);
                doc.Open();

                foreach (byte[] pdfBytes in pdfByteArrays)
                {
                    if (pdfBytes != null && pdfBytes.Length > 0 && IsValidPdfData(pdfBytes))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(pdfBytes))
                            {
                                PdfReader reader = new PdfReader(ms);
                                for (int i = 1; i <= reader.NumberOfPages; i++)
                                {
                                    PdfImportedPage page = pdfCopy.GetImportedPage(reader, i);
                                    pdfCopy.AddPage(page);
                                }
                                reader.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error merging PDF: {ex.Message}");
                        }
                    }
                }

                doc.Close();
            }
        }

        private bool IsValidImageData(byte[] imageData)
        {
            try
            {
                using (var ms = new MemoryStream(imageData))
                {
                    System.Drawing.Image.FromStream(ms); // Attempt to load the image
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid image data: {ex.Message}");
                return false; // Invalid image data
            }
        }

        private bool IsValidPdfData(byte[] pdfData)
        {
            try
            {
                using (var ms = new MemoryStream(pdfData))
                using (var reader = new PdfReader(ms))
                {
                    return true; // Valid PDF
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid PDF data: {ex.Message}");
                return false; // Invalid PDF
            }
        }
        private void TriggerZIPDownload(string filePath)
        {
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=MergedDocument.pdf");
            Response.TransmitFile(filePath);
            Response.End();
        }
    }
}