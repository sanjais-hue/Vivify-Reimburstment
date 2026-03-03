using System;
using System.IO;

namespace Vivify
{
    public partial class DisplayPdf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string filePath = Request.QueryString["filePath"];
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                //pdfFrame.Attributes["src"] = filePath;
            }
            else
            {
                Response.Write("PDF file not found.");
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            string filePath = Request.QueryString["filePath"];
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename=UserGuide.pdf");
                Response.TransmitFile(filePath);
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
    }
}
