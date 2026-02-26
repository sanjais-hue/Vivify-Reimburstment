<%@ Page Title="" Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="Expenses.aspx.cs"
    Inherits="Vivify.Expenses" Async="true" %>


    <asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <script type="text/javascript">
            function confirmSubmission() {
                return confirm("Are you sure you want to Save?");
            }
            function confirmSubmit(statusid) {
                if (statusid !== 2) {
                    return confirm("Are you sure you want to submit?");
                } else {
                    return true;
                }
            }

            function showAlert(message) {
                alert(message);
            }
            const fileUrls = {};

            function handleFileSelect(input, textBoxId, removeBtnId) {
                if (input.files && input.files[0]) {
                    const file = input.files[0];
                    document.getElementById(textBoxId).value = file.name;

                    if (fileUrls[input.id]) URL.revokeObjectURL(fileUrls[input.id]);
                    fileUrls[input.id] = URL.createObjectURL(file);

                    document.getElementById(removeBtnId).style.display = "inline-block";
                }
            }

            function triggerOrOpen(fileInputId, textBoxId) {
                const fileInput = document.getElementById(fileInputId);
                if (!fileUrls[fileInputId]) {
                    fileInput.click();
                } else {
                    window.open(fileUrls[fileInputId], "_blank");
                }
            }

            function removeFile(fileInputId, textBoxId, removeBtnId) {
                const fileInput = document.getElementById(fileInputId);
                const textBox = document.getElementById(textBoxId);

                fileInput.value = "";
                textBox.value = "";

                if (fileUrls[fileInputId]) {
                    URL.revokeObjectURL(fileUrls[fileInputId]);
                    delete fileUrls[fileInputId];
                }

                document.getElementById(removeBtnId).style.display = "none";
                document.getElementById('btnViewExcelFile').style.display = "none";

                // Clear localStorage for Excel file
                if (textBoxId === 'txtExcelFileName') {
                    localStorage.removeItem('excelFileName');
                    localStorage.removeItem('excelFileRemoveBtn');
                }
            }

            // Auto-import Excel file when selected
            function handleExcelFileSelect(fileInput, textBoxId, removeBtnId) {
                if (fileInput.files && fileInput.files[0]) {
                    const file = fileInput.files[0];
                    const fileName = file.name;
                    document.getElementById(textBoxId).value = fileName;

                    document.getElementById(removeBtnId).style.display = "inline-block";
                    document.getElementById('btnViewExcelFile').style.display = "inline-block";

                    // Save filename to localStorage before page refresh
                    localStorage.setItem('excelFileName', fileName);
                    localStorage.setItem('excelFileRemoveBtn', 'show');

                    // Trigger the server-side import by clicking the hidden button
                    document.getElementById('<%= btnImportExcel.ClientID %>').click();
                }
            }

            // Download Excel file when filename is clicked
            function downloadExcelFile() {
                const fileInput = document.getElementById('<%= fileUploadExcel.ClientID %>');
                fileInput.click();
            }

            // View and Download Excel file
            function viewAndDownloadExcelFile() {
                // Trigger server-side download
                window.location.href = '?action=download';
            }

            // Restore filename from localStorage after page loads
            window.addEventListener('load', function () {
                const savedFileName = localStorage.getItem('excelFileName');
                const showRemoveBtn = localStorage.getItem('excelFileRemoveBtn');

                if (savedFileName) {
                    document.getElementById('txtExcelFileName').value = savedFileName;
                    if (showRemoveBtn === 'show') {
                        document.getElementById('btnRemoveExcelFile').style.display = 'inline-block';
                        document.getElementById('btnViewExcelFile').style.display = 'inline-block';
                    }
                }
            });



        </script>

        <style aria-atomic="True">
            #ddlExpenseType {
                display: flex;
                border: 2px solid red;
                /* Add red border to the dropdown */
                border-radius: 5px;
                /* Optional: Rounded corners */
            }

            .form-container {
                padding: 20px;
                margin-top: 10px;
                border-radius: 5px;
                box-shadow: 0 2px 10px darkblue;
            }

            .card-title {
                text-align: left;
                color: midnightblue;
                margin-bottom: 0;
                /* Remove bottom margin */
                font-size: 1.5rem;
                /*font-family: 'Times New Roman', Times, serif;*/
                font-weight: bold;
                position: relative;
                /* Position for the pseudo-element */
            }

            .card-title.underline::after {
                content: "";
                /* Create an underline */
                display: block;
                /* Make it a block element */
                width: 100%;
                /* Full width */
                height: 2px;
                /* Thickness of the underline */
                background-color: darkslategrey;
                /* Color of the underline */
                position: absolute;
                /* Position it below the text */
                left: 0;
                /* Align to the left */
                bottom: 10px;
                /* Adjust this value to control the space below the text */
                box-shadow: 0 1px 2px rgba(0, 0, 0, 0.5);
            }

            .grid-header {
                background-color: #1f2d64;
                /* Header background color */
                color: white;
                font-weight: bold;
                text-align: center;
                padding: 10px;
                border: 1px solid red;
                /* Header border color */
            }

            .custom-grid {
                width: 70%;
                border-collapse: collapse;
                /* Collapse borders */
                margin: 0 auto;
                text-align: center;
                border: 1px solid darkblue;
                /* Table border color */
                box-shadow: 0 4px 15px darkblue;
                /* Add box shadow to the table */
            }

            .custom-grid td {
                background-color: transparent;
                /* Set table cell background color to transparent */
                border: 1px solid darkblue;
                /* Cell border color */
            }

            .custom-grid th {
                background-color: #1f2d64;
                /* Header background color */
                color: white;
                padding: 10px;
                border: 1px solid black;
                /* Header cell border color */
            }

            .custom-grid tr:hover {
                background-color: #eaeaea;
                /* Highlight row on hover */
            }

            .custom-grid td {
                background-color: transparent;
                border: 1px solid darkblue;
                padding: 8px;
            }

            .form-control {
                width: 100%;
                padding: 5px;
                margin-bottom: 15px;
                border-radius: 4px;
                border: 2px solid darkblue;
            }

            .custom-file-upload {
                display: flex;
                align-items: center;
                gap: 8px;
            }

            .custom-file-upload input[type="text"] {
                flex: 1;
                cursor: pointer;
            }

            .btn-primary {
                background-color: #3f418d;
                color: white;


                border-radius: 10px;
                cursor: pointer;

            }

            .btn-primary:hover {
                background-color: #3f418d;
            }

            /* Save Button - Green */
            .btn-submit {
                background-color: #218838;
                /* Green */
                padding: 10px;
                color: white;
                border-color: green;
                border-radius: 10px;
            }

            .btn-submit:hover {
                background-color: #218838;
                /* Darker Green */

            }

            /* Cancel Button - Red-Orange */
            .btn-cancel {
                background-color: #e03b00;
                /* Red-Orange */
                border-color: #e03b00;
                color: white;
            }

            .custom-button {
                background-color: #3f418d;
                color: white;
                border-color: midnightblue;
                padding: 10px 20px;
                font-weight: bold;
                border-radius: 8px;
            }

            .btn-cancel:hover {
                background-color: #e03b00;
                /* Darker Red-Orange */
                border-color: #d63500;
            }

            .form-label {
                font-weight: bold;
                color: midnightblue;
                margin-bottom: 0;
                /* Remove bottom margin */
                font-size: 1.3rem;
                font-family: 'Times New Roman', Times, serif;
            }

            .main {
                margin: 0;
                padding: 0;
                background-color: #cadcfc;
            }

            .centered-card {
                width: 900px;
                margin: 0 auto;
            }

            /* Mobile view adjustments */
            @media only screen and (max-width: 768px) {
                #ddlExpenseType {
                    width: 100%;
                    /* Make dropdown full width */
                    border-radius: 4px;
                    /* Rounded corners for small screens */
                    border: 1px solid #ccc;
                    /* Lighter border for mobile */
                }

                .form-container {
                    padding: 15px;
                    /* Reduce padding for smaller screens */
                    margin-top: 10px;
                }

                .card-title {
                    font-size: 1.25rem;
                    /* Reduce font size */
                    text-align: center;
                    /* Center title for mobile */
                }

                .custom-grid {
                    width: 100%;
                    /* Make table full width */
                    border: darkblue;
                    /* Remove borders for mobile */
                    box-shadow: none;
                    /* Remove shadow */
                }

                .custom-grid td,
                .custom-grid th {
                    padding: 10px;
                    /* Adjust cell padding */
                }

                .form-control {
                    padding: 8px;
                    /* Increase padding for touch targets */
                }

                .btn-primary {
                    width: 100%;
                    /* Button full width */
                    padding: 12px 0;
                    /* Increase button height */
                }

                .form-label {
                    font-size: 1.1rem;
                    /* Reduce font size for labels */
                }

                .centered-card {
                    width: 100%;
                    /* Make card full width */
                    padding: 15px;
                    /* Add padding inside the card */
                }
            }

            @media only screen and (max-width: 480px) {
                .form-container {
                    padding: 10px;
                    /* Reduce padding further for very small screens */
                }

                .card-title {
                    font-size: 1rem;
                    /* Further reduce font size */
                }

                .form-label {
                    font-size: 1rem;
                    /* Reduce label font size further */
                }

                .btn-primary {
                    padding: 10px 0;
                    /* Smaller button padding for mobile */
                }

                .centered-card {
                    width: 100%;
                    /* Ensure card takes up full width */
                    padding: 10px;
                    /* Less padding for smaller screens */
                }
            }
        </style>

        <main id="main" class="main">
            <div class="formarea">
                <section class="section dashboard">
                    <div class="row">
                        <div class="col">
                            <div class="card centered-card">
                                <div
                                    style="display: flex; align-items: center; justify-content: space-between; background-color:#3f418d; color:white; padding: 12px 20px; border-radius: 5px 5px 0 0;">
                                    <h5 class="card-title" style="text-align:center; flex: 1; margin: 0; color: white;">
                                        Expenses Type</h5>
                                    <div
                                        style="display: flex; align-items: center; gap: 8px; white-space: nowrap; position: relative;">
                                        <label for="fileUploadExcel"
                                            style="margin: 0; font-size: 16px; font-weight: 700; color: white;">Upload
                                            Excel:</label>
                                        <!-- Hidden ASP.NET FileUpload -->
                                        <asp:FileUpload ID="fileUploadExcel" runat="server" accept=".xlsx,.xls"
                                            style="display:none;"
                                            onchange="handleExcelFileSelect(this, 'txtExcelFileName', 'btnRemoveExcelFile')" />
                                        <!-- Display field (click = download and open) -->
                                        <input type="text" id="txtExcelFileName"
                                            style="padding: 4px 6px; border: 1px solid white; border-radius: 3px; font-size: 11px; width: 250px; cursor: pointer; background-color: white; color: #333;"
                                            readonly placeholder="Choose file..." onclick="downloadExcelFile()" />
                                        <!-- Remove button -->
                                        <button type="button" id="btnRemoveExcelFile"
                                            style="display:none; border:none; background:none; font-size:18px; font-weight:bold; line-height:16px; text-align:center; cursor:pointer; color:white; padding: 0; margin: 0;"
                                            title="Remove File"
                                            onclick="removeFile('<%= fileUploadExcel.ClientID %>', 'txtExcelFileName', 'btnRemoveExcelFile')">×</button>
                                        <!-- View button -->
                                        <button type="button" id="btnViewExcelFile"
                                            style="display:none; border:none; background-color:#4CAF50; color:white; padding:6px 12px; border-radius:3px; cursor:pointer; font-size:12px; font-weight:bold;"
                                            title="View and Download File"
                                            onclick="viewAndDownloadExcelFile()">View</button>
                                        <!-- Hidden button to trigger import -->
                                        <asp:Button ID="btnImportExcel" runat="server" Text="Import"
                                            OnClick="btnImportExcel_Click" style="display: none;" />
                                    </div>
                                </div>
                                <section class="form-container">
                                    <asp:HiddenField ID="hdnEditRecordId" runat="server" />
                                    <asp:HiddenField ID="hdnEditCategory" runat="server" />


                                    <div class="col-12 mb-1">
                                        <label for="ddlExpenseType" class="form-label">Expense Type</label>
                                        <asp:DropDownList ID="ddlExpenseType" runat="server" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlExpenseType_SelectedIndexChanged"
                                            CssClass="form-control">
                                            <asp:ListItem Text="Select Expense Type" Value="" />
                                            <asp:ListItem Text="Local" Value="Local" />
                                            <asp:ListItem Text="Tour" Value="Tour" />
                                            <asp:ListItem Text="Award" Value="Award" />
                                        </asp:DropDownList>
                                    </div>

                                    <!-- Local Expenses Panel -->
                                    <asp:Panel ID="pnlLocalExpenses" runat="server" Visible="false"
                                        CssClass="panel-fields">
                                        <div class="col-12">
                                            <label for="ddlLocalExpenseType" class="form-label">Expense Details</label>
                                            <asp:DropDownList ID="ddlLocalExpenseType" runat="server"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlLocalExpenseType_SelectedIndexChanged"
                                                CssClass="form-control">
                                                <asp:ListItem Text="Select Local Expense Type" Value="" />
                                                <asp:ListItem Text="Conveyance" Value="Conveyance" />
                                                <asp:ListItem Text="Food" Value="Food" />
                                                <asp:ListItem Text="Others" Value="Others" />
                                                <asp:ListItem Text="Miscellaneous" Value="Miscellaneous" />
                                            </asp:DropDownList>
                                        </div>

                                        <asp:Panel ID="pnlLocalFoodFields" runat="server" Visible="false"
                                            CssClass="panel-fields">
                                            <div class="col-12">
                                                <label for="txtLocalFoodDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtLocalFoodDate" runat="server"
                                                    CssClass="form-control" TextMode="Date" autocomplete="off" />
                                                <asp:RequiredFieldValidator ID="rfvLocalFoodDate" runat="server"
                                                    ControlToValidate="txtLocalFoodDate" ErrorMessage="Date is required"
                                                    CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalFoodFromTime" class="form-label">From Time</label>
                                                <asp:TextBox ID="txtLocalFoodFromTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" AutoPostBack="true"
                                                    OnTextChanged="txtFromTime_TextChanged" autocomplete="off" />
                                                <asp:RequiredFieldValidator ID="rfvLocalFoodFromTime" runat="server"
                                                    ControlToValidate="txtLocalFoodFromTime"
                                                    ErrorMessage="From time is required" CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalFoodToTime" class="form-label">To Time</label>
                                                <asp:TextBox ID="txtLocalFoodToTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" AutoPostBack="true"
                                                    OnTextChanged="txtToTime_TextChanged" autocomplete="off" />
                                                <asp:RequiredFieldValidator ID="rfvLocalFoodToTime" runat="server"
                                                    ControlToValidate="txtLocalFoodToTime"
                                                    ErrorMessage="To time is required" CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalFoodAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtLocalFoodAmount" runat="server"
                                                    CssClass="form-control" ReadOnly="true" autocomplete="off" />
                                                <asp:RequiredFieldValidator ID="rfvLocalFoodAmount" runat="server"
                                                    ControlToValidate="txtLocalFoodAmount"
                                                    ErrorMessage="Amount is required" CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalFoodParticulars"
                                                    class="form-label">Particulars</label>
                                                <asp:TextBox ID="txtLocalFoodParticulars" runat="server"
                                                    CssClass="form-control" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalFoodRemarks" class="form-label">Remarks</label>
                                                <asp:TextBox ID="txtLocalFoodRemarks" runat="server"
                                                    CssClass="form-control" />
                                            </div>


                                            <!-- New SMO No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalSMONo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtLocalSMONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalSMONo" runat="server"
                                                    ControlToValidate="txtLocalSMONo" ErrorMessage="SMO No is required"
                                                    CssClass="text-danger" />
                                            </div>

                                            <!-- New SONO Field -->
                                            <div class="col-12">
                                                <label for="txtLocalFoodSONo" class="form-label">SO/SAP NO</label>
                                                <asp:TextBox ID="txtLocalFoodSONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalSONo" runat="server"
                                                    ControlToValidate="txtLocalFoodSONo" ErrorMessage="SONO is required"
                                                    CssClass="text-danger" />
                                            </div>

                                            <!-- New Ref No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtLocalRefNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalRefNo" runat="server"
                                                    ControlToValidate="txtLocalRefNo" ErrorMessage="Ref No is required"
                                                    CssClass="text-danger" />
                                            </div>
                                        </asp:Panel>
                                    </asp:Panel>


                                    <asp:Panel ID="pnlLocalMiscellaneousFields" runat="server" Visible="false"
                                        CssClass="panel-fields">
                                        <div class="col-12">
                                            <label for="txtLocalMiscDate" class="form-label">Date</label>
                                            <asp:TextBox ID="txtLocalMiscDate" runat="server" CssClass="form-control"
                                                TextMode="Date" />
                                            <asp:RequiredFieldValidator ID="rfvLocalMiscDate" runat="server"
                                                ControlToValidate="txtLocalMiscDate" ErrorMessage="Date is required"
                                                CssClass="text-danger" />
                                        </div>


                                        <div class="col-12">
                                            <label for="txtLocalMiscAmount" class="form-label">Amount</label>
                                            <asp:TextBox ID="txtLocalMiscAmount" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvLocalMiscAmount" runat="server"
                                                ControlToValidate="txtLocalMiscAmount" ErrorMessage="Amount is required"
                                                CssClass="text-danger" />
                                        </div>
                                        <div class="col-12">
                                            <label for="txtLocalMiscItem" class="form-label">Purchased Item</label>
                                            <asp:TextBox ID="txtLocalMiscItem" runat="server" CssClass="form-control"
                                                OnTextChanged="txtLocalMiscItem_TextChanged" />
                                        </div>
                                        <div class="col-12">
                                            <label for="fileUploadLocalMiscellaneous" class="form-label">Upload
                                                Bill</label>
                                            <div class="custom-file-upload position-relative">
                                                <!-- Hidden ASP.NET FileUpload -->
                                                <asp:FileUpload ID="fileUploadLocalMiscellaneous" runat="server"
                                                    style="display:none;"
                                                    onchange="handleFileSelect(this, 'txtFileName', 'btnRemoveFile')"
                                                    accept="image/*,application/pdf" />

                                                <!-- Fake input (click = browse OR open) -->
                                                <input type="text" id="txtFileName" class="form-control" readonly
                                                    placeholder="Choose file..."
                                                    onclick="triggerOrOpen('<%= fileUploadLocalMiscellaneous.ClientID %>', 'txtFileName')" />

                                                <!-- Remove button -->
                                                <button type="button" id="btnRemoveFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                    onclick="removeFile('<%= fileUploadLocalMiscellaneous.ClientID %>', 'txtFileName', 'btnRemoveFile')">
                                                </button>
                                            </div>
                                        </div>
                                        <div class="col-12">
                                            <label for="txtLocalMiscFromTime" class="form-label">From Time</label>
                                            <asp:TextBox ID="txtLocalMiscFromTime" runat="server"
                                                CssClass="form-control" TextMode="Time" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtLocalMiscToTime" class="form-label">To Time</label>
                                            <asp:TextBox ID="txtLocalMiscToTime" runat="server" CssClass="form-control"
                                                TextMode="Time" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtLocalMiscParticulars" class="form-label">Particulars</label>
                                            <asp:TextBox ID="txtLocalMiscParticulars" runat="server"
                                                CssClass="form-control" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtLocalMiscRemarks" class="form-label">Remarks</label>
                                            <asp:TextBox ID="txtLocalMiscRemarks" runat="server"
                                                CssClass="form-control" />
                                        </div>

                                        <!-- New SMO No Field -->
                                        <div class="col-12">
                                            <label for="txtLocalMiscSMONo" class="form-label">SMO/WBS No</label>
                                            <asp:TextBox ID="txtLocalMiscSMONo" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvLocalMiscSMONo" runat="server"
                                                ControlToValidate="txtLocalMiscSMONo" ErrorMessage="SMO No is required"
                                                CssClass="text-danger" />
                                        </div>

                                        <!-- New SO No Field -->
                                        <div class="col-12">
                                            <label for="txtLocalMiscSONo" class="form-label">SO/SAP No</label>
                                            <asp:TextBox ID="txtLocalMiscSONo" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvLocalMiscSONo" runat="server"
                                                ControlToValidate="txtLocalMiscSONo" ErrorMessage="SO No is required"
                                                CssClass="text-danger" />
                                        </div>

                                        <!-- New Ref No Field -->
                                        <div class="col-12">
                                            <label for="txtLocalMiscRefNo" class="form-label">Ref No</label>
                                            <asp:TextBox ID="txtLocalMiscRefNo" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvLocalMiscRefNo" runat="server"
                                                ControlToValidate="txtLocalMiscRefNo" ErrorMessage="Ref No is required"
                                                CssClass="text-danger" />
                                        </div>


                                    </asp:Panel>

                                    <asp:Panel ID="pnlLocalOthersFields" runat="server" Visible="false"
                                        CssClass="panel-fields">
                                        <div class="col-12">
                                            <label for="txtLocalOthersDate" class="form-label">Date</label>
                                            <asp:TextBox ID="txtLocalOthersDate" runat="server" CssClass="form-control"
                                                TextMode="Date" />

                                        </div>
                                        <div class="col-12">
                                            <label for="txtLocalOthersAmount" class="form-label">Amount</label>
                                            <asp:TextBox ID="txtLocalOthersAmount" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvLocalOthersAmount" runat="server"
                                                ControlToValidate="txtLocalOthersAmount"
                                                ErrorMessage="Amount is required" CssClass="text-danger" />
                                        </div>
                                        <div class="col-12">
                                            <label for="fileUploadLocalBill" class="form-label">Upload Bill</label>

                                            <div class="custom-file-upload position-relative">
                                                <!-- Hidden ASP.NET FileUpload -->
                                                <asp:FileUpload ID="fileUploadLocalBill" runat="server"
                                                    style="display:none;"
                                                    onchange="handleFileSelect(this, 'txtBillFileName', 'btnRemoveBillFile')"
                                                    accept="image/*,application/pdf" />

                                                <!-- Fake textbox (click ? browse or open) -->
                                                <input type="text" id="txtBillFileName" class="form-control" readonly
                                                    placeholder="Choose file..."
                                                    onclick="triggerOrOpen('<%= fileUploadLocalBill.ClientID %>', 'txtBillFileName')"
                                                    style="padding-right:35px;" />

                                                <!-- Remove button inside input -->
                                                <button type="button" id="btnRemoveBillFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                    onclick="removeFile('<%= fileUploadLocalBill.ClientID %>', 'txtBillFileName', 'btnRemoveBillFile')">
                                                </button>
                                            </div>
                                        </div>


                                        <div class="col-12">
                                            <label for="txtLocalOthersFromTime" class="form-label">From Time</label>
                                            <asp:TextBox ID="txtLocalOthersFromTime" runat="server"
                                                CssClass="form-control" TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="rfvLocalOthersFromTime" runat="server"
                                                ControlToValidate="txtLocalOthersFromTime"
                                                ErrorMessage="From Time is required" CssClass="text-danger" />

                                        </div>
                                        <div class="col-12">
                                            <label for="txtLocalOthersToTime" class="form-label">To Time</label>
                                            <asp:TextBox ID="txtLocalOthersToTime" runat="server"
                                                CssClass="form-control" TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="rfvLocalOthersToTime" runat="server"
                                                ControlToValidate="txtLocalOthersToTime"
                                                ErrorMessage="To Time is required" CssClass="text-danger" />

                                        </div>
                                        <div class="col-12">
                                            <label for="txtLocalOthersParticulars"
                                                class="form-label">Particulars</label>
                                            <asp:TextBox ID="txtLocalOthersParticulars" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvLocalOthersParticulars" runat="server"
                                                ControlToValidate="txtLocalOthersParticulars"
                                                ErrorMessage="Particulars are required" CssClass="text-danger" />
                                        </div>
                                        <div class="col-12">
                                            <label for="txtLocalOthersRemarks" class="form-label">Remarks</label>
                                            <asp:TextBox ID="txtLocalOthersRemarks" runat="server"
                                                CssClass="form-control" TextMode="MultiLine" />
                                        </div>
                                        <div class="col-12">
                                            <label for="fileServiceReport" class="form-label">Service Report</label>

                                            <div class="custom-file-upload position-relative">
                                                <!-- Hidden ASP.NET FileUpload -->
                                                <asp:FileUpload ID="fileServiceReport" runat="server"
                                                    style="display:none;"
                                                    onchange="handleFileSelect(this, 'txtServiceReportFileName', 'btnRemoveServiceReportFile')"
                                                    accept="image/*,application/pdf" />

                                                <!-- Fake textbox -->
                                                <input type="text" id="txtServiceReportFileName" class="form-control"
                                                    readonly placeholder="Choose file..."
                                                    onclick="triggerOrOpen('<%= fileServiceReport.ClientID %>', 'txtServiceReportFileName')" />

                                                <!-- Remove button -->
                                                <button type="button" id="btnRemoveServiceReportFile" style="position:absolute; right:6px; top:4px; display:none;
   
    border:none;background:none;
       font-size:30px; font-weight:bold; line-height:24px;
       text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                    onclick="removeFile('<%= fileServiceReport.ClientID %>', 'txtServiceReportFileName', 'btnRemoveServiceReportFile')">
                                                </button>
                                            </div>


                                        </div>



                                        <!-- New SMO No Field -->
                                        <div class="col-12">
                                            <label for="txtLocalOthersSMONo" class="form-label">SMO/WBS No</label>
                                            <asp:TextBox ID="txtLocalOthersSMONo" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvLocalOthersSMONo" runat="server"
                                                ControlToValidate="txtLocalOthersSMONo"
                                                ErrorMessage="SMO No is required" CssClass="text-danger" />
                                        </div>

                                        <!-- New SO No Field -->
                                        <div class="col-12">
                                            <label for="txtLocalOthersSoNo" class="form-label">SO/SAP No</label>
                                            <asp:TextBox ID="txtLocalOthersSoNo" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvLocalOthersSoNo" runat="server"
                                                ControlToValidate="txtLocalOthersSoNo" ErrorMessage="SO No is required"
                                                CssClass="text-danger" />
                                        </div>

                                        <!-- New Ref No Field -->
                                        <div class="col-12">
                                            <label for="txtLocalOthersRefNo" class="form-label">Ref No</label>
                                            <asp:TextBox ID="txtLocalOthersRefNo" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvLocalOthersRefNo" runat="server"
                                                ControlToValidate="txtLocalOthersRefNo"
                                                ErrorMessage="Ref No is required" CssClass="text-danger" />
                                        </div>
                                        <div class="col-12">
                                            <label for="othersfileUploadApproval" class="form-label">Approval
                                                Mail</label>

                                            <div class="custom-file-upload position-relative">
                                                <!-- Hidden ASP.NET FileUpload -->
                                                <asp:FileUpload ID="othersfileUploadApproval" runat="server"
                                                    style="display:none;"
                                                    onchange="handleFileSelect(this, 'txtOthersApprovalFileName', 'btnRemoveOthersApprovalFile')"
                                                    accept="image/*,application/pdf" />

                                                <!-- Fake textbox -->
                                                <input type="text" id="txtOthersApprovalFileName" class="form-control"
                                                    readonly placeholder="Choose file..."
                                                    onclick="triggerOrOpen('<%= othersfileUploadApproval.ClientID %>', 'txtOthersApprovalFileName')" />

                                                <!-- Remove button -->
                                                <button type="button" id="btnRemoveOthersApprovalFile" style="position:absolute; right:6px; top:4px; display:none;
               
                border:none;background:none;
                   font-size:30px; font-weight:bold; line-height:24px;
                   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                    onclick="removeFile('<%= othersfileUploadApproval.ClientID %>', 'txtOthersApprovalFileName', 'btnRemoveOthersApprovalFile')">
                                                </button>
                                            </div>

                                            <!-- Required validator -->
                                            <asp:RequiredFieldValidator ID="rfvOthersApprovalUpload" runat="server"
                                                ControlToValidate="othersfileUploadApproval"
                                                ErrorMessage="Approval Mail upload is required"
                                                CssClass="text-danger" />
                                        </div>

                                    </asp:Panel>

                                    <asp:Panel ID="pnlLocalConvenience" runat="server" Visible="false"
                                        CssClass="panel-fields">

                                        <div class="col-12">
                                            <label for="ddlLocalMode" class="form-label">Mode of Transport</label>
                                            <asp:DropDownList ID="ddlLocalMode" runat="server" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlLocalMode_SelectedIndexChanged"
                                                CssClass="form-control">
                                                <asp:ListItem Text="Select Mode of Transport" Value="" />
                                                <asp:ListItem Text="Bike" Value="Bike" />
                                                <asp:ListItem Text="Cab/Bus" Value="Cab/Bus" />
                                                <asp:ListItem Text="Auto" Value="Auto" />
                                            </asp:DropDownList>
                                        </div>


                                        <!-- Bike Panel -->
                                        <asp:Panel ID="pnlBikeFields" runat="server" Visible="false"
                                            CssClass="panel-fields">

                                            <div class="col-12">
                                                <label for="txtLocalBikeDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtLocalBikeDate" runat="server"
                                                    CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvLocalBikeDate" runat="server"
                                                    ControlToValidate="txtLocalBikeDate" ErrorMessage="Date is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalDistance" class="form-label">Distance (in
                                                    km)</label>
                                                <asp:TextBox ID="txtLocalDistance" runat="server"
                                                    CssClass="form-control" AutoPostBack="true"
                                                    OnTextChanged="txtLocalDistance_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvLocalDistance" runat="server"
                                                    ControlToValidate="txtLocalDistance"
                                                    ErrorMessage="Distance is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtLocalAmount" runat="server" CssClass="form-control"
                                                    ReadOnly="true" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalBikeFromTime" class="form-label">From Time</label>
                                                <asp:TextBox ID="txtLocalBikeFromTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                                                    ControlToValidate="txtLocalBikeFromTime"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalBikeToTime" class="form-label">To Time</label>
                                                <asp:TextBox ID="txtLocalBikeToTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"
                                                    ControlToValidate="txtLocalBikeToTime"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalBikeParticular"
                                                    class="form-label">Particulars</label>
                                                <asp:TextBox ID="txtLocalBikeParticular" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalBikeParticular" runat="server"
                                                    ControlToValidate="txtLocalBikeParticular"
                                                    ErrorMessage="Particular is required" CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalBikeRemarks" class="form-label">Remarks</label>
                                                <asp:TextBox ID="txtLocalBikeRemarks" runat="server"
                                                    CssClass="form-control" TextMode="MultiLine" />
                                                <asp:RequiredFieldValidator ID="rfvtxtLocalBikeRemarks" runat="server"
                                                    ControlToValidate="txtLocalBikeRemarks"
                                                    ErrorMessage="Remarks is required" CssClass="text-danger" />
                                            </div>


                                            <!-- New SMO No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalBikeSMONo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtLocalBikeSMONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalBikeSMONo" runat="server"
                                                    ControlToValidate="txtLocalBikeSMONo"
                                                    ErrorMessage="SMO No is required" CssClass="text-danger" />
                                            </div>


                                            <div class="col-12">
                                                <label for="txtLocalBikeSONo" class="form-label">SO/SAP No</label>
                                                <asp:TextBox ID="txtLocalBikeSONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                    ControlToValidate="txtLocalBikeSONo"
                                                    ErrorMessage="SMO No is required" CssClass="text-danger" />
                                            </div>

                                            <!-- New Ref No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalBikeRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtLocalBikeRefNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalBikeRefNo" runat="server"
                                                    ControlToValidate="txtLocalBikeRefNo"
                                                    ErrorMessage="Ref No is required" CssClass="text-danger" />
                                            </div>
                                        </asp:Panel>

                                        <!-- Cab Panel -->
                                        <asp:Panel ID="pnlCabFields" runat="server" Visible="false"
                                            CssClass="panel-fields">
                                            <asp:Image ID="Image1" runat="server"
                                                ImageUrl='<%# "ImageHandler.ashx?id=" + Eval("Id") %>' />

                                            <div class="col-12">
                                                <label for="txtLocalCabDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtLocalCabDate" runat="server" CssClass="form-control"
                                                    TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvLocalCabDate" runat="server"
                                                    ControlToValidate="txtLocalCabDate" ErrorMessage="Date is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalCabAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtLocalCabAmount" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalCabAmount" runat="server"
                                                    ControlToValidate="txtLocalCabAmount"
                                                    ErrorMessage="Amount is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="fileUploadLocalCab" class="form-label">Upload Bill</label>

                                                <div class="custom-file-upload position-relative">
                                                    <!-- Hidden ASP.NET FileUpload -->
                                                    <asp:FileUpload ID="fileUploadLocalCab" runat="server"
                                                        style="display:none;"
                                                        onchange="handleFileSelect(this, 'txtCabFileName', 'btnRemoveCabFile')"
                                                        accept="image/*,application/pdf" />

                                                    <!-- Fake textbox (click ? browse or open) -->
                                                    <input type="text" id="txtCabFileName" class="form-control" readonly
                                                        placeholder="Choose file..."
                                                        onclick="triggerOrOpen('<%= fileUploadLocalCab.ClientID %>', 'txtCabFileName')" />

                                                    <!-- Remove button -->
                                                    <button type="button" id="btnRemoveCabFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                        onclick="removeFile('<%= fileUploadLocalCab.ClientID %>', 'txtCabFileName', 'btnRemoveCabFile')">
                                                    </button>
                                                </div>

                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalCabFromTime" class="form-label"> From Time</label>
                                                <asp:TextBox ID="txtLocalCabFromTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server"
                                                    ControlToValidate="txtLocalCabFromTime"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalCabToTime" class="form-label"> To Time</label>
                                                <asp:TextBox ID="txtLocalCabToTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server"
                                                    ControlToValidate="txtLocalCabToTime"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalCabParticular" class="form-label">
                                                    Particular</label>
                                                <asp:TextBox ID="txtLocalCabParticular" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalCabParticular" runat="server"
                                                    ControlToValidate="txtLocalCabParticular"
                                                    ErrorMessage="Particular is required" CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalCabRemarks" class="form-label"> Remarks</label>
                                                <asp:TextBox ID="txtLocalCabRemarks" runat="server"
                                                    CssClass="form-control" TextMode="MultiLine" />
                                                <asp:RequiredFieldValidator ID="rfvLocalCabRemarks" runat="server"
                                                    ControlToValidate="txtLocalCabRemarks"
                                                    ErrorMessage="Remarks are required" CssClass="text-danger" />
                                            </div>
                                            <!-- New SMO No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalCabSMONo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtLocalCabSMONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalCabSMONo" runat="server"
                                                    ControlToValidate="txtLocalCabSMONo"
                                                    ErrorMessage="SMO No is required" CssClass="text-danger" />
                                            </div>

                                            <!-- New SMO No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalCabSONo" class="form-label">SO/SAP No</label>
                                                <asp:TextBox ID="txtLocalCabSONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                                                    ControlToValidate="txtLocalCabSONo"
                                                    ErrorMessage="SMO No is required" CssClass="text-danger" />
                                            </div>

                                            <!-- New Ref No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalCabRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtLocalCabRefNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalCabRefNo" runat="server"
                                                    ControlToValidate="txtLocalCabRefNo"
                                                    ErrorMessage="Ref No is required" CssClass="text-danger" />
                                            </div>

                                        </asp:Panel>


                                        <!-- Auto Panel -->
                                        <asp:Panel ID="pnlAutoFields" runat="server" Visible="false"
                                            CssClass="panel-fields">
                                            <asp:Image ID="txtLocalAutoImage" runat="server"
                                                ImageUrl='<%# "ImageHandler.ashx?id=" + Eval("Id") %>' />

                                            <div class="col-12">
                                                <label for="txtLocalAutoDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtLocalAutoDate" runat="server"
                                                    CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvLocalAutoDate" runat="server"
                                                    ControlToValidate="txtLocalAutoDate" ErrorMessage="Date is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalAutoDistance" class="form-label">Distance (in
                                                    km)</label>
                                                <asp:TextBox ID="txtLocalAutoDistance" runat="server"
                                                    CssClass="form-control" AutoPostBack="true"
                                                    OnTextChanged="txtLocalDistance_TextChanged" />
                                                <asp:RequiredFieldValidator ID="rfvLocalAutoDistance" runat="server"
                                                    ControlToValidate="txtLocalAutoDistance"
                                                    ErrorMessage="Distance is required" CssClass="text-danger" />
                                            </div>


                                            <div class="col-12">
                                                <label for="txtLocalAutoAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtLocalAutoAmount" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalAutoAmount" runat="server"
                                                    ControlToValidate="txtLocalAutoAmount"
                                                    ErrorMessage="Amount is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtfileUploadLocalAuto" class="form-label">Upload
                                                    Bill</label>

                                                <div class="custom-file-upload position-relative">
                                                    <!-- Hidden ASP.NET FileUpload -->
                                                    <asp:FileUpload ID="txtfileUploadLocalAuto" runat="server"
                                                        style="display:none;"
                                                        onchange="handleFileSelect(this, 'txtAutoFileName', 'btnRemoveAutoFile')"
                                                        accept="image/*,application/pdf" />

                                                    <!-- Fake textbox -->
                                                    <input type="text" id="txtAutoFileName" class="form-control"
                                                        readonly placeholder="Choose file..."
                                                        onclick="triggerOrOpen('<%= txtfileUploadLocalAuto.ClientID %>', 'txtAutoFileName')" />

                                                    <!-- Remove button -->
                                                    <button type="button" id="btnRemoveAutoFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                        onclick="removeFile('<%= txtfileUploadLocalAuto.ClientID %>', 'txtAutoFileName', 'btnRemoveAutoFile')">
                                                    </button>
                                                </div>


                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalAutoFromTime" class="form-label"> From Time</label>
                                                <asp:TextBox ID="txtLocalAutoFromTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvLocalAutoFromTime" runat="server"
                                                    ControlToValidate="txtLocalAutoFromTime"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalAutoToTime" class="form-label"> To Time</label>
                                                <asp:TextBox ID="txtLocalAutoToTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvLocalAutoToTime" runat="server"
                                                    ControlToValidate="txtLocalAutoToTime"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtLocalAutoParticular" class="form-label">
                                                    Particular</label>
                                                <asp:TextBox ID="txtLocalAutoParticular" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalAutoParticulars" runat="server"
                                                    ControlToValidate="txtLocalAutoParticular"
                                                    ErrorMessage="Particular is required" CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtLocalAutoRemarks" class="form-label"> Remarks</label>
                                                <asp:TextBox ID="txtLocalAutoRemarks" runat="server"
                                                    CssClass="form-control" TextMode="MultiLine" />
                                                <asp:RequiredFieldValidator ID="rfvLocalAutRemarks" runat="server"
                                                    ControlToValidate="txtLocalAutoRemarks"
                                                    ErrorMessage="Remarks are required" CssClass="text-danger" />
                                            </div>
                                            <!-- New SMO No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalAutoSMONo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtLocalAutoSMONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalAutoSMONo" runat="server"
                                                    ControlToValidate="txtLocalAutoSMONo"
                                                    ErrorMessage="SMO No is required" CssClass="text-danger" />
                                            </div>


                                            <!-- New SMO No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalAutoSONo" class="form-label">SO/SAP No</label>
                                                <asp:TextBox ID="txtLocalAutoSONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                                                    ControlToValidate="txtLocalAutoSONo"
                                                    ErrorMessage="SMO No is required" CssClass="text-danger" />
                                            </div>
                                            <!-- New Ref No Field -->
                                            <div class="col-12">
                                                <label for="txtLocalAutoRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtLocalAutoRefNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLocalAutoRefNo" runat="server"
                                                    ControlToValidate="txtLocalAutoRefNo"
                                                    ErrorMessage="Ref No is required" CssClass="text-danger" />
                                            </div>


                                        </asp:Panel>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlAwardExpenses" runat="server" Visible="false"
                                        CssClass="panel-fields">
                                        <div class="col-12">
                                            <label for="ddlAwardExpenseType" class="form-label">Award Type</label>
                                            <asp:DropDownList ID="ddlAwardExpenseType" runat="server"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlAwardExpenseType_SelectedIndexChanged"
                                                CssClass="form-control">
                                                <asp:ListItem Text="Select Award Type" Value="" />
                                                <asp:ListItem Text="Star Award" Value="Star Award" />
                                                <asp:ListItem Text="Spot Award" Value="Spot Award" />
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-12 mt-3">
                                            <label for="txtAwardAmount" class="form-label">Amount</label>
                                            <asp:TextBox ID="txtAwardAmount" runat="server" CssClass="form-control"
                                                ReadOnly="true"></asp:TextBox>
                                        </div>


                                        <div class="col-12">
                                            <label for="txtAwardDate" class="form-label">Award Date</label>
                                            <asp:TextBox ID="txtAwardDate" runat="server" CssClass="form-control"
                                                TextMode="Date" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server"
                                                ControlToValidate="txtAwardDate" ErrorMessage="Date is required"
                                                CssClass="text-danger" />
                                        </div>
                                    </asp:Panel>
                                    <asp:GridView ID="GridViewSpotAward" runat="server" Visible="false"
                                        CssClass="grid-view"></asp:GridView>
                                    <asp:GridView ID="GridViewStarAward" runat="server" Visible="false"
                                        CssClass="grid-view"></asp:GridView>
                                    <!-- Tour Expenses Panel -->
                                    <asp:Panel ID="pnlTourExpenses" runat="server" Visible="false"
                                        CssClass="panel-fields">
                                        <div class="col-12">
                                            <label for="ddlTourExpenseType" class="form-label">Expense Details</label>
                                            <asp:DropDownList ID="ddlTourExpenseType" runat="server" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlTourExpenseType_SelectedIndexChanged"
                                                CssClass="form-control">
                                                <asp:ListItem Text="Select Tour Expense Type" Value="" />
                                                <asp:ListItem Text="Conveyance" Value="Conveyance" />
                                                <asp:ListItem Text="Food" Value="Food" />
                                                <asp:ListItem Text="Lodging" Value="Lodging" />
                                                <asp:ListItem Text="Miscellaneous" Value="Miscellaneous" />
                                            </asp:DropDownList>
                                        </div>

                                        <asp:Panel ID="pnlTourFoodFields" runat="server" Visible="false"
                                            CssClass="panel-fields">
                                            <div class="col-12">
                                                <label for="txtTourFoodDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtTourFoodDate" runat="server" CssClass="form-control"
                                                    TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                    ControlToValidate="txtTourFoodDate" ErrorMessage="Date is required"
                                                    CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="ddlTourFoodDesignation"
                                                    class="form-label">Designation</label>
                                                <asp:DropDownList ID="txtTourFoodDesignation" runat="server"
                                                    CssClass="form-control" AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlTourFoodDesignation_SelectedIndexChanged">
                                                    <asp:ListItem Value="">Select Designation</asp:ListItem>
                                                    <asp:ListItem Value="FSE">FSE</asp:ListItem>
                                                    <asp:ListItem Value="FST">FST</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <div class="col-12">
                                                <label for="txtTourFoodFromTime" class="form-label">From Time </label>
                                                <asp:TextBox ID="txtTourFoodFromTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" AutoPostBack="true"
                                                    OnTextChanged="txtTourFoodFromTime_TextChanged" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorFromTime"
                                                    runat="server" ControlToValidate="txtTourFoodFromTime"
                                                    ErrorMessage="From time is required" CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtTourFoodToTime" class="form-label">To Time </label>
                                                <asp:TextBox ID="txtTourFoodToTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" AutoPostBack="true"
                                                    OnTextChanged="txtTourFoodToTime_TextChanged" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorToTime"
                                                    runat="server" ControlToValidate="txtTourFoodToTime"
                                                    ErrorMessage="To time is required" CssClass="text-danger" />
                                            </div>



                                            <div class="col-12">
                                                <label for="txtTourFoodAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtTourFoodAmount" runat="server"
                                                    CssClass="form-control" ReadOnly="true" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                                                    ControlToValidate="txtTourFoodAmount"
                                                    ErrorMessage="Amount is required" CssClass="text-danger" />
                                            </div>



                                            <div class="col-12">
                                                <label for="txtTourFoodParticulars"
                                                    class="form-label">Particulars</label>
                                                <asp:TextBox ID="txtTourFoodParticulars" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server"
                                                    ControlToValidate="txtTourFoodParticulars"
                                                    ErrorMessage="Particulars is required" CssClass="text-danger" />

                                            </div>
                                            <div>
                                                <label for="txtTourFoodRemarks" class="form-label">Remarks</label>
                                                <asp:TextBox ID="txtTourFoodRemarks" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server"
                                                    ControlToValidate="txtTourFoodRemarks"
                                                    ErrorMessage="Remarks is required" CssClass="text-danger" />
                                            </div>

                                            <!-- New SMO No Field -->
                                            <div class="col-12">
                                                <label for="txtTourFoodSMONo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtTourFoodSMONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTourFoodSMONo" runat="server"
                                                    ControlToValidate="txtTourFoodSMONo"
                                                    ErrorMessage="SMO No is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtTourFoodSONo" class="form-label">SO/SAP No</label>
                                                <asp:TextBox ID="txtTourFoodSONo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTourFoodSONo" runat="server"
                                                    ControlToValidate="txtTourFoodSONo" ErrorMessage="SO No is required"
                                                    CssClass="text-danger" />
                                            </div>


                                            <!-- New Ref No Field -->
                                            <div class="col-12">
                                                <label for="txtTourFoodRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtTourFoodRefNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTourFoodRefNo" runat="server"
                                                    ControlToValidate="txtTourFoodRefNo"
                                                    ErrorMessage="Ref No is required" CssClass="text-danger" />
                                            </div>

                                        </asp:Panel>

                                        <!-- Add additional panels and ensure IDs match correctly and all required attributes are set. -->
                                    </asp:Panel>





                                    <asp:Panel ID="pnlTourMiscellaneousFields" runat="server" Visible="false"
                                        CssClass="panel-fields">
                                        <div class="col-12">
                                            <label for="txtTourMiscDate" class="form-label">Date</label>
                                            <asp:TextBox ID="txtTourMiscDate" runat="server" CssClass="form-control"
                                                TextMode="Date" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscDate" runat="server"
                                                ControlToValidate="txtTourMiscDate" ErrorMessage="Date is required"
                                                CssClass="text-danger" />
                                        </div>
                                        <div class="col-12">
                                            <label for="txtTourMiscItem" class="form-label">Purchased Item</label>
                                            <asp:TextBox ID="txtTourMiscItem" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscItem" runat="server"
                                                ControlToValidate="txtTourMiscItem"
                                                ErrorMessage="Purchased item is required" CssClass="text-danger" />
                                        </div>


                                        <div class="col-12">
                                            <label for="txtTourMiscAmount" class="form-label">Amount</label>
                                            <asp:TextBox ID="txtTourMiscAmount" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscAmount" runat="server"
                                                ControlToValidate="txtTourMiscAmount" ErrorMessage="Amount is required"
                                                CssClass="text-danger" />
                                        </div>

                                        <div class="col-12">
                                            <label for="fileUploadTourMiscellaneous" class="form-label">Upload
                                                Bill</label>

                                            <div class="custom-file-upload position-relative">
                                                <!-- Hidden ASP.NET FileUpload -->
                                                <asp:FileUpload ID="fileUploadTourMiscellaneous" runat="server"
                                                    style="display:none;"
                                                    onchange="handleFileSelect(this, 'txtTourMiscFileName', 'btnRemoveTourMiscFile')"
                                                    accept="image/*,application/pdf" />

                                                <!-- Fake textbox -->
                                                <input type="text" id="txtTourMiscFileName" class="form-control"
                                                    readonly placeholder="Choose file..."
                                                    onclick="triggerOrOpen('<%= fileUploadTourMiscellaneous.ClientID %>', 'txtTourMiscFileName')" />

                                                <!-- Remove button -->
                                                <button type="button" id="btnRemoveTourMiscFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                    onclick="removeFile('<%= fileUploadTourMiscellaneous.ClientID %>', 'txtTourMiscFileName', 'btnRemoveTourMiscFile')">
                                                </button>
                                            </div>


                                        </div>


                                        <div class="col-12">
                                            <label for="txtTourMiscFromTime" class="form-label">From Time</label>
                                            <asp:TextBox ID="txtTourMiscFromTime" runat="server" CssClass="form-control"
                                                TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscFromTime" runat="server"
                                                ControlToValidate="txtTourMiscFromTime"
                                                ErrorMessage="From Time is required" CssClass="text-danger" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtTourMiscToTime" class="form-label">To Time</label>
                                            <asp:TextBox ID="txtTourMiscToTime" runat="server" CssClass="form-control"
                                                TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscToTime" runat="server"
                                                ControlToValidate="txtTourMiscToTime" ErrorMessage="To Time is required"
                                                CssClass="text-danger" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtTourMiscParticulars" class="form-label">Particulars</label>
                                            <asp:TextBox ID="txtTourMiscParticulars" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscParticulars" runat="server"
                                                ControlToValidate="txtTourMiscParticulars"
                                                ErrorMessage="Particulars are required" CssClass="text-danger" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtTourMiscRemarks" class="form-label">Remarks</label>
                                            <asp:TextBox ID="txtTourMiscRemarks" runat="server" CssClass="form-control"
                                                TextMode="MultiLine" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscRemarks" runat="server"
                                                ControlToValidate="txtTourMiscRemarks"
                                                ErrorMessage="Remarks are required" CssClass="text-danger" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtTourMiscSmoNo" class="form-label">SMO/WBS No</label>
                                            <asp:TextBox ID="txtTourMiscSmoNo" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscSmoNo" runat="server"
                                                ControlToValidate="txtTourMiscSmoNo" ErrorMessage="SMO No is required"
                                                CssClass="text-danger" />
                                        </div>

                                        <!-- New SO No Field -->
                                        <div class="col-12">
                                            <label for="txtTourMiscSoNo" class="form-label">SO/SAP No</label>
                                            <asp:TextBox ID="txtTourMiscSoNo" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscSoNo" runat="server"
                                                ControlToValidate="txtTourMiscSoNo" ErrorMessage="SO No is required"
                                                CssClass="text-danger" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtTourMiscRefNo" class="form-label">Ref No</label>
                                            <asp:TextBox ID="txtTourMiscRefNo" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourMiscRefNo" runat="server"
                                                ControlToValidate="txtTourMiscRefNo" ErrorMessage="Ref No is required"
                                                CssClass="text-danger" />
                                        </div>


                                    </asp:Panel>

                                    <asp:Panel ID="pnlTourOthersFields" runat="server" Visible="false"
                                        CssClass="panel-fields">
                                        <div class="col-12">
                                            <label for="txtTourOthersDate" class="form-label">Date</label>
                                            <asp:TextBox ID="txtTourOthersDate" runat="server" CssClass="form-control"
                                                TextMode="Date" />
                                            <asp:RequiredFieldValidator ID="rfvTourOthersDate" runat="server"
                                                ControlToValidate="txtTourOthersDate" ErrorMessage="Date is required"
                                                CssClass="text-danger" />
                                        </div>
                                        <div class="col-12">
                                            <label for="txtTourOthersAmount" class="form-label">Amount</label>
                                            <asp:TextBox ID="txtTourOthersAmount" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourOthersAmount" runat="server"
                                                ControlToValidate="txtTourOthersAmount"
                                                ErrorMessage="Amount is required" CssClass="text-danger" />
                                        </div>
                                        <div class="col-12">
                                            <label for="fileUploadTourOthers" class="form-label">Upload Bill</label>

                                            <div class="custom-file-upload position-relative">
                                                <!-- Hidden ASP.NET FileUpload -->
                                                <asp:FileUpload ID="fileUploadTourOthers" runat="server"
                                                    style="display:none;"
                                                    onchange="handleFileSelect(this, 'txtTourOthersFileName', 'btnRemoveTourOthersFile')"
                                                    accept="image/*,application/pdf" />

                                                <!-- Fake textbox -->
                                                <input type="text" id="txtTourOthersFileName" class="form-control"
                                                    readonly placeholder="Choose file..."
                                                    onclick="triggerOrOpen('<%= fileUploadTourOthers.ClientID %>', 'txtTourOthersFileName')" />

                                                <!-- Remove button -->
                                                <button type="button" id="btnRemoveTourOthersFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                    onclick="removeFile('<%= fileUploadTourOthers.ClientID %>', 'txtTourOthersFileName', 'btnRemoveTourOthersFile')">
                                                </button>
                                            </div>

                                        </div>


                                        <div class="col-12">
                                            <label for="txtFromTimeTourOthers" class="form-label">From Time</label>
                                            <asp:TextBox ID="txtFromTimeTourOthers" runat="server"
                                                CssClass="form-control" TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="rfvFromTimeTourOthers" runat="server"
                                                ControlToValidate="txtFromTimeTourOthers"
                                                ErrorMessage="From Time is required" CssClass="text-danger" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtToTimeTourOthers" class="form-label">To Time</label>
                                            <asp:TextBox ID="txtToTimeTourOthers" runat="server" CssClass="form-control"
                                                TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="rfvToTimeTourOthers" runat="server"
                                                ControlToValidate="txtToTimeTourOthers"
                                                ErrorMessage="To Time is required" CssClass="text-danger" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtParticularsTourOthers" class="form-label">Particulars</label>
                                            <asp:TextBox ID="txtParticularsTourOthers" runat="server"
                                                CssClass="form-control" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtRemarksTourOthers" class="form-label">Remarks</label>
                                            <asp:TextBox ID="txtRemarksTourOthers" runat="server"
                                                CssClass="form-control" TextMode="MultiLine" />
                                        </div>
                                        <div class="col-12">
                                            <label for="fileUploadServiceReport" class="form-label">Service
                                                Report</label>

                                            <div class="custom-file-upload position-relative">
                                                <!-- Hidden ASP.NET FileUpload -->
                                                <asp:FileUpload ID="fileUploadServiceReport" runat="server"
                                                    style="display:none;"
                                                    onchange="handleFileSelect(this, 'txtServiceReportFileName', 'btnRemoveServiceReportFile')"
                                                    accept="image/*,application/pdf" />

                                                <!-- Fake textbox -->
                                                <input type="text" id="txtServiceReportFileName" class="form-control"
                                                    readonly placeholder="Choose file..."
                                                    onclick="triggerOrOpen('<%= fileUploadServiceReport.ClientID %>', 'txtServiceReportFileName')" />

                                                <!-- Remove button -->
                                                <button type="button" id="btnRemoveServiceReportFile" style="position:absolute; right:6px; top:4px; display:none;
   
    border:none;background:none;
       font-size:30px; font-weight:bold; line-height:24px;
       text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                    onclick="removeFile('<%= fileUploadServiceReport.ClientID %>', 'txtServiceReportFileName', 'btnRemoveServiceReportFile')">
                                                </button>
                                            </div>
                                        </div>



                                        <div class="col-12">
                                            <label for="txtTourOthersSmoNo" class="form-label">SMO/WBS No</label>
                                            <asp:TextBox ID="txtTourOthersSmoNo" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourOthersSmoNo" runat="server"
                                                ControlToValidate="txtTourOthersSmoNo" ErrorMessage="SMO No is required"
                                                CssClass="text-danger" />
                                        </div>

                                        <!-- New SO No Field -->
                                        <div class="col-12">
                                            <label for="txtTourOthersSoNo" class="form-label">SO/SAP No</label>
                                            <asp:TextBox ID="txtTourOthersSoNo" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourOthersSoNo" runat="server"
                                                ControlToValidate="txtTourOthersSoNo" ErrorMessage="SO No is required"
                                                CssClass="text-danger" />
                                        </div>

                                        <div class="col-12">
                                            <label for="txtTourOthersRefNo" class="form-label">Ref No</label>
                                            <asp:TextBox ID="txtTourOthersRefNo" runat="server"
                                                CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTourOthersRefNo" runat="server"
                                                ControlToValidate="txtTourOthersRefNo" ErrorMessage="Ref No is required"
                                                CssClass="text-danger" />
                                        </div>
                                        <div class="col-12">
                                            <label for="fileUploadTourApproval" class="form-label">Approval Mail</label>

                                            <div class="custom-file-upload position-relative">
                                                <!-- Hidden ASP.NET FileUpload -->
                                                <asp:FileUpload ID="fileUploadTourApproval" runat="server"
                                                    style="display:none;"
                                                    onchange="handleFileSelect(this, 'txtTourApprovalFileName', 'btnRemoveTourApprovalFile')"
                                                    accept="image/*,application/pdf" />

                                                <!-- Fake textbox -->
                                                <input type="text" id="txtTourApprovalFileName" class="form-control"
                                                    readonly placeholder="Choose file..."
                                                    onclick="triggerOrOpen('<%= fileUploadTourApproval.ClientID %>', 'txtTourApprovalFileName')" />

                                                <!-- Remove button -->
                                                <button type="button" id="btnRemoveTourApprovalFile" style="position:absolute; right:6px; top:4px; display:none;
   
    border:none;background:none;
       font-size:30px; font-weight:bold; line-height:24px;
       text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                    onclick="removeFile('<%= fileUploadTourApproval.ClientID %>', 'txtTourApprovalFileName', 'btnRemoveTourApprovalFile')">
                                                </button>
                                            </div>


                                    </asp:Panel>


                                    <asp:Panel ID="pnlTourConvenience" runat="server" Visible="false"
                                        CssClass="panel-fields">
                                        <asp:Image ID="imgConveyance" runat="server"
                                            ImageUrl='<%# "ImageHandler.ashx?id=" + Eval("Id") %>' />

                                        <div class="col-12">
                                            <label for="ddlTourTransportMode" class="form-label">Mode of
                                                Transport</label>
                                            <asp:DropDownList ID="ddlTourTransportMode" runat="server"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlTourTransportMode_SelectedIndexChanged"
                                                CssClass="form-control">
                                                <asp:ListItem Text="Select Transport Mode" Value="" />
                                                <asp:ListItem Text="Flight" Value="Flight" />
                                                <asp:ListItem Text="Bus" Value="Bus" />
                                                <asp:ListItem Text="Train" Value="Train" />
                                                <asp:ListItem Text="Cab" Value="Cab" />
                                                <asp:ListItem Text="Auto" Value="Auto" />

                                            </asp:DropDownList>
                                        </div>

                                        <!-- Flight Panel -->
                                        <asp:Panel ID="pnlFlightFields" runat="server" Visible="false"
                                            CssClass="panel-fields">
                                            <div class="col-12">
                                                <label for="txtFlightDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtFlightDate" runat="server" CssClass="form-control"
                                                    TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvFlightDate" runat="server"
                                                    ControlToValidate="txtFlightDate" ErrorMessage="Date is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtFlightAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtFlightAmount" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvFlightAmount" runat="server"
                                                    ControlToValidate="txtFlightAmount"
                                                    ErrorMessage="Amount is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="fileUploadFlight" class="form-label">Upload Bill</label>

                                                <div class="custom-file-upload position-relative">
                                                    <!-- Hidden ASP.NET FileUpload -->
                                                    <asp:FileUpload ID="fileUploadFlight" runat="server"
                                                        style="display:none;"
                                                        onchange="handleFileSelect(this, 'txtFlightFileName', 'btnRemoveFlightFile')"
                                                        accept="image/*,application/pdf" />

                                                    <!-- Fake textbox -->
                                                    <input type="text" id="txtFlightFileName" class="form-control"
                                                        readonly placeholder="Choose file..."
                                                        onclick="triggerOrOpen('<%= fileUploadFlight.ClientID %>', 'txtFlightFileName')" />

                                                    <!-- Remove button -->
                                                    <button type="button" id="btnRemoveFlightFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                        onclick="removeFile('<%= fileUploadFlight.ClientID %>', 'txtFlightFileName', 'btnRemoveFlightFile')">
                                                    </button>
                                                </div>

                                            </div>


                                            <!-- New fields for From Time, To Time, Particulars, and Remarks -->
                                            <div class="col-12">
                                                <label for="txtFlightFromTime" class="form-label">From Time</label>
                                                <asp:TextBox ID="txtFlightFromTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvFromTime" runat="server"
                                                    ControlToValidate="txtFlightFromTime"
                                                    ErrorMessage="From Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtFlightToTime" class="form-label">To Time</label>
                                                <asp:TextBox ID="txtFlightToTime" runat="server" CssClass="form-control"
                                                    TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvToTime" runat="server"
                                                    ControlToValidate="txtFlightToTime"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtFlightParticulars" class="form-label">Particulars</label>
                                                <asp:TextBox ID="txtFlightParticulars" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvParticulars" runat="server"
                                                    ControlToValidate="txtFlightParticulars"
                                                    ErrorMessage="Particulars are required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtFlightRemarks" class="form-label">Remarks</label>
                                                <asp:TextBox ID="txtFlightRemarks" runat="server"
                                                    CssClass="form-control" TextMode="MultiLine" />
                                                <asp:RequiredFieldValidator ID="rfvRemarks" runat="server"
                                                    ControlToValidate="txtFlightRemarks"
                                                    ErrorMessage="Remarks are required" CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtFlightSmoNo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtFlightSmoNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvFlightSmoNo" runat="server"
                                                    ControlToValidate="txtFlightSmoNo" ErrorMessage="SMO No is required"
                                                    CssClass="text-danger" />
                                            </div>


                                            <div class="col-12">
                                                <label for="txtFlightSoNo" class="form-label">SO/SAP No</label>
                                                <asp:TextBox ID="txtFlightSoNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                                    ControlToValidate="txtFlightSoNo" ErrorMessage="SMO No is required"
                                                    CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtFlightRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtFlightRefNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvFlightRefNo" runat="server"
                                                    ControlToValidate="txtFlightRefNo" ErrorMessage="Ref No is required"
                                                    CssClass="text-danger" />
                                            </div>
                                        </asp:Panel>


                                        <!-- Bus Panel -->
                                        <asp:Panel ID="pnlBusFields" runat="server" Visible="false"
                                            CssClass="panel-fields">
                                            <div class="col-12">
                                                <label for="txtBusDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtBusDate" runat="server" CssClass="form-control"
                                                    TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvBusDate" runat="server"
                                                    ControlToValidate="txtBusDate" ErrorMessage="Date is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtBusAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtBusAmount" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBusAmount" runat="server"
                                                    ControlToValidate="txtBusAmount" ErrorMessage="Amount is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="fileUploadBus" class="form-label">Upload Bill</label>

                                                <div class="custom-file-upload position-relative">
                                                    <!-- Hidden ASP.NET FileUpload -->
                                                    <asp:FileUpload ID="fileUploadBus" runat="server"
                                                        style="display:none;"
                                                        onchange="handleFileSelect(this, 'txtBusFileName', 'btnRemoveBusFile')"
                                                        accept="image/*,application/pdf" />

                                                    <!-- Fake textbox -->
                                                    <input type="text" id="txtBusFileName" class="form-control" readonly
                                                        placeholder="Choose file..."
                                                        onclick="triggerOrOpen('<%= fileUploadBus.ClientID %>', 'txtBusFileName')" />

                                                    <!-- Remove button -->
                                                    <button type="button" id="btnRemoveBusFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                        onclick="removeFile('<%= fileUploadBus.ClientID %>', 'txtBusFileName', 'btnRemoveBusFile')">
                                                    </button>
                                                </div>

                                            </div>

                                            <!-- New fields for From Time, To Time, Particulars, and Remarks -->
                                            <div class="col-12">
                                                <label for="txtFromTimeBus" class="form-label">From Time</label>
                                                <asp:TextBox ID="txtFromTimeBus" runat="server" CssClass="form-control"
                                                    TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvFromTimeBus" runat="server"
                                                    ControlToValidate="txtFromTimeBus"
                                                    ErrorMessage="From Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtToTimeBus" class="form-label">To Time</label>
                                                <asp:TextBox ID="txtToTimeBus" runat="server" CssClass="form-control"
                                                    TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvToTimeBus" runat="server"
                                                    ControlToValidate="txtToTimeBus" ErrorMessage="To Time is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtParticularsBus" class="form-label">Particulars</label>
                                                <asp:TextBox ID="txtParticularsBus" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvParticularsBus" runat="server"
                                                    ControlToValidate="txtParticularsBus"
                                                    ErrorMessage="Particulars are required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtRemarksBus" class="form-label">Remarks</label>
                                                <asp:TextBox ID="txtRemarksBus" runat="server" CssClass="form-control"
                                                    TextMode="MultiLine" />
                                                <asp:RequiredFieldValidator ID="rfvRemarksBus" runat="server"
                                                    ControlToValidate="txtRemarksBus"
                                                    ErrorMessage="Remarks are required" CssClass="text-danger" />
                                            </div>

                                            <!-- New fields for SMO No and Ref No -->
                                            <div class="col-12">
                                                <label for="txtBusSmoNo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtBusSmoNo" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBusSmoNo" runat="server"
                                                    ControlToValidate="txtBusSmoNo" ErrorMessage="SMO No is required"
                                                    CssClass="text-danger" />
                                            </div>


                                            <div class="col-12">
                                                <label for="txtBusSoNo" class="form-label">SO/SAP No</label>
                                                <asp:TextBox ID="txtBusSoNo" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server"
                                                    ControlToValidate="txtBusSoNo" ErrorMessage="SMO No is required"
                                                    CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txtBusRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtBusRefNo" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBusRefNo" runat="server"
                                                    ControlToValidate="txtBusRefNo" ErrorMessage="Ref No is required"
                                                    CssClass="text-danger" />
                                            </div>
                                        </asp:Panel>


                                        <!-- Train Panel -->
                                        <asp:Panel ID="pnlTrainFields" runat="server" Visible="false"
                                            CssClass="panel-fields">
                                            <div class="col-12">
                                                <label for="txtTrainDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtTrainDate" runat="server" CssClass="form-control"
                                                    TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvTrainDate" runat="server"
                                                    ControlToValidate="txtTrainDate" ErrorMessage="Date is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtTrainAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtTrainAmount" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTrainAmount" runat="server"
                                                    ControlToValidate="txtTrainAmount" ErrorMessage="Amount is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="fileUploadTrain" class="form-label">Upload Bill</label>

                                                <div class="custom-file-upload position-relative">
                                                    <!-- Hidden ASP.NET FileUpload -->
                                                    <asp:FileUpload ID="fileUploadTrain" runat="server"
                                                        style="display:none;"
                                                        onchange="handleFileSelect(this, 'txtTrainFileName', 'btnRemoveTrainFile')"
                                                        accept="image/*,application/pdf" />

                                                    <!-- Fake textbox -->
                                                    <input type="text" id="txtTrainFileName" class="form-control"
                                                        readonly placeholder="Choose file..."
                                                        onclick="triggerOrOpen('<%= fileUploadTrain.ClientID %>', 'txtTrainFileName')" />

                                                    <!-- Remove button -->
                                                    <button type="button" id="btnRemoveTrainFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                        onclick="removeFile('<%= fileUploadTrain.ClientID %>', 'txtTrainFileName', 'btnRemoveTrainFile')">
                                                    </button>
                                                </div>

                                            </div>

                                            <!-- New fields for From Time, To Time, Particulars, and Remarks -->
                                            <div class="col-12">
                                                <label for="txtFromTimeTrain" class="form-label">From Time</label>
                                                <asp:TextBox ID="txtFromTimeTrain" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvFromTimeTrain" runat="server"
                                                    ControlToValidate="txtFromTimeTrain"
                                                    ErrorMessage="From Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtToTimeTrain" class="form-label">To Time</label>
                                                <asp:TextBox ID="txtToTimeTrain" runat="server" CssClass="form-control"
                                                    TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvToTimeTrain" runat="server"
                                                    ControlToValidate="txtToTimeTrain"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtParticularsTrain" class="form-label">Particulars</label>
                                                <asp:TextBox ID="txtParticularsTrain" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvParticularsTrain" runat="server"
                                                    ControlToValidate="txtParticularsTrain"
                                                    ErrorMessage="Particulars are required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtRemarksTrain" class="form-label">Remarks</label>
                                                <asp:TextBox ID="txtRemarksTrain" runat="server" CssClass="form-control"
                                                    TextMode="MultiLine" />
                                                <asp:RequiredFieldValidator ID="rfvRemarksTrain" runat="server"
                                                    ControlToValidate="txtRemarksTrain"
                                                    ErrorMessage="Remarks are required" CssClass="text-danger" />
                                            </div>

                                            <!-- New fields for SMO No and Ref No -->
                                            <div class="col-12">
                                                <label for="txtTrainSmoNo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtTrainSmoNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTrainSmoNo" runat="server"
                                                    ControlToValidate="txtTrainSmoNo" ErrorMessage="SMO No is required"
                                                    CssClass="text-danger" />
                                            </div>


                                            <div class="col-12">
                                                <label for="txtTrainSoNo" class="form-label">SO/SAP No</label>
                                                <asp:TextBox ID="txtTrainSoNo" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server"
                                                    ControlToValidate="txtTrainSoNo" ErrorMessage="SMO No is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtTrainRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtTrainRefNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTrainRefNo" runat="server"
                                                    ControlToValidate="txtTrainRefNo" ErrorMessage="Ref No is required"
                                                    CssClass="text-danger" />
                                            </div>
                                        </asp:Panel>
                                        <!-- Cab Panel -->
                                        <asp:Panel ID="pnlcabTourFields" runat="server" Visible="false"
                                            CssClass="panel-fields">
                                            <div class="col-12">
                                                <label for="txtCabDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtCabDate" runat="server" CssClass="form-control"
                                                    TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvCabDate" runat="server"
                                                    ControlToValidate="txtCabDate" ErrorMessage="Date is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtCabAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtCabAmount" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCabAmount" runat="server"
                                                    ControlToValidate="txtCabAmount" ErrorMessage="Amount is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="fileUploadCab" class="form-label">Upload Bill</label>

                                                <div class="custom-file-upload position-relative">
                                                    <!-- Hidden ASP.NET FileUpload -->
                                                    <asp:FileUpload ID="fileUploadCab" runat="server"
                                                        style="display:none;"
                                                        onchange="handleFileSelect(this, 'txtCabFileName', 'btnRemoveCabFile')"
                                                        accept="image/*,application/pdf" />

                                                    <!-- Fake textbox -->
                                                    <input type="text" id="txtCabFileName" class="form-control" readonly
                                                        placeholder="Choose file..."
                                                        onclick="triggerOrOpen('<%= fileUploadCab.ClientID %>', 'txtCabFileName')" />

                                                    <!-- Remove button -->
                                                    <button type="button" id="btnRemoveCabFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                        onclick="removeFile('<%= fileUploadCab.ClientID %>', 'txtCabFileName', 'btnRemoveCabFile')">
                                                    </button>
                                                </div>
                                            </div>

                                            <!-- New fields for From Time, To Time, Particulars, and Remarks -->
                                            <div class="col-12">
                                                <label for="txtFromTimeCab" class="form-label">From Time</label>
                                                <asp:TextBox ID="txtFromTimeCab" runat="server" CssClass="form-control"
                                                    TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvFromTimeCab" runat="server"
                                                    ControlToValidate="txtFromTimeCab"
                                                    ErrorMessage="From Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtToTimeCab" class="form-label">To Time</label>
                                                <asp:TextBox ID="txtToTimeCab" runat="server" CssClass="form-control"
                                                    TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvToTimeCab" runat="server"
                                                    ControlToValidate="txtToTimeCab" ErrorMessage="To Time is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtParticularsCab" class="form-label">Particulars</label>
                                                <asp:TextBox ID="txtParticularsCab" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvParticularsCab" runat="server"
                                                    ControlToValidate="txtParticularsCab"
                                                    ErrorMessage="Particulars are required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtRemarksCab" class="form-label">Remarks</label>
                                                <asp:TextBox ID="txtRemarksCab" runat="server" CssClass="form-control"
                                                    TextMode="MultiLine" />
                                                <asp:RequiredFieldValidator ID="rfvRemarksCab" runat="server"
                                                    ControlToValidate="txtRemarksCab"
                                                    ErrorMessage="Remarks are required" CssClass="text-danger" />
                                            </div>

                                            <!-- New fields for SMO No and Ref No -->
                                            <div class="col-12">
                                                <label for="txtCabSmoNo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtCabSmoNo" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCabSmoNo" runat="server"
                                                    ControlToValidate="txtCabSmoNo" ErrorMessage="SMO No is required"
                                                    CssClass="text-danger" />
                                            </div>


                                            <div class="col-12">
                                                <label for="txtCabSoNo" class="form-label">SO/SAP No</label>
                                                <asp:TextBox ID="txtCabSoNo" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server"
                                                    ControlToValidate="txtCabSoNo" ErrorMessage="SMO No is required"
                                                    CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtCabRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtCabRefNo" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCabRefNo" runat="server"
                                                    ControlToValidate="txtCabRefNo" ErrorMessage="Ref No is required"
                                                    CssClass="text-danger" />
                                            </div>
                                        </asp:Panel>


                                        <!-- Auto Panel -->
                                        <asp:Panel ID="pnlAutoTourFields" runat="server" Visible="false"
                                            CssClass="panel-fields">
                                            <asp:Image ID="fileUploadAuto" runat="server"
                                                ImageUrl='<%# "ImageHandler.ashx?id=" + Eval("Id") %>' />

                                            <div class="col-12">
                                                <label for="txtTourAutoDate" class="form-label">Date</label>
                                                <asp:TextBox ID="txtTourAutoDate" runat="server" CssClass="form-control"
                                                    TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvTourAutoDate" runat="server"
                                                    ControlToValidate="txtTourAutoDate" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtTourAutoDistance" class="form-label">Distance</label>
                                                <asp:TextBox ID="txtTourAutoDistance" runat="server"
                                                    CssClass="form-control" AutoPostBack="true"
                                                    OnTextChanged="txtAutoTourDistance_TextChanged" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtTourAutoAmount" class="form-label">Amount</label>
                                                <asp:TextBox ID="txtTourAutoAmount" runat="server"
                                                    CssClass="form-control" ReadOnly="true" />
                                            </div>
                                            <div class="col-12">
                                                <label for="fileUploadTourAuto" class="form-label">Upload Bill</label>

                                                <div class="custom-file-upload position-relative">
                                                    <!-- Hidden ASP.NET FileUpload -->
                                                    <asp:FileUpload ID="fileUploadTourAuto" runat="server"
                                                        style="display:none;"
                                                        onchange="handleFileSelect(this, 'txtTourAutoFileName', 'btnRemoveTourAutoFile')"
                                                        accept="image/*,application/pdf" />

                                                    <!-- Fake textbox -->
                                                    <input type="text" id="txtTourAutoFileName" class="form-control"
                                                        readonly placeholder="Choose file..."
                                                        onclick="triggerOrOpen('<%= fileUploadTourAuto.ClientID %>', 'txtTourAutoFileName')" />

                                                    <!-- Remove button -->
                                                    <button type="button" id="btnRemoveTourAutoFile" style="position:absolute; right:6px; top:4px; display:none;
   
