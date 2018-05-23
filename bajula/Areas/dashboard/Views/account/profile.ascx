<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.account.AccountViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Models.users" %>
<%
    Html.RenderPartial("~/Areas/dashboard/Views/contacts/userInfo.ascx", Model.contact);%>
<div class="section_header">
    Organization Information</div>
<%
    Html.RenderPartial("~/Areas/dashboard/Views/contacts/organisationInfo.ascx", Model);%>
<% if (Model.contact.permissions.HasPermission(UserPermission.NETWORK_SETTINGS))
   {%>
<div class="section_header">
    Account Settings</div>
<div class="form_group">
    <div class="form_entry">
        <div class="form_label">
            <label for="timezone" class="required">
                Timezone</label>
        </div>
        <%=Html.DropDownList("timezone", Model.timezoneList,
                                                        new Dictionary<string, object> {{"style", "width:380px"}})%>
    </div>
    <div class="form_entry">
        <div class="form_label">
            <label for="currency" class="required">
                Currency</label></div>
        <%=Html.DropDownList("currency", Model.currencyList,
                                                        new Dictionary<string, object> {{"style", "width:300px"}})%>
    </div>
</div>
<%
                   }%>
