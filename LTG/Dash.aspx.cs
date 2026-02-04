using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class Dash : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGridView();
            }
        }

        private void BindGridView()
        {
            try
            {
                // Retrieve the connection string
                string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

                // Define the SQL query without the serial number
                string query = @"
                    SELECT
                        s.ServiceType,
                        s.ServiceId,
                        e.ExpenseId,
                        c.CustomerId,
                        emp.EmployeeId,
                        e.LocalAmount
                    FROM
                        Services s
                        INNER JOIN Expense e ON s.ServiceId = e.ServiceId
                        INNER JOIN Customers c ON s.CustomerId = c.CustomerId
                        INNER JOIN Employees emp ON s.EmployeeId = emp.EmployeeId";

                // Create and configure the SQL connection, command, and adapter
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            // Create and fill the DataTable
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            // Check if DataTable contains rows
                            if (dt.Rows.Count > 0)
                            {
                                // Bind Data to GridView
                                GridView1.DataSource = dt;
                                GridView1.DataBind();
                            }
                            else
                            {
                                // Optionally handle the case where no data is returned
                                GridView1.DataSource = null;
                                GridView1.DataBind();
                                // For debugging, uncomment the next line to show an empty message
                                // Response.Write("No data found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                // For debugging purposes, you can use Response.Write(ex.Message); but this is not recommended for production code
                Response.Write($"<div style='color: red;'>Error: {ex.Message}</div>");
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Implement your RowCommand logic here if needed
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Implement your SelectedIndexChanged logic here if needed
        }
    }
}
