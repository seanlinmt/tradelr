<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<tradelr.Models.coupons.CouponBasic>>" %>
<div id="couponSelect">
        <h3>select discount coupon</h3>
        <div class="mt10 mb10 ml10">
        <a class="icon_add" href="#" onclick="dialogBox_close();dialogBox_open('/dashboard/coupons/add', 600);return false;">create new coupon</a>
        </div>
        <div class="fixedHeight scroll_y">
    <%
        foreach (var coupon in Model)
        {
    %>
    <div class="blockClickable">
        <div class="content">
            <h4>
                <%= coupon.code%></h4>
            <%= coupon.description %>
        </div>
        <div class="clear">
        </div>
    </div>
    <%  } %>
</div>
<div class="mt10">
<button type="button" onclick="dialogBox_close();">cancel</button>
</div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        // init height
        var height = $(window).height();
        var titleHeight = $('#headingAdd', '#couponSelect').height();
        var fixedHeight = height - titleHeight - 250; // dialog padding
        if ($('.fixedHeight', '#couponSelect').height() > fixedHeight) {
            $('.fixedHeight', '#couponSelect').height(fixedHeight);
        }

        $('.blockClickable', '#couponSelect').click(function () {
            var couponcode = $.trim($('h4', this).html());
            $('#facebookCoupon').val(couponcode);
            dialogBox_close();
            return false;
        });
    });
</script>