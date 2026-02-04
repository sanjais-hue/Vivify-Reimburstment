using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vivify;

namespace Vivify
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            string connString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                WITH LocalTotal AS (
                    SELECT SUM(ISNULL(conv.Amount, 0) + ISNULL(food.Amount, 0) + ISNULL(others.Amount, 0) + ISNULL(misc.Amount, 0)) AS OverallLocalAmount
                    FROM Conveyance conv
                    LEFT JOIN Food food ON conv.ServiceId = food.ServiceId
                    LEFT JOIN Others others ON conv.ServiceId = others.ServiceId
                    LEFT JOIN Miscellaneous misc ON conv.ServiceId = misc.ServiceId
                    WHERE conv.ExpenseType = 'Local'
                ),
                TourTotal AS (
                    SELECT SUM(ISNULL(conv.Amount, 0) + ISNULL(food.Amount, 0) + ISNULL(lod.Amount, 0) + ISNULL(misc.Amount, 0)) AS OverallTourAmount
                    FROM Conveyance conv
                    LEFT JOIN Food food ON conv.ServiceId = food.ServiceId
                    LEFT JOIN Lodging lod ON conv.ServiceId = lod.ServiceId
                    LEFT JOIN Miscellaneous misc ON conv.ServiceId = misc.ServiceId
                    WHERE conv.ExpenseType = 'Tour'
                )
                
                SELECT 
                    lt.OverallLocalAmount,
                    NULL AS OverallTourAmount,
                    'Local' AS ExpenseType
                FROM 
                    LocalTotal lt

                UNION ALL

                SELECT 
                    NULL AS OverallLocalAmount,
                    tt.OverallTourAmount,
                    'Tour' AS ExpenseType
                FROM 
                    TourTotal tt
                WHERE 
                    tt.OverallTourAmount IS NOT NULL;", conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    if (dt.Rows.Count > 0)
                    {
                        gvReport.DataSource = dt;
                        gvReport.DataBind();
                    }
                    else
                    {
                        gvReport.DataSource = null;
                        gvReport.DataBind();
                        gvReport.Visible = true;
                        // lblMessage.Text = "No records found.";
                        // lblMessage.Visible = true;
                    }
                }
            }
            catch (SqlException )
            {
                //lblMessage.Text = "An error occurred: " + ex.Message;
                //lblMessage.Visible = true;
            }
        }



        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=ExpenseReport.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gvReport.RenderControl(hw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required for exporting to Excel
        }
    }
}