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
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", $"attachment; filename={Path.GetFileName(filePath)}");
                Response.TransmitFile(filePath);
                Response.End();
            }
        }
    }
}
