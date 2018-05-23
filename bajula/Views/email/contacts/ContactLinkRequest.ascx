<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.contacts.viewmodel.ContactLinkRequestViewData>" %>
<p><%= Model.senderName %>(<%= Model.senderEmail %>) wishes to link their product inventory with yours.</p>
<p>By linking inventories, both of you will be able to view each others' products and inventory levels.</p>
<p>To confirm or quietly ignore this request, go to: </p>
<p><a href="<%= Model.notificationLink %>"><%= Model.notificationLink %></a></p>
<p>&nbsp;</p>
<p>Regards,</p>
<p>The Tradelr Team</p>
