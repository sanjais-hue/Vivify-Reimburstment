using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class Main : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Fetch user information from cookies
                HttpCookie firstNameCookie = Request.Cookies["FirstName"];
                HttpCookie employeeIdCookie = Request.Cookies["EmployeeId"];

                // Ensure the controls are correctly defined in Main.Master
                if (firstNameCookie != null)
                {
                    lblProfileName.Text = "<strong>" + firstNameCookie.Value + "</strong>";
                }
                else
                {
                    lblProfileName.Text = "<strong>Guest</strong>";
                }

                if (employeeIdCookie != null)
                {
                    // Fetch the role from the database
                    string role = GetUserRole(employeeIdCookie.Value);
                    lblProfileRole.Text = role != null ? role : "<strong>Role not available</strong>";
                }
                else
                {
                    lblProfileRole.Text = "Role not available";
                }

                // Redirect to the login page if not logged in
                if (GetLoginType() == null)
                {
                    Response.Redirect("AdminPage.aspx");
                }
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string employeeId = Request.Cookies["EmployeeId"]?.Value; // Get EmployeeId from cookies

            // Ensure cookie exists
            if (string.IsNullOrEmpty(employeeId))
            {
                lblMessage.Text = "User not logged in.";
                lblMessage.ForeColor = System.Drawing.Color.Red; // You can set the color to red for error messages
                ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "$('#changePasswordModal').modal('hide');", true);

                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#changePasswordModal').modal('show');", true);
                return;
            }

            string currentPassword = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validate new password and confirm password match
            if (newPassword != confirmPassword)
            {
                lblMessage.Text = "New password and confirm password do not match.";
                lblMessage.ForeColor = System.Drawing.Color.Red; // Error message
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#changePasswordModal').modal('show');", true);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    // Verify the current password
                    string query = "SELECT COUNT(*) FROM Employees WHERE EmployeeId = @EmployeeId AND Password = @CurrentPassword";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.Parameters.AddWithValue("@CurrentPassword", currentPassword);

                        int count = (int)cmd.ExecuteScalar();
                        if (count == 0)
                        {
                            lblMessage.Text = "Current password is incorrect.";
                            lblMessage.ForeColor = System.Drawing.Color.Red; // Error message
                            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#changePasswordModal').modal('show');", true);
                            return;
                        }
                    }

                    // Update the password directly in the Password column
                    string updateQuery = @"
                UPDATE Employees 
                SET Password = @NewPassword 
                WHERE EmployeeId = @EmployeeId";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@NewPassword", newPassword); // Store the password as plain text
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // If the password change is successful
                lblMessage.Text = "Password changed successfully.";
                lblMessage.ForeColor = System.Drawing.Color.Green; // Success message
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#changePasswordModal').modal('show');", true);

            }
            catch (Exception ex)
            {
                // Handle any errors that may occur
                lblMessage.Text = "An error occurred: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red; // Error message
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#changePasswordModal').modal('show');", true);
            }
        }
        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            // Close the modal when the close button is clicked
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeModal", "$('#changePasswordModal').modal('hide');", true);
        }

        private string GetLoginType()
        {
            HttpCookie employeeIdCookie = Request.Cookies["EmployeeId"];
            HttpCookie firstNameCookie = Request.Cookies["FirstName"];

            // Check if the cookies are available
            if (employeeIdCookie == null || firstNameCookie == null)
            {
                return null;
            }

            // Set hidden fields with cookie values
            hdnLoginId.Value = employeeIdCookie.Value;
            hdnUserName.Value = firstNameCookie.Value;

            return string.Empty;
        }

        private string GetUserRole(string employeeId)
        {
            string role = null;
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string query = "SELECT Roles FROM Employees WHERE EmployeeId = @EmployeeId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        role = result.ToString();
                    }
                }
            }

            return role;
        }
    }
}