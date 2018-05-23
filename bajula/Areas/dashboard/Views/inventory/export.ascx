<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<form id="exportProductForm" action="/dashboard/inventory/export" method="post">
<h3 id="headingAutopost">
    export products</h3>
<ul class="mt20 mb20 ml10">
    <li>
        <input id="export_all" type="radio" name="exportType" checked="checked" />
        <label for="export_all">Export all products</label></li>
    <li>
        <input id="export_selected" type="radio" name="exportType" />
        <label for="export_selected">Export selected products</label></li>
    <li>
        <input id="export_shipwire" type="radio" name="exportType" />
        <label for="export_shipwire">Export all products for Shipwire</label></li>
</ul>
<div id="shipwiremessage" class="ba font_darkgrey pad5 smaller mb10 hidden">
            SKUs must be only contain alphabets and numbers and are limited to 12 characters. 
            Products not following this format will not be exported.
            </div>
<div>
    <button id="buttonExport" type="submit" class="green">
        export</button>
    <button id="buttonCancel">
        close</button>
</div>
<%= Html.Hidden("ids") %>
</form>
<script type="text/javascript">
    $(document).ready(function () {

        $('#buttonCancel', '#exportProductForm').bind("click", function () {
            dialogBox_close();
            return false;
        });

        $("input[name='exportType']", '#exportProductForm').change(function () {
            var id = $(this).attr('id');
            if (id == 'export_shipwire') {
                $('#shipwiremessage').show();
            }
            else {
                $('#shipwiremessage').hide();
            }
        });

        $('#exportProductForm').submit(function () {
            if ($('#export_shipwire').is(':checked')) {
                $(this).attr('action', '/dashboard/shipwire/export');
            }
            else {
                if ($('#export_selected:checked').length != 0) {
                    if ($('.selected-row', '#productGridView').length == 0) {
                        $.jGrowl('No product selected');
                        return false;
                    }
                    var ids = [];
                    var entries = $('.selected-row', '#productGridView');
                    $.each(entries, function () {
                        var id = $(this).attr('id');
                        ids.push(id);
                    });
                    $('#ids').val(ids.join(','));
                }
            }
        });
    });
</script>
