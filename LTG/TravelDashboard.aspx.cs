using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class TravelDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int employeeId = GetEmployeeIdFromCookies();
                if (employeeId == -1)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                // Bind employee grid on first load
                BindEmployeeGrid();
            }
        }
     
        private void BindEmployeeGrid()
        {
            int employeeId = GetEmployeeIdFromCookies();
            if (employeeId == -1)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            // Step 1: Get employee name first (to show even if no travel records)
            string empName = GetEmployeeName(employeeId, connectionString);
            if (string.IsNullOrEmpty(empName))
            {
                // Employee doesn't exist or is inactive
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Step 2: Fetch travel expenses (if any)
            string query = @"
        SELECT 
            e.EmployeeId,
            e.FirstName,
            te.Date,
            MAX(te.Status) AS StatusId  -- ✅ Use StatusId (int), not Status (string)
        FROM Employees e
        LEFT JOIN TravelExpenses te ON e.EmployeeId = te.EmployeeId
        WHERE e.EmployeeId = @EmployeeId 
          AND e.Active = 1
        GROUP BY e.EmployeeId, e.FirstName, te.Date
        ORDER BY te.Date DESC";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            // Step 3: If no travel records, add a placeholder row
            if (dt.Rows.Count == 0)
            {
                dt.Columns.Add("EmployeeId", typeof(int));
                dt.Columns.Add("FirstName", typeof(string));
                dt.Columns.Add("Date", typeof(DateTime?)); // nullable
                dt.Columns.Add("StatusId", typeof(int));

                DataRow placeholder = dt.NewRow();
                placeholder["EmployeeId"] = employeeId;
                placeholder["FirstName"] = empName;
                placeholder["Date"] = DBNull.Value; // indicates no travel date
                placeholder["StatusId"] = 0; // not verified
                dt.Rows.Add(placeholder);
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        // Helper method to get employee name
        private string GetEmployeeName(int employeeId, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT FirstName FROM Employees WHERE EmployeeId = @Id AND Active = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", employeeId);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? string.Empty;
                }
            }
        }
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AssignTravel")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int employeeId = Convert.ToInt32(args[0]);
                DateTime? expenseDate = null;

                if (args.Length > 1 && !string.IsNullOrEmpty(args[1]) && args[1] != "null")
                {
                    if (DateTime.TryParse(args[1], out DateTime dt))
                    {
                        expenseDate = dt;
                    }
                }

                // ✅ Store in Session — NO URL parameters
                Session["SelectedEmployeeId"] = employeeId;
                if (expenseDate.HasValue)
                {
                    Session["SelectedExpenseDate"] = expenseDate.Value;
                }

                Response.Redirect("TravelExpense.aspx");
            }
        }
        protected void btnNewTravel_Click(object sender, EventArgs e)
        {
            Response.Redirect("TravelExpense.aspx?new=true");
        }
        private int GetEmployeeIdFromCookies()
        {
            HttpCookie employeeIdCookie = Request.Cookies["EmployeeId"];
            if (employeeIdCookie != null && int.TryParse(employeeIdCookie.Value, out int employeeId))
            {
                return employeeId;
            }
            return -1;
        }
    }
}