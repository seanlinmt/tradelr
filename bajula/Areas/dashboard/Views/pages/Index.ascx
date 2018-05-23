<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.store.viewmodel.PagesViewModel>" %>
<%@ Import Namespace="tradelr.Areas.dashboard.Models.store.blog" %>
<div class="form_group">
<strong class="headingPage">Pages</strong>
<p class="tip">Add additional pages to your store to inform your customers about your business or product. For example, "About" section, "Return Policy" page etc.</p>
<a id="newPageLink" class="icon_add" href="<%= Url.Action("New","Pages") %>">new page</a>
<% if (Model.pages.Count() != 0) {%>
<table id="storepagesTable" class="normal">
<thead><tr><td>Title</td><td class="w100px ac">Visible?</td><td class="w100px ar">Last Updated</td><td class="w50px"></td></tr></thead>
<tbody>
<% foreach (var page in Model.pages)
{%>
  <tr alt="<%= page.id %>">
  <td><a class="pagelink" href="/dashboard/pages/<%= page.id %>"><%= page.title %></a></td>
  <td class="ac"><%= page.visible?"yes":"no" %></td>
  <td class="ar"><%= page.updated %></td><td class="ar"><span class="hover_del"></span></td>
  </tr>
<%} %>
</tbody>
</table>
 <%  } %>
</div>

<div class="form_group mt50">
<strong class="headingBlog">Blogs</strong>
<p class="tip">A blog is a list of regularly updated articles. Your customers can subscribe to your blog to keep up to date.</p>
<a id="newBlogLink" class="icon_add" href="<%= Url.Action("New","Blogs") %>">new blog</a>
<% if (Model.blogs.Count() != 0) {%>

<% foreach (var blog in Model.blogs)
{%>
  <table class="blogTable normal" id="storeBlogsTable_<%= blog.id %>">
<thead>
<tr><th colspan="<%= blog.commentType != Commenting.OFF?4:3 %>"><a class="editlink" href="/dashboard/blogs/edit/<%= blog.id %>"><%= blog.title %></a></th>
<th class="ar"><a class="hover_del_no"  href="/dashboard/blogs/delete/<%= blog.id %>"></a></th></tr>
<tr><td>Articles</td>
<% if (blog.commentType != Commenting.OFF){%>
<td>Comments</td>
<%} %>
<td>Tags</td><td class="w100px">Author</td><td class="w100px ar">Last Updated</td></tr></thead>
<tbody>
  <% foreach (var article in blog.articles){%>
    <tr alt="<%= article.id %>">
    <td><a class="articlelink" href="/dashboard/blogs/articleEdit/<%= article.id %>"><%= article.title %></a></td>
    <% if (blog.commentType != Commenting.OFF){%>
    <td><%= article.comment_count %></td>
    <%} %>
    <td><%= article.tags %></td>
    <td><%= article.creator_name %></td>
    <td class="ar"><%= article.created %></td>
    </tr>
<%} %>
  </tbody>
  <tfoot>
  <tr><td colspan="<%= blog.commentType != Commenting.OFF?5:4 %>"><a id="newArticleLink" class="icon_add" href="/dashboard/blogs/articlenew?blogid=<%= blog.id %>">add blog article</a></td></tr>
  </tfoot>
</table>
<%} %>
 <%  } %>
</div>
<script type="text/javascript">
    // comments
    $('.hover_del,.unspamlink,.spamlink,.approvelink', '.article_comments').die().live('click', function () {
        var href = $(this).attr('href');
        var row = $(this).closest('tr');
        var isdelete = $(this).hasClass('hover_del');
        $.post(href, function (json_result) {
            if (json_result.success) {
                if (isdelete) {
                    $(row).slideUp(function () {
                        $(this).remove();
                    });
                }
                else {
                    $(row).replaceWith(json_result.data);
                }
            }
            $.jGrowl(json_result.message);
        });
        return false;
    });
    
    $('.hover_del', '#storepagesTable').bind('click', function () {
        var ok = window.confirm("Are you sure? There is NO UNDO.");
        if (!ok) {
            return false;
        }
        var row = $(this).parents('tr');
        var pageid = $(row).attr('alt');
        $.post('<%= Url.Action("Delete","Pages") %>/' + pageid, null, function (json_result) {
            if (json_result.success) {
                $(row).slideUp('fast', function () {
                    $(this).remove();
                });
            }
            $.jGrowl(json_result.message);
        }, 'json');
    });

    $('#newPageLink').die().live('click', function () {
        var href = $(this).attr('href');
        tradelr.tabs.add(href, '#tab_new_page', 'new page', '#settings_tabs');
        return false;
    });

    $('#newBlogLink').die().live('click', function () {
        var href = $(this).attr('href');
        tradelr.tabs.add(href, '#tab_new_blog', 'new blog', '#settings_tabs');
        return false;
    });

    $('.editlink, .hover_del_no', '.blogTable').die().live('click', function () {
        var href = $(this).attr('href');

        if ($(this).hasClass('editlink')) {
            var txt = $(this).text();
            var id = $(this).parents('table').attr('id');
            tradelr.tabs.add(href, '#tab_' + id, txt, '#settings_tabs');
        }
        else {
            // handle delete
            var ok = window.confirm("Are you sure? This will delete all related articles, comments and tags");
            if (ok) {
                var self = $(this).parents('table');
                $.post(href, function (json_result) {
                    if (json_result.success) {
                        $(self).slideUp(function () {
                            $(this).remove();
                        });
                        $.jGrowl(json_result.message);
                    }
                });
            }
        }

        return false;
    });

    $('#newArticleLink').die().live('click', function () {
        var href = $(this).attr('href');
        tradelr.tabs.add(href, '#tab_new_article', 'new blog article', '#settings_tabs');
        return false;
    });

    $('.articlelink', '.blogTable').die().live('click', function () {
        var href = $(this).attr('href');
        var text = $(this).text();
        var idno = $(this).parents('tr').attr('alt');
        tradelr.tabs.add(href, '#article_' + idno, text, '#settings_tabs');
        return false;
    });

    $('.pagelink', '#storepagesTable').die().live('click', function () {
        var href = $(this).attr('href');
        var text = $(this).text();
        var idno = $(this).parents('tr').attr('alt');
        tradelr.tabs.add(href, '#page_' + idno, text, '#settings_tabs');
        return false;
    });

</script>