var ownerid;
var viewerid;

function getMimeType(filename) {
    var ext = /[^.]+$/.exec(filename);
    var mimetype = "";
    switch (ext) {
        case "gif":
        case "GIF":
            mimetype = "image/gif";
            break;
        case "png":
        case "PNG":
            mimetype = "image/png";
            break;
        default:
            mimetype = "image/jpeg";
            break;
    }
    return mimetype;
}

function navigate(surfaceName) {
    var surfaces = gadgets.views.getSupportedViews();
    var surfaceRef = surfaces[surfaceName];
    gadgets.views.requestNavigateTo(surfaceRef, {});
}

function GetThis(T, C, U) {
    var targetUrl = "http://www.myspace.com/index.cfm?fuseaction=postto&t=" + encodeURIComponent(T) + "&c=" + encodeURIComponent(C) + "&u=" + encodeURIComponent(U);
    window.open(targetUrl, "ptm", "height=450,width=440").focus();
}

function RenderContent(owner, viewer, isCanvas, targetUrl) {
    var param = {};
    var postdata = { ownerid: owner, viewerid: viewer, isCanvas: isCanvas };
    param[gadgets.io.RequestParameters.AUTHORIZATION] = gadgets.io.AuthorizationType.NONE;
    param[gadgets.io.RequestParameters.METHOD] = gadgets.io.MethodType.POST;
    param[gadgets.io.RequestParameters.CONTENT_TYPE] = gadgets.io.ContentType.TEXT;
    param[gadgets.io.RequestParameters.POST_DATA] = gadgets.io.encodeValues(postdata);
    gadgets.io.makeRequest(targetUrl, function (response, url, errored) {
        if (!errored) {
            $('#store_content').html(response.text);
            gadgets.window.adjustHeight();
        }
        else {
            $('#processerror').html('Render failed: No response from tradelr. Please refresh page.');
        }
    }, param);
}

function ConfigureAndRender(owner, viewer, address, isCanvas, targetUrl) {
    var param = {};
    var postdata = { address: address, ownerid: owner, viewerid: viewer, isCanvas: isCanvas };
    param[gadgets.io.RequestParameters.AUTHORIZATION] = gadgets.io.AuthorizationType.NONE;
    param[gadgets.io.RequestParameters.METHOD] = gadgets.io.MethodType.POST;
    param[gadgets.io.RequestParameters.CONTENT_TYPE] = gadgets.io.ContentType.TEXT;
    param[gadgets.io.RequestParameters.POST_DATA] = gadgets.io.encodeValues(postdata);
    gadgets.io.makeRequest(targetUrl, function (response, url, errored) {
        if (!errored) {
            if (response.text != '') {
                $('#store_content').html(response.text);
                gadgets.window.adjustHeight();
            }
            else {
                $('#inputerror').html('Value entered is not a valid store address. Please try again.');
            }
        }
        else {
            $('#inputerror').html('An error has occurred. Please try again.');
        }
        $('#buttonSave').attr('disabled', false);
    }, param);
}

function postActivity(element) {
    var confirm = window.confirm('Post this item to your profile?');
    if (!confirm) {
        return;
    }

    var title = $(element).parents('.galleryBox').children('.title').find('a').text().trim();
    var url = $(element).parents('.galleryBox').children('.title').find('a').attr('href');
    var imagelink = $(element).parents('.galleryBox').children('.productThumbnail').find('img').attr('src');

    var params = {};
    params[opensocial.Activity.Field.TITLE] = title;
    params[opensocial.Activity.Field.BODY] = url;

    var imageUrl = imagelink;
    var imageParams = {};
    imageParams[opensocial.MediaItem.Field.TYPE] = opensocial.MediaItem.Type.IMAGE;
    var mediaItem = opensocial.newActivityMediaItem(getMimeType(imageUrl), imageUrl, imageParams);

    if (gadgets.util.hasFeature('hi5') &&
       opensocial.getEnvironment().supportsField(opensocial.Environment.
          ObjectType.ACTIVITY_MEDIA_ITEM, hi5.ActivityMediaItemField.LINK)) {
        mediaItem.setField(hi5.ActivityMediaItemField.LINK, url);
    }

    var mediaItems = [];
    mediaItems.push(mediaItem);

    params[opensocial.Activity.Field.MEDIA_ITEMS] = mediaItems;
    var activity = opensocial.newActivity(params);
    opensocial.requestCreateActivity(activity, opensocial.CreateActivityPriority.HIGH);
}

function sendNotification(element) {
    var confirm = window.confirm('Post this item to your profile?');
    if (!confirm) {
        return;
    }

    var title = $(element).parents('.galleryBox').children('.title').find('a').text().trim();
    var url = $(element).parents('.galleryBox').children('.title').find('a').attr('href');
    var imagelink = $(element).parents('.galleryBox').children('.productThumbnail').find('img').attr('src');
    var body = $(element).parents('.galleryBox').children('.summary').text().trim();

    var params = {};
    params[opensocial.Message.Field.TITLE] = title;
    params[opensocial.Message.Field.TYPE] = opensocial.Message.Type.PUBLIC_MESSAGE;
    var message = opensocial.newMessage(body, params);
    var recipient = opensocial.DataRequest.PersonId.VIEWER;
    opensocial.requestSendMessage(recipient, message, function (resp) { 
        if (!resp.hadError() && resp.getData().status == "sent") {
            alert("Item posted successfully");
        } else {
            alert("There was a problem: " + resp.getErrorMessage());
        }
    });
};