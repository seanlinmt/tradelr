<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.facebook.viewmodel.FBImportAlbumViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<div class="album_row" id="album_row_<%= Model.id %>">
<div class="mb10 bg_lightgrey pad5">
<span class="w100px mr10">Import as</span>
<input type="radio" class="type_product" id="type_product_<%= Model.id %>" name="type_<%= Model.id %>" value="PRODUCT" checked="checked"/><label for="type_product_<%= Model.id %>">single product</label>
<input type="radio" class="type_collection" id="type_collection_<%= Model.id %>" name="type_<%= Model.id %>" value="COLLECTION" /><label for="type_collection_<%= Model.id %>">product collection</label>
<span class="fr mr20"><%= Model.photo_count %> <span class="type_count">photos</span></span>
</div>
<span class="albumid hidden"><%= Model.id %></span>
<span class="token hidden"><%= Model.token %></span>
<div class="pad5">
<input type="text" id="album_title" name="album_title" class="w70p hidden" title="collection name" />
<div class="album_row_content">
<%
    Html.RenderControl(TradelrControls.facebookImportRowContent, Model);%>
</div>
<div class="album_row_photos">
<p>Select up to 20 photos to include in product</p>
</div>
<a href="#" class="album_photos_showall">show all photos</a>
<div class="mt10">
<button type="button" class="createproduct green">import</button>
</div>
</div>
</div>
<script type="text/javascript">
    tradelr.ajax.jsonp("<%= GeneralConstants.FACEBOOK_GRAPH_HOST + Model.id %>/photos?limit=20&access_token=<%= Model.token %>", function (photos) {
        $.each(photos.data, function () {
            try {
                var imgsrc = '';
                imgsrc = this.images[this.images.length - 1].source;
                var photo = [
                    '<div class="thumbnail mr5" alt="', this.id, '">',
                    '<img src="', imgsrc, '" />',
                    '</div>'
                    ];
                $('.album_row_photos', '#album_row_<%= Model.id %>').append(photo.join(''));
            } catch (e) {

            }
        });
    });

</script>
