<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.transactions.viewmodel.OrderViewModel>" %>

<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<%@ Import Namespace="tradelr.Models.transactions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Purchase Orders
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area relative">
        <div id="addDivHeader" class="hidden fl mt10">
            <h3 id="headingAdd">
                new purchase order
            </h3>
        </div>
        <div id="editDivHeader" class="hidden fl mt10">
            <h3 class="headingEdit">
                edit purchase order
            </h3>
            <%
                if (Model.o.orderStatus != OrderStatus.DRAFT)
                {%>
            <p>
                The network contact will be notified of any changes made.</p>
            <%
                    }%>
        </div>
        <div class="mb10 mt10 fr">
        <button id="viewInvoice" class="small green <%= Model.o.isNew?"hidden":"" %>" type="button">
                view order</button>
            <%
               if (!Model.o.isNew && Model.o.isOwner &&
                   (Model.o.orderStatus == OrderStatus.DRAFT || Model.o.orderStatus == OrderStatus.SENT))
               {%>
            <button id="buttonDelete" class="small orange hidden" type="button">
                delete order</button>
            <% }%>
        </div>
        <div class="clear">
        </div>
        <div class="boxWarning hidden">
            <p id="NoProductWarning" class="hidden">
                None of your network contacts have any public products.
            </p>
            <p id="NoContactWarning" class="hidden">
                You need to add a network contact first before you can send a purchase order. <a
                    href="<%= Url.Action("add","contacts",new{Area = "dashboard"}) %>">Add a contact
                    now</a>
            </p>
        </div>
        <div class="clear">
        </div>
        <% if (Model.o.orderStatus != OrderStatus.DRAFT)
                   {%>
        <div class="order_status" title="order status">
                        <%= Model.o.orderStatus%>
                    </div>
                    <%} %>
        <form id="purchaseAddForm" autocomplete="off" action='<%= Url.Action("Save","Orders", new { Area = "Dashboard", t = "p"}) %>' method="post">
        <div id="order_tabs" class="tabs_clear">
            <ul class="hidden">
                <li><a href="#basic">order</a></li>
                <%if (!Model.o.isNew)
                  { %>
                <li><a href="#history">change history</a></li>
                <% } %>
            </ul>
            <div id="basic" class="hidden">
            
                <div class="fr">
                    <div class="form_entry" style="font-size: larger">
                        <div class="form_label">
                            <label for="orderNumber" class="required">
                                Purchase Order Number</label>
                        </div>
                        <%= Html.TextBox("orderNumber", Model.o.orderNumber.ToString("D8"))%>
                    </div>
                    <div class="form_entry">
                        <div class="form_label">
                            <label for="orderDate" class="required">
                                Purchase Order Date</label>
                        </div>
                        <%=Html.TextBox("orderDate", Model.o.orderDate.HasValue ? Model.o.orderDate.Value.ToString(GeneralConstants.DATEFORMAT_STANDARD)
                                       : "")%>
                        <label class="m0" for="orderDate">
                            <img src='/Content/img/date.png' alt="" /></label>
                    </div>
                    <div class="form_entry">
                        <div class="form_label">
                            <label>
                                Currency</label>
                        </div>
                    <%= Html.DropDownList("currency", Model.currencyList, new Dictionary<string, object>(){{"class","w250px"}}) %>
                    </div>
                </div>
                <div class="fr mr40">
                <div class="form_entry">
                    <div class="form_label">
                        <label for="location">
                            Inventory Location</label>
                    </div>
                    <%= Html.DropDownList("location", Model.locationList) %>
                </div>
            </div>
                <div class="fl w350px h200px">
                    <div class="form_entry">
                        <div class="form_label">
                            <label for="organisation" class="required">
                                Contact Type</label>
                        </div>
                        <%=Html.DropDownList("contact_type", 
                                                Model.ContactTypes, 
                                                !Model.o.isNew ? new Dictionary<string, object>  {{"disabled", "disabled"}}: null
                    )%>
                    </div>
                    <div id="hover_section" class="form_entry">
                        <div class="form_label">
                            <label for="organisation" class="required">
                                <span class="headingNetworkSmall">Contact</span></label>
                                <% if (Model.o.isNew){ %>
                            <a id="hover_section_reveal_inline" class="fr notbold" href="<%= Url.Action("add","contacts",new{Area = "dashboard"}) %>">
                                add contact</a>
                                <%} %>
                        </div>
                        <%
                        if (Model.ContactList.SelectedValue != null && Model.o.orderStatus != OrderStatus.DRAFT)
                        {%>
                    <%= Html.DropDownList("receiverOrgID", Model.ContactList, new Dictionary<string, object> { { "disabled", "disabled" }, { "style", "width:350px" } })%>
                    <% }
                        else
                        {%>
                    <%= Html.DropDownList("receiverOrgID", Model.ContactList, new Dictionary<string, object> { { "style", "width:350px" } })%>
                    <% }%>
                        <div class="pad5" id="contact_address">
                        </div>
                    </div>
                </div>
                <div class="clear">
                </div>
                <div class="form_entry">
                    <div class="form_label">
                        <span class="tip">Start by clicking the SKU cell to select a product.</span>
                    </div>
                    <table id="orderItemsTable" class="orderItemsGrid">
                        <thead>
                            <tr>
                                <th class="sku">
                                    SKU
                                </th>
                                <th class="desc">
                                    Description
                                </th>
                                <th class="unitPrice">
                                    Unit Price
                                </th>
                                <th class="quantity">
                                    Quantity
                                </th>
                                <th class="subtotal">
                                    Subtotal
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <%
                                foreach (var item in Model.o.orderItems)
                                {%>
                            <tr>
                                <td class="sku">
                                    <%=Html.Hidden("itemid", item.id)%>
                                    <span id="sku_text"><%= item.SKU %></span>
                                </td>
                                <td class="desc">
                                    <%= item.description%>
                                </td>
                                <td class="unitPrice">
                                    <%= Html.TextBox("unitPrice", item.UnitPrice.ToString("n" + Model.o.currency.decimalCount))%>
                                </td>
                                <td class="quantity">
                                    <%=Html.TextBox("quantity", item.quantity)%>
                                </td>
                                <td class="subtotal">
                                </td>
                                <td class="del">
                                </td>
                            </tr>
                            <%
                            }%>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan="6" class="pt20">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                                <td colspan="2" class="ar" title="shipping costs if any">
                                    <%= Html.TextBox("shippingMethod", Model.o.shippingMethod, new Dictionary<string, object>(){{"class","w100p"}})%>
                                </td>
                                <td class="shipping">
                                    <%= Html.TextBox("shippingCost", Model.o.shippingCost)%>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" class="total">
                                    Total
                                </td>
                                <td class="total pr4">
                                </td>
                                <td>
                                </td>
                            </tr>
                        </tfoot>
                    </table>
                    <div>
                        <img src='/Content/img/plus_square.png' alt="" />
                        <a id="addNewLine" href="#">add new row</a>
                    </div>
                    <span id="currencyInfo" class="hidden">
                        <%=Model.CurrencyInfo%></span>
                </div>
                <div class="form_entry mt20">
                    <div class="form_label">
                        <label for="terms">
                            Notes</label>
                    </div>
                    <%=Html.TextArea("terms", Model.o.terms)%>
                    <div class="charsleft">
                        <span id="terms-charsleft"></span>
                    </div>
                </div>
            </div>
                <%if (!Model.o.isNew)
                  { %>
            <div id="history" class="hidden pt10">
                <% Html.RenderControl(TradelrControls.changeHistory, Model.ChangeItems);%>
            </div>
            <% } %>
        </div>
        <div class="buttonRow_bottom">
            <span class="mr10">
                <% if (Model.o.isNew)
                   {%>
                <button id="buttonSaveSend" type="button" class="large green ajax">
                    save & send</button>
                <button id="buttonSave" type="button" class="large ajax">
                    save</button>
                <%}
                   else
                   {%>
                <button id="buttonSave" type="button" class="large green ajax">
                    update order</button>
                <%}%>
            </span>
        </div>
        <%= Html.Hidden("id", Model.o.id)%>
        <%= Html.Hidden("newOrder", Model.o.isNew.ToString().ToLower())%>
        <%= Html.Hidden("type", TransactionType.ORDER)%>
        </form>
        <%= Html.Hidden("dp", Model.o.currency.decimalCount)%>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script type="text/javascript">
        function page_init() {
            if ($('#newOrder').val() == 'true') {
                $('#editDivHeader').hide();
                $('#addDivHeader').show();
                $('#receiverOrgID').val('');
            }
            else {
                $('.boxHelp').hide();
                $('#editDivHeader').show();
                $('#addDivHeader').hide();
                $('#buttonDelete').show();
            }
            $('#order_tabs').tabs();
        }

        function contact_display(id) {
            $('#contact_address', '#purchaseAddForm').showLoading();
            $('#contact_address', '#purchaseAddForm').load('/org/address/' + id).fadeIn();
        }

        function currency_display(orgid) {
            $.each(currencyInfo, function (i, val) {
                if (orgid == val.id) {
                    $('#currency').val(val.currencyid);
                    return false;
                }
            });
        }

        function errors_highlight(gridid) {
            var rows = $('tbody > tr', gridid);
            var haveError = false;
            $.each(rows, function (i, val) {
                if ($(this).find('td:eq(1)').text() != '') {

                    // quantity
                    var q = $(this).find('td:eq(3) > input');
                    if (q.val() === '') {
                        q.addClass('curFocus_red').one('click', function () {
                            $(this).removeClass('curFocus_red');
                        }).focus();
                        haveError = true;
                    }

                    // unit price
                    var p = $(this).find('td:eq(2) > input');
                    if (p.val() === '') {
                        p.addClass('curFocus_red').one('click', function () {
                            $(this).removeClass('curFocus_red');
                        }).focus();
                        haveError = true;
                    }
                }
            });
            return haveError;
        }

        var clicked_row = null;
        $('.sku', '#orderItemsTable').live('click', function () {
            var orgid = $('#receiverOrgID').val();
            if (orgid == '') {
                $.jGrowl('Please select a contact');
                $('#receiverOrgID').focus();
                return false;
            }
            clicked_row = $(this).closest('tr');
            dialogBox_open('/dashboard/product/variants', 710);

            return false;
        });

        $('.blockClickable', '#variantsDialog').live('click', function () {
            var parent = $(this).parents('.product');
            var title = [];
            title.push($.trim($('.title', parent).text()));
            $('.attr', this).each(function () {
                var attr = $.trim($(this).text());
                if (attr != '') {
                    title.push();
                }
            });
            var p = {
                title: title.join(' / '),
                price: tradelr.util.stripNonNumeric($('.price', parent).text()),
                sku: $.trim($('.sku', this).text()),
                id: $(this).attr('alt')
            };

            // check if variant already exists
            var found = false;
            $('.sku > input').each(function () {
                if ($(this).val() == p.id) {
                    found = true;
                    return false;
                }
                return true;
            });

            if (found) {
                $.jGrowl("Selected product has already been added");
                return false;
            }

            dialogBox_close();
            return rowPopulate(clicked_row, p);
        });

        var rowBlankInsert = function (grid, count) {
            for (var i = 0; i < count; i++) {
                $(grid).find('tbody').append('<tr alt=""><td class="sku"><input id="itemid" name="itemid" type="hidden" readonly="readonly" /><span id="sku_text">click to select</span></td><td class="desc"></td><td class="unitPrice"><input id="unitPrice" name="unitPrice" type="text" /></td><td class="quantity"><input id="quantity" name="quantity" type="text" /></td><td class="subtotal"></td><td class="del"></td></tr>');
            }
        };

        var handleInsertionOfBlankRows = function (grid) {
            var count = $(grid).find('tbody > tr').length;
            var rowToAppend = 2 - count;
            if (rowToAppend <= 0) {
                return;
            }
            rowBlankInsert(grid, rowToAppend);
        };

        var rowPopulate = function (row, p) {
            $(row).find('#itemid').val(p.id);
            $(row).find('#sku_text').text(p.sku);
            $(row).find('.desc').html(p.title);
            // unit price
            $(row).find('.unitPrice > input').val(p.price);
        };

        var priceUpdate = function (row, context) {
            var price = $(row, context).find('#unitPrice').val();
            var quantity = $(row, context).find('#quantity').val();
            if (price !== "" && quantity !== "") {
                var subtotal = tradelr.util.stripNonNumeric(price) * quantity;
                $(row, context).find('.subtotal').html(tradelr.util.tomoneystring(subtotal, dp));
                totalUpdate(context);
            }
        };

        var rowInit = function (row, gridid) {
            // append delete button
            var id = $(row).attr('alt');
            var delb = "<span class='delIcon' title='Delete Row' ></span>";
            $(row).find('td:last').append(delb).click(function () {
                var ok = window.confirm('Delete Row?');
                if (!ok) {
                    return false;
                }
                $(this).parents('tr').fadeOut(function () {
                    $(this).remove();
                });
                return totalUpdate(gridid);
            });

            // calculate subtotal
            if (id != "") {
                priceUpdate(row, gridid);
            }

            // change events
            $(row).find('.unitPrice > input').bind("keyup", function () {
                priceUpdate(row, gridid);
            });

            $(row).find('.quantity > input').bind("keyup", function () {
                var inputval = parseInt($(this).val(), 10);
                if (inputval == 0) {
                    $(this).val(1);
                }
                priceUpdate(row, gridid);
            }).numeric();
        };

        var tableInit = function (gridid) {
            handleInsertionOfBlankRows(gridid);
            // go through each row
            var rows = $(gridid).find('tbody > tr');
            $.each(rows, function () {
                rowInit(this, gridid);
            });

            // init input highlighters
            inputSelectors_init();
            inputSelectors_uninit(gridid);

            // init field highlighter
            $('input', gridid).live('click', function () {
                $(gridid).find('input,textarea').parent().removeClass('curFocus_bg');
                $(this).parent().addClass('curFocus_bg');
            });
            $('textarea', gridid).live('click', function () {
                $(gridid).find('input,textarea').parent().removeClass('curFocus_bg');
                $(this).parent().addClass('curFocus_bg');
            });
        };

        var totalUpdate = function (gridid) {
            var rows = $(gridid).find('tbody > tr');
            var total = 0;
            // sum up all subtotals
            $.each(rows, function (i, val) {
                total += Number(tradelr.util.stripNonNumeric($(this).find('.subtotal').html()));
            });

            // add shipping cost
            total += Number(tradelr.util.stripNonNumeric($('#shippingCost', gridid).val()));

            if (total != 0) {
                // set total
                $('tfoot > tr:last', gridid).find('td:eq(1)').html(tradelr.util.tomoneystring(total, dp));
            }
            else {
                $('tfoot > tr:last', gridid).find('td:eq(1)').html('');
            }
        };

        function table_clear() {
            $('tbody', '#orderItemsTable').html('');
            rowBlankInsert('#orderItemsTable', 2);

            // go through each row
            var rows = $('#orderItemsTable').find('tbody > tr');
            $.each(rows, function () {
                rowInit(this, '#orderItemsTable');
            });

            totalUpdate('#orderItemsTable');

            $('#contact_address', '#purchaseAddForm').html('');
        }

        var dp;
        var currencyInfo;
        var sendOrder = false;
        $(document).ready(function () {
            $('#navpurchases').addClass('navselected_white');
            // check if product has already been added, otherwise disable save button
            currencyInfo = $.evalJSON($('#currencyInfo').text());
            dp = parseInt($('#dp').val(), 10);
            page_init();

            tableInit('#orderItemsTable');
            
            // init autogrow
            init_autogrow('#purchaseAddForm', 30);

            var contactid = querySt('contact');
            if (contactid) {
                $('#receiverOrgID').val(contactid);
            }

            if ($('#receiverOrgID').val() != '') {
                contact_display($('#receiverOrgID').val());
                currency_display($('#receiverOrgID').val());
            }

            $('#contact_type').change(function () {
                $.post('/dashboard/contacts/list', { type: $(this).val() }, function (json_result) {
                    if (json_result.success) {
                        $('#receiverOrgID').html('');
                        table_clear();
                        $.each(json_result.data, function () {
                            $("<option value='" + this.Value + "'>" + this.Text + "</option>").appendTo('#receiverOrgID');
                        });
                    }
                    else {
                        $.jGrowl(json_result.message);
                    }

                });
            });

            $('#shippingMethod').watermark('Shipping Method (if known)');
            $('#terms').limit('1000', '#terms-charsleft');

            // invoice number validity check
            $('#invoiceOrOrderNumber,#orderNumber').numeric();

            $('#buttonSave', '#purchaseAddForm').click(function () {
                $(this).buttonDisable();
                $('#purchaseAddForm').trigger('submit');
            });
            $(window).trackUnsavedChanges('#buttonSave');
            $('#buttonSaveSend', '#purchaseAddForm').click(function () {
                $(this).buttonDisable();
                sendOrder = true;
                $('#purchaseAddForm').trigger('submit');
            });
            $(window).trackUnsavedChanges('#buttonSaveSend');

            $('#addNewLine').click(function () {
                rowBlankInsert('#orderItemsTable', 1);
                var row = $('tbody > tr:last', '#orderItemsTable');
                rowInit(row, '#orderItemsTable');
                return false;
            });

            $('#buttonDelete').click(function () {
                var ok = window.confirm('Delete this purchase order?');
                if (!ok) {
                    return false;
                }
                var id = $('#id').val();
                if (id == '') {
                    $.jGrowl('Purchase Order has not been saved.');
                    return false;
                }

                $.ajax({
                    type: "POST",
                    url: "/dashboard/orders/delete/" + id,
                    dataType: "json",
                    success: function (json_data) {
                        if (json_data.success) {
                            $.jGrowl('Purchase Order successfully deleted');
                            window.location = '<%= Url.Action("Index","Transactions") %>';
                        }
                        else {
                            if (json_data.data == tradelr.returncode.NOPERMISSION) {
                                $.jGrowl('You do not have permission to delete purchase orders');
                            }
                            else {
                                $.jGrowl(json_data.message);
                            }
                        }
                        return false;
                    }
                }); // ajax
                return false;
            });

            // init view invoice
            $('#viewInvoice').click(function () {
                window.location = "/dashboard/orders/" + $('#id', '#purchaseAddForm').val();
                return false;
            });

            // redirect to add contact page if no contacts
            if ($('#receiverOrgID > option').length === 1) {
                $('.boxWarning').show();
                $('#NoContactWarning').show();
            }

            // init ajax contact address loading
            $('#receiverOrgID', '#purchaseAddForm').bind('change select keyup', function () {
                var selectedVal = $(this).val();
                if (selectedVal !== "" && parseInt(selectedVal, 10) > 0) {
                    contact_display(selectedVal);
                    currency_display(selectedVal);
                }
                table_clear();
                return true;
            });

            // init datepickers
            $("#orderDate", '#purchaseAddForm').datepicker(
            {
                dateFormat: 'D, d M yy'
            });

            // only init
            if ($('#orderDate', '#purchaseAddForm').val() === '') {
                $('#orderDate', '#purchaseAddForm').datepicker("setDate", new Date());
            }
            $("#orderDate", '#purchaseAddForm').attr('readonly', 'readonly');

            $("#expectedDate", '#purchaseAddForm').datepicker(
            {
                dateFormat: 'D, d M yy'
            });

            //////////////////// form submit //////////////////////       
            $('#purchaseAddForm').submit(function () {
                var ok = $('#purchaseAddForm').validate({
                    invalidHandler: function (form, validator) {
                        $(validator.invalidElements()[0]).focus();
                    },
                    focusInvalid: false,
                    rules: {
                        receiverOrgID: {
                            required: true
                        },
                        orderDate: {
                            required: true
                        }
                    }
                }).form();
                if (!ok) {
                    $('#buttonSave', '#purchaseAddForm').buttonEnable();
                    $('#buttonSaveSend', '#purchaseAddForm').buttonEnable();
                    return false;
                }

                // check that order items are valid
                var haveError = errors_highlight('#orderItemsTable');
                if (haveError) {
                    $('#buttonSave', '#purchaseAddForm').buttonEnable();
                    $('#buttonSaveSend', '#purchaseAddForm').buttonEnable();
                    $.jGrowl("Some fields are missing");
                    return false;
                }

                // save all order items row
                var action = $(this).attr("action");
                var serialized = $(this).serialize();

                // post form
                $.ajax({
                    type: "POST",
                    url: action,
                    dataType: "json",
                    data: serialized,
                    success: function (json_data) {
                        if (json_data.success) {
                            if ($('#newOrder').val() == 'true') {
                                // new order
                                $('#viewInvoice').show();
                                $('#id').val(json_data.data);
                                if (sendOrder) {
                                    dialogBox_open('/dashboard/orders/email/' + json_data.data + "?t=p", 500);
                                }
                                else {
                                    $.jGrowl('Order saved');
                                    scrollToTop();
                                }
                                $('#buttonSaveSend').hide();
                                $('#buttonSave').text('update order').addClass('green');
                            }
                            else {
                                $.jGrowl('Purchase order successfully updated');
                                scrollToTop();
                            }
                            $('#newOrder').val('false');
                            page_init();
                        }
                        else {
                            $.jGrowl(json_data.message);
                        }
                        $('#buttonSave', '#purchaseAddForm').buttonEnable();
                    }
                });
                return false;
            });
        });
    </script>
</asp:Content>
