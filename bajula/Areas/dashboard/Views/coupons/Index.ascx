<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div class="content_filter">
<div class="div_action">
        <a id="addCoupon" href="#"><span class="action_add">
            new coupon
        </span></a>
    </div>
    
    <div id="couponsTimeLine" class="filter">
        <h4>
            show</h4>
        <div class="sideboxEntry selected">
            All</div>
    </div>
    <a class="icon_help" target="_blank" href="http://www.tradelr.com/help/dashboard/marketing">need help?</a>
</div>
<div class="main_columnright">
    <div id="grid_content">
        <div class="buttonRow">
            <div id="search_area" class="fr">
    <span class="search"></span>
                    <input type="text" name="searchbox" id="searchInput" class="searchbox" />
    </div>
        </div>
        <table id="couponsGridView" class="scroll">
        </table>
        <div id="couponsGridNavigation" class="scroll" style="text-align: center;">
        </div>
    </div>
</div>
<span id="filterBy" class="hidden"></span>
<script type="text/javascript">
    $(document).ready(function () {

        $('#searchInput', '#marketing_coupons').keyup(function (e) {
            if (!isEnterKey(e)) {
                return;
            }
            searchCoupons($(this).val());
        });
    });

    $('#addCoupon').live('click', function () {
        dialogBox_open('/dashboard/coupons/add', 600);
        return false;
    });
</script>
