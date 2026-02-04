<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="TravelDashboard.aspx.cs" Inherits="Vivify.TravelDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <main id="main" class="main">
     <style>
    .main {
        background-color: #cadcfc;
        min-height: 85vh;
        display: flex;
        justify-content: center;
        align-items: flex-start;
        padding: 20px;
        box-sizing: border-box;
    }

    .refresh-container {
        width: 98%;
        max-width: 1200px;
        background-color: white;
        box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        border-radius: 8px;
        overflow: hidden;
        display: flex;
        flex-direction: column;
        padding: 20px;
    }

    .refresh-title {
        background-color: #3f418d;
        color: white;
        padding: 15px 0;
        font-size: 20px;
        font-weight: bold;
        text-align: center;
        border-bottom: 1px solid #ddd;
        margin-bottom: 15px;
        border-radius: 8px 8px 0px 0px;
    }

    /* Table Container with Horizontal Scroll */
    .table-container {
        width: 100%;
        overflow-x: auto;
        overflow-y: auto;
        max-height: 300px; /* Fixed height for web view */
        -webkit-overflow-scrolling: touch; /* Smooth scrolling on iOS */
        margin-top: 10px;
        border: 1px solid #ddd;
        border-radius: 8px;
    }

    .refresh-grid {
        width: 100%;
        border-collapse: collapse;
        min-width: 600px; /* Minimum width before scrolling activates */
    }

    .refresh-grid th,
    .refresh-grid td {
        padding: 14px;
        text-align: center;
        border: 1px solid black;
        white-space: nowrap; /* Prevent text wrapping */
    }

    .refresh-grid th {
        background-color: #3f418d;
        color: white;
        font-weight: bold;
        font-size: 15px;
        position: sticky;
        top: 0;
        z-index: 10;
    }

    .refresh-grid td {
        font-size: 14px;
        color: #333;
    }

    /* Button Styles - Unchanged */
    .btnAssign {
        background-color: #3f418d;
        color: white;
        border: none;
        padding: 8px 16px;
        cursor: pointer;
        border-radius: 4px;
        text-decoration: none;
        display: inline-block;
        font-size: 13px;
        transition: background-color 0.3s;
        white-space: nowrap;
    }

    .btnAssign:hover {
        background-color: #2a2c6b;
         color: white;
    }

    .btnVerified {
        background-color: forestgreen;
        color: white;
        border: none;
        padding: 8px 16px;
        cursor: not-allowed;
        border-radius: 4px;
        text-decoration: none;
        display: inline-block;
        font-size: 13px;
        pointer-events: none;
        white-space: nowrap;
    }
    /* Mobile: Stack title and button */
/* Mobile: Stack title and button */
@media (max-width: 768px) {
    .refresh-title {
        flex-direction: column;
        align-items: stretch;
        text-align: center;
        padding: 12px 15px;
    }
    .refresh-title h5 {
        font-size: 18px;
        margin-bottom: 10px;
        width: 100%;
    }
    .refresh-title .btn {
        width: 100%;
        max-width: 200px;
        margin: 0 auto;
        padding: 8px 16px;
        font-size: 14px;
    }
}

