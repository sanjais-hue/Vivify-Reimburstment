<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="StorageFee.aspx.cs" Inherits="Vivify.StorageFee" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <main id="main" class="main">

    

    <section class="section dashboard">
      <div class="row">

       

        <!-- Right side columns -->
        <div class="col">

          <!-- Recent Activity -->
          <div class="card">
           
               <h5 class="card-title" style="text-align:center;background-color: #4090ce;color:white">Storage Fee</h5>
           <section class="section error-404 d-flex flex-column align-items-center justify-content-center">
                <div class="row g-3 needs-validation">
                    <div class="col-12">
                      <label for="ddlBranch" class="form-label">Branch</label>
                        <asp:DropDownList ID ="ddlBranch" runat="server" class="form-select">
                           
                        </asp:DropDownList>
                        
                    </div>
                     <div class="col-12">
                      <label for="ddlBin" class="form-label">Default Bin</label>
                     <asp:DropDownList ID ="ddlBin" runat="server" class="form-select">
                           
                        </asp:DropDownList>
                    </div>
                    <div class="col-12">
                      <label for="txtExistingFee" class="form-label">Existing Storage Fee</label>
                        <asp:TextBox id="txtExistingFee" runat="server" ValidationGroup="TimeSlot" ReadOnly="true" BackColor="#dfe8f0" Text="80"  ClientIDMode="Static" class="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" id="reqName" controltovalidate="txtExistingFee" ForeColor="OrangeRed" />
                      
                    </div>

                    <div class="col-12">
                      <label for="txtNewFee" class="form-label">Enter Storage Fee</label>
                     <asp:TextBox id="txtNewFee" runat="server"  class="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator1" ForeColor="OrangeRed" controltovalidate="txtNewFee" errormessage="Please enter a Storage Fee!" />
                      
                    </div>
                  
                    <div class="col-12">
                        <asp:Button ID="btnCreate" OnClick="btnCreate_Click" class="btn btn-primary w-100" Text="Save" runat="server" />
                    </div>
                   
                  </div>
       
      </section>

          </div><!-- End Recent Activity -->

         
      </div>
          </div>
    </section>

  </main><!-- End #main -->

</asp:Content>
