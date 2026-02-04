using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class InBoundProcess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                bindBranch();
                bindCustomer();

                //GetContainer();
            }
        }

        private void bindBranch()
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "Select * from Branch";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        ddlBranch.DataSource = dt;
                        ddlBranch.DataBind();
                        ddlBranch.DataTextField = "BranchName";
                        ddlBranch.DataValueField = "BranchId";
                        ddlBranch.DataBind();
                        // ddlBranch.da
                    }
                }
            }
        }
        private void bindCustomer()
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "Select * from Customers";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        ddlCustomer.DataSource = dt;
                        ddlCustomer.DataBind();
                        ddlCustomer.DataTextField = "CustomerName";
                        ddlCustomer.DataValueField = "CustomerCode";
                        ddlCustomer.DataBind();
                        // ddlBranch.da
                    }
                }
            }
        }
        protected void btnCusNext_Click(object sender, ImageClickEventArgs e)
        {
            divCustomer.Visible = false;
            divScan.Visible = true;
            txtContainer1.Text = txtContainer.Text;
            getInboundFee();
        }
        private void getInboundFee()
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "Select InboundFee,Bin from Customers where CustomerCode='" + ddlCustomer.SelectedValue + "'";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        hdnInboundFee.Value = dt.Rows[0][0].ToString();
                        hdnBin.Value = dt.Rows[0][1].ToString();
                        // ddlBranch.da
                    }
                }
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                HiddenField hdLoginId = (HiddenField)this.Master.FindControl("hdnLoginId");

                var userid = hdLoginId.Value;
                HiddenField hdUserName = (HiddenField)this.Master.FindControl("hdnUserName");

                var userName = hdUserName.Value;
                string qry = "Insert into Inbound(ContainerId,BranchId,BranchName,CustomerCode,CustomerName,HU,Qty,UnitInBoundCost,TotalInBoundCost,Loginname,DateTimeofScan,CreatedBy,CreatedDate)values('" + txtContainer.Text+"',"+ ddlBranch.SelectedValue+",'" +ddlBranch.SelectedItem.Text +"','" + ddlCustomer.SelectedValue + "','" + ddlCustomer.SelectedItem.Text + "','" + txtHU.Text+ "','" + txtQty.Text + "',"+ hdnInboundFee.Value + "," + hdnInboundFee.Value + ",'" + userid + "',getdate(),'" + userName + "',getdate())";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                cmd1.ExecuteNonQuery();
                // qry = "Insert into WarehouseProcess(Bin,BranchId,BranchName,CustomerCode,CustomerName,HU,QtyIn,QtyOnHand,UnitStorageCost,TotalStorageCost,UserName,ScannedInTime,CreatedBy,CreatedDate)values('" + hdnBin.Value + "'," + ddlBranch.SelectedValue + ",'" + ddlBranch.SelectedItem.Text + "','" + ddlCustomer.SelectedValue + "','" + ddlCustomer.SelectedItem.Text + "','" + txtHU.Text + "','" + txtQty.Text + "','" + txtQty.Text + "'," + hdnInboundFee.Value + "," + hdnInboundFee.Value + ",'" + userid + "',getdate(),'" + userName + "',getdate())";
                // cmd1 = new SqlCommand(qry, con);
                //cmd1.ExecuteNonQuery();
            }
            txtHU.Text = "";
        }

        protected void btnComplete_Click(object sender, EventArgs e)
        {
            txtHU.Text = "";
            divCustomer.Visible = true;
            divScan.Visible = false;
            txtContainer.Text = "";

        }
        protected void GetContainer()
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "SELECT  isnull(max([ContainerId])+1,1)  FROM Inbound";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        hdnContainer.Value = dt.Rows[0][0].ToString();
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            txtHU.Text = "";
        }
    }
}