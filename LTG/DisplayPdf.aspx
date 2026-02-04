<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayPdf.aspx.cs" Inherits="Vivify.DisplayPdf" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <title>Select Images</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
</head>
<body>
    <div class="container">
        <h2>Select Images to Download</h2>
        <form id="imageSelectionForm">
            <div id="imageContainer"></div>
            <button type="button" class="btn btn-primary" id="btnDownloadSelected">Download Selected Images</button>
        </form>
    </div>

    <script>
        $(document).ready(function () {
            // Assume 'imageData' is passed from the server as a JavaScript object
            let imageData = <%= new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(ViewState["ImageData"]) %>;

            imageData.forEach(function (image) {
                $('#imageContainer').append(`
                    <div>
                        <img src="${image.src}" style="width: 100px; height: auto;" />
                        <label>
                            <input type="checkbox" class="image-checkbox" value="${image.src}"> ${image.label}
                        </label>
                    </div>
                `);
            });

            $("#btnDownloadSelected").click(function () {
                let selectedImages = [];
                $(".image-checkbox:checked").each(function () {
                    selectedImages.push($(this).val());
                });

                $.ajax({
                    type: 'POST',
                    url: 'DocView.aspx/DownloadSelectedImages',
                    data: JSON.stringify({ images: selectedImages }),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (response) {
                        window.open(response.d);
                    },
                    error: function (xhr, status, error) {
                        alert("Error: " + xhr.responseText);
                    }
                });
            });
        });
    </script>
</body>
</html>
