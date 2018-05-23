<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="tradelr.Common"%>
<%@ Import Namespace="tradelr.Common.Constants" %>
<%@ Import Namespace="tradelr.Library.Constants" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Not Authorized</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link rel="icon" href="/Content/img/favicon.png" type="image/png" />
    <link rel="shortcut icon" href="/Content/img/favicon.png" type="image/png" />
    <link href="/Content/css/common.css" rel="stylesheet" type="text/css" media="screen, print" />
    <link href="/Content/css/top.css" rel="stylesheet" type="text/css" media="screen, print" />
    <link href="../../Content/css/outside.css" rel="stylesheet" type="text/css" />
    <!--[if IE]>
	<link rel="stylesheet" type="text/css" href="../../Content/css/ie.css" />
	<![endif]-->

</head>
<body>
    <div id="container">
        <div id="header">
        <div class="content pt5">
            <div id="logo">
                <a href="/"><img src="/Content/img/tradelr.png" alt="tradelr" /></a>
            </div>
        </div>
        </div>
        <div class="banner_main">
            <div class="content">
            <div class="panel_content">
            <h2 class="headingError">Not Authorized</h2>
            <p class="larger font_darkgrey">You are not authorized to access the page. This is probably either due to an unverified email address or you may need to <a href="<%= GeneralConstants.HTTP_HOST %>/pricing">create an account</a>.</p>
            <p class="mt50">« <a href="#" onclick="window.history.back();return false;">Go back</a></p>
            </div>
            </div>
        </div>
    </div>
</body>
</html>
