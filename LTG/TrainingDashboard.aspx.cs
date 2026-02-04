using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class TrainingDashboard : System.Web.UI.Page
    {
        private const string ConnectionStringName = "vivify";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTrainingExpenses();
            }
        }

        private void LoadTrainingExpenses()
        {
            // Retrieve the logged-in employee's ID from the session
            string employeeId = Session["EmployeeID"]?.ToString();

            if (string.IsNullOrEmpty(employeeId))
            {
                // Redirect to login if the EmployeeID is not set
                //Response.Redirect("Login.aspx");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT Training_Id, FirstName, FromDate AS Date, Training_Details, StatusId
            FROM Training
            WHERE EmployeeId = @EmployeeId
            ORDER BY Training_Id DESC, FromDate DESC"; // Include StatusId

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add EmployeeId as a parameter to prevent SQL injection
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Bind the filtered data to the GridView
                        gvTrainingExpenses.DataSource = dataTable;
                        gvTrainingExpenses.DataBind();
                    }
                }
            }
        }

        protected void gvTrainingExpenses_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Code to handle row selection
        }

        protected void gvTrainingExpenses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Proceed")
            {
                // Retrieve the training ID from the CommandArgument
                string trainingId = e.CommandArgument.ToString();

                // Store the TrainingId in a cookie
                HttpCookie trainingCookie = new HttpCookie("TrainingId", trainingId);

                Response.Cookies.Add(trainingCookie);

                // Redirect to the Training page
                Response.Redirect("Training_Expense.aspx");
            }
        }

    }
}