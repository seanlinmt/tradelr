<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div id="banner">
        <div class="content">
            <h1>Help</h1>
        </div>
    </div>
    <div class="banner_main">
        <div class="content pt20">
            <div id="help_nav" class="fl">
            <% Html.RenderPartial("~/Areas/help/Views/help_navigation.ascx"); %>
            </div>
            <div id="help_content_top" class="ml200"></div>
            <div id="help_content" class="ml200">
            <h1>Documentation</h1>
            <p>The following are some resources that will help you get started with our platform.</p>
            <ul class="fl mt20" id="features_table">
            <li>
            <div class="desc">
            <h3><a href="/help/dashboard">Dashboard</a></h3>
            <p>Information on various sections of the tradelr dashboard
            </p>
            </div>
            </li>
            <li>
            <div class="desc">
            <h3><a href="/help/storefront">Storefront</a></h3>
            <p>Information on how to customize your online store
            </p>
            </div>
            </li>
            
            </ul>
            </div>
            <div id="help_content_bottom" class="ml200"></div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
Help
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
