<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Customer.aspx.cs" Inherits="Vivify.Customer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <main id="main" class="main">

    

    <section class="section dashboard">
      <div class="row">

       

        <!-- Right side columns -->
        <div class="col">

          <!-- Recent Activity -->
          <div class="card">
           

           <section class="section error-404 d-flex flex-column align-items-center justify-content-center">
                <div class="row g-3 needs-validation">
                    <div class="col-12" style="margin-top: -9px;">
                      <label for="txtCode" class="form-label">Customer Code</label>
                        <asp:TextBox id="txtCode" runat="server" ValidationGroup="TimeSlot"  ClientIDMode="Static" class="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" id="reqName" controltovalidate="txtCode" ForeColor="OrangeRed" errormessage="Please enter a customercode!" />
                      
                    </div>

                    <div class="col-12" style="margin-top: -9px;">
                      <label for="txtSurName" class="form-label">Sur Name</label>
                     <asp:TextBox id="txtSurName" runat="server"  class="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator1" ForeColor="OrangeRed" controltovalidate="txtSurName" errormessage="Please enter a customername!" />
                      
                    </div>
                  
                    <div class="col-12">
                        <asp:Button ID="btnCreate" OnClick="btnCreate_Click" class="btn btn-primary w-100" Text="Create Customer" runat="server" />
                    </div>
                   
                  </div>
       
      </section>

          </div><!-- End Recent Activity -->

         
      </div>
          </div>
    </section>

  </main><!-- End #main -->

</asp:Content>
