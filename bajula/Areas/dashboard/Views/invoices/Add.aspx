<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.transactions.viewmodel.OrderViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<%@ Import Namespace="tradelr.Models.transactions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Invoice
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area relative">
    <div id="addDivHeader" class="mt10 hidden fl">
                <h3 id="headingAdd">
                    new invoice
                </h3>
            </div>
            <div id="editDivHeader" class="mt10 hidden fl">
                <h3 class="headingEdit">
                    edit invoice
                </h3>
                <% if (Model.o.orderStatus != OrderStatus.DRAFT)
                   {%>
                <p>
                    The customer will be notified of any changes made.</p>
                <%} %>
            </div>
            <div class="mb10 mt10 fr">
            <button id="viewInvoice" class="small green <%= Model.o.isNew?"hidden":"" %>" type="button">
                    view invoice
                </button>
                <%
                   if (Model.o.isOwner &&
                       (Model.o.orderStatus == OrderStatus.DRAFT || Model.o.orderStatus == OrderStatus.SENT))
                   {%>
                <button id="buttonDelete" class="small orange" type="button">
                    delete invoice</button>
                <% }%>
            </div>
            <div class="clear"></div>
            <div class="boxWarning hidden">
                <p id="NoProductWarning" class="hidden">
                    A product needs to be added first before you can create an invoice. <a href="<%= Url.Action("Add","Product", new { Area = "Dashboard"}) %>">
                        Add a product now</a>
                </p>
                <p id="NoContactWarning" class="hidden">
                    You need to add a contact first before you can create an invoice. <a href="<%= Url.Action("Add","Contacts", new { Area = "Dashboard"}) %>">
                        Add a contact now</a>
                </p>
            </div>
            <div class="clear">
            </div>
            <% if (Model.o.orderStatus != OrderStatus.DRAFT)
               {%>
            <div class="order_status" title="invoice status">
                        <%=Model.o.orderStatus%>
                    </div>
                    <% } %>
            <form autocomplete="off" id="salesInvoiceAddForm" action="<%= Url.Action("save","orders", new { Area = "dashboard"}) %>" method="post">
            <div id="order_tabs" class="tabs_clear">
            <ul class="hidden">
                <li><a href="#basic">invoice</a></li>
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
                            Invoice Number</label>
                    </div>
                    <%= Html.TextBox("orderNumber", Model.o.orderNumber.ToString("D8"))%>
                </div>
                <div class="form_entry">
                    <div class="form_label">
                        <label for="orderDate" class="required">
                            Invoice Date</label>
                    </div>
                    <%= Html.TextBox("orderDate", Model.o.orderDate.HasValue ? Model.o.orderDate.Value.ToString(GeneralConstants.DATEFORMAT_STANDARD) : "")%>
                    <label class="m0" for="orderDate">
                        <img src='/Content/img/date.png' alt="" /></label>
                </div>
                <div class="form_entry">
                    <div class="form_label">
                        <label for="discount">
                            Discount</label>
                    </div>
                    <input type="text" value="<%= Model.o.discount %>" id="discount" name="discount" class="w150px" />
                    <select id="discountType" name="discountType" class="w60px">
                        <option value="<%= Model.o.currency.code %>"><%= Model.o.currency.code %></option>
                    <option value="%" <%= Model.o.discountType == "%"?"selected='selected'":"" %>>%</option>
                    </select>
                    <div class="smaller"><%= Model.o.discountCode %></div>
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
                <div id="hover_section" class="form_entry">
                    <div class="form_label">
                        <label for="organisation" class="required">
                            <span class="icon_contact">Contact</span></label>
                        <a id="hover_section_reveal_inline" class="fr notbold" href="<%= Url.Action("add","contacts",new{Area = "dashboard"}) %>">add contact</a>
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
                <table id="invoiceItemsTable" class="orderItemsGrid">
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
                            <th class="tax">
                                Tax (%)
                            </th>
                            <th class="quantity">
                                Quantity
                            </th>
                            <th class="subtotal">
                                Subtotal 
                            </th>
                            <th class="del">
                                &nbsp;
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (var item in Model.o.orderItems)
                           {%>
                        <tr alt="<%= item.id %>">
                            <td class="sku al">
                                <%=Html.Hidden("itemid", item.id)%>
                                <span id="sku_text"><%= item.SKU %></span>
                            </td>
                            <td class="desc">
                                <%= item.description%>
                            </td>
                            <td class="unitPrice">
                                <%= Html.TextBox("unitPrice", item.UnitPrice.ToString("n" + Model.o.currency.decimalCount))%>
                            </td>
                            <td class="tax">
                                <%= Html.TextBox("tax",item.tax)  %>
                            </td>
                            <td class="quantity">
                                <%= Html.TextBox("quantity", item.quantity)  %>
                            </td>
                            <td class="subtotal">
                            </td>
                            <td class="del">
                                &nbsp;
                            </td>
                        </tr>
                        <% } %>
                    </tbody>
                    <tfoot>
                    <tr><td colspan="7" class="pt20">&nbsp;</td></tr>
                        <tr>
                        <td colspan="3"></td>
                            <td colspan="2" class="ar" title="shipping costs if any">
                                <%= Html.TextBox("shippingMethod", Model.o.shippingMethod)%>
                            </td>
                            <td class="shipping">
                                <%= Html.TextBox("shippingCost", Model.o.shippingCost)%>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="5" class="total">
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
            </div>
            <div class="form_entry mt20">
                <div class="form_label">
                    <label for="terms">
                        Payment Terms</label><span id="setTerms" class="tip_inline">(<a href="javascript:return false;">set
                            default terms</a>)</span> <span class="tip">Payment terms for this invoice.</span>
                </div>
                <%= Html.TextArea("terms", Model.o.terms)%>
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
                    <img src="/Content/img/save.png" alt='' />
                    update</button>
                <%}%>
                </span>
            </div>
            <%= Html.Hidden("type", TransactionType.INVOICE)%>
            <%= Html.Hidden("id", Model.o.id)%>
            <%= Html.Hidden("newOrder", Model.o.isNew.ToString().ToLower())%>
            <%= Html.Hidden("limit", Model.LimitHit)%>
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
           }
           else {
               $('.boxHelp').hide();
               $('#editDivHeader').show();
               $('#addDivHeader').hide();
           }

           $('#order_tabs').tabs();
       }

       function contact_display(id) {
           $('#contact_address', '#salesInvoiceAddForm').showLoading();
           $('#contact_address', '#salesInvoiceAddForm').load('/org/address/' + id).fadeIn();
       }

       function errors_highlight(gridid) {
           var rows = $('tbody > tr', gridid);
           var haveError = false;
           $.each(rows, function (i, val) {
               if ($(this).find('td:eq(1)').text() !== '') {

                   // quantity
                   var q = $(this).find('td:eq(4) > input');
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

       function row_blank_insert(grid, count) {
           for (var i = 0; i < count; i++) {
               $(grid).find('tbody').append('<tr alt=""><td class="sku"><input id="itemid" name="itemid" type="hidden" readonly="readonly" /><span id="sku_text">click to select</span></td><td class="desc"></td><td class="unitPrice"><input id="unitPrice" name="unitPrice" type="text" /></td><td class="tax"><input id="tax" name="tax" type="text" /></td><td class="quantity"><input id="quantity" name="quantity" type="text" /></td><td class="subtotal"></td><td class="del"></td></tr>');
           }
       }

       function handleInsertionOfBlankRows(grid) {
           var count = $(grid).find('tbody > tr').length;
           var rowToAppend = 2 - count;
           if (rowToAppend <= 0) {
               return;
           }
           row_blank_insert(grid, rowToAppend);
       }

       function price_update(row, context) {
           var price = $(row, context).find('#unitPrice').val();
           var tax = $(row, context).find('#tax').val(); ;
           var quantity = $(row, context).find('#quantity').val();
           if (price !== "" && quantity !== "") {
               var subtotal = tradelr.util.stripNonNumeric(price) * quantity;
               if (tax !== "") {
                   var rate = parseFloat(tax);
                   subtotal = (1 + rate / 100) * subtotal;
               }

               $(row, context).find('.subtotal').html(tradelr.util.tomoneystring(subtotal, dp));
               total_update(context);
           }
       }

       function total_update(gridid) {
           var rows = $(gridid).find('tbody > tr');
           var total = 0;
           // sum up all subtotals
           $.each(rows, function (i, val) {
               total += Number(tradelr.util.stripNonNumeric($(this).find('.subtotal').html()));
           });

           // add shipping cost
           total += Number(tradelr.util.stripNonNumeric($('#shippingCost', gridid).val()));

           if (total != 0) {
               // apply discount if set
               var discount = parseFloat($('#discount').val());
               if (!isNaN(discount) && discount != '') {
                   // get type
                   if ($('#discountType').val() == '%') {
                       discount = (1 - discount / 100);
                       total = total * discount;
                   }
                   else {
                       total = Math.max(0, total - discount);
                   }
               }

               // set total
               $('tfoot > tr:last', gridid).find('td:eq(1)').html(tradelr.util.tomoneystring(total, dp));
           }
       }

        function row_populate(row, p) {
            $(row).find('#itemid').val(p.id);
            $(row).find('#sku_text').text(p.sku);
            $(row).find('.desc').html(p.title);
            
            // unit price
            $(row).find('.unitPrice > input').val(p.price);
            // tax
            $(row).find('.tax > input').val(p.tax);
        }

       function row_init(row, gridid) {
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
               total_update(gridid);
           });

           // calculate subtotal
           if (id != "") {
               price_update(row, gridid);
           }

           // change events
           $(row).find('.unitPrice > input').bind("keyup", function () {
               price_update(row, gridid);
           });
           $(row).find('.tax > input').bind("keyup", function () {
               price_update(row, gridid);
           }).numeric({ allow: '.' });

           $(row).find('.quantity > input').bind("keyup", function () {
               var inputval = parseInt($(this).val(), 10);
               if (inputval == 0) {
                   $(this).val(1);
               }
               price_update(row, gridid);
           }).numeric();
       }

        var clicked_row = null;
       $('.sku', '#invoiceItemsTable').live('click', function () {
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
           row_populate(clicked_row, p);
       });

       function table_init(gridid) {
           handleInsertionOfBlankRows(gridid);
           // go through each row
           var rows = $(gridid).find('tbody > tr');
           $.each(rows, function () {
               row_init(this, gridid);
           });

           // calculate total
           total_update(gridid);

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

           // bind shipping amount
           $('#shippingCost', gridid).bind('keyup', function () {
               total_update(gridid);
           }).numeric({ allow: '.' });

           $('#discount').keyup(function () {
               total_update(gridid);
           }).numeric({ allow: '.' });

           $('#discountType').change(function () {
               total_update(gridid);
           });
       }

       var dp;
       var sendOrder = false;
       $(document).ready(function () {
           $('#navsales').addClass('navselected_white');
           // check if product has already been added, otherwise disable save button
           dp = parseInt($('#dp').val(), 10);
           page_init();

           // check if limit has been hit
           if ($('#limit').val() == 'True') {
               dialogBox_show('You have exceeded the number of invoices you can send for your plan. Please upgrade your <a href="/dashboard/account/plan">plan</a>.');
           }

           // handle table
           table_init('#invoiceItemsTable');

           // init autogrow
           init_autogrow('#salesInvoiceAddForm', 30);

           // bind chars left
           $('#terms').limit('1000', '#terms-charsleft');

           var contactid = querySt('contact');
           if (contactid) {
               $('#receiverOrgID').val(contactid);
           }
           if ($('#receiverOrgID').val() != '') {
               contact_display($('#receiverOrgID').val());
           }

           // limit characters
           $('#shippingMethod').watermark('Shipping Method');
           $('#shippingCost').numeric({ allow: '.' });

           $('#buttonSaveSend', '#salesInvoiceAddForm').click(function () {
               $(this).buttonDisable();
               sendOrder = true;
               $('#salesInvoiceAddForm').trigger('submit');
           });
           $(window).trackUnsavedChanges('#buttonSaveSend');

           $('#buttonSave', '#salesInvoiceAddForm').click(function () {
               $(this).buttonDisable();
               $('#salesInvoiceAddForm').trigger('submit');
           });
           $(window).trackUnsavedChanges('#buttonSave');

           $('#addNewLine').click(function () {
               row_blank_insert('#invoiceItemsTable', 1);
               var row = $('tbody > tr:last', '#invoiceItemsTable');
               row_init(row, '#invoiceItemsTable');
               init_autogrow('#salesInvoiceAddForm', 30);
               return false;
           });

           $('#buttonDelete').click(function () {
               var ok = window.confirm('Delete this invoice?');
               if (!ok) {
                   return false;
               }
               var id = $('#id').val();
               if (id == '') {
                   $.jGrowl('Invoice has not been saved.');
                   return false;
               }

               $.ajax({
                   type: "POST",
                   url: "/dashboard/invoices/delete/" + id,
                   dataType: "json",
                   success: function (json_data) {
                       if (json_data.success) {
                           $.jGrowl('Invoice successfully deleted');
                           window.location = '/dashboard/transactions';
                       }
                       else {
                           if (json_data.data == tradelr.returncode.NOPERMISSION) {
                               $.jGrowl('You do not have permission to delete invoices');
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

           // init set default terms
           $('#setTerms').click(function () {
               dialogBox_open('/dashboard/invoices/terms', 500);
               return false;
           });

           // init view invoice
           $('#viewInvoice').click(function () {
               window.location = "/dashboard/invoices/" + $('#id', '#salesInvoiceAddForm').val();
               return false;
           });

           // init ajax contact address loading
           $('#receiverOrgID', '#salesInvoiceAddForm').bind('change select keyup', function () {
               var selectedVal = $(this).val();
               if (selectedVal !== "" && parseInt(selectedVal, 10) > 0) {
                   contact_display(selectedVal);
               }
               return true;
           });

           // redirect to add contact page if no contacts
           if ($('#receiverOrgID > option').length === 1) {
               $('.boxWarning').show();
               $('#NoContactWarning').show();
           }

           // handle dates
           $("#orderDate", '#salesInvoiceAddForm').datepicker(
        {
            dateFormat: 'D, d M yy'
        });
           $("#orderDate", '#salesInvoiceAddForm').attr('readonly', 'readonly');

           // handle submit
           $.validator.addMethod("percent", function (value, element) {
               return this.optional(element) || (parseFloat(value, 10) < 100);
           }, 'must be less than 100%');
           
           //////////////////// form submit //////////////////////     
           $('#salesInvoiceAddForm').submit(function () {
               var ok = $('#salesInvoiceAddForm').validate({
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
                       },
                       tax: {
                           percent: true
                       }
                   }
               }).form();
               if (!ok) {
                   $('#buttonSave', '#salesInvoiceAddForm').buttonEnable();
                   $('#buttonSaveSend', '#salesInvoiceAddForm').buttonEnable();
                   return false;
               }

               // check that order items are valid
               var haveError = errors_highlight('#invoiceItemsTable');
               if (haveError) {
                   $('#buttonSave', '#salesInvoiceAddForm').buttonEnable();
                   $('#buttonSaveSend', '#salesInvoiceAddForm').buttonEnable();
                   $.jGrowl("Some fields are missing");
                   return false;
               }

               // encode
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
                               $('#viewInvoice').show();
                               $('#id').val(json_data.data);
                               if (sendOrder) {
                                   dialogBox_open('/dashboard/orders/email/' + json_data.data + "?t=s", 500);
                               }
                               else {
                                   $.jGrowl('Invoice saved');
                                   scrollToTop();
                               }
                               $('#buttonSaveSend').hide();
                               $('#buttonSave').text('update order').addClass('green');
                           }
                           else {
                               $.jGrowl('Invoice successfully updated');
                               scrollToTop();
                           }
                           $('#newOrder').val('false');
                           page_init();
                       }
                       else {
                           $.jGrowl(json_data.message);
                       }
                       $('#buttonSave', '#salesInvoiceAddForm').buttonEnable();
                   }
               }); // ajax
               return false;
           }); // submit
       }); // ready
   </script>
</asp:Content>
