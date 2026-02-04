using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class RefreshClaim : System.Web.UI.Page
    {
        string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["SelectedId"] != null)
            {
                LoadEmployeeData(Session["SelectedId"].ToString());
            }
        }

        private void LoadEmployeeData(string id)
        {
            string query = @"SELECT e.FirstName, r.FromDate, r.ToDate, r.RefreshAmount, r.Id
                             FROM Refreshment r
                             INNER JOIN Employees e ON r.EmployeeId = e.EmployeeId
                             WHERE r.Id = @Id";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditAmount")
            {
                GridViewRow row = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                TextBox txtAmount = (TextBox)row.FindControl("txtAmount");
                Button btnEdit = (Button)row.FindControl("btnEdit");

                // Toggle textbox editable state
                txtAmount.ReadOnly = !txtAmount.ReadOnly;
                btnEdit.Text = txtAmount.ReadOnly ? "Edit" : "Save";
            }
        }

        private void UpdateAmountAndVerify(string id, decimal amount)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Refreshment SET RefreshAmount = @Amount, IsVerified = 1 WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count > 0)
            {
                GridViewRow row = GridView1.Rows[0]; // only one row expected
                TextBox txtAmount = (TextBox)row.FindControl("txtAmount");

                if (decimal.TryParse(txtAmount.Text.Trim(), out decimal amount))
                {
                    string id = GridView1.DataKeys[0].Value.ToString();
                    UpdateAmountAndVerify(id, amount);

                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Amount claimed and verified successfully!');", true);
                    LoadEmployeeData(id); // refresh grid
                }
                else
                {
                    lblError.Text = "Invalid amount.";
                    lblError.Visible = true;
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Optional: reset changes or redirect
            Response.Redirect("RefreshmentVerify.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("RefreshmentVerify.aspx");
        }
    }
}
