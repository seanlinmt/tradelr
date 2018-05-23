<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.collections.CollectionViewModel>" %>
<form id="collectionEditForm" action="<%= Url.Action("Update","Collections") %>" method="post">
    <div id="header_area">
        <h3>edit collection</h3>
        <div class="fl">
            <div class="form_entry">
                <div class="form_label">
                    <label for="title">
                        Collection Title</label>
                </div>
                <%= Html.TextBox("title", Model.title)%>
            </div>
        </div>
        <div class="fl ml50">
            <div class="form_entry">
                <div class="form_label">
                    <label for="handle">
                        Handle</label>
                </div>
                <%= Html.TextBox("handle", Model.permalink)%>
            </div>
        </div>
        <div class="fl ml50">
            <div class="form_entry">
                <div class="form_label">
                    <label for="visible">
                        Visibility</label>
                </div>
                <%= Html.CheckBox("visible", Model.visible) %>
                <label for="visible">visible</label>
            </div>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="details">
                    Description <span class="tip_inline"><%= Model.fullUrl %></span>
                </label>
            </div>
            <%= Html.TextArea("details", Model.details, new Dictionary<string, object>(){{"class","w99p"}})%>
        </div>
    </div>
    <% Html.RenderPartial("~/Areas/dashboard/Views/category/productList.ascx", Model.products); %>
    <div id="buttons" class="pt5">
        <button id="buttonSave" type="submit" class="green">
            save</button>
        <button id="buttonCancel">
            cancel</button>
    </div>
    <%= Html.Hidden("id", Model.id) %>
    <%= Html.Hidden("ids", Model.productids) %>
    <%= Html.Hidden("selectedIDs") %>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        // init height
        var height = $(window).height();
        var titleHeight = $('#header_area', '#collectionEditForm').height();
        var buttonHeight = $('#buttons', '#collectionEditForm').height();
        var fixedHeight = height - titleHeight - buttonHeight - 150; // dialog padding
        $('.fixedHeight', '#collectionEditForm').height(fixedHeight);

        inputSelectors_init();
        $("#handle", "#collectionEditForm").alphanumeric({ allow: '-_' });

        // bind buttons
        $('#buttonCancel', '#collectionEditForm').click(function () {
            dialogBox_close();
            return false;
        });

        // highlight products in category
        var ids = $('#ids').val().split(',');
        var blocks = $('#collectionEditForm').find('.blockSelectable');
        $.each(blocks, function () {
            var id = $.trim($(this).find('span').text());
            if ($.inArray(id, ids) != -1) {
                $(this).addClass('selected');
            }
        });

        $('#collectionEditForm').submit(function () {
            var action = $(this).attr("action");
            var selectedIDs = [];
            var selected = $(this).find(".selected");
            for (var i = 0; i < selected.length; i++) {
                selectedIDs.push($.trim($(selected[i]).find('span').text()));
            }
            $('#selectedIDs').val(selectedIDs.toString());

            var serialized = $(this).serialize();

            // post form
            $.ajax({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function (json_data) {
                    if (json_data.success) {
                        var collection = json_data.data;
                        console.log(collection.id + " " + collection.name);
                        $('.sideboxEntry[fid=' + collection.id + ']', '#collectionsList').find('.title').text(collection.name);
                        dialogBox_close();
                        $.jGrowl("Collection successfully updated");
                        reloadProductGrid(getFilterByField('#inventory_mine'));
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
