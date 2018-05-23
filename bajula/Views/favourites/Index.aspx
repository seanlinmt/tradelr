<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.favourites.FavouritesViewData>" %>

<%@ Import Namespace="tradelr.Libraries" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Favourites
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area pt5">
        <div class="content_filter">
            <h3 id="headingFav" title="this page shows favourited products">
                Favourites</h3>
            <br />
            <div id="categoryList" class="filter">
                <h4>
                    product categories</h4>
                <div class="sideboxEntry selected pl4" fid="">
                    <strong>All</strong>
                </div>
                <%= Html.FilterBoxList("categoryList", Model.categoryList)%>
            </div>
        </div>
        <div class="main_columnright">
            <div id="grid_content">
                <div class="buttonRow">
                </div>
                <table id="favGridView" class="scroll">
                </table>
                <div id="favNavigation" class="scroll" style="text-align: center;">
                </div>
            </div>
        </div>
        <div class="clear">
        </div>
        <span id="filterBy" style='display: none'></span>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <%= Html.RegisterViewJS() %>
</asp:Content>
