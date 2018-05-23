<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.networks.viewmodels.NetworkViewModel>" %>
<div class="section_header">
    <img src="/Content/img/social/icons/facebook_16.png" alt="facebook" />
    Facebook
</div>
<div class="form_group">
    <div id="connected" class="hidden">
        <div class="inline-block w400px fl">
        <p><%= Model.FacebookProfileUrl %><img src="/Content/img/tick.png" /> Connected to Facebook</p>
        <div id="connected_accounts">
    <p>Import photos as products</p>
        <%= Html.DropDownList("fbstreams", Model.facebookStreams) %>
        <button id="buttonget" class="small green" type="button">get photo albums</button>
        <div class="mt10" id="result_albums">
        </div>
        <a href="#" id="albums_showall" class='hidden'>show all albums</a>
        <%= Html.Hidden("fb_stream_id") %>
        <%= Html.Hidden("fb_stream_token") %>
        </div>
        </div>
        <div class="ml30 fl w450px">
        <div class="info">
            <div>To properly disconnect your Facebook account from Tradelr, 
            you will need to go to Facebook and remove Tradelr under 
            <strong><a target="_blank" href="http://www.facebook.com/settings?tab=applications">Applications</a></strong>.</div>
        </div>
        <div class="mt10 ml10">
        <button id="buttonlogout" type="button">
            disconnect from facebook</button>
    </div>
    </div>
    <div class="clear"></div>
    </div>
