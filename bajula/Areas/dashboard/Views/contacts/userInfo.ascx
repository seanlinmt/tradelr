<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Contact>" %>
<%@ Import Namespace="tradelr.Models.contacts"%>
<div class="section_header">
        Basic Information</div>
<div id="profile_image" class="images_column fl">
    <div class="results">
        <%
     if (Model.profilePhoto != null)
     {%>
        <div class='thumbnail' style="width:100%;">
            <img src='<%=Model.profilePhoto.url%>' alt='<%=Model.profilePhoto.id%>' />
            <div class='del' onclick="thm_delete(this,'<%=Model.profilePhoto.id%>','profile')">
                <span>delete</span></div>
        </div>
        <%
     }
     else
     {%>
        <div class="nophoto">
            no profile photo has been uploaded yet</div>
        <%
     }%>
    </div>
    <%if (Model.isOwner || Model.canModify){%>
    <div id="swfu_profile" class="swfu_container">
        <div class="swfu_button">
        </div>
    </div>
    <%}%> 
</div>
    <div class="form_group fl">
    <div class="fl">
        <div class="form_entry" style="width:190px;">
            <div class="form_label">
                <label for="email">
                    Email</label>
            </div>
            <%= Html.TextBox("email", Model.email) %>
            <div id="emailCheckResponse"></div>
        </div>
        </div>
        <%if (!Model.isOwner)
          {%>
        <div class="fl pl10">
        <div class="form_entry">
            <div class="form_label">
                <label for="password">
                    Password</label>
            </div>
            <%= Html.TextBox("password")%>
            <span class="tip">allows user to login if specified</span>
        </div>
        </div>
        <%} %>
        <div class="clear"></div>
        <div class="fl">
            <div class="form_entry">
                <div class="form_label">
                    <label for="firstName">
                        First Name</label>
                </div>
                <%= Html.TextBox("firstName", Model.firstName)%>
            </div>
        </div>
        <div class="fl pl10">
            <div class="form_entry">
                <div class="form_label">
                    <label for="lastName">
                        Last Name</label>
                </div>
                <%= Html.TextBox("lastName", Model.lastName)%>
            </div>
        </div>
        <div class="clear">
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="gender">
                    Gender</label>
            </div>
            <%= Html.DropDownList("gender", 
        new SelectList(new List<SelectListItem>
            {
                new SelectListItem{Text = "Select ...", Value = ""},
                new SelectListItem{Text = "Male", Value = "male"}, 
                new SelectListItem{Text = "Female", Value = "female"}
            }, "Value", "Text", Model.gender))%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="phone">
                    Phone Number</label>
            </div>
            <%= Html.TextBox("phone", Model.phone)%>
        </div>
    </div>
<div class="clear">
</div>
<%= Html.Hidden("profilePhotoID") %>
<script src="/jsapi/uploader" type="text/javascript"></script>
<script type="text/javascript">
    var uploadUrl;
    function bindUserUploader() {
        if ($('#swfu_profile').length != 0) {
            new AjaxUpload($('#swfu_profile'), {
                action: uploadUrl,
                onSubmit: function (file, ext) {
                    if (!(ext && /^(jpg|jpeg|png|gif)$/i.test(ext))) {
                        // extension is not allowed
                        $.jGrowl('Error: Unsupported file extension');
                        // cancel upload
                        return false;
                    }
                    this.disable();
                    $.prettyLoader.show();
                },
                onComplete: function (file, response) {
                    // enable upload button
                    this.enable();
                    $.prettyLoader.hide();
                    var info = response.split(',');
                    var imageid = info[0];
                    var targetid = info[1];
                    var url = info[2];
                    if ($('.nophoto', targetid).length != 0) {
                        $('.nophoto', targetid).hide();
                    }

                    var thumbnail = "<div class='thumbnail' style='display:none'><img src='" + url +
            "' alt='" + imageid + "' /><div class='del'><span>delete</span></div></div>";
                    var existing = $(targetid + " > .results").find('.thumbnail');
                    if ($(existing).length == 0) {
                        $(thumbnail).fadeIn().appendTo(targetid + " > .results");
                    }
                    else {
                        $(existing).fadeOut('normal', function () {
                            $(thumbnail).fadeIn().appendTo(targetid + " > .results");
                        });
                    }
                }
            });
        }
    }

    $(document).ready(function () {
        if ($('#id').val() !== undefined) {
            uploadUrl = "/photos/Upload/profile/" + $('#id').val();
        }
        else {
            uploadUrl = "/photos/Upload/profile";
        }
        bindUserUploader();
    });
</script>
