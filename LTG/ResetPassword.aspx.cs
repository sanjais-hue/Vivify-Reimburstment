using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

namespace LTG
{
    public partial class ResetPassword : Page
    {
        // This method is triggered when the "Reset Password" button is clicked
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validate the passwords
            if (newPassword != confirmPassword)
            {
                lblMessage.Text = "Passwords do not match.";
                lblMessage.Visible = true;
                return;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                lblMessage.Text = "Password cannot be empty.";
                lblMessage.Visible = true;
                return;
            }

            // Get the user's email from the cookie
            string officialEmail = GetEmailFromCookie();

            if (string.IsNullOrEmpty(officialEmail))
            {
                lblMessage.Text = "No email found in cookies.";
                lblMessage.Visible = true;
                return;
            }

            // Call the method to reset the password (this should be implemented based on your database logic)
            bool isPasswordResetSuccessful = ResetUserPassword(officialEmail, newPassword);

            if (isPasswordResetSuccessful)
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Your password has been reset successfully.";
                lblMessage.Visible = true;
            }
            else
            {
                lblMessage.Text = "There was an error resetting your password.";
                lblMessage.Visible = true;
            }
        }

        // Method to reset user password (database update logic)
        private bool ResetUserPassword(string officialEmail, string newPassword)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            bool passwordReset = false;

            // Hash the new password using SHA256 or bcrypt
            string hashedPassword = HashPassword(newPassword);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Update query based on official email
                string query = "UPDATE Employees SET Password = @Password WHERE OfficialEmail = @OfficialEmail";

                // Open the connection
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@OfficialEmail", officialEmail); // Use the official email to identify the user

                    // Execute the query and check if any rows were affected
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        passwordReset = true;
                    }
                }
            }

            return passwordReset;
        }

        // Method to get the official email from the cookie
        private string GetEmailFromCookie()
        {
            // Retrieve the email from the cookie
            HttpCookie emailCookie = Request.Cookies["UserEmail"];
            if (emailCookie != null)
            {
                return emailCookie.Value;
            }
            return null;
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login.aspx"); // Replace "Login.aspx" with the actual login page URL.
        }


        // Method to hash the password (using SHA256 for simplicity, bcrypt is more secure for production)
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute hash from the password string
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                foreach (byte byteValue in bytes)
                {
                    builder.Append(byteValue.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
