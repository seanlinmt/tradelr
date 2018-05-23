<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Reports
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="content_area_2col">
<div id="content_main">
<h3 class="headingFeatures">reports</h3>
<p>Reports are currently unavailable for your account.</p>
</div>
<div id="content_side">
<div class="top"></div>
<div class="middle">
<div class="content_side_box">
<div class="header">Finance Reports</div>
<div class="content">
<ul>
<li>Accounts Aging</li>
<li>Profit and Loss</li>
</ul>
</div>
</div>
<div class="content_side_box">
<div class="header">Invoice Reports</div>
<div class="content">
<ul>
<li>Invoice Details</li>
<li>Item Sales</li>
<li>Revenue by Client</li>
</ul>
</div>
</div>
<div class="content_side_box">
<div class="header">Inventory Reports</div>
<div class="content">
<ul>
<li>Supplier Lead Time</li>
</ul>
</div>
</div>
</div>
<div class="bottom"></div>
</div>
<div class="clear"></div>
</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        $.post('/monitor', { feature: "reporting" });
    });
</script>
</asp:Content>
