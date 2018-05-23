<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<form id="videoAddForm" action="/video/create" method="post">
        <h3 id="headingAdd">add YouTube video link</h3>
        <div class="form_entry">
            <div class="form_label">
                <label for="categoryTitle">
                    YouTube video location</label>
                    <span class="tip">Example: http://www.youtube.com/watch?v=V74AxCqOTvg</span>
            </div>
            <input type="text" name="url" id="url" value="" class="w300px" />
        </div>
        <div class="pt5">
            <button id="saveButton" type="button" class="green">
                add</button>
            <button id="cancelButton">
                cancel</button>
        </div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#videoAddForm').submit(function () {
            var action = $(this).attr("action");
            var postdata = {
                url: $('#url', '#videoAddForm').val(),
                productid: $('#id').val()
            };

            // post form
            $.ajaxswitch({
                type: "POST",
                url: action,
                dataType: "json",
                data: postdata,
                success: function (json_data) {
                    if (json_data.success) {
                        var img = json_data.data;
                        renderThumbnail('#product_images', img.url, img.id, img.externalid);
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                }
            });
            return false;
        });

        $('#saveButton').click(function () {
            $('#videoAddForm').trigger('submit');
        });

        $('#cancelButton').click(function () {
            dialogBox_close();
            return false;
        });

        // input highlighters
        inputSelectors_init();
    });
</script>
