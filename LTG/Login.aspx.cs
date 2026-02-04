using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using WebGrease.Activities;

namespace Vivify
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Optionally, clear session on each load
            // Session.Clear();
        }

        protected void btnLogin1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate the user credentials
            if (ValidateUser(username, password, out DataRow userRow))
            {
                // Store employee ID in session
                Session["EmployeeId"] = userRow["EmployeeId"];

                // Create cookies for user details
                CreateUserCookies(userRow);

                // Check user role and redirect accordingly
                string userRole = userRow["Roles"].ToString();
                if (userRole.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect("AdminPage.aspx");
                }
                else if (userRole.Equals("employee", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect("Dashboard.aspx");
                }
                else
                {
                    ShowErrorMessage("Unknown Role. Access denied.");
                }
            }
            else
            {
                ShowErrorMessage("Invalid Email or Password. Please try again.");
            }
        }
        private bool ValidateUser(string username, string password, out DataRow userRow)
        {
            userRow = null;
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                // Modified query: No NewPassword check, only Password
                string query = @"
                    SELECT * 
                    FROM Employees 
                    WHERE Active = 1 
                      AND OfficialMail = @User 
                      AND Password = @Password"; // Only checking Password column

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@User", username);
                    cmd.Parameters.AddWithValue("@Password", password); // Use hashed password in production!

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            userRow = dt.Rows[0];
                            return true; // User found
                        }
                    }
                }
            }
            return false; // User not found
        }
        private void CreateUserCookies(DataRow userRow)
        {
            CreateCookie("EmployeeId", userRow["EmployeeId"].ToString());
            CreateCookie("FirstName", userRow["FirstName"].ToString());
            CreateCookie("DepartmentName", userRow["DepartmentName"].ToString());
            CreateCookie("Roles", userRow["Roles"].ToString()); // Store the role in a cookie if needed
        }

        private void CreateCookie(string name, string value)
        {
            HttpCookie cookie = new HttpCookie(name)
            {
                Value = value,
                Expires = DateTime.Now.AddHours(50) // Set expiration time as needed
            };
            Response.Cookies.Add(cookie);
        }

        private void ShowErrorMessage(string message)
        {
            lblError.Visible = true; // Show the error message
            lblError.Text = message; // Set the error message text
        }

    }
}
