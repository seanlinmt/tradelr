<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.account.plans.PlanViewData>" %>
<%@ Import Namespace="tradelr.Common.Constants" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<%@ Import Namespace="tradelr.Models.account" %>
<%= Html.RegisterViewJS() %>
<table id="pricing">
    <tbody>
        <tr>
            <th class="top empty">
            </th>
            <td class="top">
                Single
            </td>
            <td class="top">
                Basic
            </td>
            <td class="top">
                Pro
            </td>
            <td class="top">
                Ultimate
            </td>
        </tr>
        <% if (Model == null)
           {%>
        <tr>
            <th class="empty signupEnd">
                <div class="font_darkgrey">
                    You can upgrade, downgrade or cancel your account at anytime.
                </div>
            </th>
            <td class="empty">
                <a href="<%=GeneralConstants.HTTP_SECURE%>/register/single">
                    <img src="/Content/img/buttons/signup_small.png" alt="single" /></a>
            </td>
            <td class="empty">
                <a href="<%=GeneralConstants.HTTP_SECURE%>/register/basic">
                    <img src="/Content/img/buttons/signup_small.png" alt="basic" /></a>
            </td>
            <td class="empty">
                <a href="<%=GeneralConstants.HTTP_SECURE%>/register/pro">
                    <img src="/Content/img/buttons/signup_small.png" alt="pro" /></a>
            </td>
            <td class="empty">
                <a href="<%=GeneralConstants.HTTP_SECURE%>/register/ultimate">
                    <img src="/Content/img/buttons/signup_small.png" alt="ultimate" /></a>
            </td>
        </tr>
        <%
            }%>
        <tr>
            <th title="Price per month - How much you have to pay every month (no hidden fees).">
                Price per month ($US)
            </th>
            <td>
                <span class="price">$9</span><span class="month"> /month</span>
            </td>
            <td>
                <span class="price">$19</span><span class="month"> /month</span>
            </td>
            <td>
                <span class="price">$49</span><span class="month"> /month</span>
            </td>
            <td>
                <span class="price">$99</span><span class="month"> /month</span>
            </td>
        </tr>
        <tr>
            <th title="Number of SKUs - The total number of unique products you can have.">
                Number of SKUs
            </th>
            <td>
                25
            </td>
            <td>
                100
            </td>
            <td>
                500
            </td>
            <td>
                Unlimited
            </td>
        </tr>
        <tr>
            <th title="Invoices per month - Number of invoices you can send out every month.">
                Invoices per month
            </th>
            <td>
                100
            </td>
            <td>
                500
            </td>
            <td>
                1000
            </td>
            <td>
                Unlimited
            </td>
        </tr>
        <tr>
            <th title="Inventory location - The maximum number of inventory locations that you can specify">
                Inventory Locations
            </th>
            <td>
                5
            </td>
            <td>
                10
            </td>
            <td>
                50
            </td>
            <td>
                Unlimited
            </td>
        </tr>
        <tr>
            <th title="Bandwidth - How much internet traffic you can use per month.">
                Bandwidth
            </th>
            <td>
                Unlimited
            </td>
            <td>
                Unlimited
            </td>
            <td>
                Unlimited
            </td>
            <td>
                Unlimited
            </td>
        </tr>
        <tr>
            <th title="Disk space - How much disk space you are allocated for uploaded images and other files.">
                Disk space
            </th>
            <td>
                100 MB
            </td>
            <td>
                500 MB
            </td>
            <td>
                1000 MB
            </td>
            <td>
                Unlimited
            </td>
        </tr>
        <tr>
            <th title="Ecommerce Storefront - Customizable storefront to take orders online">
                Ecommerce Storefront
            </th>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
        </tr>
        <tr>
            <th title="Facebook Storefront - A Facebook app is provided for you to draw traffic to your online shop">
                Facebook Storefront
            </th>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
        </tr>
        <tr>
            <th title="Mobile Storefront - Mobile themes for customers on mobile devices visiting your online store">
                Mobile Storefront
            </th>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
        </tr>
        <tr>
            <th title="SSL Encryption - Encrypted connection to prevent others from eavesdropping on your connection.">
                SSL Encryption
            </th>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
        </tr>
            <tr>
            <th title="Multi Currency - Invoices and product prices can be specified in your local currency">
                Multi Currency Support
            </th>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
        </tr>
        <tr>
            <th title="Support - Free support 24 hours a day, 7 days a week.">
                Free Support
            </th>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
        </tr>
        <tr>
            <th title="Personal Branding - Use your own organization logo and details in your invoices and purchase orders">
                Personal Branding
            </th>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
        </tr>
        <tr>
            <th title="Analytics - Track and analyze traffic on your online store">
                Analytics
            </th>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
        </tr>
        <tr>
            <th title="Custom domain name - Use your own domain name for your online store">
                Custom Domain Name
            </th>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
            <td>
                <img src="/Content/img/bullets/bullet_grey.png" alt="yes" />
            </td>
        </tr>
        <tr>
            <th title="Setup fees - What we charge for setting up your account">
                Setup Fees
            </th>
            <td>
                $0
            </td>
            <td>
                $0
            </td>
            <td>
                $0
            </td>
            <td>
                $0
            </td>
        </tr>
        <tr>
            <th title="Transaction fees - Only Paypal, Google Checkout or credit card fees applies.">
                Transaction Fees
            </th>
            <td>
                $0
            </td>
            <td>
                $0
            </td>
            <td>
                $0
            </td>
            <td>
                $0
            </td>
        </tr>
        <% if (Model == null)
           {%>
        <tr>
            <th class="empty signupEnd h100px">
                <div class="font_darkgrey">
                    You can upgrade, downgrade or cancel your account at anytime.
                </div>
            </th>
            <td class="empty">
                <a href="<%=GeneralConstants.HTTP_SECURE%>/register/single">
                    <img src="/Content/img/buttons/signup_small.png" alt="single" /></a>
            </td>
            <td class="empty">
                <a href="<%=GeneralConstants.HTTP_SECURE%>/register/basic">
                    <img src="/Content/img/buttons/signup_small.png" alt="basic" /></a>
            </td>
            <td class="empty">
                <a href="<%=GeneralConstants.HTTP_SECURE%>/register/pro">
                    <img src="/Content/img/buttons/signup_small.png" alt="pro" /></a>
            </td>
            <td class="empty">
                <a href="<%=GeneralConstants.HTTP_SECURE%>/register/ultimate">
                    <img src="/Content/img/buttons/signup_small.png" alt="ultimate" /></a>
            </td>
        </tr>
        <%
            }
           else
           {%>
        <tr>
            <th class="empty signupEnd h100px">
            </th>
            <td class="empty">
                <%if (Model.accountType != AccountPlanType.SINGLE)
                  {%>
                <a id="plan-single" href="#">
                    <img src="/Content/img/pricing/upgrade_small.png" alt="single" /></a>
                <%}
                  else
                  {%>
                <%if (Model.showPayTrialButton)
                  {%>
                <a id="plan-single" href="#">
                    <img src="/Content/img/pricing/payment_pending.png" alt="single" /></a>
                <%}
                  else
                  {%>
                <span class="grey ac larger bold">Current</span>
                <%} %>
                <%} %>
            </td>
            <td class="empty">
                <%if (Model.accountType != AccountPlanType.BASIC)
                  {%>
                <a id="plan-basic" href="#">
                    <img src="/Content/img/pricing/upgrade_small.png" alt="basic" /></a>
                <%}
                  else
                  {%>
                <%if (Model.showPayTrialButton)
                  {%>
                <a id="plan-basic" href="#">
                    <img src="/Content/img/pricing/payment_pending.png" alt="basic" /></a>
                <%}
                  else
                  {%>
                <span class="grey ac larger bold">Current</span>
                <%} %>
                <%} %>
            </td>
            <td class="empty">
                <%if (Model.accountType != AccountPlanType.PRO)
                  {%>
                <a id="plan-pro" href="#">
                    <img src="/Content/img/pricing/upgrade_small.png" alt="pro" /></a>
                <%}
                  else
                  {%>
                <%if (Model.showPayTrialButton)
                  {%>
                <a id="plan-pro" href="#">
                    <img src="/Content/img/pricing/payment_pending.png" alt="pro" /></a>
                <%}
                  else
                  {%>
                <span class="grey ac larger bold">Current</span>
                <%} %>
                <%} %>
            </td>
            <td class="empty">
                <%if (Model.accountType != AccountPlanType.ULTIMATE)
                  {%>
                <a id="plan-ultimate" href="#">
                    <img src="/Content/img/pricing/upgrade_small.png" alt="ultimate" /></a>
                <%}
                  else
                  {%>
                <%if (Model.showPayTrialButton)
                  {%>
                <a id="plan-ultimate" href="#">
                    <img src="/Content/img/pricing/payment_pending.png" alt="ultimate" /></a>
                <%}
                  else
                  {%>
                <span class="grey ac larger bold">Current</span>
                <%} %>
                <%} %>
            </td>
        </tr>
        <%
            }%>
        <tr>
            <th class="end">
            </th>
            <td class="end">
            </td>
            <td class="end">
            </td>
            <td class="end">
            </td>
            <td class="end">
            </td>
        </tr>
    </tbody>
</table>
