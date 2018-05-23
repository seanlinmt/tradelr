<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.contacts.viewmodel.ContactListViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div class="content_filter">
                <div id="contactsList" class="filter">
                    <h4>
                    <img src="/Content/img/contacts/contact_group.png" />
                        contact groups</h4>
                    <div class="sideboxEntry selected">
                        <strong>All</strong><div class="addmain">
                        </div>
                    </div>
                    <%= Html.FilterBoxList("contactsList", Model.contactGroups)%>
                </div>
            </div>
            <div class="main_columnright">
                <div id="grid_content">
                    <div class="buttonRow">
                    <button id="addProduct" disabled="disabled" class="small green" title="add products to this group">
                    add products</button>
                    <button id="removeProduct" disabled="disabled" class="small white" title="remove product from this group">
                            remove</button>
                        <div class="clear">
                        </div>
                    </div>
                    <table id="groupPricingGridView" class="scroll">
                    </table>
                    <div id="groupPricingNavigation" class="scroll ac">
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
            <span id="filterBy" class='hidden'></span>
           <script type="text/javascript">
               $(document).ready(function () {
                   // bind side filter click event
                   $('#contactsList', '#group_pricing').find('.sideboxEntry').live("click", function () {

                       $('#contactsList', '#group_pricing').find('.sideboxEntry').removeClass('selected');
                       $(this).addClass('selected');
                       var fid = '';
                       if ($(this).attr('fid') != undefined) {
                           fid = $(this).attr('fid');
                           $('#addProduct', '.buttonRow').attr('disabled', false);
                           $('#removeProduct', '.buttonRow').attr('disabled', false);
                       }
                       else {
                           $('#addProduct', '.buttonRow').attr('disabled', true);
                           $('#removeProduct', '.buttonRow').attr('disabled', true);
                       }
                       setFilterByField(fid);
                       reloadGroupPricingGrid(getFilterByField());
                   });

                   $('#addProduct', '#group_pricing').click(function () {
                       var groupid = $('#contactsList .selected', '#group_pricing').attr('fid');
                       if (groupid == "") {
                           $.jGrowl('Select a group first');
                           return false;
                       }
                       dialogBox_open('/dashboard/group/productPricing/' + groupid, 750);
                       return false;
                   });

                   $('#removeProduct', '#group_pricing').click(function () {
                       var groupid = $('#contactsList .selected', '#group_pricing').attr('fid');
                       if (groupid == "") {
                           $.jGrowl('Select a group first');
                           return false;
                       }

                       if ($('.selected-row', '#groupPricingGridView').length == 0) {
                           $.jGrowl('No products selected');
                           return false;
                       }

                       var ok = window.confirm('Are you sure? There is no UNDO.');
                       if (!ok) {
                           return false;
                       }

                       var ids = [];
                       var entries = $('.selected-row', '#groupPricingGridView');
                       $.each(entries, function () {
                           var id = $(this).attr('id');
                           ids.push(id);
                       });
                       $.ajax({
                           type: 'POST',
                           url: '/dashboard/group/pricingdelete/' + groupid,
                           data: { productids: ids.join(',') },
                           dataType: 'json',
                           success: function (json_data) {
                               if (json_data.success) {
                                   var ids = json_data.data;
                                   if (ids.length != 0) {
                                       // remove deleted products
                                       reloadGroupPricingGrid(getFilterByField());
                                   }
                                   else {
                                       $.jGrowl(json_data.message);
                                   }
                               }
                               else {
                                   $.jGrowl(json_data.message);
                               }
                               return false;
                           }
                       });
                       return false;
                   });
               });
           </script>
