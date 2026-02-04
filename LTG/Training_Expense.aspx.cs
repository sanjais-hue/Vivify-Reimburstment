using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Vivify
{
    public partial class Training_Expense : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize any necessary data on first load
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string reimbursementType = ddlReimbursementType.SelectedValue;

            // Ensure the user selects a reimbursement type
            if (string.IsNullOrEmpty(reimbursementType))
            {
                ShowAlert("Please select a reimbursement type.", "error");
                return;
            }

            // Fetch user information and other data
            string createdBy = GetFirstNameFromCookies();
            string trainingId = FetchTrainingIdByEmployee(createdBy);

            if (string.IsNullOrEmpty(trainingId))
            {
                ShowAlert("No training record found for the employee.", "error");
                return;
            }

            string fromDate = string.Empty;
            string toDate = string.Empty;
            string fromTime = string.Empty;
            string toTime = string.Empty;

            if (reimbursementType == "Conveyance")
            {
                // For Conveyance: Get the respective fields
                fromDate = txtFromDate.Text.Trim();
                toDate = txtToDate.Text.Trim();
                fromTime = txtFromTime.Text.Trim();
                toTime = txtToTime.Text.Trim();
            }
            else if (reimbursementType == "Food")
            {
                // For Food: Get the respective fields
                fromDate = txtFromDateFood.Text.Trim();
                toDate = txtToDateFood.Text.Trim();
                fromTime = txtFromTimeFood.Text.Trim();
                toTime = txtToTimeFood.Text.Trim();
            }

            // Validate if dates and times are correct (if not, show an error)
            DateTime parsedFromDate, parsedToDate;
            if (string.IsNullOrEmpty(fromDate) || !DateTime.TryParse(fromDate, out parsedFromDate))
            {
                ShowAlert("Please select a valid From Date.", "error");
                return;
            }

            if (string.IsNullOrEmpty(toDate) || !DateTime.TryParse(toDate, out parsedToDate))
            {
                ShowAlert("Please select a valid To Date.", "error");
                return;
            }

            if (string.IsNullOrEmpty(fromTime) || string.IsNullOrEmpty(toTime))
            {
                ShowAlert("Please enter both From Time and To Time.", "error");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (reimbursementType == "Conveyance")
                    {
                        // Validate required fields for Conveyance
                        string transportType = ddlTransportType.SelectedValue;
                        string amount = txtAmountConveyance.Text.Trim();
                        string particulars = txtParticularsConveyance.Text.Trim();

                        if (string.IsNullOrEmpty(transportType) || string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(particulars))
                        {
                            ShowAlert("Please fill all fields for Conveyance reimbursement.", "error");
                            return;
                        }

                        // Execute SQL command for Conveyance
                        cmd.CommandText = @"
                INSERT INTO Conv_Training (TransportType, Distance, Amount, FromDate, FromTime, ToDate, ToTime, Training_Id, Particulars, CreatedDate, CreatedBy)
                VALUES (@TransportType, @Distance, @Amount, @FromDate, @FromTime, @ToDate, @ToTime, @TrainingID, @Particulars, GETDATE(), @CreatedBy)";

                        cmd.Parameters.AddWithValue("@TransportType", transportType);
                        cmd.Parameters.AddWithValue("@Distance", string.IsNullOrEmpty(txtDistance.Text) ? (object)DBNull.Value : txtDistance.Text);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@FromDate", parsedFromDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@FromTime", DateTime.Parse(fromTime).ToString("HH:mm"));
                        cmd.Parameters.AddWithValue("@ToDate", parsedToDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@ToTime", DateTime.Parse(toTime).ToString("HH:mm"));
                        cmd.Parameters.AddWithValue("@TrainingID", trainingId);
                        cmd.Parameters.AddWithValue("@Particulars", particulars);
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    }
                    else if (reimbursementType == "Food")
                    {
                        // Validate required fields for Food
                        string amount = txtAmountFood.Text.Trim();
                        string particulars = txtParticularsFood.Text.Trim();

                        if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(particulars))
                        {
                            ShowAlert("Please fill all fields for Food reimbursement.", "error");
                            return;
                        }

                        // Execute SQL command for Food
                        cmd.CommandText = @"
                INSERT INTO Food_Training (Amount, FromDate, FromTime, ToDate, ToTime, Training_Id, Particulars, CreatedDate, CreatedBy)
                VALUES (@Amount, @FromDate, @FromTime, @ToDate, @ToTime, @TrainingID, @Particulars, GETDATE(), @CreatedBy)";

                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@FromDate", parsedFromDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@FromTime", DateTime.Parse(fromTime).ToString("HH:mm"));
                        cmd.Parameters.AddWithValue("@ToDate", parsedToDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@ToTime", DateTime.Parse(toTime).ToString("HH:mm"));
                        cmd.Parameters.AddWithValue("@TrainingID", trainingId);
                        cmd.Parameters.AddWithValue("@Particulars", particulars);
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    }

                    // Execute the command
                    cmd.ExecuteNonQuery();
                    ClearFormFields();
                    ShowAlert("Data  Submitted successfully.", "success");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("An error occurred: " + ex.Message, "error");
            }
        }


        private void ShowAlert(string message, string type)
        {
            string script = $"<script type=\"text/javascript\">showAlert('{message}', '{type}');</script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showalert", script);
        }

        private void ClearFormFields()
        {
            // Reset all fields
            txtAmountConveyance.Text = string.Empty;
            txtDistance.Text = string.Empty;
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
            txtFromTime.Text = string.Empty;
            txtToTime.Text = string.Empty;
            txtParticularsConveyance.Text = string.Empty;

            txtAmountFood.Text = string.Empty;
            txtParticularsFood.Text = string.Empty;
            txtFromDateFood.Text = string.Empty;
            txtToDateFood.Text = string.Empty;
            txtFromTimeFood.Text = string.Empty;
            txtToTimeFood.Text = string.Empty;

            ddlReimbursementType.SelectedIndex = 0;
        }

        private string GetFirstNameFromCookies()
        {
            HttpCookie firstNameCookie = Request.Cookies["FirstName"];
            return firstNameCookie?.Value;
        }

        private string FetchTrainingIdByEmployee(string createdBy)
        {
            string trainingId = null;
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand
                {
                    Connection = conn,
                    CommandText = @"
                    SELECT TOP 1 Training_Id
                    FROM Training
                    WHERE FirstName = @FirstName
                    ORDER BY CreatedDate DESC"
                };
                cmd.Parameters.AddWithValue("@FirstName", createdBy);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    trainingId = reader["Training_Id"].ToString();
                }
            }

            return trainingId;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Call the ClearFormFields method to reset all form fields
            ClearFormFields1();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string createdBy = GetFirstNameFromCookies();
            string trainingId = FetchTrainingIdByEmployee(createdBy);

            // Ensure that the TrainingId exists
            if (string.IsNullOrEmpty(trainingId))
            {
                ShowAlert("No training record found for the employee.", "error");
                return;
            }

            // Fetch EmployeeId from the cookies or another method (assuming you can get it from cookies)
            int employeeId = GetEmployeeIdFromCookies();

            // Call the method to update the training status to 'Saved' (statusId = 2)
            bool updateSuccess = UpdateTrainingStatus(trainingId, 2, employeeId);

            // Show the appropriate alert based on the result
            if (updateSuccess)
            {
                ShowAlert("Training status updated successfully to 'Saved'.", "success");
            }
            else
            {
                ShowAlert("Failed to update the training status. Please check the TrainingId.", "error");
            }
        }

    

        private int GetEmployeeIdFromCookies()
        {
            // Fetch EmployeeId from cookies (you need to adjust this according to your logic)
            HttpCookie employeeIdCookie = Request.Cookies["EmployeeId"];
            if (employeeIdCookie != null && int.TryParse(employeeIdCookie.Value, out int employeeId))
            {
                return employeeId;
            }

            // If employeeId is not found in cookies, handle appropriately
            ShowAlert("Employee ID not found.", "error");
            return -1; // Return an invalid ID or throw an exception if required
        }

        private bool UpdateTrainingStatus(string trainingId, int statusId, int employeeId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string updateQuery = "UPDATE Training SET StatusId = @StatusId WHERE Training_Id = @TrainingId AND EmployeeId = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        // Set the parameters for the query
                        cmd.Parameters.AddWithValue("@StatusId", statusId);
                        cmd.Parameters.AddWithValue("@TrainingId", trainingId);
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                        // Execute the update query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Return true if rows were affected, meaning the update was successful
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Show an error message if an exception occurs
                ShowAlert("An error occurred: " + ex.Message, "error");
                return false;  // Return false on error
            }
        }

        private void ClearFormFields1()
        {
            // Reset all fields
            txtAmountConveyance.Text = string.Empty;
            txtDistance.Text = string.Empty;
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
            txtFromTime.Text = string.Empty;
            txtToTime.Text = string.Empty;
            txtParticularsConveyance.Text = string.Empty;

            txtAmountFood.Text = string.Empty;
            txtParticularsFood.Text = string.Empty;
            txtFromDateFood.Text = string.Empty;
            txtToDateFood.Text = string.Empty;
            txtFromTimeFood.Text = string.Empty;
            txtToTimeFood.Text = string.Empty;

            ddlReimbursementType.SelectedIndex = 0;
            ddlTransportType.SelectedIndex = 0;  // Reset transport type dropdown if needed
        }

    }
}