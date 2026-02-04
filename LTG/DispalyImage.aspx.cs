using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vivify
{
    public partial class DisplayImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int employeeId = GetCurrentEmployeeId(); // Implement logic to retrieve employee ID
                int serviceId = Convert.ToInt32(Session["ServiceId"]); // Assuming ServiceId is stored in session
                LoadClaimableImages(employeeId, serviceId);
            }
        }

        private void LoadClaimableImages(int employeeId, int serviceId)
        {
            string constr = ConfigurationManager.ConnectionStrings["vivify"].ConnectionString;
            bool hasClaimable = false;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"
            SELECT 
                c.Image AS ConveyanceImage,
                o.Image AS OthersImage,
                m.Image AS MiscellaneousImage,
                l.Image AS LodgingImage,
                c.IsClaimable AS IsClaimableConveyance,
                o.IsClaimable AS IsClaimableOthers,
                m.IsClaimable AS IsClaimableMiscellaneous,
                l.IsClaimable AS IsClaimableLodging
            FROM 
                Expense e
            LEFT JOIN Conveyance c ON e.ServiceId = c.ServiceId
            LEFT JOIN Others o ON e.ServiceId = o.ServiceId
            LEFT JOIN Miscellaneous m ON e.ServiceId = m.ServiceId
            LEFT JOIN Lodging l ON e.ServiceId = l.ServiceId
            WHERE 
                e.EmployeeId = @EmployeeId AND e.ServiceId = @ServiceId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Check each expense type
                            if (reader.GetBoolean(reader.GetOrdinal("IsClaimableConveyance")) && reader["ConveyanceImage"] != DBNull.Value)
                            {
                                byte[] imgData = (byte[])reader["ConveyanceImage"];
                                string base64String = Convert.ToBase64String(imgData);
                                string imgUrl = "data:image/png;base64," + base64String;

                                Image imgConveyance = new Image
                                {
                                    ImageUrl = imgUrl,
                                    CssClass = "image-container"
                                };
                                ImagePlaceholder.Controls.Add(imgConveyance);
                                hasClaimable = true;
                            }
                            if (reader.GetBoolean(reader.GetOrdinal("IsClaimableOthers")) && reader["OthersImage"] != DBNull.Value)
                            {
                                byte[] imgData = (byte[])reader["OthersImage"];
                                string base64String = Convert.ToBase64String(imgData);
                                string imgUrl = "data:image/png;base64," + base64String;

                                Image imgOthers = new Image
                                {
                                    ImageUrl = imgUrl,
                                    CssClass = "image-container"
                                };
                                ImagePlaceholder.Controls.Add(imgOthers);
                                hasClaimable = true;
                            }
                            if (reader.GetBoolean(reader.GetOrdinal("IsClaimableMiscellaneous")) && reader["MiscellaneousImage"] != DBNull.Value)
                            {
                                byte[] imgData = (byte[])reader["MiscellaneousImage"];
                                string base64String = Convert.ToBase64String(imgData);
                                string imgUrl = "data:image/png;base64," + base64String;

                                Image imgMiscellaneous = new Image
                                {
                                    ImageUrl = imgUrl,
                                    CssClass = "image-container"
                                };
                                ImagePlaceholder.Controls.Add(imgMiscellaneous);
                                hasClaimable = true;
                            }
                            if (reader.GetBoolean(reader.GetOrdinal("IsClaimableLodging")) && reader["LodgingImage"] != DBNull.Value)
                            {
                                byte[] imgData = (byte[])reader["LodgingImage"];
                                string base64String = Convert.ToBase64String(imgData);
                                string imgUrl = "data:image/png;base64," + base64String;

                                Image imgLodging = new Image
                                {
                                    ImageUrl = imgUrl,
                                    CssClass = "image-container"
                                };
                                ImagePlaceholder.Controls.Add(imgLodging);
                                hasClaimable = true;
                            }
                        }
                    }
                }
            }

            if (!hasClaimable)
            {
                lblMessage.Text = "No claimable expenses found.";
                lblMessage.Visible = true;
            }
        }

        private int GetCurrentEmployeeId()
        {
            // Implement your logic to retrieve the current employee ID
            return 1; // Placeholder value; replace with actual logic
        }
    }
}
