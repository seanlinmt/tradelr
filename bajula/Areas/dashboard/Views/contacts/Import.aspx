<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.contacts.viewmodel.ImportContactsViewData>" %>
<%@ Import Namespace="tradelr.Common.Constants" %>
<%@ Import Namespace="tradelr.Library" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Import Contacts
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area">
        <h3 id="headingAdd" title="this page allows you to import contacts from various sources">
            invite and import contacts</h3>
        <div class="fl pad5 opacity">
            <img id="inviteFacebook" src="/Content/img/social/facebook.png" alt="facebook" />
        </div>
        <div class="fl pad5 opacity">
            <img id="inviteGmail" src="/Content/img/social/gmail.png" alt="gmail" />
        </div>
        <!--
        <div class="fl pad5 opacity">
            <img id="inviteLinkedIn" src="/Content/img/social/linkedin.png" alt="linkedin" />
        </div>
        <div class="fl pad5 opacity">
            <img id="inviteLive" src="/Content/img/social/live.png" alt="live" />
        </div>
        -->
        <div class="fl pad5 opacity">
            <img id="inviteYahoo" src="/Content/img/social/yahoo.png" alt="yahoo" />
        </div>
        <div class="clear">
        </div>
        <div id="facebookBit" class="importPanel mt20 hidden">
            <fb:serverfbml style="width: 750px;">
<%-- ReSharper disable OtherTagsInsideScript --%>
                <script type="text/fbml"> 
             <fb:fbml> 
            <fb:request-form type="network" invite="true" method="GET" action="<%=Model.hostName.ToDomainUrl("/dashboard/contacts/import")%>"
                content="<fb:name uid='<%=Model.fbuid%>' useyou='false' /> would like to invite you to their network on tradelr.com. To join their network, simply click on the 'Connect' button below. <fb:req-choice url='<%=Model.hostName.ToDomainUrl("/login")%>' label='Connect' />. 
                If you would like to create your own network, you can register <a href='<%=GeneralConstants.HTTP_HOST + "/pricing"%>'>here</a>.">
                    <fb:multi-friend-selector bypass="cancel" actiontext="Invite your Facebook friends to your network" showborder="false" exclude_ids="<%=Model.invitedFbuidList%>" />
            </fb:request-form>
            </fb:fbml>  
                </script>
