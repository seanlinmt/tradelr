<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.store.navigation.LinkList>" %>
<%@ Import Namespace="tradelr.Areas.dashboard.Models.store.navigation" %>
<div class="linklist mb50" id="linklist_<%= Model.id %>">
    <div class="section_header">
        <span class="title">
            <%= Model.title %></span><span class="fr"><% if (!Model.permanent)
                                                         {%>
                <span class="hover_del m0"></span>
                <%}%></span></div>
    <div class="form_group">
        <div class="fl mr20">
            <div class="form_entry">
                <div class="form_label">
                    <label>
                        Name</label>
                </div>
                <%=Html.TextBox("list_name", Model.title)%>
            </div>
        </div>
        <div class="fl">
            <div class="form_entry">
                <div class="form_label">
                    <label>
                        Handle</label>
                </div>
                <input id="list_handle" name="list_handle" type="text" value="<%= Model.handle %>" <%= Model.permanent?"disabled='disabled'":"" %> />
                <% if (Model.permanent)
                   { %>
                <span class="tip_inline">Default link-list cannot be modified</span>
                <% } %>
            </div>
        </div>
        <div class="clear mb20">
        </div>
        <span class="link_title bold smaller">Name of link</span><span class="link_url bold smaller">Links
            to</span>
        <ul id="list_links" class="mb10">
            <% foreach (var link in Model.links)
               { %>
            <li id="link_<%= link.id %>"><span class="link_title">
                <%= Html.TextBox("link_title", link.title) %></span> <span class="link_url">
                    <%= Html.DropDownList("link_type", link.typeList, new Dictionary<string, object>() { { "class", "mr5" } })%>
                    <select id="link_url_select" name="link_url_select" alt="<%= link.url_selected %>" class="hidden">
                    </select>
                    <input type="text" id="link_url_raw" name="link_url_raw" value="<%= link.url_raw %>"
                        class="hidden" />
                    <input type="text" id="link_filter" name="link_filter" value="<%= link.url_filter %>"
                        class="hidden" />
                </span><span class="fr hover_del mt6 mr5"></span></li>
            <%} %>
        </ul>
        <a class="icon_add" href="#">add new link</a>
        <ul class="hidden spare">
            <li id="link_"><span class="link_title">
                <%= Html.TextBox("link_title") %></span> <span class="link_url">
                    <%= Html.DropDownList("link_type", LinkList.referenceLinkTypeList, new Dictionary<string, object>() { { "class", "mr5" } })%>
                    <select id="link_url_select" name="link_url_select" alt="" class="hidden"></select>
                    <input type="text" id="link_url_raw" name="link_url_raw" value="" class="hidden" />
                    <input type="text" id="link_filter" name="link_filter" value="" class="hidden" />
                </span><span class="fr hover_del mt6 mr5"></span></li>
        </ul>
    </div>
    </div>
<script type="text/javascript">
    $(document).ready(function() {
        $("input[name=list_handle]", ".linklist").alphanumeric({ allow: '-_' });
    });
</script>