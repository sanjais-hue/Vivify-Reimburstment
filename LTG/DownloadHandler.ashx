<%@ WebHandler Language="C#" Class="DownloadHandler" %>

using System;
using System.Web;

public class DownloadHandler : IHttpHandler
{
  public void ProcessRequest(HttpContext context)
{
    string disposition = context.Request.QueryString["disposition"] ?? "inline"; 
    context.Response.ContentType = "application/pdf";
    context.Response.AddHeader("content-disposition", $"{disposition}; filename=document.pdf");

    string base64PdfData = context.Request.Form["pdfData"];
    if (!string.IsNullOrEmpty(base64PdfData))
    {
        byte[] pdfBytes = Convert.FromBase64String(base64PdfData);
        context.Response.BinaryWrite(pdfBytes);
    }
    else
    {
        context.Response.Write("No PDF data found.");
    }
    context.Response.End();
}

    public bool IsReusable => false;
}