<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<SelectListItem>>" %>
<div class="form_group">
<div class="form_entry">
    <div class="form_label">
        <label for="ebay_shippingsites">
            eBay Site</label>
    </div>
    <%= Html.DropDownList("ebay_shippingsites", Model)%>
</div>    
<% Html.RenderPartial("ebayProfileContainer"); %>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        tradelr.dashboard.ebay.shipping_init('#ebay_shippingsites');
    });
</script>
