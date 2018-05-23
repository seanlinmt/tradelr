<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.facebook.viewmodel.FBImportPhotoViewModel>" %>
<div class="content mb10">
<div class="fl w150px bg_lightgrey am ac">
<% foreach (var photoLink in Model.photo_links)
   {%>
    <img alt="" src="<%=photoLink%>" />
    <%
   }%>
</div>
<div class="fl ml10">
<input type="text" id="title_product" name="title_product" value="<%= Model.name %>" class="mr10" title="product title" />
<input type="text" id="sku" name="sku" value="<%= Model.sku %>" class="mr10" title="product SKU (unique ID)"  />
<input type="text" id="price" name="price" value="<%= Model.price %>" class="mr10" title="selling price" />
<div class="mt10">
<textarea id="details" name="details" class="w100p block" title="product description"><%= Model.details %></textarea>
</div>
</div>
<div class="fr selection">

</div>
<span class="hidden contentid"><%= Model.id %></span>
<span class="hidden contentids"><%= string.Join(",",Model.ids) %></span>
<div class="clear"></div>
</div>