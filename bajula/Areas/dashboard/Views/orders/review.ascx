<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.review.ReviewViewData>" %>
<h3 id="headingReview">
    Review</h3>
<form id="reviewForm_<%= Model.reviewID %>" action="/review/create" method="post">
<div id="content">
    <p>
        Please review our service. <a href="<%= Model.transactionLink %>">
            <%= Model.transactionName %></a>
    </p>
    <ul>
        <li><span class="w150px inline-block">Overall rating</span> <span id="overall"></span>
        </li>
        <li><span class="w150px inline-block">Would shop here again</span> <span id="willshopagain">
        </span></li>
        <li><span class="w150px inline-block">On time delivery</span> <span id="delivery"></span>
        </li>
        <li><span class="w150px inline-block">Customer Support</span> <span id="support"></span>
        </li>
    </ul>
    <div class="form_entry w50p">
        <div class="form_label fl">
            <label for="firstName">
                Additional Comment</label><span class="tip_inline">optional</span>
        </div>
        <div class="charsleft fr">
            <span id="comment-charsleft"></span>
        </div>
        <div class="clear">
        </div>
        <%= Html.TextArea("comment") %>
    </div>
    <%= Html.Hidden("reviewid", Model.reviewID) %>
    <div class="clear">
    </div>
    <div class="mt5">
        <button id="buttonSave" class="green" type="button">
            save</button>
    </div>
</div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#comment', '#reviewForm_<%= Model.reviewID %>').limit('500', '#comment-charsleft');
        $('#buttonSave', '#reviewForm_<%= Model.reviewID %>').click(function () {
            $(this).trigger('submit');
        });
        inputSelectors_init('#reviewForm_<%= Model.reviewID %>');
        init_autogrow('#reviewForm_<%= Model.reviewID %>');
        $('#overall').raty({
            start: 3,
            scoreName: 'overall'
        });
        $('#willshopagain', '#reviewForm_<%= Model.reviewID %>').raty({
            start: 3,
            scoreName: 'willshopagain'
        });
        $('#delivery', '#reviewForm_<%= Model.reviewID %>').raty({
            start: 3,
            scoreName: 'delivery'
        });
        $('#support', '#reviewForm_<%= Model.reviewID %>').raty({
            start: 3,
            scoreName: 'support'
        });
    });

    $('#reviewForm_<%= Model.reviewID %>').submit(function () {
        var action = $(this).attr("action");
        var serialized = $(this).serialize();

        // post form
        $.ajaxswitch({
            type: "POST",
            url: action,
            dataType: "json",
            data: serialized,
            success: function (json_data) {
                if (json_data.success) {
                    $('#content', '#reviewForm_<%= Model.reviewID %>').html("<p>Review saved. Thank you.</p>");
                }
                else {
                    $.jGrowl(json_data.message);
                }
            }
        });
        return false;
    });
</script>
