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
                    Reference</h1>
                    <a name="imagesize"></a>
                <h2>
                    Image Sizes</h2>
                <p>
                    The following image sizes are supported. The image size can be specified when used
                    in conjunction with the <span class="code_span">product_img_url</span> filter.</p>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                16x16
                            </td>
                            <td>
                                pico
                            </td>
                        </tr>
                        <tr>
                            <td>
                                32x32
                            </td>
                            <td>
                                icon
                            </td>
                        </tr>
                        <tr>
                            <td>
                                50x50
                            </td>
                            <td>
                                thumb
                            </td>
                        </tr>
                        <tr>
                            <td>
                                100x100
                            </td>
                            <td>
                                small
                            </td>
                        </tr>
                        <tr>
                            <td>
                                160x160
                            </td>
                            <td>
                                compact
                            </td>
                        </tr>
                        <tr>
                            <td>
                                240x240
                            </td>
                            <td>
                                medium
                            </td>
                        </tr>
                        <tr>
                            <td>
                                480x480
                            </td>
                            <td>
                                large
                            </td>
                        </tr>
                        <tr>
                            <td>
                                600x600
                            </td>
                            <td>
                                grande
                            </td>
                        </tr>
                        <tr>
                            <td>
                                1024x1024
                            </td>
                            <td>
                                original
                            </td>
                        </tr>
                    </tbody>
                </table>
                <a name="operators"></a>
                <h2>Operators</h2>
                <p></p>
                <table>
                <tbody>
                <tr><td>==</td><td>equal</td></tr>
                <tr><td>!=</td><td>not equal</td></tr>
                <tr><td>></td><td>bigger than</td></tr>
                <tr><td><</td><td>less than</td></tr>
                <tr><td>&gt;=</td><td>bigger or equal</td></tr>
                <tr><td>&lt;=</td><td>less or equal</td></tr>
                <tr><td>or</td><td>this or that</td></tr>
                <tr><td>and</td><td>must be this and that</td></tr>
                </tbody>
                </table>
                <a name="filters"></a>
                <h2>
                    Filters</h2>
                <p>
                    The following lists supported output filters</p>
                <table id="filter_table">
                    <thead>
                        <tr>
                            <td>
                                Filter
                            </td>
                            <td>
                                Description
                            </td>
                            <td>
                                Usage
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                append
                            </td>
                            <td>
                                appends characters to a string
                            </td>
                            <td>
                                {{ 'sales' | append: '.jpg' }} <p><span class="code_result">sales.jpg</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                asset_url
                            </td>
                            <td>
                                gives you the url for an asset
                            </td>
                            <td>
                                {{ 'screen.css' | asset_url }} <p><span class="code_result">/Content/store/398/assets/screen.css</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                camelize
                            </td>
                            <td>
                                converts text into CamelCase
                            </td>
                            <td>
                                {{ 'camel case' | camelize }} <p><span class="code_result">CamelCase</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                capitalize
                            </td>
                            <td>
                                capitalises the first character
                            </td>
                            <td>
                                {{ 'capitalize me' | capitalize }} <p><span class="code_result">Capitalize Me</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                date
                            </td>
                            <td>
                                Reformats a date
                            </td>
                            <td>
                                <ul>
                                    <li>%a - The abbreviated weekday name (``Sun'')</li>
                                    <li>%A - The full weekday name (``Sunday'')</li>
                                    <li>%b - The abbreviated month name (``Jan'')</li>
                                    <li>%B - The full month name (``January'')</li>
                                    <li>%c - The preferred local date and time representation</li>
                                    <li>%d - Day of the month (01..31)</li>
                                    <li>%H - Hour of the day,24-hour clock (00..23)</li>
                                    <li>%I - Hour of the day,12-hour clock (01..12)</li>
                                    <li>%j - Day of the year (001..366)</li>
                                    <li>%m - Month of the year (01..12)</li>
                                    <li>%M - Minute of the hour (00..59)</li>
                                    <li>%p - Meridian indicator (``AM'' or ``PM'')</li>
                                    <li>%S - Second of the minute (00..60)</li>
                                    <li>%U - Week number of the current year, starting with the first Sunday as the first
                                        day of the first week (00..53)</li>
                                    <li>%W - Week number of the current year, starting with the first Monday as the first
                                        day of the first week (00..53)</li>
                                    <li>%w - Day of the week (Sunday is 0,0..6)</li>
                                    <li>%x - Preferred representation for the date alone,no time</li>
                                    <li>%X - Preferred representation for the time alone,no date</li>
                                    <li>%y - Year without a century (00..99)</li>
                                    <li>%Y - Year with century</li>
                                    <li>%Z - Time zone name</li>
                                    <li>%% - Literal ``%'' character</li>
                                </ul>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                default_pagination
                            </td>
                            <td>
                                used with the {% paginate %} tag to generate pagination links
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                downcase
                            </td>
                            <td>
                                converts all characters into lowercase
                            </td>
                            <td>
                                {{ 'SALES' | downcase }} <p><span class="code_result">sales</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                escape
                            </td>
                            <td>
                                escapes a string
                            </td>
                            <td>
                                {{ 'you & me > them' | escape }} <p><span class="code_result">you &amp; me &gt; them</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                first
                            </td>
                            <td>
                                gets the first item of an array
                            </td>
                            <td>
                                {{ product.images | first }}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                handleize
                            </td>
                            <td>
                                Strips and converts special characters out of the string, so you can use the text
                                in a url
                            </td>
                            <td>
                                {{ '100% M&Ms!!!' | handleize }} <p><span class="code_result">100-m-ms</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                highlight
                            </td>
                            <td>
                                adds the css class "highlight" to a specified html output
                            </td>
                            <td>
                                {{ product.description | highlight: 'ipod' }}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                img_tag
                            </td>
                            <td>
                                creates an img tag
                            </td>
                            <td>
                                {{ 'image-name.gif' | img_tag }} <p><span class="code_result">&lt;img src='http://clearpixels.tradelr.com/Uploads/images/3456/image-name.jpg'
                                alt='' /&gt;</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                join
                            </td>
                            <td>
                                joins an array of strings into a string separated with specified separator
                            </td>
                            <td>
                                {{ product.tags | join: ', ' }} <p><span class="code_result">tag1, tag2, tag3</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                json
                            </td>
                            <td>
                                serializes a liquid variable into a JSON string
                            </td>
                            <td>
                                {{ product | json }}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                last
                            </td>
                            <td>
                                gets the last element of an array
                            </td>
                            <td>
                                {{ product.images | first }}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                link_to
                            </td>
                            <td>
                                creates a HTML link
                            </td>
                            <td>
                                {{ 'Click' | link_to: 'http://www.tradelr.com' }} <p><span class="code_result">&lt;a href="http://www.tradelr.com"
                                &gt;Click&lt;/a&gt;</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                link_to_tag
                            </td>
                            <td>
                                creates a HTML link to the tag
                            </td>
                            <td>
                                {{ tag | link_to_tag: tag }}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                minus
                            </td>
                            <td>
                                subtracts from the passed in value
                            </td>
                            <td>
                                {{ product.price | minus: 10 }}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                money
                            </td>
                            <td>
                                formats a number value into the appropriate currency format and symbol
                            </td>
                            <td>
                                {{ product.price | money }} <p><span class="code_result">$10.00</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                money_with_currency
                            </td>
                            <td>
                                formats a number value into the appropriate currency fomat, code and symbol
                            </td>
                            <td>
                                {{ product.price | money_with_currency }} <p><span class="code_result">$10.00 USD</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                money_without_currency
                            </td>
                            <td>
                                formats a number value into the appropriate currency format
                            </td>
                            <td>
                                {{ product.price | money_without_currency }} <p><span class="code_result">10.00</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                newline_to_br
                            </td>
                            <td>
                                replaces any newline characters with &lt;br/&gt;
                            </td>
                            <td>
                                {{ '11 John St\nRiccarton 8011' | newline_to_br }} <p><span class="code_result">11 John St&lt;br/&gt;Riccarton
                                8011</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                paragraphs
                            </td>
                            <td>
                                wraps text between newlines with &lt;p&gt;&lt;/p&gt;
                            </td>
                            <td>
                                {{ '\n11 John St\nRiccarton 8011' | paragraphs }} <p><span class="code_result">&lt;p&gt;11 John St&lt;/p&gt;Riccarton</span></p>
                                8011
                            </td>
                        </tr>
                        <tr>
                            <td>
                                pluralize
                            </td>
                            <td>
                                specifies the plural version if required
                            </td>
                            <td>
                                {{ cart.item_count | pluralize: 'item', 'items' }} <p><span class="code_result italic">"if cart.item_count is 1 it will
                                use item, otherwise items will be displayed"</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                plus
                            </td>
                            <td>
                                adds to the passed in value
                            </td>
                            <td>
                                Showing {{ paginate.current_offset }}-{{ paginate.current_offset | plus: paginate.page_size
                                }} items
                            </td>
                        </tr>
                        <tr>
                            <td>
                                prepend
                            </td>
                            <td>
                                prepends characters to a string
                            </td>
                            <td>
                                {{ 'sales' | prepend: 'no ' }} <p><span class="code_result">no sales</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                product_img_url
                            </td>
                            <td>
                                returns the product image url. Image size can be specified (optional)
                            </td>
                            <td>
                                {{ product.featured_image | product_img_url: 'thumb' }} <p><span class="code_result">/Uploads/1243545/car_thumb.jpg</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                remove
                            </td>
                            <td>
                                removes the occurance of a string from the input
                            </td>
                            <td>
                                {{ product.description | remove: 'way too expensive'}}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                remove_first
                            </td>
                            <td>
                                removes the first occurance of a string from the input
                            </td>
                            <td>
                                {{ product.description | remove: 'new'}}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                replace
                            </td>
                            <td>
                                replaces occurance of a string with another
                            </td>
                            <td>
                                {{ product.description | replace: 'expensive', 'cheap'}}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                replace_first
                            </td>
                            <td>
                                replaces first occurance of a string with another
                            </td>
                            <td>
                                {{ product.description | replace: 'expensive', 'cheap'}}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                script_tag
                            </td>
                            <td>
                                creates a script tag
                            </td>
                            <td>
                                {{ 'shop.js' | asset_url | script_tag }} <p><span class="code_result">&lt;script src="/files/assets/shop.js"
                                type="text/javascript"&gt;&lt;/script&gt;</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                size
                            </td>
                            <td>
                                returns the size of a string
                            </td>
                            <td>
                                {{ 'this is an 30 character string' | size }} <p><span class="code_result">30</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                strip_html
                            </td>
                            <td>
                                removes all HTML tags from a string
                            </td>
                            <td>
                                {{ article.content | strip_html | truncate: 20 }} <p><span class="code_result italic">"returns first 20 characters of
                                an article content"</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                strip_newlines
                            </td>
                            <td>
                                removes all newlines from a string
                            </td>
                            <td>
                                {{ article.content | strip_html | strip_newlines | truncate: 80 }}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                stylesheet_tag
                            </td>
                            <td>
                                creates a stylesheet tag
                            </td>
                            <td>
                                {{ 'shop.css' | asset_url | stylesheet_tag }} <p><span class="code_result">&lt;link href="/files/assets/shop.css"
                                rel="stylesheet" type="text/css" media="all" /&gt;</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                tradelr_asset_url
                            </td>
                            <td>
                                gives you the url of a global tradelr asset
                            </td>
                            <td>
                                {{ 'option_selection.js' | tradelr_asset_url | script_tag }} <p><span class="code_result">/Scripts/store/option_selection.js</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                truncate
                            </td>
                            <td>
                                truncate a string down to x characters
                            </td>
                            <td>
                                {{ 'shop' | truncate: 3 }} <p><span class="code_result">sho</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                truncatewords
                            </td>
                            <td>
                                truncate a string down to x words
                            </td>
                            <td>
                                {{ 'how are you' | truncatewords: 1 }} <p><span class="code_result">how</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                upcase
                            </td>
                            <td>
                                converts all characters into lowercase
                            </td>
                            <td>
                                {{ 'sales' | downcase }} <p><span class="code_result">SALES</span></p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                within
                            </td>
                            <td>
                                creates a filtered url of a product
                            </td>
                            <td>
                                {{product.url | within: collection }}
                                <p><span class="code_result">/collections/front-page/products/2/new-shirt</span></p>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Help - Reference
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        $('tr', '#filter_table').each(function () {
            $('td:eq(2)', this).css('font-family', 'monospace');
        });
    });
</script>
</asp:Content>
