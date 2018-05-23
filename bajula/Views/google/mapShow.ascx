<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GoogleMapData>" %>
<%@ Import Namespace="tradelr.Models.google"%>
<%@ Import Namespace="tradelr.Common"%>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>
<div id="map_content">
<div id="profile_map" style="width:745px;height:270px">
</div>
<%= Html.Hidden("latitude", Model.latitude) %>
<%= Html.Hidden("longtitude", Model.longtitude) %>
<%= Html.Hidden("mapzoom", Model.mapZoom) %>
<div class="clear"></div>
</div>
<div class="pt5">
<button id="buttonClose" class="small">close map</button>
</div>
<%= Html.RegisterViewJS() %>
