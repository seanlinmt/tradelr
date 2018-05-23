<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.store.navigation.LinksViewModel>" %>
<form autocomplete="off" id="linkListForm" method="POST" action="<%= Url.Action("Save","Links") %>">
<div class="form_group">
    <strong>Store Navigation (Link Lists)</strong>
    <p>
        <a id="newlinklist" class="icon_add" href="#">
            create new list</a></p>
    <p class="tip">
        Link Lists helps you built navigation menus. You will need to edit your store templates to use any newly created lists.</p>
        <div id="store_linklists">
    <% foreach (var list in Model.linklists)
       {
           Html.RenderPartial("ListContent", list);
       } %>
       </div>
       <%= Html.Hidden("liquid_linklists") %>
</div>
<div class="buttonRow_bottom">
<div class="mr10">
    <button id="buttonSaveLinkLists" type="submit" class="green ajax">
        <img src="/Content/img/save.png" alt='' />
        update link lists</button>
</div>
</div>
</form>
<script type="text/javascript">
    $(document).ready(function() {
        linklists_handler.init();
        $('#newlinklist').unbind().bind('click', function () {
            $.get('/dashboard/links/new', function (resultstring) {
                var result = $(resultstring);
                result = $(result).addClass('hidden');
                $(result).find('.title').text('New List');
                $('#store_linklists').prepend(result);
                $('.icon_add', result).trigger('click');
                $(result).fadeIn();
            });
            return false;
        });
    });

    
    // add links
    $('.icon_add', '.linklist').live('click', function () {
        var newrow = $(this).siblings('.spare').children('li').clone();
        var ul = $(this).siblings('#list_links');
        ul.append(newrow);
        $('li:last #link_type', ul).trigger('change');
        return false;
    });
    
    // delete link
    $('.hover_del', '.linklist li').die().live('click', function () {
        var title = $(this).parents('li').find('#link_title').val();
        var row = $(this).parents('li');
        var linkid = $(row).attr('id').replace(/link_/, '');
        var container = $(this).parents('.linklist');
        var listid = $(container).attr('id').replace(/linklist_/, '');
        if (linkid == 0 || listid == 0) {
            $(row).fadeOut('fast', function () {
                $(this).remove();
            });
            return false;
        }

        var ok = window.confirm("Delete " + title + "? There is NO UNDO.");
        if (!ok) {
            return false;
        }
        $.post('/dashboard/links/delete/' + listid, { linkid: linkid }, function (json_result) {
            if (json_result.success) {
                $(row).slideUp('fast', function () {
                    $(this).remove();
                });
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
        return false;
    });
    
    // delete list
    $('.hover_del', '.linklist > .section_header').die().live('click', function () {
        var title = $(this).parents('.section_header').find('title').text();
        var container = $(this).parents('.linklist');
        var listid = $(container).attr('id').replace(/linklist_/, '');
        if (listid == 0) {
            $(this).parents('.linklist').fadeOut('fast', function () {
                $(this).remove();
            });
            return false;
        }

        var ok = window.confirm("Delete " + title + "? There is NO UNDO.");
        if (!ok) {
            return false;
        }
        $.post('/dashboard/links/deletelist/' + listid, null, function (json_result) {
            if (json_result.success) {
                $(container).fadeOut('fast', function () {
                    $(this).remove();
                });
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');

        return false;
    });
    
    var linklists_handler = function() {
        var linklinkoptions = <%= Model.selectablesString %>;

        var processSelectors = function(row, type) {
            var options = linklinkoptions[type];
            if (options != undefined) {
                $('#link_url_select', row).html('').removeClass('hidden');
                for (var i = 0; i < options.length; i++) {
                    var option = ["<option value='", options[i][1], "'>", options[i][0], "</option>"];
                    $('#link_url_select', row).append(option.join(''));
                }

                var selected = $('#link_url_select', row).attr('alt');
                $('#link_url_select', row).val(selected);
            }

            // handle type specific
            switch (type) {
            case "BLOG":
                $('#link_url_select', row).maxwidth(330);
                break;
            case "COLLECTION":
                $('#link_url_select', row).width(155);
                $('#link_filter', row).width(160);
                $('#link_filter', row).removeClass('hidden');
                break;
            case "PAGE":
                $('#link_url_select', row).maxwidth(330);
                break;
            case "PRODUCT":
                $('#link_url_select', row).maxwidth(330);
                break;
            case "WEB":
                $('#link_url_raw', row).removeClass('hidden');
                break;
            }
        };
        
        var init = function() {
            var self = this;
            $.each($('#link_type', '#store_linklists'), function() {
                var row = $(this).parents('li');
                var type = $(this).val();
                processSelectors(row, type);
            });

            $('#link_type', '#store_linklists').live('change', function() {
                var type = $(this).val();
                var row = $(this).parents('li');
                initRowUrlSelectors(row);
                processSelectors(row, type);
            });
        };

        var initRow = function(row) {
            $(row).attr('id', '0');
            $('input, select', row).val('');
            initRowUrlSelectors(row);
            inputSelectors_init();
        };

        var initRowUrlSelectors = function(row) {
            $('#link_url_select', row).html('').attr('alt', '');
            $('#link_url_select,#link_url_raw,#link_filter', row).addClass('hidden');
            $('#link_url_select,#link_filter', row).val('');

            // remove any width attributes from setting for PRODUCT type
            $('#link_url_select', row).width('auto');
        };

        var convertFieldsToJsonStringAndSetField = function() {
            var listo = function(id, name, links, handle) {
                this.id = id;
                this.title = name;
                this.links = links;
                this.handle = handle;
            };

            var linko = function(id, name, type, select, filter, raw) {
                this.id = id;
                this.title = name;
                this.type = type;
                this.url_selected = select;
                this.url_filter = filter;
                this.url_raw = raw;
            };

            // get all lists
            var lists = [];
            var linklists = $('.linklist', '#store_linklists');
            for (var j = 0; j < linklists.length; j++) {
                var ls = linklists[j];
                var name = $('#list_name', ls).val();
                var handle = $('#list_handle', ls).val();
                var id = $(ls).attr('id').replace(/linklist_/, '');
                var list = new listo(id, name, [], handle);
                var links = $('#list_links li', ls);
                for (var i = 0; i < links.length; i++) {
                    var l = links[i];
                    var link_id = $(l).attr('id').replace(/link_/, '');
                    var link_name = $('#link_title', l).val();
                    var link_type = $('#link_type', l).val();
                    var select = $('#link_url_select', l).val();
                    var raw = $('#link_url_raw', l).val();
                    var filter = $('#link_filter', l).val();
                    var link = new linko(link_id, link_name, link_type, select, filter, raw);
                    list.links.push(link);
                }
                lists.push(list);
            }
            $('#liquid_linklists').val($.toJSON(lists));
        };

        var validate = function() {
            var ok = true;
            var elements = $('#list_name, #list_links #link_title', '.linklist');
            for (var i = 0; i < elements.length; i++) {
                var entry = elements[i];
                if ($(entry).val() == '') {
                    ok = false;
                    $(entry).addClass('curFocus_red').one('click', function() {
                        $(this).removeClass('curFocus_red');
                    }).focus();
                }
            }
            return ok;
        };

        return {
            init: init,
            row_init:initRow,
            set: convertFieldsToJsonStringAndSetField,
            validate:validate
        };
    }();

    $('#linkListForm').unbind().bind('submit', function() {
        var action = $(this).attr("action");

        var linklistsOk = linklists_handler.validate();

        if (!linklistsOk) {
            $('#settings_tabs').tabs('select', 2);
            return false;
        }
        $('#buttonSaveLinkLists').buttonDisable();

        linklists_handler.set();
        var serialized = $(this).serialize();
        // post form
        $.ajax({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function(json_data) {
                    if (json_data.success) {
                        $.jGrowl('Settings successfully updated');
                        isDirty = false;
                        scrollToTop();
                    }
                    else {
                        $.jGrowl(json_data.message, { sticky: true });
                    }
                    $('#buttonSave').buttonEnable();
                    return false;
                }
            });
        return false;
    });
</script>
