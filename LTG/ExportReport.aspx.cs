using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class ExportReport : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // Check if it's the first time the page is loaded
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            string query = @"
       WITH LocalTotals AS (
           SELECT 
               SUM(ISNULL(conv.Amount, 0)) AS TotalConveyance,
               SUM(ISNULL(food.Amount, 0)) AS TotalFood,
               SUM(ISNULL(others.Amount, 0)) AS TotalOthers,
               SUM(ISNULL(misc.Amount, 0)) AS TotalMiscellaneous,
               SUM(ISNULL(refresh.RefreshAmount, 0)) AS TotalRefreshment,
               SUM(ISNULL(conv.Amount, 0) + ISNULL(food.Amount, 0) + ISNULL(others.Amount, 0) + ISNULL(misc.Amount, 0) + ISNULL(refresh.RefreshAmount, 0)) AS OverallLocalAmount
           FROM 
               Conveyance conv
           LEFT JOIN 
               Food food ON conv.ServiceId = food.ServiceId
           LEFT JOIN 
               Others others ON conv.ServiceId = others.ServiceId
           LEFT JOIN 
               Miscellaneous misc ON conv.ServiceId = misc.ServiceId
LEFT JOIN           
    Refreshment refresh ON 1 = 1
           WHERE 
               conv.ExpenseType = 'Local'
       ),
       TourTotals AS (
           SELECT 
               SUM(ISNULL(conv.Amount, 0)) AS TotalConveyance,
               SUM(ISNULL(food.Amount, 0)) AS TotalFood,
               NULL AS TotalOthers,
               SUM(ISNULL(misc.Amount, 0)) AS TotalMiscellaneous,
               SUM(ISNULL(lod.Amount, 0)) AS TotalLodging,
               SUM(ISNULL(conv.Amount, 0) + ISNULL(food.Amount, 0) + ISNULL(misc.Amount, 0) + ISNULL(lod.Amount, 0)) AS OverallTourAmount
           FROM 
               Conveyance conv
           LEFT JOIN 
               Food food ON conv.ServiceId = food.ServiceId
           LEFT JOIN 
               Lodging lod ON conv.ServiceId = lod.ServiceId
           LEFT JOIN 
               Miscellaneous misc ON conv.ServiceId = misc.ServiceId
           WHERE 
               conv.ExpenseType = 'Tour'
       ),
       SalesTotals AS (
           SELECT 
               SUM(ISNULL(conv.Amount, 0)) AS TotalConveyance,
               SUM(ISNULL(food.Amount, 0)) AS TotalFood,
               SUM(ISNULL(others.Amount, 0)) AS TotalOthers,
               SUM(ISNULL(misc.Amount, 0)) AS TotalMiscellaneous,
               SUM(ISNULL(lod.Amount, 0)) AS TotalLodging,
               SUM(ISNULL(conv.Amount, 0) + ISNULL(food.Amount, 0) + ISNULL(others.Amount, 0) + ISNULL(misc.Amount, 0) + ISNULL(lod.Amount, 0)) AS OverallSalesAmount
           FROM 
               Services s
           LEFT JOIN 
               Conveyance conv ON s.ServiceId = conv.ServiceId
           LEFT JOIN 
               Food food ON s.ServiceId = food.ServiceId
           LEFT JOIN 
               Others others ON s.ServiceId = others.ServiceId
           LEFT JOIN 
               Miscellaneous misc ON s.ServiceId = misc.ServiceId
           LEFT JOIN 
               Lodging lod ON s.ServiceId = lod.ServiceId
           WHERE 
               s.Department = 'Sales'
       ),
       ServiceTotals AS (
           SELECT 
               SUM(ISNULL(conv.Amount, 0)) AS TotalConveyance,
               SUM(ISNULL(food.Amount, 0)) AS TotalFood,
               SUM(ISNULL(others.Amount, 0)) AS TotalOthers,
               SUM(ISNULL(misc.Amount, 0)) AS TotalMiscellaneous,
               SUM(ISNULL(lod.Amount, 0)) AS TotalLodging,
               SUM(ISNULL(conv.Amount, 0) + ISNULL(food.Amount, 0) + ISNULL(others.Amount, 0) + ISNULL(misc.Amount, 0) + ISNULL(lod.Amount, 0)) AS OverallServicesAmount
           FROM 
               Services s
           LEFT JOIN 
               Conveyance conv ON s.ServiceId = conv.ServiceId
           LEFT JOIN 
               Food food ON s.ServiceId = food.ServiceId
           LEFT JOIN 
               Others others ON s.ServiceId = others.ServiceId
           LEFT JOIN 
               Miscellaneous misc ON s.ServiceId = misc.ServiceId
           LEFT JOIN 
               Lodging lod ON s.ServiceId = lod.ServiceId
           WHERE 
               s.Department = 'Services'
       )

       SELECT 
           'Local' AS ExpenseType,
           lt.TotalConveyance,
           lt.TotalFood,
           lt.TotalOthers,
           lt.TotalMiscellaneous,
           NULL AS TotalLodging,
           lt.TotalRefreshment,
           lt.OverallLocalAmount
       FROM 
           LocalTotals lt
       UNION ALL
       SELECT 
           'Tour' AS ExpenseType,
           tt.TotalConveyance,
           tt.TotalFood,
           NULL AS TotalOthers,
           tt.TotalMiscellaneous,
           tt.TotalLodging,
           NULL AS TotalRefreshment,
           tt.OverallTourAmount
       FROM 
           TourTotals tt
       UNION ALL
       SELECT 
           'Sales' AS ExpenseType,
           st.TotalConveyance,
           st.TotalFood,
           st.TotalOthers,
           st.TotalMiscellaneous,
           st.TotalLodging,
           NULL AS TotalRefreshment,
           (st.OverallSalesAmount + ISNULL((SELECT SUM(OverallServicesAmount) FROM ServiceTotals), 0)) AS OverallLocalAmount
       FROM 
           SalesTotals st
       UNION ALL
       SELECT 
           'Services' AS ExpenseType,
           sv.TotalConveyance,
           sv.TotalFood,
           sv.TotalOthers,
           sv.TotalMiscellaneous,
           sv.TotalLodging,
           NULL AS TotalRefreshment,
           (ISNULL((SELECT SUM(OverallSalesAmount) FROM SalesTotals), 0) + sv.OverallServicesAmount) AS OverallLocalAmount
       FROM 
           ServiceTotals sv
       UNION ALL
       SELECT 
           'Total' AS ExpenseType,
           SUM(TotalConveyance) AS TotalConveyance,
           SUM(TotalFood) AS TotalFood,
           SUM(TotalOthers) AS TotalOthers,
           SUM(TotalMiscellaneous) AS TotalMiscellaneous,
           SUM(TotalLodging) AS TotalLodging,
           SUM(TotalRefreshment) AS TotalRefreshment,
           SUM(OverallLocalAmount) AS OverallLocalAmount
       FROM (
           SELECT 
               lt.TotalConveyance,
               lt.TotalFood,
               lt.TotalOthers,
               lt.TotalMiscellaneous,
               NULL AS TotalLodging,
               lt.TotalRefreshment,
               lt.OverallLocalAmount
           FROM LocalTotals lt
           UNION ALL
           SELECT 
               tt.TotalConveyance,
               tt.TotalFood,
               NULL AS TotalOthers,
               tt.TotalMiscellaneous,
               tt.TotalLodging,
               NULL AS TotalRefreshment,
               tt.OverallTourAmount
           FROM TourTotals tt
           UNION ALL
           SELECT 
               st.TotalConveyance,
               st.TotalFood,
               st.TotalOthers,
               st.TotalMiscellaneous,
               st.TotalLodging,
               NULL AS TotalRefreshment,
               (st.OverallSalesAmount + ISNULL((SELECT SUM(OverallServicesAmount) FROM ServiceTotals), 0)) AS OverallLocalAmount
           FROM SalesTotals st
           UNION ALL
           SELECT 
               sv.TotalConveyance,
               sv.TotalFood,
               sv.TotalOthers,
               sv.TotalMiscellaneous,
               sv.TotalLodging,
               NULL AS TotalRefreshment,
               (ISNULL((SELECT SUM(OverallSalesAmount) FROM SalesTotals), 0) + sv.OverallServicesAmount) AS OverallLocalAmount
           FROM ServiceTotals sv
       ) AS AllTotals;";

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
            if (e.Row.Cells[0].Text == "Total")
            {
                e.Row.Visible = false; // Remove the extra total row
            }
        }
    }
}