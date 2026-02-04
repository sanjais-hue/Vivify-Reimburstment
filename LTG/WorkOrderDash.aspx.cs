using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class WorkOrderdash : Page
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
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string qry = @"
                    SELECT 
                        c.CustomerName, 
                        e.EmployeeId,
                        e.FirstName,
                        s.Advance, 
                        s.FromDate,
                        s.ServiceType,
                        s.Status,
                        s.ServiceId,
                        cv.ExpenseType, -- From Expenses table
                        cv.SmoNo -- From Conveyance table
                    FROM 
                        Customers c
                    INNER JOIN 
                        Services s ON c.CustomerId = s.CustomerId
                    INNER JOIN 
                        Employees e ON s.EmployeeId = e.EmployeeId
                    LEFT JOIN 
                        Expense ex ON s.ServiceId = ex.ServiceId -- Join with Expenses
                    LEFT JOIN 
                        Conveyance cv ON s.ServiceId = cv.ServiceId -- Join with Conveyance
                    ORDER BY 
                        s.ServiceId DESC";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
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

                // Get the HiddenFields for ServiceId, ServiceType, ExpenseType, and SmoNo
                HiddenField hdnServiceId = (HiddenField)row.FindControl("hdnServiceId");
                HiddenField hdnServiceType = (HiddenField)row.FindControl("hdnServiceType");
                HiddenField hdnExpenseType = (HiddenField)row.FindControl("hdnExpenseType");
                HiddenField hdnSmoNo = (HiddenField)row.FindControl("hdnSmoNo");

                // Get the FirstName directly from the GridView row
                Label lblFirstName = (Label)row.FindControl("lblFirstName");

                if (hdnServiceId != null && hdnServiceType != null && hdnExpenseType != null && hdnSmoNo != null &&
                    lblFirstName != null &&
                    int.TryParse(hdnServiceId.Value, out int serviceId))
                {
                    // Store values in session
                    Session["ServiceId"] = serviceId;
                    Session["EmployeeFirstName"] = lblFirstName.Text;
                    Session["ServiceType"] = hdnServiceType.Value;
                    Session["ExpenseType"] = hdnExpenseType.Value;
                    Session["SmoNo"] = hdnSmoNo.Value;

                    // Redirect to the WorkOrder page
                    Response.Redirect("WorkOrder.aspx"); // Adjust the URL if necessary
                }
            }
        }
    }
}