/* Reduce font size and padding in grid on mobile */
@media (max-width: 480px) {
    .refresh-grid th,
    .refresh-grid td {
        padding: 6px 4px;
        font-size: 12px;
    }

    .refresh-grid th {
        font-size: 12px;
    }
}
    /* Mobile Optimizations */
    @media (max-width: 768px) {
        .main {
            padding: 10px;
            height: auto;
            min-height: 85vh;
        }

        .refresh-container {
            width: 100%;
            padding: 15px;
            margin: 0;
        }

        .refresh-title {
            font-size: 18px;
            padding: 12px 0;
            margin-bottom: 10px;
        }

        .table-container {
            border-radius: 6px;
            margin-top: 8px;
            max-height: 400px; /* Slightly taller for mobile for better usability */
        }

        .refresh-grid {
            min-width: 500px; /* Slightly smaller min-width for mobile */
        }

        .refresh-grid th,
        .refresh-grid td {
            padding: 10px 8px;
            font-size: 13px;
        }

        .btnAssign,
        .btnVerified {
            padding: 6px 12px;
            font-size: 12px;
        }
    }

    @media (max-width: 480px) {
        .refresh-title {
            font-size: 16px;
            padding: 10px 0;
        }

        .table-container {
            max-height: 350px; /* Adjusted for very small screens */
        }

        .refresh-grid {
            min-width: 450px; /* Even smaller for very small screens */
        }

        .refresh-grid th,
        .refresh-grid td {
            padding: 8px 6px;
            font-size: 12px;
        }

        .btnAssign,
        .btnVerified {
            padding: 5px 10px;
            font-size: 11px;
        }
    }

    /* Scrollbar Styling */
    .table-container::-webkit-scrollbar {
        height: 8px;
        width: 8px;
    }

    .table-container::-webkit-scrollbar-track {
        background: #f1f1f1;
        border-radius: 4px;
    }

    .table-container::-webkit-scrollbar-thumb {
        background: #c1c1c1;
        border-radius: 4px;
    }

    .table-container::-webkit-scrollbar-thumb:hover {
        background: #a8a8a8;
    }

    /* Vertical scrollbar for web view */
    .table-container::-webkit-scrollbar:vertical {
        width: 8px;
    }

    /* No Data Styling */
    .no-data {
        text-align: center;
        padding: 40px;
        color: #666;
        font-size: 16px;
        background: white;
        border-radius: 8px;
        min-height: 200px;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    /* Empty Data Template */
    .empty-grid {
        width: 100%;
        text-align: center;
        padding: 40px;
        color: #666;
    }
</style>

        <!-- Centered Container with Side Padding -->
        <div class="refresh-container">

<!-- Centered Header -->
<div class="refresh-title d-flex flex-column flex-md-row justify-content-between align-items-center px-3 position-relative">
    <!-- Invisible spacer to balance the button width -->
    <div style="width: 90px;"></div>

    <!-- Centered title -->
    <h5 class="mb-0" style="color: white; position: absolute; left: 50%; transform: translateX(-50%); margin: 0; font-size: 18px; text-align: center; width: 100%; max-width: 300px;">
        Travel Expenses
    </h5>

    <!-- New Travel Button (right-aligned) -->
    <asp:Button ID="btnNewTravel" runat="server"
                Text="New Travel"
                CssClass="btn btn-sm"
                style="background-color: white; color: black; border: none; padding: 6px 12px; font-size: 13px;"
                OnClick="btnNewTravel_Click" />
</div>
            <!-- Table Container with Horizontal Scroll -->
            <div class="table-container">
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CssClass="refresh-grid" GridLines="None" OnRowCommand="GridView1_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="First Name">
                            <ItemTemplate>
                                <asp:Label ID="lblFirstName" runat="server" Text='<%# Eval("FirstName") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Travel Date">
                            <ItemTemplate>
                                <%# Eval("Date") == DBNull.Value ? "—" : Convert.ToDateTime(Eval("Date")).ToString("dd/MM/yyyy") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <%# (Eval("StatusId") != null && Eval("StatusId").ToString() == "3") ? 
                                    "<span class='btnVerified'>Verified</span>" : 
                                    "" %>
                                <asp:LinkButton 
                                    ID="lnkAssignTravel" 
                                    runat="server" 
                                    Text="Assign TravelExpenses" 
                                    CssClass="btnAssign"
                                    CommandName="AssignTravel"
                                    CommandArgument='<%# string.Format("{0}|{1}", Eval("EmployeeId"), Eval("Date") ?? "") %>'
                                    Visible='<%# Eval("StatusId") != null && Eval("StatusId").ToString() != "3" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            No travel expenses found.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>

        </div>

    </main>

    <script>
        // Add touch scrolling improvements for mobile
        document.addEventListener('DOMContentLoaded', function () {
            var tableContainer = document.querySelector('.table-container');

            if (tableContainer) {
                // Prevent vertical scroll when horizontally scrolling on touch devices
                tableContainer.addEventListener('touchstart', function (e) {
                    if (e.touches.length === 1) {
                        this.style.overflowY = 'hidden';
                    }
                });

                tableContainer.addEventListener('touchend', function (e) {
                    this.style.overflowY = 'auto';
                });

                // Reset overflow on mouse leave
                tableContainer.addEventListener('mouseleave', function (e) {
                    this.style.overflowY = 'auto';
                });
            }
        });

        // Handle window resize for better mobile experience
        window.addEventListener('resize', function () {
            var tableContainer = document.querySelector('.table-container');
            if (tableContainer) {
                if (window.innerWidth <= 768) {
                    // Mobile view - use responsive max-height
                    tableContainer.style.maxHeight = '400px';
                } else {
                    // Web view - fixed 300px height
                    tableContainer.style.maxHeight = '300px';
                }
            }
        });

        // Initial call to set correct height on page load
        window.dispatchEvent(new Event('resize'));
    </script>

</asp:Content>