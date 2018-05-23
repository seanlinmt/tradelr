<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.contacts.viewmodel.ContactListViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div class="content_filter">
                <div id="sideSelected" class="hidden bt bl bb pl4 mb10">
                </div>
                <div id="typesList" class="filter">
                    <h4>
                    <img src="/Content/img/contacts/contact.png" />
                        contact types</h4>
                    <div class="sideboxEntry selected">
                        <strong>All</strong></div>
                    <%= Html.FilterBoxList("contactTypes", Model.contactTypes)%>
                </div>
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
                    <div class="buttonRow" style="height: 50px;">
                    <button class="small green" title="add new contact" onclick="javascript:window.location = '/dashboard/contacts/add';">new contact</button>
                    <button class="small white" title="import contacts" onclick="javascript:window.location = '/dashboard/contacts/import';">import</button>
                    <button id="deleteContact" class="small white" title="delete selected contacts">
                            delete</button>
                            <div id="search_area" class="fr">
    <span class="search"></span>
                    <input type="text" name="searchbox" id="ContactsSearchBox" class="searchbox" />
    </div>
                        <div class="mt4">
                        </div>
                        <div id="sortrow" class="fl">
                            <button class="small white">
                                A</button>
                            <button class="small white">
                                B</button>
                            <button class="small white">
                                C</button>
                            <button class="small white">
                                D</button>
                            <button class="small white">
                                E</button>
                            <button class="small white">
                                F</button>
                            <button class="small white">
                                G</button>
                            <button class="small white">
                                H</button>
                            <button class="small white">
                                I</button>
                            <button class="small white">
                                J</button>
                            <button class="small white">
                                K</button>
                            <button class="small white">
                                L</button>
                            <button class="small white">
                                M</button>
                            <button class="small white">
                                N</button>
                            <button class="small white">
                                O</button>
                            <button class="small white">
                                P</button>
                            <button class="small white">
                                Q</button>
                            <button class="small white">
                                R</button>
                            <button class="small white">
                                S</button>
                            <button class="small white">
                                T</button>
                            <button class="small white">
                                U</button>
                            <button class="small white">
                                V</button>
                            <button class="small white">
                                W</button>
                            <button class="small white">
                                X</button>
                            <button class="small white">
                                Y</button>
                            <button class="small white">
                                Z</button>
                        </div>
                        
                        <div class="clear">
                        </div>
                    </div>
                    <table id="contactsGridView" class="scroll">
                    </table>
                    <div id="contactNavigation" class="scroll" style="text-align: center;">
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
            <%= Html.Hidden("ContactFilterBy")%>
            <%= Html.Hidden("ContactFilterByType")%>
            <%= Html.Hidden("ContactFilterByLetter")%>
            <script type="text/javascript">
                $(document).ready(function () {
                    $('#ContactsSearchBox').keyup(function (e) {
                        if (!isEnterKey(e)) {
                            return;
                        }
                        reloadContactsGrid();
                    });

                    // bind side filter click event
                    $('#contactsList', '#contacts_mine').find('.sideboxEntry').live("click", function () {

                        $('#contactsList', '#contacts_mine').find('.sideboxEntry').removeClass('selected');
                        $(this).addClass('selected');
                        var fid = '';
                        if ($(this).attr('fid') != undefined) {
                            fid = $(this).attr('fid');
                        }
                        else {
                            $('#ContactFilterByLetter').val('');
                            $('#ContactsSearchBox').val('');
                        }
                        $('#ContactFilterBy').val(fid);
                        reloadContactsGrid();
                    });

                    // bind contact types filter
                    $('#typesList').find('.sideboxEntry').live("click", function () {

                        $('#typesList').find('.sideboxEntry').removeClass('selected');
                        $(this).addClass('selected');
                        var fid = '';
                        if ($(this).attr('fid') != undefined) {
                            fid = $(this).attr('fid');
                        }
                        else {
                            $('#ContactFilterByLetter').val('');
                            $('#ContactsSearchBox').val('');
                        }
                        $('#ContactFilterByType').val(fid);
                        reloadContactsGrid();
                    });

                    // handle any actions
                    $('#' + querySt('a')).trigger('click');


                    // button row
                    $('button', '#sortrow').click(function () {
                        var sortid = $.trim($(this).text());
                        $('#ContactFilterByLetter').val(sortid);
                        reloadContactsGrid();
                    });

                    $('#deleteContact').click(function () {
                        if ($('.selected-row', '#contactsGridView').length == 0) {
                            $.jGrowl('No contacts selected');
                            return false;
                        }

                        var ok = window.confirm('Are you sure? There is no UNDO.');
                        if (!ok) {
                            return false;
                        }

                        var ids = [];
                        var entries = $('.selected-row', '#contactsGridView');
                        $.each(entries, function () {
                            var id = $(this).attr('id');
                            ids.push(id);
                        });
                        $.ajaxswitch({
                            type: 'POST',
                            url: '/dashboard/contacts/delete',
                            data: { id: ids.join(',') },
                            dataType: 'json',
                            success: function (json_data) {
                                if (json_data.success) {
                                    var ids = json_data.data;
                                    if (ids.length != 0) {
                                        for (var i = 0; i < ids.length; i++) {
                                            sideSelector_delete(ids[i], '#contacts_mine');
                                        }
                                        // remove deleted products
                                        reloadContactsGrid();
                                    }
                                    else {
                                        $.jGrowl('Delete fail. Contact in use.');
                                    }
                                }
                                else {
                                    if (json_data.data == tradelr.returncode.NOPERMISSION) {
                                        $.jGrowl('You do not have permission to delete contacts.');
                                    }
                                    else {
                                        $.jGrowl(json_data.message);
                                    }
                                }
                                return false;
                            }
                        });
                        return false;
                    });
                });
            </script>