using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class Training_Verify : System.Web.UI.Page
    {
        private const string ConnectionStringName = "vivify";
        private const string TrainingidSessionKey = "trainingid";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if the trainingid is available in the query string
                if (Request.QueryString["trainingid"] != null)
                {
                    int trainingid = Convert.ToInt32(Request.QueryString["trainingid"]);
                    Session[TrainingidSessionKey] = trainingid; // Store it in the session
                }

                // Retrieve the training ID from the session
                if (Session[TrainingidSessionKey] != null)
                {
                    int trainingid = Convert.ToInt32(Session[TrainingidSessionKey]);
                    BindTrainingGridView(trainingid);  // Ensure you're passing the valid session ID
                }
                else
                {
                    lblMessage.Text = "Training ID is missing from the session.";
                    lblMessage.Visible = true;
                }

                // Check if the form has been submitted, if so, disable the edit/save buttons
                if (Session["IsFormSubmitted"] != null && (bool)Session["IsFormSubmitted"])
                {
                    DisableEditAndSaveButtons();
                }
            }
        }

        private void DisableEditAndSaveButtons()
        {
            foreach (GridViewRow row in TrainingGridView.Rows)
            {
                // Find the Edit and Save buttons in the row
                Button btnEdit = (Button)row.FindControl("btnEdit");
                Button btnSave = (Button)row.FindControl("btnSave");

                // Disable the buttons
                if (btnEdit != null) btnEdit.Enabled = false;
                if (btnSave != null) btnSave.Enabled = false;
            }
        }

        private void BindTrainingGridView(int trainingid)
        {
            // Check if the Training_Id exists in the database
            if (!IsTrainingIdValid(trainingid))
            {
                lblMessage.Text = "The specified Training ID does not exist.";
                lblMessage.Visible = true;
                return;
            }

            // If the Training_Id is valid, proceed to fetch data
            DataTable dt = GetTrainingData(trainingid);

            if (dt != null && dt.Rows.Count > 0)
            {
                // Bind the data to the GridView
                TrainingGridView.DataSource = dt;
                TrainingGridView.DataBind();
                lblMessage.Visible = false;  // Hide any error message if data is found
            }
            else
            {
                lblMessage.Text = "No data found for the specified training ID.";
                lblMessage.Visible = true;
            }
        }

        private bool IsTrainingIdValid(int trainingid)
        {
            bool exists = false;
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string qry = "SELECT COUNT(1) FROM conv_Training WHERE Training_Id = @Training_Id";
                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@Training_Id", trainingid);
                    con.Open();
                    exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            return exists;
        }

        private DataTable GetTrainingData(int trainingid)
        {
            DataTable dt = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                // SQL query to fetch training data
                string qry = @"
                    SELECT 
                        t.Training_Id AS TrainingId,  
                        'Conveyance' AS Source,
                        FORMAT(t.FromDate, 'dd/MMM/yyyy') AS Date,
                        t.Particulars,
                        t.Distance,     
                        t.Amount,
                        t.ID -- Assuming there's an ID field
                    FROM conv_Training t
                    WHERE t.Training_Id = @Training_Id

                    UNION ALL

                    SELECT 
                        f.Training_Id AS TrainingId,  
                        'Food' AS Source,
                        FORMAT(f.FromDate, 'dd/MMM/yyyy') AS Date,
                        NULL AS Particulars,
                        NULL AS Distance,
                        f.Amount,
                        f.ID -- Assuming there's an ID field
                    FROM Food_Training f
                    WHERE f.Training_Id = @Training_Id

                    ORDER BY Date DESC";

                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.Parameters.AddWithValue("@Training_Id", trainingid);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Button btnSave = (Button)sender;
            GridViewRow row = (GridViewRow)btnSave.NamingContainer;

            // Retrieve the updated amount from the TextBox
            TextBox txtAmount = (TextBox)row.FindControl("txtAmount");
            decimal updatedAmount;

            if (!decimal.TryParse(txtAmount.Text, out updatedAmount))
            {
                lblMessage.Text = "Please enter a valid amount.";
                lblMessage.Visible = true;
                return;
            }

            int trainingId = Convert.ToInt32(((Label)row.FindControl("lblTrainingId")).Text);
            int id = Convert.ToInt32(((Label)row.FindControl("lblId")).Text);
            string source = ((Label)row.FindControl("lblSource")).Text;

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string updateQuery = source == "Conveyance" ?
                        "UPDATE conv_Training SET Verified_Amount = @Amount WHERE Training_Id = @Training_Id AND ID = @ID" :
                        "UPDATE Food_Training SET Verified_Amount = @Amount WHERE Training_Id = @Training_Id AND ID = @ID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Amount", updatedAmount);
                        cmd.Parameters.AddWithValue("@Training_Id", trainingId);
                        cmd.Parameters.AddWithValue("@ID", id);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Set the TextBox to read-only after saving
                txtAmount.ReadOnly = true;

                // Show success message
                lblMessage.Text = "Amount updated successfully.";
                lblMessage.Visible = true;

                // Toggle the buttons
                Button btnEdit = (Button)row.FindControl("btnEdit");
                btnEdit.Visible = true; // Ensure the Edit button is visible
                btnSave.Visible = false; // Hide the Save button
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred: " + ex.Message;
                lblMessage.Visible = true;
            }
        }


        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btnEdit = (Button)sender;
            GridViewRow row = (GridViewRow)btnEdit.NamingContainer;

            TextBox txtAmount = (TextBox)row.FindControl("txtAmount");
            Button btnSave = (Button)row.FindControl("btnSave");

            // Enable editing for the TextBox
            txtAmount.ReadOnly = false;

            // Toggle button visibility
            btnSave.Visible = true;
            btnEdit.Visible = false;
        }



        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Check if the form is already submitted
            if (Session["IsFormSubmitted"] != null && (bool)Session["IsFormSubmitted"])
            {
                lblMessage.Text = "Form has already been submitted.";
                lblMessage.Visible = true;
                return;
            }

            // Mark the form as submitted
            Session["IsFormSubmitted"] = true;

            bool isSuccessful = true;

            foreach (GridViewRow row in TrainingGridView.Rows)
            {
                TextBox txtAmount = (TextBox)row.FindControl("txtAmount");
                if (txtAmount != null)
                {
                    decimal updatedAmount;
                    if (!decimal.TryParse(txtAmount.Text, out updatedAmount))
                    {
                        lblMessage.Text = "Invalid amount in one or more rows. Please correct and try again.";
                        lblMessage.Visible = true;
                        isSuccessful = false;
                        break;
                    }

                    int id = Convert.ToInt32(((Label)row.FindControl("lblId")).Text);
                    string source = ((Label)row.FindControl("lblSource")).Text;
                    int trainingId = Convert.ToInt32(((Label)row.FindControl("lblTrainingId")).Text);

                    string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

                    try
                    {
                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            // Update the amount in the respective table (Conveyance or Food)
                            string updateQuery = source == "Conveyance"
                                ? "UPDATE conv_Training SET Verified_Amount = @Amount WHERE ID = @ID"
                                : "UPDATE Food_Training SET Verified_Amount = @Amount WHERE ID = @ID";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@Amount", updatedAmount);
                                cmd.Parameters.AddWithValue("@ID", id);

                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }

                            // Update the isclaimable column in the conv_Training table
                            // Assuming both Conveyance and Food data need to update the same conv_Training table
                            string updateClaimableQuery = @"
                        UPDATE Training
                        SET isclaimable = 1
                        WHERE Training_Id = @Training_Id";

                            using (SqlCommand cmdClaimable = new SqlCommand(updateClaimableQuery, con))
                            {
                                cmdClaimable.Parameters.AddWithValue("@Training_Id", trainingId);

                                con.Open();
                                cmdClaimable.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Error during update: " + ex.Message;
                        lblMessage.Visible = true;
                        isSuccessful = false;
                        break;
                    }
                }
            }

            if (isSuccessful)
            {
                lblMessage.Text = "Amount updated successfully.";
                lblMessage.Visible = true;

                // Disable edit/save buttons after submission
                DisableEditAndSaveButtons();
            }
        }



        protected void TrainingGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the TextBox and set it to read-only by default
                TextBox txtAmount = (TextBox)e.Row.FindControl("txtAmount");
                if (txtAmount != null)
                {
                    txtAmount.ReadOnly = true; // Make the TextBox read-only initially
                }

                // Find the Save button and hide it by default
                Button btnSave = (Button)e.Row.FindControl("btnSave");
                if (btnSave != null)
                {
                    btnSave.Visible = false; // Hide the Save button initially
                }

                // You can add additional logic here if needed
            }
        }

    }
}