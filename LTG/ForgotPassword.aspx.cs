using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Vivify
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If session data exists, populate the fields
            if (Session["OfficialMail"] != null)
            {
                txtOfficialEmail.Text = Session["OfficialMail"].ToString();
                txtNewPassword.Text = Session["NewPassword"].ToString();
                txtConfirmPassword.Text = Session["ConfirmPassword"].ToString();
            }

            lblMessage.Text = "";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string officialMail = txtOfficialEmail.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validate matching passwords
            if (newPassword != confirmPassword)
            {
                lblMessage.Text = "Passwords do not match.";
                return;
            }

            // Validate empty fields
            if (string.IsNullOrWhiteSpace(officialMail) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                lblMessage.Text = "All fields are required.";
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if the email exists
                    string checkEmailQuery = "SELECT COUNT(*) FROM Employees WHERE OfficialMail = @OfficialMail";
                    using (SqlCommand cmd = new SqlCommand(checkEmailQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@OfficialMail", officialMail);

                        int count = (int)cmd.ExecuteScalar();
                        if (count == 0)
                        {
                            lblMessage.Text = "Email not found.";
                            return;
                        }
                    }

                    // Hash the new password
                    string hashedPassword = HashPassword(newPassword);

                    // Update password and nullify NewPassword column
                    string updatePasswordQuery = "UPDATE Employees SET Password = @Password WHERE OfficialMail = @OfficialMail";
                    using (SqlCommand cmd = new SqlCommand(updatePasswordQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@OfficialMail", officialMail);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            lblMessage.Text = "Password updated successfully.";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            lblMessage.Text = "Failed to update password.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);

                // Provide a user-friendly message
                lblMessage.Text = "An unexpected error occurred. Please try again later.";
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            // Redirect to the login page
            Response.Redirect("Login.aspx");
        }


        private string HashPassword(string password)
        {
            // Simply return the password as-is
            return password;
        }

    }
}