border:none;background:none;
   font-size:30px; font-weight:bold; line-height:24px;
   text-align:center; cursor:pointer; color:#d00;" title="Remove File"
                                                        onclick="removeFile('<%= fileUploadTourAuto.ClientID %>', 'txtTourAutoFileName', 'btnRemoveTourAutoFile')">
                                                    </button>
                                                </div>

                                            </div>

                                            <div class="col-12">
                                                <label for="txtTourAutoFromTime" class="form-label"> From Time</label>
                                                <asp:TextBox ID="txtTourAutoFromTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvTourAutoFromTime" runat="server"
                                                    ControlToValidate="txtTourAutoFromTime"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtTourAutoToTime" class="form-label"> To Time</label>
                                                <asp:TextBox ID="txtTourAutoToTime" runat="server"
                                                    CssClass="form-control" TextMode="Time" />
                                                <asp:RequiredFieldValidator ID="rfvTourAutoToTime" runat="server"
                                                    ControlToValidate="txtTourAutoToTime"
                                                    ErrorMessage="To Time is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtTourAutoParticular" class="form-label">
                                                    Particular</label>
                                                <asp:TextBox ID="txtTourAutoParticular" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTourAutoParticulars" runat="server"
                                                    ControlToValidate="txtTourAutoParticular"
                                                    ErrorMessage="Particular is required" CssClass="text-danger" />
                                            </div>

                                            <div class="col-12">
                                                <label for="txTourAutoRemarks" class="form-label"> Remarks</label>
                                                <asp:TextBox ID="txTourAutoRemarks" runat="server"
                                                    CssClass="form-control" TextMode="MultiLine" />
                                                <asp:RequiredFieldValidator ID="rfvTourAutoRemarks" runat="server"
                                                    ControlToValidate="txTourAutoRemarks"
                                                    ErrorMessage="Remarks are required" CssClass="text-danger" />
                                            </div>

                                            <!-- New fields for SMO No and Ref No -->
                                            <div class="col-12">
                                                <label for="txtTourAutoSmoNo" class="form-label">SMO/WBS No</label>
                                                <asp:TextBox ID="txtTourAutoSmoNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTourAutoSmoNo" runat="server"
                                                    ControlToValidate="txtTourAutoSmoNo"
                                                    ErrorMessage="SMO No is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtTourAutoSoNo" class="form-label">SO/SAP No</label>
                                                <asp:TextBox ID="txtTourAutoSoNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server"
                                                    ControlToValidate="txtTourAutoSoNo"
                                                    ErrorMessage="SMO No is required" CssClass="text-danger" />
                                            </div>
                                            <div class="col-12">
                                                <label for="txtTourAutoRefNo" class="form-label">Ref No</label>
                                                <asp:TextBox ID="txtTourAutoRefNo" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTourAutoRefNo" runat="server"
                                                    ControlToValidate="txtTourAutoRefNo"
                                                    ErrorMessage="Ref No is required" CssClass="text-danger" />
                                            </div>
                                        </asp:Panel>


                                    </asp:Panel>




                                    <asp:Panel ID="pnlConveyanceSection" runat="server" Visible="false">
                                        <h4 class="card-title underline">Conveyance</h4>

                                        <asp:GridView ID="GridViewConveyance" runat="server" AutoGenerateColumns="False"
                                            DataKeyNames="Id" CssClass="custom-grid">
                                            <Columns>


                                                <asp:TemplateField HeaderText="Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:dd-MMM-yyyy}") %>'></asp:Label>
                                                        <asp:TextBox ID="txtDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:yyyy-MM-dd}") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>'></asp:Label>
                                                        <asp:TextBox ID="txtAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnEditConveyance" runat="server" Text="Edit"
                                                            CssClass="btn btn-secondary" OnClick="btnEdit1_Click"
                                                            CommandArgument="Conveyance" />
                                                        <asp:Button ID="btnDelete" runat="server"
                                                            CommandName="Conveyance" Text="Delete"
                                                            CssClass="btn btn-danger" OnClick="btnDelete_Click"
                                                            CommandArgument='<%# Eval("Id") %>' />


                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>


                                        <asp:Label ID="lblTotalLocalConveyance" runat="server" ForeColor="Green" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlFoodSection" runat="server" Visible="false">
                                        <h4 class="card-title underline">Food</h4>
                                        <asp:GridView ID="GridViewFood" runat="server" AutoGenerateColumns="False"
                                            DataKeyNames="Id" CssClass="custom-grid">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFoodDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:dd-MMM-yyyy}") %>'></asp:Label>
                                                        <asp:TextBox ID="txtFoodDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:yyyy-MM-dd}") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFoodAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>'></asp:Label>
                                                        <asp:TextBox ID="txtFoodAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnEditFood" runat="server" Text="Edit"
                                                            CssClass="btn btn-secondary" OnClick="btnEdit_Click"
                                                            CommandArgument="Food" />
                                                        <asp:Button ID="btnDeleteFood" runat="server" CommandName="Food"
                                                            Text="Delete" CssClass="btn btn-danger"
                                                            OnClick="btnDelete_Click"
                                                            CommandArgument='<%# Eval("Id") %>' />

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:Label ID="lblTotalLocalFood" runat="server" ForeColor="Green" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlOthersSection" runat="server" Visible="false">
                                        <h4 class="card-title underline">Others</h4>
                                        <asp:GridView ID="GridViewOthers" runat="server" AutoGenerateColumns="False"
                                            DataKeyNames="Id" CssClass="custom-grid">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOthersDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:dd-MMM-yyyy}") %>'></asp:Label>
                                                        <asp:TextBox ID="txtOthersDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:yyyy-MM-dd}") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOthersAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>'></asp:Label>
                                                        <asp:TextBox ID="txtOthersAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnEditOthers" runat="server" Text="Edit"
                                                            CssClass="btn btn-secondary" OnClick="btnEdit1_Click"
                                                            CommandArgument="Others" />
                                                        <asp:Button ID="btnDeleteOthers" runat="server"
                                                            CommandName="Others" Text="Delete" CssClass="btn btn-danger"
                                                            OnClick="btnDelete_Click"
                                                            CommandArgument='<%# Eval("Id") %>' />

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:Label ID="lblTotalOthers" runat="server" ForeColor="Green" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlMiscellaneousSection" runat="server" Visible="false">
                                        <h4 class="card-title underline">Miscellaneous</h4>
                                        <asp:GridView ID="GridViewMiscellaneous" runat="server"
                                            AutoGenerateColumns="False" DataKeyNames="Id" CssClass="custom-grid">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMiscDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:dd-MMM-yyyy}") %>'></asp:Label>
                                                        <asp:TextBox ID="txtMiscDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:yyyy-MM-dd}") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMiscAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>'></asp:Label>
                                                        <asp:TextBox ID="txtMiscAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnEditMiscellaneous" runat="server" Text="Edit"
                                                            CssClass="btn btn-secondary" OnClick="btnEdit1_Click"
                                                            CommandArgument="Miscellaneous" />
                                                        <asp:Button ID="btnDeleteMiscellaneous" runat="server"
                                                            CommandName="Miscellaneous" Text="Delete"
                                                            CssClass="btn btn-danger" OnClick="btnDelete_Click"
                                                            CommandArgument='<%# Eval("Id") %>' />

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:Label ID="lblTotalMiscellaneous" runat="server" ForeColor="Green" />
                                    </asp:Panel>

                                    <asp:Panel ID="pnlLodgingSection" runat="server" Visible="false">
                                        <h4 class="card-title underline">Lodging</h4>
                                        <asp:GridView ID="GridViewLodging" runat="server" AutoGenerateColumns="False"
                                            DataKeyNames="Id" CssClass="custom-grid">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLodgingDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:dd-MMM-yyyy}") %>'></asp:Label>
                                                        <asp:TextBox ID="txtLodgingDate" runat="server"
                                                            Text='<%# Eval("Date", "{0:yyyy-MM-dd}") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLodgingAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>'></asp:Label>
                                                        <asp:TextBox ID="txtLodgingAmount" runat="server"
                                                            Text='<%# Eval("Amount") %>' Visible="false"
                                                            CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnEditLodging" runat="server" Text="Edit"
                                                            CssClass="btn btn-secondary" OnClick="btnEdit_Click"
                                                            CommandArgument="Lodging" />
                                                        <asp:Button ID="btnDeleteLodging" runat="server"
                                                            CommandName="Lodging" Text="Delete"
                                                            CssClass="btn btn-danger" OnClick="btnDelete_Click"
                                                            CommandArgument='<%# Eval("Id") %>' />

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:Label ID="lblTotalLodging" runat="server" ForeColor="Green" />
                                    </asp:Panel>
                                    <!-- GridView for Conveyance -->
                                    <!-- GridView for Conveyance -->
                                    <div class="col-12">
                                        <!-- Reimbursement Total -->
                                        <div class="d-flex justify-content-between align-items-center mb-3">
                                            <!-- Label for "Total Reimbursement" -->
                                            <asp:Label ID="lblTotalReimbursement" runat="server"
                                                Text="Total Reimbursement:" Font-Bold="True" ForeColor="DarkBlue"
                                                Font-Size="Large" CssClass="me-auto">
                                            </asp:Label>


                                        </div>
                                    </div>
                                    <!-- Individual Category Summary Grids -->
                                    <asp:Repeater ID="rptIndividualSummaries" runat="server">
                                        <ItemTemplate>
                                            <div class="col-12" style="margin-top: 30px; margin-bottom: 20px;">
                                                <asp:Label ID="lblCategoryHeader" runat="server" CssClass="form-label"
                                                    Text='<%# Eval("CategoryName") %>'
                                                    style="margin-bottom: 5px; display: block;"></asp:Label>
                                                <div
                                                    style="width: 100%; height: 1px; background-color: #3f418d; margin-bottom: 15px;">
                                                </div>

                                                <div
                                                    style="border: 1px solid #ddd; border-radius: 4px; overflow: hidden;">
                                                    <asp:HiddenField ID="hdnCategoryName" runat="server"
                                                        Value='<%# Eval("CategoryName") %>' />
                                                    <asp:GridView ID="gvCategoryDetail" runat="server"
                                                        AutoGenerateColumns="false" CellPadding="8" CellSpacing="0"
                                                        DataSource='<%# Eval("Details") %>' DataKeyNames="Id"
                                                        OnRowCommand="gvCategoryDetail_RowCommand"
                                                        OnRowDataBound="gvCategoryDetail_RowDataBound"
                                                        style="width: 100%; margin-bottom: 0; border-collapse: collapse; font-size: 13px;">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Date" ItemStyle-Width="25%"
                                                                ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblDate" runat="server"
                                                                        Text='<%# Eval("Date", "{0:dd-MMM-yyyy}") %>'>
                                                                    </asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Total Amount"
                                                                ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Right">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblAmount" runat="server"
                                                                        Text='<%# Eval("Amount", "{0:N2}") %>'>
                                                                    </asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Action" ItemStyle-Width="50%"
                                                                ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Button ID="btnEditSummary" runat="server"
                                                                        Text="Edit" CommandName="EditSummary"
                                                                        CommandArgument='<%# Container.DataItemIndex %>'
                                                                        CssClass="btn btn-primary btn-sm"
                                                                        style="background-color: #3f418d; color: white; border: none; padding: 4px 10px; border-radius: 3px; cursor: pointer; margin-right: 5px; font-size: 11px;" />
                                                                    <asp:Button ID="btnDeleteSummary" runat="server"
                                                                        Text="Delete" CommandName="DeleteSummary"
                                                                        CommandArgument='<%# Eval("Id") %>'
                                                                        CssClass="btn btn-danger btn-sm"
                                                                        OnClientClick="return confirm('Are you sure you want to delete this expense?');"
                                                                        style="background-color: #dc3545; color: white; border: none; padding: 4px 10px; border-radius: 3px; cursor: pointer; font-size: 11px;" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <HeaderStyle BackColor="#1a2a5e" ForeColor="white"
                                                            Font-Bold="true" Font-Size="13px" HorizontalAlign="Center"
                                                            VerticalAlign="Middle" />
                                                        <RowStyle BackColor="white" BorderColor="#1a2a5e"
                                                            BorderWidth="1px" HorizontalAlign="Center"
                                                            VerticalAlign="Middle" Height="35px" />
                                                        <AlternatingRowStyle BackColor="#f0f4f8" BorderColor="#1a2a5e"
                                                            BorderWidth="1px" HorizontalAlign="Center"
                                                            VerticalAlign="Middle" Height="35px" />
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <!-- Imported Data Grid -->
                                    <div class="col-12"
                                        style="margin-top: 40px; margin-bottom: 20px; padding: 15px; border: 1px solid #ddd; border-radius: 5px; background-color: #f9f9f9;">
                                        <h5
                                            style="color: midnightblue; margin-bottom: 15px; font-weight: bold; font-size: 1.3rem; font-family: 'Times New Roman', Times, serif;">
                                            Imported Excel Data Summary</h5>
                                        <div
                                            style="max-height: 400px; overflow-y: auto; border: 1px solid #ddd; border-radius: 4px;">
                                            <asp:GridView ID="gvImportedData" runat="server" AutoGenerateColumns="false"
                                                CellPadding="8" CellSpacing="0"
                                                style="width: 100%; margin-bottom: 0; border-collapse: collapse; font-size: 13px;">
                                                <Columns>
                                                    <asp:BoundField DataField="Date" HeaderText="Date"
                                                        ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:BoundField DataField="Category" HeaderText="Local/Tour"
                                                        ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center" />

                                                    <asp:BoundField DataField="ExpenseType" HeaderText="Expenses"
                                                        ItemStyle-HorizontalAlign="Center"
                                                        HeaderStyle-HorizontalAlign="Center" />
                                                    <asp:BoundField DataField="Total" HeaderText="Amount"
                                                        ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="45%"
                                                        ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:Button ID="btnEditRow" runat="server" Text="Edit"
                                                                style="background-color: #999; color: white; border: none; padding: 6px 12px; border-radius: 3px; cursor: pointer; margin-right: 5px; font-weight: bold; font-size: 12px;"
                                                                CommandArgument='<%# Container.DataItemIndex %>'
                                                                OnClick="btnEditRow_Click" />
                                                            <asp:Button ID="btnDeleteRow" runat="server" Text="Delete"
                                                                style="background-color: #e74c3c; color: white; border: none; padding: 6px 12px; border-radius: 3px; cursor: pointer; font-weight: bold; font-size: 12px;"
                                                                CommandArgument='<%# Container.DataItemIndex %>'
                                                                OnClick="btnDeleteRow_Click" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <HeaderStyle BackColor="#1a2a5e" ForeColor="white" Font-Bold="true"
                                                    Font-Size="13px" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                <RowStyle BackColor="white" BorderColor="#1a2a5e" BorderWidth="1px"
                                                    HorizontalAlign="Center" VerticalAlign="Middle" Height="35px" />
                                                <AlternatingRowStyle BackColor="#f0f4f8" BorderColor="#1a2a5e"
                                                    BorderWidth="1px" HorizontalAlign="Center" VerticalAlign="Middle"
                                                    Height="35px" />
                                            </asp:GridView>
                                        </div>
                                        <asp:Label ID="lblNoData" runat="server" Text="No imported data to display"
                                            style="color: #999; font-style: italic; display: block; margin-top: 10px;"
                                            Visible="false" />
                                    </div>

                                    <div class="d-flex justify-content-center gap-3 mt-3">

                                        <!-- Save Button -->
                                        <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="custom-button"
                                            OnClick="btnSubmit_Click" OnClientClick="return confirmSubmission();" />

                                        <!-- Submit Button -->
                                        <asp:Button ID="btnChangeStatus" runat="server" Text="Submit"
                                            CssClass="btn-submit " OnClick="btnChangeStatus_Click"
                                            OnClientClick="return confirmSubmit();" />

                                        <!-- Cancel Button -->
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel"
                                            CssClass="btn btn-danger btn-lg " OnClick="btnCancel_Click"
                                            CausesValidation="false" />
                                    </div>

                                    <!-- Error Messages -->
                                    <div class="text-center mt-2">
                                        <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                                        <asp:Label ID="lblStatusError" runat="server" ForeColor="Green"
                                            CssClass="error-message" Visible="false"></asp:Label>
                                    </div>




                                </section>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </main>

    </asp:Content>