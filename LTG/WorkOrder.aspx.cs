using System;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web.UI;

namespace Vivify
{
    public partial class WorkOrder : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEmployee();
            }
        }

        private void LoadEmployee()
        {
            hiddenEmployeeId.Value = string.Empty;
            txtEmployeeName.Text = string.Empty;

            if (Session["EmployeeFirstName"] != null)
            {
                txtEmployeeName.Text = Session["EmployeeFirstName"].ToString(); // Populate the TextBox with the employee's first name
            }

            hiddenServiceId.Value = string.Empty;
            txtServiceId.Text = string.Empty;

            if (Session["ServiceId"] != null)
            {
                txtServiceId.Text = Session["ServiceId"].ToString(); // Populate the TextBox with the service ID
            }

            hiddenServiceType.Value = string.Empty;
            txtServiceType.Text = string.Empty;

            if (Session["ServiceType"] != null)
            {
                txtServiceType.Text = Session["ServiceType"].ToString(); // Populate the TextBox with the service type
            }

            hiddenExpenseType.Value = string.Empty;
            txtExpenseType.Text = string.Empty;

            if (Session["ExpenseType"] != null)
            {
                txtExpenseType.Text = Session["ExpenseType"].ToString(); // Populate the TextBox with the expense type
            }

            hiddenSmoNo.Value = string.Empty;
            txtSmoNo.Text = string.Empty;

            if (Session["SmoNo"] != null)
            {
                txtSmoNo.Text = Session["SmoNo"].ToString(); // Populate the TextBox with the SMO number
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtServiceId.Text))
            {
                byte[] imageData = null;

                if (fileUploadAttachment.HasFile)
                {
                    using (BinaryReader br = new BinaryReader(fileUploadAttachment.PostedFile.InputStream))
                    {
                        imageData = br.ReadBytes(fileUploadAttachment.PostedFile.ContentLength);
                    }
                }

                // Update the query to use 'Image' instead of 'Attachment'
                string query = "INSERT INTO WorkOrder (ServiceId, FirstName, ExpenseType, ServiceType, SmoNo, Image) VALUES (@ServiceId, @FirstName, @ExpenseType, @ServiceType, @SmoNo, @Image)";

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["vivify"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@ServiceId", System.Data.SqlDbType.Int).Value = Convert.ToInt32(txtServiceId.Text);
                    cmd.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar).Value = txtEmployeeName.Text; // Use the employee name
                    cmd.Parameters.Add("@ExpenseType", System.Data.SqlDbType.NVarChar).Value = txtExpenseType.Text;
                    cmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.NVarChar).Value = txtServiceType.Text;
                    cmd.Parameters.Add("@SmoNo", System.Data.SqlDbType.NVarChar).Value = txtSmoNo.Text;

                    // Change parameter name to match 'Image'
                    if (imageData != null)
                    {
                        cmd.Parameters.Add("@Image", System.Data.SqlDbType.VarBinary).Value = imageData; // Store the image as binary
                    }
                    else
                    {
                        cmd.Parameters.Add("@Image", System.Data.SqlDbType.VarBinary).Value = DBNull.Value; // Handle null image case
                    }

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        lblMessage.Text = "Work order saved successfully.";
                        lblMessage.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "An error occurred while saving the work order: " + ex.Message;
                        lblMessage.Visible = true;
                    }
                }
            }
            else
            {
                lblMessage.Text = "Service ID cannot be empty.";
                lblMessage.Visible = true;
            }
        }
    }
}
