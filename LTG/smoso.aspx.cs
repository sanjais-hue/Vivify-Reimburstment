using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class smoso : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadData();
        }
    }

    private void LoadData()
    {
        string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
        string query = @"
            WITH CombinedData AS (
                SELECT 
                    conv.SmoNo,
                    conv.SoNo,
                    SUM(ISNULL(conv.Amount, 0)) AS TotalAmount
                FROM 
                    Conveyance conv
                GROUP BY 
                    conv.SmoNo, conv.SoNo
                
                UNION ALL
                
                SELECT 
                    food.SmoNo,
                    food.SoNo,
                    SUM(ISNULL(food.Amount, 0)) AS TotalAmount
                FROM 
                    Food food
                GROUP BY 
                    food.SmoNo, food.SoNo
                
                UNION ALL
                
                SELECT 
                    lod.SmoNo,
                    lod.SoNo,
                    SUM(ISNULL(lod.Amount, 0)) AS TotalAmount
                FROM 
                    Lodging lod
                GROUP BY 
                    lod.SmoNo, lod.SoNo
                
                UNION ALL
                
                SELECT 
                    others.SmoNo,
                    others.SoNo,
                    SUM(ISNULL(others.Amount, 0)) AS TotalAmount
                FROM 
                    Others others
                GROUP BY 
                    others.SmoNo, others.SoNo
                
                UNION ALL
                
                SELECT 
                    misc.SmoNo,
                    misc.SoNo,
                    SUM(ISNULL(misc.Amount, 0)) AS TotalAmount
                FROM 
                    Miscellaneous misc
                GROUP BY 
                    misc.SmoNo, misc.SoNo
            )

            SELECT 
                COALESCE(SmoNo, '') + ' - ' + COALESCE(SoNo, '') AS CombinedNo,
                SUM(TotalAmount) AS TotalAmount
            FROM 
                CombinedData
            GROUP BY 
                SmoNo, SoNo
            HAVING 
                SUM(TotalAmount) > 0  -- Ensure no empty groups are included
            ORDER BY 
                SmoNo, SoNo;";

        DataTable dataTable = new DataTable();

        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dataTable);
                }
            }
        }

        gvExpenseReport.DataSource = dataTable;
        gvExpenseReport.DataBind();
    }

    protected void gvExpenseReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // Get the TotalAmount from the current row
            decimal totalAmount = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "TotalAmount"));

            // Maintain a running total in ViewState
            if (ViewState["OverallTotal"] == null)
            {
                ViewState["OverallTotal"] = 0m;
            }
            ViewState["OverallTotal"] = (decimal)ViewState["OverallTotal"] + totalAmount;
        }
    }

    protected void gvExpenseReport_PreRender(object sender, EventArgs e)
    {
        // Check if there are rows in the GridView
        if (gvExpenseReport.Rows.Count > 0)
        {
            // Create a footer row for the overall total
            GridViewRow footerRow = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell1 = new TableCell { Text = "Overall Total", ColumnSpan = 2, HorizontalAlign = HorizontalAlign.Right };
            TableCell cell2 = new TableCell { Text = ((decimal)ViewState["OverallTotal"]).ToString() }; // No formatting

            footerRow.Cells.Add(cell1);
            footerRow.Cells.Add(cell2);

            gvExpenseReport.Controls[0].Controls.Add(footerRow);
        }
    }
}
