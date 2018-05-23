<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<String>" %>
<h3 id="headingTerms">
     Set Default Payment Information</h3>
     <p>The description set here will be used in future payment information. Please use plain text (no HTML tags).</p>
<form id="termForm" action="/dashboard/product/paymentTerms" method="post">
<div class="form_entry">
    <div class="form_label">
        <label>
            Payment Information</label>
    </div>
    <%= Html.TextArea("paymentTerms", Model, 
        new Dictionary<string, object> { { "style", "width:100%;height:150px" } })%>
</div>
<span class="fr" id="terms-charsleft"></span>
<input name="useTerms" id="useTerms" type="checkbox" checked="checked" />
<label for="useTerms">apply description to this product</label>

    <div class="pad5">
        <button id="buttonSave" type="button" class="green ajax">
            save</button>
        <button id="buttonCancel" type="button">
            cancel</button>
            </div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#paymentTerms', '#termForm').limit('1000', '#terms-charsleft');
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
                        var term = $('textarea#paymentTerms', '#termForm').val();
                        $('textarea#paymentTerms', '#productAddForm').val(term);
                    }
                    $('#buttonSave', '#termForm').buttonEnable();
                    dialogBox_close();
                }
            });
            return false;
        });
        inputSelectors_init('#termForm');
    });
</script>
