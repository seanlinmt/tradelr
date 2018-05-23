<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
                    Shipping</h1>
                    <p>Specifying shipping cost rules will allow the seller to automatically provide buyers
                        with estimated shipping costs. This will help shorten the checkout process. 
                        Otherwise, a customer's order will need to be updated after a customer places their order.</p>
                    <p>There are three methods of specifying shipping costs</p>
                    <ol>
                    <li><strong>flat rate</strong> - where you specify specific costs to specific shipping destinations</li>
                    <li><strong>calculated</strong> - where you specify shipping costs based on total weight or total price</li>
                    <li><strong>automatic</strong> - where you use <a href="http://www.shipwire.com/pp/o.php?id=5191" target="_blank">Shipwire</a> to automatically provide shipping costs</li>
                    </ol>

                    <h2>Example</h2>
                    <p>
                            Let us imagine that I have an online shop on Tradelr selling Manuka honey in Auckland.
                            Using weight-based rules, I will structure my shipping costs as follows:</p>
                        <ol>
                            <li>Cost will be based on the total weight of the order. </li>
                            <li>Free shipping in Auckland.</li>
                            <li>Everywhere else in New Zealand will cost $10 for 0 to 5 kg and $20 for 5kg or more.</li>
                            <li>Charge a flat rate of $100 for the rest of the world</li>
                        </ol>
                        <p>
                            The following table shows the rules that I would need to create for the shipping
                            costs defined above.</p>
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        <strong>Name</strong>
                                    </td>
                                    <td>
                                        <strong>Country</strong>
                                    </td>
                                    <td>
                                        <strong>State</strong>
                                    </td>
                                    <td>
                                        <strong>Type</strong>
                                    </td>
                                    <td>
                                        <strong>Effective Value</strong>
                                    </td>
                                    <td>
                                        <strong>Shipping Cost</strong>
                                    </td>
                                    <td>
                                        <strong>Description</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Free Shipping<br/>
                                    </td>
                                    <td>
                                        NZ
                                    </td>
                                    <td>
                                        Auckland
                                    </td>
                                    <td>
                                        weight
                                    </td>
                                    <td>
                                        0
                                    </td>
                                    <td>
                                        0
                                    </td>
                                    <td>
                                        Free shipping for Auckland<br/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        NZPost<br/>
                                    </td>
                                    <td>
                                        NZ
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                        weight
                                    </td>
                                    <td>
                                        0
                                    </td>
                                    <td>
                                        10
                                    </td>
                                    <td>
                                        $10 for 0 - 5kg in NZ<br/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        NZPost<br/>
                                    </td>
                                    <td>
                                        NZ
                                    </td>
                                    <td>
                                        <br/>
                                    </td>
                                    <td>
                                        weight
                                    </td>
                                    <td>
                                        5
                                    </td>
                                    <td>
                                        20
                                    </td>
                                    <td>
                                        $20 for 5kg or more in NZ<br/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        International<br/>
                                    </td>
                                    <td>
                                        Others
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                        weight
                                    </td>
                                    <td>
                                        0
                                    </td>
                                    <td>
                                        100
                                    </td>
                                    <td>
                                        The rest of the world<br/>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="mt20">For a more detailed explanation, please refer to this <a href="http://blog.tradelr.com/new-shipping-cost-rules" target="_blank">blog post on shipping rules</a>. 
                        If you're having problems, please don't hesitate to contact us for help.</div>
                        <h2>eBay Shipping</h2>
                        <img class="img_border mt10 mb10" src="/Content/img/help/dashboard/ebay_shipping.jpg" alt="eBay shipping" />
                        <p>When posting products on eBay, an eBay specific shipping profile needs to be specified.</p>
                        <p>Depending on the eBay marketplace selected, you will be required to define local and international shipping services. 
                        For international shipping services, you must specify shipping destinations.</p>
                        <p>Shipping profiles are defined separately for each eBay marketplace.</p>
                        <div class="mt50 ar">
                    Next: <a href="/help/dashboard/account">Account Settings</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
Help - Shipping
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
