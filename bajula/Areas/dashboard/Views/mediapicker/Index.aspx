<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<tradelr.Areas.dashboard.Models.orchard.media.viewmodels.MediaFolderEditViewModel>" %>
<%@ Import Namespace="tradelr.Library" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pick Image</title>
    <script type="text/javascript" src="/Scripts/jquery-1.6.4.min.js"></script>
    <script type="text/javascript" src="/Scripts/jquery/ui/jquery.ui.core.min.js"></script>
    <script type="text/javascript" src="/Scripts/jquery/ui/jquery.ui.widget.min.js"></script>
    <script type="text/javascript" src="/Scripts/jquery/ui/jquery.ui.tabs.min.js"></script>
    <script type="text/javascript" src="/Scripts/media/mediabrowser.js"></script>
    <%= Html.CssInclude("/css/mediapicker","screen, print") %>
</head>
<body id="tradelrmediapicker">
<div id="tabs" class="group">
    <ul>
        <li><a href="#tab-url" data-edittext="Update/Upload Image" data-edittext-content="true">Insert/Upload Image</a></li>
        <li><a href="#tab-gallery">Browse Media</a></li>
    </ul>
    <div id="tab-url">
<% Html.RenderPartial("Tab_Url", Model); %>
    </div>
    <div id="tab-gallery">
<% Html.RenderPartial("Tab_Gallery", Model); %>
    </div>
</div>
</body>
</html>
