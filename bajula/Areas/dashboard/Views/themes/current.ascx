<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.store.themes.Theme>" %>
<div class="fl w250px">
<img class="img_border" src="<%= Model.thumbnail %>" alt="<%= Model.title %>"/>
</div>
<div class="fl w300px">
<h3>Main Theme</h3>
<h4><%= Model.title %></h4>
<ul class="theme-action-links custom mt50">
<li><a href="#" onclick="$('#settings_tabs').tabs('select', 1);return false;">theme editor</a></li>
<li><a href="#" onclick="$('#settings_tabs').tabs('select', 2);return false;">edit theme settings</a></li>
</ul>
</div>
<div class="fl">
<h3>Mobile Theme</h3>
<h4>&nbsp;</h4>
<ul class="theme-action-links custom mt50">
<li><a href="#" onclick="$('#settings_tabs').tabs('select', 3);return false;">mobile theme editor</a></li>
<li><a href="#" onclick="$('#settings_tabs').tabs('select', 4);return false;">edit mobile theme settings</a></li>
<li><strong><a href="#" id="resetMobileLink">reset mobile theme</a></strong></li>
</ul>
</div>
<div class="clear"></div>
<script type="text/javascript">
    $('li', '.theme-action-links').each(function () {
        $(this).addClass('icon_edit');
    });

    $('#resetMobileLink').click(function () {
        var ok = window.confirm("Are you sure? This will overwrite the current mobile theme.");
        if (!ok) {
            return false;
        }

        $.post("/dashboard/themes/mobilereset", function (json_result) {
            if (json_result.success) {
                window.location.reload();
            }
            $.jGrowl(json_result.message);
        });

        return false;
    });
</script>