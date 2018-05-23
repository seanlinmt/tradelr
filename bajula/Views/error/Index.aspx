<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.error.ErrorViewModel>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>An error has occurred.</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link rel="icon" href="/Content/img/favicon.png" type="image/png" />
    <link rel="shortcut icon" href="/Content/img/favicon.png" type="image/png" />
    <link href="/Content/css/common.css" rel="stylesheet" type="text/css" media="screen, print" />
    <link href="/Content/css/top.css" rel="stylesheet" type="text/css" media="screen, print" />
    <link href="../../Content/css/outside.css" rel="stylesheet" type="text/css" />
    <!--[if IE]>
	<link rel="stylesheet" type="text/css" href="../../Content/css/ie.css" />
	<![endif]-->
    <script src="/Scripts/jquery-1.4.2.min.js" type="text/javascript"></script>
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
            <h2>An error has occurred.</h2>
            <p class="larger font_darkgrey"><%= Model.message %> </p>
            <% if (!string.IsNullOrEmpty(Model.redirectUrl))
               { %>
                   <p class="mt30"><a href="<%= Model.redirectUrl %>">Click here to continue</a></p>
              <% }
               else
               { %>
              <p class="mt50">« <a href="#" onclick="window.history.back();return false;">Go back</a></p>
              <% } %>
            </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            var redirect = '<%= Model.redirectUrl %>';
            if (redirect != null && redirect != '') {
                setTimeout(function () {
                    window.location = redirect;
                }, 10000);
            }
        });
    </script>
</body>
</html>