<%-- ReSharper restore OtherTagsInsideScript --%>
            </fb:serverfbml>
        </div>
        <div id="gmailBit" class="importPanel mt20 hidden">
            <%
                if (Model.contacts != null)
                {
            %>
            <div class="section_header">
                Select contacts that you would like to import to tradelr</div>
            <div class="form_group">
                <div id="googleList">
                    <div class="mb10">
                        <input class="selectall" type="checkbox" /><label for="selectall" class="bold">Select
                            All Contacts</label>
                    </div>
                    <%
                        Html.RenderPartial("contactList", Model.contacts); %>
                </div>
                <button id="googleSave" class="green mt10" type="button">
                    import selected contacts</button></div>
            <%
                }
                else
                {%>
            <p>
                Could not find contacts to import.</p>
            <%
                }%>
        </div>
        <div id="ajaxBit" class="importPanel mt20 hidden">
            <div class="section_header">
                Select contacts that you would like to import to tradelr</div>
            <div class="form_group">
                <div id="ajaxList">
                    <div class="mb10">
                        <input class="selectall" type="checkbox" /><label for="selectall" class="bold">Select
                            All Contacts</label>
                    </div>
                </div>
                <button id="ajaxSave" class="green mt10" type="button">
                    import selected contacts</button></div>
        </div>
    </div>
    <%= Html.Hidden("FBAPI", GeneralConstants.FACEBOOK_API_KEY)%>
    <%= Html.Hidden("fbuid", Model.fbuid)%>
    <%= Html.Hidden("gapikey", GoogleConstants.GOOGLE_APIKEY)%>
    <%= Html.Hidden("subdomainid", Model.subdomainid) %>
    <%= Html.Hidden("appid", Model.appid) %>
    <div id="fb-root">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
   <script type="text/javascript">
       var win1;
       function check() {
           if (win1.closed) {
               $.post('/import/yahooContacts', null, function (json_result) {
                   if (json_result.success) {
                       $('.importPanel').hide();
                       $('#ajaxList').append(json_result.data);
                       $('#ajaxList').parents('.importPanel').fadeIn();
                   }
                   else {
                       $.jGrowl(json_result.message);
                   }
               }, 'json');
           } else {
               setTimeout("check()", 1);
           }
       }

       $(document).ready(function () {
           $('#navcontact').addClass('navselected_white');

           var importType = querySt('type');
           switch (importType) {
               case 'GOOGLE':
                   $('#gmailBit').show();
                   break;
               default:
                   break;
           }

           $('#inviteLinkedIn, #inviteLive').click(function () {
               $.jGrowl('Coming soon');
           });

           $('#inviteYahoo').click(function () {
               var subdomainid = $('#subdomainid').val();
               var appid = $('#appid').val();
               var params = ['subdomainid=', subdomainid, '&appid=', appid];
               if (DEBUG) {
                   win1 = window.open('https://secure.localhost/oauthclient/yahoo?' + params.join(''), '', 'width=785px,height=420px,toolbar=0');
               }
               else {
                   win1 = window.open('https://secure.tradelr.com/oauthclient/yahoo?' + params.join(''), '', 'width=785px,height=420px,toolbar=0');
               }
               check();
           });

           $('#inviteFacebook').click(function () {
               if ($('#fbuid').val() == '') {
                   $.jGrowl('You need to connect your account with facebook first');
                   return false;
               }
               $('.importPanel').hide();
               $('#facebookBit').show();
           });

           // google
           $('#inviteGmail').click(function () {
               window.location = '/import/googleContacts';
           });


           $('.selectall', '#googleList').click(function () {
               $('#googleList .blockSelectable').trigger('click');
           });

           $('.selectall', '#ajaxList').click(function () {
               $('#ajaxList .blockSelectable').trigger('click');
           });

           $('#googleSave, #ajaxSave').click(function () {
               var button = this;
               var parent = $(this).parents('.importPanel');
               var checkboxes = parent.find('.blockSelectable');
               var contacts = [];
               $.each(checkboxes, function (i, val) {
                   if ($(this).hasClass('selected')) {
                       var contact = new Object();
                       contact.email = $.trim($(this).find('h4').text());
                       contact.firstName = $.trim($(this).find('.firstname').text());
                       contact.lastName = $.trim($(this).find('.lastname').text());
                       contact.address = $.trim($(this).attr('title'));
                       contacts.push(contact);
                   }
               });

               if (contacts.length == 0) {
                   $.jGrowl('Nothing selected');
                   return;
               }
               $(this).buttonDisable();
               var encoded = $.toJSON(contacts);

               // post form
               $.ajax({
                   contentType: 'application/json',
                   type: "POST",
                   url: '/dashboard/contacts/import',
                   dataType: "json",
                   data: encoded,
                   success: function (json_data) {
                       if (json_data.success) {
                           $(parent).hide();
                           $.jGrowl('Contacts imported successfully');
                       }
                       else {
                           $.jGrowl(json_data.message);
                       }
                       $(button).buttonEnable();
                   }
               }); // ajax
           });

           window.fbAsyncInit = function () {
               FB.init({ appId: $('#FBAPI').val(), status: true, cookie: true,
                   xfbml: false
               });
               FB.login(function (response) {
                   if (response.session) {
                       // user successfully logged in
                       FB.init({ appId: $('#FBAPI').val(), xfbml: true });
                   } else {
                       // user cancelled login
                   }
               });

           };
           (function () {
               var e = document.createElement('script'); e.async = true;
               e.src = document.location.protocol + '//connect.facebook.net/en_US/all.js';
               document.getElementById('fb-root').appendChild(e);
           } ());
       });
   </script>
</asp:Content>
