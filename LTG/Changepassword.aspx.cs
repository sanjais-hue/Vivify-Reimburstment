using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Vivify
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Cookies["EmployeeId"] == null)
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string employeeId = Request.Cookies["EmployeeId"].Value; // Get EmployeeId from cookies

            string currentPassword = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (newPassword != confirmPassword)
            {
                lblMessage.Text = "New password and confirm password do not match.";
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
                            return;
                        }
                    }

                    // Update the password and set the Password column to NULL
                    string updateQuery = @"
                UPDATE Employees 
                SET NewPassword = @NewPassword, 
                    Password = NULL 
                WHERE EmployeeId = @EmployeeId";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Password changed successfully.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred: " + ex.Message;
            }


        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            // Redirect to the login page or another page as needed
            Response.Redirect("CustomerCreation.aspx"); // Change this to the page you want to redirect to
        }

    }
}