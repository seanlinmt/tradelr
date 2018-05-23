<%@ Page ValidateRequest="false" Title="Admin Page" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.admin.Admin>" %>
<%@ Import Namespace="System.Threading" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Admin</h2>
    <div>
        <table>
            <tr>
                <td colspan="3">
                    1min:<%= Model.cacheTimer1Min %>
                    5min:<%= Model.cacheTimer5Min %>
                    10min:<%= Model.cacheTimer10Min %>
                    60min:<%= Model.cacheTimer60Min %>
                </td>
            </tr>
            <% int availableWorker, availableIO;
               int maxWorker, maxIO;

               ThreadPool.GetAvailableThreads(out availableWorker, out availableIO);
               ThreadPool.GetMaxThreads(out maxWorker, out maxIO); 
            %>
            <tr>
                <td colspan="3">
                    availableWorker:<%= availableWorker%>
                    availableIO:<%= availableIO%>
                    maxWorker:<%= maxWorker%>
                    maxIO:<%= maxIO%>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    mailQueue: <%= Model.mailQueueLength %>, 
                    creators: <%= Model.creatorCount %>,
                    users: <%= Model.userCount %>, 
                    orders: <%= Model.orderCount %>,
                    supportMessages: <%= Model.supportCount %>
                </td>
            </tr>
            <tr>
            <td colspan="3">
                products: <%= Model.productCount %>, onlineusers: <%= Model.sessionCount %>
            </td>
            </tr>
            <tr>
                <td>
                    Send Test Email
                </td>
                <td>
                </td>
                <td>
                    <button id="sendEmail">
                        Send</button>
                </td>
            </tr>
            <tr>
                <td>
                    Generate Invitation Code
                </td>
                <td id="inviteCode">
                </td>
                <td>
                    <button id="getInvitationCode">
                        Get</button>
                </td>
            </tr>
            <tr>
                <td>
                    Delete User
                </td>
                <td>
                    <%= Html.TextBox("email") %>
                </td>
                <td>
                    <button id="deleteUser">
                        Delete</button>
                </td>
            </tr>
            <tr>
                <td>
                    Delete Subdomain
                </td>
                <td>
                    <%= Html.TextBox("subdomain") %>
                </td>
                <td>
                    <button id="deleteSubdomain">
                        Delete</button>
                </td>
            </tr>
            <tr>
                <td>
                    Delete Order (enter order id)
                </td>
                <td>
                    <%= Html.TextBox("orderid") %>
                </td>
                <td>
                    <button id="deleteOrder">
                        Delete</button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    Demo Account
                </td>
                <td>
                    <button id="demo_create" type="button">
                        create demo account</button>
                        <button id="demo_delete" type="button">
                        delete demo account
                        </button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                <input type="text" id="emailsubject" />
                    <textarea id="emailcontent" class="w100p" rows="10"></textarea></td>
                
                <td>
                    <button onclick="javascript:sendMassMail();">Email account creators</button></td>
            </tr>
        </table>
    </div>
    <div class="mt20">
        <ul>
            <% foreach (var country in Model.registeredCountries)
               { %>
  <li><%= country %></li>
            <%   } %>
        </ul>
    </div>
    <div id="fb-root">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#deleteSubdomain').click(function () {
                $.post("/secret/deletesubdomain/" + $('#subdomain').val(), null, function (json_result) {
                    if (json_result.success) {
                        $.jGrowl('Delete successful');
                    }
                    else {
                        $.jGrowl(json_result.message);
                    }
                }, 'json');
            });

            $('#sendEmail').click(function () {
                $.post("/email/test", null, function () {
                    $.jGrowl('Email sent');
                });
            });

            $('#getInvitationCode').click(function () {
                $.post("/secret/invitecode", null, function (data) {
                    $('#inviteCode').html(data);
                });
            });

            // demo account
            $('#demo_create').click(function () {
                $.post('/secret/demo_create', null, function (json_result) {
                    $.jGrowl(json_result.message);
                }, 'json');
                return false;
            });

            $('#demo_delete').click(function () {
                $.post('/secret/demo_delete', null, function (json_result) {
                    $.jGrowl(json_result.message);
                }, 'json');
                return false;
            });

            $('#deleteOrder').click(function() {
                $.post("/secret/deleteorder" ,{ orders: $('#orderid').val()}, function (json_data) {
                    $.jGrowl(json_data.message);
                }, 'json');
            });

            $('#deleteUser').click(function () {
                $.post("/secret/deleteuser", { email: $('#email').val() }, function (json_data) {
                    $.jGrowl(json_data.message);
                }, 'json');
            });
            setTimeout(function () {
                window.location.reload();
            }, 60000);
        });
        function sendMassMail() {
            var content = $('#emailcontent').val();
            var subject = $('#emailsubject').val();
            $.post('/email/creators', { content: content, subject: subject }, function (json_result) {
                if (json_result.success) {
                    $.jGrowl('Done');
                }
                else {
                    $.jGrowl(json_result.message);
                }
            }, 'json');
        }
    </script>

</asp:Content>
