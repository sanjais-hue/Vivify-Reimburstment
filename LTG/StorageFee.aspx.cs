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
    public partial class StorageFee : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindBranch();
                bindBin();
                FillData();
            }
        }
        private void bindBin()
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "Select * from Bin";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        ddlBin.DataSource = dt;
                        ddlBin.DataBind();
                        ddlBin.DataTextField = "BinName";
                        ddlBin.DataValueField = "BinId";
                        ddlBin.DataBind();
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
        private void FillData()
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "Select Fee from FeeMaster Where FeeType='StorageFee' and BranchId=" + ddlBranch.SelectedValue +" order by FeeId desc";
                SqlCommand cmd1 = new SqlCommand(qry, con);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        txtExistingFee.Text = dt.Rows[0]["Fee"].ToString();
                        // ddlBranch.da
                    }
                    else
                    {
                        txtExistingFee.Text = "0";
                    }
                }
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["LTGConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string qry = "";
                HiddenField hdLoginId = (HiddenField)this.Master.FindControl("hdnLoginId");

                var userid = hdLoginId.Value;
                HiddenField hdUserName = (HiddenField)this.Master.FindControl("hdnUserName");

                var userName = hdUserName.Value;

                qry = "Insert into FeeMaster(BranchId,BranchName,FeeType,Fee,Active,ValidFrom,CreatedBy,CreatedDate,Bin)values('" + ddlBranch.SelectedValue + "','" + ddlBranch.SelectedItem.Text + "','StorageFee','" + txtNewFee.Text + "',1,getdate(),'" + userName + "',getdate(),'" + ddlBin.SelectedValue + "')";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {



                    cmd.ExecuteNonQuery();
                    qry = "Update Customers set StorageFee='" + txtNewFee.Text + "',PreviousStorageFee='" + txtExistingFee.Text + "' where BranchId=" + ddlBranch.SelectedValue;
                    cmd.CommandText = qry;
                    cmd.ExecuteNonQuery();
                    qry = "Update WarehouseProcess set UnitStorageCost='" + txtNewFee.Text + "',TotalStorageCost='" + txtNewFee.Text + "' where BranchId=" + ddlBranch.SelectedValue +" and QtyOnHand>0";
                    cmd.CommandText = qry;
                    cmd.ExecuteNonQuery();
                    qry = "Insert into AuditLogs([Table],Field,ExistingValue,NewValue,ModifiedByUserId,ModifiedByUserName,ModifiedByDate,BranchName,BranchId,Custom)Values('FeeMaster','Fee','" + txtExistingFee.Text + "','" + txtNewFee.Text + "'," + userid + ",'" + userName + "',getdate(),'" + ddlBranch.SelectedItem.Text + "','" + ddlBranch.SelectedValue + "','Update the StorageFee Fee')";
                    cmd.CommandText = qry;
                    cmd.ExecuteNonQuery();
                    qry = "Insert into AuditLogs([Table],Field,ExistingValue,NewValue,ModifiedByUserId,ModifiedByUserName,ModifiedByDate,BranchName,BranchId,Custom)Values('Customers','StorageFee','" + txtExistingFee.Text + "','" + txtNewFee.Text + "'," + userid + ",'" + userName + "',getdate(),'" + ddlBranch.SelectedItem.Text + "','" + ddlBranch.SelectedValue + "','Update the StorageFee Fee for Customer')";
                    cmd.CommandText = qry;
                    cmd.ExecuteNonQuery();
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "showalert", "alert('Storage Fee Successfully Updated.');", true);
                    txtNewFee.Text = "";
                    FillData();
                }
            }
        }
    }
}