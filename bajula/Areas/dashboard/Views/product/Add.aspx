<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.products.viewmodel.ProductViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area">
        <form autocomplete="off" id="productAddForm" action="/dashboard/product/create" method="post">
        <div id="added" class="boxSuccess hidden">
            <h3>
                Your product has been saved</h3>
            <ul>
                <li>
                    <button type="button" onclick="window.location = '/dashboard/product/add';" class="small">
                        add another product</button></li>
                <li>
                    <button type="button" onclick="window.location = '/dashboard/orders/add';" class="small">
                        create purchase order</button></li>
                <li>
                    <button type="button" onclick="window.location = '/dashboard/invoices/add';" class="small">
                        create sales invoice</button></li>
                <li>
                    <button type="button" onclick="window.location = '/dashboard/inventory';" class="small">
                        view inventory</button></li>
            </ul>
            <div class="clear">
            </div>
        </div>
        <h3 id="headingAdd" class="hidden mt10 fl">
            new product</h3>
        <h3 id="headingTerms" class="hidden mt10 fl">
            edit product</h3>
        <div id="actionButtons" class="hidden mt10 fr">
            <button id="buttonDelete" type="button" class="small red">
                delete product</button>
        </div>
        <div class="clear">
        </div>
        <div id="product_tabs" class="tabs_clear">
            <ul class="hidden">
                <li><a href="#basic">basic information</a></li>
                <li><a href="#inventory">inventory</a></li>
                <li><a href="#autoposting">autopost</a></li>
                <li><a href="#digital">digital products</a></li>
                <li><a href="#shipping">shipping</a></li>
                <li><a href="#notes">notes</a></li>
            </ul>
            <div id="basic" class="hidden">
                <div class="fl mr10" style="width: 690px">
                    <div class="form_group">
                        <div class="info">
                            Required fields are marked with
                            <img src="/Content/img/required.png" alt="" />
                        </div>
                        <div>
                            <div class="form_entry">
                                <div class="form_label">
                                    <label for="title" class="required">
                                        Title</label>
                                    <div class="clear">
                                    </div>
                                    <span class="tip fl">Examples: Peanut Butter, 24" LCD Monitor </span>
                                    <div class="charsleft fr">
                                        <span id="title-charsleft"></span>
                                    </div>
                                </div>
                                <%= Html.TextBox("title", Model.product.title,
                                        new Dictionary<string, object> { { "style", "width:98%" } })%>
                            </div>
                            <div class="form_entry">
                                <div class="form_label">
                                    <label for="details">
                                        Describe your product</label>
                                </div>
                                <%= Html.TextArea("details", Model.product.details, 
                                new Dictionary<string, object>()
                                    {
                                        {"class", "tinymce"},
                                        {"style","width:670px"}
                                    })%>
                            </div>
                        </div>
                        <div class="section_header">
                            Variants</div>
                        <div class="form_group">
                            <div class="form_entry">
                                <table class="variants_table">
                                    <thead>
                                        <tr>
                                            <td class="w200px">
                                            </td>
                                            <td colspan="3">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="form_label fl">
                                                    <label for="sku">
                                                        SKU (Unique ID)
                                                    </label>
                                                </div>
                                                <div class="charsleft fr pr10">
                                                    <span id="sku-charsleft"></span>
                                                </div>
                                            </td>
                                            <td>
                                                color
                                                <div class="charsleft fr pr30">
                                                    <span id="color-charsleft"></span>
                                                </div>
                                            </td>
                                            <td>
                                                size
                                                <div class="charsleft fr pr30">
                                                    <span id="size-charsleft"></span>
                                                </div>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <%
                                            int count = 0;
                                            foreach (var variant in Model.product.variants)
                                            {%>
                                        <tr>
                                            <td>
                                                <%= Html.TextBox("sku", variant.sku)%>
                                                <%= Html.Hidden("variantid", variant.id) %>
                                                <%= Html.Hidden("sku_old", variant.sku) %>
                                            </td>
                                            <td>
                                                <%= Html.TextBox("color", variant.color)%>
                                            </td>
                                            <td>
                                                <%= Html.TextBox("size", variant.size)%>
                                            </td>
                                            <td>
                                                <a href="#" class="<%= count++ != 0?"icon_del":"icon_add"%>"></a>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div class="section_header">
                            Grouping</div>
                        <div class="form_group">
                            <div class="fl">
                                <% if (!Model.editMode)
                                   {%>
                                <div class="form_entry">
                                    <div class="form_label">
                                        <label for="collection">
                                            Collection</label>
                                        <span class="tip">You can add to more than one collection from your <a href="/dashboard/inventory">
                                            inventory view</a></span>
                                    </div>
                                    <%=Html.DropDownList("collection", Model.collections,
                                         new Dictionary<string, object> { { "class", "w300px" } })%>
                                </div>
                                <% }
                                   else
                                   { %>
                                <p class="icon_info mt20 smaller">
                                    Product collections can be edited from your <a href="/dashboard/inventory">inventory view</a></p>
                                <% } %>
                            </div>
                            <div class="fl ml50">
                                <div class="form_entry">
                                    <div class="form_label">
                                        <label for="maincategory">
                                            Product Category</label>
                                        <span class="tip_inline">(internal use)</span> <span class="tip">Main Category</span>
                                    </div>
                                    <%= Html.DropDownList("maincategory", Model.mainCategoryList)%>
                                </div>
                                <div class="form_entry">
                                    <div class="form_label">
                                        <span class="tip">Sub Category</span>
                                    </div>
                                    <%= Html.DropDownList("subcategory", Model.subCategoryList)%>
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="form_entry">
                                <div class="form_label">
                                    <label for="tags">
                                        Tags</label>
                                    <span class="tip">product keywords</span>
                                </div>
                                <div id="tags_original">
                                    <input type="text" id="tags" name="tags" value="<%= Model.product.tags %>" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="section_header">
                        Pricing Information</div>
                    <div class="form_group">
                        <div>
                            <div class="fl w200px">
                                <div class="form_entry">
                                    <div class="form_label">
                                        <label for="sellingPrice">
                                            Selling Price (<%= Model.product.currency.symbol %>)</label>
                                        <span class="tip">what is shown to others</span>
                                    </div>
                                    <%= Html.TextBox("sellingPrice", Model.product.sellingPrice)%>
                                </div>
                            </div>
                            <div class="fl w200px">
                                <div class="form_entry">
                                    <div class="form_label">
                                        <label for="taxrate">
                                            Special Price (<%= Model.product.currency.symbol%>)</label>
                                        <span class="tip">offer / sale / discount price</span>
                                    </div>
                                    <%= Html.TextBox("specialPrice", Model.product.specialPrice)%>
                                </div>
                            </div>
                            <div class="fl">
                                <div class="form_entry">
                                    <div class="form_label">
                                        <label for="taxrate">
                                            Tax / GST / VAT (%)</label>
                                        <span id="setTaxRate" class="tip_inline">(<a href="javascript:return false;">set default
                                            tax rate</a>)</span> <span class="tip">if specified, selling price will include tax</span>
                                    </div>
                                    <%= Html.TextBox("taxrate", Model.product.tax)%>
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div>
                            <div class="fl w200px">
                                <div class="form_entry">
                                    <div class="form_label">
                                        <label for="costPrice">
                                            Cost Price (<%= Model.product.currency.symbol%>)</label>
                                        <span class="tip">only you can see this</span>
                                    </div>
                                    <%= Html.TextBox("costPrice", Model.product.supplierPrice)%>
                                </div>
                            </div>
                            <div class="fl w200px">
                                <div class="form_entry">
                                    <div class="form_label">
                                        <label for="stockUnit">
                                            Item Unit</label><span class="tip">Examples:1kg box, 1x6 bottles</span>
                                    </div>
                                    <%= Html.DropDownList("stockUnit", Model.stockUnitList)%>
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                    </div>
                </div>
                <div id="product_images" class="images_column">
                    <div class="section_header">
                        Product Images and Videos</div>
                    <div class="results">
                        <%
                            if (Model.product.productPhotos.Count != 0)
                            {
                                foreach (var photo in Model.product.productPhotos)
                                {%>
                        <div class='thumbnail'>
                            <img src='<%=photo.url%>' alt='<%=photo.id%>' class="hidden" />
                            <div class='del'>
                                <span>delete</span></div>
                            <% if (!string.IsNullOrEmpty(photo.externalid))
                               { %>
                            <div class='view'>
                                <a target="_blank" href='http://www.youtube.com/watch?v=<%= photo.externalid %>'>view
                                    video</a></div>
                            <%} %>
                        </div>
                        <%
                                }%>
                        <div class="nophoto hidden">
                            no product images or videos added yet</div>
                        <%}
                            else
                            {%>
                        <div class="nophoto">
                            no product images or videos added yet</div>
                        <%} %>
                    </div>
                    <div class="swfu_container">
                        <div class="swfu_button">
                        </div>
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
            <div id="inventory" class="hidden">
                <div class="form_group">
                    <div class="form_entry mb20">
                        <div class="form_label">
                            <label for="trackInventory">
                                Inventory Tracking</label>
                        </div>
                        <%= Html.DropDownList("trackInventory", Model.trackInventoryList, new Dictionary<string, object>(){{"class","w400px"}})%>
                    </div>
                    <div class="fl w350px">
                        <% foreach (var location in Model.product.inventoryLocations)
                           {%>
                        <div class="inventoryLocation">
                            <% Html.RenderPartial("~/Areas/dashboard/Views/inventory/inventoryInfo.ascx", location); %>
                        </div>
                        <%} %>
                    </div>
                    <div class="fr w550px relative">
                        <div id="inventory_history">
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                </div>
            </div>
            <div id="autoposting" class="hidden">
                    <div class="info">
                        Select additional networks to list product. Don't select any if you want your products
                        to be private. <a href="/dashboard/networks">Configure network permissions</a></div>
                    <div id="autopost_networks">
                        <ul>
                            <li><a href="#autopost_blogger">
                                <%= Html.CheckBox("toBlogger", Model.isPostToBlogger)%>
                                <label>
                                    <img src="/Content/img/social/icons/blogger_16.png" alt="blogger" />
                                    Blogger
                                </label>
                            </a></li>
                            <li><a href="#autopost_ebay">
                                <%= Html.CheckBox("toEbay", Model.isPostToEbay)%>
                                <label>
                                    <img src="/Content/img/social/icons/ebay_16.png" alt="ebay" />
                                    eBay
                                </label>
                            </a></li>
                            <li><a href="#autopost_facebook">
                                <%= Html.CheckBox("toFB", Model.isPostToFacebook)%>
                                <label for="toFB">
                                    <img src="/Content/img/social/icons/facebook_16.png" alt="facebook" />
                                    Facebook
                                </label>
                            </a></li>
                            <li><a href="#autopost_google">
                                <%= Html.CheckBox("toGoogle", Model.isPostToGoogle)%>
                                <label for="toGoogle">
                                    <img src="/Content/img/social/icons/google_16.png" alt="google" />
                                    Google Base
                                </label>
                            </a></li>
                            <li class="hidden"><a href="#autopost_trademe">
                                <%= Html.CheckBox("toTrademe", Model.isPostToTrademe)%>
                                <label>
                                    <img src="/Content/img/social/icons/trademe_16.png" alt="trademe" />
                                    TradeMe
                                </label>
                            </a></li>
                            <li><a href="#autopost_tumblr">
                                <%= Html.CheckBox("toTumblr", Model.isPostToTumblr)%>
                                <label for="toTumblr">
                                    <img src="/Content/img/social/icons/tumblr_16.png" alt="tumblr" />
                                    Tumblr
                                </label>
                            </a></li>
                            <li><a href="#autopost_wordpress">
                                <%= Html.CheckBox("toWordpress", Model.isPostToWordpress)%>
                                <label for="toWordpress">
                                    <img src="/Content/img/social/icons/wordpress_16.png" alt="wordpress" />
                                    Wordpress
                                </label>
                            </a></li>
                        </ul>
                        <div id="autopost_blogger">
                            <p class="icon_help">
                                Product will be posted to your Blogger account. A Blogger account will be required.
                            </p>
                            <p>
                                <a target="_blank" href="http://www.blogger.com">Register a free account here.</a></p>
                        </div>
                        <div id="autopost_ebay">
                            <% if (Model.isEbaySynced && Model.hasPaypalAccount)
                               {
                                   Html.RenderAction("ProductSettings", "ebay", new { id = Model.product.id });
                               }
                               else
                               {
                                   if (!Model.hasPaypalAccount)
                                   { %>
                             <p class="icon_help">
                                 Paypal must be specified as a payment method. <a href="/dashboard/account#account_payment">Add payment methods here.</a>
                             </p>
                             <% }
                                   else
                                   { %>
                             <p class="icon_help">
                                Product will be posted to eBay. An eBay account is required. 
                            </p>
                            <p>
                                <a target="_blank" href="http://www.ebay.com">Register a free eBay account here.</a> If you already have an account, you will need to <a target="_blank" href="/dashboard/networks#ebay">link your eBay account here.</a></p>
                             <% }
                               } %>
                        </div>
                        <div id="autopost_facebook">
                            <% if (Model.facebookStreams.Count != 0)
                               {%>
                            <ul class="list_fl">
                                <li>
                                    <div>
                                        <h4 class="smaller">
                                            share product link on:</h4>
                                        <%= Html.CheckBoxList("fbstreams", Model.facebookStreams) %>
                                    </div>
                                </li>
                                <li>
                                    <div class="ml40">
                                        <h4 class="smaller">
                                            create photo album on:</h4>
                                        <%= Html.CheckBoxList("fbalbums", Model.facebookAlbums) %>
                                    </div>
                                </li>
                            </ul>
                            <%}
                               else
                               {%>
                            <p class="icon_help">
                                Product will be posted to your Facebook stream. A Facebook account will be required.
                                <a target="_blank" href="http://www.facebook.com">Register a free account here.</a>
                            </p>
                            <%} %>
                        </div>
                        <div id="autopost_google">
                            <p class="icon_help">
                                Product will be posted to Google Merchant Center.
                            </p>
                            <p>
                                <a href="http://www.google.com/base/">Register your own account here.</a> If you
                                have an existing account, <a target="_blank" href="/dashboard/networks#gbase">sync with
                                    your account first.</a></p>
                        </div>
                        <div id="autopost_trademe" class="hidden">
                            <% if (Model.isTrademeSynced)
                               {
                                   Html.RenderAction("ProductSettings", "trademe", new { id = Model.product.id });
                               }
                               else
                               {%>
                               <p class="icon_help">
                                Product will be listed on TradeMe New Zealand.
                            </p>
                            <p>
                                <a href="http://www.trademe.co.nz/">Register your own account here.</a> If you
                                have an existing account, <a target="_blank" href="/dashboard/networks#trademe">sync with
                                    your account first.</a></p>
                               <% }%>
                        </div>
                        <div id="autopost_tumblr">
                            <p class="icon_help">
                                Product will be posted to your Tumblr blog. A Tumblr account will be required.
                            </p>
                            <p>
                                <a target="_blank" href="http://www.tumblr.com">Register a free account here.</a></p>
                        </div>
                        <div id="autopost_wordpress">
                            <p class="icon_help">
                                Product will be posted to your Wordpress blog.
                            </p>
                            <p>
                                <a target="_blank" href="http://www.wordpress.com">Register a free account on wordpress.com.</a></p>
                        </div>
                    </div>
            </div>
            <div id="digital" class="hidden">
                <div class="info">
                    <div>
                        For selling digital products, upload a digital file. This can include videos, ebooks,
                        software, music, art etc.</div>
                    <div>
                        Shipping costs will be ignored for digital products.</div>
                </div>
                <div class="form_group">
                    <div class="fl">
                        <div class="form_entry">
                            <div id="buttonUploadDigital">
                            </div>
                            <%= Html.Hidden("digital_id", Model.product.digital.id) %>
                        </div>
                    </div>
                    <div class="fl">
                        <div id="DigitalFileSection" class="form_entry hidden">
                            <div class="form_label">
                                <label>
                                    Uploaded File <a href="#" id="digital_delete" class="icon_del notbold smaller">delete</a></label>
                            </div>
                            <div id="uploadedFile">
                                <a target="_blank" href="<%= Model.product.digital.url %>">
                                    <%= Model.product.digital.name %></a></div>
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                </div>
                <div id="DigitalUploadSection" class="hidden">
                    <div class="section_header">
                        Download Settings</div>
                    <div class="form_group">
                        <div class="fl">
                            <div class="form_entry">
                                <div class="form_label">
                                    <label for="digital_limit">
                                        Download limit</label>
                                    <span class="tip">No. of times the buyer can download this file</span>
                                </div>
                                <%= Html.TextBox("digital_limit", Model.product.digital.limit) %>
                            </div>
                            <div class="form_entry">
                                <div class="form_label">
                                    <label>
                                        Expiry Date</label>
                                    <span class="tip">When download links will expire</span>
                                </div>
                                <%= Html.TextBox("digital_expiry", Model.product.digital.expiry.HasValue ? Model.product.digital.expiry.Value.ToString(GeneralConstants.DATEFORMAT_STANDARD) : "")%>
                            </div>
                        </div>
                        <div class="fl ml50">
                            <div class="form_entry">
                                <div class="form_label">
                                    <label>
                                        Download Count</label>
                                    <span class="tip">No. of times this file has been downloaded</span>
                                </div>
                                <%= Model.product.digital.downloadCount %>
                            </div>
                        </div>
                        <div class="fl ml50">
                            <div class="form_entry">
                                <div class="form_label">
                                    <label for="digital_limit">
                                        Buttons / Buy Links</label>
                                    <span class="tip">Copy & paste these onto any website, blog, or social networking site</span>
                                </div>
                                <ul id="digital_examples">
                                    <li>
                                        <p id="buy_link_example" class="inline-block w100px"></p>
                                        <input readonly="readonly" type="text" value="" id="buy_link_code" /></li>
                                    <li>
                                        <p id="buy_button_example" class="inline-block w100px">
                                            </p>
                                        <input readonly="readonly" type="text" value="" id="buy_button_code" /></li>
                                    <li>
                                        <p id="buy_url_example" class="inline-block w100px">
                                            Direct URL</p>
                                        <input readonly="readonly" type="text" value="" id="buy_url_code" /></li>
                                </ul>
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                </div>
            </div>
            <div id="shipping" class="hidden">
                <% Html.RenderPartial("~/Areas/dashboard/Views/shipping/shipping.ascx", Model.shippingProfiles);%>
                <div class="clear">
                </div>
                <div class="section_header">
                    Product Dimension</div>
                <div class="form_group">
                    <span class="tip">Product dimensions are used for shipping cost calculations. <a
                        id="usemetric" href="#">Use metric system (cm, kg)</a> | <a id="useimperial" href="#">
                            Use imperial system (in, lb)</a></span>
                    <div class="fl mr10">
                        <div class="form_entry">
                            <div class="form_label">
                                <label for="length">
                                    Length, <span class="notbold distance">
                                        <%= Model.distanceUnit %></span></label>
                            </div>
                            <%= Html.TextBox("length", Model.product.dimension.length)%>
                        </div>
                        <div class="form_entry">
                            <div class="form_label">
                                <label for="height">
                                    Height, <span class="notbold distance">
                                        <%= Model.distanceUnit %></span></label>
                            </div>
                            <%= Html.TextBox("height", Model.product.dimension.height)%>
                        </div>
                        <div class="form_entry">
                            <div class="form_label">
                                <label for="width">
                                    Width, <span class="notbold distance">
                                        <%= Model.distanceUnit %></span></label>
                            </div>
                            <%= Html.TextBox("width", Model.product.dimension.width)%>
                        </div>
                    </div>
                    <div class="fl">
                        <div class="form_entry">
                            <div class="form_label">
                                <label for="weight">
                                    Weight, <span class="notbold weight">
                                        <%= Model.weightUnit %></span></label>
                            </div>
                            <%= Html.TextBox("weight", Model.product.dimension.weight)%>
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                </div>
                <% if (Model.showShipwire)
                   {
                       Html.RenderPartial("shipwireDetails", Model.product.shipwireDetails);
                   } %>
            </div>
            <div id="notes" class="hidden">
                <div class="form_group">
                    <div class="form_entry">
                        <div class="form_label">
                            <label for="notes">
                                Notes</label>
                            <span class="tip_inline">can only be seen by you and other staff members</span>
                            <div class="charsleft fr">
                                <span id="notes-charsleft"></span>
                            </div>
                        </div>
                        <%= Html.TextArea("notes", Model.product.otherNotes)%>
                    </div>
                </div>
            </div>
        </div>
        <div class="buttonRow_bottom">
            <span class="mr10">
                <button id="buttonSave" type="button" class="large green ajax">
                    <img src="/Content/img/save.png" alt='' />
                    save</button>
            </span>
        </div>
        <%= Html.Hidden("photoIDs")%>
        <%= Html.Hidden("id", Model.product.id)%>
        <%= Html.Hidden("defaultPhotoID", Model.product.mainPhoto)%>
        <%= Html.Hidden("limit", Model.product.limitHit)%>
        <%= Html.Hidden("FBAPI", GeneralConstants.FACEBOOK_API_KEY)%>
        <%= Html.Hidden("metric", Model.isMetric) %>
        </form>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<% if (!GeneralConstants.DEBUG) {%>
<script type="text/javascript" src="/Scripts/tinymce/tiny_mce.js"></script>
<% } else { %>
<script type="text/javascript" src="/Scripts/tinymce/tiny_mce_src.js"></script>
<% } %>
    <script src="/jsapi/uploader" type="text/javascript"></script>
    <script type="text/javascript">
        function digital_addButtonLinks(uniqueid) {
            var url = "http://z.tradelr.com/c/" + uniqueid;
            var link = '<a title="Buy Now" href="' + url + '">Buy Now</a>';
            var button = '<a style="padding: 7px 16px; font: 12px/28px Arial, Helvetica, sans-serif; color: #fff;' +
                                                'text-decoration: none; background: #333333; border: 0; border-radius: 16px; -moz-border-radius: 16px;' +
                                                '-webkit-border-radius: 16px;" title="Buy Now" href="' + url + '">Buy Now</a>';
            $('#buy_url_code').val(url);
            $('#buy_button_example').html(button);
            $('#buy_button_code').val(button);
            $('#buy_link_example').html(link);
            $('#buy_link_code').val(link);
        }

        function addProductInit() {
            $('#headingAdd').show();
            $('#headingTerms').hide();
            document.title = 'New Product';
        }

        function editProductInit() {
            $('.boxHelp').hide();
            $('#headingAdd').hide();
            $('#headingTerms').show();
            document.title = 'Edit Product';
            $('#productAddForm').attr('action', '/dashboard/product/update');
            $('#actionButtons').show();
            if ($('#digital_id').val() != '') {
                $('#buttonUploadDigital,#digital_delete').hide();
                $('#DigitalFileSection,#DigitalUploadSection').show();
                digital_addButtonLinks('<%= Model.product.digital.linkid %>');
            }
        }

        function getSubCategory(selectedValue, subvalue) {
            if (selectedValue == '' || parseInt(selectedValue, 10) < 0) {
                return false;
            }
            
            $('#subcategory').html('<option>Getting values ...</option>');
            
            $.ajaxswitch({
                type: "POST",
                url: "/dashboard/category/getsub/" + selectedValue,
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        data = json_data.data;
                        $('#subcategory').html('<option value="">None</option>');
                        $.each(data, function (i, val) {
                            $('#subcategory').append("<option value='" + val.id + "'>" + val.title + "</option>");
                        });
                        if (subvalue != undefined) {
                            $('#subcategory').val(subvalue);
                        }
                        $('#subcategory').appendable('/dashboard/category/addsub/' + selectedValue, 'Add Sub Category');
                    }
                }
            });
        };

        function InitialisePhotos() {
            var defaultphoto = $('#defaultPhotoID').val();
            $.each($('.thumbnail > img,.thumbnail > canvas'), function () {
                if ($(this).attr('alt') == defaultphoto) {
                    $(this).after("<div class='thm_overlay'></div>");
                }
            });

            // fadeInPhotos
            $('.thumbnail > img, .thumbnail > canvas').fadeIn();
        }

        function InitialiseVariantsTable() {
            $('.icon_add', '.variants_table').live('click', function () {
                // check for existing empty row
                var haveEmptyRow = false;
                $.each($('tr', '.variants_table'), function () {
                    if ($('#sku', this).val() == '' && $('#size', this).val() == '' && $('#color', this).val() == '') {
                        haveEmptyRow = true;
                        $.jGrowl('An empty row already exists');
                        return false;
                    }
                });

                if (haveEmptyRow) {
                    return false;
                }

                var row = "<tr><td><input type='text' value='' name='sku' id='sku' /><input type='hidden' name='variantid' id='variantid' />" +
                    "<input type='hidden' name='sku_old' id='sku_old' />" +
                        "</td><td><input type='text' value='' name='color' id='color' /></td>" +
                            "<td><input type='text' value='' name='size' id='size' /></td>" +
                                "<td><a class='icon_del' href='#'></a></td></tr>";
                $('.variants_table tbody').append(row);
                inputSelectors_init();
                return false;
            });
            $('.icon_del', '.variants_table').live('click', function () {
                var parent = $(this).parents('tr');
                var variantid = $(parent).find('#variantid').val();
                if (variantid == '') {
                    $(parent).fadeOut('fast', function () {
                        $(this).remove();
                    });
                }
                else {
                    var ok = window.confirm('Are you sure? This will delete this variant and related inventory information.');
                    if (!ok) {
                        return false;
                    }
                    $.post('/dashboard/product/variantsdelete/' + variantid, null, function (json_result) {
                        if (json_result.success) {
                            $(parent).fadeOut('fast', function () {
                                $(this).remove();
                            });
                        }
                        else {
                            $.jGrowl(json_result.message);
                        }
                    }, 'json');
                }
                return false;
            });

            // handle charsleft
            $('#sku').live('focus', function () {
                $(this).limit('32', '#sku-charsleft');
                $(this).alphanumeric();
            });

            $('#color').live('focus', function () {
                $(this).limit('20', '#color-charsleft');
            });

            $('#size').live('focus', function () {
                $(this).limit('20', '#size-charsleft');
            });
        }

        function inventoryLocationTemplate(locinfo) {
            /* only for offline access
            var temp = "<div class='inventoryLocation'>" +
            "<span class='name'>" + locinfo.name + "</span>" +
            "<div><div class='fl'><div class='form_entry'>" +
            "<div class='form_label'><label for='inStock'>Inventory Level</label>" +
            "</div><input type='text' value='" + tradelr.webdb.emptyIfNull(locinfo.inventoryLevel) + "' name='inStock' id='inStock'></div></div>" +
            "<div class='fl pl10'><div class='form_entry'><div class='form_label'><label for='reorderLevel'>" +
            "Stock Alarm Level</label>" +
            "</div><input type='text' value='" + tradelr.webdb.emptyIfNull(locinfo.alarmLevel) + "' name='reorderLevel' id='reorderLevel'></div></div>" +
            "<div class='clear'></div></div><input type='hidden' value='" + locinfo.locid + "' name='location' id='location'></div>";
            return temp;
            */
        }

        function VerifyVariantsTable() {
            $('input', '.variants_table ').removeClass('curFocus_red');

            var parsedsku = [];
            var variantcombo = [];
            var haveError = false;

            var rows = $('tbody > tr', '.variants_table');
            for (var i = 0; i < rows.length; i++) {
                var idx = i;
                var sku = $.trim($('#sku', rows[idx]).val());
                var color = $.trim($('#color', rows[idx]).val());
                var size = $.trim($('#size', rows[idx]).val());

                // check for empty skus
                if (sku == '' && (color != '' || size != '')) {
                    haveError = true;
                    $.jGrowl('SKU not specified');
                    $('#sku', rows[idx]).addClass('curFocus_red').one('click', function () {
                        $(this).removeClass('curFocus_red');
                    }).focus();
                    break;
                }

                if (sku != '') {
                    if ($.inArray(sku, parsedsku) != -1) {
                        haveError = true;
                        $.jGrowl('Duplicate SKU ');
                        $('#sku', rows[idx]).addClass('curFocus_red').one('click', function() {
                            $(this).removeClass('curFocus_red');
                        }).focus();
                        break;
                    }
                    parsedsku.push(sku);
                }

                // check variant values
                var entry = {
                    color: color,
                    size: size
                };
                var existIndex = variantcombo.search(entry, function (a, b) { return a.color == b.color && a.size == b.size; });
                if (existIndex != -1) {
                    haveError = true;
                    $.jGrowl('Similar product variant');
                    var markarray = [$('#color', rows[idx]), $('#color', rows[existIndex]), $('#size', rows[idx]), $('#size', rows[existIndex])];
                    $.each(markarray, function () {
                        $(this).addClass('curFocus_red').one('click', function () {
                            $(this).removeClass('curFocus_red');
                        });
                    });
                    break;
                }
                
                variantcombo.push(entry);
            }

            return !haveError;
        }

        
        function inventoryLocationUpdate() {
            // get skus
            var skus = $('#sku', '.variants_table').toArray();
            var allskus = [];
            // draw or insert if does not exist
            var locs = $('.inventoryLocation', '#inventory');
            $.each(skus, function () {
                var sku = $(this).val();
                allskus.push(sku);
                $.each(locs, function () {
                    // find entry with sku
                    var locrows = $('.content_row', this);
                    var foundEntry = null;
                    var foundEmptyEntry = null;
                    $.each(locrows, function () {
                        var inventorysku = $(this).find('.inventorySKU').text();
                        if (inventorysku == sku) {
                            foundEntry = this;
                        }
                        if (inventorysku == '') {
                            foundEmptyEntry = this;
                        }
                    });
                    if (foundEntry == null) {
                        // we need to add an entry
                        if (foundEmptyEntry != null) {
                            // just set text
                            $(foundEmptyEntry).find('.inventorySKU').text(sku);
                        }
                        else {
                            // need to insert a row
                            var emptyRow = ["<div class='content_row'>",
                                "<div class='inventorySKU' style=''>",
                                sku,
                                "</div>",
                                "<div class='fl'>",
                                "<div class='form_label'>",
                                "<label for='inStock'>",
                                "Inventory Level</label>",
                                "</div>",
                                "<input type='text' value='' name='inStock' id='inStock' class='w150px'>",
                                "</div>",
                                "<div class='fl pl10'>",
                                "<div class='form_label'>",
                                "<label for='reorderLevel'>",
                                "Stock Alarm Level</label>",
                                "</div>",
                                "<input type='text' value='' name='reorderLevel' id='reorderLevel' class='w150px'>",
                                "</div>",
                                "<div class='clear'>",
                                "</div>",
                                "</div>"];
                            $('.content_row:last', this).after(emptyRow.join(''));
                            $("input[name='inStock'],input[name='reorderLevel']").numeric();
                            inputSelectors_init();
                        }
                    }
                });
            });

            // find out which rows need deleting
            $.each(locs, function () {
                var locrows = $('.content_row', this);
                $.each(locrows, function () {
                    var inventorysku = $(this).find('.inventorySKU').text();
                    var matched = false;
                    for (var i = 0; i < allskus.length; i++) {
                        if (allskus[i] == inventorysku) {
                            matched = true;
                        }
                    }
                    if (!matched) {
                        $(this).remove();
                    }
                });
            });
        }

        function mainPageUpdate() {
            // handle moving of tag
            var copy = $('#tags_etsy').children();
            if (copy.length != 0) {
                $('#tags_original').html(copy);
            }
        }

        function networkSpecificsUpdate() {
            // handle etsy stuff
            var copy = $('#tags_original').children();
            if (copy.length != 0) {
                $('#tags_etsy').html(copy);
            }
        }

        function pageinit() {
            var submitUrl;
            if ($('#id').val() !== '') {
                editProductInit();
            }
            else {
                addProductInit();
            }

            // text editor
            tinyMCE.init({
                height: "400",
                theme: "advanced",
                mode: "specific_textareas",
                editor_selector: "tinymce",
                plugins: "fullscreen,searchreplace",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_buttons1: "search,replace,|,cut,copy,paste,|,undo,redo,|,link,unlink,charmap,emoticon,codeblock,|,bold,italic,|,numlist,bullist,formatselect,|,code,fullscreen",
                theme_advanced_buttons2: "",
                theme_advanced_buttons3: "",
                convert_urls: false,
                valid_elements: "*[*]",
                // shouldn't be needed due to the valid_elements setting, but TinyMCE would strip script.src without it.
                extended_valid_elements: "script[type|defer|src|language]"
            });

            // check if limit has been hit
            if ($('#limit').val() == 'True') {
                dialogBox_show('You have exceeded the number of unique products for your plan. Please upgrade your <a href="/dashboard/account/plan">plan</a> or delete existing products to continue.');
            }

            // main tab
            $('#product_tabs').tabs({
                select: function (event, ui) {
                    if (ui.index == 0) {
                        mainPageUpdate();
                    }
                    else if (ui.index == 1) {
                        if (!VerifyVariantsTable()) {
                            return false;
                        }
                        inventoryLocationUpdate();
                    }
                    else if (ui.index == 2) {
                        networkSpecificsUpdate();
                    }
                }
            });

            // networks tab
            $("#autopost_networks").tabs().addClass('ui-tabs-vertical ui-helper-clearfix');
            $("#autopost_networks li").removeClass('ui-corner-top').addClass('ui-corner-left');

            $('#tags').tagsInput({
                autocomplete_url: '/tags/find',
                autocomplete: {
                    autoFill: true,
                    selectFirst: false
                },
                width: '400px'
            });

            InitialisePhotos();
            InitialiseVariantsTable();

            // handle main category
            $('#maincategory').bind('change', function () {
                var selectedValue = $(this).val();
                if (selectedValue == '') {
                    $('#subcategory').html('<option value="">None</option>');
                }
                else {
                    getSubCategory(selectedValue);
                }
            });

            // handle post to blogger
            $('#toBlogger').click(function () {
                $.post('/settings/postTo', { network: "blogger" }, function () {
                    if ($('#toBlogger').is(':checked')) {
                        $('#toBlogger').attr('checked', false);
                    }
                    else {
                        $('#toBlogger').attr('checked', true);
                    }
                });
                if ($(this).is(":checked")) {
                    // check that we have permission
                    $.post('/dashboard/blogger/haveToken', null, function (json_result) {
                        if (json_result.success) {
                            if (!json_result.data) {
                                var ok = window.confirm('Permission is required to post to Blogger. Request permission?');
                                $('#toBlogger').attr('checked', false);
                                if (ok) {
                                    window.location = '/dashboard/networks#blogger';
                                }
                            }
                        }
                    }, 'json');
                }
            });

            // handle post to ebay
            $('#toEbay').click(function () {
                $.post('/settings/postTo', { network: "ebay" }, function () {
                    if ($('#toEbay').is(':checked')) {
                        $('#toEbay').attr('checked', false);
                    }
                    else {
                        $('#toEbay').attr('checked', true);
                    }
                });
            });
            
            // handle post to facebook
            $('#toFB').click(function () {
                $.post('/settings/postTo', { network: "facebook" }, function () {
                    if ($('#toFB').is(':checked')) {
                        $('#toFB').attr('checked', false);
                    }
                    else {
                        $('#toFB').attr('checked', true);
                    }
                });
                if ($(this).is(":checked")) {
                    // check that we have permission
                    $.post('/dashboard/networks/haveToken', { type: 'facebook' }, function (json_result) {
                        if (json_result.success) {
                            if (!json_result.data) {
                                var ok = window.confirm('Permission is required to post to Facebook. Request permission?');
                                $('#toFB').attr('checked', false);
                                if (ok) {
                                    window.location = '/dashboard/networks#facebook';
                                }
                            }
                        }
                    }, 'json');
                }
            });

            // handle post to google
            $('#toGoogle').click(function () {
                $.post('/settings/postTo', { network: "gbase" }, function () {
                    if ($('#toGoogle').is(':checked')) {
                        $('#toGoogle').attr('checked', false);
                    }
                    else {
                        $('#toGoogle').attr('checked', true);
                    }
                });
            });

            // handle post to trademe
            $('#toTrademe').click(function () {
                $.post('/settings/postTo', { network: "trademe" }, function () {
                    if ($('#toTrademe').is(':checked')) {
                        $('#toTrademe').attr('checked', false);
                    }
                    else {
                        $('#toTrademe').attr('checked', true);
                    }
                });
            });

            // handle post to tumblr
            $('#toTumblr').click(function () {
                $.post('/settings/postTo', { network: "tumblr" }, function () {
                    if ($('#toTumblr').is(':checked')) {
                        $('#toTumblr').attr('checked', false);
                    }
                    else {
                        $('#toTumblr').attr('checked', true);
                    }
                });
                if ($(this).is(":checked")) {
                    // check that we have permission
                    $.post('/tumblr/connected', null, function (json_result) {
                        if (json_result.success) {
                            if (!json_result.data) {
                                var ok = window.confirm('Permission is required to post to tumblr. Request permission?');
                                $('#toTumblr').attr('checked', false);
                                if (ok) {
                                    window.location = '/dashboard/networks#tumblr';
                                }
                            }
                        }
                    }, 'json');
                }
            });

            // handle post to wordpress
            $('#toWordpress').click(function () {
                $.post('/settings/postTo', { network: "wordpress" }, function () {
                    if ($('#toWordpress').is(':checked')) {
                        $('#toWordpress').attr('checked', false);
                    }
                    else {
                        $('#toWordpress').attr('checked', true);
                    }
                });
                if ($(this).is(":checked")) {
                    // check that we have permission
                    $.post('/wordpress/connected', null, function (json_result) {
                        if (json_result.success) {
                            if (!json_result.data) {
                                var ok = window.confirm('Permission is required to post to wordpress. Request permission?');
                                $('#toWordpress').attr('checked', false);
                                if (ok) {
                                    window.location = '/dashboard/networks#wordpress';
                                }
                            }
                        }
                    }, 'json');
                }
            });

            // handle facebook posts
            $("input['type=checkbox'][name=fbstreams]").click(function () {
                $.post('/settings/fbstream/' + $(this).val());
            });

            $("input['type=checkbox'][name=fbalbums]").click(function () {
                $.post('/settings/fbalbum/' + $(this).val());
            });

            $('.swfu_button_youtube').parent().click(function () {
                dialogBox_open('/video/add');
                return false;
            });

            // submit button
            $('#buttonSave').click(function () {
                $(this).buttonDisable();
                $('#productAddForm').trigger('submit');
            });

            // handle delete product
            $('#buttonDelete').click(function () {
                var ok = window.confirm("Are you sure you want to delete this product? Product images and inventory information for this product will be permanently deleted.");
                if (!ok) {
                    return false;
                }
                var productid = $('#id').val();

                $.ajaxswitch({
                    type: "POST",
                    url: "/dashboard/product/delete",
                    data: { id: productid },
                    dataType: 'json',
                    success: function (json_data) {
                        if (json_data.success) {
                            $.jGrowl('Product successfully deleted');
                            window.location = '/dashboard/inventory';
                        }
                        else {
                            if (json_data.data == tradelr.returncode.NOPERMISSION) {
                                $.jGrowl('You do not have permission to delete products');
                            }
                            else {
                                $.jGrowl(json_data.message);
                            }
                        }
                        return false;
                    }
                });
            });

            // delete thumbnail button
            $('.thumbnail .del').live('click', function () {
                var photoid = $(this).siblings('img,canvas').attr('alt');
                // if selected, clear if there's only one left
                if ($(this).parent().siblings('.thm_overlay').length <= 1) {
                    $("#defaultPhotoID").val('');
                }
                thm_delete(this, photoid, 'product');
                return false;
            });

            // new inventory location button
            $('#inventoryLocationAdd').bind('click', function () {
                dialogBox_open('/dashboard/inventory/locationAdd');
                return false;
            });

            // handle mouse overs
            $('.thumbnail').live('mouseover', function () {
                if ($(this).children('.thm_overlay').length != 0) {
                    return;
                }
                if ($(this).find('.product_setdefault').length == 0) {
                    $(this).append('<div class="product_setdefault"></div>');
                }
            });

            $('.thumbnail').live('mouseout', function (event) {
                if ($(event.relatedTarget).hasClass('product_setdefault')) {
                    return;
                }
                $(this).find('.product_setdefault').remove();
            });

            // handle set as default
            $('.thumbnail').live('click', function () {
                var current = $(this);
                // if already selected, we ignore
                if ($(this).children('.thm_overlay').length != 0) {
                    return false;
                }

                var imageid = $(this).children('img,canvas').attr('alt');
                $("#defaultPhotoID").val(imageid);

                // ignore if this is a new product
                if ($('#id').val() == '') {
                    $('.thumbnail').find('.thm_overlay').remove();
                    $(this).append('<div class="thm_overlay"></div>');
                    return false;
                }

                // otherwise clear all thumbnails and append overlay
                $.ajaxswitch({
                    type: "POST",
                    url: "/dashboard/product/update",
                    data: "id=" + $('#id').val() + "&defaultPhotoID=" + imageid,
                    dataType: 'json',
                    success: function (json_data) {
                        if (json_data.success) {
                            $('.thumbnail').find('.thm_overlay').remove();
                            $(current).append('<div class="thm_overlay"></div>');
                        }
                    }
                });
                return false;
            });

            $(window).trackUnsavedChanges('#buttonSave');

            // init appendable dropdown lists
            $('#maincategory').appendable('/dashboard/category/add', 'Add New Category');
            if ($('#maincategory').val() != '') {
                $('#subcategory').appendable('/dashboard/category/addsub/' + $('#maincategory').val(), 'Add Sub Category');
            }
            $('#stockUnit').appendable('/dashboard/stockUnit/add', 'Add New Unit');
            $('#supplier').appendableRedirect('/dashboard/contacts/add', 'Add New Supplier', false);

            // handle inventory information
            $('.hover_alarm', '#inventory').live('click', function () {
                var parent = $(this).parents('.content_row');
                var id = $.trim($('.itemid', parent).html());
                dialogBox_open('/dashboard/inventory/alarm/' + id);
            });

            $('.hover_edit', '#inventory').live('click', function () {
                var parent = $(this).parents('.content_row');
                var id = $.trim($('.itemid', parent).html());
                dialogBox_open('/dashboard/inventory/available/' + id);
            });

            $('.hover_history', '#inventory').live('click', function () {
                var parent = $(this).parents('.content_row');
                var id = $.trim($('.itemid', parent).html());
                $('#inventory_history').html('');
                $('#inventory_history').load('/dashboard/inventory/history/' + id, function () {
                    $('#inventory_history').css("marginTop", self.pageYOffset);
                });
            });

            var timer;
            // variant values check
            $('#color,#size,#sku').live('keyup', function () {
                if (timer !== undefined) {
                    clearTimeout(timer);
                }
                timer = setTimeout(function () {
                    VerifyVariantsTable();
                }, 500);
            });

            $('#setTaxRate').click(function () {
                dialogBox_open('/dashboard/product/tax', 500);
                return false;
            });

            // handle limits
            $('#title').limit('100', '#title-charsleft');
            $('#notes').limit('1000', '#notes-charsleft');
            $('#sellingPrice,#costPrice,#weight,#height,#width,#length,#taxrate,#specialPrice,#ebay_startprice,#ebay_buynowprice,#trademe_startprice,#trademe_reserveprice,#trademe_buynowprice').numeric({ allow: '.' });
            $("input[name='inStock'],input[name='reorderLevel'],#trademe_quantity,#ebay_quantity").numeric();

            // handle dimensions
            $('#usemetric').click(function () {
                if ($('#metric').val() == 'True') {
                    return false;
                }

                $('#weight').val(tradelr.util.convertweight($('#weight').val(), true));
                $('#height').val(tradelr.util.convertdistance($('#height').val(), true));
                $('#length').val(tradelr.util.convertdistance($('#length').val(), true));
                $('#width').val(tradelr.util.convertdistance($('#width').val(), true));

                $('.distance').html('cm');
                $('.weight').html('kg');
                $('#metric').val('True');
                $.post('/settings/metric/1');
                return false;
            });
            $('#useimperial').click(function () {
                if ($('#metric').val() == 'False') {
                    return false;
                }

                $('#weight').val(tradelr.util.convertweight($('#weight').val(), false));
                $('#height').val(tradelr.util.convertdistance($('#height').val(), false));
                $('#length').val(tradelr.util.convertdistance($('#length').val(), false));
                $('#width').val(tradelr.util.convertdistance($('#width').val(), false));

                $('.distance').html('in');
                $('.weight').html('lb');
                $('#metric').val('False');
                $.post('/settings/metric/0');
                return false;
            });

            //// handle digital
            
            $('input','#digital_examples').click(function() {
                $(this).select();
            });

            $("#digital_expiry").datepicker(
                {
                    dateFormat: 'D, d M yy'
                });
            $("#digital_expiry").attr('readonly', 'readonly');

            $('#digital_delete').click(function () {
                var ok = window.confirm("Are you sure? This will delete the current file and remove all download links");
                if (!ok) {
                    return false;
                }
                var digitalid = $('#digital_id').val();
                $.post('<%= Url.Action("digitaldelete","product") %>', { id: digitalid }, function (json_result) {
                    if (json_result.success) {
                        $('#digital_id').val('');
                        $('#DigitalFileSection,#DigitalUploadSection').hide();
                        $('#buttonUploadDigital').show();
                        $('#uploadedFile').html('');
                    } else {
                        $.jGrowl(json_result.message);
                    }
                });
            });

            var uploaderDigital = new qq.FileUploader({
                element: $('#buttonUploadDigital')[0],
                action: '<%= Url.Action("digitalupload","product") %>',
                params: { productid: $('#id').val() },
                onSubmit: function (id, filename) {
                },
                onComplete: function (id, filename, json_data) {
                    if (json_data.success) {
                        var file = json_data.data;
                        $('#digital_id').val(file.id);
                        $('#DigitalFileSection,#DigitalUploadSection').show();
                        $('#buttonUploadDigital').hide();
                        $('#uploadedFile').html("<a target='_blank' href='" + file.url + "'>" + file.name + "</a>");
                        digital_addButtonLinks(file.uniqueid);
                        
                    } else {
                        $.jGrowl(json_data.message);
                    }
                }
            });

            //////////////////////////// handle submit ///////////////////////////
            $.validator.addMethod("percent", function (value, element) {
                return this.optional(element) || (parseFloat(value, 10) < 100);
            }, 'must be less than 100%');

            $('#productAddForm').submit(function () {
                // validate
                var ok = $('#productAddForm').validate({
                    invalidHandler: function (form, validator) {
                        $(validator.invalidElements()[0]).focus();
                    },
                    focusInvalid: false,
                    rules: {
                        title: {
                            required: true
                        },
                        sellingPrice: {
                            number: true,
                            required: $('#toEtsy').is(":checked")
                        },
                        costPrice: {
                            number: true
                        },
                        taxrate: {
                            percent: true
                        }
                    }
                }).form();
                if (!ok || !VerifyVariantsTable()) {
                    $('#product_tabs').tabs('select', 0);
                    $.scrollTo('.variants_table', 800);
                    $('#buttonSave').buttonEnable();
                    return false;
                }

                // update inventory location
                inventoryLocationUpdate();

                // get ids from uploaded images and put it into photoIDs was comma separated string
                var photoids = [];
                var thumbs = $('.thumbnail > img, .thumbnail > canvas', '#product_images');
                $.each(thumbs, function (i, v) {
                    photoids.push($(this).attr('alt'));
                });

                $('#photoIDs').val(photoids.toString());

                // sometimes stockunit seems to be -100
                if ($('#stockUnit').val() == "-100") {
                    $('#stockUnit').val('');
                }

                // populate description
                var content = tinyMCE.get('details').getContent();
                $('#details', '#productAddForm').val(content);

                var action = $(this).attr("action");
                var serialized = $(this).serialize();

                // check prerequisites for network posts
                if ($('#toEtsy').is(":checked")) {
                    // check that there's a shipping profile
                    if ($('#shippingprofile').val() == '') {
                        $.jGrowl('A shipping profile must be selected to post to Etsy');
                        $('#product_tabs').tabs('select', 0);
                        window.scrollTo(0, $('body').height());
                        $('#buttonSave').buttonEnable();
                        return false;
                    }

                    // check that a top level category has been selected
                    var toplevelcategories = $('#etsy_toplevel').find('option[value!=""]').text();
                    if ($('.tag:first > span:first').length == 0 ||
                        toplevelcategories.indexOf($('.tag:first > span:first').text()) == -1) {
                        $.jGrowl('A top-level Etsy category must be selected to post to Etsy');
                        $('#product_tabs').tabs('select', 2);
                        $('#autopost_networks').tabs('select', '#autopost_etsy');
                        $('#buttonSave').buttonEnable();
                        return false;
                    }
                }

                // post form
                $.ajaxswitch({
                    type: "POST",
                    url: action,
                    dataType: "json",
                    data: serialized,
                    success: function (json_data) {
                        if (json_data.success) {
                            var data = json_data.data;
                            if ($('#id').val() !== '') {
                                $.jGrowl('Product successfully updated');
                            }
                            else {
                                // update order number
                                $('#id', '#productAddForm').val(data);
                                $.jGrowl('Product successfully saved');
                                $('#added').fadeIn();
                                editProductInit();
                            }
                            scrollToTop();
                        }
                        else {
                            $.jGrowl(json_data.message);
                        }
                        $('#buttonSave').buttonEnable();
                    }
                });
                return false;
            });

            inputSelectors_init();
            init_autogrow('#productAddForm');

            /*
            if (supports_file_api() && supports_canvas() && !DEBUG) {
            initHtml5Upload('photoUpload');
            }
            else {
            initAjaxUpload();
            }
            */
            initAjaxUpload();

        } // page init

        function handleFileSelect(evt) {
            var files = evt.target.files; // FileList object
            var f;
            for (var i = 0; f = files[i]; i++) {
                // Only process image files.
                if (!f.type.match('image.*')) {
                    continue;
                }
                var reader = new FileReader();

                // Closure to capture the file information.
                reader.onload = (function (theFile) {
                    return function (e) {
                        var productid = $('#id').val();
                        var url = e.target.result;
                        if (productid) {
                            var item = {
                                tablename: 'photos',
                                params: [{ url: url, contextid: productid, type: 'PRODUCT'}]
                            };
                            tradelr.webdb.sqlInsertRows([item], function (rowids) {
                                // render thumbnail
                                drawThumbnail('#product_images', url, rowids[0]);
                            });
                        }
                        else {
                            // render thumbnail
                            drawThumbnail('#product_images', url, '');
                        }
                    };
                })(f);

                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
            }
        }

        function initHtml5Upload(idname) {
            var html5button = '<input type="file" id="' + idname + '" class="opacity0" name="files[]" multiple />';
            $('.swfu_button').append(html5button);
            $('#' + idname).bind('change', handleFileSelect);
        }

        function renderThumbnail(targetid, url, imageid, youtubeid) {
            tradelr.log('render thumbnail');
            if ($('.nophoto', targetid)) {
                $('.nophoto', targetid).hide();
            }
            var thumbnail = ["<div class='thumbnail hidden'>", "<img src='", url, "' alt='", imageid, "' />", "<div class='del'><span>delete</span></div>"];
            if (youtubeid != undefined) {
                thumbnail.push("<div class='view'><a target='_blank' href='http://www.youtube.com/watch?v=" + youtubeid + "'>view video</a></div>");
            }

            thumbnail.push("</div>");

            // 3 locs to handle product, profile, company
            $(thumbnail.join('')).fadeIn().appendTo(targetid + " > .results");
        }

        function drawThumbnail(targetid, url, imageid, callback) {
            if ($('.nophoto', targetid)) {
                $('.nophoto', targetid).hide();
            }

            tradelr.photo.resize(url, imageid, tradelr.photo.size.MEDIUM, function (canvasObj) {
                var thumbnail = $("<div class='thumbnail' style='display:none'></div>");
                $(thumbnail).append(canvasObj).append("<div class='del'><span>delete</span></div>");

                $(thumbnail).fadeIn().appendTo(targetid + " > .results");
                if (callback != undefined) {
                    callback();
                }

            });
        }

        function initAjaxUpload() {
            // upload url
            var uploadUrl;
            if ($('#id').val() !== '') {
                uploadUrl = "/photos/Upload/product/" + $('#id').val();
            }
            else {
                uploadUrl = "/photos/Upload/product";
            }

            var ajaxupload = new AjaxUpload($('.swfu_button'), {
                action: uploadUrl,
                onSubmit: function (file, ext) {
                    if (!(ext && /^(jpg|jpeg|png|gif)$/i.test(ext))) {
                        // extension is not allowed
                        $.jGrowl('Error: Unsupported file extension');
                        // cancel upload
                        return false;
                    }
                    this.disable();
                    $.prettyLoader.show();
                },
                onComplete: function (file, response) {
                    // enable upload button
                    this.enable();
                    $.prettyLoader.hide();
                    var info = response.split(',');
                    var imageid = info[0];
                    var targetid = info[1];
                    var url = info[2];

                    renderThumbnail(targetid, url, imageid);
                }
            });
        }


        $(document).ready(function () {
            $('#navinventory').addClass('navselected_white');
            /*
            if (tradelr.webdb.db != null) {
            if (window.location.pathname.indexOf('edit') != -1) {
            var pageid = parseInt(getPageID(), 10);
            if (DEBUG) {
            tradelr.log('offline edit ' + pageid);
            }
            $('#id').val(pageid);
            tradelr.webdb.sqlGetRows("SELECT DISTINCT p.*, locs.name AS locsname, c.id AS categoryid,pc.id AS parentid, ph.id AS imageid, ph.url AS imageurl, locs.id AS locationid, ilocitem.inventoryLevel, ilocitem.alarmLevel, iloc.name AS locname" +
            " FROM inventoryloc AS locs, products AS p" +
            " LEFT OUTER JOIN category AS c ON c.id = p.categoryid AND c.cflag !=" + tradelr.webdb.flags.DELETE +
            " LEFT OUTER JOIN category AS pc ON c.parentid = pc.id" +
            " LEFT OUTER JOIN stockunit AS su ON p.stockunitid = su.id" +
            " LEFT OUTER JOIN photos AS ph ON ph.contextid = p.id AND ph.type = 'PRODUCT' AND ph.cflag !=" + tradelr.webdb.flags.DELETE +
            " LEFT OUTER JOIN inventorylocitem AS ilocitem ON ilocitem.productid = p.id" +
            " LEFT OUTER JOIN inventoryloc AS iloc ON iloc.id = ilocitem.locationid" +
            " WHERE p.id = ?",
            [pageid], pageid, function (result) {
            if (result != null) {
            var row = result.item(0);

            // fill em up
            $('#title').val(row['title']);
            $('#sku').val(row['SKU']);
            $('#details').val(row['details']);

            // category
            tradelr.webdb.sqlGetRows("SELECT * FROM category WHERE parentid IS NULL", [], null, function (rows) {
            $('#maincategory').html('<option value="">None</option>');

            for (var i = 0; i < rows.length; i++) {
            var crow = rows.item(i);
            $('#maincategory').append("<option value='" + crow['id'] + "'>" + crow['name'] + "</option>");
            }

            if (row['parentid'] != null) {
            $('#maincategory').val(row['parentid']);
            getSubCategory(row['parentid'], row['categoryid']);
            }
            else if (row['categoryid'] != null) {
            $('#maincategory').val(row['categoryid']);
            getSubCategory(row['categoryid']);
            }
            $('#maincategory').appendable('/dashboard/category/add', 'Add New Category');
            });

            // stockunit
            if (row['stockunitid'] != null) {
            $('#stockUnit').val(row['stockunitid']);
            }
            if (row['sellingPrice'] != null) {
            $('#sellingPrice').val(parseFloat(row['sellingPrice']).toFixed(2));
            }
            if (row['costPrice'] != null) {
            $('#costPrice').val(parseFloat(row['costPrice']).toFixed(2));
            }
            $('#notes').val(row['notes']);
            $('#sku').val(row['SKU']);
            $('#defaultPhotoID').val(row['thumbnailid']);

            // need to handle inventory location
            // need to handle images
            var images = [];
            var locs = []; // inventory location that has something
            var locholders = [];  // all known inventory locations
            for (var i = 0; i < result.length; i++) {
            var row2 = result.item(i);
            if (row2['imageid'] != null) {
            var image = {
            id: row2['imageid'],
            url: row2['imageurl']
            };
            // check that entry does not already exist
            if ($.toJSON(images).indexOf($.toJSON(image)) == -1) {
            images.push(image);
            }
            }
            if (row2['locsname'] != null) {

            var loc = {
            name: row2['locsname'],
            locid: row2['locationid'],
            inventoryLevel: null,
            alarmLevel: null
            }
            // check that entry does not already exist
            if ($.toJSON(locholders).indexOf($.toJSON(loc)) == -1) {
            locholders.push(loc);
            }
            }
            if (row2['locname'] != null) {

            var loc = {
            name: row2['locname'],
            inventoryLevel: row2['inventoryLevel'],
            alarmLevel: row2['alarmLevel']
            }
            // check that entry does not already exist
            if ($.toJSON(locs).indexOf($.toJSON(loc)) == -1) {
            locs.push(loc);
            }
            }
            }

            // init the locholders
            for (var i = 0; i < locs.length; i++) {
            for (var j = 0; j < locholders.length; j++) {
            var loc = locs[i];
            var locholder = locholders[j];
            if (locholder.name == loc.name) {
            locholder.inventoryLevel = loc.inventoryLevel;
            locholder.alarmLevel = loc.alarmLevel;
            }
            }
            }

            // sort the locholders
            locholders.sort(tradelr.inventory.location.order);

            // add loc
            var loccontainer = $('#inventoryLocationAdd').parent();
            $('.inventoryLocation').remove();
            for (var i = 0; i < locholders.length; i++) {
            var loc = locholders[i];
            $(loccontainer).prepend(inventoryLocationTemplate(loc));
            }

            // render images, needs a callback
            $('.thumbnail').remove();
            $('.nophoto', '#product_images').show();
            var imgcount = images.length;
            if (imgcount == 0) {
            pageinit();
            }
            else {
            for (var i = 0; i < images.length; i++) {
            var img = images[i];
            drawThumbnail('#product_images', img.url, img.id, function () {
            if (--imgcount == 0) {
            pageinit(); // INIT PAGE
            }
            });
            }
            }
            }
            });
            }
            else {
            // add new product
            pageinit();
            }
            }
            else {
            */
            pageinit();
            //}
        });
    </script>
</asp:Content>
