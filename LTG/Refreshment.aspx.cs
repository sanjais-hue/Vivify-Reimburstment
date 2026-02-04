using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vivify
{
    public partial class Refreshment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["EmployeeId"] != null)
                {
                    txtEmployeeName.Text = Session["EmployeeFirstName"]?.ToString();
                    LoadAssignedRefreshments();
                }
                else
                {
                    lblValidationMessage.Text = "EmployeeId not found in session.";
                    lblValidationMessage.Visible = true;
                    Response.Redirect("Login.aspx");
                }
            }

            lblSuccessMessage.Visible = false;
        }

        private byte[] ConvertImageToPdf(byte[] imageBytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                    image.ScaleToFit(
                        document.PageSize.Width - document.LeftMargin - document.RightMargin,
                        document.PageSize.Height - document.TopMargin - document.BottomMargin
                    );
                    document.Add(image);
                    document.Close();
                }
                return memoryStream.ToArray();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                lblValidationMessage.Visible = false;
                lblSuccessMessage.Visible = false;

                if (Session["EmployeeId"] == null)
                {
                    lblValidationMessage.Text = "EmployeeId not found in session.";
                    lblValidationMessage.Visible = true;
                    return;
                }

                int employeeId = Convert.ToInt32(Session["EmployeeId"]);
                string fromDateStr = txtLocalRefreshmentFromDate.Text;
                string toDateStr = txtLocalRefreshmentToDate.Text;

                if (!DateTime.TryParse(fromDateStr, out DateTime fromDate) ||
                    !DateTime.TryParse(toDateStr, out DateTime toDate))
                {
                    lblValidationMessage.Text = "Please enter valid dates in the format MM/dd/yyyy.";
                    lblValidationMessage.Visible = true;
                    return;
                }

                int totalDays = (toDate - fromDate).Days + 1;
                if (totalDays != 28 && totalDays != 29 && totalDays != 30 && totalDays != 31)
                {
                    lblValidationMessage.Text = "The date range must be exactly 28, 29, 30, or 31 days.";
                    lblValidationMessage.Visible = true;
                    return;
                }

                if (ViewState["EditingId"] == null)
                {
                    if (IsRefreshmentAssignedForMonth(fromDate, employeeId))
                    {
                        lblValidationMessage.Text = "A refreshment has already been assigned for this month.";
                        lblValidationMessage.Visible = true;
                        return;
                    }
                }

                if (!decimal.TryParse(txtLocalRefreshmentAmount.Text, out decimal amount))
                {
                    lblValidationMessage.Text = "Please enter a valid amount.";
                    lblValidationMessage.Visible = true;
                    return;
                }

                byte[] fileBytes = null;
                if (fileUploadRefBill.HasFile)
                {
                    string fileExtension = Path.GetExtension(fileUploadRefBill.FileName).ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png" && fileExtension != ".pdf")
                    {
                        lblValidationMessage.Text = "Only JPG, JPEG, PNG, and PDF files are allowed.";
                        lblValidationMessage.Visible = true;
                        return;
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        fileUploadRefBill.PostedFile.InputStream.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }

                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        fileBytes = ConvertImageToPdf(fileBytes);
                    }
                }

                string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query;
                    if (ViewState["EditingId"] != null)
                    {
                        query = @"
                            UPDATE Refreshment SET 
                                FromDate = @FromDate,
                                ToDate = @ToDate,
                                RefreshAmount = @RefreshAmount,
                                Image = CASE WHEN @DataImage IS NULL THEN Image ELSE @DataImage END,
                                FirstName = @FirstName,
                                Department = @Department,
                                ServiceType = @ServiceType
                            WHERE Id = @Id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@FromDate", fromDate);
                            cmd.Parameters.AddWithValue("@ToDate", toDate);
                            cmd.Parameters.AddWithValue("@RefreshAmount", amount);
                            cmd.Parameters.AddWithValue("@FirstName", Session["EmployeeFirstName"]?.ToString());
                            cmd.Parameters.AddWithValue("@Department", "Refresh");
                            cmd.Parameters.AddWithValue("@ServiceType", "Refresh");
                            cmd.Parameters.AddWithValue("@Id", ViewState["EditingId"]);

                            if (fileBytes == null)
                            {
                                cmd.Parameters.Add("@DataImage", SqlDbType.VarBinary).Value = DBNull.Value;
                            }
                            else
                            {
                                cmd.Parameters.Add("@DataImage", SqlDbType.VarBinary).Value = fileBytes;
                            }

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }

                        btnSave.Text = "Save";
                        ViewState.Remove("EditingId");
                    }
                    else
                    {
                        query = @"
                            INSERT INTO Refreshment (Image, FromDate, ToDate, RefreshAmount, FirstName, Department, ServiceType, EmployeeId) 
                            VALUES (@DataImage, @FromDate, @ToDate, @RefreshAmount, @FirstName, @Department, @ServiceType, @EmployeeId)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.Add("@DataImage", SqlDbType.VarBinary).Value = (object)fileBytes ?? DBNull.Value;
                            cmd.Parameters.AddWithValue("@FromDate", fromDate);
                            cmd.Parameters.AddWithValue("@ToDate", toDate);
                            cmd.Parameters.AddWithValue("@RefreshAmount", amount);
                            cmd.Parameters.AddWithValue("@FirstName", Session["EmployeeFirstName"]?.ToString());
                            cmd.Parameters.AddWithValue("@Department", "Refresh");
                            cmd.Parameters.AddWithValue("@ServiceType", "Refresh");
                            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                lblSuccessMessage.Text = "Refreshment saved successfully!";
                lblSuccessMessage.Visible = true;

                ResetFormFields();
                LoadAssignedRefreshments();
            }
            catch (Exception ex)
            {
                lblValidationMessage.Text = $"Error: {HttpUtility.HtmlEncode(ex.Message)}";
                lblValidationMessage.Visible = true;
            }
        }

        private void ResetFormFields()
        {
            txtLocalRefreshmentFromDate.Text = string.Empty;
            txtLocalRefreshmentToDate.Text = string.Empty;
            txtLocalRefreshmentAmount.Text = string.Empty;
            fileUploadRefBill.Attributes.Clear();
            btnSave.Text = "Save";
            btnViewAttachment.Visible = false;
            ViewState.Remove("EditingId");
            ViewState.Remove("EditingAttachmentId");

            // 👇 Reset custom file input UI completely
            ScriptManager.RegisterStartupScript(this, GetType(), "resetFileUI", @"
        var customInput = document.getElementById('customFileInput');
        var clearIcon = document.getElementById('fileClearIcon');
        var viewButton = document.getElementById('" + btnViewAttachment.ClientID + @"');
        if (customInput) customInput.value = '';
        if (clearIcon) clearIcon.style.display = 'none';
        if (viewButton) viewButton.style.display = 'none';
    ", true);
        }

        private bool IsRefreshmentAssignedForMonth(DateTime date, int employeeId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
                SELECT COUNT(*) 
                FROM Refreshment 
                WHERE EmployeeId = @EmployeeId 
                  AND MONTH(FromDate) = @Month 
                  AND YEAR(FromDate) = @Year";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@Month", date.Month);
                cmd.Parameters.AddWithValue("@Year", date.Year);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        protected void gvAssignedRefreshments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.Style.Add("background-color", "#3f418d");
                    cell.Style.Add("color", "white");
                    cell.Style.Add("font-weight", "bold");
                    cell.Style.Add("text-align", "center");
                }
            }
        }

        private void LoadAssignedRefreshments()
        {
            if (Session["EmployeeId"] == null) return;

            int employeeId = Convert.ToInt32(Session["EmployeeId"]);
            string connStr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
                SELECT Id, FromDate, ToDate, RefreshAmount, ServiceType, Department, Image
                FROM Refreshment
                WHERE EmployeeId = @EmployeeId
                ORDER BY FromDate DESC";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                conn.Open();
                gvAssignedRefreshments.DataSource = cmd.ExecuteReader();
                gvAssignedRefreshments.DataBind();
            }
        }

        protected void gvAssignedRefreshments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!int.TryParse(e.CommandArgument?.ToString(), out int id))
            {
                lblValidationMessage.Text = "Invalid record ID.";
                lblValidationMessage.Visible = true;
                return;
            }

            if (e.CommandName == "EditRecord")
            {
                LoadRefreshmentForEdit(id);
            }
            else if (e.CommandName == "DeleteRecord")
            {
                DeleteRefreshment(id);
                LoadAssignedRefreshments();
            }
        }

        private void DeleteRefreshment(int id)
        {
            string connStr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM Refreshment WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadRefreshmentForEdit(int id)
        {
            string connStr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT FromDate, ToDate, RefreshAmount, Image FROM Refreshment WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtLocalRefreshmentFromDate.Text = Convert.ToDateTime(reader["FromDate"]).ToString("yyyy-MM-dd");
                            txtLocalRefreshmentToDate.Text = Convert.ToDateTime(reader["ToDate"]).ToString("yyyy-MM-dd");
                            txtLocalRefreshmentAmount.Text = reader["RefreshAmount"].ToString();

                            // Handle existing attachment
                          if (reader["Image"] != DBNull.Value)
{
    btnViewAttachment.Visible = true;
    ViewState["EditingAttachmentId"] = id;

    ScriptManager.RegisterStartupScript(this, GetType(), "setFileNameOnEdit", @"
        var customInput = document.getElementById('customFileInput');
        var clearIcon = document.getElementById('fileClearIcon');
        if (customInput) {
            customInput.value = 'Existing File';
            clearIcon.style.display = 'inline-block';
        }
    ", true);
}
else
{
    btnViewAttachment.Visible = false;
    ViewState.Remove("EditingAttachmentId");
}
                            btnSave.Text = "Update";
                            ViewState["EditingId"] = id;
                        }
                    }
                }
            }
        }
        protected void btnViewAttachment_Click(object sender, EventArgs e)
        {
            if (ViewState["EditingAttachmentId"] == null)
            {
                lblValidationMessage.Text = "No attachment to view.";
                lblValidationMessage.Visible = true;
                return;
            }

            int id = Convert.ToInt32(ViewState["EditingAttachmentId"]);
            string connStr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Image FROM Refreshment WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    var result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        byte[] fileBytes = (byte[])result;
                        string base64 = Convert.ToBase64String(fileBytes);
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
                        ClientScript.RegisterStartupScript(this.GetType(), "ViewAttachment", script, true);
                    }
                    else
                    {
                        lblValidationMessage.Text = "Attachment not found.";
                        lblValidationMessage.Visible = true;
                    }
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }
    }
}