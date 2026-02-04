using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class TrainingAssign : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Fetch first name from cookies
                string firstName = GetFirstNameFromCookies();
                if (!string.IsNullOrEmpty(firstName))
                {
                    txtEmployeeName.Text = firstName;  // Set the text field with the first name
                }
                else
                {
                    txtEmployeeName.Text = "First name not found.";  // Display a message if cookie is not found
                    txtEmployeeName.ForeColor = System.Drawing.Color.Red;  // Change text color to red
                }
            }
        }

        // Get First Name from Cookies
        private string GetFirstNameFromCookies()
        {
            HttpCookie employeeCookie = Request.Cookies["FirstName"];
            return employeeCookie != null && !string.IsNullOrEmpty(employeeCookie.Value) ? employeeCookie.Value : null;
        }

        // Handle Submit Button Click
        // Handle Submit Button Click
        // Handle Submit Button Click
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validate inputs
            string employeeName = txtEmployeeName.Text;
            string trainingDetails = txtTrainingDetails.Text;

            // Check if employee name is valid
            if (string.IsNullOrEmpty(employeeName) || employeeName == "First name not found.")
            {
                ShowErrorMessage("Employee name is missing or invalid.");
                return; // Prevent form submission
            }

            // Check if training details are provided
            if (string.IsNullOrEmpty(trainingDetails))
            {
                ShowErrorMessage("Training details are required.");
                return; // Prevent form submission
            }

            // Validate From Date
            if (!DateTime.TryParse(txtFromDate.Text, out DateTime fromDate))
            {
                ShowErrorMessage("From Date is invalid.");
                return; // Prevent form submission
            }

            // Validate To Date
            if (!DateTime.TryParse(txtToDate.Text, out DateTime toDate))
            {
                ShowErrorMessage("To Date is invalid.");
                return; // Prevent form submission
            }

            // Check if From Date is not later than To Date
            if (fromDate > toDate)
            {
                ShowErrorMessage("From Date cannot be later than To Date.");
                return; // Prevent form submission
            }

            // Fetch Employee details based on FirstName
            var employeeDetails = GetEmployeeDetailsByFirstName(employeeName);

            // Validate employee details
            if (string.IsNullOrEmpty(employeeDetails.EmployeeId) ||
                string.IsNullOrEmpty(employeeDetails.BranchId) ||
                string.IsNullOrEmpty(employeeDetails.OfficialMail))
            {
                ShowErrorMessage("Employee details (ID, Branch, or Official Mail) not found.");
                return; // Prevent form submission
            }

            // Get the FirstName (for CreatedBy) from cookies
            string createdBy = GetFirstNameFromCookies();

            if (string.IsNullOrEmpty(createdBy))
            {
                ShowErrorMessage("CreatedBy (FirstName) is missing from cookies.");
                return; // Prevent form submission
            }

            // Save a new training record to the database
            SaveTrainingDetails(employeeDetails.EmployeeId, employeeDetails.BranchId, createdBy, fromDate, toDate, trainingDetails);

            // Reset fields after successful submission
            ResetFields();

            // Clear the error messages
            errorMessage.Text = string.Empty;
            errorToDate.Visible = false; // Hide the error message for To Date if validation passes
        }

        // Method to reset the fields
        private void ResetFields()
        {

            txtTrainingDetails.Text = string.Empty;
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
        }

        // Method to display error messages
        private void ShowErrorMessage(string message)
        {
            errorMessage.Text = message;
        }


        // Reset the fields after successful submission


        // Show error message
        protected void txtToDate_TextChanged(object sender, EventArgs e)
        {
            // Check if From Date and To Date are entered and valid
            if (DateTime.TryParse(txtFromDate.Text, out DateTime fromDate) && DateTime.TryParse(txtToDate.Text, out DateTime toDate))
            {
                // Validate if From Date is later than To Date
                if (fromDate > toDate)
                {
                    // Display the error message if From Date is later than To Date
                    errorToDate.Text = "To Date cannot be earlier than From Date.";
                    errorToDate.Visible = true; // Show the error message
                }
                else
                {
                    // Clear error message if dates are valid
                    errorToDate.Visible = false;
                }
            }
        }


        // Event handler for ToDate TextBox TextChanged


        // Fetch Employee details (EmployeeId, BranchId, OfficialMail) based on FirstName
        private (string EmployeeId, string BranchId, string OfficialMail) GetEmployeeDetailsByFirstName(string firstName)
        {
            string employeeId = null, branchId = null, officialMail = null;
            string connString = ConfigurationManager.ConnectionStrings["vivify"]?.ConnectionString;

            if (string.IsNullOrEmpty(connString))
            {
                ShowErrorMessage("Connection string is missing or invalid.");
                return (null, null, null);
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT EmployeeId, BranchId, OfficialMail FROM Employees WHERE FirstName = @FirstName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", firstName);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            employeeId = reader["EmployeeId"].ToString();
                            branchId = reader["BranchId"].ToString();
                            officialMail = reader["OfficialMail"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("An error occurred while fetching employee details: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }

            return (employeeId, branchId, officialMail); // Return tuple of employee details
        }

        // Save training details to the database
        private void SaveTrainingDetails(string employeeId, string branchId, string createdBy, DateTime fromDate, DateTime toDate, string trainingDetails)
        {
            string connString = ConfigurationManager.ConnectionStrings["vivify"]?.ConnectionString;

            if (string.IsNullOrEmpty(connString))
            {
                ShowErrorMessage("Connection string is missing or invalid.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "INSERT INTO Training (EmployeeId, BranchId, FirstName, FromDate, ToDate, Training_Details, CreatedDate, CreatedBy) " +
                               "VALUES (@EmployeeId, @BranchId, @FirstName, @FromDate, @ToDate, @TrainingDetails, GETDATE(), @CreatedBy)";
                SqlCommand cmd = new SqlCommand(query, conn);

                // Add parameters
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId); // Make sure this is not null or empty
                cmd.Parameters.AddWithValue("@BranchId", branchId); // Make sure this is not null or empty
                cmd.Parameters.AddWithValue("@FirstName", txtEmployeeName.Text); // Use the first name from the UI
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@TrainingDetails", trainingDetails);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Response.Write("<script>alert('Training details saved successfully.');</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Failed to save training details. No rows were affected.');</script>");
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("An error occurred while saving training details: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        // Event handler for ToDate TextBox TextChanged
        // Event handler for ToDate TextBox TextChanged
        // Event handler for ToDate TextBox TextChanged


    }
}