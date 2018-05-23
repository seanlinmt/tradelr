<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.checkout.Models.ShippingViewModel>" %>
<h2 class="cart_subheader">
        Delivery Address</h2>
    <div class="content">
        <%= Model.shippingAddress %>
        <a href="#" id="editAddresses">edit</a>
    </div>
    <h2 class="cart_subheader">
        Shipping Method</h2>
    <div class="content">
        <% if (Model.shippingMethods != null && Model.shippingMethods.Count() != 0)
           {%>
        <p>
            Select how you would like your order delivered to you.</p>
        <%= Html.DropDownList("shippingmethod", Model.shippingMethods) %>
        <% } %>
    </div>
