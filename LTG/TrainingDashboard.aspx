<%@ Page Language="C#" MasterPageFile="/Main.Master" AutoEventWireup="true" CodeBehind="TrainingDashboard.aspx.cs" Inherits="Vivify.TrainingDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
    <main id="main" class="main">
        <style>
     .mydatagrid th, .mydatagrid td {
         border: 1.5px solid black;
         padding: 12px;
         box-shadow: #1f2b60;
         
     }
     .mydatagrid td{
         align-items:center;
     }
     .header {
         background-color: #3f418d;
         font-weight: bold;
         color: ghostwhite;
         position: sticky;
         top: 0;
         z-index: 10;
         text-align: center;
     }

     .rows {
         background-color: #ffffff;
     }

     .pager {
         text-align: right;
     }

     .scrollable-container, .scrollable-container1 {
         max-height: 390px;
         overflow: auto;
         border: 1px solid #1f2b60;
         box-shadow: 0 2px 10px darkblue;
         margin: 20px auto;
         width: 90%;
     }

     .custom-button {
         background-color: #3f418d;
         color: white;
         align-items: center;
         margin-left: 30px;
         box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
         border: none;
     }

     .custom-button:hover {
         background-color: #3f418d;
         color: white;
         box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
         cursor: pointer;
     }

     .main {
         margin: 0;
         padding: 0;
         background-color: #cadcfc;
         height: 85vh;
         display: flex;
         justify-content: center;
         align-items: center;
         overflow: auto;
     }

     .custom-grid {
         box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
     }

     .card {
         box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
         border-radius: 10px;
         background-color: #ffffff;
         margin: 20px auto;
         padding: 20px;
         width: 90%;
     }

     .card-header {
         text-align: center;
         background-color: #3f418d;
         color: ghostwhite;
         padding: 15px;
         font-size: 20px;
         border-radius: 5px 5px 0 0;
     }
 </style>
   
   
       
            <div class="card">
                <div class="card-header">
                    Training Reimbursement
                </div>
                <div class="card-body">
                   <section class="scrollable-container" style="background-color: transparent; padding: 0;">
                        <div class="custom-grid">
<asp:GridView ID="gvTrainingExpenses" runat="server" AutoGenerateColumns="False" 
    OnRowCommand="gvTrainingExpenses_RowCommand" 
    CssClass="mydatagrid" 
    PagerStyle-CssClass="pager" 
    RowStyle-CssClass="rows" 
    HeaderStyle-CssClass="header"
    style="border: 1.5px solid red; border-collapse: collapse; font-size: 14px; line-height: 20px; box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2); width: 100%;">
    <Columns>
        <asp:BoundField DataField="FirstName" HeaderText="Employee Name" />
        <asp:BoundField DataField="Date" HeaderText="From Date" DataFormatString="{0:yyyy-MM-dd}" />
        <asp:BoundField DataField="Training_Details" HeaderText="Training Details" />
        <asp:BoundField DataField="StatusId" HeaderText="Status" Visible="false" /> 
        <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <!-- Conditional logic to change button text based on StatusId -->
       <asp:Button ID="btnProceed" runat="server" Text='<%# Eval("StatusId") != DBNull.Value && Convert.ToInt32(Eval("StatusId")) == 2 ? "In Progress" : "Proceed to Reimbursement" %>' 
    CommandName="Proceed" 
    CommandArgument='<%# Eval("Training_Id") %>' 
    CssClass="btn btn-primary" 
    Enabled='<%# Eval("StatusId") != DBNull.Value && Convert.ToInt32(Eval("StatusId")) == 2 ? false : true %>' 
              Style='<%# Eval("StatusId") != DBNull.Value && Convert.ToInt32(Eval("StatusId")) == 2 ? "background: #3f418d; color: white; border: none;" : " border: 1px solid #3f418d;color: white;" %>' />


            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>


                        </div>
                    </section>
                </div>
            </div>
        </main>
   
</asp:Content>