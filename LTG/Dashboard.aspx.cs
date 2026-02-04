using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Vivify
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGridView();
                BindGridView2();
            }
        }

        private void InsertServiceIdIntoChildTables(int serviceId)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // Fetch the advance value, defaulting to 0 if not provided
                string advanceQuery = "SELECT ISNULL(Advance, 0) FROM Services WHERE ServiceId = @ServiceId";
                decimal advanceValue = 0;

                using (SqlCommand cmd = new SqlCommand(advanceQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        advanceValue = Convert.ToDecimal(result);
                    }
                }

                string[] tables = { "Food", "Others", "Lodging", "Miscellaneous", "Conveyance" };

                foreach (var table in tables)
                {
                    InsertIntoTable(table, serviceId, advanceValue, con);
                }
            }
        }

        private void InsertIntoTable(string tableName, int serviceId, decimal advanceValue, SqlConnection con)
        {
            string insertQry = $@"
                INSERT INTO {tableName} (ServiceId, Advance) 
                VALUES (@ServiceId, @Advance)";

            using (SqlCommand cmd = new SqlCommand(insertQry, con))
            {
                cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                cmd.Parameters.AddWithValue("@Advance", advanceValue);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error inserting into {tableName}: {ex.Message}");
                }
            }
        }

        private void BindGridView()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            int employeeId = Convert.ToInt32(Session["EmployeeId"]);

            using (SqlConnection con = new SqlConnection(constr))
            {
                string serviceQuery = @"
            SELECT 
                ServiceId,
                CustomerId,
                ISNULL(Advance, 0) AS Advance,
                FromDate,
                ServiceType, 
                Status,
                StatusId
            FROM Services
            WHERE EmployeeId = @EmployeeId AND StatusId != 0
            ORDER BY ServiceId DESC";

                DataTable servicesTable = new DataTable();
                using (SqlCommand cmd = new SqlCommand(serviceQuery, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(servicesTable);
                    }
                }

                DataTable customersTable = new DataTable();
                using (SqlCommand cmd = new SqlCommand("SELECT CustomerId, CustomerName FROM Customers", con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(customersTable);
                    }
                }

                var customerLookup = new Dictionary<int, string>();
                foreach (DataRow row in customersTable.Rows)
                {
                    if (int.TryParse(row["CustomerId"].ToString(), out int id))
                    {
                        customerLookup[id] = row["CustomerName"].ToString();
                    }
                }

                DataTable resultTable = new DataTable();
                resultTable.Columns.Add("CustomerNames", typeof(string));
                resultTable.Columns.Add("Advance", typeof(decimal));
                resultTable.Columns.Add("FromDate", typeof(string));
                resultTable.Columns.Add("ServiceType", typeof(string));
                resultTable.Columns.Add("Status", typeof(string));
                resultTable.Columns.Add("StatusId", typeof(int));
                resultTable.Columns.Add("ServiceId", typeof(int));

                foreach (DataRow serviceRow in servicesTable.Rows)
                {
                    string customerIdList = serviceRow["CustomerId"].ToString();
                    var customerNames = new List<string>();

                    foreach (string idStr in customerIdList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(idStr.Trim(), out int customerId) && customerLookup.ContainsKey(customerId))
                        {
                            customerNames.Add(customerLookup[customerId]);
                        }
                    }

                    int statusId = Convert.ToInt32(serviceRow["StatusId"]);
                    string displayStatus;

                    // ✅ C# 7.3 compatible switch
                    switch (statusId)
                    {
                        case 1:
                            displayStatus = "Service Assigned";
                            break;
                        case 2:
                            displayStatus = "Reimbursement Submitted";
                            break;
                        case 3:
                            displayStatus = "Verified";
                            break;
                        default:
                            displayStatus = serviceRow["Status"].ToString();
                            break;
                    }

                    resultTable.Rows.Add(
                        string.Join(", ", customerNames),
                        serviceRow["Advance"],
                        serviceRow["FromDate"],
                        serviceRow["ServiceType"],
                        displayStatus,
                        statusId,
                        serviceRow["ServiceId"]
                    );
                }

                GridView1.DataSource = resultTable;
                GridView1.DataBind();
            }
        }
        private void BindGridView2()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            if (Session["EmployeeId"] != null)
            {
                int employeeId = Convert.ToInt32(Session["EmployeeId"]);
                string firstName = Session["EmployeeFirstName"] != null ? Session["EmployeeFirstName"].ToString() : "";

                using (SqlConnection con = new SqlConnection(constr))
                {
                    string qry = @"
                        SELECT 
                            e.EmployeeId,
                            e.FirstName,
                            e.Roles
                        FROM 
                            Employees e
                        WHERE 
                            e.EmployeeId = @EmployeeId AND
                            (e.Roles != 'Admin' OR e.Roles IS NULL);";

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            GridView2.DataSource = dt;
                            GridView2.DataBind();
                        }
                    }
                }

                if (GridView2.Rows.Count > 0)
                {
                    GridViewRow headerRow = GridView2.HeaderRow;
                    if (headerRow != null)
                    {
                        Label lblEmployeeId = (Label)headerRow.FindControl("lblEmployeeId");
                        Label lblFirstName = (Label)headerRow.FindControl("lblFirstName");

                        if (lblEmployeeId != null && lblFirstName != null)
                        {
                            lblEmployeeId.Text = employeeId.ToString();
                            lblFirstName.Text = firstName;
                        }
                    }
                }
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reimburse")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[rowIndex];

                HiddenField hdnServiceId = (HiddenField)row.FindControl("hdnServiceId");
                Label lblStatus = (Label)row.FindControl("lblStatus");

                if (hdnServiceId != null && int.TryParse(hdnServiceId.Value, out int serviceId))
                {
                    string currentStatus = lblStatus.Text;
                    decimal advanceValue = 0;
                    if (row.Cells[2].Text != null && decimal.TryParse(row.Cells[2].Text, out decimal advance))
                    {
                        advanceValue = advance;
                    }

                    if (currentStatus == "Service Assigned")
                    {
                        InsertServiceIdIntoChildTables(serviceId);
                        Session["ServiceId"] = serviceId;
                        Session["Advance"] = advanceValue;
                        Response.Redirect("Expenses.aspx");
                    }
                    else if (currentStatus == "Reimbursement Submitted")
                    {
                        Session["ServiceId"] = serviceId;
                        Session["Advance"] = advanceValue;
                        Response.Redirect("Expenses.aspx");
                    }
                    else if (currentStatus == "Verified") // ✅ Handle Verified click
                    {
                        Session["ServiceId"] = serviceId;
                        Session["Advance"] = advanceValue;
                        Session["IsReadOnly"] = true; // Tell Expenses.aspx to be read-only
                        Response.Redirect("Expenses.aspx");
                    }
                }
            }
            else if (e.CommandName == "DeleteRow")
            {
                int serviceId = Convert.ToInt32(e.CommandArgument);
                UpdateServiceStatus(serviceId, 0);
                BindGridView();

                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Service record deleted successfully.');", true);
            }
        }
        private void UpdateServiceStatus(int serviceId, int statusId)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string updateQuery = @"
            UPDATE Services 
            SET StatusId = @StatusId, 
                Status = CASE WHEN @StatusId = 0 THEN 'Deleted' ELSE Status END
            WHERE ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    cmd.Parameters.AddWithValue("@StatusId", statusId);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Remove this block completely or modify as below
                    /*if (rowsAffected > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Service record deleted successfully.');", true);
                    }*/
                }
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GridView1.SelectedRow != null)
            {
                GridViewRow row = GridView1.SelectedRow;
                string customerName = row.Cells[0].Text;
                string serviceId = row.Cells[2].Text;

                // Example action: Display the selected data in a label or log it
                // lblSelectedData.Text = $"Selected Customer: {customerName}, Service Type: {serviceType}" ;

                // Or perform other actions based on the selected data
            }
        }
        protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Your logic to handle the event when an item in GridView2 is selected
            // For example:
            var selectedRow = GridView2.SelectedRow;
            var employeeId = selectedRow.Cells[0].Text;  // Assuming EmployeeId is in the first column
                                                         // Do something with the selected employeeId, like redirect or show details
        }

     
        protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reimburse")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView2.Rows[rowIndex];

                Label lblEmployeeId = (Label)row.FindControl("lblEmployeeId");
                Label lblFirstName = (Label)row.FindControl("lblFirstName");

                if (lblEmployeeId != null && lblFirstName != null)
                {
                    int employeeId = Convert.ToInt32(lblEmployeeId.Text);
                    string firstName = lblFirstName.Text;

                    string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        string insertQuery = @"
                            INSERT INTO Refreshment (EmployeeId)
                            VALUES (@EmployeeId);";

                        using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                    Session["EmployeeId"] = employeeId;
                    Session["EmployeeFirstName"] = firstName;

                    BindGridView();

                    Response.Redirect("Refreshment.aspx");
                }
            }
        }
    }
}