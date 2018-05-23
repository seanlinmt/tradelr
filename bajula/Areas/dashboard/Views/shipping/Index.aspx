<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Areas.dashboard.Models.shipping.viewmodel.ShippingViewModel>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Shipping
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class="content_area">
        <div id="shipping_tabs">
                <ul class="hidden">
                    <li><a href="#shipping"><img src="/Content/img/streamsource/tradelr.png" /> shipping</a></li>
                    <li><a href="#ebay_shipping"><img src="/Content/img/social/icons/ebay_14.png" /> shipping</a></li>
                </ul>
                <div id="shipping" class="hidden">
                    <form id="shippingForm" action="<%= Url.Action("Settings","Shipping", new { Area = "Dashboard"}) %>" method="post">
                <div class="info">Shipping profiles for use within Tradelr</div>
                    <% Html.RenderPartial("shipping", Model.shippingProfiles);%>
                    <div class="clear"></div>
                    <div class="buttonRow_bottom hidden">
                    <span class="mr10">
                        <button id="buttonSave" type="button" class="large green ajax">
                            <img src="/Content/img/save.png" alt='' />
                            save</button>
                    </span>
                </div>
                    </form>
                </div>
                <div id="ebay_shipping" class="hidden">
                    <% Html.RenderPartial("ebay_shipping", Model.ebay_sites);%>
                </div>
            </div>
        </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">

    $(document).ready(function () {
        $('#navshipping').addClass('navselected');
        var $tabs = $('#shipping_tabs').tabs({
            select: function (event, ui) {
            }
        });

        $('#buttonSave', '#shippingForm').click(function () {
            $(this).buttonDisable();
            $('#shippingForm').trigger('submit');
        });
        $(window).trackUnsavedChanges('#buttonSave');

        $('#shippingprofile').appendable('/dashboard/shipping/addprofile', 'Create New Profile');
    });

    $('#shippingForm').submit(function () {
        var action = $(this).attr("action");

        var ok = $('#shippingForm').validate({
            invalidHandler: function (form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false
        }).form();
        if (!ok) {
            $('#buttonSave', '#shippingForm').buttonEnable();
            return false;
        }
        var serialized = $(this).serialize();
        // post form
        $.ajax({
            type: "POST",
            url: action,
            dataType: "json",
            data: serialized,
            success: function (json_data) {
                if (json_data.success) {
                    $.jGrowl('Settings successfully updated');
                    scrollToTop();
                }
                else {
                    $.jGrowl(json_data.message, { sticky: true });
                }
                $('#buttonSave', '#shippingForm').buttonEnable();
                return false;
            }
        });
        return false;
    });

</script>
</asp:Content>
