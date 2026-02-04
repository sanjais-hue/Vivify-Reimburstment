<%@ Control Language="C#" AutoEventWireup="true" Codefile="ViewSwitcher.ascx.cs" Inherits="Vivify.ViewSwitcher" %>
<div id="viewSwitcher">
    <%: CurrentView %> view | <a href="<%: SwitchUrl %>" data-ajax="false">Switch to <%: AlternateView %></a>
</div>