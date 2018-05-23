<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.shipping.EbayShippingProfile>" %>
<div class="pad5 ba box_shadow mt20" style="max-width: 915px;">
<div class="fr">
    <a id="deleteShippingProfile" href="#" class="icon_del smaller hidden">delete shipping profile</a>
</div>
<div class="clear">
</div>
<div class="info">To specify free shipping, select a shipping service and set a cost of 0. Buyers pay all shipping costs.</div>
<div class="w450px inline-block at mr10">
    <div class="section_header">Domestic Shipping</div>
    <div class="form_group">
    <a id="ebay_add_domestic" href="#" class="icon_add">add domestic service</a>
    <table id="ebay_domestic_rules" class="header_bold">
        <thead>
            <tr><td>Shipping Service</td><td class="ar">Cost</td><td></td></tr>
        </thead>
        <tbody>
        <%foreach (var rule in Model.domesticRules)
          {
              Html.RenderPartial("EbayServicesSingleRow", rule);
          } %>
           </tbody>
    </table>
    </div>
</div>
<div class="w450px inline-block at">
    <div class="section_header">International Shipping</div>
    <div class="form_group">
    <a id="ebay_add_international" href="#" class="icon_add">add international service</a>
    <table id="ebay_international_rules" class="header_bold">
        <thead>
            <tr><td>Shipping Service</td><td class="ar">Cost</td><td></td></tr>
        </thead>
        <tbody>
        <% foreach (var rule in Model.internationalRules)
           {
               Html.RenderPartial("EbayServicesSingleRow", rule);
           }%>
           </tbody>
    </table>
    </div>
</div>
</div>