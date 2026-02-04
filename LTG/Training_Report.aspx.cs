using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Linq;

namespace Vivify
{
    public partial class Training_Report : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateBranches();
                PopulateEmployees();
            }
        }

        private void PopulateBranches()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT BranchId, BranchName FROM Branch", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                ddlBranch.DataSource = reader;
                ddlBranch.DataTextField = "BranchName";
                ddlBranch.DataValueField = "BranchId";
                ddlBranch.DataBind();
            }
            ddlBranch.Items.Insert(0, new ListItem("Select Branch", "0"));
        }

        private void PopulateEmployees(string branchId = "0")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (branchId == "0") // Load all employees if no branch is selected
                {
                    cmd = new SqlCommand("SELECT EmployeeId, FirstName FROM Employees", conn);
                }
                else // Load employees specific to the selected branch
                {
                    cmd = new SqlCommand("SELECT EmployeeId, FirstName FROM Employees WHERE BranchId = @BranchId", conn);
                    cmd.Parameters.AddWithValue("@BranchId", branchId);
                }

                SqlDataReader reader = cmd.ExecuteReader();
                ddlEmployeeName.DataSource = reader;
                ddlEmployeeName.DataTextField = "FirstName";
                ddlEmployeeName.DataValueField = "EmployeeId";
                ddlEmployeeName.DataBind();
            }
            ddlEmployeeName.Items.Insert(0, new ListItem("All Employees", "0"));
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBranch = ddlBranch.SelectedValue;
            if (selectedBranch != "0")
            {
                PopulateEmployees(selectedBranch);
            }
            else
            {
                PopulateEmployees(); // Load all employees if no branch is selected
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime fromDate, toDate;
            string selectedBranch = ddlBranch.SelectedValue;
            string selectedEmployee = ddlEmployeeName.SelectedValue;

            if (selectedBranch == "0")
            {
                lblError.Text = "Please select a valid branch.";
                lblError.Visible = true;
                return;
            }

            if (DateTime.TryParse(txtFromDate.Text, out fromDate) && DateTime.TryParse(txtToDate.Text, out toDate))
            {
                if (fromDate <= toDate)
                {
                    DataTable dt = LoadData(selectedBranch, selectedEmployee, fromDate, toDate);

                    if (dt.Rows.Count > 0)
                    {
                        gvReport.DataSource = dt;
                        gvReport.DataBind();
                        lblError.Visible = false;
                    }
                    else
                    {
                        gvReport.DataSource = null;
                        gvReport.DataBind();
                        lblError.Text = "No data found for the selected filters.";
                        lblError.Visible = true;
                    }
                }
                else
                {
                    lblError.Text = "From Date cannot be later than To Date.";
                    lblError.Visible = true;
                }
            }
            else
            {
                lblError.Text = "Please enter valid dates.";
                lblError.Visible = true;
            }
        }

        private DataTable LoadData(string branchId, string employeeId, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    t.Training_Id,
                    t.FirstName AS EngineerName,
                    t.FromDate,
                    t.ToDate,
                    t.Training_Details AS TrainingDetails,
                    ct.FromTime AS FromTime,
                    ct.ToTime AS ToTime,
                    ct.Particulars AS Particulars,
                    ISNULL(ct.Verified_Amount, 0) AS ConveyanceAmount,
                    NULL AS FoodAmount,
                    ISNULL(ct.Distance, 0) AS Distance,
                    ISNULL(ct.TransportType, 0) AS ModeOfTransport,
                    'Conveyance' AS ExpenseType,
                    b.BranchName
                FROM 
                    Training t
                JOIN 
                    Employees e ON t.EmployeeId = e.EmployeeId
                LEFT JOIN 
                    Conv_Training ct ON t.Training_Id = ct.Training_Id
                JOIN 
                    Branch b ON t.BranchId = b.BranchId
                WHERE 
                    t.FromDate >= @FromDate AND t.ToDate <= @ToDate
                    AND b.BranchId = @BranchId
                    " + (employeeId != "0" ? "AND e.EmployeeId = @EmployeeId" : "") + @"

                UNION ALL

                SELECT 
                    t.Training_Id,
                    t.FirstName AS EngineerName,
                    t.FromDate,
                    t.ToDate,
                    t.Training_Details AS TrainingDetails,
                    ft.FromTime AS FromTime,
                    ft.ToTime AS ToTime,
                    ft.Particulars AS Particulars,
                    NULL AS ConveyanceAmount,
                    ISNULL(ft.Verified_Amount, 0) AS FoodAmount,
                    NULL AS Distance,
                    NULL AS ModeOfTransport,
                    'Food' AS ExpenseType,
                    b.BranchName
                FROM 
                    Training t
                JOIN 
                    Employees e ON t.EmployeeId = e.EmployeeId
                LEFT JOIN 
                    Food_Training ft ON t.Training_Id = ft.Training_Id
                JOIN 
                    Branch b ON t.BranchId = b.BranchId
                WHERE 
                    t.FromDate >= @FromDate AND t.ToDate <= @ToDate
                    AND b.BranchId = @BranchId
                    " + (employeeId != "0" ? "AND e.EmployeeId = @EmployeeId" : "") + @"

                ORDER BY 
                    EngineerName, FromDate;
            ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandTimeout = 120;
                    cmd.Parameters.AddWithValue("@BranchId", branchId);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    if (employeeId != "0")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    }

                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            // Process and calculate totals
            DataTable dtSeparate = dt.Clone();
            dtSeparate.Columns.Add("Total", typeof(decimal));

            foreach (DataRow row in dt.Rows)
            {
                decimal conveyanceAmount = row["ConveyanceAmount"] != DBNull.Value ? Convert.ToDecimal(row["ConveyanceAmount"]) : 0;
                decimal foodAmount = row["FoodAmount"] != DBNull.Value ? Convert.ToDecimal(row["FoodAmount"]) : 0;
                decimal totalAmount = conveyanceAmount + foodAmount;

                if (conveyanceAmount > 0)
                {
                    DataRow convRow = dtSeparate.NewRow();
                    convRow.ItemArray = row.ItemArray.Clone() as object[];
                    convRow["Total"] = totalAmount;
                    dtSeparate.Rows.Add(convRow);
                }

                if (foodAmount > 0)
                {
                    DataRow foodRow = dtSeparate.NewRow();
                    foodRow.ItemArray = row.ItemArray.Clone() as object[];
                    foodRow["Total"] = totalAmount;
                    dtSeparate.Rows.Add(foodRow);
                }
            }

            DataRow totalRow = dtSeparate.NewRow();
            totalRow["EngineerName"] = "Total";
            totalRow["ConveyanceAmount"] = dtSeparate.AsEnumerable().Sum(r => r.Field<decimal?>("ConveyanceAmount") ?? 0);
            totalRow["FoodAmount"] = dtSeparate.AsEnumerable().Sum(r => r.Field<decimal?>("FoodAmount") ?? 0);
            totalRow["Total"] = dtSeparate.AsEnumerable().Sum(r => r.Field<decimal?>("Total") ?? 0);
            dtSeparate.Rows.Add(totalRow);

            return dtSeparate;
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            ExportToExcel(gvReport);
        }

        private void ExportToExcel(GridView gridView)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Training_Report.xls");
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            gridView.AllowPaging = false;
            gridView.RenderControl(hw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Format FromTime and ToTime if needed
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }
    }
}
