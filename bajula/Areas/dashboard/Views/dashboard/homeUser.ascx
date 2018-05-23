<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.home.HomeViewData>" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<div id="content_main">
<div id="activity_list">
    <%
        if (Model.stats != null)
        {
            Html.RenderPartial("statistics", Model.stats);
        }
        Html.RenderPartial("activities", Model.activities.tradelr); %>
</div>
</div>
<div id="content_side">
    <div class="top">
    </div>
    <div class="middle">
        <div class="content_side_box">
            <div class="header">
                Why Get Your Own Account?
            </div>
            <div class="content">
                <ul class="bullets_green">
                <li>Send purchase orders</li>
                <li>Link your inventory with others</li>
                <li>Send out invoices</li>
                <li>Manage your inventory</li>
                <li>Create your own store</li>
                <li>Use your own domain name</li>
                </ul>
                <p class="mt20 icon_add"><a href="<%= GeneralConstants.HTTP_HOST %>/pricing">Register Now!</a></p>
            </div>
        </div>
    </div>
    <div class="bottom">
    </div>
</div>
<div class="clear">
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('#navhome').addClass('navselected_white');
    });
</script>