<div id="notconnected" class="hidden">
<button id="buttonlogin" class="green">connect with facebook</button>
</div>
</div>
<script type="text/javascript">
    $('#buttonlogin').click(function () {
        $(this).attr('disabled', true);
        window.location = '/fb/getToken?perms=read_stream,publish_stream,offline_access,manage_pages';
    });

    $('#buttonlogout').click(function () {
        $.post('/fb/clearToken', null, function (json_result) {
            if (json_result.success && json_result.data) {
                $('#connected').fadeOut(function () {
                    $('#notconnected').fadeIn();
                });
            }
            else {
                $.jGrowl('Unable to disconnect from facebook');
            }
        }, 'json');
    });

    $(document).ready(function () {
        $.post('/dashboard/networks/haveToken', { type: 'facebook' }, function (json_result) {
            if (json_result.success) {
                if (json_result.data) {
                    $('#connected').show();
                }
                else {
                    $('#notconnected').show();
                }
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');

    });


    function initrows() {
        $('#title_product', '.album_row').watermark('product title');
        $('#sku', '.album_row').watermark('SKU (unique ID)');
        $('#price', '.album_row').watermark('selling price');
        $('#price', '.album_row').numeric({ allow: '.' });
        $('#details', '.album_row').watermark('description');
        $('#album_title', '.album_row').watermark('collection name');
        init_autogrow('#result_albums');
        inputSelectors_init('#result_albums');
    }

    $('#buttonget').click(function () {
        $(this).buttonDisable();
        var streamid = $('#fbstreams').val();
        $('#result_albums').html('');
        $.post('/fb/tokens/' + streamid, null, function (json_result) {
            if (json_result.success) {
                var id = json_result.data.id;
                if (id == '') {
                    id = 'me';
                }
                var token = json_result.data.token;
                $('#fb_stream_id').val(id);
                $('#fb_stream_token').val(token);
                tradelr.ajax.jsonp(tradelr.constants.facebook.GRAPH_URL + id + '/albums?limit=3&fields=count,description,id,name&access_token=' + token, function (result) {
                    $.ajax({
                        contentType: 'application/json',
                        type: "POST",
                        url: '/fb/importRow',
                        dataType: "html",
                        data: $.toJSON({ albums: result.data, access_token: token }),
                        success: function (result) {
                            if (result == '') {
                                $.jGrowl('No albums found');
                            }
                            else {
                                $('#result_albums').append(result);
                                $('.selection').hide();
                                initrows();
                                $('#albums_showall').show();
                            }
                        }
                    });
                });
            }
            else {
                $.jGrowl(json_result.message);
            }
            $('#buttonget').buttonEnable();
        }, 'json');
    });

    // get photos in album as individual products
    $('.type_collection').die().live('click', function () {
        // get more photos
        var album = $(this).parents('.album_row');
        var id = $.trim($(album).find('.albumid').html());
        var token = $.trim($(album).find('.token').html());
        var albumtitle = $('#title_product', album).val();
        $('#album_title', album).val(albumtitle);
        $('#album_title', album).show();
        $('.album_row_content', album).html('');
        $('.album_row_photos', album).html('');
        $('.album_row_content', album).showLoadingFacebook();
        $('.album_photos_showall', album).show();
        tradelr.ajax.jsonp(tradelr.constants.facebook.GRAPH_URL + id + '/photos?limit=3&fields=id,name,images&access_token=' + token, function (json_result) {
            $.ajax({
                contentType: 'application/json',
                type: "POST",
                url: '/fb/importRowContent',
                dataType: "html",
                data: $.toJSON(json_result.data),
                success: function (result) {
                    $('.album_row_content', album).html(result);
                    initrows();
                }
            });
        });
    });

    // get photos in album for single product
    $('.type_product').die().live('click', function () {
        var album = $(this).parents('.album_row');
        var id = $.trim($(album).find('.albumid').html());
        var token = $.trim($(album).find('.token').html());
        $('#album_title', album).hide();
        $(album).showLoadingFacebook();
        $('.album_photos_showall', album).show();
        tradelr.ajax.jsonp(tradelr.constants.facebook.GRAPH_URL + id + '?&fields=name,count,description&access_token=' + token, function (json_result) {
            var albums = [];
            albums.push(json_result);
            $.ajax({
                contentType: 'application/json',
                type: "POST",
                url: '/fb/importRow',
                dataType: "html",
                data: $.toJSON({ albums: albums, access_token: token }),
                success: function (result) {
                    $(album).replaceWith(result);
                    initrows();
                }
            });
        });
    });

    // selecting photos
    $('.thumbnail img,.thumbnail .selected').die().live('click', function () {
        var thumbnail = $(this).parents(".thumbnail");
        if ($(thumbnail).contents('.selected').length == 0) {
            $(thumbnail).addClass("opacity100");
            $(thumbnail).append("<div class='selected'></div>");

        }
        else {
            $(thumbnail).removeClass("opacity100");
            $(thumbnail).contents('.selected').remove();
        }
    });

    // selecting product
    $('.selection', '.album_row').die().live('click', function () {
        $(this).toggleClass('row_selected');
    });

    function facebook_album_getmore(albumid, token, nextlink) {
        if (nextlink == null || nextlink == undefined) {
            return false;
        }

        var url = tradelr.constants.facebook.GRAPH_URL + albumid + '/photos?offset=20&fields=id,name,images&access_token=' + token;

        if (nextlink != '') {
            url = nextlink;
        }
        tradelr.ajax.jsonp(url, function (json_result) {
            $('.loader', '#album_row_' + albumid).remove();
            var photos = json_result.data;
            if (json_result.data.length == 0 || json_result.paging == undefined) {
                $.jGrowl('No more photos');
                url = null;
            }
            else {
                url = json_result.paging.next;
            }
            facebook_album_getmore(albumid, token, url);
            $.each(photos, function () {
                try {
                    var imgsrc = '';
                    imgsrc = this.images[this.images.length - 1].source;
                    var photo = [
                '<div class="thumbnail mr5" alt="', this.id, '">',
                '<img src="', imgsrc, '" />',
                '</div>'
                ];
                    $('.album_row_photos', '#album_row_' + albumid).append(photo.join(''));
                } catch (e) {

                }
            });
        });
    };

    // photo details
    var fb_photos = [];
    function facebook_photo_getmore(albumid, token, nextlink) {
        if (nextlink == null || nextlink == undefined) {
            $.ajax({
                contentType: 'application/json',
                type: "POST",
                url: '/fb/importRowContent',
                dataType: "html",
                data: $.toJSON(fb_photos),
                success: function (result) {
                    $('.loader', '#album_row_' + albumid).remove();
                    $('.album_row_content', '#album_row_' + albumid).html(result);
                    initrows();
                }
            });
            return false;
        }

        var url = tradelr.constants.facebook.GRAPH_URL + albumid + '/photos?fields=id,name,images&access_token=' + token;

        if (nextlink != '') {
            url = nextlink;
        }
        else {
            // initialise result array
            fb_photos = [];
        }

        tradelr.ajax.jsonp(url, function (json_result) {
            fb_photos = fb_photos.concat(json_result.data);
            if (json_result.data.length == 0 || json_result.paging == undefined) {
                url = null;
            }
            else {
                url = json_result.paging.next;
            }
            facebook_photo_getmore(albumid, token, url);
        });
    };

    $('.album_photos_showall').die().live('click', function () {
        var album = $(this).parents('.album_row');
        var id = $.trim($(album).find('.albumid').html());
        var token = $.trim($(album).find('.token').html());
        $(this).hide();
        if ($('.type_product', album).is(':checked')) {
            // get just product photos
            $('.album_row_photos', '#album_row_' + id).showLoadingFacebook('medium', true);
            facebook_album_getmore(id, token, '');
        }
        else {
            $('.album_row_content', '#album_row_' + id).showLoadingFacebook('medium', true);
            facebook_photo_getmore(id, token, '');
        }

        return false;
    });

    // albums
    var fb_albums = [];
    function facebook_albums_getmore(nextlink) {
        var id = $('#fb_stream_id').val();
        var token = $('#fb_stream_token').val();
        if (nextlink == null || nextlink == undefined) {
            if (fb_albums.length == 0) {
                $.jGrowl('No more albums');
                return false;
            }
            $.ajax({
                contentType: 'application/json',
                type: "POST",
                url: '/fb/importRow',
                dataType: "html",
                data: $.toJSON({ albums: fb_albums, access_token: token }),
                success: function (result) {
                    $('.loader', '#result_albums').remove();
                    $('#result_albums').append(result);
                    initrows();
                }
            });
            return false;
        }

        var url = tradelr.constants.facebook.GRAPH_URL + id + '/albums?offset=3&fields=count,description,id,name&access_token=' + token;

        if (nextlink != '') {
            url = nextlink;
        }
        else {
            // initialise result array
            fb_albums = [];
        }

        tradelr.ajax.jsonp(url, function (json_result) {
            fb_albums = fb_albums.concat(json_result.data);
            if (json_result.data.length == 0 || json_result.paging == undefined) {
                url = null;
            }
            else {
                url = json_result.paging.next;
            }
            facebook_albums_getmore(url);
        });
    };

    // show all albums
    $('#albums_showall').click(function () {
        $(this).hide();
        $('#result_albums').showLoadingFacebook('medium', true);
        facebook_albums_getmore('');
        return false;
    });

    // import products
    $('.createproduct').die().live('click', function () {
        var album = $(this).parents('.album_row');
        var id = $.trim($(album).find('.albumid').html());
        if ($('.type_product', album).is(':checked')) {
            var ok = window.confirm('Import this album as a product with selected photos?');
            if (!ok) {
                return false;
            }
            var selectedphotos = [];
            $.each($('.selected', album).parent(), function () {
                selectedphotos.push($(this).attr('alt'));
            });

            if (selectedphotos.length == 0) {
                $.jGrowl('No photos selected');
                return false;
            }

            var sproduct = {
                access_token: $.trim($('.token', album).html()),
                product: {
                    id: $.trim($('.albumid', album).html()),
                    title: $('#title_product', album).val(),
                    description: $('#details', album).val(),
                    sellingprice: $('#price', album).val(),
                    sku: $('#sku', album).val(),
                    photoids: selectedphotos
                }
            };
            $.ajax({
                contentType: 'application/json',
                type: "POST",
                url: '/fb/importproduct',
                dataType: "json",
                data: $.toJSON(sproduct),
                success: function (json_result) {
                    if (json_result.success) {
                        $.jGrowl('Product imported successfully');
                        $(album).fadeOut('fast', function () {
                            $(this).remove();
                        });
                    }
                    else {
                        $.jGrowl(json_result.message);
                    }
                }
            });
        }
        else {
            var collectiontitle = $('#album_title', album).val();
            var ok = window.confirm('Import selected products and create product collection ' + collectiontitle + '?');
            if (!ok) {
                return false;
            }
            var selectedproducts = [];
            $.each($('.row_selected', album).parent(), function () {
                var product = {
                    id: $.trim($('.contentid', this).html()),
                    title: $('#title_product', this).val(),
                    description: $('#details', this).val(),
                    sellingprice: $('#price', this).val(),
                    sku: $('#sku', this).val(),
                    photoids: $.trim($('.contentids', this).html()).split(',')
                };
                selectedproducts.push(product);
            });

            if (selectedproducts.length == 0) {
                $.jGrowl('No products selected');
                return false;
            }

            var collection = {
                id: $.trim($('.albumid', album).html()),
                title: collectiontitle,
                access_token: $.trim($('.token', album).html()),
                products: selectedproducts
            };
            $.ajax({
                contentType: 'application/json',
                type: "POST",
                url: '/fb/importcollection',
                dataType: "json",
                data: $.toJSON(collection),
                success: function (json_result) {
                    if (json_result.success) {
                        var hasError = false;
                        $.each(json_result.data, function () {
                            var container = $('.contentid:contains(' + this.id + ')', album).parent();
                            if (this.success) {
                                $(container).fadeOut('fast', function () {
                                    $(this).remove();
                                });
                            }
                            else {
                                $('.selection', container).append('<div class="error_post">' + this.message + '</div>');
                                hasError = true;
                            }
                        });
                        if (hasError) {
                            $.jGrowl('Some products not imported');
                        }
                        else {
                            $.jGrowl('Products imported successfully');
                        }
                    }
                    else {
                        $.jGrowl(json_result.message);
                    }
                }
            });
        }
        return false;
    });
</script>
