using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class AdminCustomer_Creation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindBranch();
                LoadView();
            }
        }

        private void bindBranch()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "SELECT BranchId, BranchName FROM Branch";
                using (SqlCommand cmd = new SqlCommand(qry, con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        ddlBranch.DataSource = dt;
                        ddlBranch.DataTextField = "BranchName";
                        ddlBranch.DataValueField = "BranchId";
                        ddlBranch.DataBind();
                    }
                }

                // Add a default item at the top
                ddlBranch.Items.Insert(0, new ListItem("Select Branch", "0"));
            }
        }

        protected void btnCustomerCreate_Click(object sender, EventArgs e)
        {
            save();
        }

        private void save()
        {
            if (ddlBranch.SelectedValue == "0")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Please select a branch.');", true);
                return;
            }

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // Check if the customer already exists in the selected branch
                string checkCustomerQuery = @"
                    SELECT COUNT(*) 
                    FROM Customers 
                    WHERE CustomerName = @CustomerName AND BranchId = @BranchId";

                using (SqlCommand cmd = new SqlCommand(checkCustomerQuery, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text.Trim());
                    cmd.Parameters.AddWithValue("@BranchId", ddlBranch.SelectedValue);

                    int existingCustomerCount = (int)cmd.ExecuteScalar();

                    if (existingCustomerCount > 0)
                    {
                        // Customer already exists for this branch, show an alert
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Customer already exists for the selected branch.');", true);
                        return;
                    }
                }

                // Retrieve the CreatedBy value from the cookie
                HttpCookie firstNameCookie = Request.Cookies["FirstName"];
                string createdBy = firstNameCookie != null ? firstNameCookie.Value : "Unknown";

                // Insert the new customer
                string insertCustomerQuery = @"
                    INSERT INTO Customers (CustomerName, BranchId, Address1, Active, CreatedDate, CreatedBy) 
                    VALUES (@CustomerName, @BranchId, @Address1, 1, GETDATE(), @CreatedBy)";

                using (SqlCommand insertCmd = new SqlCommand(insertCustomerQuery, con))
                {
                    insertCmd.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@BranchId", ddlBranch.SelectedValue);
                    insertCmd.Parameters.AddWithValue("@Address1", txtAddress.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    insertCmd.ExecuteNonQuery();

                    // Show success alert
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                        "alert('Customer created successfully.');", true);

                    // Clear the form fields after successful insert
                    txtCustomerName.Text = string.Empty;
                    txtAddress.Text = string.Empty;
                    ddlBranch.SelectedIndex = 0;

                    // Reload the GridView to display the updated data
                    LoadView();
                }
            }
        }

        private void LoadView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT CustomerId, CustomerName, Address1, BranchId FROM Customers";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind the data to the GridView
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }
    }
}