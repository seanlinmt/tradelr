<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.account.plans.PlanViewData>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Library" %>
<%@ Import Namespace="tradelr.Payment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Change Current Plan
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="pt10">
    <h3 id="headingPlans">
        change current plan</h3>
    <%
        Html.RenderControl(TradelrControls.pricingPlans, Model); %>
        <p>Payment can be made either by Credit Card or Paypal. <img src="/Content/img/payments/acceptedPayments.png" alt="" /></p>

        <div class="hidden">
        <form id="paypalForm" action="<%= PaymentConstants.PaypalPostAddress %>" method="post">
            <input type="hidden" name="cmd" id="cmd" value="" />
            <input type="hidden" value="<%= PaymentConstants.PaypalSubscribeEmail %>" name="business" />
	        <input type="hidden" value="1" name="no_shipping"/>
	        <input type="hidden" value="2" name="rm"/>
            <input type="hidden" name="custom" value="<%= Model.subdomainid %>" />
            <input id="item_name" type="hidden" name="item_name" value="" />
            <input id="no_note" type="hidden" name="no_note" value="1" />
            <input type="hidden" value="<%= PaymentConstants.PaypalIPNUrl %>" name="notify_url" />
            <input type="hidden" value="<%= Model.hostName.ToDomainUrl("/dashboard/account") %>" name="return" />
	        <input type="hidden" value="<%= Model.hostName.ToDomainUrl("/dashboard/account/plan") %>" name="cancel_return" />
        </form>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            $('#navaccount').addClass('navselected_white');

            $('#plan-single').click(function () {
                $.post('/dashboard/account/plan/single', null, function (json_result) {
                    if (json_result.success) {
                        $('#cmd').val('_xclick-subscriptions');
                        var input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'a3').attr('name', 'a3').attr('value', '9');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'p3').attr('name', 'p3').attr('value', '1');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 't3').attr('name', 't3').attr('value', 'M');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'src').attr('name', 'src').attr('value', '1');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'sra').attr('name', 'sra').attr('value', '1');
                        $('#paypalForm').append(input);
                        $('#item_name').val('tradelr.com - Single');
                        $('#paypalForm').trigger('submit');
                    }
                    else {

                    }
                }, 'json');
                return false;
            });

            $('#plan-basic').click(function () {
                $.post('/dashboard/account/plan/basic', null, function (json_result) {
                    if (json_result.success) {
                        $('#cmd').val('_xclick-subscriptions');
                        var input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'a3').attr('name', 'a3').attr('value', '19');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'p3').attr('name', 'p3').attr('value', '1');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 't3').attr('name', 't3').attr('value', 'M');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'src').attr('name', 'src').attr('value', '1');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'sra').attr('name', 'sra').attr('value', '1');
                        $('#paypalForm').append(input);
                        $('#item_name').val('tradelr.com - Basic');
                        $('#paypalForm').trigger('submit');
                    }
                    else {

                    }
                }, 'json');
                return false;
            });

            $('#plan-pro').click(function () {
                $.post('/dashboard/account/plan/pro', null, function (json_result) {
                    if (json_result.success) {
                        $('#cmd').val('_xclick-subscriptions');
                        var input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'a3').attr('name', 'a3').attr('value', '49');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'p3').attr('name', 'p3').attr('value', '1');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 't3').attr('name', 't3').attr('value', 'M');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'src').attr('name', 'src').attr('value', '1');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'sra').attr('name', 'sra').attr('value', '1');
                        $('#paypalForm').append(input);
                        $('#item_name').val('tradelr.com - Pro');
                        $('#paypalForm').trigger('submit');
                    }
                    else {

                    }
                }, 'json');
                return false;
            });

            $('#plan-ultimate').click(function () {
                $.post('/dashboard/account/plan/ultimate', null, function (json_result) {
                    if (json_result.success) {
                        $('#cmd').val('_xclick-subscriptions');
                        var input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'a3').attr('name', 'a3').attr('value', '99');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'p3').attr('name', 'p3').attr('value', '1');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 't3').attr('name', 't3').attr('value', 'M');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'src').attr('name', 'src').attr('value', '1');
                        $('#paypalForm').append(input);
                        input = $('<input>');
                        $(input).attr('type', 'hidden').attr('id', 'sra').attr('name', 'sra').attr('value', '1');
                        $('#paypalForm').append(input);
                        $('#item_name').val('tradelr.com - Ultimate');
                        $('#paypalForm').trigger('submit');
                    }
                    else {

                    }
                }, 'json');
                return false;
            });

        });
    </script>
</asp:Content>
