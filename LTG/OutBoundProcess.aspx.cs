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
    public partial class OutBoundProcess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                bindBranch();
                bindCustomer();
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
        private void getOutboundFee()
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "Select OutboundFee,Bin from Customers where CustomerCode='" + ddlCustomer.SelectedValue + "'";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        hdnInboundFee.Value = dt.Rows[0][0].ToString();
                    }
                }
            }
        }
        protected void btnCusNext_Click(object sender, ImageClickEventArgs e)
        {
            divCustomer.Visible = false;
            divScan.Visible = true;
            getOutboundFee();
        }
        private bool checkBin()
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "Select * from Bin where BinName='" + txtBin.Text + "'";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        return true;
                        // ddlBranch.da
                    }
                }
            }
            return false;
        }
        private int IsHUAvailable()
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "Select * from WarehouseProcess where Bin='" + txtBin.Text + "' and HU='" + txtHU.Text + "'";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["QtyOnHand"].ToString() == "1")
                            return 0;
                        else
                            return 2;
                        // ddlBranch.da
                    }
                }
            }
            return 1;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (checkBin())
            {

            }
            else
            {
                string script = "alert(\"Bin is not available\");";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
                txtBin.Text = "";
                return;
            }
            var HUAvailability = IsHUAvailable();
            if(HUAvailability ==2)
            {
                string script = "alert(\"HU is already Outbounded.\");";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
                txtHU.Text = "";
                return;
            }
            if (HUAvailability == 1)
            {
                string script = "alert(\"HU is not available for the bin!\");";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
                txtHU.Text = "";
                return;
            }
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                HiddenField hdLoginId = (HiddenField)this.Master.FindControl("hdnLoginId");

                var userid = hdLoginId.Value;
                HiddenField hdUserName = (HiddenField)this.Master.FindControl("hdnUserName");

                var userName = hdUserName.Value;
                string qry = "Update WarehouseProcess set QtyOut=1,QtyOnHand=0,ScannedOutTime=getdate(),ModifiedBy='" + userName + "',ModifiedDate=getdate() where HU='" +txtHU.Text+"' and Bin='" + txtBin.Text +"'";
                            SqlCommand cmd1 = new SqlCommand(qry, con);
                cmd1.ExecuteNonQuery();
                qry = "Insert into Outbound(ContainerId,BranchId,BranchName,CustomerCode,CustomerName,HU,Qty,UnitOutBoundCost,TotalOutBoundCost,Loginname,DateTimeofScan,CreatedBy,CreatedDate,BinName)values('" + txtContainer.Text + "'," + ddlBranch.SelectedValue + ",'" + ddlBranch.SelectedItem.Text + "','" + ddlCustomer.SelectedValue + "','" + ddlCustomer.SelectedItem.Text + "','" + txtHU.Text + "','" + txtQty.Text + "'," + hdnInboundFee.Value + "," + hdnInboundFee.Value + ",'" + userid + "',getdate(),'" + userName + "',getdate(),'" + txtBin.Text + "')";
                 cmd1 = new SqlCommand(qry, con);
                cmd1.ExecuteNonQuery();
                // qry = "Insert into WarehouseProcess(Bin,BranchId,BranchName,CustomerCode,CustomerName,HU,QtyIn,QtyOnHand,UnitStorageCost,TotalStorageCost,UserName,ScannedInTime,CreatedBy,CreatedDate)values('" + hdnBin.Value + "'," + ddlBranch.SelectedValue + ",'" + ddlBranch.SelectedItem.Text + "','" + ddlCustomer.SelectedValue + "','" + ddlCustomer.SelectedItem.Text + "','" + txtHU.Text + "','" + txtQty.Text + "','" + txtQty.Text + "'," + hdnInboundFee.Value + "," + hdnInboundFee.Value + ",'" + userid + "',getdate(),'" + userName + "',getdate())";
                // cmd1 = new SqlCommand(qry, con);
                //cmd1.ExecuteNonQuery();
            }
            txtHU.Text = "";
            txtBin.Text = "";
        }

        protected void btnComplete_Click(object sender, EventArgs e)
        {
            txtHU.Text = "";
            divCustomer.Visible = true;
            txtBin.Text = "";
            divScan.Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            txtHU.Text = "";
        }
    }
}