<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="OutBoundProcess.aspx.cs" Inherits="Vivify.OutBoundProcess" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <main id="main" class="main">

    
             <script>
function myFunction() {
  confirm("Are you sure complete the OutBound Scan !");
}
             </script>
    <section class="section dashboard">
      <div class="row">

       

        <!-- Right side columns -->
        <div class="col">

          <!-- Recent Activity -->
          <div class="card" id="divCustomer" runat="server">
           
               <h5 class="card-title" style="text-align:center;background-color: #4090ce;color:white">OutBound Process</h5>
           <div class="section error-404 d-flex flex-column align-items-center justify-content-center">
                <div class="row g-3 needs-validation">
                   
                    <div class="col-12">
                      <label for="ddlBranch" class="form-label">Branch</label>
                        <asp:DropDownList ID ="ddlBranch" runat="server" class="form-select">
                           
                        </asp:DropDownList>
                        
                    </div>

                    <div class="col-12">
                      <label for="txtSurName" class="form-label">Customer</label>
                    <asp:DropDownList ID ="ddlCustomer" runat="server" class="form-select">
                            <asp:ListItem  Text="BMW" Value="1"></asp:ListItem>
                             <asp:ListItem  Text="Other" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                      
                    </div>
                  <div class="col-12">
                      <label for="txtContainer" class="form-label">Type Ref Number</label>
                     <asp:TextBox id="txtContainer" runat="server" ValidationGroup="TimeSlot"   ClientIDMode="Static" class="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator2" controltovalidate="txtContainer" ErrorMessage="Please type Ref Number" ForeColor="OrangeRed" />
                    </div>
                    <div class="col-12" style="text-align:center;">
                        <asp:ImageButton ID="btnCusNext"  class="btn btn-primary" OnClick="btnCusNext_Click" BackColor="Transparent" Width="120" ImageUrl="~/assets/img/next.png" Text="Next" runat="server" />
                    </div>
                   
                  </div>
       
      </div>

          </div><!-- End Recent Activity -->
             <div class="card" id="divScan" visible="false" runat="server">
           
               <h5 class="card-title" style="text-align:center;background-color: #4090ce;color:white">OutBound Process</h5>
           <div class="section error-404 d-flex flex-column align-items-center justify-content-center">
                <div class="row g-3 needs-validation">
                   
                   

                    <div class="col-12">
                      <label for="txtHU" class="form-label">Scan or Type HU Number</label>
                     <asp:TextBox id="txtHU" runat="server" ValidationGroup="TimeSlot"   ClientIDMode="Static" class="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" id="reqName" controltovalidate="txtHU" ErrorMessage="Please type HU Number" ForeColor="OrangeRed" />
                    </div>
                     <div class="col-12">
                      <label for="txtBin" class="form-label">Type or Scan Bin</label>
                     <asp:TextBox id="txtBin" runat="server" ValidationGroup="TimeSlot" Text=""  ClientIDMode="Static" class="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator3" controltovalidate="txtBin" ErrorMessage="Please type/scan bin" ForeColor="OrangeRed" />
                    </div>
                     <div class="col-12">
                      <label for="txtQty" class="form-label">Quantity</label>
                     <asp:TextBox id="txtQty" runat="server" ValidationGroup="TimeSlot" Text="1" ReadOnly="true" BackColor="#dfe8f0"  ClientIDMode="Static" class="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator1" controltovalidate="txtQty" ErrorMessage="Please Enter Qty" ForeColor="OrangeRed" />
                    </div>
                  </div>
               <div class="row">
                    <div class="col-6">
                        <asp:HiddenField ID="hdnContainer" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnBin" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnInboundFee" runat="server" Value="0" />
                          <asp:Button ID="btnSave" class="btn btn-primary" OnClick="btnSave_Click" Text="Save" runat="server" />
                         </div>
                   <div class="col-6">
                         <asp:Button ID="btnComplete" class="btn btn-primary" OnClick="btnComplete_Click" Text="Scanning Complete" OnClientClick="return myFunction();" BackColor="#bc623c" runat="server" />
                       </div>
                  </div>
        
      </div>

          </div>
         
      </div>
          </div>
    </section>

  </main><!-- End #main -->

</asp:Content>
