<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.shipping.viewmodel.EbayServicesEditViewModel>" %>
<tr>
    <td>
        <%= Html.DropDownList("service_name", Model.servicesList, new Dictionary<string, object>(){{"class","w250px"}}) %>
    <% if (Model.isInternational){%>
  <div class="mt6">
      <div><strong>Ship To Location</strong></div>
      <%= Html.DropDownList("shipToLocation", Model.locationList) %> 
      <button class="small buttonEbayAddLocation" type="button">add destination</button>
      <ul id="shipToLocation_list" class="mt10 ml10 normal5">
      </ul>
  </div>
      <% } %>
    </td>
    <td>
        <input type="text" id="cost" class="w75px" />
    </td>
    <td class="w60px ac">
        <div class="action_cancel">&nbsp;</div>
        <div class="action_ok">&nbsp;</div>
    </td>
</tr>
<script type="text/javascript">
    $(document).ready(function () {
        inputSelectors_init('table');
        $('#cost', '#ebay_shipping_profile').numeric({ allow: '.' });
    });
</script>