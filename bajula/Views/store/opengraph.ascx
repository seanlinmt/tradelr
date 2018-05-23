<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.facebook.OpenGraph>" %>
<%@ Import Namespace="tradelr.Common.Constants" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<meta property="fb:admins" content="<%= Model.fbid %>" />
<meta property="fb:app_id" content="<%= GeneralConstants.FACEBOOK_APP_ID %>"/>
<meta property="og:site_name" content="<%= Model.sitename %>" />
<meta property="og:description"  content="<%= Model.description %>" />
<meta property="og:latitude" content="<%= Model.latitude %>" />
<meta property="og:longitude" content="<%= Model.longtitude %>" />
<meta property="og:street-address" content="<%= Model.address %>" />
<meta property="og:locality" content="<%= Model.locality %>" />
<meta property="og:region" content="<%= Model.region %>" />
<meta property="og:postal-code" content="<%= Model.postcode %>" />
<meta property="og:country-name" content="<%= Model.countryname %>" />
<meta property="og:email" content="<%= Model.email %>" />
<meta property="og:phone" content="<%= Model.phone %>" />
<meta property="og:fax" content="<%= Model.fax %>" />
<meta property="og:title" content="<%= Model.title %>" />
<meta property="og:type" content="<%= Model.type %>" />
<meta property="og:url" content="<%= Model.url %>" />
<meta property="og:image" content="<%= Model.image %>" />


