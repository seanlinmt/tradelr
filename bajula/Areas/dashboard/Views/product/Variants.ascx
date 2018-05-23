<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.products.viewmodel.VariantsContentOptions>" %>
<div id="variantsDialog">
<h3 class="fl headingEdit">
    Select Product</h3>
    <div id="search_area" class="fr">
    <span class="search"></span>
                    <input type="text" id="searchbox" name="searchbox" class="searchbox" />
    </div>
    <div class="clear pt5"></div>
<div class="fixedHeight scroll_y">
    <%
        Html.RenderAction("VariantsContent",new { term = Model.term, sinceid = Model.sinceid });%>
</div>
<div class="clear">
</div>
<div id="buttons" class="mt10">
<button type="button" class="button" onclick="dialogBox_close();">close</button>
</div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        // init height
        var height = $(window).height();
        var titleHeight = $('.headingEdit', '#variantsDialog').height();
        var buttonHeight = $('#buttons', '#variantsDialog').height();
        var fixedHeight = height - titleHeight - buttonHeight - 150; // dialog padding
        $('.fixedHeight', '#variantsDialog').height(fixedHeight);
    });

    var searchtimer;
    var searchterm = "";
    // login name availability check
    $('#searchbox').keyup(function () {
        searchterm = $('#searchbox').val();

        if (searchtimer !== undefined) {
            clearTimeout(searchtimer);
        }
        searchtimer = setTimeout(function () {
            $.post("/dashboard/product/variantscontent",{ term: searchterm }, function(result){
                $('.fixedHeight', '#variantsDialog').html(result);
            });
        }, 500);
    });

    $('.fixedHeight', '#variantsDialog').scroll(function () {
        var elem = $(this);
        if (elem[0].scrollHeight - elem.scrollTop() != elem.outerHeight()) {
            // We're not at bottom
            return false;
        }
        $(this).showLoadingBlock();

        // get highest productid
        var maxid = $('.product:last', '#variantsDialog').attr('alt');
        $.post('/dashboard/product/variantscontent', { term: searchterm, sinceid: maxid }, function (result) {
            $(elem).removeLoading();
            $('.fixedHeight', '#variantsDialog').append(result);
            return false;
        });
    });
</script>