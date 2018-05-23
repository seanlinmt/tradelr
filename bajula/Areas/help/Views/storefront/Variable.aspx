<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
                    Liquid Variables</h1>
                <p>
                    To access your inventory, pages, articles and other settings, each template page
                    has access to liquid variables. There are two types of variables:</p>
                <ol>
                    <li><strong>Global</strong> - accessible from all pages</li>
                    <li><strong>Page</strong> - accessible within a page</li>
                </ol>
                <p>
                    The following describes the various variables supported by Tradelr</p>
                <h2>
                    Blog Variables</h2>
                <p>
                    Blog related variables consists of the following variables</p>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                Blogs
                            </td>
                            <td>
                                <span class="help_tag">global</span>
                            </td>
                            <td>
                                Holds a list of blogs created. Individual blogs can be accessed by their handle.
                                For example, to get all the articles in the blog called news, <span class="code_span">
                                    blogs.news.articles</span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Blog
                            </td>
                            <td>
                                <span class="help_tag">page</span>
                            </td>
                            <td>
                                An individual blog
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Article
                            </td>
                            <td>
                                <span class="help_tag">page</span>
                            </td>
                            <td>
                                A blog article
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Comment
                            </td>
                            <td>
                                <span class="help_tag">page</span>
                            </td>
                            <td>
                                A comment in a blog article
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    Blog</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                id
                            </td>
                            <td>
                                the id of this blog
                            </td>
                        </tr>
                        <tr>
                            <td>
                                title
                            </td>
                            <td>
                                the title of this blog
                            </td>
                        </tr>
                        <tr>
                            <td>
                                handle
                            </td>
                            <td>
                                the handle for this blog
                            </td>
                        </tr>
                        <tr>
                            <td>
                                url
                            </td>
                            <td>
                                returns the url of the blog, For example, /blogs/news
                            </td>
                        </tr>
                        <tr>
                            <td>
                                articles
                            </td>
                            <td>
                                returns the list of articles in this blog
                            </td>
                        </tr>
                        <tr>
                            <td>
                                articles_count
                            </td>
                            <td>
                                displays the number of articles in this blog
                            </td>
                        </tr>
                        <tr>
                            <td>
                                comments_enabled
                            </td>
                            <td>
                                returns true if users can leave comments
                            </td>
                        </tr>
                        <tr>
                            <td>
                                moderated
                            </td>
                            <td>
                                returns true if user comments will be moderated
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    Article</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                id
                            </td>
                            <td>
                                the unique id for this article
                            </td>
                        </tr>
                        <tr>
                            <td>
                                title
                            </td>
                            <td>
                                the title of this article
                            </td>
                        </tr>
                        <tr>
                            <td>
                                author
                            </td>
                            <td>
                                the name of the person who published the article
                            </td>
                        </tr>
                        <tr>
                            <td>
                                content
                            </td>
                            <td>
                                the article content
                            </td>
                        </tr>
                        <tr>
                            <td>
                                created_at
                            </td>
                            <td>
                                returns the date the article was created
                            </td>
                        </tr>
                        <tr>
                            <td>
                                published_at
                            </td>
                            <td>
                                returns the date the article was published
                            </td>
                        </tr>
                        <tr>
                            <td>
                                url
                            </td>
                            <td>
                                the url of the article. For example, /blogs/news/12-new-products-launched
                            </td>
                        </tr>
                        <tr>
                            <td>
                                comments
                            </td>
                            <td>
                                returns all comments in this article. This excludes comments which have not been
                                moderated or has been marked as spam.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                comments_count
                            </td>
                            <td>
                                the number of comments
                            </td>
                        </tr>
                        <tr>
                            <td>
                                comment_post_url
                            </td>
                            <td>
                                the url a new comment will be submitted
                            </td>
                        </tr>
                        <tr>
                            <td>
                                comments_enabled
                            </td>
                            <td>
                                returns true if users can leave comments
                            </td>
                        </tr>
                        <tr>
                            <td>
                                moderated
                            </td>
                            <td>
                                returns true if user comments will be moderated
                            </td>
                        </tr>
                        <tr>
                            <td>
                                tags
                            </td>
                            <td>
                                returns a collection of tags
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    Comment</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                id
                            </td>
                            <td>
                                the unique id for this comment
                            </td>
                        </tr>
                        <tr>
                            <td>
                                author
                            </td>
                            <td>
                                the name of the person who posted the comment
                            </td>
                        </tr>
                        <tr>
                            <td>
                                email
                            </td>
                            <td>
                                the email of the person who posted the comment
                            </td>
                        </tr>
                        <tr>
                            <td>
                                content
                            </td>
                            <td>
                                the content of the comment
                            </td>
                        </tr>
                        <tr>
                            <td>
                                created_at
                            </td>
                            <td>
                                date and time the comment was created
                            </td>
                        </tr>
                        <tr>
                            <td>
                                status
                            </td>
                            <td>
                                the status of the comment
                            </td>
                        </tr>
                        <tr>
                            <td>
                                url
                            </td>
                            <td>
                                the url of the the comment. For example, /blogs/coming-soon/3456-new-product#23
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h2>
                    Form Variables</h2>
                <p>
                    Form variables are used in conjunction with the Form Tag. They are used in blog
                    articles to allow users to post comments and also in contact forms.</p>
                <h3>
                    Form</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                posted_successfully
                            </td>
                            <td>
                                true if form has been posted successfully with no errors
                            </td>
                        </tr>
                        <tr>
                            <td>
                                errors
                            </td>
                            <td>
                                this references the FormErrors variable
                            </td>
                        </tr>
                        <tr>
                            <td>
                                email
                            </td>
                            <td>
                                if form was not posted successfully, this will contain the value of the email form
                                field
                            </td>
                        </tr>
                        <tr>
                            <td>
                                body
                            </td>
                            <td>
                                if form was not posted successfully, this will contain the value of the body form
                                field
                            </td>
                        </tr>
                        <tr>
                            <td>
                                name
                            </td>
                            <td>
                                if form was not posted successfully, this will contain the value of the name form
                                field
                            </td>
                        </tr>
                        <tr>
                            <td>
                                phone
                            </td>
                            <td>
                                if form was not posted successfully, this will contain the value of the phone form
                                field
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    FormErrors</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                errorFields
                            </td>
                            <td>
                                an array which contains the name of fields that caused an error
                            </td>
                        </tr>
                        <tr>
                            <td>
                                messages
                            </td>
                            <td>
                                an associative array which contains the error message associated with the name of
                                the field that caused the error
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h2>
                    Page Variables</h2>
                <p>
                    Page variables relate to custom pages created by the store owner</p>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                pages
                            </td>
                            <td>
                                <span class="help_tag">global</span>
                            </td>
                            <td>
                                This is a collection of all pages created by the owner.A specific page can be obtained
                                via the page's handle. For example, to obtain all articles in the blog "new-products":
                                <span class="code_span">pages.new-products.articles</span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                page
                            </td>
                            <td>
                                <span class="help_tag">page</span>
                            </td>
                            <td>
                                An individual page
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    Page</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                title
                            </td>
                            <td>
                                the page title
                            </td>
                        </tr>
                        <tr>
                            <td>
                                handle
                            </td>
                            <td>
                                the handle for this page. Usually used to obtain a specific page for the Pages variable
                            </td>
                        </tr>
                        <tr>
                            <td>
                                url
                            </td>
                            <td>
                                the url for this page. For example, /pages/about-us
                            </td>
                        </tr>
                        <tr>
                            <td>
                                content
                            </td>
                            <td>
                                the page content
                            </td>
                        </tr>
                        <tr>
                            <td>
                                author
                            </td>
                            <td>
                                the name of the creator of the page
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h2>
                    Product Variables</h2>
                <p>
                    The following variables relates to products and how they are grouped</p>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                collections
                            </td>
                            <td>
                                <span class="help_tag">global</span>
                            </td>
                            <td>
                                Contains a list of known product collections. Each collection in the collections
                                variable can be accessed via the collection’s name.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                collection
                            </td>
                            <td>
                                <span class="help_tag">page</span>
                            </td>
                            <td>
                                Contains a list of products
                            </td>
                        </tr>
                        <tr>
                            <td>
                                product
                            </td>
                            <td>
                                <span class="help_tag">page</span>
                            </td>
                            <td>
                                A product which may contain a list of variants
                            </td>
                        </tr>
                        <tr>
                            <td>
                                variant
                            </td>
                            <td>
                            </td>
                            <td>
                                A product variant
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    Collection</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                title
                            </td>
                            <td>
                                the title of the collection
                            </td>
                        </tr>
                        <tr>
                            <td>
                                handle
                            </td>
                            <td>
                                the hande for the collection
                            </td>
                        </tr>
                        <tr>
                            <td>
                                description
                            </td>
                            <td>
                                the description of the collection
                            </td>
                        </tr>
                        <tr>
                            <td>
                                products
                            </td>
                            <td>
                                returns list of all products in this collection
                            </td>
                        </tr>
                        <tr>
                            <td>
                                products_count
                            </td>
                            <td>
                                returns the total number of products in the current view. This takes into accounts
                                any filtering of the collection by tags or category.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                all_products_count
                            </td>
                            <td>
                                returns the total number of products in the collection
                            </td>
                        </tr>
                        <tr>
                            <td>
                                all_tags
                            </td>
                            <td>
                                returns all tags in the collection as a comma-separated string
                            </td>
                        </tr>
                        <tr>
                            <td>
                                url
                            </td>
                            <td>
                                returns the url of the collection. For example, /collection/t-shirts
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    Product</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                id
                            </td>
                            <td>
                                the unique id for this product
                            </td>
                        </tr>
                        <tr>
                            <td>
                                title
                            </td>
                            <td>
                                the product title
                            </td>
                        </tr>
                        <tr>
                            <td>
                                category
                            </td>
                            <td>
                                the main product category
                            </td>
                        </tr>
                        <tr>
                            <td>
                                price
                            </td>
                            <td>
                                the effective selling price of the product. This returns the cheapest price if a
                                special price or group price has been specified
                            </td>
                        </tr>
                        <tr>
                            <td>
                                selling_price
                            </td>
                            <td>
                                the selling price
                            </td>
                        </tr>
                        <tr>
                            <td>
                                url
                            </td>
                            <td>
                                url of the product. For example, /products/34/hawaiian-shirt
                            </td>
                        </tr>
                        <tr>
                            <td>
                                description
                            </td>
                            <td>
                                the full description of the product
                            </td>
                        </tr>
                        <tr>
                            <td>
                                handle
                            </td>
                            <td>
                                returns the product handle. Currently, this is the product id
                            </td>
                        </tr>
                        <tr>
                            <td>
                                options
                            </td>
                            <td>
                                List of possible option types. This list will contain the value "color" and "size"
                            </td>
                        </tr>
                        <tr>
                            <td>
                                variants
                            </td>
                            <td>
                                List of all of this product's variants
                            </td>
                        </tr>
                        <tr>
                            <td>
                                available
                            </td>
                            <td>
                                returns true if there are products in stock. If inventory tracking is turned off,
                                this will always return true
                            </td>
                        </tr>
                        <tr>
                            <td>
                                tags
                            </td>
                            <td>
                                lists all product's tags
                            </td>
                        </tr>
                        <tr>
                            <td>
                                images
                            </td>
                            <td>
                                returns a list of url of all products images
                            </td>
                        </tr>
                        <tr>
                            <td>
                                default_image
                            </td>
                            <td>
                                returns the url of the default image
                            </td>
                        </tr>
                        <tr>
                            <td>
                                new
                            </td>
                            <td>
                                returns true if the product was created less than a week ago
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    Variant</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                id
                            </td>
                            <td>
                                the unique id for this variant
                            </td>
                        </tr>
                        <tr>
                            <td>
                                title
                            </td>
                            <td>
                                the variant title. For example, "large / blue"
                            </td>
                        </tr>
                        <tr>
                            <td>
                                available
                            </td>
                            <td>
                                true if this variant is available. Will also be true if inventory tracking is turned
                                off
                            </td>
                        </tr>
                        <tr>
                            <td>
                                inventory_quantity
                            </td>
                            <td>
                                the number of items available
                            </td>
                        </tr>
                        <tr>
                            <td>
                                sku
                            </td>
                            <td>
                                the stock keeping unit
                            </td>
                        </tr>
                        <tr>
                            <td>
                                price
                            </td>
                            <td>
                                the effective selling price of this variant (similar to product.price)
                            </td>
                        </tr>
                        <tr>
                            <td>
                                option1
                            </td>
                            <td>
                                the color
                            </td>
                        </tr>
                        <tr>
                            <td>
                                option2
                            </td>
                            <td>
                                the size
                            </td>
                        </tr>
                        <tr>
                            <td>
                                options
                            </td>
                            <td>
                                List of option values. If options have been specified, this will contain the values
                                of option1 and/or option2.
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h2>
                    Shopping Cart</h2>
                <p>
                    A customer's shopping cart is represented by the Cart variable which contains a
                    collection of CartItem variables.</p>
                <h3>
                    Cart <span class="help_tag">global</span></h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                item_count
                            </td>
                            <td>
                                the number of items in the cart
                            </td>
                        </tr>
                        <tr>
                            <td>
                                subtotal_price
                            </td>
                            <td>
                                total price of items in the cart before any discount
                            </td>
                        </tr>
                        <tr>
                            <td>
                                items
                            </td>
                            <td>
                                collection of CartItems in the cart
                            </td>
                        </tr>
                        <tr>
                            <td>
                                note
                            </td>
                            <td>
                                a message that a customer may leave
                            </td>
                        </tr>
                        <tr>
                            <td>
                                coupon_code
                            </td>
                            <td>
                                the coupon code that has been used
                            </td>
                        </tr>
                        <tr>
                            <td>
                                discount_amount
                            </td>
                            <td>
                                the discount amount of a valid coupon code specified by the customer
                            </td>
                        </tr>
                        <tr>
                            <td>
                                total_price
                            </td>
                            <td>
                                the total price of items in the cart after any discount. If there was no discount
                                then total_price will equal to subtotal_price.
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    CartItem</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                id
                            </td>
                            <td>
                                unique id of the cart item
                            </td>
                        </tr>
                        <tr>
                            <td>
                                title
                            </td>
                            <td>
                                full product name
                            </td>
                        </tr>
                        <tr>
                            <td>
                                product
                            </td>
                            <td>
                                the product variable this item belongs to
                            </td>
                        </tr>
                        <tr>
                            <td>
                                variant
                            </td>
                            <td>
                                the variant variable this item belongs to
                            </td>
                        </tr>
                        <tr>
                            <td>
                                quantity
                            </td>
                            <td>
                                the number of items for this current item
                            </td>
                        </tr>
                        <tr>
                            <td>
                                price
                            </td>
                            <td>
                                the unit price for this item
                            </td>
                        </tr>
                        <tr>
                            <td>
                                total_price
                            </td>
                            <td>
                                the subtotal for this entry. This would be the unit price multiplied by the quantity.
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h2>
                    Navigation Variables</h2>
                <p>
                    Navigation allows you to create custom links for navigating your online store. The
                    linklist variable which contains a bunch of such links. A linklist for the main
                    store navigation menu and one for navigation links located in the footer of your
                    store page is defined for you when you first create your account.
                </p>
                <div class="code_block">
                    &lt;ul&gt;<br />
                    <span class="tab"></span>{% for link in linklists.main-menu.links %}<br />
                    <span class="tab"></span><span class="tab"></span>&lt;li&gt;{{ link.title | link_to:
                    link.url }}&lt;/li&gt;<br />
                    <span class="tab"></span>{% endfor %}<br />
                    &lt;/ul&gt;<br />
                </div>
                <table class="mt20">
                    <tbody>
                        <tr>
                            <td>
                                linklists
                            </td>
                            <td>
                                <span class="help_tag">global</span>
                            </td>
                            <td>
                                Holds a collection of linklists that you have created. There are two default linklists
                                that cannot be removed and are created for you when your account was setuped; main-menu
                                and footer.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                linklist
                            </td>
                            <td>
                            </td>
                            <td>
                                holds a collection of links
                            </td>
                        </tr>
                        <tr>
                            <td>
                                link
                            </td>
                            <td>
                            </td>
                            <td>
                                a link
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    Linklist</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                title
                            </td>
                            <td>
                                the name of the linklist
                            </td>
                        </tr>
                        <tr>
                            <td>
                                handle
                            </td>
                            <td>
                                the handle of the linklist
                            </td>
                        </tr>
                        <tr>
                            <td>
                                links
                            </td>
                            <td>
                                a list of links in this linklist
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h3>
                    Link</h3>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                url
                            </td>
                            <td>
                                the url of the link. For example, /collections/all
                            </td>
                        </tr>
                        <tr>
                            <td>
                                title
                            </td>
                            <td>
                                the name of the link
                            </td>
                        </tr>
                        <tr>
                            <td>
                                type
                            </td>
                            <td>
                                the type of link which will be of one of the following types
                                <ul>
                                    <li><span class="w200px inline-block">blog_link </span>if the link points to a blog</li>
                                    <li><span class="w200px inline-block">collection_link </span>if the link points to a
                                        product collection</li>
                                    <li><span class="w200px inline-block">page_link </span>if the link points to a page</li>
                                    <li><span class="w200px inline-block">product_link </span>if the link points to a product</li>
                                    <li><span class="w200px inline-block">http_link </span>if the link points to an external
                                        web page</li>
                                    <li><span class="w200px inline-block">relative_link </span>other links not falling into
                                        the above link types</li>
                                </ul>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <h2>
                    Search Variable <span class="help_tag">page</span></h2>
                <p>
                    The search variable is used by the search page to return product search results</p>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                performed
                            </td>
                            <td>
                                true if a searched has been performed. This is normally used to reveal search results
                                on the search page
                            </td>
                        </tr>
                        <tr>
                            <td>
                                terms
                            </td>
                            <td>
                                the search term
                            </td>
                        </tr>
                        <tr>
                            <td>
                                results
                            </td>
                            <td>
                                a list of products returned from the search operation
                            </td>
                        </tr>
                        <tr>
                            <td>
                                results_count</td>
                            <td>
                                total number of items matching the search term</td>
                        </tr>
                    </tbody>
                </table>
                <h2>
                    Shop Variable <span class="help_tag">global</span></h2>
                <p>
                    The Shop variable contains general settings and store configuration</p>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                name
                            </td>
                            <td>
                                the name of your shop
                            </td>
                        </tr>
                        <tr>
                            <td>
                                url
                            </td>
                            <td>
                                the url of your shop. For example, http://myshop.tradelr.com
                            </td>
                        </tr>
                        <tr>
                            <td>
                                domain
                            </td>
                            <td>
                                the domain of you shop. For example, if a custom domain has been specified, it will
                                be wwww.myshop.com. Otherwise, it will be myshop.tradelr.com
                            </td>
                        </tr>
                        <tr>
                            <td>
                                permanent_domain
                            </td>
                            <td>
                                the full tradelr domain name. For example, myshop.tradelr.com
                            </td>
                        </tr>
                        <tr>
                            <td>
                                email
                            </td>
                            <td>
                                the email address of the owner of the store
                            </td>
                        </tr>
                        <tr>
                            <td>
                                fb_adminid</td>
                            <td>
                                the facebook id of the store owner</td>
                        </tr>
                        <tr>
                            <td>
                                products_count
                            </td>
                            <td>
                                the total number of active products
                            </td>
                        </tr>
                        <tr>
                            <td>
                                collections_count
                            </td>
                            <td>
                                the total number of visible product collections
                            </td>
                        </tr>
                        <tr>
                            <td>
                                currency
                            </td>
                            <td>
                                the currency code of the current store currency. For example, USD.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                currency_symbol
                            </td>
                            <td>
                                the currency symbol of the current store currency. For example, $.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                currency_decimal_places
                            </td>
                            <td>
                                the number of decimal places of the current store currency. For example, 0 for the
                                Japanese yen, 2 for US dollars.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                message
                            </td>
                            <td>
                                the store message
                            </td>
                        </tr>
                        <tr>
                            <td>
                                payment_policy
                            </td>
                            <td>
                                the store payment policy
                            </td>
                        </tr>
                        <tr>
                            <td>
                                refund_policy
                            </td>
                            <td>
                                the store refund policy
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
    Help - Liquid Variables
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
