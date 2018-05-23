<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.products.viewmodel.VariantsContentOptions>" %>
<div id="pricingDialog">
<h3 class="headingEdit fl">
    Product Group Pricing</h3>
    <div id="search_area" class="fr">
    <span class="search"></span>
                    <input type="text" id="searchbox" name="searchbox" class="searchbox" />
    </div>
    <div class="clear pt5"></div>
<div class="fixedHeight scroll_y">
    <%
        Html.RenderAction("productPricingContent",new { term = Model.term, sinceid = Model.sinceid });%>
</div>
<div class="clear">
</div>
<%= Html.Hidden("groupid", Model.groupid) %>
<div id="buttons" class="mt10">
<button type="button" class="button green ajax" id="buttonSave" type="button">save</button>
<button type="button" class="button" onclick="dialogBox_close();">cancel</button>
</div>
</div>
<script type="text/javascript">
    function pricing_initboxes() {
        $('#groupPrice', '#pricingDialog').watermark('enter group price');
        $('#groupPrice', '#pricingDialog').numeric({ allow: '.' });
    }

    $(document).ready(function () {
        // init height
        var height = $(window).height();
        var titleHeight = $('.headingEdit', '#pricingDialog').height();
        var buttonHeight = $('#buttons', '#pricingDialog').height();
        var fixedHeight = height - titleHeight - buttonHeight - 150; // dialog padding
        $('.fixedHeight', '#pricingDialog').height(fixedHeight);
        pricing_initboxes();
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
            $.post("/dashboard/group/productPricingContent",{ term: searchterm }, function(result){
                $('.fixedHeight', '#pricingDialog').html(result);
            });
        }, 500);
    });

    $('.fixedHeight', '#pricingDialog').scroll(function () {
        var elem = $(this);
        if (elem[0].scrollHeight - elem.scrollTop() != elem.outerHeight()) {
            // We're not at bottom
            return false;
        }
        $(this).showLoadingBlock();

        // get highest productid
        var maxid = $('.product:last', '#pricingDialog').attr('alt');
        $.post('/dashboard/group/productPricingContent', { term: searchterm, sinceid: maxid }, function (result) {
            $(elem).removeLoading();
            $('.fixedHeight', '#pricingDialog').append(result);
            pricing_initboxes();
            return false;
        });
    });

    $('#buttonSave', '#pricingDialog').click(function () {
        // find prices that have been set
        var products = $('.product', '#pricingDialog');
        var pricings = [];
        $.each(products, function () {
            var price = $('#groupPrice', this).val();
            if (!isNaN(parseFloat(price))) {
                var id = $(this).attr('alt');
                pricings.push({ id: id, price: price });
            }
        });

        if (pricings.length == 0) {
            $.jGrowl('No prices were entered');
            return false;
        }
        var postdata = {
            prices: pricings,
            groupid: $('#groupid').val()
        };

        // post
        $.ajax({
            contentType: 'application/json',
            type: 'POST',
            url: '/dashboard/group/pricingadd',
            data: $.toJSON(postdata),
            dataType: 'json',
            success: function (json_data) {
                if (json_data.success) {
                    reloadGroupPricingGrid(getFilterByField());
                    dialogBox_close();
                }
                else {
                    $.jGrowl(json_data.message);
                }
                return false;
            }
        });
        return false;
    });
</script>