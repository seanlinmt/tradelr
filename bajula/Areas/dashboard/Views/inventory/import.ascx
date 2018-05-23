<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="tradelr.Common" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Libraries.Helpers" %>
<%@ Import Namespace="tradelr.Library" %>
<div id="importProductForm">
<h3 id="headingAutopost">
    import products</h3>
<div>
    <p>
        Upload your products according to the following template: <a href="/Content/templates/tradelrInventoryTemplate.xls">tradelr inventory template</a></p>
</div>
<div class="mt10 mb10">
<div id="buttonUpload">
</div>
</div>
<div id="duplicates" class="mt10 mb10 error">
</div>
<div class="clear"></div>
<p class="icon_help">You can also <a href="/dashboard/networks">import products from other networks</a>.</p>
<button id="buttonCancel" type="button">close</button>
</div>
<script src="/jsapi/uploader" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {

        $('#buttonCancel', '#importProductForm').click(function () {
            dialogBox_close();
        });

        initAjaxUpload();
    });


    function initAjaxUpload() {
        // upload url
        var uploader = new qq.FileUploader({
            element: $('#buttonUpload', '#importProductForm')[0],
            action: '/dashboard/inventory/import',
            allowedExtensions: ['xls'],
            onSubmit: function (id, filename) {
            },
            onComplete: function (id, filename, json_data) {
                // enable upload button
                if (json_data.success) {
                    var skus = json_data.data;
                    if (skus.length == 0) {
                        $.jGrowl('Products successfully imported');
                        reloadProductGrid(getFilterByField('#inventory_mine'));
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl('Products successfully imported with duplicates');
                        reloadProductGrid(getFilterByField('#inventory_mine'));
                        $('#duplicates').html('<strong>Duplicate Products:</strong> ' + skus.join(', '));
                    }
                }
                else {
                    $.jGrowl(json_data.message);
                }
            }
        });
    }

</script>

