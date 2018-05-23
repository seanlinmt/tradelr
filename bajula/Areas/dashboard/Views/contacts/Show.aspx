<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Areas.dashboard.Models.contact.ContactViewModel>" %>

<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Models.address" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= string.IsNullOrEmpty(Model.contact.fullName) ? "Contact Information" : Model.contact.fullName%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area_2col">
        <div id="content_main" class="w650px mt10">
            <div id="profile_photo" class="fl">
                <%
                    if (Model.contact.profilePhoto != null)
                    {%>
                <img src='<%=Model.contact.profilePhoto.url%>' alt='<%=Model.contact.profilePhoto.id%>' />
                <%}
                    else
                    {%>
                <img src='/Content/img/profile_nophoto_64.png' alt='' />
                <%}%>
            </div>
            <div class="fl ml10">
                <h3 class="fl">
                    <%= string.IsNullOrEmpty(Model.contact.fullName) ? "Contact Information" : Model.contact.fullName%></h3>
                <span class="font_grey">
                    <%= Model.contact.contactTypeLink%></span>
                <div class="clear">
                </div>
                <div class="fl">
                    <%
                        if (!Model.contact.isOwner)
                        {
                            if (Model.contact.isPrivate)
                            {%>
                    <button id="buttonEdit" class="small" type="button">
                        edit</button>
                    <%} %>
                    <button id="buttonInvoice" class="small" type="button">
                        create invoice</button>
                    <button id="buttonOrder" class="small" type="button">
                        create order</button>
                    <% } %>
                </div>
            </div>
            <div class="clear">
            </div>
            <% if (!Model.contact.isOwner && !string.IsNullOrEmpty(Model.contact.notes))
               {%>
            <div class="stickynote mt10">
                <%: Model.contact.notes%>
            </div>
            <% } %>
            <div id="contact_tabs" class="tabs_clear">
                <ul class="hidden">
                    <li><a href="#activity">activity</a></li>
                    <li><a href="#products">products bought/sold</a></li>
                </ul>
                <div id="activity" class="hidden">
                    <%
                        foreach (var comment in Model.comments.OrderByDescending(x => x.created))
                        {
                            Html.RenderPartial("contextualcomment", comment);
                        } %>
                </div>
                <div id="products" class="hidden">
                    <div id="search_area" class="fr">
                        <span class="search"></span>
                        <input type="text" name="searchbox" id="searchInput" class="searchbox" />
                    </div>
                    <div class="clear"></div>
                    <div id="product_transactions_results">
                    <% Html.RenderAction("productTransactions", new{ id = Model.contact.id}); %>
                    </div>
                </div>
            </div>
        </div>
        <div id="content_side" class="w300px">
            <div class="top">
            </div>
            <div class="middle">
                <div class="content_side_box">
                    <div class="header">
                        Contact Information</div>
                    <div class="content">
                        <ul>
                            <li><span class="icon_contact">
                                <%= Model.contact.fullName %></span> </li>
                            <li>
                                <%= Model.contact.gender %></li>
                            <li>
                                <%= Model.contact.email %></li>
                            <li>
                                <%= Model.contact.phone %></li>
                            <li><span class="icon_company">
                                <%= Model.contact.companyName %></span></li>
                            <li>
                                <%= Model.contact.address %></li>
                            <%if (!string.IsNullOrEmpty(Model.contact.coPhone))
                              { %>
                            <li><span class="font_grey w50px inline-block">work</span><%= Model.contact.coPhone %></li>
                            <%} %>
                            <%if (!string.IsNullOrEmpty(Model.contact.fax))
                              { %>
                            <li><span class="font_grey w50px inline-block">fax</span><%= Model.contact.fax %></li>
                            <%} %>
                        </ul>
                    </div>
                </div>
                <div class="content_side_box">
                    <div class="header">
                        Billing Address</div>
                    <div class="content">
                        <%= Model.contact.billingAddress %>
                    </div>
                </div>
                <div class="content_side_box">
                    <div class="header">
                        Shipping Address</div>
                    <div class="content">
                        <%= Model.contact.shippingAddress %>
                    </div>
                </div>
            </div>
            <div class="bottom">
            </div>
        </div>
    </div>
    <%= Html.Hidden("orgid", Model.contact.orgid)%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#navcontact').addClass('navselected_white');
            var contactid = getPageID();
            $('#buttonEdit').click(function () {
                window.location = '/dashboard/contacts/edit/' + contactid;
            });
            $('#buttonInvoice').click(function () {
                window.location = '/dashboard/invoices/add?contact=' + $('#orgid').val();
            });
            $('#buttonOrder').click(function () {
                window.location = '/dashboard/orders/add?contact=' + $('#orgid').val();
            });

            if ($('#activity').children().length == 0) {
                $('#activity').append("<p class='mt20 font_darkgrey'>No recent activity.</p>");
            }

            $('#contact_tabs').tabs();

            var searchtimer;
            var searchterm = "";
            // login name availability check
            $('#searchInput', '#products').keyup(function () {
                searchterm = $(this).val();

                if (searchtimer !== undefined) {
                    clearTimeout(searchtimer);
                }
                searchtimer = setTimeout(function () {
                    $.post('/dashboard/contacts/producttransactions/<%= Model.contact.id %>', { term: searchterm }, function (result) {
                        $('#product_transactions_results', '#products').html(result);
                    });
                }, 500);
            });
        });
    </script>
</asp:Content>
