using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace Vivify
{
    public partial class Expenses : Page
    {
        // Declare DataTables at the class level
        private DataTable dtFood = new DataTable();
        private DataTable dtMiscellaneous = new DataTable();
        private DataTable dtOthers = new DataTable();
        private DataTable dtLodging = new DataTable();
        private DataTable dtConveyance = new DataTable(); // Added here
        private DataTable dtRefreshment = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            // Handle download action
            if (Request.QueryString["action"] == "download")
            {
                DownloadExcelFile();
                return;
            }

            if (!IsPostBack)
            {
                GridViewFood.Visible = true;
                GridViewMiscellaneous.Visible = true;
                GridViewOthers.Visible = true;
                GridViewLodging.Visible = true;
                GridViewConveyance.Visible = true;

                InitializeControls();
                pnlLocalExpenses.Visible = false;
                pnlTourExpenses.Visible = false;
                lblError.Text = string.Empty;

                txtLocalMiscAmount.Text = string.Empty;
                txtLocalOthersAmount.Text = string.Empty;
                txtTourMiscAmount.Text = string.Empty;
                txtTourOthersAmount.Text = string.Empty;

                // Clear localStorage when page first loads (fresh navigation)
                ClientScript.RegisterStartupScript(this.GetType(), "ClearExcelStorage",
                    "localStorage.removeItem('excelFileName'); localStorage.removeItem('excelFileRemoveBtn');", true);

                if (Session["ServiceId"] != null)
                {
                    int serviceId = (int)Session["ServiceId"];
                    DisplayExpenses(serviceId);

                    string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        con.Open();
                        string getServiceDetailsSql = "SELECT StatusId FROM Services WHERE ServiceId = @ServiceId";
                        using (SqlCommand cmd = new SqlCommand(getServiceDetailsSql, con))
                        {
                            cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                            var status = cmd.ExecuteScalar();

                            if (status != null && (int)status == 3)
                            {
                                // ✅ Only lock if VERIFIED (status = 3)
                                LockEntireForm();
                            }
                            else
                            {
                                // ✅ Allow resubmission if status is 1 (Draft) or 2 (Submitted)
                                // Re-enable ALL relevant controls
                                EnableEntireForm(); // ← We'll define this
                            }
                        }
                    }
                }
                else
                {
                    // No ServiceId in session - allow user to fill in new expenses
                    ddlExpenseType.Enabled = true;
                }
            }
        }
        private void LockEntireForm()
        {
            // Disable ALL input fields
            ddlExpenseType.Enabled = false;
            ddlLocalExpenseType.Enabled = false;
            ddlTourExpenseType.Enabled = false;
            ddlAwardExpenseType.Enabled = false;
            ddlLocalMode.Enabled = false;
            ddlTourTransportMode.Enabled = false;
            txtTourFoodDesignation.Enabled = false;

            // Disable all textboxes
            foreach (Control c in Page.Controls)
                DisableAllTextboxes(c);

            // Disable submit buttons
            btnSubmit.Enabled = false;
            btnChangeStatus.Enabled = false;
            btnCancel.Enabled = false;

            // Disable edit/delete in GridViews
            DisableEditAndDeleteButtons(); // your existing method
        }

        private void DisableAllTextboxes(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox tb) tb.Enabled = false;
                else if (ctrl is FileUpload fu) fu.Enabled = false;
                else if (ctrl.HasControls()) DisableAllTextboxes(ctrl);
            }
        }

        private void EnableAllButtons()
        {
            // Re-enable if needed (optional)
            btnSubmit.Enabled = true;
            btnChangeStatus.Enabled = true;
        }
        private void DisableEditAndDeleteButtons()
        {
            foreach (GridViewRow row in GridViewConveyance.Rows)
            {
                Button btnEdit = (Button)row.FindControl("btnEditConveyance");
                Button btnDelete = (Button)row.FindControl("btnDelete");

                if (btnEdit != null)
                    btnEdit.Enabled = false; // Disable Edit button

                if (btnDelete != null)
                    btnDelete.Enabled = false; // Disable Delete button
            }

            foreach (GridViewRow row in GridViewFood.Rows)
            {
                Button btnEditFood = (Button)row.FindControl("btnEditFood");
                Button btnDeleteFood = (Button)row.FindControl("btnDeleteFood");

                if (btnEditFood != null)
                    btnEditFood.Enabled = false; // Disable Edit button

                if (btnDeleteFood != null)
                    btnDeleteFood.Enabled = false; // Disable Delete button
            }

            foreach (GridViewRow row in GridViewOthers.Rows)
            {
                Button btnEditOthers = (Button)row.FindControl("btnEditOthers");
                Button btnDeleteOthers = (Button)row.FindControl("btnDeleteOthers");

                if (btnEditOthers != null)
                    btnEditOthers.Enabled = false; // Disable Edit button

                if (btnDeleteOthers != null)
                    btnDeleteOthers.Enabled = false; // Disable Delete button
            }

            foreach (GridViewRow row in GridViewMiscellaneous.Rows)
            {
                Button btnEditMiscellaneous = (Button)row.FindControl("btnEditMiscellaneous");
                Button btnDeleteMiscellaneous = (Button)row.FindControl("btnDeleteMiscellaneous");

                if (btnEditMiscellaneous != null)
                    btnEditMiscellaneous.Enabled = false; // Disable Edit button

                if (btnDeleteMiscellaneous != null)
                    btnDeleteMiscellaneous.Enabled = false; // Disable Delete button
            }

            foreach (GridViewRow row in GridViewLodging.Rows)
            {
                Button btnEditLodging = (Button)row.FindControl("btnEditLodging");
                Button btnDeleteLodging = (Button)row.FindControl("btnDeleteLodging");

                if (btnEditLodging != null)
                    btnEditLodging.Enabled = false; // Disable Edit button

                if (btnDeleteLodging != null)
                    btnDeleteLodging.Enabled = false; // Disable Delete button
            }
        }

        protected void InitializeControls()
        {
            pnlLocalExpenses.Visible = false;
            pnlTourExpenses.Visible = false;
            //lblError.Text = string.Empty;

            // Clear amounts for Others and Miscellaneous
            txtLocalMiscAmount.Text = string.Empty;
            txtLocalOthersAmount.Text = string.Empty;
            txtTourMiscAmount.Text = string.Empty;
            txtTourOthersAmount.Text = string.Empty;

            // Initialize dropdowns and textboxes
            ddlLocalExpenseType.SelectedIndex = -1;
            ddlTourExpenseType.SelectedIndex = -1;
            ddlLocalExpenseType.Enabled = false;
            ddlTourExpenseType.Enabled = false;
            ddlLocalMode.SelectedIndex = -1;
            ddlTourTransportMode.SelectedIndex = -1;
            ddlLocalMode.Enabled = false;
            ddlTourTransportMode.Enabled = false;
        }



        // Helper method to bind GridViews
        private void BindGridView(GridView gridView, DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                gridView.DataSource = dataTable;
                gridView.DataBind();
            }
            else
            {
                gridView.DataSource = null; // Clear if no data
                gridView.DataBind(); // Bind to refresh the GridView
            }
        }

        protected void DisplayExpenses(int serviceId)
        {
            // If serviceId is 0 (not set), skip loading expenses
            if (serviceId == 0)
            {
                return;
            }

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            DataTable dtFood = new DataTable();
            DataTable dtMiscellaneous = new DataTable();
            DataTable dtOthers = new DataTable();
            DataTable dtLodging = new DataTable();
            DataTable dtConveyance = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string[] queries = {
            "SELECT * FROM Food WHERE ServiceId = @ServiceId",
            "SELECT * FROM Miscellaneous WHERE ServiceId = @ServiceId",
            "SELECT * FROM Others WHERE ServiceId = @ServiceId",
            "SELECT * FROM Lodging WHERE ServiceId = @ServiceId",
            "SELECT * FROM Conveyance WHERE ServiceId = @ServiceId"
        };
                DataTable[] tables = { dtFood, dtMiscellaneous, dtOthers, dtLodging, dtConveyance };
                for (int i = 0; i < queries.Length; i++)
                {
                    using (SqlCommand cmd = new SqlCommand(queries[i], con))
                    {
                        cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(tables[i]);
                        }
                    }
                }
            }

            // ✅ STEP 1: Find the LATEST DATE across ALL tables
            DateTime? globalLatestDate = null;
            DataTable[] allTables = { dtFood, dtConveyance, dtOthers, dtMiscellaneous, dtLodging };
            foreach (var table in allTables)
            {
                var latestInTable = GetLatestDate(table, "Date");
                if (latestInTable.HasValue && (!globalLatestDate.HasValue || latestInTable.Value > globalLatestDate.Value))
                {
                    globalLatestDate = latestInTable;
                }
            }

            // ✅ STEP 2: Set that date in EVERY date field (Local + Tour)
            string latestDateString = globalLatestDate?.ToString("yyyy-MM-dd") ?? string.Empty;

            if (!string.IsNullOrEmpty(latestDateString))
            {
                // Local date fields
                txtLocalFoodDate.Text = latestDateString;
                txtLocalBikeDate.Text = latestDateString;
                txtLocalCabDate.Text = latestDateString;
                txtLocalAutoDate.Text = latestDateString;
                txtLocalOthersDate.Text = latestDateString;
                txtLocalMiscDate.Text = latestDateString;

                // Tour date fields
                txtTourFoodDate.Text = latestDateString;
                txtCabDate.Text = latestDateString;
                txtBusDate.Text = latestDateString;
                txtTrainDate.Text = latestDateString;
                txtFlightDate.Text = latestDateString;
                txtTourAutoDate.Text = latestDateString;
                txtTourOthersDate.Text = latestDateString;
                txtTourMiscDate.Text = latestDateString;

                // Award date (optional — include if needed)
                // txtAwardDate.Text = latestDateString;
            }

            // ✅ STEP 3: Bind GridViews and calculate totals (unchanged)
            decimal totalConveyance = BindGridAndCalculateTotal(GridViewConveyance, dtConveyance, lblTotalLocalConveyance, "Total Conveyance:");
            decimal totalFood = BindGridAndCalculateTotal(GridViewFood, dtFood, lblTotalLocalFood, "Total Food:");
            decimal totalMisc = BindGridAndCalculateTotal(GridViewMiscellaneous, dtMiscellaneous, lblTotalMiscellaneous, "Total Miscellaneous:");
            decimal totalOthers = BindGridAndCalculateTotal(GridViewOthers, dtOthers, lblTotalOthers, "Total Others:");
            decimal totalLodging = BindGridAndCalculateTotal(GridViewLodging, dtLodging, lblTotalLodging, "Total Lodging:");

            decimal overallTotal = totalConveyance + totalFood + totalMisc + totalOthers + totalLodging;
            decimal localAmount = totalFood; // Adjust logic if needed
            decimal tourAmount = totalConveyance + totalLodging; // Example

            lblTotalReimbursement.Text = $"Overall Total: {overallTotal:N2}";
            UpsertExpenseTotals(serviceId, totalConveyance, totalFood, totalMisc, totalOthers, totalLodging, overallTotal, localAmount, tourAmount);

            // Bind the individual category grids
            BindExpenseSummaryTable(dtConveyance, dtFood, dtMiscellaneous, dtOthers, dtLodging);
        }

        /// <summary>
        /// Creates and binds a summary table showing each expense category with its total amount
        /// </summary>
        private void BindExpenseSummaryTable(DataTable dtConveyance, DataTable dtFood, DataTable dtMisc, DataTable dtOthers, DataTable dtLodging)
        {
            var summaryData = new List<object>();

            // Add categories to the data list if they have data
            if (dtConveyance != null && dtConveyance.Rows.Count > 0)
            {
                summaryData.Add(new
                {
                    CategoryName = "Conveyance",
                    Details = dtConveyance
                });
            }
            if (dtFood != null && dtFood.Rows.Count > 0)
            {
                summaryData.Add(new
                {
                    CategoryName = "Food",
                    Details = dtFood
                });
            }
            if (dtLodging != null && dtLodging.Rows.Count > 0)
            {
                summaryData.Add(new
                {
                    CategoryName = "Lodging",
                    Details = dtLodging
                });
            }
            if (dtOthers != null && dtOthers.Rows.Count > 0)
            {
                summaryData.Add(new
                {
                    CategoryName = "Others",
                    Details = dtOthers
                });
            }
            if (dtMisc != null && dtMisc.Rows.Count > 0)
            {
                summaryData.Add(new
                {
                    CategoryName = "Miscellaneous",
                    Details = dtMisc
                });
            }

            // Bind to Repeater if it exists
            if (rptIndividualSummaries != null)
            {
                rptIndividualSummaries.DataSource = summaryData;
                rptIndividualSummaries.DataBind();
            }
        }

        private void DeleteExpense(int id, string category)
        {
            // List of valid categories to avoid SQL injection
            var validCategories = new List<string> { "Conveyance", "Food", "Others", "Miscellaneous", "Lodging" };

            // Check if the category is valid
            if (!validCategories.Contains(category))
            {
                throw new Exception("Invalid category specified.");
            }

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            // Construct the query safely by using parameterized queries
            string query = $"DELETE FROM {category} WHERE Id = @Id"; // Deleting from the specified category table

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id); // Add Id as parameter for deletion

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery(); // Executes the delete query
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error deleting record: " + ex.Message);
                    }
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btnDelete = (Button)sender;
            int id = Convert.ToInt32(btnDelete.CommandArgument); // Get the Id from CommandArgument

            string category = btnDelete.CommandName; // This will get the category (Food, Others, etc.)

            try
            {
                // Call method to delete the row from the database by Id
                DeleteExpense(id, category);

                // Rebind all GridViews to reflect the updated data
                BindGridData("Conveyance", 0);
                BindGridData("Food", 0);
                BindGridData("Others", 0);
                BindGridData("Miscellaneous", 0);
                BindGridData("Lodging", 0);

                // Optionally, force a page refresh
                Response.Redirect(Request.Url.ToString()); // Refresh the page to reload data
            }
            catch (Exception ex)
            {
                // Handle the error (display error message or log)
                lblError.Text = "Error deleting the record: " + ex.Message;
            }
        }

        protected void gvCategoryDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView gv = (GridView)sender;
            
            // Find the category hidden field in the repeater item
            HiddenField hdnCategory = (HiddenField)gv.NamingContainer.FindControl("hdnCategoryName");
            string category = hdnCategory?.Value;

            if (e.CommandName == "EditSummary")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                int id = Convert.ToInt32(gv.DataKeys[rowIndex].Value);

                try
                {
                    PopulateFormForEdit(category, id);
                    
                    // Scroll to top of form
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ScrollToTop", "window.scrollTo(0,0);", true);
                }
                catch (Exception ex)
                {
                    lblError.Text = "Error populating form: " + ex.Message;
                    lblError.ForeColor = System.Drawing.Color.Red;
                }
            }
            else if (e.CommandName == "DeleteSummary")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                try
                {
                    DeleteExpense(id, category);
                    Response.Redirect(Request.Url.ToString());
                }
                catch (Exception ex)
                {
                    lblError.Text = "Error deleting: " + ex.Message;
                    lblError.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private void PopulateFormForEdit(string category, int id)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = $"SELECT * FROM {category} WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string expenseType = reader["ExpenseType"]?.ToString();
                    ddlExpenseType.SelectedValue = expenseType;
                    ddlExpenseType_SelectedIndexChanged(null, null); // Show correct local/tour panel

                    // Track ID and Category for the Save button's "Update" logic
                    hdnEditRecordId.Value = id.ToString();
                    hdnEditCategory.Value = category;
                    btnSubmit.Text = "Update";

                    // Common fields
                    string date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd") : "";
                    string amount = reader["Amount"]?.ToString();
                    string particulars = category == "Miscellaneous" ? reader["PurchasedItem"]?.ToString() : reader["Particulars"]?.ToString();
                    string remarks = reader["Remarks"]?.ToString();
                    string smoNo = reader["SmoNo"]?.ToString();
                    string soNo = reader["SoNo"]?.ToString();
                    string refNo = reader["RefNo"]?.ToString();

                    if (expenseType == "Local")
                    {
                        if (category == "Food")
                        {
                            ddlLocalExpenseType.SelectedValue = "Food";
                            ddlLocalExpenseType_SelectedIndexChanged(null, null);
                            PopulateLocalFoodFields(date, amount, particulars, remarks, smoNo, soNo, refNo);
                            if (reader["FromTime"] != DBNull.Value) txtLocalFoodFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                            if (reader["ToTime"] != DBNull.Value) txtLocalFoodToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                        }
                        else if (category == "Miscellaneous")
                        {
                            ddlLocalExpenseType.SelectedValue = "Miscellaneous";
                            ddlLocalExpenseType_SelectedIndexChanged(null, null);
                            PopulateLocalMiscellaneousFields(date, amount, particulars, remarks, smoNo, soNo, refNo);
                            if (reader["FromTime"] != DBNull.Value) txtLocalMiscFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                            if (reader["ToTime"] != DBNull.Value) txtLocalMiscToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                        }
                        else if (category == "Others")
                        {
                            ddlLocalExpenseType.SelectedValue = "Others";
                            ddlLocalExpenseType_SelectedIndexChanged(null, null);
                            PopulateLocalOthersFields(date, amount, particulars, remarks, smoNo, soNo, refNo);
                            if (reader["FromTime"] != DBNull.Value) txtLocalOthersFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                            if (reader["ToTime"] != DBNull.Value) txtLocalOthersToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                        }
                        else if (category == "Conveyance")
                        {
                            ddlLocalExpenseType.SelectedValue = "Conveyance";
                            ddlLocalExpenseType_SelectedIndexChanged(null, null);
                            string transportType = reader["TransportType"]?.ToString();
                            ddlLocalMode.SelectedValue = transportType;
                            ddlLocalMode_SelectedIndexChanged(null, null);
                            PopulateLocalConveyanceFields(date, amount, particulars, remarks, smoNo, soNo, refNo);
                            
                            if (transportType == "Bike")
                            {
                                if (reader["FromTime"] != DBNull.Value) txtLocalBikeFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                                if (reader["ToTime"] != DBNull.Value) txtLocalBikeToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                                txtLocalBikeParticular.Text = particulars;
                                txtLocalBikeRemarks.Text = remarks;
                                txtLocalBikeSMONo.Text = smoNo;
                                txtLocalBikeSONo.Text = soNo;
                                txtLocalBikeRefNo.Text = refNo;
                                txtLocalDistance.Text = reader["Distance"]?.ToString();
                            }
                            else if (transportType == "Cab/Bus")
                            {
                                if (reader["FromTime"] != DBNull.Value) txtLocalCabFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                                if (reader["ToTime"] != DBNull.Value) txtLocalCabToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                                txtLocalCabParticular.Text = particulars;
                                txtLocalCabRemarks.Text = remarks;
                                txtLocalCabSMONo.Text = smoNo;
                                txtLocalCabSONo.Text = soNo;
                                txtLocalCabRefNo.Text = refNo;
                            }
                            else if (transportType == "Auto")
                            {
                                if (reader["FromTime"] != DBNull.Value) txtLocalAutoFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                                if (reader["ToTime"] != DBNull.Value) txtLocalAutoToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                                txtLocalAutoParticular.Text = particulars;
                                txtLocalAutoRemarks.Text = remarks;
                                txtLocalAutoSMONo.Text = smoNo;
                                txtLocalAutoSONo.Text = soNo;
                                txtLocalAutoRefNo.Text = refNo;
                                txtLocalAutoDistance.Text = reader["Distance"]?.ToString();
                            }
                        }
                    }
                    else if (expenseType == "Tour")
                    {
                        if (category == "Food")
                        {
                            ddlTourExpenseType.SelectedValue = "Food";
                            ddlTourExpenseType_SelectedIndexChanged(null, null);
                            PopulateTourFoodFields(date, amount, particulars, remarks, smoNo, soNo, refNo);
                            if (reader["FromTime"] != DBNull.Value) txtTourFoodFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                            if (reader["ToTime"] != DBNull.Value) txtTourFoodToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                            if (reader["Designation"] != DBNull.Value) txtTourFoodDesignation.SelectedValue = reader["Designation"].ToString();
                        }
                        else if (category == "Miscellaneous")
                        {
                            ddlTourExpenseType.SelectedValue = "Miscellaneous";
                            ddlTourExpenseType_SelectedIndexChanged(null, null);
                            PopulateTourMiscellaneousFields(date, amount, particulars, remarks, smoNo, soNo, refNo);
                            if (reader["FromTime"] != DBNull.Value) txtTourMiscFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                            if (reader["ToTime"] != DBNull.Value) txtTourMiscToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                        }
                        else if (category == "Others")
                        {
                            ddlTourExpenseType.SelectedValue = "Lodging"; // This seems to be mapped to Lodging in UI for Tour
                            ddlTourExpenseType_SelectedIndexChanged(null, null);
                            PopulateTourOthersFields(date, amount, particulars, remarks, smoNo, soNo, refNo);
                            if (reader["FromTime"] != DBNull.Value) txtFromTimeTourOthers.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                            if (reader["ToTime"] != DBNull.Value) txtToTimeTourOthers.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                        }
                        else if (category == "Lodging")
                        {
                            ddlTourExpenseType.SelectedValue = "Lodging";
                            ddlTourExpenseType_SelectedIndexChanged(null, null);
                            // Lodging use same fields as Others in Tour
                            PopulateTourOthersFields(date, amount, particulars, remarks, smoNo, soNo, refNo);
                            if (reader["FromTime"] != DBNull.Value) txtFromTimeTourOthers.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                            if (reader["ToTime"] != DBNull.Value) txtToTimeTourOthers.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                        }
                        else if (category == "Conveyance")
                        {
                            ddlTourExpenseType.SelectedValue = "Conveyance";
                            ddlTourExpenseType_SelectedIndexChanged(null, null);
                            string transportType = reader["TransportType"]?.ToString();
                            ddlTourTransportMode.SelectedValue = transportType;
                            ddlTourTransportMode_SelectedIndexChanged(null, null);
                            
                            if (transportType == "Cab")
                            {
                                txtCabDate.Text = date;
                                txtCabAmount.Text = amount;
                                if (reader["FromTime"] != DBNull.Value) txtFromTimeCab.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                                if (reader["ToTime"] != DBNull.Value) txtToTimeCab.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                                txtParticularsCab.Text = particulars;
                                txtRemarksCab.Text = remarks;
                                txtCabSmoNo.Text = smoNo;
                                txtCabSoNo.Text = soNo;
                                txtCabRefNo.Text = refNo;
                            }
                            else if (transportType == "Train")
                            {
                                txtTrainDate.Text = date;
                                txtTrainAmount.Text = amount;
                                if (reader["FromTime"] != DBNull.Value) txtFromTimeTrain.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                                if (reader["ToTime"] != DBNull.Value) txtToTimeTrain.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                                txtParticularsTrain.Text = particulars;
                                txtRemarksTrain.Text = remarks;
                                txtTrainSmoNo.Text = smoNo;
                                txtTrainSoNo.Text = soNo;
                                txtTrainRefNo.Text = refNo;
                            }
                            else if (transportType == "Flight")
                            {
                                txtFlightDate.Text = date;
                                txtFlightAmount.Text = amount;
                                if (reader["FromTime"] != DBNull.Value) txtFlightFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                                if (reader["ToTime"] != DBNull.Value) txtFlightToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                                txtFlightParticulars.Text = particulars;
                                txtFlightRemarks.Text = remarks;
                                txtFlightSmoNo.Text = smoNo;
                                txtFlightSoNo.Text = soNo;
                                txtFlightRefNo.Text = refNo;
                            }
                            else if (transportType == "Bus")
                            {
                                txtBusDate.Text = date;
                                txtBusAmount.Text = amount;
                                if (reader["FromTime"] != DBNull.Value) txtFromTimeBus.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                                if (reader["ToTime"] != DBNull.Value) txtToTimeBus.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                                txtParticularsBus.Text = particulars;
                                txtRemarksBus.Text = remarks;
                                txtBusSmoNo.Text = smoNo;
                                txtBusSoNo.Text = soNo;
                                txtBusRefNo.Text = refNo;
                            }
                            else if (transportType == "Auto")
                            {
                                txtTourAutoDate.Text = date;
                                txtTourAutoAmount.Text = amount;
                                if (reader["FromTime"] != DBNull.Value) txtTourAutoFromTime.Text = ((TimeSpan)reader["FromTime"]).ToString(@"hh\:mm");
                                if (reader["ToTime"] != DBNull.Value) txtTourAutoToTime.Text = ((TimeSpan)reader["ToTime"]).ToString(@"hh\:mm");
                                txtTourAutoParticular.Text = particulars;
                                txTourAutoRemarks.Text = remarks;
                                txtTourAutoSmoNo.Text = smoNo;
                                txtTourAutoSoNo.Text = soNo;
                                txtTourAutoRefNo.Text = refNo;
                                txtTourAutoDistance.Text = reader["Distance"]?.ToString();
                            }
                        }
                    }
                }
                reader.Close();
            }
        }

        protected void gvCategoryDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Optional: Handle specialized formatting or locking logic here
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // You can add logic here to disable buttons if the form is locked
            }
        }

        private void BindGridData(string category, int id)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = id == 0
                    ? $"SELECT * FROM {category}" // No filtering by Id
                    : $"SELECT * FROM {category} WHERE Id=@Id"; // Filter by Id

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (id != 0)  // Only add parameter if not refreshing all data
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dtCategory = new DataTable();
                        da.Fill(dtCategory);

                        // Bind the correct GridView based on the category
                        if (category == "Conveyance")
                            GridViewConveyance.DataSource = dtCategory;
                        else if (category == "Food")
                            GridViewFood.DataSource = dtCategory;
                        else if (category == "Others")
                            GridViewOthers.DataSource = dtCategory;
                        else if (category == "Miscellaneous")
                            GridViewMiscellaneous.DataSource = dtCategory;
                        else if (category == "Lodging")
                            GridViewLodging.DataSource = dtCategory;

                        // Rebind the GridView
                        if (category == "Conveyance")
                            GridViewConveyance.DataBind();
                        else if (category == "Food")
                            GridViewFood.DataBind();
                        else if (category == "Others")
                            GridViewOthers.DataBind();
                        else if (category == "Miscellaneous")
                            GridViewMiscellaneous.DataBind();
                        else if (category == "Lodging")
                            GridViewLodging.DataBind();
                    }
                }
            }
        }



        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btnEdit = (Button)sender;
            GridViewRow row = (GridViewRow)btnEdit.NamingContainer;

            // Get the table name from CommandArgument
            string tableName = btnEdit.CommandArgument;

            // Dynamically get the control IDs based on the tableName
            string lblDateID = "lbl" + tableName + "Date";
            string txtDateID = "txt" + tableName + "Date";
            string lblAmountID = "lbl" + tableName + "Amount";
            string txtAmountID = "txt" + tableName + "Amount";

            // Find the controls in the current row
            Label lblDate = (Label)row.FindControl(lblDateID);
            TextBox txtDate = (TextBox)row.FindControl(txtDateID);
            Label lblAmount = (Label)row.FindControl(lblAmountID);
            TextBox txtAmount = (TextBox)row.FindControl(txtAmountID);

            if (lblDate != null && txtDate != null && lblAmount != null && txtAmount != null)
            {
                if (btnEdit.Text == "Edit")
                {
                    // Switch to Edit mode: Show textboxes, hide labels
                    txtDate.Visible = true;
                    txtAmount.Visible = true;

                    lblDate.Visible = false;
                    lblAmount.Visible = false;

                    btnEdit.Text = "Save";
                }
                else
                {
                    // Save mode: Validate and update
                    if (decimal.TryParse(txtAmount.Text, out decimal updatedAmount) &&
                        DateTime.TryParse(txtDate.Text, out DateTime updatedDate))
                    {
                        int id = Convert.ToInt32(((GridView)row.NamingContainer).DataKeys[row.RowIndex].Value);

                        try
                        {
                            // Call the method to update the database
                            UpdateExpense(tableName, id, updatedAmount, updatedDate);

                            // Refresh the page after saving
                            Response.Redirect(Request.Url.ToString());
                        }
                        catch (Exception ex)
                        {
                            lblError.Text = "Error updating the record: " + ex.Message;
                        }
                    }
                    else
                    {
                        lblError.Text = "Invalid input. Please check the date and amount.";
                    }
                }
            }
        }

        protected void btnEdit1_Click(object sender, EventArgs e)
        {
            Button btnEdit = (Button)sender;
            string commandArgument = btnEdit.CommandArgument;

            switch (commandArgument)
            {
                case "Conveyance":
                    EditConveyance(sender);
                    break;
                case "Others":
                    EditOthers(sender);
                    break;
                case "Miscellaneous":
                    EditMiscellaneous(sender);
                    break;
                case "Food":
                    EditFood(sender);
                    break;
                case "Lodging":
                    EditLodging(sender);
                    break;
                default:
                    lblError.Text = "Invalid CommandArgument.";
                    break;
            }
        }

        protected void EditConveyance(object sender)
        {
            try
            {
                Button btnEdit = (Button)sender;
                GridViewRow row = (GridViewRow)btnEdit.NamingContainer;

                if (row == null)
                {
                    lblError.Text = "Error: Unable to find the row.";
                    return;
                }

                // Find controls in the Conveyance GridView row
                Label lblDate = (Label)row.FindControl("lblDate");
                TextBox txtDate = (TextBox)row.FindControl("txtDate");
                Label lblAmount = (Label)row.FindControl("lblAmount");
                TextBox txtAmount = (TextBox)row.FindControl("txtAmount");

                if (lblDate == null || txtDate == null || lblAmount == null || txtAmount == null)
                {
                    lblError.Text = "Error: One or more controls in Conveyance are missing.";
                    return;
                }

                if (btnEdit.Text == "Edit")
                {
                    // Switch to Edit mode
                    txtDate.Visible = true;
                    txtAmount.Visible = true;

                    lblDate.Visible = false;
                    lblAmount.Visible = false;

                    btnEdit.Text = "Save";
                }
                else
                {
                    // Validate and save data
                    if (decimal.TryParse(txtAmount.Text, out decimal updatedAmount) &&
                        DateTime.TryParse(txtDate.Text, out DateTime updatedDate))
                    {
                        int id = Convert.ToInt32(GridViewConveyance.DataKeys[row.RowIndex].Value);

                        // Update the database
                        UpdateExpense("Conveyance", id, updatedAmount, updatedDate);

                        // Refresh the page after saving
                        Response.Redirect(Request.Url.ToString());
                    }
                    else
                    {
                        lblError.Text = "Invalid input for Conveyance. Please check the date and amount.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error in Conveyance: " + ex.Message;
            }
        }

        protected void EditOthers(object sender)
        {
            try
            {
                Button btnEdit = (Button)sender;
                GridViewRow row = (GridViewRow)btnEdit.NamingContainer;

                if (row == null)
                {
                    lblError.Text = "Error: Unable to find the row.";
                    return;
                }

                // Find controls in the Others GridView row
                Label lblDate = (Label)row.FindControl("lblOthersDate");
                TextBox txtDate = (TextBox)row.FindControl("txtOthersDate");
                Label lblAmount = (Label)row.FindControl("lblOthersAmount");
                TextBox txtAmount = (TextBox)row.FindControl("txtOthersAmount");

                if (lblDate == null || txtDate == null || lblAmount == null || txtAmount == null)
                {
                    lblError.Text = "Error: One or more controls in Others are missing.";
                    return;
                }

                if (btnEdit.Text == "Edit")
                {
                    // Switch to Edit mode
                    txtDate.Visible = true;
                    txtAmount.Visible = true;

                    lblDate.Visible = false;
                    lblAmount.Visible = false;

                    btnEdit.Text = "Save";
                }
                else
                {
                    // Validate and save data
                    if (decimal.TryParse(txtAmount.Text, out decimal updatedAmount) &&
                        DateTime.TryParse(txtDate.Text, out DateTime updatedDate))
                    {
                        int id = Convert.ToInt32(GridViewOthers.DataKeys[row.RowIndex].Value);

                        // Update the database
                        UpdateExpense("Others", id, updatedAmount, updatedDate);

                        // Refresh the page after saving
                        Response.Redirect(Request.Url.ToString());
                    }
                    else
                    {
                        lblError.Text = "Invalid input for Others. Please check the date and amount.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error in Others: " + ex.Message;
            }
        }

        protected void EditMiscellaneous(object sender)
        {
            try
            {
                Button btnEdit = (Button)sender;
                GridViewRow row = (GridViewRow)btnEdit.NamingContainer;

                if (row == null)
                {
                    lblError.Text = "Error: Unable to find the row.";
                    return;
                }

                // Find controls in the Miscellaneous GridView row
                Label lblDate = (Label)row.FindControl("lblMiscDate");
                TextBox txtDate = (TextBox)row.FindControl("txtMiscDate");
                Label lblAmount = (Label)row.FindControl("lblMiscAmount");
                TextBox txtAmount = (TextBox)row.FindControl("txtMiscAmount");

                if (lblDate == null || txtDate == null || lblAmount == null || txtAmount == null)
                {
                    lblError.Text = "Error: One or more controls in Miscellaneous are missing.";
                    return;
                }

                if (btnEdit.Text == "Edit")
                {
                    // Switch to Edit mode
                    txtDate.Visible = true;
                    txtAmount.Visible = true;

                    lblDate.Visible = false;
                    lblAmount.Visible = false;

                    btnEdit.Text = "Save";
                }
                else
                {
                    // Validate and save data
                    if (decimal.TryParse(txtAmount.Text, out decimal updatedAmount) &&
                        DateTime.TryParse(txtDate.Text, out DateTime updatedDate))
                    {
                        int id = Convert.ToInt32(GridViewMiscellaneous.DataKeys[row.RowIndex].Value);

                        // Update the database
                        UpdateExpense("Miscellaneous", id, updatedAmount, updatedDate);

                        // Refresh the page after saving
                        Response.Redirect(Request.Url.ToString());
                    }
                    else
                    {
                        lblError.Text = "Invalid input for Miscellaneous. Please check the date and amount.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error in Miscellaneous: " + ex.Message;
            }
        }

        protected void EditFood(object sender)
        {
            try
            {
                Button btnEdit = (Button)sender;
                GridViewRow row = (GridViewRow)btnEdit.NamingContainer;

                if (row == null)
                {
                    lblError.Text = "Error: Unable to find the row.";
                    return;
                }

                // Find controls in the Food GridView row
                Label lblDate = (Label)row.FindControl("lblFoodDate");
                TextBox txtDate = (TextBox)row.FindControl("txtFoodDate");
                Label lblAmount = (Label)row.FindControl("lblFoodAmount");
                TextBox txtAmount = (TextBox)row.FindControl("txtFoodAmount");

                if (lblDate == null || txtDate == null || lblAmount == null || txtAmount == null)
                {
                    lblError.Text = "Error: One or more controls in Food are missing.";
                    return;
                }

                if (btnEdit.Text == "Edit")
                {
                    // Switch to Edit mode
                    txtDate.Visible = true;
                    txtAmount.Visible = true;

                    lblDate.Visible = false;
                    lblAmount.Visible = false;

                    btnEdit.Text = "Save";
                }
                else
                {
                    // Validate and save data
                    if (decimal.TryParse(txtAmount.Text, out decimal updatedAmount) &&
                        DateTime.TryParse(txtDate.Text, out DateTime updatedDate))
                    {
                        int id = Convert.ToInt32(GridViewFood.DataKeys[row.RowIndex].Value);

                        // Update the database
                        UpdateExpense("Food", id, updatedAmount, updatedDate);

                        // Refresh the page after saving
                        Response.Redirect(Request.Url.ToString());
                    }
                    else
                    {
                        lblError.Text = "Invalid input for Food. Please check the date and amount.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error in Food: " + ex.Message;
            }
        }

        protected void EditLodging(object sender)
        {
            try
            {
                Button btnEdit = (Button)sender;
                GridViewRow row = (GridViewRow)btnEdit.NamingContainer;

                if (row == null)
                {
                    lblError.Text = "Error: Unable to find the row.";
                    return;
                }

                // Find controls in the Lodging GridView row
                Label lblDate = (Label)row.FindControl("lblLodgingDate");
                TextBox txtDate = (TextBox)row.FindControl("txtLodgingDate");
                Label lblAmount = (Label)row.FindControl("lblLodgingAmount");
                TextBox txtAmount = (TextBox)row.FindControl("txtLodgingAmount");

                if (lblDate == null || txtDate == null || lblAmount == null || txtAmount == null)
                {
                    lblError.Text = "Error: One or more controls in Lodging are missing.";
                    return;
                }

                if (btnEdit.Text == "Edit")
                {
                    // Switch to Edit mode
                    txtDate.Visible = true;
                    txtAmount.Visible = true;

                    lblDate.Visible = false;
                    lblAmount.Visible = false;

                    btnEdit.Text = "Save";
                }
                else
                {
                    // Validate and save data
                    if (decimal.TryParse(txtAmount.Text, out decimal updatedAmount) &&
                        DateTime.TryParse(txtDate.Text, out DateTime updatedDate))
                    {
                        int id = Convert.ToInt32(GridViewLodging.DataKeys[row.RowIndex].Value);

                        // Update the database
                        UpdateExpense("Lodging", id, updatedAmount, updatedDate);

                        // Refresh the page after saving
                        Response.Redirect(Request.Url.ToString());
                    }
                    else
                    {
                        lblError.Text = "Invalid input for Lodging. Please check the date and amount.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error in Lodging: " + ex.Message;
            }
        }



        // Method to update Conveyance records in the database
        private void UpdateConveyance(int id, decimal amount, DateTime date)
        {
            // Add your database update logic for Conveyance
            // Example:
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Conveyance SET Amount = @Amount, Date = @Date WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Method to update Food records in the database
        private void UpdateFood(int id, decimal amount, DateTime date)
        {
            // Add your database update logic for Food
            // Example:
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Food SET Amount = @Amount, Date = @Date WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Method to update Lodging records in the database
        private void UpdateLodging(int id, decimal amount, DateTime date)
        {
            // Add your database update logic for Lodging
            // Example:
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Lodging SET Amount = @Amount, Date = @Date WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Method to update Others records in the database
        private void UpdateOthers(int id, decimal amount, DateTime date)
        {
            // Add your database update logic for Others
            // Example:
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Others SET Amount = @Amount, Date = @Date WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Method to update Miscellaneous records in the database
        private void UpdateMiscellaneous(int id, decimal amount, DateTime date)
        {
            // Add your database update logic for Miscellaneous
            // Example:
            string connectionString = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Miscellaneous SET Amount = @Amount, Date = @Date WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        private void UpdateExpense(string tableName, int id, decimal amount, DateTime date)
        {
            // Depending on the table name, perform different updates
            if (tableName == "Conveyance")
            {
                // Update the conveyance record in the database
                UpdateConveyance(id, amount, date);
            }
            else if (tableName == "Food")
            {
                // Update the food record in the database
                UpdateFood(id, amount, date);
            }
            else if (tableName == "Lodging")
            {
                // Update the lodging record in the database
                UpdateLodging(id, amount, date);
            }
            else if (tableName == "Others")
            {
                // Update the others record in the database
                UpdateOthers(id, amount, date);
            }
            else if (tableName == "Miscellaneous")
            {
                // Update the miscellaneous record in the database
                UpdateMiscellaneous(id, amount, date);
            }
        }



        private decimal BindGridAndCalculateTotal(GridView gridView, DataTable dataTable, Label totalLabel, string totalText)
        {
            decimal totalAmount = 0;

            // Bind data to GridView
            gridView.DataSource = dataTable;
            gridView.DataBind();

            if (dataTable.Rows.Count > 0)
            {
                // Calculate total amount, using a nullable type to handle DBNull
                totalAmount = dataTable.AsEnumerable()
                    .Sum(row => row.Field<decimal?>("Amount") ?? 0); // Use nullable decimal and default to 0 for DBNull

                // Update the total label with the formatted amount
                totalLabel.Text = $"{totalText} {totalAmount:N2}"; // Format total with descriptive text
            }
            else
            {
                totalLabel.Text = $"{totalText} 0.00"; // Handle the case where there are no rows
            }

            // Optionally call to bind footer totals if required
            BindFooterTotal(gridView, dataTable);

            return totalAmount; // Return the total amount for overall total calculation
        }

        private void BindFooterTotal(GridView gridView, DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                // Use a nullable type and sum only non-null values
                decimal totalAmount = dataTable.AsEnumerable()
                    .Sum(row => row.Field<decimal?>("Amount") ?? 0); // Use nullable decimal

                // Set footer values
                GridViewRow footer = gridView.FooterRow;
                if (footer != null)
                {
                    // Format as currency
                    footer.Cells[1].Text = totalAmount.ToString("C2"); // Adjust index if necessary
                }
            }
        }


        private void UpsertExpenseTotals(int serviceId, decimal totalConveyance, decimal totalFood, decimal totalMisc, decimal totalOthers, decimal totalLodging, decimal overallTotal, decimal localAmount, decimal tourAmount)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

            // Corrected SQL query to fetch EmployeeId, FirstName, and Advance
            string upsertExpenseSql = @"
DECLARE @EmployeeId INT, @FirstName NVARCHAR(100), @Advance DECIMAL(18, 2);

-- Fetch EmployeeId and FirstName from Employees table
SELECT @EmployeeId = e.EmployeeId, @FirstName = e.FirstName
FROM Employees e
INNER JOIN Services s ON e.EmployeeId = s.EmployeeId  -- Use Services table (plural)
WHERE s.ServiceId = @ServiceId;

-- Fetch Advance from Services table (plural)
SELECT @Advance = s.Advance  -- Make sure column name is 'Advance'
FROM Services s  -- Using Services table (plural)
WHERE s.ServiceId = @ServiceId;

-- Upsert the Expense record
IF EXISTS (SELECT 1 FROM Expense WHERE ServiceId = @ServiceId)
BEGIN
    UPDATE Expense
    SET ConveyanceTotal = @ConveyanceTotal,
        FoodTotal = @FoodTotal,
        MiscellaneousTotal = @MiscellaneousTotal,
        OthersTotal = @OthersTotal,
        LodgingTotal = @LodgingTotal,
        Total = @TotalAmount,
        LocalAmount = @LocalAmount,
        TourAmount = @TourAmount,
        EmployeeId = @EmployeeId,
        FirstName = @FirstName,
        Advance = @Advance
    WHERE ServiceId = @ServiceId;
END
ELSE
BEGIN
    INSERT INTO Expense (ServiceId, ConveyanceTotal, FoodTotal, MiscellaneousTotal, OthersTotal, LodgingTotal, Total, LocalAmount, TourAmount, EmployeeId, FirstName, Advance)
    VALUES (@ServiceId, @ConveyanceTotal, @FoodTotal, @MiscellaneousTotal, @OthersTotal, @LodgingTotal, @TotalAmount, @LocalAmount, @TourAmount, @EmployeeId, @FirstName, @Advance);
END";


            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmdUpsertExpense = new SqlCommand(upsertExpenseSql, con))
                {
                    // Add the parameters for the expense details
                    cmdUpsertExpense.Parameters.AddWithValue("@ServiceId", serviceId);
                    cmdUpsertExpense.Parameters.AddWithValue("@ConveyanceTotal", totalConveyance);
                    cmdUpsertExpense.Parameters.AddWithValue("@FoodTotal", totalFood);
                    cmdUpsertExpense.Parameters.AddWithValue("@MiscellaneousTotal", totalMisc);
                    cmdUpsertExpense.Parameters.AddWithValue("@OthersTotal", totalOthers);
                    cmdUpsertExpense.Parameters.AddWithValue("@LodgingTotal", totalLodging);
                    cmdUpsertExpense.Parameters.AddWithValue("@TotalAmount", overallTotal);
                    cmdUpsertExpense.Parameters.AddWithValue("@LocalAmount", localAmount);
                    cmdUpsertExpense.Parameters.AddWithValue("@TourAmount", tourAmount);

                    // Execute the SQL command
                    cmdUpsertExpense.ExecuteNonQuery();
                }
            }





            // Manage visibility based on selected values
            string expenseType = ddlExpenseType.SelectedValue;
            string localExpenseType = ddlLocalExpenseType.SelectedValue;
            string tourExpenseType = ddlTourExpenseType.SelectedValue;
            string AwardExpenseType = ddlAwardExpenseType.SelectedValue;

            if (expenseType == "Local")
            {
                if (localExpenseType == "Food")
                {
                    GridViewFood.Visible = true;
                }
                else if (localExpenseType == "Miscellaneous")
                {
                    GridViewMiscellaneous.Visible = true;
                }
                else if (localExpenseType == "Others")
                {
                    GridViewOthers.Visible = true;
                }
                else if (localExpenseType == "Conveyance")
                {
                    GridViewConveyance.Visible = true;
                }
            }
            else if (expenseType == "Tour")
            {
                // Show only the GridView corresponding to the selected tour expense type
                if (tourExpenseType == "Food")
                {
                    GridViewFood.Visible = true;
                }
                else if (tourExpenseType == "Miscellaneous")
                {
                    GridViewMiscellaneous.Visible = true;
                }
                else if (tourExpenseType == "Others")
                {
                    GridViewOthers.Visible = true;
                }
                else if (tourExpenseType == "Conveyance")
                {
                    GridViewConveyance.Visible = true;
                }
                else if (tourExpenseType == "Lodging")
                {
                    GridViewLodging.Visible = true;
                }
            }
            else if (expenseType == "Award")
            {
                // Show only the GridView corresponding to the selected award expense type
                if (AwardExpenseType == "Spot Award")
                {
                    GridViewSpotAward.Visible = true;
                }
                else if (AwardExpenseType == "Star Award")
                {
                    GridViewStarAward.Visible = true;
                }
            }
        }
        //  protected void ddlAwardExpenseType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    // Reset fields
        //    txtAwardDate.Text = string.Empty;
        //    txtAwardAmount.Text = string.Empty;

        //    // Get the selected award type
        //    string selectedAwardType = ddlAwardExpenseType.SelectedValue;

        //    if (!string.IsNullOrEmpty(selectedAwardType))
        //    {
        //        // Set the current date in the date field
        //        txtAwardDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

        //        // Calculate and set the amount based on the selected award type
        //        switch (selectedAwardType)
        //        {
        //            case "Star Award":
        //                txtAwardAmount.Text = "2000";
        //                break;

        //            case "Spot Award":
        //                txtAwardAmount.Text = "1500";
        //                break;

        //            default:
        //                // No action needed for invalid selections
        //                break;
        //        }
        //    }
        //}
        private int GetServiceId()
        {
            // Assuming ServiceId is stored in the session
            if (Session["ServiceId"] != null)
            {
                return Convert.ToInt32(Session["ServiceId"]);
            }
            else
            {
                // Return 0 if ServiceId is not found in session
                // This allows the form to work even without a service selected
                return 0;
            }
        }


        protected void ddlExpenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = string.Empty;

                // Hide all panels initially
                pnlLocalExpenses.Visible = false;
                pnlTourExpenses.Visible = false;
                pnlAwardExpenses.Visible = false;

                // Hide all local and tour panels initially
                pnlLocalFoodFields.Visible = false;
                pnlLocalMiscellaneousFields.Visible = false;
                pnlLocalOthersFields.Visible = false;
                pnlLocalConvenience.Visible = false;

                pnlBikeFields.Visible = false;
                pnlCabFields.Visible = false;

                pnlTourFoodFields.Visible = false;
                pnlTourMiscellaneousFields.Visible = false;
                pnlTourOthersFields.Visible = false;
                pnlTourConvenience.Visible = false;

                pnlFlightFields.Visible = false;
                pnlBusFields.Visible = false;
                pnlTrainFields.Visible = false;
                pnlcabTourFields.Visible = false;

                // Show or hide panels based on the selected expense type
                string selectedExpenseType = ddlExpenseType.SelectedValue;

                if (selectedExpenseType == "Local")
                {
                    pnlLocalExpenses.Visible = true;
                    ddlLocalExpenseType.Enabled = true;
                    ddlLocalExpenseType.SelectedIndex = 0; // Reset to default
                }
                else if (selectedExpenseType == "Tour")
                {
                    pnlTourExpenses.Visible = true;
                    ddlTourExpenseType.Enabled = true;
                    ddlTourExpenseType.SelectedIndex = 0; // Reset to default
                }
                else if (selectedExpenseType == "Award")
                {
                    pnlAwardExpenses.Visible = true;
                    ddlAwardExpenseType.Enabled = true;
                    ddlAwardExpenseType.SelectedIndex = 0; // Reset to default
                }

                // Only call DisplayExpenses if we have a valid ServiceId
                int serviceId = GetServiceId();
                if (serviceId > 0)
                {
                    DisplayExpenses(serviceId);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Error: {ex.Message}";
                lblError.ForeColor = System.Drawing.Color.Red;
            }
        }
        protected void ddlAwardExpenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int serviceId = GetServiceId(); // Implement this method to get the current ServiceId
            DisplayExpenses(serviceId);

            string selectedValue = ddlAwardExpenseType.SelectedValue.Trim();

            // Hide the panel if no valid award type is selected
            if (string.IsNullOrEmpty(selectedValue))
            {
                pnlAwardExpenses.Visible = false;
                return;
            }

            // Show the panel and populate the fields
            pnlAwardExpenses.Visible = true;

            // Set the award amount based on the selected award type
            switch (selectedValue)
            {
                case "Star Award":
                    txtAwardAmount.Text = "10000"; // Automatically set the amount for Star Award
                    break;

                case "Spot Award":
                    txtAwardAmount.Text = "1500"; // Automatically set the amount for Spot Award
                    break;

                default:
                    txtAwardAmount.Text = ""; // Clear the amount field for invalid selections
                    break;
            }
        }
        protected void ddlLocalExpenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int serviceId = GetServiceId(); // Implement this method to get the current ServiceId
            DisplayExpenses(serviceId);
            string selectedValue = ddlLocalExpenseType.SelectedValue;

            // Hide all local expense panels initially
            pnlLocalFoodFields.Visible = false;
            pnlLocalMiscellaneousFields.Visible = false;
            pnlLocalOthersFields.Visible = false;
            pnlLocalConvenience.Visible = false;


            // Show the relevant panel based on the selected expense type
            switch (selectedValue)
            {
                case "Food":
                    pnlLocalFoodFields.Visible = true;
                    txtLocalFoodAmount.Text = ""; // Clear the amount field
                    totalAccumulatedHours = 0; // Reset accumulated hours
                    break;
                case "Miscellaneous":
                    pnlLocalMiscellaneousFields.Visible = true;
                    break;
                case "Others":
                    pnlLocalOthersFields.Visible = true;
                    break;

                case "Conveyance":
                    pnlLocalConvenience.Visible = true;
                    ddlLocalMode.Enabled = true; // Enable Local mode dropdown
                    break;
            }
        }



        // Dummy method to demonstrate fetching existing dates



        protected void txtFromTime_TextChanged(object sender, EventArgs e)
        {
            CalculateFoodAmount();
        }

        protected void txtToTime_TextChanged(object sender, EventArgs e)
        {
            CalculateFoodAmount();
        }

        private double totalAccumulatedHours = 0;

        private void CalculateFoodAmount()
        {
            TimeSpan fromTimeSpan;
            TimeSpan toTimeSpan;

            // Try to parse the From and To time inputs as TimeSpan
            bool fromParsed = TimeSpan.TryParse(txtLocalFoodFromTime.Text, out fromTimeSpan);
            bool toParsed = TimeSpan.TryParse(txtLocalFoodToTime.Text, out toTimeSpan);

            if (fromParsed && toParsed)
            {
                // Calculate total hours worked
                double totalHoursWorked = (toTimeSpan - fromTimeSpan).TotalHours;

                // Handle crossing over midnight
                if (totalHoursWorked < 0)
                {
                    totalHoursWorked += 24; // Assume toTime is on the next day
                }

                // Add to accumulated hours
                totalAccumulatedHours += totalHoursWorked;

                // Determine amount based on accumulated hours worked
                if (totalAccumulatedHours >= 8)
                {
                    txtLocalFoodAmount.Text = "300"; // Amount for 12 hours or more
                }
                else
                {
                    txtLocalFoodAmount.Text = "150"; // Amount for less than 12 hours
                }
            }
            else
            {
                txtLocalFoodAmount.Text = ""; // Clear amount field for invalid input
            }
        }

        protected void ddlLocalMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide all panels first
            pnlBikeFields.Visible = false;
            pnlCabFields.Visible = false;
            pnlAutoFields.Visible = false;

            // Show the selected panel
            switch (ddlLocalMode.SelectedValue)
            {
                case "Bike":
                    pnlBikeFields.Visible = true;
                    break;
                case "Cab/Bus":
                    pnlCabFields.Visible = true;
                    break;
                case "Auto":
                    pnlAutoFields.Visible = true;
                    break;
                default:
                    // Optionally handle the case when no valid selection is made
                    break;
            }
        }
        protected void txtLocalDistance_TextChanged(object sender, EventArgs e)
        {
            if (ddlLocalMode.SelectedValue == "Bike")
            {
                CalculateAmount(txtLocalDistance.Text, 13.5, txtLocalAmount); // Example rate for bike
            }
            else if (ddlLocalMode.SelectedValue == "Auto")
            {
                CalculateAmount(txtLocalAutoDistance.Text, 13.5, txtLocalAutoAmount); // Rate for auto
            }
        }

        private void CalculateAmount(string distanceInput, double ratePerKm, TextBox amountTextBox)
        {
            if (double.TryParse(distanceInput, out double distance))
            {
                double amount = distance * ratePerKm;
                amountTextBox.Text = amount.ToString("0.00");
            }
            else
            {
                amountTextBox.Text = "0.00"; // Reset if input is invalid
            }
        }

        protected void ddlTourExpenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int serviceId = GetServiceId(); // Implement this method to get the current ServiceId
            DisplayExpenses(serviceId);

            string selectedValue = ddlTourExpenseType.SelectedValue;

            // Hide all tour expense panels initially
            pnlTourFoodFields.Visible = false;
            pnlTourMiscellaneousFields.Visible = false;
            pnlTourOthersFields.Visible = false;
            pnlTourConvenience.Visible = false;

            // Show panels based on selected tour expense type
            if (selectedValue == "Food")
            {
                pnlTourFoodFields.Visible = true;
            }
            else if (selectedValue == "Miscellaneous")
            {
                pnlTourMiscellaneousFields.Visible = true;
            }
            else if (selectedValue == "Lodging")
            {
                pnlTourOthersFields.Visible = true;
            }
            else if (selectedValue == "Conveyance")
            {
                pnlTourConvenience.Visible = true;
                ddlTourTransportMode.Enabled = true; // Enable Tour mode dropdown
            }
            else
            {
                // Hide all panels if no valid selection
                pnlTourFoodFields.Visible = false;
                pnlTourMiscellaneousFields.Visible = false;
                pnlTourOthersFields.Visible = false;
                pnlTourConvenience.Visible = false;
            }
        }
        protected void txtTourFoodFromTime_TextChanged(object sender, EventArgs e)
        {
            CalculateTourFoodAmount();
        }

        protected void ddlTourFoodDesignation_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateTourFoodAmount();
        }

        protected void txtTourFoodToTime_TextChanged(object sender, EventArgs e)
        {
            CalculateTourFoodAmount();
        }

        private void CalculateTourFoodAmount()
        {
            string selectedValue = txtTourFoodDesignation.SelectedValue;
            double designatedAmount = 0;

            // Set the designated amount based on the selected designation
            switch (selectedValue)
            {
                case "FSE":
                    designatedAmount = 1000;
                    break;
                case "FST":
                    designatedAmount = 850;
                    break;
                default:
                    txtTourFoodAmount.Text = "Select a valid designation";
                    return;
            }

            // Check if From and To times are provided
            if (!string.IsNullOrWhiteSpace(txtTourFoodFromTime.Text) && !string.IsNullOrWhiteSpace(txtTourFoodToTime.Text))
            {
                DateTime fromTime;
                DateTime toTime;

                // Attempt to parse the time values directly from the TextBoxes
                bool fromParsed = DateTime.TryParse(txtTourFoodFromTime.Text, out fromTime);
                bool toParsed = DateTime.TryParse(txtTourFoodToTime.Text, out toTime);

                // Check if parsing failed
                if (!fromParsed || !toParsed)
                {
                    txtTourFoodAmount.Text = "Invalid time format. Use hh:mm AM/PM.";
                    return;
                }

                // Handle crossing over midnight
                if (toTime < fromTime)
                {
                    toTime = toTime.AddDays(1); // Assume toTime is on the next day
                }

                // Calculate total hours worked
                double totalHoursWorked = (toTime - fromTime).TotalHours;
                double amountToDisplay = 0;

                // Determine amount based on total hours worked and designation
                if (totalHoursWorked <= 8)
                {
                    if (selectedValue == "FSE")
                    {
                        amountToDisplay = 500; // Half amount for FSE
                    }
                    else if (selectedValue == "FST")
                    {
                        amountToDisplay = 425; // Half amount for FST
                    }
                }
                else
                {
                    amountToDisplay = designatedAmount; // Full amounts for 12 hours or more
                }

                // Set the amount in the text box
                txtTourFoodAmount.Text = amountToDisplay > 0 ? amountToDisplay.ToString("F2") : string.Empty;
            }
            else
            {
                txtTourFoodAmount.Text = "Enter both From and To times.";
            }
        }

        protected void ddlTourTransportMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMode = ddlTourTransportMode.SelectedValue;

            // Hide all transport panels initially
            pnlFlightFields.Visible = false;
            pnlBusFields.Visible = false;
            pnlTrainFields.Visible = false;
            pnlcabTourFields.Visible = false;
            pnlAutoTourFields.Visible = false;

            // Show the selected transport panel
            switch (selectedMode)
            {
                case "Flight":
                    pnlFlightFields.Visible = true;
                    break;
                case "Bus":
                    pnlBusFields.Visible = true;
                    break;
                case "Train":
                    pnlTrainFields.Visible = true;
                    break;
                case "Cab":
                    pnlcabTourFields.Visible = true;
                    break;
                case "Auto":
                    pnlAutoTourFields.Visible = true;
                    break;
                default:
                    break;
            }
        }
        protected void txtAutoTourDistance_TextChanged(object sender, EventArgs e)
        {
            // Assuming a rate of 13.5 per km for Auto
            CalculateTourAmount(txtTourAutoDistance.Text, 13.5, txtTourAutoAmount);
        }

        private void CalculateTourAmount(string distanceInput, double ratePerKm, TextBox amountTextBox)
        {
            if (double.TryParse(distanceInput, out double distance))
            {
                double amount = distance * ratePerKm;
                amountTextBox.Text = amount.ToString("0.00");
            }
            else
            {
                amountTextBox.Text = "0.00"; // Reset if input is invalid
            }
        }
        protected void txtLocalMiscItem_TextChanged(object sender, EventArgs e)
        {
            // Example logic: Display the text of the TextBox in a label (or any other logic you need)
            // lblMiscItem.Text = txtLocalMiscItem.Text;
        }


        private decimal GetAdvanceAmount(SqlConnection con, SqlTransaction transaction, int serviceId, int employeeId)
        {
            string getAdvanceSql = @"
        SELECT Advance
        FROM Services 
        WHERE ServiceId = @ServiceId AND EmployeeId = @EmployeeId";

            using (SqlCommand cmdGetAdvance = new SqlCommand(getAdvanceSql, con, transaction))
            {
                cmdGetAdvance.Parameters.AddWithValue("@ServiceId", serviceId);
                cmdGetAdvance.Parameters.AddWithValue("@EmployeeId", employeeId);
                object advanceResult = cmdGetAdvance.ExecuteScalar();

                return advanceResult != null ? Convert.ToDecimal(advanceResult) : 0;
            }
        }

        private bool ValidateTourExpenses()
        {
            bool isValid = true;
            string errorMessage = "";

            // Validate Tour Lodging
            if (!string.IsNullOrEmpty(txtTourOthersAmount.Text))
            {
                if (string.IsNullOrEmpty(txtTourOthersDate.Text))
                {
                    errorMessage += "Tour Lodging Date is required.\n";
                    isValid = false;
                }

                if (string.IsNullOrEmpty(txtTourOthersSmoNo.Text) || string.IsNullOrEmpty(txtTourOthersRefNo.Text) || string.IsNullOrEmpty(txtTourOthersSoNo.Text))
                {
                    errorMessage += "SMO No, Ref No, and SO No are required for Tour Lodging.\n";
                    isValid = false;
                }
            }

            // Validate Tour Miscellaneous
            if (!string.IsNullOrEmpty(txtTourMiscItem.Text))
            {
                if (string.IsNullOrEmpty(txtTourMiscAmount.Text))
                {
                    errorMessage += "Tour Miscellaneous Amount is required.\n";
                    isValid = false;
                }

                if (string.IsNullOrEmpty(txtTourMiscDate.Text))
                {
                    errorMessage += "Tour Miscellaneous Date is required.\n";
                    isValid = false;
                }

                if (string.IsNullOrEmpty(txtTourMiscSmoNo.Text) || string.IsNullOrEmpty(txtTourMiscRefNo.Text) || string.IsNullOrEmpty(txtTourMiscSoNo.Text))
                {
                    errorMessage += "SMO No, Ref No, and SO No are required for Tour Miscellaneous.\n";
                    isValid = false;
                }
            }

            // Validate Tour Food
            if (!string.IsNullOrEmpty(txtTourFoodAmount.Text))
            {
                if (string.IsNullOrEmpty(txtTourFoodDate.Text))
                {
                    errorMessage += "Tour Food Date is required.\n";
                    isValid = false;
                }

                if (string.IsNullOrEmpty(txtTourFoodSMONo.Text) || string.IsNullOrEmpty(txtTourFoodRefNo.Text) || string.IsNullOrEmpty(txtTourFoodSONo.Text))
                {
                    errorMessage += "SMO No, Ref No, and SO No are required for Tour Food.\n";
                    isValid = false;
                }
            }

            // Validate Tour Conveyance (Cab, Train, Flight, Bus, Auto)
            if (ddlTourTransportMode.SelectedValue == "Cab" && string.IsNullOrEmpty(txtCabAmount.Text))
            {
                errorMessage += "Cab Amount is required.\n";
                isValid = false;
            }
            else if (ddlTourTransportMode.SelectedValue == "Train" && string.IsNullOrEmpty(txtTrainAmount.Text))
            {
                errorMessage += "Train Amount is required.\n";
                isValid = false;
            }
            else if (ddlTourTransportMode.SelectedValue == "Flight" && string.IsNullOrEmpty(txtFlightAmount.Text))
            {
                errorMessage += "Flight Amount is required.\n";
                isValid = false;
            }
            else if (ddlTourTransportMode.SelectedValue == "Bus" && string.IsNullOrEmpty(txtBusAmount.Text))
            {
                errorMessage += "Bus Amount is required.\n";
                isValid = false;
            }
            else if (ddlTourTransportMode.SelectedValue == "Auto" && string.IsNullOrEmpty(txtTourAutoAmount.Text))
            {
                errorMessage += "Auto Amount is required.\n";
                isValid = false;
            }

            // Show the error message if there are validation issues
            if (!isValid)
            {
                lblError.Text = errorMessage;
            }

            return isValid;
        }


        // Method to validate local fields


        // Method to register the script for the client-side message
        private void ShowMessage(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "Message", script, true);
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string scriptMessage = ""; // For passing messages to the client-side
            try
            {
                lblError.Visible = false;  // Do not show the error message in the label
                lblError.Text = ""; // Clear previous error messages

                if (Session["ServiceId"] == null)
                {
                    scriptMessage = "Invalid Service ID.";
                    ShowMessage(scriptMessage);  // Show error message if Service ID is not found
                    return;
                }

                int serviceId = (int)Session["ServiceId"];
                const int maxRetries = 3;
                int retryCount = 0;
                string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    while (retryCount < maxRetries)
                    {
                        try
                        {
                            con.Open();
                            break;
                        }
                        catch (SqlException)
                        {
                            retryCount++;
                            Thread.Sleep(1000);
                        }
                    }

                    if (retryCount == maxRetries)
                    {
                        scriptMessage = "Failed to connect to database after retries.";
                        ShowMessage(scriptMessage);  // Show error message if database connection fails
                        return;
                    }

                    // Retrieve EmployeeId and StatusId based on ServiceId
                    string getServiceStatusSql = @"
            SELECT EmployeeId, StatusId
            FROM Services
            WHERE ServiceId = @ServiceId";

                    using (SqlCommand cmdGetServiceStatus = new SqlCommand(getServiceStatusSql, con))
                    {
                        cmdGetServiceStatus.Parameters.AddWithValue("@ServiceId", serviceId);
                        using (SqlDataReader reader = cmdGetServiceStatus.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Safely check and cast EmployeeId
                                if (reader["EmployeeId"] != DBNull.Value)
                                {
                                    // employeeId = Convert.ToInt32(reader["EmployeeId"]); // Removed as per instruction
                                }
                                else
                                {
                                    scriptMessage = "EmployeeId not found.";
                                    ShowMessage(scriptMessage);  // Show error message if EmployeeId is NULL
                                    return;
                                }

                                // Safely check and cast StatusId
                                if (reader["StatusId"] != DBNull.Value)
                                {
                                    int statusId = Convert.ToInt32(reader["StatusId"]);

                                    // Check if the status is already "Reimbursement Submitted" (Status ID = 2)
                                    if (statusId == 2)  // Status ID 2 corresponds to "Reimbursement Submitted"
                                    {
                                        scriptMessage = "Reimbursement already submitted. Status cannot be changed again.";
                                        ShowMessage(scriptMessage);  // Show error message if already submitted
                                        return;
                                    }
                                }
                                else
                                {
                                    scriptMessage = "StatusId not found.";
                                    ShowMessage(scriptMessage);  // Show error message if StatusId is NULL
                                    return;
                                }
                            }
                            else
                            {
                                scriptMessage = "Service ID not found.";
                                ShowMessage(scriptMessage);  // Show error message if Service ID is not found
                                return;
                            }
                        }
                    }

                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            if (btnSubmit.Text == "Update")
                            {
                                UpdateExpensesEntry(con, transaction, serviceId);
                                scriptMessage = "Data updated successfully.";

                                // Reset Edit state
                                hdnEditRecordId.Value = "";
                                hdnEditCategory.Value = "";
                                btnSubmit.Text = "Save";
                            }
                            else
                            {
                                // Insert expenses
                                InsertExpenses(con, transaction, serviceId);
                                scriptMessage = "Data saved successfully.";
                            }

                            // Commit the transaction
                            transaction.Commit();

                            // Display the expenses (calculate and show the overall total)
                            DisplayExpenses(serviceId);

                            // Show success message in an alert
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "showAlert", "alert('" + scriptMessage + "');", true);

                            // Clear fields after success
                            ClearExpenseFields();

                            // Refresh page to reset everything
                            Response.Redirect(Request.Url.ToString());
                        }
                        catch (SqlException sqlEx)
                        {
                            try { transaction.Rollback(); } catch (InvalidOperationException) { }

                            if (sqlEx.Number == 2627)
                            {
                                scriptMessage = "Refreshment is already added for the specified date range.";
                            }
                            else
                            {
                                scriptMessage = $"SQL Error: {sqlEx.Message}";
                            }
                            ShowMessage(scriptMessage);  // Show SQL error message
                        }
                        catch (Exception ex)
                        {
                            try { transaction.Rollback(); } catch (InvalidOperationException) { }

                            scriptMessage = $"Error: {ex.Message}";
                            ShowMessage(scriptMessage);  // Show unexpected error message
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                scriptMessage = $"Unexpected error: {ex.Message}";
                ShowMessage(scriptMessage);  // Show error message for unexpected errors
            }
        }

        private void UpdateExpensesEntry(SqlConnection con, SqlTransaction transaction, int serviceId)
        {
            if (string.IsNullOrEmpty(hdnEditRecordId.Value)) return;

            int id = Convert.ToInt32(hdnEditRecordId.Value);
            string category = hdnEditCategory.Value;
            string expenseType = ddlExpenseType.SelectedValue;

            if (expenseType == "Local")
            {
                string localType = ddlLocalExpenseType.SelectedValue;
                if (localType == "Food")
                {
                    UpdateFoodFull(con, transaction, id, txtLocalFoodAmount.Text, txtLocalFoodDate.Text, txtLocalFoodFromTime.Text, txtLocalFoodToTime.Text, txtLocalFoodParticulars.Text, txtLocalFoodRemarks.Text, txtLocalSMONo.Text, txtLocalRefNo.Text, txtLocalFoodSONo.Text, null);
                }
                else if (localType == "Miscellaneous")
                {
                    UpdateMiscellaneousFull(con, transaction, id, txtLocalMiscAmount.Text, txtLocalMiscDate.Text, fileUploadLocalMiscellaneous, txtLocalMiscFromTime.Text, txtLocalMiscToTime.Text, txtLocalMiscItem.Text, txtLocalMiscRemarks.Text, txtLocalMiscSMONo.Text, txtLocalMiscRefNo.Text, txtLocalMiscSONo.Text);
                }
                else if (localType == "Others")
                {
                    UpdateOthersFull(con, transaction, id, txtLocalOthersAmount.Text, txtLocalOthersDate.Text, txtLocalOthersFromTime.Text, txtLocalOthersToTime.Text, txtLocalOthersParticulars.Text, txtLocalOthersRemarks.Text, fileUploadLocalBill, othersfileUploadApproval, fileServiceReport, txtLocalOthersSMONo.Text, txtLocalOthersRefNo.Text, txtLocalOthersSoNo.Text);
                }
                else if (localType == "Conveyance")
                {
                    string transportMode = ddlLocalMode.SelectedValue;
                    if (transportMode == "Bike")
                    {
                        UpdateConveyanceFull(con, transaction, id, transportMode, txtLocalAmount.Text, txtLocalBikeDate.Text, txtLocalBikeFromTime.Text, txtLocalBikeToTime.Text, txtLocalBikeParticular.Text, txtLocalBikeRemarks.Text, null, txtLocalDistance.Text, txtLocalBikeSMONo.Text, txtLocalBikeRefNo.Text, txtLocalBikeSONo.Text);
                    }
                    else if (transportMode == "Cab/Bus")
                    {
                        UpdateConveyanceFull(con, transaction, id, transportMode, txtLocalCabAmount.Text, txtLocalCabDate.Text, txtLocalCabFromTime.Text, txtLocalCabToTime.Text, txtLocalCabParticular.Text, txtLocalCabRemarks.Text, fileUploadLocalCab, null, txtLocalCabSMONo.Text, txtLocalCabRefNo.Text, txtLocalCabSONo.Text);
                    }
                    else if (transportMode == "Auto")
                    {
                        UpdateConveyanceFull(con, transaction, id, transportMode, txtLocalAutoAmount.Text, txtLocalAutoDate.Text, txtLocalAutoFromTime.Text, txtLocalAutoToTime.Text, txtLocalAutoParticular.Text, txtLocalAutoRemarks.Text, txtfileUploadLocalAuto, txtLocalAutoDistance.Text, txtLocalAutoSMONo.Text, txtLocalAutoRefNo.Text, txtLocalAutoSONo.Text);
                    }
                }
            }
            else if (expenseType == "Tour")
            {
                string tourType = ddlTourExpenseType.SelectedValue;
                if (tourType == "Food")
                {
                    UpdateFoodFull(con, transaction, id, txtTourFoodAmount.Text, txtTourFoodDate.Text, txtTourFoodFromTime.Text, txtTourFoodToTime.Text, txtTourFoodParticulars.Text, txtTourFoodRemarks.Text, txtTourFoodSMONo.Text, txtTourFoodRefNo.Text, txtTourFoodSONo.Text, txtTourFoodDesignation.SelectedValue);
                }
                else if (tourType == "Miscellaneous")
                {
                    UpdateMiscellaneousFull(con, transaction, id, txtTourMiscAmount.Text, txtTourMiscDate.Text, fileUploadTourMiscellaneous, txtTourMiscFromTime.Text, txtTourMiscToTime.Text, txtTourMiscParticulars.Text, txtTourMiscRemarks.Text, txtTourMiscSmoNo.Text, txtTourMiscRefNo.Text, txtTourMiscSoNo.Text);
                }
                else if (tourType == "Lodging")
                {
                    UpdateLodgingFull(con, transaction, id, txtTourOthersAmount.Text, txtTourOthersDate.Text, txtFromTimeTourOthers.Text, txtToTimeTourOthers.Text, txtParticularsTourOthers.Text, txtRemarksTourOthers.Text, fileUploadTourOthers, fileUploadTourApproval, fileUploadServiceReport, txtTourOthersSmoNo.Text, txtTourOthersRefNo.Text, txtTourOthersSoNo.Text);
                }
                else if (tourType == "Conveyance")
                {
                    string transportMode = ddlTourTransportMode.SelectedValue;
                    if (transportMode == "Cab")
                    {
                        UpdateConveyanceFull(con, transaction, id, transportMode, txtCabAmount.Text, txtCabDate.Text, txtFromTimeCab.Text, txtToTimeCab.Text, txtParticularsCab.Text, txtRemarksCab.Text, fileUploadCab, null, txtCabSmoNo.Text, txtCabRefNo.Text, txtCabSoNo.Text);
                    }
                    else if (transportMode == "Train")
                    {
                        UpdateConveyanceFull(con, transaction, id, transportMode, txtTrainAmount.Text, txtTrainDate.Text, txtFromTimeTrain.Text, txtToTimeTrain.Text, txtParticularsTrain.Text, txtRemarksTrain.Text, fileUploadTrain, null, txtTrainSmoNo.Text, txtTrainRefNo.Text, txtTrainSoNo.Text);
                    }
                    else if (transportMode == "Flight")
                    {
                        UpdateConveyanceFull(con, transaction, id, transportMode, txtFlightAmount.Text, txtFlightDate.Text, txtFlightFromTime.Text, txtFlightToTime.Text, txtFlightParticulars.Text, txtFlightRemarks.Text, fileUploadFlight, null, txtFlightSmoNo.Text, txtFlightRefNo.Text, txtFlightSoNo.Text);
                    }
                    else if (transportMode == "Bus")
                    {
                        UpdateConveyanceFull(con, transaction, id, transportMode, txtBusAmount.Text, txtBusDate.Text, txtFromTimeBus.Text, txtToTimeBus.Text, txtParticularsBus.Text, txtRemarksBus.Text, fileUploadBus, null, txtBusSmoNo.Text, txtBusRefNo.Text, txtBusSoNo.Text);
                    }
                    else if (transportMode == "Auto")
                    {
                        UpdateConveyanceFull(con, transaction, id, transportMode, txtTourAutoAmount.Text, txtTourAutoDate.Text, txtTourAutoFromTime.Text, txtTourAutoToTime.Text, txtTourAutoParticular.Text, txTourAutoRemarks.Text, fileUploadTourAuto, txtTourAutoDistance.Text, txtTourAutoSmoNo.Text, txtTourAutoRefNo.Text, txtTourAutoSoNo.Text);
                    }
                }
            }
        }

        protected void btnChangeStatus_Click(object sender, EventArgs e)
        {
            lblStatusError.Text = "";
            if (Session["ServiceId"] == null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "err", "alert('Invalid Service ID.');", true);
                return;
            }

            int serviceId = (int)Session["ServiceId"];
            int employeeId;

            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                // ✅ Check CURRENT status — allow resubmit if NOT 3
                string getStatusSql = "SELECT StatusId FROM Services WHERE ServiceId = @ServiceId";
                using (SqlCommand cmd = new SqlCommand(getStatusSql, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    var currentStatus = cmd.ExecuteScalar();
                    int currentStatusId = currentStatus != null ? Convert.ToInt32(currentStatus) : 0;

                    // ❌ Block only if already VERIFIED (status = 3)
                    if (currentStatusId == 3)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "err", "alert('Expense is already verified. No further changes allowed.');", true);
                        return;
                    }
                }

                // ✅ Proceed: get employeeId
                string getEmpSql = "SELECT EmployeeId FROM Services WHERE ServiceId = @ServiceId";
                using (SqlCommand cmd = new SqlCommand(getEmpSql, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    employeeId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // ✅ SET STATUS = 2 ("Reimbursement Submitted")
                        UpdateStatus(transaction, serviceId, employeeId, 2, "Reimbursement Submitted");

                        transaction.Commit();

                        // 🔒 Disable buttons AFTER successful submit

                        ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('Reimbursement submitted successfully.');", true);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ClientScript.RegisterStartupScript(this.GetType(), "err", $"alert('Error: {ex.Message.Replace("'", "\\'")}');", true);
                    }
                }
            }
        }
        private void EnableEntireForm()
        {
            // Re-enable dropdowns
            ddlExpenseType.Enabled = true;
            ddlLocalExpenseType.Enabled = true;
            ddlTourExpenseType.Enabled = true;
            ddlAwardExpenseType.Enabled = true;
            ddlLocalMode.Enabled = true;
            ddlTourTransportMode.Enabled = true;
            txtTourFoodDesignation.Enabled = true;

            // Re-enable all textboxes and file uploads
            EnableAllTextboxes(Page);

            // Re-enable submit buttons
            btnSubmit.Enabled = true;
            btnChangeStatus.Enabled = true;
            btnCancel.Enabled = true;

            // Re-enable edit/delete in GridViews
            EnableEditAndDeleteButtons();
        }

        private void EnableAllTextboxes(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox tb) tb.Enabled = true;
                else if (ctrl is FileUpload fu) fu.Enabled = true;
                else if (ctrl.HasControls()) EnableAllTextboxes(ctrl);
            }
        }

        private void EnableEditAndDeleteButtons()
        {
            EnableGridViewButtons(GridViewConveyance, "btnEditConveyance", "btnDelete");
            EnableGridViewButtons(GridViewFood, "btnEditFood", "btnDeleteFood");
            EnableGridViewButtons(GridViewOthers, "btnEditOthers", "btnDeleteOthers");
            EnableGridViewButtons(GridViewMiscellaneous, "btnEditMiscellaneous", "btnDeleteMiscellaneous");
            EnableGridViewButtons(GridViewLodging, "btnEditLodging", "btnDeleteLodging");
        }

        private void EnableGridViewButtons(GridView gv, string editBtnId, string deleteBtnId)
        {
            foreach (GridViewRow row in gv.Rows)
            {
                Button btnEdit = (Button)row.FindControl(editBtnId);
                Button btnDelete = (Button)row.FindControl(deleteBtnId);
                if (btnEdit != null) btnEdit.Enabled = true;
                if (btnDelete != null) btnDelete.Enabled = true;
            }
        }
        // Method to disable the buttons
        private void DisableButtons()
        {
            btnChangeStatus.Enabled = false;

            foreach (GridViewRow row in GridViewConveyance.Rows)
            {
                Button btnEdit = (Button)row.FindControl("btnEditConveyance");
                Button btnDelete = (Button)row.FindControl("btnDelete");

                if (btnEdit != null) btnEdit.Enabled = false;
                if (btnDelete != null) btnDelete.Enabled = false;
            }

            foreach (GridViewRow row in GridViewFood.Rows)
            {
                Button btnEditFood = (Button)row.FindControl("btnEditFood");
                Button btnDeleteFood = (Button)row.FindControl("btnDeleteFood");

                if (btnEditFood != null) btnEditFood.Enabled = false;
                if (btnDeleteFood != null) btnDeleteFood.Enabled = false;
            }

            foreach (GridViewRow row in GridViewOthers.Rows)
            {
                Button btnEditOthers = (Button)row.FindControl("btnEditOthers");
                Button btnDeleteOthers = (Button)row.FindControl("btnDeleteOthers");

                if (btnEditOthers != null) btnEditOthers.Enabled = false;
                if (btnDeleteOthers != null) btnDeleteOthers.Enabled = false;
            }

            foreach (GridViewRow row in GridViewMiscellaneous.Rows)
            {
                Button btnEditMiscellaneous = (Button)row.FindControl("btnEditMiscellaneous");
                Button btnDeleteMiscellaneous = (Button)row.FindControl("btnDeleteMiscellaneous");

                if (btnEditMiscellaneous != null) btnEditMiscellaneous.Enabled = false;
                if (btnDeleteMiscellaneous != null) btnDeleteMiscellaneous.Enabled = false;
            }

            foreach (GridViewRow row in GridViewLodging.Rows)
            {
                Button btnEditLodging = (Button)row.FindControl("btnEditLodging");
                Button btnDeleteLodging = (Button)row.FindControl("btnDeleteLodging");

                if (btnEditLodging != null) btnEditLodging.Enabled = false;
                if (btnDeleteLodging != null) btnDeleteLodging.Enabled = false;
            }
        }





        private void InsertExpenses(SqlConnection con, SqlTransaction transaction, int serviceId)
        {
            if (ddlExpenseType.SelectedValue == "Award")
            {
                if (!string.IsNullOrEmpty(txtAwardAmount.Text))
                {
                    // Get the award type (e.g., "Star Award" or "Spot Award")
                    string awardType = ddlAwardExpenseType.SelectedValue;

                    // Convert file upload to byte array for the bill
                    //byte[] fileUploadAwardBillBytes = null;
                    //if (fileUploadAwardBill.HasFile)
                    //{
                    //    fileUploadAwardBillBytes = GetFileBytes(fileUploadAwardBill); // Convert FileUpload control to byte array
                    //}

                    // Call InsertStarAwardExpense with all required parameters
                    InsertStarAwardExpense(
                        con,
                        transaction,
                        serviceId,
                        txtAwardAmount.Text,               // Amount of the award
                        txtAwardDate.Text,                 // Date of the award
                        awardType                        // Type of award (e.g., "Star Award")
                                                         //txtAwardParticulars.Text ?? "",    // Particulars/description of the award (empty string if null)
                                                         // txtAwardRemarks.Text ?? "",        // Remarks (empty string if null)
                                                         // fileUploadAwardBillBytes           // Uploaded bill as byte array
                    );
                }
            }

            // Inserting logic for expenses based on type
            if (ddlExpenseType.SelectedValue == "Local")
            {
                if (!string.IsNullOrEmpty(txtLocalOthersAmount.Text))
                {
                    // Get SMO No, Ref No, and SO No values from their respective input fields
                    string smoNo = txtLocalOthersSMONo.Text;
                    string refNo = txtLocalOthersRefNo.Text;
                    string soNo = txtLocalOthersSoNo.Text;

                    // Convert file upload to byte array
                    byte[] fileUploadLocalBillBytes = null;
                    if (fileUploadLocalBill.HasFile)
                    {
                        fileUploadLocalBillBytes = GetFileBytes(fileUploadLocalBill); // Convert FileUpload control to byte array
                    }

                    byte[] serviceReport = null;
                    if (fileServiceReport.HasFile)
                    {
                        // Convert the uploaded file into byte array (service report)
                        serviceReport = GetFileBytes(fileServiceReport);
                    }

                    // Call InsertOthersExpense with the serviceReport byte array
                    InsertOthersExpense(
                        con,
                        transaction,
                        serviceId,
                        txtLocalOthersAmount.Text,
                        txtLocalOthersDate.Text,
                        txtLocalOthersFromTime.Text,
                        txtLocalOthersToTime.Text,
                        txtLocalOthersParticulars.Text,
                        txtLocalOthersRemarks.Text,
                        fileUploadLocalBillBytes,   // Pass the byte array
                        othersfileUploadApproval,   // Pass the approval file upload control
                        smoNo,
                        refNo,
                        soNo,
                        serviceReport            // Pass the service report as byte array
                    );
                }







                if (!string.IsNullOrEmpty(txtLocalMiscAmount.Text))
                {
                    // Parse times from the input fields - use default values if empty
                    TimeSpan fromTime = TimeSpan.Zero;
                    TimeSpan toTime = TimeSpan.Zero;

                    if (!string.IsNullOrEmpty(txtLocalMiscFromTime.Text))
                    {
                        TimeSpan.TryParse(txtLocalMiscFromTime.Text, out fromTime);
                    }

                    if (!string.IsNullOrEmpty(txtLocalMiscToTime.Text))
                    {
                        TimeSpan.TryParse(txtLocalMiscToTime.Text, out toTime);
                    }

                    // Retrieve the values for SMO No, Ref No, and SO No from the input fields
                    string smoNo = txtLocalMiscSMONo.Text; // New SMO No input
                    string refNo = txtLocalMiscRefNo.Text; // New Ref No input
                    string soNo = txtLocalMiscSONo.Text;   // SO No input

                    // Assuming 'fileUploadLocalMiscellaneous' is for the bill, and 'fileUploadApproval' is for the approval mail
                    InsertMiscellaneousExpense(
                        con,
                        transaction,
                        serviceId,
                        txtLocalMiscItem.Text,                     // Purchased Item
                        txtLocalMiscAmount.Text,                   // Amount
                        txtLocalMiscDate.Text,                     // Date
                        fileUploadLocalMiscellaneous,              // File upload for the bill
                        "Local",                                   // Expense Type
                        fromTime,                                  // From Time
                        toTime,                                    // To Time
                        txtLocalMiscParticulars.Text,              // Particulars
                        txtLocalMiscRemarks.Text,                  // Remarks
                        smoNo,                                     // SMO No
                        refNo,                                     // Ref No
                        soNo                                       // SO No
                    );
                }


                if (!string.IsNullOrEmpty(txtLocalFoodAmount.Text))
                {
                    TimeSpan? fromTime = string.IsNullOrEmpty(txtLocalFoodFromTime.Text) ? (TimeSpan?)null : TimeSpan.Parse(txtLocalFoodFromTime.Text);
                    TimeSpan? toTime = string.IsNullOrEmpty(txtLocalFoodToTime.Text) ? (TimeSpan?)null : TimeSpan.Parse(txtLocalFoodToTime.Text);

                    // Assuming you have text boxes for SMO No, Ref No, and SONO
                    string smoNo = txtLocalSMONo.Text; // Change this to the actual control ID
                    string refNo = txtLocalRefNo.Text; // Change this to the actual control ID
                    string soNo = txtLocalFoodSONo.Text; // Add this line to get SONO value

                    InsertFoodExpense(
                        con,
                        transaction,
                        serviceId,
                        "Local",
                        txtLocalFoodAmount.Text,
                        txtLocalFoodDate.Text,
                        DBNull.Value,
                        fromTime,
                        toTime,
                        txtLocalFoodParticulars.Text,
                        txtLocalFoodRemarks.Text,
                        smoNo,
                        refNo,
                        soNo // Pass the SONO value here
                    );
                }

                if (ddlLocalMode.SelectedValue == "Bike")
                {
                    string bikeDistance = txtLocalDistance.Text;  // Capture distance for Bike

                    // Ensure the date is parsed properly
                    DateTime bikeDate;
                    if (DateTime.TryParse(txtLocalBikeDate.Text, out bikeDate))
                    {
                        // Format the DateTime to string in the desired format (e.g., yyyy-MM-dd)
                        string formattedBikeDate = bikeDate.ToString("yyyy-MM-dd");

                        // Insert using the formatted date string
                        InsertConveyanceExpense(con, transaction, serviceId, "Bike", txtLocalAmount.Text, formattedBikeDate,
                                                txtLocalBikeFromTime.Text, txtLocalBikeToTime.Text, txtLocalBikeParticular.Text,
                                                txtLocalBikeRemarks.Text, null, "Local", bikeDistance,
                                                txtLocalBikeSMONo.Text, txtLocalBikeRefNo.Text, txtLocalBikeSONo.Text);
                    }
                    else
                    {
                        // Handle invalid date input
                        Console.WriteLine("Invalid date format entered.");
                        // Optionally, show an error message to the user.
                        lblError.Text = "Please enter a valid date.";
                    }
                }



                else if (ddlLocalMode.SelectedValue == "Cab/Bus")
                {
                    InsertConveyanceExpense(con, transaction, serviceId, "Cab/Bus", txtLocalCabAmount.Text, txtLocalCabDate.Text,
                                            txtLocalCabFromTime.Text, txtLocalCabToTime.Text, txtLocalCabParticular.Text, txtLocalCabRemarks.Text,
                                            fileUploadLocalCab, "Local", null,
                                            txtLocalCabSMONo.Text, txtLocalCabRefNo.Text, txtLocalCabSONo.Text); // Pass SMO No, Ref No, and SO No
                }
                else if (ddlLocalMode.SelectedValue == "Auto" && !string.IsNullOrEmpty(txtLocalAutoAmount.Text))
                {
                    decimal autoAmount = Convert.ToDecimal(txtLocalAutoAmount.Text);
                    string autoDistance = txtLocalAutoDistance.Text; // Capture distance for Auto
                    InsertConveyanceExpense(con, transaction, serviceId, "Auto", autoAmount.ToString(), txtLocalAutoDate.Text,
                                            txtLocalAutoFromTime.Text, txtLocalAutoToTime.Text, txtLocalAutoParticular.Text,
                                            txtLocalAutoRemarks.Text, txtfileUploadLocalAuto, "Local", autoDistance,
                                            txtLocalAutoSMONo.Text, txtLocalAutoRefNo.Text, txtLocalAutoSONo.Text); // Pass SMO No, Ref No, and SO No
                }

            }


            else if (ddlExpenseType.SelectedValue == "Tour")
            {
                if (!string.IsNullOrEmpty(txtTourOthersAmount.Text))
                {
                    // Get SMO No, Ref No, and SO No values from the text boxes
                    string smoNo = txtTourOthersSmoNo.Text;
                    string refNo = txtTourOthersRefNo.Text;
                    string soNo = txtTourOthersSoNo.Text;

                    // Get the byte array for the Service Report file
                    byte[] serviceReport = null;
                    if (fileUploadServiceReport.HasFile)
                    {
                        serviceReport = GetFileBytes(fileUploadServiceReport);
                    }

                    // Call InsertLodgingExpense method
                    InsertLodgingExpense(
                        con,
                        transaction,
                        serviceId,
                        txtTourOthersAmount.Text,
                        txtTourOthersDate.Text,
                        txtFromTimeTourOthers.Text,
                        txtToTimeTourOthers.Text,
                        txtParticularsTourOthers.Text,
                        txtRemarksTourOthers.Text,
                        fileUploadTourOthers,
                         fileUploadTourApproval,// File Upload (for bill or image)
                        smoNo,                // SMO No
                        refNo,                // Ref No
                        soNo,                 // SO No
                        serviceReport         // Service Report byte array
                    );
                }



                if (!string.IsNullOrEmpty(txtTourMiscItem.Text))
                {
                    // Parse the from and to times from the TextBox controls
                    TimeSpan fromTime = TimeSpan.Parse(txtTourMiscFromTime.Text);
                    TimeSpan toTime = TimeSpan.Parse(txtTourMiscToTime.Text);

                    // Retrieve values for SMO No, Ref No, and SO No
                    string smoNo = txtTourMiscSmoNo.Text; // SMO No input
                    string refNo = txtTourMiscRefNo.Text; // Ref No input
                    string soNo = txtTourMiscSoNo.Text;   // SO No input

                    // Call the InsertMiscellaneousExpense method, passing all required parameters including the approval mail file upload
                    InsertMiscellaneousExpense(
                        con,
                        transaction,
                        serviceId,
                        txtTourMiscItem.Text,                // Purchased Item
                        txtTourMiscAmount.Text,              // Amount
                        txtTourMiscDate.Text,                // Date
                        fileUploadTourMiscellaneous,         // File upload for the bill
                                                             // File upload for the approval mail
                        "Tour",                              // Expense Type
                        fromTime,                            // From Time
                        toTime,                              // To Time
                        txtTourMiscParticulars.Text,         // Particulars
                        txtTourMiscRemarks.Text,             // Remarks
                        smoNo,                               // SMO No
                        refNo,                               // Ref No
                        soNo                                  // SO No
                    );
                }


                if (!string.IsNullOrEmpty(txtTourFoodAmount.Text))
                {
                    TimeSpan? fromTime = string.IsNullOrEmpty(txtTourFoodFromTime.Text) ? (TimeSpan?)null : TimeSpan.Parse(txtTourFoodFromTime.Text);
                    TimeSpan? toTime = string.IsNullOrEmpty(txtTourFoodToTime.Text) ? (TimeSpan?)null : TimeSpan.Parse(txtTourFoodToTime.Text);

                    // Assuming you have text boxes for SMO No and Ref No
                    string smoNo = txtTourFoodSMONo.Text; // Change this to the actual control ID
                    string refNo = txtTourFoodRefNo.Text; // Change this to the actual control ID
                    string soNo = txtTourFoodSONo.Text;
                    InsertFoodExpense(
                        con,
                        transaction,
                        serviceId,
                        "Tour",
                        txtTourFoodAmount.Text,
                        txtTourFoodDate.Text,
                        txtTourFoodDesignation.SelectedValue,
                        fromTime,
                        toTime,
                        txtTourFoodParticulars.Text,
                        txtTourFoodRemarks.Text,
                        smoNo,
                        refNo,
                        soNo
                    );
                }


                string distance = null; // Default distance to null

                if (ddlTourTransportMode.SelectedValue == "Cab")
                {
                    string smoNo = txtCabSmoNo.Text; // Capture SMO No
                    string refNo = txtCabRefNo.Text; // Capture Ref No
                    string soNo = txtCabSoNo.Text;
                    InsertConveyanceExpense(con, transaction, serviceId, "Cab", txtCabAmount.Text, txtCabDate.Text,
                                            txtFromTimeCab.Text, txtToTimeCab.Text, txtParticularsCab.Text, txtRemarksCab.Text,
                                            fileUploadCab, "Tour", distance, smoNo, refNo, soNo); // Pass SMO No and Ref No
                }
                else if (ddlTourTransportMode.SelectedValue == "Train")
                {
                    string smoNo = txtTrainSmoNo.Text; // Capture SMO No
                    string refNo = txtTrainRefNo.Text; // Capture Ref No
                    string soNo = txtTrainSoNo.Text;
                    InsertConveyanceExpense(con, transaction, serviceId, "Train", txtTrainAmount.Text, txtTrainDate.Text,
                                            txtFromTimeTrain.Text, txtToTimeTrain.Text, txtParticularsTrain.Text, txtRemarksTrain.Text,
                                            fileUploadTrain, "Tour", distance, smoNo, refNo, soNo); // Pass SMO No and Ref No
                }
                else if (ddlTourTransportMode.SelectedValue == "Flight")
                {
                    string smoNo = txtFlightSmoNo.Text; // Capture SMO No
                    string refNo = txtFlightRefNo.Text; // Capture Ref No
                    string soNo = txtFlightSoNo.Text;
                    InsertConveyanceExpense(con, transaction, serviceId, "Flight", txtFlightAmount.Text,
                                            txtFlightDate.Text, txtFlightFromTime.Text, txtFlightToTime.Text,
                                            txtFlightParticulars.Text, txtFlightRemarks.Text, fileUploadFlight, "Tour", distance, smoNo, refNo, soNo); // Pass SMO No and Ref No
                }
                else if (ddlTourTransportMode.SelectedValue == "Bus")
                {
                    string smoNo = txtBusSmoNo.Text; // Capture SMO No
                    string refNo = txtBusRefNo.Text; // Capture Ref No 
                    string soNo = txtBusSoNo.Text;
                    InsertConveyanceExpense(con, transaction, serviceId, "Bus", txtBusAmount.Text,
                                            txtBusDate.Text, txtFromTimeBus.Text, txtToTimeBus.Text,
                                            txtParticularsBus.Text, txtRemarksBus.Text, fileUploadBus, "Tour", distance, smoNo, refNo, soNo); // Pass SMO No and Ref No
                }
                else if (ddlTourTransportMode.SelectedValue == "Auto" && !string.IsNullOrEmpty(txtTourAutoAmount.Text))
                {
                    decimal autoAmount = Convert.ToDecimal(txtTourAutoAmount.Text);
                    string autoDistance = txtTourAutoDistance.Text; // Capture distance for Auto
                    string smoNo = txtTourAutoSmoNo.Text; // Capture SMO No
                    string refNo = txtTourAutoRefNo.Text; // Capture Ref No
                    string soNo = txtTourAutoSoNo.Text;
                    InsertConveyanceExpense(con, transaction, serviceId, "Auto", autoAmount.ToString(), txtTourAutoDate.Text,
                                            txtTourAutoFromTime.Text, txtTourAutoToTime.Text, txtTourAutoParticular.Text,
                                            txTourAutoRemarks.Text, fileUploadTourAuto, "Tour", autoDistance, smoNo, refNo, soNo); // Pass SMO No and Ref No
                }


            }
        }



        private bool CheckLocalExpenses()
        {
            return !string.IsNullOrEmpty(txtLocalFoodAmount.Text) ||
                   !string.IsNullOrEmpty(txtLocalMiscAmount.Text) ||
                   !string.IsNullOrEmpty(txtLocalOthersAmount.Text) ||
                   !string.IsNullOrEmpty(txtLocalAmount.Text);
        }

        private bool CheckTourExpenses()
        {
            return !string.IsNullOrEmpty(txtTourFoodAmount.Text) ||
                   !string.IsNullOrEmpty(txtTourMiscAmount.Text) ||
                   !string.IsNullOrEmpty(txtTourOthersAmount.Text) ||
                   !string.IsNullOrEmpty(txtFlightAmount.Text) ||
                   !string.IsNullOrEmpty(txtBusAmount.Text) ||
                   !string.IsNullOrEmpty(txtTrainAmount.Text) ||
                   !string.IsNullOrEmpty(txtCabAmount.Text);
        }
        private void InsertStarAwardExpense(
       SqlConnection con,
       SqlTransaction transaction,
       int serviceId,
       string Amount,
       string AwardDate,
       string AwardType)
        {
            try
            {
                // Validate input data
                if (string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(AwardDate) || string.IsNullOrEmpty(AwardType))
                {
                    throw new ArgumentException("Amount, Award Date, and Award Type are required fields.");
                }

                // Define the SQL query to insert the award expense
                string query = @"
            INSERT INTO Award (
                ServiceId,
                AwardType,
                Amount,
                AwardDate   
            )
            VALUES (
                @ServiceId,              
                @AwardType,
                @Amount,
                @AwardDate
            )";

                // Create the command object
                using (SqlCommand cmd = new SqlCommand(query, con, transaction))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    // cmd.Parameters.AddWithValue("@EmployeId", GetCurrentEmployeeId()); // Replace with logic to get the current employee ID
                    cmd.Parameters.AddWithValue("@AwardType", AwardType);
                    cmd.Parameters.AddWithValue("@Amount", decimal.Parse(Amount));
                    cmd.Parameters.AddWithValue("@AwardDate", DateTime.Parse(AwardDate));

                    // Execute the query
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Error inserting Star Award expense: " + ex.Message, ex);
            }
        }
        private void InsertFoodExpense(
     SqlConnection con,
     SqlTransaction transaction,
     int serviceId,
     string expenseType,
     string amount,
     string date,
     object designation,
     TimeSpan? fromTime,
     TimeSpan? toTime,
     string particulars,
     string remarks,
     string smoNo, // Parameter for SMO No
     string refNo, // Parameter for Ref No
     string soNo   // Parameter for SONO
 )
        {
            // Check for mandatory fields
            if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(date))
            {
                throw new ArgumentException("Amount and Date are required fields.");
            }

            // Get the value of 'CreatedBy' from HttpCookie (assuming 'FirstName' is stored in the cookie)
            HttpCookie firstNameCookie = Request.Cookies["FirstName"];
            string firstName = firstNameCookie?.Value;

            // Get current date for 'CreatedDate'
            DateTime createdDate = DateTime.Now;

            string sqlFood = @"
    INSERT INTO Food 
    (ServiceId, ExpenseType, Designation, Date, Amount, FromTime, ToTime, Particulars, Remarks, Smono, Refno, Sono, CreatedDate, CreatedBy) 
    VALUES 
    (@ServiceId, @ExpenseType, @Designation, @Date, @Amount, @FromTime, @ToTime, @Particulars, @Remarks, @Smono, @Refno, @Sono, @CreatedDate, @CreatedBy)";

            using (SqlCommand cmdFood = new SqlCommand(sqlFood, con, transaction))
            {
                // Add parameters with null checks for nullable types
                cmdFood.Parameters.AddWithValue("@ServiceId", serviceId);
                cmdFood.Parameters.AddWithValue("@ExpenseType", expenseType);
                cmdFood.Parameters.AddWithValue("@Designation", designation ?? DBNull.Value);
                cmdFood.Parameters.AddWithValue("@Date", DateTime.Parse(date));
                cmdFood.Parameters.AddWithValue("@Amount", Convert.ToDecimal(amount));
                cmdFood.Parameters.AddWithValue("@FromTime", fromTime.HasValue ? (object)fromTime.Value : DBNull.Value);
                cmdFood.Parameters.AddWithValue("@ToTime", toTime.HasValue ? (object)toTime.Value : DBNull.Value);
                cmdFood.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmdFood.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);

                // Parameters for SMO No, Ref No, and SONO
                cmdFood.Parameters.AddWithValue("@Smono", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmdFood.Parameters.AddWithValue("@Refno", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmdFood.Parameters.AddWithValue("@Sono", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo);

                // Add parameters for CreatedDate and CreatedBy
                cmdFood.Parameters.AddWithValue("@CreatedDate", createdDate); // Current date and time
                cmdFood.Parameters.AddWithValue("@CreatedBy", string.IsNullOrEmpty(firstName) ? (object)DBNull.Value : firstName); // User from HttpCookie

                // Execute the command
                cmdFood.ExecuteNonQuery();
            }
        }




        private void InsertMiscellaneousExpense(
      SqlConnection con,
      SqlTransaction transaction,
      int serviceId,
      string item,
      string amount,
      string date,
      FileUpload fileUpload,        // File upload for the bill
      string expenseType,
      TimeSpan fromTime,
      TimeSpan toTime,
      string particulars,
      string remarks,
      string smoNo,  // New parameter for SMO No
      string refNo,   // New parameter for Ref No
      string soNo     // New parameter for SO No
  )
        {
            if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(date))
            {
                return; // Validation: Ensure amount and date are not empty
            }

            // Get the value of 'CreatedBy' from HttpCookie (assuming 'FirstName' is stored in the cookie)
            HttpCookie firstNameCookie = Request.Cookies["FirstName"];
            string firstName = firstNameCookie?.Value;

            // Correct SQL query with VALUES instead of SELECT
            string sqlMisc = @"
    INSERT INTO Miscellaneous 
    (ServiceId, ExpenseType, PurchasedItem, Amount, Date, Image, FromTime, ToTime, Particulars, Remarks, Smono, Refno, SoNo, CreatedDate, CreatedBy) 
    VALUES
    (@ServiceId, @ExpenseType, @Item, @Amount, @Date, @Image, @FromTime, @ToTime, @Particulars, @Remarks, @Smono, @Refno, @SoNo, @CreatedDate, @CreatedBy)";

            using (SqlCommand cmdMisc = new SqlCommand(sqlMisc, con, transaction))
            {
                // Adding parameters to the command
                cmdMisc.Parameters.AddWithValue("@ServiceId", serviceId);
                cmdMisc.Parameters.AddWithValue("@ExpenseType", expenseType);
                cmdMisc.Parameters.AddWithValue("@Item", item);
                cmdMisc.Parameters.AddWithValue("@Amount", string.IsNullOrEmpty(amount) ? (object)DBNull.Value : Convert.ToDecimal(amount));
                cmdMisc.Parameters.AddWithValue("@Date", string.IsNullOrEmpty(date) ? (object)DBNull.Value : DateTime.Parse(date));

                // For image, handle the file upload
                cmdMisc.Parameters.Add("@Image", SqlDbType.VarBinary).Value = fileUpload?.HasFile == true ? GetFileBytes(fileUpload) : (object)DBNull.Value;

                cmdMisc.Parameters.AddWithValue("@FromTime", fromTime);
                cmdMisc.Parameters.AddWithValue("@ToTime", toTime);
                cmdMisc.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmdMisc.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);

                // Parameters for SMO No, Ref No, and SO No
                cmdMisc.Parameters.AddWithValue("@Smono", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmdMisc.Parameters.AddWithValue("@Refno", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmdMisc.Parameters.AddWithValue("@SoNo", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo);

                // Add parameters for CreatedDate and CreatedBy
                DateTime createdDate = DateTime.Now;
                cmdMisc.Parameters.AddWithValue("@CreatedDate", createdDate); // Current date and time
                cmdMisc.Parameters.AddWithValue("@CreatedBy", string.IsNullOrEmpty(firstName) ? (object)DBNull.Value : firstName); // User from HttpCookie

                // Execute the command
                cmdMisc.ExecuteNonQuery();
            }
        }



        private void InsertOthersExpense(
     SqlConnection con,
     SqlTransaction transaction,
     int serviceId,
     string amount,
     string date,
     string fromTime,
     string toTime,
     string particulars,
     string remarks,
     byte[] fileUploadLocalBill, // Accept byte[] for the file
     FileUpload fileUploadApproval, // FileUpload control for approval bill
     string smoNo, // SMO No
     string refNo, // Ref No
     string soNo, // SO No
     byte[] serviceReport // ServiceReport as byte[]
 )
        {
            // Validate the Amount and Date input fields
            if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(date))
            {
                throw new Exception("Amount and Date are required.");
            }

            // Get FirstName from HttpCookie for CreatedBy
            HttpCookie firstNameCookie = Request.Cookies["FirstName"];
            string firstName = firstNameCookie?.Value;

            // Convert Amount to Decimal
            decimal parsedAmount;
            if (!decimal.TryParse(amount, out parsedAmount))
            {
                throw new Exception("Invalid amount format.");
            }

            // Convert Date to DateTime
            DateTime parsedDate;
            if (!DateTime.TryParse(date, out parsedDate))
            {
                throw new Exception("Invalid date format.");
            }

            // Convert FromTime and ToTime to TimeSpan
            TimeSpan parsedFromTime;
            if (!TimeSpan.TryParse(fromTime, out parsedFromTime))
            {
                throw new Exception("Invalid 'FromTime' format.");
            }

            TimeSpan parsedToTime;
            if (!TimeSpan.TryParse(toTime, out parsedToTime))
            {
                throw new Exception("Invalid 'ToTime' format.");
            }

            // Add CreatedDate and CreatedBy
            DateTime createdDate = DateTime.Now;

            string sqlOthers = @"
    INSERT INTO Others 
    (ServiceId, ExpenseType, Date, Amount, FromTime, ToTime, Particulars, Remarks, Image, SmoNo, RefNo, SoNo, ServiceReport, ApprovalMail, CreatedDate, CreatedBy) 
    VALUES
    (@ServiceId, 'Local', @Date, @Amount, @FromTime, @ToTime, @Particulars, @Remarks, @Image, @SmoNo, @RefNo, @SoNo, @ServiceReport, @ApprovalMail, @CreatedDate, @CreatedBy)";

            using (SqlCommand cmdOthers = new SqlCommand(sqlOthers, con, transaction))
            {
                cmdOthers.Parameters.AddWithValue("@ServiceId", serviceId);
                cmdOthers.Parameters.AddWithValue("@Date", parsedDate); // Use parsed DateTime
                cmdOthers.Parameters.AddWithValue("@Amount", parsedAmount); // Use parsed decimal
                cmdOthers.Parameters.AddWithValue("@FromTime", parsedFromTime); // Use parsed TimeSpan
                cmdOthers.Parameters.AddWithValue("@ToTime", parsedToTime); // Use parsed TimeSpan
                cmdOthers.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmdOthers.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);

                // For the bill image, if the file is uploaded, convert it to byte array
                cmdOthers.Parameters.Add("@Image", SqlDbType.VarBinary).Value = fileUploadLocalBill ?? (object)DBNull.Value;

                // Add parameters for SMO No, Ref No, and SO No
                cmdOthers.Parameters.AddWithValue("@SmoNo", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmdOthers.Parameters.AddWithValue("@RefNo", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmdOthers.Parameters.AddWithValue("@SoNo", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo);

                // Add the ServiceReport (as byte array)
                cmdOthers.Parameters.Add("@ServiceReport", SqlDbType.VarBinary).Value = serviceReport ?? (object)DBNull.Value;

                // For the approval bill, if the file is uploaded, convert it to byte array
                cmdOthers.Parameters.Add("@ApprovalMail", SqlDbType.VarBinary).Value = fileUploadApproval.HasFile ? GetFileBytes(fileUploadApproval) : (object)DBNull.Value;

                // Add CreatedDate and CreatedBy
                cmdOthers.Parameters.AddWithValue("@CreatedDate", createdDate); // Current DateTime
                cmdOthers.Parameters.AddWithValue("@CreatedBy", string.IsNullOrEmpty(firstName) ? (object)DBNull.Value : firstName); // Use FirstName from HttpCookie

                try
                {
                    // Execute the SQL command to insert the data into the database
                    cmdOthers.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // Log or display the error to debug further
                    throw new Exception("Error inserting Others expense: " + ex.Message);
                }
            }
        }


        private void InsertLodgingExpense(
    SqlConnection con,
    SqlTransaction transaction,
    int serviceId,
    string amount,
    string date,
    string fromTime,
    string toTime,
    string particulars,
    string remarks,
    FileUpload fileUpload, // File upload for the bill
    FileUpload fileUploadTourApproval, // New parameter for the approval bill upload
    string smoNo, // SMO No
    string refNo, // Ref No
    string soNo, // SO No
    byte[] serviceReport // New parameter for the byte array of the service report
)
        {
            if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(date))
            {
                return; // Ensure amount and date are not empty
            }

            // Validate file size (example: 10 MB limit per file)
            int maxFileSize = 10485760; // 10 MB in bytes
            if (fileUpload.HasFile && fileUpload.PostedFile.ContentLength > maxFileSize)
            {
                throw new HttpException("File size exceeds the limit of 10 MB.");
            }

            if (fileUploadTourApproval.HasFile && fileUploadTourApproval.PostedFile.ContentLength > maxFileSize)
            {
                throw new HttpException("Approval file size exceeds the limit of 10 MB.");
            }

            // Get the first name from the HttpCookie
            HttpCookie firstNameCookie = Request.Cookies["FirstName"];
            string firstName = firstNameCookie?.Value;

            string sqlLodging = @"
        INSERT INTO Lodging (
            ServiceId, ExpenseType, Date, Amount, FromTime, ToTime, Particulars, Remarks, Image, 
            ServiceReport, ApprovalMail, SmoNo, RefNo, SoNo, CreatedDate, CreatedBy
        ) 
        VALUES (
            @ServiceId, 'Tour', @Date, @Amount, @FromTime, @ToTime, @Particulars, @Remarks, @Image, 
            @ServiceReport, @ApprovalMail, @SmoNo, @RefNo, @SoNo, @CreatedDate, @CreatedBy
        );";

            using (SqlCommand cmdLodging = new SqlCommand(sqlLodging, con, transaction))
            {
                cmdLodging.Parameters.AddWithValue("@ServiceId", serviceId);
                cmdLodging.Parameters.AddWithValue("@Date", string.IsNullOrEmpty(date) ? (object)DBNull.Value : DateTime.Parse(date));
                cmdLodging.Parameters.AddWithValue("@Amount", string.IsNullOrEmpty(amount) ? (object)DBNull.Value : Convert.ToDecimal(amount));
                cmdLodging.Parameters.AddWithValue("@FromTime", TimeSpan.Parse(fromTime));
                cmdLodging.Parameters.AddWithValue("@ToTime", TimeSpan.Parse(toTime));
                cmdLodging.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmdLodging.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);

                // Use the existing GetFileBytes method for file upload
                cmdLodging.Parameters.Add("@Image", SqlDbType.VarBinary).Value = GetFileBytes(fileUpload) ?? (object)DBNull.Value;
                cmdLodging.Parameters.Add("@ServiceReport", SqlDbType.VarBinary).Value = serviceReport ?? (object)DBNull.Value;
                cmdLodging.Parameters.Add("@ApprovalMail", SqlDbType.VarBinary).Value = GetFileBytes(fileUploadTourApproval) ?? (object)DBNull.Value;

                cmdLodging.Parameters.AddWithValue("@SmoNo", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmdLodging.Parameters.AddWithValue("@RefNo", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmdLodging.Parameters.AddWithValue("@SoNo", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo);

                // Add CreatedDate and CreatedBy
                DateTime createdDate = DateTime.Now;
                cmdLodging.Parameters.AddWithValue("@CreatedDate", createdDate);
                cmdLodging.Parameters.AddWithValue("@CreatedBy", string.IsNullOrEmpty(firstName) ? (object)DBNull.Value : firstName);

                // Execute the SQL command
                cmdLodging.ExecuteNonQuery();
            }
        }


        // Use your GetFileBytes method here as is
        private byte[] GetFileBytes(FileUpload fileUpload)
        {
            if (fileUpload.HasFile)
            {
                // Validate file size (less than 5 MB)
                const int maxFileSize = 70 * 1024 * 1024; // 5 MB in bytes
                if (fileUpload.PostedFile.ContentLength > maxFileSize)
                {
                    throw new InvalidOperationException("File size should be less than 5 MB.");
                }

                // Validate file format (only jpeg, jpg, png, pdf)
                string[] allowedExtensions = { ".jpeg", ".jpg", ".png", ".pdf" };
                string fileExtension = System.IO.Path.GetExtension(fileUpload.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Only JPEG, JPG, PNG, and PDF files are allowed.");
                }

                if (fileExtension != ".pdf")
                {
                    Stream fileStream = fileUpload.PostedFile.InputStream;

                    // Create a MemoryStream to save the PDF
                    byte[] fileBytes = ReadStreamToByteArray(fileStream);

                    // Create a MemoryStream to save the PDF
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        // Create PDF writer with the memory stream
                        PdfWriter writer = new PdfWriter(pdfStream);
                        PdfDocument pdf = new PdfDocument(writer);
                        Document document = new Document(pdf);

                        // Load the image from the byte array
                        ImageData imageData = ImageDataFactory.Create(fileBytes);
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);

                        // Add the image to the document
                        document.Add(image);

                        // Close the document
                        document.Close();

                        // Get the PDF as a byte array
                        byte[] pdfBytes = pdfStream.ToArray();
                        return pdfBytes;
                        // Save the binary data as a file (optional)

                    }
                }


                if (fileExtension != ".pdf")
                {
                    Stream fileStream = fileUpload.PostedFile.InputStream;


                    // Create a MemoryStream to save the PDF
                    byte[] fileBytes = ReadStreamToByteArray(fileStream);

                    // Create a MemoryStream to save the PDF
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        // Create PDF writer with the memory stream
                        PdfWriter writer = new PdfWriter(pdfStream);
                        PdfDocument pdf = new PdfDocument(writer);
                        Document document = new Document(pdf);

                        // Load the image from the byte array
                        ImageData imageData = ImageDataFactory.Create(fileBytes);
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);

                        // Add the image to the document
                        document.Add(image);

                        // Close the document
                        document.Close();

                        // Get the PDF as a byte array
                        byte[] pdfBytes = pdfStream.ToArray();
                        return pdfBytes;
                        // Save the binary data as a file (optional)

                    }
                }



                // If validations pass, return the file bytes
                using (var binaryReader = new System.IO.BinaryReader(fileUpload.PostedFile.InputStream))
                {
                    return binaryReader.ReadBytes(fileUpload.PostedFile.ContentLength);
                }
            }

            return null;
        }
        private byte[] ReadStreamToByteArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private void InsertConveyanceExpense(
       SqlConnection con,
       SqlTransaction transaction,
       int serviceId,
       string transportType,
       string amount,
       string date,
       string fromTime,
       string toTime,
       string particulars,
       string remarks,
       FileUpload fileUpload,
       string expenseType,
       string distance,
       string smoNo,  // New parameter for SMO No
       string refNo,  // New parameter for Ref No
       string soNo    // New parameter for SO No
   )
        {
            if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(date))
            {
                return; // Handle missing required values
            }

            // Get the value of 'FirstName' from HttpCookie
            HttpCookie firstNameCookie = Request.Cookies["FirstName"];
            string firstName = firstNameCookie?.Value;

            string sqlConveyance = @"
    INSERT INTO Conveyance (
        ServiceId, TransportType, Amount, ExpenseType, Date, 
        FromTime, ToTime, Particulars, Remarks, Distance, 
        Image, SmoNo, RefNo, SoNo, CreatedDate, CreatedBy
    ) 
    values(@ServiceId, @TransportType, @Amount, @ExpenseType, @Date, 
           @FromTime, @ToTime, @Particulars, @Remarks, @Distance, 
           @Image, @SmoNo, @RefNo, @SoNo, @CreatedDate, @CreatedBy
    )";

            using (SqlCommand cmdConveyance = new SqlCommand(sqlConveyance, con, transaction))
            {
                cmdConveyance.CommandTimeout = 120; // Increase timeout
                cmdConveyance.Parameters.AddWithValue("@ServiceId", serviceId);
                cmdConveyance.Parameters.AddWithValue("@TransportType", transportType);
                cmdConveyance.Parameters.AddWithValue("@Amount", string.IsNullOrEmpty(amount) ? (object)DBNull.Value : Convert.ToDecimal(amount));
                cmdConveyance.Parameters.AddWithValue("@Date", string.IsNullOrEmpty(date) ? (object)DBNull.Value : DateTime.Parse(date));
                cmdConveyance.Parameters.AddWithValue("@FromTime", string.IsNullOrEmpty(fromTime) ? (object)DBNull.Value : TimeSpan.Parse(fromTime));
                cmdConveyance.Parameters.AddWithValue("@ToTime", string.IsNullOrEmpty(toTime) ? (object)DBNull.Value : TimeSpan.Parse(toTime));
                cmdConveyance.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmdConveyance.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);
                cmdConveyance.Parameters.AddWithValue("@ExpenseType", expenseType); // Ensure this is set
                cmdConveyance.Parameters.AddWithValue("@Distance", string.IsNullOrEmpty(distance) ? (object)DBNull.Value : Convert.ToDecimal(distance)); // Added distance parameter

                // Add parameters for SMO No, Ref No, and SO No
                cmdConveyance.Parameters.AddWithValue("@SmoNo", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmdConveyance.Parameters.AddWithValue("@RefNo", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmdConveyance.Parameters.AddWithValue("@SoNo", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo); // SO No parameter

                // Add parameter for CreatedDate (current date and time)
                DateTime createdDate = DateTime.Now;
                cmdConveyance.Parameters.AddWithValue("@CreatedDate", createdDate); // Current DateTime
                cmdConveyance.Parameters.AddWithValue("@CreatedBy", string.IsNullOrEmpty(firstName) ? (object)DBNull.Value : firstName); // Use FirstName from HttpCookie

                // Handle file upload (Image)
                cmdConveyance.Parameters.Add("@Image", SqlDbType.VarBinary).Value = fileUpload?.HasFile == true ? GetFileBytes(fileUpload) : (object)DBNull.Value;

                // Execute the insert query
                cmdConveyance.ExecuteNonQuery();
            }
        }

        private void UpdateFoodFull(SqlConnection con, SqlTransaction transaction, int id, string amount, string date, string fromTime, string toTime, string particulars, string remarks, string smoNo, string refNo, string soNo, string designation)
        {
            string query = @"UPDATE Food SET Amount = @Amount, Date = @Date, FromTime = @FromTime, ToTime = @ToTime, 
                            Particulars = @Particulars, Remarks = @Remarks, Smono = @Smono, Refno = @Refno, Sono = @Sono, 
                            Designation = @Designation WHERE Id = @Id";
            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@Amount", string.IsNullOrEmpty(amount) ? (object)DBNull.Value : Convert.ToDecimal(amount));
                cmd.Parameters.AddWithValue("@Date", string.IsNullOrEmpty(date) ? (object)DBNull.Value : DateTime.Parse(date));
                cmd.Parameters.AddWithValue("@FromTime", string.IsNullOrEmpty(fromTime) ? (object)DBNull.Value : TimeSpan.Parse(fromTime));
                cmd.Parameters.AddWithValue("@ToTime", string.IsNullOrEmpty(toTime) ? (object)DBNull.Value : TimeSpan.Parse(toTime));
                cmd.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);
                cmd.Parameters.AddWithValue("@Smono", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmd.Parameters.AddWithValue("@Refno", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmd.Parameters.AddWithValue("@Sono", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo);
                cmd.Parameters.AddWithValue("@Designation", string.IsNullOrEmpty(designation) ? (object)DBNull.Value : designation);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateMiscellaneousFull(SqlConnection con, SqlTransaction transaction, int id, string amount, string date, FileUpload fileUpload, string fromTime, string toTime, string particulars, string remarks, string smoNo, string refNo, string soNo)
        {
            string query = @"UPDATE Miscellaneous SET Amount = @Amount, Date = @Date, FromTime = @FromTime, ToTime = @ToTime, 
                            PurchasedItem = @Item, Remarks = @Remarks, Smono = @Smono, Refno = @Refno, SoNo = @SoNo, Particulars = @Particulars";
            
            if (fileUpload != null && fileUpload.HasFile)
                query += ", Image = @Image";
            
            query += " WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@Amount", string.IsNullOrEmpty(amount) ? (object)DBNull.Value : Convert.ToDecimal(amount));
                cmd.Parameters.AddWithValue("@Date", string.IsNullOrEmpty(date) ? (object)DBNull.Value : DateTime.Parse(date));
                cmd.Parameters.AddWithValue("@FromTime", string.IsNullOrEmpty(fromTime) ? (object)DBNull.Value : TimeSpan.Parse(fromTime));
                cmd.Parameters.AddWithValue("@ToTime", string.IsNullOrEmpty(toTime) ? (object)DBNull.Value : TimeSpan.Parse(toTime));
                cmd.Parameters.AddWithValue("@Item", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmd.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);
                cmd.Parameters.AddWithValue("@Smono", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmd.Parameters.AddWithValue("@Refno", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmd.Parameters.AddWithValue("@SoNo", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo);
                cmd.Parameters.AddWithValue("@Id", id);
                
                if (fileUpload != null && fileUpload.HasFile)
                    cmd.Parameters.Add("@Image", SqlDbType.VarBinary).Value = GetFileBytes(fileUpload);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateOthersFull(SqlConnection con, SqlTransaction transaction, int id, string amount, string date, string fromTime, string toTime, string particulars, string remarks, FileUpload fileUploadLocalBill, FileUpload fileUploadApproval, FileUpload fileServiceReport, string smoNo, string refNo, string soNo)
        {
            string query = @"UPDATE Others SET Amount = @Amount, Date = @Date, FromTime = @FromTime, ToTime = @ToTime, 
                            Particulars = @Particulars, Remarks = @Remarks, SmoNo = @SmoNo, RefNo = @RefNo, SoNo = @SoNo";
            
            if (fileUploadLocalBill != null && fileUploadLocalBill.HasFile) query += ", Image = @Image";
            if (fileUploadApproval != null && fileUploadApproval.HasFile) query += ", ApprovalMail = @ApprovalMail";
            if (fileServiceReport != null && fileServiceReport.HasFile) query += ", ServiceReport = @ServiceReport";
            
            query += " WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@Amount", string.IsNullOrEmpty(amount) ? (object)DBNull.Value : Convert.ToDecimal(amount));
                cmd.Parameters.AddWithValue("@Date", string.IsNullOrEmpty(date) ? (object)DBNull.Value : DateTime.Parse(date));
                cmd.Parameters.AddWithValue("@FromTime", string.IsNullOrEmpty(fromTime) ? (object)DBNull.Value : TimeSpan.Parse(fromTime));
                cmd.Parameters.AddWithValue("@ToTime", string.IsNullOrEmpty(toTime) ? (object)DBNull.Value : TimeSpan.Parse(toTime));
                cmd.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);
                cmd.Parameters.AddWithValue("@SmoNo", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmd.Parameters.AddWithValue("@RefNo", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmd.Parameters.AddWithValue("@SoNo", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo);
                cmd.Parameters.AddWithValue("@Id", id);

                if (fileUploadLocalBill != null && fileUploadLocalBill.HasFile)
                    cmd.Parameters.Add("@Image", SqlDbType.VarBinary).Value = GetFileBytes(fileUploadLocalBill);
                if (fileUploadApproval != null && fileUploadApproval.HasFile)
                    cmd.Parameters.Add("@ApprovalMail", SqlDbType.VarBinary).Value = GetFileBytes(fileUploadApproval);
                if (fileServiceReport != null && fileServiceReport.HasFile)
                    cmd.Parameters.Add("@ServiceReport", SqlDbType.VarBinary).Value = GetFileBytes(fileServiceReport);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateLodgingFull(SqlConnection con, SqlTransaction transaction, int id, string amount, string date, string fromTime, string toTime, string particulars, string remarks, FileUpload fileUpload, FileUpload fileUploadTourApproval, FileUpload fileUploadServiceReport, string smoNo, string refNo, string soNo)
        {
            string query = @"UPDATE Lodging SET Amount = @Amount, Date = @Date, FromTime = @FromTime, ToTime = @ToTime, 
                            Particulars = @Particulars, Remarks = @Remarks, SmoNo = @SmoNo, RefNo = @RefNo, SoNo = @SoNo";
            
            if (fileUpload != null && fileUpload.HasFile) query += ", Image = @Image";
            if (fileUploadTourApproval != null && fileUploadTourApproval.HasFile) query += ", ApprovalMail = @ApprovalMail";
            if (fileUploadServiceReport != null && fileUploadServiceReport.HasFile) query += ", ServiceReport = @ServiceReport";
            
            query += " WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@Amount", string.IsNullOrEmpty(amount) ? (object)DBNull.Value : Convert.ToDecimal(amount));
                cmd.Parameters.AddWithValue("@Date", string.IsNullOrEmpty(date) ? (object)DBNull.Value : DateTime.Parse(date));
                cmd.Parameters.AddWithValue("@FromTime", string.IsNullOrEmpty(fromTime) ? (object)DBNull.Value : TimeSpan.Parse(fromTime));
                cmd.Parameters.AddWithValue("@ToTime", string.IsNullOrEmpty(toTime) ? (object)DBNull.Value : TimeSpan.Parse(toTime));
                cmd.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);
                cmd.Parameters.AddWithValue("@SmoNo", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmd.Parameters.AddWithValue("@RefNo", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmd.Parameters.AddWithValue("@SoNo", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo);
                cmd.Parameters.AddWithValue("@Id", id);

                if (fileUpload != null && fileUpload.HasFile)
                    cmd.Parameters.Add("@Image", SqlDbType.VarBinary).Value = GetFileBytes(fileUpload);
                if (fileUploadTourApproval != null && fileUploadTourApproval.HasFile)
                    cmd.Parameters.Add("@ApprovalMail", SqlDbType.VarBinary).Value = GetFileBytes(fileUploadTourApproval);
                if (fileUploadServiceReport != null && fileUploadServiceReport.HasFile)
                    cmd.Parameters.Add("@ServiceReport", SqlDbType.VarBinary).Value = GetFileBytes(fileUploadServiceReport);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateConveyanceFull(SqlConnection con, SqlTransaction transaction, int id, string transportType, string amount, string date, string fromTime, string toTime, string particulars, string remarks, FileUpload fileUpload, string distance, string smoNo, string refNo, string soNo)
        {
            string query = @"UPDATE Conveyance SET TransportType = @TransportType, Amount = @Amount, Date = @Date, 
                            FromTime = @FromTime, ToTime = @ToTime, Particulars = @Particulars, Remarks = @Remarks, 
                            Distance = @Distance, SmoNo = @SmoNo, RefNo = @RefNo, SoNo = @SoNo";
            
            if (fileUpload != null && fileUpload.HasFile) query += ", Image = @Image";
            
            query += " WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@TransportType", transportType);
                cmd.Parameters.AddWithValue("@Amount", string.IsNullOrEmpty(amount) ? (object)DBNull.Value : Convert.ToDecimal(amount));
                cmd.Parameters.AddWithValue("@Date", string.IsNullOrEmpty(date) ? (object)DBNull.Value : DateTime.Parse(date));
                cmd.Parameters.AddWithValue("@FromTime", string.IsNullOrEmpty(fromTime) ? (object)DBNull.Value : TimeSpan.Parse(fromTime));
                cmd.Parameters.AddWithValue("@ToTime", string.IsNullOrEmpty(toTime) ? (object)DBNull.Value : TimeSpan.Parse(toTime));
                cmd.Parameters.AddWithValue("@Particulars", string.IsNullOrEmpty(particulars) ? (object)DBNull.Value : particulars);
                cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);
                cmd.Parameters.AddWithValue("@Distance", string.IsNullOrEmpty(distance) ? (object)DBNull.Value : Convert.ToDecimal(distance));
                cmd.Parameters.AddWithValue("@SmoNo", string.IsNullOrEmpty(smoNo) ? (object)DBNull.Value : smoNo);
                cmd.Parameters.AddWithValue("@RefNo", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo);
                cmd.Parameters.AddWithValue("@SoNo", string.IsNullOrEmpty(soNo) ? (object)DBNull.Value : soNo);
                cmd.Parameters.AddWithValue("@Id", id);

                if (fileUpload != null && fileUpload.HasFile)
                    cmd.Parameters.Add("@Image", SqlDbType.VarBinary).Value = GetFileBytes(fileUpload);

                cmd.ExecuteNonQuery();
            }
        }








        private void UpdateStatus(SqlTransaction transaction, int serviceId, int employeeId, int statusId, string status)
        {
            string updateStatusSql = @"
        UPDATE Services
        SET StatusId = @StatusId, Status = @StatusDescription
        WHERE ServiceId = @ServiceId AND EmployeeId = @EmployeeId";

            using (SqlCommand cmdUpdateStatus = new SqlCommand(updateStatusSql, transaction.Connection, transaction))
            {
                cmdUpdateStatus.Parameters.AddWithValue("@ServiceId", serviceId);
                cmdUpdateStatus.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmdUpdateStatus.Parameters.AddWithValue("@StatusId", statusId);
                cmdUpdateStatus.Parameters.AddWithValue("@StatusDescription", status);
                cmdUpdateStatus.ExecuteNonQuery();
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Clear the fields
            ClearExpenseFields1();

            // Hide GridViews if required
            GridViewFood.Visible = false;
            GridViewMiscellaneous.Visible = false;
            GridViewOthers.Visible = false;
            GridViewLodging.Visible = false;
            GridViewConveyance.Visible = false;

            // Refresh the page
            Response.Redirect(Request.Url.ToString(), true);
        }
        private void ClearExpenseFields1(Control parent = null)
        {
            // If no parent is provided, start with the page itself
            if (parent == null) parent = Page;

            // Loop through all child controls
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox txtBox)
                {
                    txtBox.Text = ""; // Clear textboxes
                }
                else if (ctrl is DropDownList ddl)
                {
                    ddl.SelectedIndex = 0; // Reset dropdowns to the first option
                }
                else if (ctrl.HasControls())
                {
                    // Recursively handle child controls
                    ClearExpenseFields1(ctrl);
                }
            }
        }
        private DateTime? GetLatestDate(DataTable table, string dateColumnName = "Date")
        {
            if (table.Rows.Count == 0) return null;

            DateTime? maxDate = null;
            foreach (DataRow row in table.Rows)
            {
                if (row[dateColumnName] != DBNull.Value)
                {
                    DateTime date = (DateTime)row[dateColumnName];
                    if (maxDate == null || date > maxDate)
                        maxDate = date;
                }
            }
            return maxDate;
        }
        private void ClearExpenseFields()
        {
            // Capture the selected ExpenseType value before clearing fields
            string selectedExpenseType = ddlExpenseType.SelectedValue;

            // Local Expenses Fields
            txtLocalOthersAmount.Text = "";
            txtLocalOthersDate.Text = "";
            txtLocalOthersFromTime.Text = "";
            txtLocalOthersToTime.Text = "";
            txtLocalOthersParticulars.Text = "";
            txtLocalOthersRemarks.Text = "";
            txtLocalOthersSMONo.Text = "";
            txtLocalOthersRefNo.Text = "";
            txtLocalOthersSoNo.Text = "";

            txtLocalMiscItem.Text = "";
            txtLocalMiscAmount.Text = "";
            txtLocalMiscDate.Text = "";
            txtLocalMiscFromTime.Text = "";
            txtLocalMiscToTime.Text = "";
            txtLocalMiscParticulars.Text = "";
            txtLocalMiscRemarks.Text = "";
            txtLocalMiscSMONo.Text = "";
            txtLocalMiscRefNo.Text = "";
            txtLocalMiscSONo.Text = "";

            txtLocalFoodAmount.Text = "";
            txtLocalFoodDate.Text = "";
            txtLocalFoodFromTime.Text = "";
            txtLocalFoodToTime.Text = "";
            txtLocalFoodParticulars.Text = "";
            txtLocalFoodRemarks.Text = "";
            txtLocalSMONo.Text = "";
            txtLocalRefNo.Text = "";
            txtLocalFoodSONo.Text = "";

            txtLocalDistance.Text = "";
            txtLocalAmount.Text = "";
            txtLocalBikeDate.Text = "";
            txtLocalBikeFromTime.Text = "";
            txtLocalBikeToTime.Text = "";
            txtLocalBikeParticular.Text = "";
            txtLocalBikeRemarks.Text = "";
            txtLocalBikeSMONo.Text = "";
            txtLocalBikeSONo.Text = "";
            txtLocalBikeRefNo.Text = "";
            txtLocalCabDate.Text = "";
            txtLocalCabAmount.Text = "";
            txtLocalCabFromTime.Text = "";
            txtLocalCabToTime.Text = "";
            txtLocalCabParticular.Text = "";
            txtLocalCabRemarks.Text = "";
            txtLocalCabSMONo.Text = "";
            txtLocalCabSONo.Text = "";
            txtLocalCabRefNo.Text = "";
            txtLocalAutoAmount.Text = "";
            txtLocalAutoDate.Text = "";
            txtLocalAutoFromTime.Text = "";
            txtLocalAutoToTime.Text = "";
            txtLocalAutoParticular.Text = "";
            txtLocalAutoRemarks.Text = "";
            txtLocalAutoDistance.Text = "";
            txtLocalAutoSMONo.Text = "";
            txtLocalAutoRefNo.Text = "";
            txtLocalAutoSONo.Text = "";

            // Reset dropdowns for Local
            ddlLocalMode.SelectedIndex = 0;

            // Reset Tour Expenses Fields
            txtTourOthersAmount.Text = "";
            txtTourOthersDate.Text = "";
            txtFromTimeTourOthers.Text = "";
            txtToTimeTourOthers.Text = "";
            txtParticularsTourOthers.Text = "";
            txtRemarksTourOthers.Text = "";
            txtTourOthersSmoNo.Text = "";
            txtTourOthersRefNo.Text = "";
            txtTourOthersSoNo.Text = "";

            txtTourMiscItem.Text = "";
            txtTourMiscAmount.Text = "";
            txtTourMiscDate.Text = "";
            txtTourMiscFromTime.Text = "";
            txtTourMiscToTime.Text = "";
            txtTourMiscParticulars.Text = "";
            txtTourMiscRemarks.Text = "";
            txtTourMiscSmoNo.Text = "";
            txtTourMiscRefNo.Text = "";
            txtTourMiscSoNo.Text = "";

            txtTourFoodAmount.Text = "";
            txtTourFoodDate.Text = "";
            txtTourFoodFromTime.Text = "";
            txtTourFoodToTime.Text = "";
            txtTourFoodParticulars.Text = "";
            txtTourFoodRemarks.Text = "";
            txtTourFoodSMONo.Text = "";
            txtTourFoodRefNo.Text = "";
            txtTourFoodSONo.Text = "";

            // Reset Tour Conveyance Fields
            txtCabAmount.Text = "";
            txtCabDate.Text = "";
            txtFromTimeCab.Text = "";
            txtToTimeCab.Text = "";
            txtParticularsCab.Text = "";
            txtRemarksCab.Text = "";
            txtCabSmoNo.Text = "";
            txtCabRefNo.Text = "";
            txtCabSoNo.Text = "";

            txtTrainAmount.Text = "";
            txtTrainDate.Text = "";
            txtFromTimeTrain.Text = "";
            txtToTimeTrain.Text = "";
            txtParticularsTrain.Text = "";
            txtRemarksTrain.Text = "";
            txtTrainSmoNo.Text = "";
            txtTrainRefNo.Text = "";
            txtTrainSoNo.Text = "";

            txtFlightAmount.Text = "";
            txtFlightDate.Text = "";
            txtFlightFromTime.Text = "";
            txtFlightToTime.Text = "";
            txtFlightParticulars.Text = "";
            txtFlightRemarks.Text = "";
            txtFlightSmoNo.Text = "";
            txtFlightRefNo.Text = "";
            txtFlightSoNo.Text = "";

            txtBusAmount.Text = "";
            txtBusDate.Text = "";
            txtFromTimeBus.Text = "";
            txtToTimeBus.Text = "";
            txtParticularsBus.Text = "";
            txtRemarksBus.Text = "";
            txtBusSmoNo.Text = "";
            txtBusRefNo.Text = "";
            txtBusSoNo.Text = "";

            txtTourAutoAmount.Text = "";
            txtTourAutoDate.Text = "";
            txtTourAutoFromTime.Text = "";
            txtTourAutoToTime.Text = "";
            txtTourAutoParticular.Text = "";
            txTourAutoRemarks.Text = "";
            txtTourAutoDistance.Text = "";
            txtTourAutoSmoNo.Text = "";
            txtTourAutoRefNo.Text = "";
            txtTourAutoSoNo.Text = "";
            txtAwardAmount.Text = "";
            txtAwardDate.Text = "";
            ddlAwardExpenseType.Text = "";
            ddlTourTransportMode.SelectedIndex = 0;

            // After clearing all fields, reset the ExpenseType dropdown to the previously selected value
            if (!string.IsNullOrEmpty(selectedExpenseType))
            {
                ddlExpenseType.SelectedValue = selectedExpenseType; // Set the previously selected value
            }
        }

        // ─── Excel Import Handler ─────────────────────────────────────────────────
        // Parses the Excel, stores rows in Session, and shows the preview GridView.
        // No DB insert happens here — DB insert happens only when "Save" is clicked.
        protected void btnImportExcel_Click(object sender, EventArgs e)
        {
            if (!fileUploadExcel.HasFile)
            {
                lblError.Text = "Please select an Excel file to import.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Save raw bytes so the "View" button can re-download it
            byte[] excelBytes = fileUploadExcel.FileBytes;
            Session["UploadedExcelFileBytes"] = excelBytes;
            Session["UploadedExcelFileName"]  = fileUploadExcel.FileName;

            // Build a preview DataTable — one row per expense entry
            DataTable dt = BuildExcelPreviewSchema();
            int skipCount = 0;

            try
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var ms = new MemoryStream(excelBytes))
                using (var package = new OfficeOpenXml.ExcelPackage(ms))
                {
                    var ws = package.Workbook.Worksheets[0];
                    if (ws == null || ws.Dimension == null)
                    {
                        lblError.Text = "No data found in Excel file.";
                        lblError.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    int totalRows = ws.Dimension.End.Row;
                    int totalCols = ws.Dimension.End.Column;

                    // ── Find header row ──────────────────────────────────────────
                    int headerRow = -1;
                    var colMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                    for (int r = 1; r <= Math.Min(6, totalRows); r++)
                    {
                        for (int c = 1; c <= totalCols; c++)
                        {
                            string cv = (ws.Cells[r, c].Text ?? "").Trim().ToLower();
                            if (cv.Contains("particulars") || cv.Contains("expense type") || cv.Contains("from time"))
                            {
                                headerRow = r;
                                break;
                            }
                        }
                        if (headerRow > 0) break;
                    }

                    if (headerRow > 0)
                    {
                        for (int c = 1; c <= totalCols; c++)
                        {
                            string hdr = (ws.Cells[headerRow, c].Text ?? "").Trim().ToLower();
                            if (!string.IsNullOrEmpty(hdr) && !colMap.ContainsKey(hdr)) colMap[hdr] = c;
                        }
                        if (headerRow > 1)
                        {
                            for (int c = 1; c <= totalCols; c++)
                            {
                                string hdr = (ws.Cells[headerRow - 1, c].Text ?? "").Trim().ToLower();
                                if (!string.IsNullOrEmpty(hdr) && !colMap.ContainsKey(hdr)) colMap[hdr] = c;
                            }
                        }
                    }

                    int colExpType     = GetColIndex(colMap, new[] { "expense type" }, 2);
                    int colDate        = GetColIndex(colMap, new[] { "date" }, 3);
                    int colFromTime    = GetColIndex(colMap, new[] { "from time", "from" }, 4);
                    int colToTime      = GetColIndex(colMap, new[] { "to time", "to" }, 5);
                    int colParticulars = GetColIndex(colMap, new[] { "particulars" }, 6);
                    int colDistance    = GetColIndex(colMap, new[] { "distance in (km)", "distance (km)", "distance" }, 7);
                    int colTransport   = GetColIndex(colMap, new[] { "mode of transport" }, 8);
                    int colConveyance  = GetColIndex(colMap, new[] { "conveyance" }, 9);
                    int colLodging     = GetColIndex(colMap, new[] { "lodging" }, 10);
                    int colFooding     = GetColIndex(colMap, new[] { "fooding", "food" }, 11);
                    int colSONo        = GetColIndex(colMap, new[] { "so", "sap", "so/sap no", "so no", "sap no" }, 12);
                    int colSMONo       = GetColIndex(colMap, new[] { "smo", "wbs", "smo/wbs no", "smo no", "wbs no" }, 13);
                    int colRefNo       = GetColIndex(colMap, new[] { "ref", "ref no" }, 14);
                    int colRemarks     = GetColIndex(colMap, new[] { "remarks" }, 15);

                    int dataStartRow = (headerRow > 0) ? headerRow + 1 : 2;

                    string lastDate      = "";
                    string lastExpType   = "";
                    string lastTransport = "";
                    int    rowId         = 0;

                    for (int r = dataStartRow; r <= totalRows; r++)
                    {
                        string rawDate        = (ws.Cells[r, colDate].Text        ?? "").Trim();
                        string rawExpType     = (ws.Cells[r, colExpType].Text     ?? "").Trim();
                        string rawFromTime    = (ws.Cells[r, colFromTime].Text    ?? "").Trim();
                        string rawToTime      = (ws.Cells[r, colToTime].Text      ?? "").Trim();
                        string rawParticulars = (ws.Cells[r, colParticulars].Text ?? "").Trim();
                        string rawDistance    = (ws.Cells[r, colDistance].Text    ?? "").Trim();
                        string rawTransport   = (ws.Cells[r, colTransport].Text   ?? "").Trim();
                        string rawConveyance  = (ws.Cells[r, colConveyance].Text  ?? "").Trim();
                        string rawLodging     = (ws.Cells[r, colLodging].Text     ?? "").Trim();
                        string rawFooding     = (ws.Cells[r, colFooding].Text     ?? "").Trim();
                        string rawSONo        = (ws.Cells[r, colSONo].Text        ?? "").Trim();
                        string rawSMONo       = (ws.Cells[r, colSMONo].Text       ?? "").Trim();
                        string rawRefNo       = (ws.Cells[r, colRefNo].Text       ?? "").Trim();
                        string rawRemarks     = (ws.Cells[r, colRemarks].Text     ?? "").Trim();

                        if (!string.IsNullOrEmpty(rawDate))      lastDate      = rawDate;
                        if (!string.IsNullOrEmpty(rawExpType))   lastExpType   = rawExpType;
                        if (!string.IsNullOrEmpty(rawTransport)) lastTransport = rawTransport;

                        string useDate      = string.IsNullOrEmpty(rawDate)      ? lastDate      : rawDate;
                        string useExpType   = string.IsNullOrEmpty(rawExpType)   ? lastExpType   : rawExpType;
                        string useTransport = string.IsNullOrEmpty(rawTransport) ? lastTransport : rawTransport;

                        bool hasConveyance = TryParsePositiveDecimal(rawConveyance, out decimal convAmt);
                        bool hasLodging    = TryParsePositiveDecimal(rawLodging,    out decimal lodgAmt);
                        bool hasFooding    = TryParsePositiveDecimal(rawFooding,    out decimal foodAmt);

                        if (!hasConveyance && !hasLodging && !hasFooding) { skipCount++; continue; }
                        if (string.IsNullOrEmpty(useDate)) { skipCount++; continue; }

                        // Try parse date
                        if (!TryParseExcelDate(useDate, ws.Cells[r, colDate].Value, out DateTime parsedDate))
                        { skipCount++; continue; }

                        string isoDate  = parsedDate.ToString("yyyy-MM-dd");
                        string fromTime = ParseExcelTime(rawFromTime, ws.Cells[r, colFromTime].Value);
                        string toTime   = ParseExcelTime(rawToTime,   ws.Cells[r, colToTime].Value);

                        string expType  = string.IsNullOrEmpty(useExpType) ? "Local" : useExpType;

                        // Normalise transport
                        string transport = string.IsNullOrEmpty(useTransport) ? "Bike" : useTransport;
                        if (transport.Equals("bike", StringComparison.OrdinalIgnoreCase)) transport = "Bike";
                        else if (transport.ToLower().Contains("cab") || transport.ToLower().Contains("bus")) transport = "Cab/Bus";
                        else if (transport.Equals("auto", StringComparison.OrdinalIgnoreCase)) transport = "Auto";

                        // Each amount column gets its own preview row so OK fills one form at a time
                        if (hasConveyance)
                        {
                            DataRow dr = dt.NewRow();
                            dr["RowId"]         = ++rowId;
                            dr["ExpenseType"]   = expType;
                            dr["Category"]      = "Conveyance";
                            dr["Date"]          = isoDate;
                            dr["FromTime"]      = fromTime;
                            dr["ToTime"]        = toTime;
                            dr["Particulars"]   = rawParticulars;
                            dr["Distance"]      = rawDistance;
                            dr["TransportType"] = transport;
                            dr["Amount"]        = convAmt.ToString("0.##");
                            dr["SONo"]          = rawSONo;
                            dr["SMONo"]         = rawSMONo;
                            dr["RefNo"]         = rawRefNo;
                            dr["Remarks"]       = rawRemarks;
                            dt.Rows.Add(dr);
                        }
                        if (hasFooding)
                        {
                            DataRow dr = dt.NewRow();
                            dr["RowId"]         = ++rowId;
                            dr["ExpenseType"]   = expType;
                            dr["Category"]      = "Food";
                            dr["Date"]          = isoDate;
                            dr["FromTime"]      = fromTime;
                            dr["ToTime"]        = toTime;
                            dr["Particulars"]   = rawParticulars;
                            dr["Distance"]      = "";
                            dr["TransportType"] = "";
                            dr["Amount"]        = foodAmt.ToString("0.##");
                            dr["SONo"]          = rawSONo;
                            dr["SMONo"]         = rawSMONo;
                            dr["RefNo"]         = rawRefNo;
                            dr["Remarks"]       = rawRemarks;
                            dt.Rows.Add(dr);
                        }
                        if (hasLodging)
                        {
                            DataRow dr = dt.NewRow();
                            dr["RowId"]         = ++rowId;
                            dr["ExpenseType"]   = expType;
                            dr["Category"]      = "Lodging";
                            dr["Date"]          = isoDate;
                            dr["FromTime"]      = fromTime;
                            dr["ToTime"]        = toTime;
                            dr["Particulars"]   = rawParticulars;
                            dr["Distance"]      = "";
                            dr["TransportType"] = "";
                            dr["Amount"]        = lodgAmt.ToString("0.##");
                            dr["SONo"]          = rawSONo;
                            dr["SMONo"]         = rawSMONo;
                            dr["RefNo"]         = rawRefNo;
                            dr["Remarks"]       = rawRemarks;
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Error reading Excel: {ex.Message}";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (dt.Rows.Count == 0)
            {
                lblError.Text = $"No valid data rows found in the Excel file ({skipCount} rows skipped).";
                lblError.ForeColor = System.Drawing.Color.Orange;
                return;
            }

            // Store in Session and show preview
            Session["ExcelPreviewData"] = dt;
            BindExcelPreview(dt);
            pnlExcelPreview.Visible = true;

            lblError.ForeColor = System.Drawing.Color.Green;
            lblError.Text = $"{dt.Rows.Count} row(s) ready to review. Click OK on each row to fill the form.";
        }

        // ── Preview GridView RowCommand ────────────────────────────────────────────
        protected void gvExcelPreview_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "FillForm" && e.CommandName != "RemoveRow") return;

            DataTable dt = Session["ExcelPreviewData"] as DataTable;
            if (dt == null) return;

            int rowId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "RemoveRow")
            {
                // Delete without filling the form
                DataRow[] rows = dt.Select($"RowId = {rowId}");
                foreach (var r in rows) dt.Rows.Remove(r);
                Session["ExcelPreviewData"] = dt;
                BindExcelPreview(dt);
                if (dt.Rows.Count == 0) pnlExcelPreview.Visible = false;
                return;
            }

            // FillForm — find the row and populate the web form
            DataRow[] matchingRows = dt.Select($"RowId = {rowId}");
            if (matchingRows.Length == 0) return;

            DataRow row = matchingRows[0];
            FillFormFromExcelRow(row);

            // Remove from preview so user knows it's been loaded
            dt.Rows.Remove(row);
            Session["ExcelPreviewData"] = dt;
            BindExcelPreview(dt);
            if (dt.Rows.Count == 0) pnlExcelPreview.Visible = false;

            // Scroll the page to the form area
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scrollForm",
                "window.scrollTo({top: document.getElementById('mainFormSection') ? document.getElementById('mainFormSection').offsetTop : 300, behavior:'smooth'});", true);
        }

        // ── Fill all relevant form fields from one Excel preview row ──────────────
        private void FillFormFromExcelRow(DataRow row)
        {
            string expType   = row["ExpenseType"].ToString();   // Local / Tour
            string category  = row["Category"].ToString();      // Conveyance / Food / Lodging
            string date      = row["Date"].ToString();
            string fromTime  = row["FromTime"].ToString();
            string toTime    = row["ToTime"].ToString();
            string particulars = row["Particulars"].ToString();
            string distance  = row["Distance"].ToString();
            string transport = row["TransportType"].ToString();
            string amount    = row["Amount"].ToString();
            string soNo      = row["SONo"].ToString();
            string smoNo     = row["SMONo"].ToString();
            string refNo     = row["RefNo"].ToString();
            string remarks   = row["Remarks"].ToString();

            // ── Hide all sub-panels first (same as ddlExpenseType_SelectedIndexChanged) ──
            pnlLocalExpenses.Visible           = false;
            pnlTourExpenses.Visible            = false;
            pnlAwardExpenses.Visible           = false;
            pnlLocalFoodFields.Visible         = false;
            pnlLocalMiscellaneousFields.Visible= false;
            pnlLocalOthersFields.Visible       = false;
            pnlLocalConvenience.Visible        = false;
            pnlBikeFields.Visible              = false;
            pnlCabFields.Visible               = false;
            pnlAutoFields.Visible              = false;
            pnlTourFoodFields.Visible          = false;
            pnlTourMiscellaneousFields.Visible = false;
            pnlTourOthersFields.Visible        = false;
            pnlTourConvenience.Visible         = false;
            pnlFlightFields.Visible            = false;
            pnlBusFields.Visible               = false;
            pnlTrainFields.Visible             = false;
            pnlcabTourFields.Visible           = false;

            // ── Set top-level Expense Type ────────────────────────────────────────
            ddlExpenseType.SelectedValue = expType;

            if (expType == "Local")
            {
                pnlLocalExpenses.Visible = true;

                if (category == "Food")
                {
                    ddlLocalExpenseType.SelectedValue = "Food";
                    pnlLocalFoodFields.Visible         = true;
                    txtLocalFoodDate.Text              = date;
                    txtLocalFoodFromTime.Text          = fromTime;
                    txtLocalFoodToTime.Text            = toTime;
                    txtLocalFoodParticulars.Text       = particulars;
                    txtLocalFoodAmount.Text            = amount;
                    txtLocalFoodSONo.Text              = soNo;
                    txtLocalSMONo.Text                 = smoNo;
                    txtLocalRefNo.Text                 = refNo;
                    txtLocalFoodRemarks.Text           = remarks;
                }
                else if (category == "Conveyance")
                {
                    ddlLocalExpenseType.SelectedValue = "Conveyance";
                    pnlLocalConvenience.Visible        = true;
                    ddlLocalMode.SelectedValue         = transport;

                    if (transport == "Bike")
                    {
                        pnlBikeFields.Visible         = true;
                        txtLocalBikeDate.Text         = date;
                        txtLocalBikeFromTime.Text     = fromTime;
                        txtLocalBikeToTime.Text       = toTime;
                        txtLocalBikeParticular.Text   = particulars;
                        txtLocalDistance.Text         = distance;
                        txtLocalAmount.Text           = amount;
                        txtLocalBikeSONo.Text         = soNo;
                        txtLocalBikeSMONo.Text        = smoNo;
                        txtLocalBikeRefNo.Text        = refNo;
                        txtLocalBikeRemarks.Text      = remarks;
                    }
                    else if (transport == "Cab/Bus")
                    {
                        pnlCabFields.Visible          = true;
                        txtLocalCabDate.Text          = date;
                        txtLocalCabFromTime.Text      = fromTime;
                        txtLocalCabToTime.Text        = toTime;
                        txtLocalCabParticular.Text    = particulars;
                        txtLocalCabAmount.Text        = amount;
                        txtLocalCabSONo.Text          = soNo;
                        txtLocalCabSMONo.Text         = smoNo;
                        txtLocalCabRefNo.Text         = refNo;
                        txtLocalCabRemarks.Text       = remarks;
                    }
                    else if (transport == "Auto")
                    {
                        pnlAutoFields.Visible         = true;
                        txtLocalAutoDate.Text         = date;
                        txtLocalAutoFromTime.Text     = fromTime;
                        txtLocalAutoToTime.Text       = toTime;
                        txtLocalAutoParticular.Text   = particulars;
                        txtLocalAutoDistance.Text     = distance;
                        txtLocalAutoAmount.Text       = amount;
                        txtLocalAutoSONo.Text         = soNo;
                        txtLocalAutoSMONo.Text        = smoNo;
                        txtLocalAutoRefNo.Text        = refNo;
                        txtLocalAutoRemarks.Text      = remarks;
                    }
                }
                else if (category == "Lodging" || category == "Others")
                {
                    // Lodging is a Tour item; if Excel says Local+Lodging treat as Others
                    ddlLocalExpenseType.SelectedValue  = "Others";
                    pnlLocalOthersFields.Visible       = true;
                    txtLocalOthersDate.Text            = date;
                    txtLocalOthersFromTime.Text        = fromTime;
                    txtLocalOthersToTime.Text          = toTime;
                    txtLocalOthersParticulars.Text     = particulars;
                    txtLocalOthersAmount.Text          = amount;
                    txtLocalOthersSoNo.Text            = soNo;
                    txtLocalOthersSMONo.Text           = smoNo;
                    txtLocalOthersRefNo.Text           = refNo;
                    txtLocalOthersRemarks.Text         = remarks;
                }
            }
            else if (expType == "Tour")
            {
                pnlTourExpenses.Visible = true;

                if (category == "Food")
                {
                    ddlTourExpenseType.SelectedValue  = "Food";
                    pnlTourFoodFields.Visible          = true;
                    txtTourFoodDate.Text               = date;
                    txtTourFoodFromTime.Text           = fromTime;
                    txtTourFoodToTime.Text             = toTime;
                    txtTourFoodParticulars.Text        = particulars;
                    txtTourFoodAmount.Text             = amount;
                    txtTourFoodSONo.Text               = soNo;
                    txtTourFoodSMONo.Text              = smoNo;
                    txtTourFoodRefNo.Text              = refNo;
                    txtTourFoodRemarks.Text            = remarks;
                }
                else if (category == "Lodging")
                {
                    ddlTourExpenseType.SelectedValue  = "Lodging";
                    pnlTourOthersFields.Visible        = true;
                    txtTourOthersDate.Text             = date;
                    txtFromTimeTourOthers.Text         = fromTime;
                    txtToTimeTourOthers.Text           = toTime;
                    txtParticularsTourOthers.Text      = particulars;
                    txtTourOthersAmount.Text           = amount;
                    txtTourOthersSoNo.Text             = soNo;
                    txtTourOthersSmoNo.Text            = smoNo;
                    txtTourOthersRefNo.Text            = refNo;
                    txtRemarksTourOthers.Text          = remarks;
                }
                else if (category == "Conveyance")
                {
                    ddlTourExpenseType.SelectedValue  = "Conveyance";
                    pnlTourConvenience.Visible         = true;
                    ddlTourTransportMode.SelectedValue = transport;

                    if (transport == "Bike" || transport == "Auto")
                    {
                        pnlAutoTourFields.Visible         = true;
                        txtTourAutoDate.Text               = date;
                        txtTourAutoFromTime.Text           = fromTime;
                        txtTourAutoToTime.Text             = toTime;
                        txtTourAutoParticular.Text         = particulars;
                        txtTourAutoDistance.Text           = distance;
                        txtTourAutoAmount.Text             = amount;
                        ddlTourTransportMode.SelectedValue = "Auto";
                        txtTourAutoSoNo.Text               = soNo;
                        txtTourAutoSmoNo.Text              = smoNo;
                        txtTourAutoRefNo.Text              = refNo;
                        txTourAutoRemarks.Text             = remarks;
                    }
                    else if (transport == "Cab" || transport == "Cab/Bus")
                    {
                        pnlcabTourFields.Visible           = true;
                        txtCabDate.Text                    = date;
                        txtFromTimeCab.Text                = fromTime;
                        txtToTimeCab.Text                  = toTime;
                        txtParticularsCab.Text             = particulars;
                        txtCabAmount.Text                  = amount;
                        ddlTourTransportMode.SelectedValue = "Cab";
                        txtCabSoNo.Text                    = soNo;
                        txtCabSmoNo.Text                   = smoNo;
                        txtCabRefNo.Text                   = refNo;
                        txtRemarksCab.Text                 = remarks;
                    }
                    else if (transport == "Train")
                    {
                        pnlTrainFields.Visible             = true;
                        txtTrainDate.Text                   = date;
                        txtFromTimeTrain.Text               = fromTime;
                        txtToTimeTrain.Text                 = toTime;
                        txtParticularsTrain.Text            = particulars;
                        txtTrainAmount.Text                 = amount;
                        txtTrainSoNo.Text                   = soNo;
                        txtTrainSmoNo.Text                  = smoNo;
                        txtTrainRefNo.Text                  = refNo;
                        txtRemarksTrain.Text                = remarks;
                    }
                    else if (transport == "Bus")
                    {
                        pnlBusFields.Visible               = true;
                        txtBusDate.Text                    = date;
                        txtFromTimeBus.Text                = fromTime;
                        txtToTimeBus.Text                  = toTime;
                        txtParticularsBus.Text             = particulars;
                        txtBusAmount.Text                  = amount;
                        txtBusSoNo.Text                    = soNo;
                        txtBusSmoNo.Text                   = smoNo;
                        txtBusRefNo.Text                   = refNo;
                        txtRemarksBus.Text                 = remarks;
                    }
                    else if (transport == "Flight")
                    {
                        pnlFlightFields.Visible            = true;
                        txtFlightDate.Text                 = date;
                        txtFlightFromTime.Text             = fromTime;
                        txtFlightToTime.Text               = toTime;
                        txtFlightParticulars.Text          = particulars;
                        txtFlightAmount.Text               = amount;
                        txtFlightSoNo.Text                 = soNo;
                        txtFlightSmoNo.Text                = smoNo;
                        txtFlightRefNo.Text                = refNo;
                        txtFlightRemarks.Text              = remarks;
                    }
                }
            }
        }

        // ── Build the schema DataTable for Excel preview ──────────────────────────
        private DataTable BuildExcelPreviewSchema()
        {
            var dt = new DataTable("ExcelPreview");
            dt.Columns.Add("RowId",         typeof(int));
            dt.Columns.Add("ExpenseType",   typeof(string));
            dt.Columns.Add("Category",      typeof(string));  // Conveyance / Food / Lodging
            dt.Columns.Add("Date",          typeof(string));
            dt.Columns.Add("FromTime",      typeof(string));
            dt.Columns.Add("ToTime",        typeof(string));
            dt.Columns.Add("Particulars",   typeof(string));
            dt.Columns.Add("Distance",      typeof(string));
            dt.Columns.Add("TransportType", typeof(string));
            dt.Columns.Add("Amount",        typeof(string));
            dt.Columns.Add("SONo",          typeof(string));
            dt.Columns.Add("SMONo",         typeof(string));
            dt.Columns.Add("RefNo",         typeof(string));
            dt.Columns.Add("Remarks",       typeof(string));
            return dt;
        }

        // ── Bind (or hide) the preview GridView ────────────────────────────────────
        private void BindExcelPreview(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                gvExcelPreview.DataSource = null;
                gvExcelPreview.DataBind();
                pnlExcelPreview.Visible = false;
                return;
            }
            gvExcelPreview.DataSource = dt;
            gvExcelPreview.DataBind();
            pnlExcelPreview.Visible = true;
        }

        // ── Helper: resolve column index from a header dictionary ─────────────────
        private int GetColIndex(Dictionary<string, int> colMap, string[] candidates, int fallback)
        {
            foreach (var key in candidates)
            {
                if (colMap.TryGetValue(key, out int idx)) return idx;
                // Partial match
                foreach (var kvp in colMap)
                    if (kvp.Key.Contains(key) || key.Contains(kvp.Key)) return kvp.Value;
            }
            return fallback;
        }

        // ── Helper: try parse a positive decimal from a string ────────────────────
        private bool TryParsePositiveDecimal(string s, out decimal result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            string clean = System.Text.RegularExpressions.Regex.Replace(s, @"[^\d.]", "");
            if (decimal.TryParse(clean, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out result) && result > 0)
                return true;
            result = 0;
            return false;
        }

        // ── Helper: parse Excel date (handles numeric OA date, string, merged) ────
        private bool TryParseExcelDate(string rawText, object cellValue, out DateTime result)
        {
            result = DateTime.MinValue;

            // Try OLE Automation date (numeric value EPPlus gives for date cells)
            if (cellValue is double d)
            {
                try { result = DateTime.FromOADate(d); return true; } catch { }
            }

            // Try parsing the text directly with common Indian date formats
            string[] fmts = { "dd-MM-yyyy", "d-M-yyyy", "dd/MM/yyyy", "d/M/yyyy",
                               "yyyy-MM-dd", "MM/dd/yyyy", "dd-MMM-yyyy", "d-MMM-yyyy",
                               "dd.MM.yyyy", "d.M.yyyy" };
            foreach (var fmt in fmts)
            {
                if (DateTime.TryParseExact(rawText, fmt,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out result))
                    return true;
            }

            // General parse as last resort
            return DateTime.TryParse(rawText, out result);
        }

        // ── Helper: parse Excel time to "HH:mm" string ────────────────────────────
        private string ParseExcelTime(string rawText, object cellValue)
        {
            if (string.IsNullOrWhiteSpace(rawText)) return "";

            // EPPlus may give a double fraction of a day
            if (cellValue is double d && d >= 0 && d < 1)
            {
                try
                {
                    TimeSpan ts = TimeSpan.FromDays(d);
                    return ts.ToString(@"hh\:mm");
                }
                catch { }
            }

            // Text like "10:30", "10:30 AM", "10.30"
            rawText = rawText.Trim().Replace(".", ":");
            TimeSpan parsed;
            if (TimeSpan.TryParse(rawText, out parsed))
                return parsed.ToString(@"hh\:mm");

            // Try DateTime parse for "10:30 AM" style
            if (DateTime.TryParse(rawText, out DateTime dt))
                return dt.ToString("HH:mm");

            return rawText; // Return as-is and let SQL handle it
        }

        private string ExtractAmountFromCell(OfficeOpenXml.ExcelWorksheet worksheet, int row, int col)
        {
            var cellValue = worksheet.Cells[row, col].Value;
            if (cellValue == null) return "0";

            string cellStr = cellValue.ToString().Trim();
            if (string.IsNullOrEmpty(cellStr)) return "0";

            // FIX: Check for time format (contains colon) BEFORE stripping non-numerics
            if (cellStr.Contains(":")) return "0";

            // Remove currency symbols, commas, and other non-numeric chars except decimal point
            // This handles "$1,200.00", "Rs. 500", "500.00", etc.
            string cleanStr = System.Text.RegularExpressions.Regex.Replace(cellStr, @"[^\d.-]", "");

            // If it ends with a dot, remove it
            if (cleanStr.EndsWith(".")) cleanStr = cleanStr.Substring(0, cleanStr.Length - 1);

            if (decimal.TryParse(cleanStr, out decimal amount) && amount >= 0)
            {
                return amount.ToString();
            }

            return "0";
        }

        // Download Excel file from session
        public void DownloadExcelFile()
        {
            try
            {
                byte[] fileBytes = Session["UploadedExcelFileBytes"] as byte[];
                string fileName = Session["UploadedExcelFileName"] as string;

                if (fileBytes != null && !string.IsNullOrEmpty(fileName))
                {
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                    Response.BinaryWrite(fileBytes);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Error downloading file: {ex.Message}";
                lblError.ForeColor = System.Drawing.Color.Red;
            }
        }
        // ── Populate helpers called by PopulateFormForEdit ────────────────────────

        private void PopulateLocalFoodFields(string date, string amount, string particulars, string remarks, string smoNo, string soNo, string refNo)
        {
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d)) txtLocalFoodDate.Text = d.ToString("yyyy-MM-dd"); else txtLocalFoodDate.Text = date;
            txtLocalFoodAmount.Text = amount;
            txtLocalFoodParticulars.Text = particulars;
            txtLocalFoodRemarks.Text = remarks;
            txtLocalSMONo.Text = smoNo;
            txtLocalFoodSONo.Text = soNo;
            txtLocalRefNo.Text = refNo;
        }

        private void PopulateLocalMiscellaneousFields(string date, string amount, string particulars, string remarks, string smoNo, string soNo, string refNo)
        {
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d)) txtLocalMiscDate.Text = d.ToString("yyyy-MM-dd"); else txtLocalMiscDate.Text = date;
            txtLocalMiscAmount.Text = amount;
            txtLocalMiscItem.Text = particulars;
            txtLocalMiscRemarks.Text = remarks;
            txtLocalMiscSMONo.Text = smoNo;
            txtLocalMiscSONo.Text = soNo;
            txtLocalMiscRefNo.Text = refNo;
        }

        private void PopulateLocalOthersFields(string date, string amount, string particulars, string remarks, string smoNo, string soNo, string refNo)
        {
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d)) txtLocalOthersDate.Text = d.ToString("yyyy-MM-dd"); else txtLocalOthersDate.Text = date;
            txtLocalOthersAmount.Text = amount;
            txtLocalOthersParticulars.Text = particulars;
            txtLocalOthersRemarks.Text = remarks;
            txtLocalOthersSMONo.Text = smoNo;
            txtLocalOthersSoNo.Text = soNo;
            txtLocalOthersRefNo.Text = refNo;
        }

        private void PopulateLocalConveyanceFields(string date, string amount, string particulars, string remarks, string smoNo, string soNo, string refNo)
        {
            string mode = ddlLocalMode.SelectedValue;
            if (mode == "Bike")
            {
                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d)) txtLocalBikeDate.Text = d.ToString("yyyy-MM-dd"); else txtLocalBikeDate.Text = date;
                txtLocalAmount.Text = amount;
                txtLocalBikeParticular.Text = particulars;
                txtLocalBikeRemarks.Text = remarks;
                txtLocalBikeSMONo.Text = smoNo;
                txtLocalBikeSONo.Text = soNo;
                txtLocalBikeRefNo.Text = refNo;
            }
            else if (mode == "Cab/Bus")
            {
                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d)) txtLocalCabDate.Text = d.ToString("yyyy-MM-dd"); else txtLocalCabDate.Text = date;
                txtLocalCabAmount.Text = amount;
                txtLocalCabParticular.Text = particulars;
                txtLocalCabRemarks.Text = remarks;
                txtLocalCabSMONo.Text = smoNo;
                txtLocalCabSONo.Text = soNo;
                txtLocalCabRefNo.Text = refNo;
            }
            else if (mode == "Auto")
            {
                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d)) txtLocalAutoDate.Text = d.ToString("yyyy-MM-dd"); else txtLocalAutoDate.Text = date;
                txtLocalAutoAmount.Text = amount;
                txtLocalAutoParticular.Text = particulars;
                txtLocalAutoRemarks.Text = remarks;
                txtLocalAutoSMONo.Text = smoNo;
                txtLocalAutoSONo.Text = soNo;
                txtLocalAutoRefNo.Text = refNo;
            }
        }

        private void PopulateTourFoodFields(string date, string amount, string particulars, string remarks, string smoNo, string soNo, string refNo)
        {
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d)) txtTourFoodDate.Text = d.ToString("yyyy-MM-dd"); else txtTourFoodDate.Text = date;
            txtTourFoodAmount.Text = amount;
            txtTourFoodParticulars.Text = particulars;
            txtTourFoodRemarks.Text = remarks;
            txtTourFoodSMONo.Text = smoNo;
            txtTourFoodSONo.Text = soNo;
            txtTourFoodRefNo.Text = refNo;
        }

        private void PopulateTourMiscellaneousFields(string date, string amount, string particulars, string remarks, string smoNo, string soNo, string refNo)
        {
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d)) txtTourMiscDate.Text = d.ToString("yyyy-MM-dd"); else txtTourMiscDate.Text = date;
            txtTourMiscAmount.Text = amount;
            txtTourMiscItem.Text = particulars;
            txtTourMiscParticulars.Text = particulars;
            txtTourMiscRemarks.Text = remarks;
            txtTourMiscSmoNo.Text = smoNo;
            txtTourMiscSoNo.Text = soNo;
            txtTourMiscRefNo.Text = refNo;
        }

        private void PopulateTourOthersFields(string date, string amount, string particulars, string remarks, string smoNo, string soNo, string refNo)
        {
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d)) txtTourOthersDate.Text = d.ToString("yyyy-MM-dd"); else txtTourOthersDate.Text = date;
            txtTourOthersAmount.Text = amount;
            txtParticularsTourOthers.Text = particulars;
            txtRemarksTourOthers.Text = remarks;
            txtTourOthersSmoNo.Text = smoNo;
            txtTourOthersSoNo.Text = soNo;
            txtTourOthersRefNo.Text = refNo;
        }
    }
}
