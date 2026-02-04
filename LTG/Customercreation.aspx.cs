using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class Customercreation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Bind branches to the dropdown list
                bindBranch();

                // If there are any branches in the dropdown list, load customers for the first branch
                if (ddlBranch.Items.Count > 0)
                {
                    int branchId = Convert.ToInt32(ddlBranch.SelectedValue); // Get the first branch's ID
                    LoadView(branchId); // Load customers for that branch
                }
            }
        }

        private void bindBranch()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // Assuming you store the employee's ID or other unique identifier in a session (e.g., employee ID)
                int employeeId = Convert.ToInt32(Session["EmployeeId"]); // Replace with your session key
                string qry = "SELECT BranchId, BranchName FROM Branch WHERE BranchId = (SELECT BranchId FROM Employees WHERE EmployeeId = @EmployeeId)";

                SqlCommand cmd1 = new SqlCommand(qry, con);
                cmd1.Parameters.AddWithValue("@EmployeeId", employeeId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Check if the query returned any branch for the employee
                    if (dt.Rows.Count > 0)
                    {
                        ddlBranch.DataSource = dt;
                        ddlBranch.DataTextField = "BranchName";
                        ddlBranch.DataValueField = "BranchId";
                        ddlBranch.DataBind();

                        // Optionally, set the default selected index if needed
                        ddlBranch.SelectedIndex = 0;
                    }
                }
            }
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected branch ID from the DropDownList
            int branchId = Convert.ToInt32(ddlBranch.SelectedValue);

            // Call LoadView to filter customers by the selected branch
            LoadView(branchId);
        }

        protected void btnCustomerCreate_Click(object sender, EventArgs e)
        {
            save();
        }

        private void save()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // Check if the customer already exists in the selected branch
                string checkCustomerQuery = "SELECT COUNT(*) FROM Customers WHERE CustomerName = @CustomerName AND BranchId = @BranchId";

                using (SqlCommand cmd = new SqlCommand(checkCustomerQuery, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
                    cmd.Parameters.AddWithValue("@BranchId", ddlBranch.SelectedValue);

                    int existingCustomerCount = (int)cmd.ExecuteScalar();

                    if (existingCustomerCount > 0)
                    {
                        // Customer already exists for this branch, show an alert
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showalert", "alert('Customer already exists for the selected branch.');", true);
                    }
                    else
                    {
                        // Retrieve the CreatedBy value from the cookie
                        HttpCookie firstNameCookie = Request.Cookies["FirstName"];
                        string createdBy = firstNameCookie != null ? firstNameCookie.Value : "Unknown";

                        // Insert the new customer
                        string insertCustomerQuery = @"
                        INSERT INTO Customers (CustomerName, BranchId, Address1, Active, CreatedDate, CreatedBy) 
                        VALUES (@CustomerName, @BranchId, @Address1, 1, GETDATE(), @CreatedBy)";

                        using (SqlCommand insertCmd = new SqlCommand(insertCustomerQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
                            insertCmd.Parameters.AddWithValue("@BranchId", ddlBranch.SelectedValue);
                            insertCmd.Parameters.AddWithValue("@Address1", txtAddress.Text);
                            insertCmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                            insertCmd.ExecuteNonQuery();

                            // Show success alert
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "showalert", "alert('Customer Created Successfully.');", true);

                            // Clear the form fields after successful insert
                            txtCustomerName.Text = "";
                            txtAddress.Text = "";

                            // Reload the GridView to display the new record
                            int branchId = Convert.ToInt32(ddlBranch.SelectedValue);
                            LoadView(branchId);
                        }
                    }
                }
            }
        }

        private void LoadView(int branchId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT CustomerId, CustomerName, Address1 FROM Customers WHERE BranchId = @BranchId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BranchId", branchId); // Pass the branchId as a parameter to the query

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
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