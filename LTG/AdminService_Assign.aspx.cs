using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class AdminService_Assign : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindBranch();
                BindCustomer();

                if (ddlBranch.Items.Count > 0)
                {
                    ddlBranch.SelectedIndex = 0;
                    BindEmployee(ddlBranch.SelectedValue);
                }
            }
        }

        private void BindBranch()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "SELECT BranchId, BranchName FROM Branch";
                SqlCommand cmd = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Add a "Select" option
                    DataRow selectRow = dt.NewRow();
                    selectRow["BranchId"] = DBNull.Value;
                    selectRow["BranchName"] = "Select";
                    dt.Rows.InsertAt(selectRow, 0);

                    ddlBranch.DataSource = dt;
                    ddlBranch.DataTextField = "BranchName";
                    ddlBranch.DataValueField = "BranchId";
                    ddlBranch.DataBind();
                }
            }
        }

        private void BindCustomer()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // Get the selected branch ID
                string selectedBranchId = ddlBranch.SelectedValue;
                string qry = "SELECT CustomerId, CustomerName FROM Customers WHERE BranchId = @BranchId";
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.Parameters.AddWithValue("@BranchId", selectedBranchId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Add a "Select" option
                    DataRow selectRow = dt.NewRow();
                    selectRow["CustomerId"] = DBNull.Value;
                    selectRow["CustomerName"] = "Select";
                    dt.Rows.InsertAt(selectRow, 0);

                    ddlCustId.DataSource = dt;
                    ddlCustId.DataTextField = "CustomerName";
                    ddlCustId.DataValueField = "CustomerId";
                    ddlCustId.DataBind();
                }
            }
        }

        private void BindEmployee(string branchId)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "SELECT EmployeeId, FirstName + '(' + EmployeeCode + ')' AS displayname FROM Employees WHERE BranchId = @Branchid";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@Branchid", branchId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Add a "Select" option
                        DataRow selectRow = dt.NewRow();
                        selectRow["EmployeeId"] = DBNull.Value;
                        selectRow["displayname"] = "Select";
                        dt.Rows.InsertAt(selectRow, 0);

                        ddlEmpId.DataSource = dt;
                        ddlEmpId.DataTextField = "displayname";
                        ddlEmpId.DataValueField = "EmployeeId";
                        ddlEmpId.DataBind();
                    }
                }
            }
        }
        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Rebind customers
            BindCustomer(); // Correct method name

            // Bind employees based on the selected branch
            string selectedBranchId = ddlBranch.SelectedValue;
            BindEmployee(selectedBranchId); // Correct method name
        }

        protected void btnService_Click(object sender, EventArgs e)
        {
            try
            {
                HttpCookie firstNameCookie = Request.Cookies["FirstName"];
                string firstName = firstNameCookie.Value;
                string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

                if (string.IsNullOrEmpty(constr))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Database connection string is not defined.');", true);
                    return;
                }

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Insert the new service record
                            string insertServiceQry = @"
INSERT INTO Services (BranchId, CustomerId, EmployeeId, ServiceType, FromDate, ToDate, Advance, Remarks, Department, CreatedDate, CreatedBy) 
VALUES (@BranchId, @CustomerId, @EmployeeId, @ServiceType, @FromDate, @ToDate, @Advance, @Remarks, @Department, GETDATE(), @CreatedBy); 
SELECT SCOPE_IDENTITY();";

                            int newServiceId;
                            using (SqlCommand cmd = new SqlCommand(insertServiceQry, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BranchId", ddlBranch.SelectedValue);
                                cmd.Parameters.AddWithValue("@CustomerId", ddlCustId.SelectedValue);
                                cmd.Parameters.AddWithValue("@EmployeeId", ddlEmpId.SelectedValue);
                                cmd.Parameters.AddWithValue("@ServiceType", ddlservice.SelectedItem.Text);

                                DateTime fromDate, toDate;
                                cmd.Parameters.AddWithValue("@FromDate", DateTime.TryParse(txtFromDate.Text, out fromDate) ? (object)fromDate : DBNull.Value);
                                cmd.Parameters.AddWithValue("@ToDate", DateTime.TryParse(txtToDate.Text, out toDate) ? (object)toDate : DBNull.Value);

                                // Adjust Advance logic as discussed
                                decimal advanceAmount = 0;
                                if (!string.IsNullOrEmpty(txtAdvance.Text) && decimal.TryParse(txtAdvance.Text, out advanceAmount))
                                {
                                    cmd.Parameters.AddWithValue("@Advance", advanceAmount);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Advance", 0);
                                }
                                cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(txtRemarks.Text) ? DBNull.Value : (object)txtRemarks.Text);
                                cmd.Parameters.AddWithValue("@Department", ddldepartment.SelectedItem.Text); // Ensure ddldepartment is accessible
                                cmd.Parameters.AddWithValue("@CreatedBy", firstName);
                                newServiceId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Check if there's already an assignment for the employee
                            string checkAssignmentQry = @"
               IF EXISTS (SELECT 1 FROM Services WHERE ServiceId = @ServiceId AND EmployeeId = @EmployeeId)
               BEGIN
                   UPDATE Services
                   SET StatusId = @StatusId 
                   WHERE ServiceId = @ServiceId AND EmployeeId = @EmployeeId;
               END
               ELSE
               BEGIN
                   INSERT INTO Services (ServiceId, EmployeeId, StatusId) 
                   VALUES (@ServiceId, @EmployeeId, @StatusId);
               END";

                            using (SqlCommand cmdAssignment = new SqlCommand(checkAssignmentQry, con, transaction))
                            {
                                cmdAssignment.Parameters.AddWithValue("@ServiceId", newServiceId);
                                cmdAssignment.Parameters.AddWithValue("@EmployeeId", ddlEmpId.SelectedValue);
                                cmdAssignment.Parameters.AddWithValue("@StatusId", 1); // StatusId for "Service Assigned"

                                cmdAssignment.ExecuteNonQuery();
                            }

                            // Update the service status
                            string updateServiceStatusQry = @"
               UPDATE Services 
               SET Status = 'Service Assigned', StatusId = 1 
               WHERE ServiceId = @ServiceId";

                            using (SqlCommand cmdUpdateStatus = new SqlCommand(updateServiceStatusQry, con, transaction))
                            {
                                cmdUpdateStatus.Parameters.AddWithValue("@ServiceId", newServiceId);
                                cmdUpdateStatus.ExecuteNonQuery();
                            }

                            // Commit the transaction
                            transaction.Commit();
                            string customerName = ddlCustId.SelectedItem.Text; // Assuming ddlCustId contains customer name
                            string serviceType = ddlservice.SelectedItem.Text; // Assuming ddlservice contains service type
                            string servicePeriod = $"{txtFromDate.Text} to {txtToDate.Text}";
                            // Send email notification
                            string employeeEmail = GetEmployeeEmail(ddlEmpId.SelectedValue);

                            ScriptManager.RegisterStartupScript(this, GetType(), "Alert", $"alert('Service assigned succesfully ');", true);
                            // Clear input fields after successful insertion
                            ClearFormFields();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ScriptManager.RegisterStartupScript(this, GetType(), "Alert", $"alert('An error occurred: {ex.Message}');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Alert", $"alert('An error occurred: {ex.Message}');", true);
            }
        }




        private void ClearFormFields()
        {
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
            txtAdvance.Text = string.Empty;
            txtRemarks.Text = string.Empty;
            ddlBranch.SelectedIndex = -1;
            ddlCustId.SelectedIndex = -1;
            ddlEmpId.SelectedIndex = -1;
            ddlservice.SelectedIndex = -1;
            ddldepartment.SelectedIndex = -1;
        }

        private string GetEmployeeEmail(string employeeId)
        {
            string email = string.Empty;
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "SELECT OfficialMail FROM Employees WHERE EmployeeId = @EmployeeId";
                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    email = cmd.ExecuteScalar()?.ToString();
                }
            }
            return email;
        }

        private void SendEmail(string toEmail, string subject, string customerName, string serviceType, string servicePeriod, string firstName)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                message.From = new MailAddress("noreply@vivifysoft.in");
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;


                message.IsBodyHtml = true;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


                smtp.Port = 465;
                smtp.Host = "mail.rdsindia.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("noreply@vivifysoft.in", "3Vivify@reimbURSE");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Send(message);

                string body = $@"
                    <p>Dear {firstName},</p>
                    <p>We are pleased to inform you that you have been assigned to provide service to the following customer:</p>
                    <ul>
                        <li><b>Customer Name:</b> {customerName}</li>
                        <li><b>Service Type:</b> {serviceType}</li>
                        <li><b>Service Period:</b> {servicePeriod}</li>
                    </ul>
                    <p>Please ensure that you provide high-quality service and maintain our company's standards.</p>
                    <p>Best regards,</p>
                    <p><b>HR,</b><br>Vivify Technocrats</p>";


            }
            catch (SmtpException smtpEx)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Alert", $"alert('SMTP Error: {smtpEx.Message}');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Alert", $"alert('General Error: {ex.Message}');", true);
            }
        }
    }
}