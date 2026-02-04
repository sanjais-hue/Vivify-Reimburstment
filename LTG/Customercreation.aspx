<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" Codebehind="Customercreation.aspx.cs" Inherits="Vivify.Customercreation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main" class="main">
        <style>
            /* General Styling */
            /*.mydatagrid th, .mydatagrid td {
                border: 1.5px solid black;
                padding: 12px;/ / Increase padding for more height */
                /*box-shadow: #1f2b60;
            }*/

            .header {
                background-color: #3f418d; /* Color for the GridView header */
                font-weight: bold;
                color: ghostwhite; /* Text color for the header */
                position: sticky; /* Make header sticky */
                top: 0; /* Stick to the top */
                z-index: 10; /* Ensure it is above other content */
                text-align: center;
            }

            .mydatagrid th {
                background-color: #3f418d;
                color: ghostwhite;
                text-align: center;
                position: sticky; /* Make header sticky */
                top: 0; /* Stick to the top */
                z-index: 10; /* Ensure header stays above other content */
            }

            /* Form Container Styling */
            .form-container {
                background-color: #f8f9fa;
                border-radius: 5px;
                margin: 0 auto;
            }

            /* Form controls (input, select, button) */
            .form-select, .form-control, .btn-primary {
                width: 100%;
                padding: 12px;
                border-color: darkblue;
            }

            .form-control {
                width: 100%;
                padding: 5px;
                border-radius: 4px;
                border: 2px solid darkblue;
            }

            .form-select {
                width: 100%;
                padding: 5px;
                margin-bottom: 10px;
                border-radius: 4px;
                border: 2px solid darkblue;
            }

            .main {
                background-color: #cadcfc;
            }

            .form-label {
                font-weight: bold;
            }

            /* Scrollable Container */
           /* .scrollable-container {
                max-height: 390px;
                overflow: auto;
                border: 1px solid black;
                box-shadow: 0 2px 10px #1f2b60;
                margin: 25px auto;
                width: 100%;
                max-width: 900px;
            }*/
            .hidden-column {
                display: none;
            }
            .table {
                width: 100%;
                table-layout: fixed;
                border-collapse: collapse;
                border-color:black;
            }

            .table th, .table td {
                padding: 10px;
            }

            /* For Sticky Header */
            .mydatagrid {
                width: 100%; /* Make the grid 100% width */
                border-collapse: collapse; /* Optional: collapse the table borders */
                
            }

            .mydatagrid td,
            .mydatagrid th {
                padding: 10px; /* Add padding for readability */
                text-align: left;
              border: 1.5px solid black; /* Set border color here */
            }

            .mydatagrid th {
                background-color: #3f418d; /* Dark background for header */
                color: white;
                text-align: center;
                position: sticky; /* Make header sticky */
                top: 0; /* Stick to the top */
                z-index: 10; /* Ensure it stays above other content */
            }
             .table-container {
     display: block;
     max-height: 400px;
     overflow-y: auto;
     box-shadow: 0 4px 10px rgba(0, 0, 0, 0.3);
     background-color: white;
     border-radius: 5px;
     margin-bottom:50px;
     
 }
            /* Mobile-specific styles */
            @media only screen and (max-width: 768px) {
                .scrollable-container {
                    max-width: 100%; /* Make container full width */
                   
                }

                .table-responsive {
                    width: 100%;
                    overflow-x: auto; /* Enable horizontal scroll */
                }

                .mydatagrid {
                    min-width: 600px; /* Ensure a minimum width so that the table can scroll horizontally */
                }

                .mydatagrid td,
                .mydatagrid th {
                    padding: 8px; /* Adjust padding for mobile screens */
                    font-size: 0.9rem; /* Slightly smaller text on mobile */
                }
            }
        </style>

        <section class="section dashboard">
            <div class="row">
                <div class="col">
                    <div class="card">
                        <h5 class="card-title" style="text-align:center;background-color: #3f418d;color:ghostwhite">Customer Creation</h5>
                        <section class="form-container section error-404 d-flex flex-column" style="box-shadow: darkblue; margin:0px;">
                            <div class="col-12">
                                <label for="ddlBranch" class="form-label">Branch</label>
                                <asp:DropDownList ID="ddlBranch" runat="server" class="form-select">
                                    <asp:ListItem Text="Select Branch" Value="0" Selected="True"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                       <div class="col-12">
<label for="txtCustomerName" class="form-label">Customer Name</label>
<asp:TextBox id="txtCustomerName" runat="server" class="form-control" autoComplete="off" 
             oninput="this.value = this.value.toUpperCase();"></asp:TextBox>

<asp:RequiredFieldValidator 
    runat="server" 
    id="RequiredFieldValidator1" 
    ForeColor="OrangeRed" 
    controltovalidate="txtCustomerName" 
    errormessage="Please enter a customer name!" />

<asp:RegularExpressionValidator 
    runat="server" 
    id="RegexValidatorCustomerName" 
    controltovalidate="txtCustomerName" 
    ForeColor="OrangeRed" 
    errormessage="Customer name must be in uppercase letters and can include spaces!" 
    ValidationExpression="^[A-Z\s]+$" />

</div>

                            <div class="col-12">
                                <label for="txtAddress" class="form-label">Address</label>
                                <asp:TextBox id="txtAddress" runat="server" TextMode="MultiLine" class="form-control" autoComplete="off"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator2" ForeColor="OrangeRed" controltovalidate="txtAddress" errormessage="Please enter an address!" />
                            </div>

                            <div class="col-12" style="color:#1f2b60">
                                <asp:Button ID="btnCustomerCreate" class="btn btn-primary w-100" OnClick="btnCustomerCreate_Click" Text="Create Customer" runat="server" style="background-color: #3f418d;" />
                            </div>
                        </section>
                    </div>
                </div>
            </div>
        </section>

       <section class="scrollable-container mt-4">
     <div class="table-responsive">
         <div class="table-container">
         
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CssClass="mydatagrid table">
                    <Columns>
                        <asp:BoundField DataField="CustomerId" HeaderText="CustomerId" ItemStyle-CssClass="hidden-column" HeaderStyle-CssClass="hidden-column" />
                        <asp:BoundField DataField="CustomerName" HeaderText="CustomerName" />
                        <asp:BoundField DataField="Address1" HeaderText="Address" />
                       
                    </Columns>
                </asp:GridView>
             </div>
         </div>
       
        </section>

    </main>
</asp:Content>