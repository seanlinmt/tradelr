<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.checkout.Models.PaymentViewModel>" %>
<h2 class="mt30">
        Payment Method</h2>
    <div class="content">
        <% if (Model.paymentMethods.count > 1)
    {%>
        Choose how you would like to pay for your order.
        <ul id="payment_methods_list">
            <% foreach (var method in Model.paymentMethods.items)
               {%>
            <li>
                <input id="pmethod_<%= method.Value %>" type="radio" name="paymentmethod" value="<%= method.Value %>" />
                <label for="pmethod_<%= method.Value %>"><%= method.Text %></label></li>
            <%} %>
        </ul>
        <% }
   else {%>
    <p><%= Model.paymentMethods.items[0].Text %></p>
        <%= Html.Hidden("paymentmethod", Model.paymentMethods.items[0].Value)%>
    <%}%>
    </div>
    
