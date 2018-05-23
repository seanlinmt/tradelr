<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Facebook.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Facebook.Models.facebook.FacebookViewData>" %>
<%@ Import Namespace="tradelr.Library.Constants" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Welcome to tradelr for Facebook</h1>
<div class="contentdiv">
<h3>Why add tradelr to your Facebook Page?</h3>
<ul class="list_tick mt10">
<li>Allow friends and fans to share your products</li>
<li>Reveal discount codes only to those who likes your fan page</li>
<li>Draw traffic to your store on <a target="_blank" href="http://www.tradelr.com">tradelr.com</a></li>
</ul>
<p>
<a  href="http://www.facebook.com/add.php?api_key=<%= GeneralConstants.FACEBOOK_API_KEY %>&pages=1" 
class="uiButton uiButtonDefault uiButtonMedium">
<i class="customimg img spritemap_d13xof sx_cd2c5b"></i><span class="uiButtonText" style="">Add to Fan Page</span></a>
</p>
</div>
<div class="imgdiv">
<img src="http://fb.tradelr.com/Content/img/canvas.jpg" alt="" />
</div>
</asp:Content>