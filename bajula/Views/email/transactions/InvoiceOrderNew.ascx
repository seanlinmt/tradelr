<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<NewOrderEmailContent>" %>
<%@ Import Namespace="tradelr.Models.transactions"%>
<p>From <%=Model.sender%>:</p>
<p><%=Model.message%></p>
<p><%=Model.viewloc%></p>
