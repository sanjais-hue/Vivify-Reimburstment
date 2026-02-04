using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Web.UI;

namespace Vivify
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["success"] == "true")
                {
                    ShowAlert("Your message has been sent successfully!");
                }
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                string userEmail = txtEmail.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string subject = txtSubject.Text.Trim();
                string messageBody = txtMessage.Text.Trim();

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(userEmail) ||
                    string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(messageBody))
                {
                    ShowAlert("Please fill all required fields!");
                    return;
                }

                if (SendEmail(name, userEmail, phone, subject, messageBody))
                {
                 
                    Response.Redirect("Home.aspx?success=true", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    ShowAlert("Oops! Something went wrong.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                ShowAlert("Oops! Something went wrong.");
            }
        }




        private bool SendEmail(string name, string userEmail, string phone, string subject, string messageBody)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["SMTPFrom"], name),
                    Subject = $"Message from VivifySoft:",

                    IsBodyHtml = true,
                    Body = $@"
                <p><b>Name:</b> {name}</p>
                <p><b>Email:</b> {userEmail}</p>
                <p><b>Phone:</b> {phone}</p>
                <p><b>Message:</b></p>
                <p>{messageBody}</p>
            "
                };

                // ✅ Add the recipient (your company email)
                mail.To.Add(ConfigurationManager.AppSettings["SMTPFrom"]);

                // ✅ Set the user's email in Reply-To so you can reply directly
                mail.ReplyToList.Add(new MailAddress(userEmail));

                using (SmtpClient smtp = new SmtpClient(
                    ConfigurationManager.AppSettings["SMTPHost"],
                    int.Parse(ConfigurationManager.AppSettings["SMTPPort"])
                ))
                {
                    smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["SMTPEnableSsl"]);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(
                        ConfigurationManager.AppSettings["SMTPUser"],
                        ConfigurationManager.AppSettings["SMTPPass"]
                    );
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mail);
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowAlert("Error: " + ex.Message);
                return false;
            }
        }


        private void ShowAlert(string message)
        {
            string script = $"if (!localStorage.getItem('emailSent')) {{ alert('{message}'); }}";
            ScriptManager.RegisterStartupScript(this, GetType(), "Alert", script, true);
        }

    }
}
