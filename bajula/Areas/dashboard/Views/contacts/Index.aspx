<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.BaseViewModel>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Contacts
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area">
    <ul class="hidden">
            <li><a href="#contacts_mine">contacts</a></li>
            <li><a href="#group_pricing">group pricing</a></li>
        </ul>
        <div id="contacts_mine" class="hidden">
            <%
                Html.RenderAction("contacts", "contacts");%>
        </div>
        <div id="group_pricing" class="hidden">
        <%
                Html.RenderAction("pricing", "group");%>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#navcontact').addClass('navselected');
            var $tabs = $('.content_area').tabs({
                select: function (event, ui) {
                    inittabs(ui.index);
                }
            });
            inittabs($tabs.tabs('option', 'selected'));

            $('.addmain').bind("click", function () {
                dialogBox_open("/dashboard/contacts/listAdd", 710, null, function () {
                    // init height
                    var height = $(window).height();
                    var titleHeight = $('#headingAdd', '#listAddForm').height();
                    var buttonHeight = $('#buttons', '#listAddForm').height();
                    var inputHeight = $('.form_entry', '#listAddForm').height();
                    var fixedHeight = height - titleHeight - buttonHeight - inputHeight - 150; // dialog padding
                    tradelr.log(height + " " + titleHeight + " " + buttonHeight + " " + inputHeight + " " + fixedHeight);
                    if ($('.fixedHeight', '#listAddForm').height() > fixedHeight) {
                        $('.fixedHeight', '#listAddForm').height(fixedHeight);
                    }
                });
            });

            var groups = $('#contactsList .sideboxEntry', '#group_pricing');
            $.each(groups, function (i, val) {
                // skip the first entry
                if (i > 0) {
                    $(this).append('<div class="edit" title="edit contacts in group"></div>');
                    $(this).append('<div class="del" title="delete group"></div>');
                }
            });

            groups = $('#contactsList .sideboxEntry', '#contacts_mine');
            $.each(groups, function (i, val) {
                // skip the first entry
                if (i > 0) {
                    $(this).append('<div class="edit" title="edit contacts in group"></div>');
                    $(this).append('<div class="del" title="delete group"></div>');
                }
            });


            $('.edit').live("click", function () {
                var id = $(this).parent().attr('fid');
                var url = "/dashboard/contacts/listEdit/" + id;
                dialogBox_open(url, 710, null, function () {
                    // init height
                    var height = $(window).height();
                    var titleHeight = $('.headingEdit', '#listEditForm').height();
                    var buttonHeight = $('#buttons', '#listEditForm').height();
                    var fixedHeight = height - titleHeight - buttonHeight - 150; // dialog padding
                    //tradelr.log(height + " " + titleHeight + " " + buttonHeight + " " + fixedHeight);
                    if ($('.fixedHeight', '#listEditForm').height() > fixedHeight) {
                        $('.fixedHeight', '#listEditForm').height(fixedHeight);
                    }
                });
                return false;
            });

            $('.del').live("click", function () {
                var ok = window.confirm("Are you sure? This will delete the group '" + $(this).parent().text() +
                                                                            "'. Your contacts WILL NOT be deleted.");
                if (!ok) {
                    return false;
                }
                var id = $(this).parent().attr('fid');
                $.ajaxswitch({
                    type: 'POST',
                    url: '/dashboard/contacts/listDelete/' + id,
                    dataType: 'json',
                    success: function (json_data) {
                        if (json_data.success) {
                            $('.sideboxEntry[fid=' + id + ']').each(function () {
                                $(this).slideUp(function () {
                                    $(this).remove();
                                });
                            });
                            // reload grid
                            $('#ContactFilterBy').va('');
                            reloadContactsGrid();
                            $.jGrowl('Group successfully deleted');
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

        function inittabs(index) {
            var active = $('.ui-tabs-nav > li:eq(' + index + ') > a', '.content_area').attr('href');
            switch (active) {
                case "#contacts_mine":
                    contactBindToGrid();
                    break;
                case "#group_pricing":
                    groupPricingBindToGrid(getFilterByField("#group_pricing"));
                    break;
                default:
                    break;
            }
        }
    </script>
</asp:Content>
