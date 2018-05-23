<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.category.CategoryViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>
<form id="categoryEditForm" action="<%= Url.Action("Update","Category") %>" method="post" autocomplete="off">
        <h3 id="headingProductCategory"><%= Model.categoryTitle %></h3>
        <%
            Html.RenderPartial("~/Areas/dashboard/Views/category/productList.ascx", Model.products); %>
        <div id="buttons" class="mt10">
            <button id="buttonSave" type="submit" class="green">
                update</button> 
            <button id="buttonCancel">
                cancel</button>
        </div>
<%= Html.Hidden("id", Model.catid) %>
<%= Html.Hidden("ids", Model.productIdsInCategory) %>
<%= Html.Hidden("selectedIDs") %>
</form>
<script type="text/javascript">
    $('.fixedHeight', '#categoryEditForm').scroll(function () {
        var elem = $(this);
        if (elem[0].scrollHeight - elem.scrollTop() != elem.outerHeight()) {
            // We're not at bottom
            return false;
        }
        $(this).showLoadingBlock();

        // get highest productid
        var maxid = $.trim($('.hidden', '.blockSelectable:last').html());
        $.post('/dashboard/category/products/' + maxid, null, function (json_result) {
            if (json_result.success) {
                $(elem).removeLoading();
                $.each(json_result.data, function () {
                    var html = ['<div class="blockSelectable">',
                                    '<div class="fl w50px ac">',
                                    '<img src="',
                                        this.thumbnailUrl,
                                        '" />',
                                    '</div>',
                                    '<div class="content">',
                                        this.title,
                                    '</div>',
                                    '<span class="hidden">',
                                         this.id,
                                         '</span>',
                                    '<div class="clear">',
                                    '</div>',
                                '</div>'];

                    // check if  element exists
                    if ($('.hidden:contains(' + this.id + ')', '.blockSelectable').length == 0) {
                        $(elem).append(html.join(''));
                    }
                });
            }
            else {
                $.jGrowl(json_result.message);
            }
            return false;
        }, 'json');
    });

    $(document).ready(function () {
        // init height
        var height = $(window).height();
        var titleHeight = $('.headingEdit', '#categoryEditForm').height();
        var buttonHeight = $('#buttons', '#categoryEditForm').height();
        var fixedHeight = height - titleHeight - buttonHeight - 150; // dialog padding
        $('.fixedHeight', '#categoryEditForm').height(fixedHeight);

        // bind buttons
        $('#buttonCancel', '#categoryEditForm').click(function () {
            dialogBox_close();
            return false;
        });

        // highlight products in category
        var ids = $('#ids').val().split(',');
        var blocks = $('#categoryEditForm').find('.blockSelectable');
        $.each(blocks, function () {
            var id = $.trim($(this).find('span').text());
            if ($.inArray(id, ids) != -1) {
                $(this).addClass('selected');
            }
        });

        $('#categoryEditForm').submit(function () {
            var action = $(this).attr("action");
            var ids = [];
            var selected = $(this).find(".selected");
            for (var i = 0; i < selected.length; i++) {
                ids.push($(selected[i]).find('span').text());
            }
            $('#selectedIDs').val(ids.toString());

            var serialized = $(this).serialize();

            // post form
            $.ajax({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function (json_data) {
                    if (json_data.success) {
                        reloadProductGrid(getFilterByField());
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                }
            });
            return false;
        });
    });
</script>
