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
                    Ebay Marketplace Integration</h1>
                    <p>With our integration with eBay, the following can be done from your dashboard</p>
                    <ol>
                        <li>post products to ebay</li>
                        <li>automatically download new and recent orders from ebay</li>
                        <li>revise listings to adjust quantity</li>
                        <li>choose to automatically relist an in stock item when the current listing ends</li>
                        <li>mark an order as shipped</li>
                        <li>end active listings</li>
                    </ol>
                    <h2>Linking with eBay</h2>
                    <p>To enable the above functionality, you will first need to link your Tradelr account with eBay</p>
                    <ol>
                    <li>
                        <span>First, click on networks under your account dropdown from your admin dashboard. Click on the eBay logo</span>
                    <img class="img_border mt10 mb20" src="/Content/img/help/integration/networks_ebay.jpg" alt="link with eBay" />
                     </li>
                    <li> <span>Click on connect and you will be redirected to eBay. Once you have grant permission to Tradelr, you will be redirected back to your Tradelr dashboard.</span>
                    <img class="img_border mt10" src="/Content/img/help/integration/ebay.jpg" alt="eBay grant permission" />
                    </li>
                    </ol>
                    <h2>Order synchronisation</h2>
                    <p>When your Tradelr account is first linked with eBay, your orders from the previous month will be downloaded to your Tradelr account.
                    Products, invoices and buyer contacts will be automatically created and saved.</p>
                    <p>Checks for any new orders placed by buyers are done every hour automatically.</p>
                   
                    
                    <h2>Posting products to eBay</h2>
                    <img class="img_border mt10 mb10" src="/Content/img/help/integration/ebay_postproducts.jpg" alt="post to eBay" />
                    <p>Listing products on eBay is done while creating or updating any existing product. 
                    Tick the eBay checkbox if you want to post the current product or upload any changes to eBay.</p>
                    <p>Some important notes:</p>
                    <ol>
                        <li>Paypal must be specified as a payment method. This is done under account settings.</li>
                        <li>You can only post to eBay sites that support the currency you have configured for your account.</li>
                    </ol>
                    <p>The following fields must be specified when posting a product to eBay</p>
                    <table>
                        <thead>
                            <tr><td>Parameter</td><td>Description</td></tr>
                        </thead>
                        <tbody>
                            <tr><td>Site</td><td>The eBay marketplace you wish to list your product</td></tr>
                            <tr><td>Category</td><td>The eBay category to list your product in</td></tr>
                            <tr><td>Condition</td><td>If available for your selected category, the condition of your product</td></tr>
                            <tr><td>Duration</td><td>The number of days to list your product</td></tr>
                            <tr><td>Quantity</td><td>The number of items to list</td></tr>
                            <tr><td>Handling Time</td><td>The time between payment received and when the product will be shipped</td></tr>
                            <tr><td>Return Policy</td><td>Whether returns are accepted</td></tr>
                            <tr><td>Shipping Profile</td><td>Domestic and international shipping options. For more details, please refer to <a href="/help/dashboard/shipping">Shipping</a>.</td></tr>
                            <tr><td>Returns Within</td><td>If returns are accepted, returns will only be accepted within the specified days</td></tr>
                            <tr><td>Refund Policy</td><td>If returns are accepted, will the item be exchanged for a similar product or will it be a refund</td></tr>
                            <tr><td>Auto Relist</td><td>If checked, the product will be relisted once the current listing ends. Product must be in stock for this to happen.</td></tr>
                            <tr><td>Include Contact Information</td><td>Whether to include the seller's organisational contact information</td></tr>
                        </tbody>
                    </table>
                    
                    <h2>Relist products</h2>
                    <img class="img_border mt10 mb10" src="/Content/img/help/integration/ebay_relist.jpg" alt="eBay relist product" />
                    <p>If auto relisting is not enabled, individual products can be relisted with the same saved parameters or with a new set of parameters.</p>

                    <h2>End active listings</h2>
                    <img class="img_border mt10 mb10" src="/Content/img/help/integration/ebay_endlisting.jpg" alt="eBay end listing" />
                    <p>Any active listings can be ended from Tradelr. You will be prompted for a reason for ending your listing early.</p>

                    <h2>Mark orders as shipped</h2>
                    <img class="img_border mt10 mb10" src="/Content/img/help/integration/ebay_shiporder.jpg" alt="eBay ship order" />
                    <p>Once payment has been received from the buyer, the order can be marked as dispatched by clicking on the <strong>ship order</strong> button when viewing an order.</p>
                    <p>At this point, the shipping service used, tracking number (if available) and a feedback should be specified. The status of the order will be updated on eBay and the buyer will be notified automatically.</p>
                    
                     <p>However, orders with multiple order items are handled a little differenty by Tradelr. On eBay, the shipping and payment status is recorded individually for each order item. On tradelr, shipping and payment status is on a per order basis.</p>
<p> This means that when an order is marked as shipped in Tradelr, each order item on eBay will be recorded as having been shipped using the same shipping service.</p>
                    <div class="mt50 ar">
                Next: <a href="/help/dashboard">Tradelr Dashboard</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Help - eBay Integration
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
