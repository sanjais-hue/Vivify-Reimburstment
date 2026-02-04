using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class AdminTraining : Page
    {
        // Connection string from Web.config
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindBranches();
                // Initially hide the GridView
                gvTraining.Visible = false;
            }
        }

        private void BindBranches()
        {
            string query = "SELECT BranchId, BranchName FROM Branch";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlBranch.DataSource = dt;
                    ddlBranch.DataTextField = "BranchName";
                    ddlBranch.DataValueField = "BranchId";
                    ddlBranch.DataBind();
                }
            }

            // Add a default item for all branches
            ddlBranch.Items.Insert(0, new ListItem("Select a Branch", "0"));
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlBranch.SelectedValue == "0")
            {
                // Hide GridView if no specific branch is selected
                gvTraining.Visible = false;
            }
            else
            {
                // Bind the GridView with the selected branch filter
                BindGridView();
            }
        }

        private void BindGridView()
        {
            string selectedBranchId = ddlBranch.SelectedValue;

            // Base query without WHERE clause
            string query = @"
        SELECT 
            T.Training_Id, 
            T.FirstName, 
            T.FromDate, 
            B.BranchName
        FROM 
            Training T
        INNER JOIN 
            Branch B 
        ON 
            T.BranchId = B.BranchId";

            // Apply the WHERE clause only if a specific branch is selected
            if (selectedBranchId != "0")
            {
                query += " WHERE T.BranchId = @BranchId"; // Add WHERE clause if needed
            }

            // Append the ORDER BY clause
            query += " ORDER BY T.Training_Id DESC, T.FromDate DESC";

            // Debugging: Output the query to check for issues
            // You can use this to verify the query before it's executed.
            System.Diagnostics.Debug.WriteLine(query);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Add the parameter for the WHERE clause, if applicable
                    if (selectedBranchId != "0")
                    {
                        cmd.Parameters.AddWithValue("@BranchId", selectedBranchId);
                    }

                    try
                    {
                        // Execute the query and bind the data
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Check if data exists
                        if (dt.Rows.Count > 0)
                        {
                            gvTraining.DataSource = dt;
                            gvTraining.DataBind();
                            gvTraining.Visible = true; // Show the GridView
                        }
                        else
                        {
                            gvTraining.Visible = false; // Hide the GridView
                                                        // Show an alert for no data
                            string alertMessage = "There are no training details for the selected branch.";
                            ClientScript.RegisterStartupScript(this.GetType(), "Alert", $"alert('{alertMessage}');", true);
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Log or show the SQL error for debugging
                        string errorMessage = "SQL Error: " + ex.Message;
                        ClientScript.RegisterStartupScript(this.GetType(), "Error", $"alert('{errorMessage}');", true);
                    }
                }
            }
        }

        protected void gvTraining_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Verify")
            {
                int trainingId = Convert.ToInt32(e.CommandArgument);
                // Perform action on the verification, e.g., update training status
                Response.Redirect("Training_Verify.aspx?trainingid=" + trainingId);
            }
        }
    }
}