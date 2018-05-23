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
                    Storefront</h1>
                <p>
                    Your online storefront is fully customizable. It can be customized to look exactly
                    how you want it to look. You can customize your store using the following methods:
                </p>
                <ol>
                    <li>By adjusting theme settings</li>
                    <li>By editing the liquid templates</li>
                </ol>
                <h2>
                    Liquid Templates</h2>
                <p>
                    Tradelr uses the Liquid Templating Language to create beautiful themes with simple
                    markup.</p>
                <p>
                    A liquid template is a file which has the “.liquid” extension. Liquid files are
                    simply HTML files with embedded code. Since Liquid allows full customization of
                    your HTML, you can literally design a shop to look like anything.</p>
                <h2>
                    Who else uses Liquid Templates?</h2>
                <p>
                    The following are some of the current sites that uses the Liquid Templating system:
                    Shopify, Mephisto, Chameleon, Cashboard, Edicy, Workory, Zendesk, SandwichBoard,
                    YikeSite (CMS), Simplicant (Applicant Tracking System), 3scale (API Management System),
                    Chaptercore, ScreenSteps Live, PokerAffiliateSolutions, Assistly, Ronin, CrowdVine,
                    AboutOne, RightScale, Menumill, Moxie Software, Rusic, Development Seed, peerTransfer,
                    NationBuilder, Vendder, Storenvy.
                </p>
                <h2>
                    Anatomy of a Theme</h2>
                <p>
                    Each Tradelr theme comprises of a simple directory structure. Each theme contains
                    the following folders:</p>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                assets
                            </td>
                            <td>
                                contains javascript (.js files), stylesheets (.css files), image files and other
                                files that you want to use in your theme
                            </td>
                        </tr>
                        <tr>
                            <td>
                                config
                            </td>
                            <td>
                                contains 2 files, settings.html and settings_data.json. These are used to allow
                                users to quickly customise various settings in your theme. For example, the type
                                of fonts, colour scheme that a user might like to fine tune
                            </td>
                        </tr>
                        <tr>
                            <td>
                                layout
                            </td>
                            <td>
                                contains theme.liquid which is the master page that will contain all your theme
                                templates
                            </td>
                        </tr>
                        <tr>
                            <td>
                                snippets
                            </td>
                            <td>
                                contains views that can be used by other templates
                            </td>
                        </tr>
                        <tr>
                            <td>
                                templates
                            </td>
                            <td>
                                contains templates required by each theme
                                <ul>
                                <li><span class="code_span inline-block w150px">index.liquid</span>  Displays the main index page of your shop.</li>
                                <li><span class="code_span inline-block w150px">product.liquid</span>  Each product will use this template to display itself.</li>
                                <li><span class="code_span inline-block w150px">cart.liquid</span>  Displays the current user’s shopping cart.</li>
                                <li><span class="code_span inline-block w150px">collection.liquid</span>  Displays product collections.</li>
                                <li><span class="code_span inline-block w150px">page.liquid</span>  Displays static pages created for the store.</li>
                                <li><span class="code_span inline-block w150px">article.liquid</span>  Displays blog articles and may include a form for comments.</li>
                                <li><span class="code_span inline-block w150px">404.liquid</span>  Displays when a page is not found</li>
                                <li><span class="code_span inline-block w150px">search.liquid</span>  Displays product search results.</li>
                                </ul>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div class="mt50 ar">
                Next: <a href="/help/storefront/liquid">Using the Liquid Templating Language</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Help - Storefront
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
