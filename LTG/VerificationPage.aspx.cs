using System;
using System.Data.SqlClient;
using System.Configuration;

namespace Vivify
{
    public partial class VerificationPage : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get ServiceId and ExpenseType from QueryString
                lblServiceId.Text = $"Service ID: {Request.QueryString["serviceId"]}";
                string expenseType = Request.QueryString["expenseType"];

                // Store the expense type for later use
                ViewState["ExpenseType"] = expenseType;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int serviceId = Convert.ToInt32(Request.QueryString["serviceId"]);
            bool isClaimable = chkClaimable.Checked;
            bool isNonClaimable = chkNonClaimable.Checked;

            // Check if ExpenseType exists in ViewState
            string expenseType = ViewState["ExpenseType"] as string;
            if (expenseType == null)
            {
                // Handle the error or set a default value
                // e.g., Response.Redirect("ErrorPage.aspx");
                return; // Exit if ExpenseType is not available
            }

            // Save to database
            UpdateClaimableStatus(serviceId, isClaimable, isNonClaimable, expenseType);
        }

        private void UpdateClaimableStatus(int serviceId, bool isClaimable, bool isNonClaimable, string expenseType)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "UPDATE Expenses SET Claimable = @Claimable, NonClaimable = @NonClaimable, ExpenseType = @ExpenseType WHERE ServiceId = @ServiceId"; // Adjust based on your DB schema
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Claimable", isClaimable);
                    cmd.Parameters.AddWithValue("@NonClaimable", isNonClaimable);
                    cmd.Parameters.AddWithValue("@ExpenseType", expenseType);
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminPage.aspx");
        }
    }
}
