<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.facebook.OpenGraph>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<% Html.RenderControl(TradelrControls.opengraph, Model); %>
<% if (!GeneralConstants.DEBUG)
   {%>
<script type="text/javascript">
    window.onload = function() {
        var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www."),
            pageTracker,
            s;
        s = document.createElement('script');
        s.src = gaJsHost + 'google-analytics.com/ga.js';
        s.type = 'text/javascript';
        s.onloadDone = false;
        function init() {
            pageTracker = _gat._getTracker("UA-12041588-1");
            pageTracker._setDomainName(".tradelr.com");
            pageTracker._setAllowLinker(true);
            pageTracker._setAllowHash(false);
            pageTracker._trackPageview();
        }
        s.onload = function() {
            s.onloadDone = true;
            init();
        };
        s.onreadystatechange = function() {
            if (('loaded' === s.readyState || 'complete' === s.readyState) && !s.onloadDone) {
                s.onloadDone = true;
                init();
            }
        };
        document.getElementsByTagName('head')[0].appendChild(s);
    };
</script>
<%}%>