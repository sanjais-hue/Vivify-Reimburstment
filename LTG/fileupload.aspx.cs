using System;
using System.IO;

namespace Vivify
{
    public partial class fileupload : System.Web.UI.Page
    {
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                try
                {
                    // Get the virtual path for the "Uploads" directory
                    string virtualFolderPath = "~/Uploads/";

                    // Convert the virtual path to a physical path
                    string physicalFolderPath = Server.MapPath(virtualFolderPath);

                    // Ensure the folder exists
                    if (!Directory.Exists(physicalFolderPath))
                    {
                        Directory.CreateDirectory(physicalFolderPath);
                    }

                    // Combine the physical path with the file name
                    string filePath = Path.Combine(physicalFolderPath, Path.GetFileName(FileUpload1.PostedFile.FileName));

                    // Save the file
                    FileUpload1.SaveAs(filePath);

                    // Update status
                    StatusLabel.Text = "Upload status: File uploaded successfully!";
                }
                catch (Exception ex)
                {
                    StatusLabel.Text = "Upload status: The file could not be uploaded. The following error occurred: " + ex.Message;
                }
            }
            else
            {
                StatusLabel.Text = "Upload status: No file selected.";
            }
        }
    }
}
