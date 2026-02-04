using System;
using System.Collections.Generic;
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
    public partial class ServiceAssignment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Bind Branch and Customer on page load
                bindBranch();
                BindCustomer();

                // Assuming the branchId is set from the selected branch
                string branchId = ddlBranch.SelectedValue;

                // Bind employee dropdown with the logged-in employee
                BindEmployee(branchId);

                // If the user is logged in, pre-select the employee's branch
                int employeeId = Convert.ToInt32(Session["EmployeeId"]);
                if (employeeId > 0)
                {
                    SetEmployeeBranch(employeeId);
                }

                // If branch is selected, bind customers for that branch
                if (ddlBranch.Items.Count > 0)
                {
                    ddlBranch.SelectedIndex = 0;
                    BindCustomer(); // Rebind customers after setting branch
                }
            }
        }


        private void bindBranch()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // Assuming you store the employee's ID in a session (e.g., employee ID)
                int employeeId = Convert.ToInt32(Session["EmployeeId"]);  // Replace with your session key

                // Query to fetch the branch associated with the logged-in employee
                string qry = "SELECT BranchId, BranchName FROM Branch WHERE BranchId = (SELECT BranchId FROM Employees WHERE EmployeeId = @EmployeeId)";

                SqlCommand cmd1 = new SqlCommand(qry, con);
                cmd1.Parameters.AddWithValue("@EmployeeId", employeeId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Check if a branch was found for the employee
                    if (dt.Rows.Count > 0)
                    {
                        ddlBranch.DataSource = dt;
                        ddlBranch.DataTextField = "BranchName";
                        ddlBranch.DataValueField = "BranchId";
                        ddlBranch.DataBind();

                        // Set the selected index to the first branch (employee's assigned branch)
                        ddlBranch.SelectedIndex = 0;  // or you can set it based on some other condition
                    }
                    else
                    {
                        // If no branch is found, you can either show a message or handle it as needed
                        ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('No branch found for the logged-in employee.');", true);
                    }
                }
            }
        }

        private void BindCustomer()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string selectedBranchId = ddlBranch.SelectedValue;
                // Skip if no branch selected
                if (string.IsNullOrEmpty(selectedBranchId) || selectedBranchId == "0")
                {
                    ddlCustId.Items.Clear();
                    return;
                }

                string qry = "SELECT CustomerId, CustomerName FROM Customers WHERE BranchId = @BranchId";
                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@BranchId", selectedBranchId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlCustId.DataSource = dt;
                        ddlCustId.DataTextField = "CustomerName";
                        ddlCustId.DataValueField = "CustomerId";
                        ddlCustId.DataBind();
                    }
                }
            }
        }
        private void BindEmployee(string branchId)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // Get the EmployeeId from session (Assuming this is the logged-in employee)
                int employeeId = Convert.ToInt32(Session["EmployeeId"]);

                // Query to fetch employee details for the logged-in user
                string qry = "SELECT EmployeeId, FirstName + ' (' + EmployeeCode + ')' AS displayname " +
                             "FROM Employees WHERE BranchId = @BranchId AND EmployeeId = @EmployeeId";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@BranchId", branchId);
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId); // Filter by the logged-in employee

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // If employee is found, bind it to the dropdown
                        if (dt.Rows.Count > 0)
                        {
                            // Clear any existing items in the dropdown
                            ddlEmpId.Items.Clear();

                            // Add the logged-in employee as the only item in the dropdown
                            ListItem listItem = new ListItem(dt.Rows[0]["displayname"].ToString(), dt.Rows[0]["EmployeeId"].ToString());
                            ddlEmpId.Items.Add(listItem);

                            // Set the selected index to the first item (which is the logged-in employee)
                            ddlEmpId.SelectedIndex = 0;
                        }
                        else
                        {
                            ddlEmpId.Items.Clear(); // In case no employee found for the branch, clear the dropdown
                            ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('No employee found for the selected branch.');", true);
                        }
                    }
                }
            }
        }


        private void SetEmployeeBranch(int employeeId)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "SELECT BranchId FROM Employees WHERE EmployeeId = @EmployeeId";
                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    object result = cmd.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        string branchId = result.ToString();
                        ddlBranch.SelectedValue = branchId;

                        // After setting the branch, rebind customers for that branch
                        BindCustomer();
                    }
                }
            }
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Rebind customers when the branch is changed
            BindCustomer();
        }

        protected void btnService_Click(object sender, EventArgs e)
        {
            try
            {
                HttpCookie firstNameCookie = Request.Cookies["FirstName"];
                string firstName = firstNameCookie?.Value ?? "System";
                string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

                if (string.IsNullOrEmpty(constr))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Database connection string is not defined.');", true);
                    return;
                }

                // === Collect ALL selected customer IDs ===
                var selectedIds = new List<string>();
                foreach (ListItem item in ddlCustId.Items)
                {
                    if (item.Selected && !string.IsNullOrEmpty(item.Value) && item.Value != "0")
                    {
                        selectedIds.Add(item.Value);
                    }
                }

                if (selectedIds.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Please select at least one customer.');", true);
                    return;
                }

                string customerIdsString = string.Join(",", selectedIds); // e.g., "34,45,67"

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // === INSERT using CustomerId as STRING (comma-separated) ===
                            string insertServiceQry = @"
                        INSERT INTO Services (BranchId, CustomerId, EmployeeId, ServiceType, FromDate, ToDate, Advance, Remarks, Department, CreatedDate, CreatedBy, StatusId, Status) 
                        VALUES (@BranchId, @CustomerId, @EmployeeId, @ServiceType, @FromDate, @ToDate, @Advance, @Remarks, @Department, GETDATE(), @CreatedBy, 1, 'Service Assigned'); 
                        SELECT SCOPE_IDENTITY();";

                            int newServiceId;
                            using (SqlCommand cmd = new SqlCommand(insertServiceQry, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BranchId", ddlBranch.SelectedValue);
                                cmd.Parameters.AddWithValue("@CustomerId", customerIdsString); // ✅ Multi IDs as string
                                cmd.Parameters.AddWithValue("@EmployeeId", ddlEmpId.SelectedValue);
                                cmd.Parameters.AddWithValue("@ServiceType", ddlservice.SelectedItem.Text);

                                DateTime fromDate, toDate;
                                cmd.Parameters.AddWithValue("@FromDate", DateTime.TryParse(txtFromDate.Text, out fromDate) ? (object)fromDate : DBNull.Value);
                                cmd.Parameters.AddWithValue("@ToDate", DateTime.TryParse(txtToDate.Text, out toDate) ? (object)toDate : DBNull.Value);

                                decimal advanceAmount = 0;
                                decimal.TryParse(txtAdvance.Text, out advanceAmount);
                                cmd.Parameters.AddWithValue("@Advance", advanceAmount);

                                cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(txtRemarks.Text) ? DBNull.Value : (object)txtRemarks.Text);
                                cmd.Parameters.AddWithValue("@Department", ddldepartment.SelectedItem.Text);
                                cmd.Parameters.AddWithValue("@CreatedBy", firstName);

                                newServiceId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            transaction.Commit();
                            ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Service assigned successfully!');", true);
                            ClearFormFields();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ScriptManager.RegisterStartupScript(this, GetType(), "Alert", $"alert('Error: {ex.Message}');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Alert", $"alert('Unexpected error: {ex.Message}');", true);
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