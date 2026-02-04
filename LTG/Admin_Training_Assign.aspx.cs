using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class Admin_Training_Assign : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Bind the dropdowns or other initializations
                BindBranchDropdown();

                // Check if the form was submitted using Session
                if (Session["IsFormSubmitted"] != null && (bool)Session["IsFormSubmitted"])
                {
                    // Disable the submit button
                    btnSubmit.Enabled = false;

                    // Check if there is a success message in Session
                    if (Session["SuccessMessage"] != null)
                    {
                        lblMessage.Text = Session["SuccessMessage"].ToString();
                        lblMessage.ForeColor = System.Drawing.Color.Green;

                        // Remove the message after displaying it once
                        Session.Remove("SuccessMessage");
                    }
                }
            }
        }

        private void BindBranchDropdown()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT BranchId, BranchName FROM Branch";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlBranch.DataSource = reader;
                ddlBranch.DataTextField = "BranchName";
                ddlBranch.DataValueField = "BranchId";
                ddlBranch.DataBind();
            }

            ddlBranch.Items.Insert(0, new ListItem("Select Branch", ""));
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            string branchId = ddlBranch.SelectedValue;
            if (!string.IsNullOrEmpty(branchId))
            {
                BindEmployeeDropdown(branchId);
            }
            else
            {
                ddlEmployee.Items.Clear();
                ddlEmployee.Items.Add(new ListItem("Select Employee", ""));
            }
        }

        private void BindEmployeeDropdown(string branchId)
        {
            List<Employee> employees = GetEmployeesByBranch(branchId);

            ddlEmployee.Items.Clear();
            ddlEmployee.Items.Add(new ListItem("Select Employee", ""));

            foreach (var employee in employees)
            {
                ddlEmployee.Items.Add(new ListItem(employee.FirstName, employee.EmployeeId));
            }
        }

        private List<Employee> GetEmployeesByBranch(string branchId)
        {
            List<Employee> employees = new List<Employee>();
            string connectionString = ConfigurationManager.ConnectionStrings["Vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT EmployeeId, FirstName FROM Employees WHERE BranchId = @BranchId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BranchId", branchId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        EmployeeId = reader["EmployeeId"].ToString(),
                        FirstName = reader["FirstName"].ToString()
                    });
                }
            }

            return employees;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Get values from form fields
            string branchId = ddlBranch.SelectedValue;
            string employeeId = ddlEmployee.SelectedValue;
            string fromDate = txtFromDate.Text;
            string toDate = txtToDate.Text;
            string trainingDetails = txtTrainingDetails.Text;

            // Fetch the employee's first name from the database based on the selected EmployeeId
            string employeeFirstName = GetEmployeeFirstName(employeeId);

            // Fetch 'CreatedBy' from cookies (admin's first name)
            string createdBy = GetLoggedInEmployeeFirstNameFromCookies();
            DateTime createdDate = DateTime.Now;

            // Save the training data to the database
            string connectionString = ConfigurationManager.ConnectionStrings["Vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO Training 
            (BranchId, EmployeeId, FirstName, FromDate, ToDate, Training_Details, CreatedBy, CreatedDate) 
            VALUES (@BranchId, @EmployeeId, @FirstName, @FromDate, @ToDate, @TrainingDetails, @CreatedBy, @CreatedDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BranchId", branchId);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@FirstName", employeeFirstName);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@TrainingDetails", trainingDetails);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@CreatedDate", createdDate);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Mark form as submitted and store success message in Session
            Session["IsFormSubmitted"] = true;

            // Call JavaScript to display alert, clear fields, and re-enable button
            string script = "alert('Training assigned successfully!'); clearFormAndReenableSubmitButton();";
            ClientScript.RegisterStartupScript(this.GetType(), "submitSuccess", script, true);
        }

        private string GetEmployeeFirstName(string employeeId)
        {
            string firstName = string.Empty;

            string connectionString = ConfigurationManager.ConnectionStrings["Vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT FirstName FROM Employees WHERE EmployeeId = @EmployeeId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    firstName = reader["FirstName"].ToString();
                }
            }

            return firstName;
        }

        private string GetLoggedInEmployeeFirstNameFromCookies()
        {
            string firstName = "Admin";

            if (Request.Cookies["UserFirstName"] != null)
            {
                firstName = Request.Cookies["UserFirstName"].Value;
            }

            return firstName;
        }

        public class Employee
        {
            public string EmployeeId { get; set; }
            public string FirstName { get; set; }
        }
    }
}
