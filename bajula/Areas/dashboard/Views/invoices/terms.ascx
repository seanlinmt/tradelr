<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<String>" %>
<%@ Import Namespace="tradelr.Models.transactions"%>
<h3 id="headingTerms">
     Set Default Payment Terms</h3>
     <p>The terms set here will be used in future payment terms.</p>
<form id="termForm" action="/dashboard/invoices/terms" method="post">
<div class="form_entry">
    <div class="form_label">
        <label>
            Payment Terms</label>
    </div>
    <%= Html.TextArea("terms", Model, new Dictionary<string, object> { { "style", "width:100%" } })%>
</div>
<input id="useTerms" type="checkbox" checked="checked" /><label for="useTerms">Apply terms to this invoice</label>

    <div class="pad5">
        <button id="buttonSave" type="button" class="green ajax">
            Save</button>&nbsp;
        <button id="buttonCancel" type="button">
            Cancel</button>
            </div>
</form>
<script type="text/javascript">
    $(document).ready(function() {
        $('#buttonSave', '#termForm').click(function() {
            $(this).buttonDisable();
            $(this).trigger('submit');
        });
        $('#buttonCancel', '#termForm').click(function() {
            dialogBox_close();
        });

        $('#termForm').submit(function() {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();

            // post form
            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                success: function(data) {
                    // flash message
                    $.jGrowl('Your default payment terms have been updated successfully');

                    // if checkbox is set then set the current term
                    if ($('#useTerms:checked', '#termForm').val() !== undefined) {
                        var term = $('textarea#terms', '#termForm').val();
                        $('textarea#terms', '#salesInvoiceAddForm').val(term);
                    }
                    $('#buttonSave', '#termForm').buttonEnable();
                    dialogBox_close();
                }
            });
            return false;
        });
        inputSelectors_init('#termForm');
        init_autogrow('#termForm');
    });
</script>
