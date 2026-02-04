using System;
using System.Net;
using System.Net.Mail;
using System.Web.UI;

namespace LTG
{
    public partial class ForgetPassword : System.Web.UI.Page
    {
        protected void btnSubmitEmail_Click(object sender, EventArgs e)
        {
            string userEmail = txtEmail.Text.Trim();

            // Check if the email is provided
            if (string.IsNullOrEmpty(userEmail))
            {
                lblMessage.Text = "Please enter a valid email address.";
                return;
            }

            // Step 1: Generate a password reset token (this can be a GUID or more secure token)
            string resetToken = Guid.NewGuid().ToString(); // Simple GUID for now, could be more secure

            // Step 2: Generate the reset link (this URL should be replaced with your actual reset password page URL)
            // Step 2: Generate the reset link
            string resetLink = $"https://vivifysoft.in/ResetPassword.aspx?token={resetToken}";


            // Step 3: Create the email body with the reset link
            string emailBody = $"<p>Click <a href='{resetLink}'>here</a> to reset your password.</p>";

            // Step 4: Send the email with the reset link
            try
            {
                SendEmail(userEmail, "Password Reset Request", emailBody); // Call the SendEmail method
                lblMessage.Text = "A password reset link has been sent to your email.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "There was an error sending the reset email. Please try again later.";
                Console.WriteLine("Error details: " + ex.Message); // Log error for debugging
            }
        }

        // The method that sends the email (as previously explained)
        private void SendEmail(string toEmail, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.vivifytec.in", 587); // Use the correct port (587 for TLS)

            try
            {
                // Set the email properties
                mail.From = new MailAddress("info@vivifytec.in");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;  // Send HTML emails

                // Set the credentials for the SMTP server
                smtpClient.Credentials = new NetworkCredential("info@vivifytec.in", "Dff6MSwzPMtq"); // SMTP username and password

                // Enable SSL/TLS (This is important for a secure connection)
                smtpClient.EnableSsl = true;

                // Send the email
                smtpClient.Send(mail);

                Console.WriteLine("Email sent successfully!");
            }
            catch (SmtpException smtpEx)
            {
                // Handle SMTP-specific errors
                string smtpErrorMessage = smtpEx.Message;
                Console.WriteLine($"SMTP Error: {smtpErrorMessage}");
            }
            catch (Exception ex)
            {
                // Handle other general errors
                string errorMessage = ex.Message;
                Console.WriteLine($"Error: {errorMessage}");
            }
        }

    }
}
