<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.contact.ContactViewModel>" %>
<%@ Import Namespace="tradelr.Models.contacts"%>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%if (Model.contact.isOwner)
  { %>
<div id="company_image" class="images_column fl">
    <div class="results">
        <%
            if (Model.contact.companyLogo != null)
            {
        %>
        <div class='thumbnail'>
            <img src='<%= Model.contact.companyLogo.url%>' alt='<%= Model.contact.companyLogo.id %>' />
            <div class='del' onclick="thm_delete(this,'<%= Model.contact.companyLogo.id %>','company')">
                <span>delete</span></div>
        </div>
        <%}
            else
            {%>
        <div class="nophoto">
            no organization logo has been uploaded yet</div>
        <%} %>
    </div>
    <div id="swfu_company" class="swfu_container">
        <div class="swfu_button">
        </div>
    </div>
</div>
<%} %>
<div id="company_info" class="form_group fl">
    <div class="form_entry">
        <div class="form_label">
            <label for="companyName">
                Organization Name</label>
        </div>
        <%= Html.TextBox("companyName", Model.contact.companyName, new Dictionary<string, object> { { "style", "width:360px" } })%>
    </div>
    <% Html.RenderPartial("~/Views/contacts/userLocation.ascx", "#company_info"); %>
    <div>
        <div class="form_entry">
            <div class="form_label">
                <label for="address">
                    Street Address</label>
            </div>
            <div style="width: 360px">
                <%= Html.TextArea("address", Model.contact.address)%>
            </div>
        </div>
        <div>
            <div class="fl">
                <div class="form_entry">
                    <div class="form_label">
                        <label for="city">
                            City</label>
                    </div>
                    <%= Html.TextBox("city", Model.contact.city)%>
                    <%= Html.Hidden("citySelected")%>
                </div>
                <div class="form_entry">
            <div class="form_label">
                <label for="coPhone">
                    Phone Number</label>
            </div>
            <%= Html.TextBox("coPhone", Model.contact.coPhone)%>
        </div>
            </div>
            <div class="fl pl10">
                <div class="form_entry">
                    <div class="form_label">
                        <label for="postcode">
                            Postal/Zip Code</label>
                    </div>
                    <%= Html.TextBox("postcode", Model.contact.postcode)%>
                </div>
                <div class="form_entry">
            <div class="form_label">
                <label for="fax">
                    Fax Number</label>
            </div>
            <%= Html.TextBox("fax", Model.contact.fax)%>
        </div>
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
    <div class="clear">
    </div>
    <span id="countryval" class="hidden"><%= Model.contact.country  %></span>
<span id="stateval" class="hidden"><%= Model.contact.state %></span>
</div>
<div class="clear">
</div>
<script type="text/javascript">
    function bindOrgUploader() {
        if ($('#swfu_company').length != 0 && $('#swfu_company').is(':visible')) {
            new AjaxUpload($('#swfu_company'), {
                action: '/photos/Upload/company',
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
        $('#country').val($('#countryval').text());

        $('#city').autocomplete('/city/find', {
            dataType: "json",
            parse: function (data) {
                var rows = new Array();
                if (data != null && data.length != null) {
                    for (var i = 0; i < data.length; i++) {
                        rows[i] = { data: data[i], value: data[i].title, result: data[i].title };
                    }
                }
                return rows;
            },
            autoFill: true,
            formatItem: function (row, i, max) {
                return row.title;
            }
        });

        $("#city").bind('keyup', function () {
            $("#citySelected").val('');
        });

        $("#city").result(function (event, data, formatted) {
            if (data) {
                $("#citySelected").val(data.id);
            }
        });

        user_location_update('', "#company_info");
    });
</script>