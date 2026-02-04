using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

public partial class ExpenseReport : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) // Load data only when the page is first loaded
        {
            LoadExpenseData();
        }
    }

    private void LoadExpenseData()
    {
        string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
        string query = @"
    SELECT 
        e.FirstName,
        SUM(COALESCE(exp.LocalAmount, 0)) AS TotalLocal,
        SUM(COALESCE(exp.TourAmount, 0)) AS TotalTour,
        SUM(COALESCE(exp.LocalAmount, 0) + COALESCE(exp.TourAmount, 0)) AS OverallAmount
    FROM 
        Employees e
    LEFT JOIN 
        Expense exp ON e.EmployeeId = exp.EmployeeId
    GROUP BY 
        e.FirstName
    ORDER BY 
        e.FirstName;
    ";

        DataTable dataTable = new DataTable();

        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open(); // Ensure the connection is open
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dataTable);
                }
            }
        }

        // Calculate overall totals
        decimal totalLocal = 0;
        decimal totalTour = 0;
        decimal overallAmount = 0;

        foreach (DataRow row in dataTable.Rows)
        {
            totalLocal += Convert.ToDecimal(row["TotalLocal"]);
            totalTour += Convert.ToDecimal(row["TotalTour"]);
            overallAmount += Convert.ToDecimal(row["OverallAmount"]);
        }

        // Add a new row for the totals
        DataRow totalRow = dataTable.NewRow();
        totalRow["FirstName"] = "Total"; // Label for the total row
        totalRow["TotalLocal"] = totalLocal;
        totalRow["TotalTour"] = totalTour;
        totalRow["OverallAmount"] = overallAmount;

        dataTable.Rows.Add(totalRow);

        gvExpenseReport.DataSource = dataTable;
        gvExpenseReport.DataBind();
    }
}
