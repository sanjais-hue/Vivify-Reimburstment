using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Vivify
{
    public partial class Training : System.Web.UI.Page
    {
        // Connection string name from Web.config
        private const string ConnectionStringName = "vivify";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Fetch first name from cookies (instead of employee name)
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
            // Fetch the cookie with the name "FirstName"
            HttpCookie employeeCookie = Request.Cookies["FirstName"];
            if (employeeCookie != null && !string.IsNullOrEmpty(employeeCookie.Value))
            {
                return employeeCookie.Value;  // Return the value of the cookie (employee first name)
            }
            return null; // Return null if cookie is not found or if the cookie value is empty
        }

        // Handle Form Submission
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string employeeName = txtEmployeeName.Text.Trim();
                string trainingDetails = txtTrainingDetails.Text.Trim();
                string reimbursementType = ddlReimbursementType.SelectedValue;

                if (string.IsNullOrEmpty(reimbursementType))
                {
                    throw new Exception("Please select a reimbursement type.");
                }

                if (reimbursementType == "Conveyance")
                {
                    string transportType = ddlTransportType.SelectedValue;
                    double distance = double.TryParse(txtDistance.Text, out double dist) ? dist : 0;
                    double amount = double.TryParse(txtAmountConveyance.Text, out double amt) ? amt : 0;

                    // Validate inputs
                    if (distance <= 0) throw new Exception("Invalid distance for conveyance.");

                    // Process conveyance details (e.g., save to database)
                }
                else if (reimbursementType == "Food")
                {
                    // Process food details
                    DateTime.TryParse(txtFromDateFood.Text, out DateTime fromDateFood);
                    DateTime.TryParse(txtToDateFood.Text, out DateTime toDateFood);

                    if (fromDateFood > toDateFood)
                        throw new Exception("Invalid date range for food reimbursement.");
                }

                // Redirect or show success message
                Response.Write("<script>alert('Reimbursement submitted successfully!');</script>");
            }
            catch (Exception ex)
            {
                // Show error message
                Response.Write($"<script>alert('Error: {ex.Message}');</script>");
            }
        }

        // Save Conveyance Details
        private void SaveConveyanceDetails(string firstName, string trainingDetails, string fromTime, string toTime)
        {
            string fromDate = txtFromDateConveyance.Text;  // From Date
            string toDate = txtToDateConveyance.Text;      // To Date

            string transportType = ddlTransportType.SelectedValue;
            string distanceText = txtDistance.Text;
            decimal distance = 0;

            if (!decimal.TryParse(distanceText, out distance) || distance <= 0)
            {
                Response.Write("<script>alert('Invalid Distance');</script>");
                return;
            }

            const decimal ratePerKilometer = 13.5m;
            decimal amount = distance * ratePerKilometer;

            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO Training (FirstName, TrainingType, TrainingDetails, FromDate, ToDate, Particulars, TransportType, Distance, Amount, FromTime, ToTime)
                    VALUES (@FirstName, @TrainingType, @TrainingDetails, @FromDate, @ToDate, @Particulars, @TransportType, @Distance, @Amount, @FromTime, @ToTime)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@TrainingType", "Conveyance");
                    cmd.Parameters.AddWithValue("@TrainingDetails", trainingDetails);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    //cmd.Parameters.AddWithValue("@Particulars", txtParticularsConveyance.Text);
                    cmd.Parameters.AddWithValue("@TransportType", transportType);
                    cmd.Parameters.AddWithValue("@Distance", distance);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@FromTime", fromTime);
                    cmd.Parameters.AddWithValue("@ToTime", toTime);

                    cmd.ExecuteNonQuery();
                }
            }

            Response.Write("<script>alert('Conveyance details saved successfully.');</script>");
        }

        // Save Food Details
        private void SaveFoodDetails(string firstName, string trainingDetails)
        {
            string fromDate = txtFromDateFood.Text;  // From Date
            string toDate = txtToDateFood.Text;      // To Date

            decimal amount = 0;
            if (!decimal.TryParse(txtAmountFood.Text, out amount))
            {
                Response.Write("<script>alert('Invalid Food Amount');</script>");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO Training (FirstName, TrainingType, TrainingDetails, FromDate, ToDate, Amount, FromTime, ToTime)
                    VALUES (@FirstName, @TrainingType, @TrainingDetails, @FromDate, @ToDate, @Amount, @FromTime, @ToTime)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@TrainingType", "Food");
                    cmd.Parameters.AddWithValue("@TrainingDetails", trainingDetails);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    //cmd.Parameters.AddWithValue("@FromTime", txtFromTimeFood.Text);
                    //cmd.Parameters.AddWithValue("@ToTime", txtToTimeFood.Text);

                    cmd.ExecuteNonQuery();
                }
            }

            Response.Write("<script>alert('Food details saved successfully.');</script>");
        }
    }
}
