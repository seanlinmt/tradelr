<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Myspace.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.OpenSocial.Models.OpenSocialViewData>" %>
<%@ Import Namespace="tradelr.Common.Constants" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	  <h2>
		Enter a tradelr store address
	  </h2>
	  <div>
		<div>
		  <label>tradelr Store Address</label>
		</div>
		<div class="tip">Example: http://nice2tradez.tradelr.com</div>
		<div class="mt5">
		  <input type="text" name="address" id="address" />
		  <span id="inputerror" class="error"></span>
		</div>
		<div class="mt10">
		  <img id="buttonSave" class="clickable" src="http://os.tradelr.com/Content/img/save.png" alt="Save" />
		</div>
	  </div>
	  <div class="mt20">
		<h2>
		  Not a member?
		</h2>
		<div>
		  <p>
			In order to list your products using tradelr, you'll need to register an account.
		  </p>
		  <p>
			Create an account for free. All you need is a valid email address.
		  </p>
		  <a target="_blank" href="http://www.tradelr.com/pricing">
			<img src="http://os.tradelr.com/Content/img/signup.png" alt="Sign Up" />
		  </a>
		</div>
	  </div>
</asp:Content>
