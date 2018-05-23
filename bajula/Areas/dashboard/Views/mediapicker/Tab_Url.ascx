<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.orchard.media.viewmodels.MediaFolderEditViewModel>" %>
<%
    // media directory to save uploaded files into
    var mediaPath = Request["uploadpath"];
    if (!Url.IsLocalUrl(mediaPath)) {
        mediaPath = "";
    }
%>
        <div id="image-preview">
            <img alt="" id="img-loader" style="display:none" src="" />
            <div class="media-largepreview">
                <img alt="Preview of Image" id="img-preview" src="/Content/img/mediapicker/imagepreview.png" onload="jQuery.mediaPicker.scalePreview(this)" />
            </div>
            <div>
                <label for="img-src">URL for the image resource</label>
                <input class="text-box" type="text" id="img-src" />
            </div>
            <div>
            <form action="<%= Url.Action("AddFromClient", "Media") %>" id="img-uploadform" enctype="multipart/form-data" onsubmit="jQuery.mediaPicker.uploadMedia(this)" method="POST">
            <input type="hidden" name="MediaPath" value="<%= mediaPath %>" />
                    <label for="fileUpload">Upload an image from your computer</label>
                    <input type="file" name="fileUpload" id="fileUpload"  />
                    <button id="upload" class="green" type="submit">Upload</button>
            </form>
				<img id="img-indicator" src="/Content/img/mediapicker/synchronizing.gif" alt="" class="throbber" />
            </div>

        </div>
        <div id="file-details">
            <fieldset>
            <ol>
                <li>
                <label for="img-alt">Alternative Text</label>
                <input class="text-box" type="text" id="img-alt" />
                </li>

                <li>
                <label for="img-class">Class</label>
                <input class="text-box" type="text" id="img-class" />
                </li>

                <li>
                <label for="img-style">Style</label>
                <input class="text-box" type="text" id="img-style" />
                </li>

                <li>
                <label for="img-align">Alignment</label>
                <select id="img-align">
                    <option value="">None</option>
                    <option value="left">Left</option>
                    <option value="right">Right</option>
                    <option value="top">Top</option>
                    <option value="texttop">Text Top</option>
                    <option value="middle">Middle</option>
                    <option value="absmiddle">AbsMiddle</option>
                    <option value="bottom">Bottom</option>
                    <option value="absbottom">AbsBottom</option>
                    <option value="baseline">Baseline</option>
                </select>
                </li>

                <li class="group">
                <div class="image-width">
                    <label for="img-width">Width</label>
                    <input class="text-box" type="text" id="img-width" />&nbsp;x
                </div>
                <div class="image-height">
                    <label for="img-height">Height</label>
                    <input class="text-box" type="text" id="img-height" />
                </div>
                </li>

                <li>
                <input type="checkbox" id="img-lock" checked="checked" />
                <label class="forcheckbox" for="img-lock">Lock Aspect Ratio</label>
                </li>

                <li class="actions">
                <button id="img-insert" class="disabled green"  data-edittext="Update" type="button">Insert</button>
                <button id="img-cancel" type="button">Cancel</button>
                </li>
                </ol>
            </fieldset>
        </div>

