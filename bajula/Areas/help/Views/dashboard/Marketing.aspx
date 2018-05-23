<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                Help</h1>
        </div>
    </div>
    <div class="banner_main">
        <div class="content pt20">
            <div id="help_nav" class="fl">
                <% Html.RenderPartial("~/Areas/help/Views/help_navigation.ascx"); %>
            </div>
            <div id="help_content_top" class="ml200">
            </div>
            <div id="help_content" class="ml200">
                <h1>
                    Marketing - Coupons</h1>
                <h2>
                    Why Use Coupons?</h2>
                <p>
                    Coupons are highly effective sales tools. According to research by A.C. Nielson,
                    95% of all shoppers like coupons and 60% actively look for coupons. It has been
                    shown that as the economy in any area deteriorates, the use of coupons increases.</p>
                <p>
                    The following are the main reasons why marketing professionals recommends the use
                    of coupons</p>
                <ol>
                    <li>Coupons are an effective way of expanding your market area</li>
                    <li>Coupons can lure customers from competitors</li>
                    <li>Coupons creates an opportunity for selling related items</li>
                    <li>Coupons will attract new and old customers to visit your store</li>
                </ol>
                <h2>
                    Creating Coupons</h2>
                <img class="img_border mt10" src="/Content/img/help/dashboard/coupons.jpg" alt="creating discount coupons" />
                <p>
                    When creating a coupon, you can specify the following parameters.</p>
                <table>
                    <thead>
                        <tr>
                            <td>
                                Parameter
                            </td>
                            <td>
                                Explanation
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="w200px">
                                Description
                            </td>
                            <td>
                                A short description for the coupon
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Coupon Code
                            </td>
                            <td>
                                A unique code that customers will need to enter into their shopping cart
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Coupon Value
                            </td>
                            <td>
                                A specific amount or a percentage of the total order to be discounted
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Max uses
                            </td>
                            <td>
                                To control the number of times this coupon can be used, you can specify the maximum
                                number of impressions
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Coupon Validity
                            </td>
                            <td>
                                By specifying a start date and/or an end date, you can control the duration this
                                coupon will be valid for
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Minimum purchase
                            </td>
                            <td>
                                You could also allow this coupon to be valid only for total purchases above a certain
                                amount
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h2>
                    Promoting Your Coupons</h2>
                <p>
                    There are numerous websites dedicated to coupons. The following are some recommended
                    sites where you could submit your coupons.</p>
                <ul>
                    <li><a target="_blank" href="http://couponfollow.com">couponfollow.com</a></li>
                    <li><a target="_blank" href="http://www.coupons.com">www.coupons.com</a></li>
                    <li><a target="_blank" href="http://www.couponcabin.com">www.couponcabin.com</a></li>
                    <li><a target="_blank" href="http://www.couponmom.com">www.couponmom.com</a></li>
                    <li><a target="_blank" href="http://www.retailmenot.com">www.retailmenot.com</a></li>
                    <li><a target="_blank" href="http://www.smartsource.com">www.smartsource.com</a></li>
                </ul>
                <div class="mt50 ar">
                    Next: <a href="/help/dashboard/shipping">Shipping Rules</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Help - Marketing
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
