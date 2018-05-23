<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.inventory.InventoryViewData>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div class="content_filter">
    <div id="statusList" class="filter">
        <h4 class="acc">
        <img src="/Content/img/product_status.png" />
            status</h4>
        <div fid="" class="sideboxEntry selected" title="Active and visible products">
            Active</div>
        <div fid="inactive" class="sideboxEntry" title="Private and hidden products">
            Inactive</div>
    </div>
    <div id="locationList" class="filter">
        <h4 class="acc">
        <img src="/Content/img/headings/heading_network_16.png" />
            locations / networks</h4>
        <div class="sideboxEntry selected pl4" fid="">
            <strong>All</strong><div class="addmain"></div>
        </div>
        <%= Html.FilterBoxList("locationList", Model.locationList)%>
    </div>
    <div id="collectionsList" class="filter">
        <h4 class="acc">
        <img src="/Content/img/product_collections.png" />
            product collections</h4>
        <div class="sideboxEntry selected pl4" fid="">
            <strong>All</strong><div class="addmain">
            </div>
        </div>
        <%= Html.FilterBoxList("collectionsList", Model.collectionsList)%>
    </div>
    <div id="categoryList" class="filter">
        <h4 class="acc">
        <img src="/Content/img/product_category.png" />
            product categories</h4>
        <div class="sideboxEntry selected pl4" fid="">
            <strong>All</strong><div class="addmain">
            </div>
        </div>
        <%= Html.FilterBoxList("categoryList", Model.categoryList)%>
    </div>
</div>
<div class="main_columnright">
    <div id="grid_content">
        <div class="buttonRow">
        <button id="newProduct" type="button" class="small green" title="add new product" onclick="javascript:window.location = '/dashboard/product/add';">
                new product</button>
            <button id="deleteProduct" type="button" class="small white" title="delete selected products">
                delete</button>
            <button id="privateProduct" type="button" class="small white" title="toggle active / inactive products">
                activate / deactivate</button>
                <button id="importProduct" type="button" class="small white">import</button>
                <button id="exportProduct" type="button" class="small white">export</button>
            <div id="search_area" class="fr">
    <span class="search"></span>
                    <input type="text" name="searchbox" id="searchInput" class="searchbox" />
    </div>
        </div>
        <table id="productGridView" class="scroll">
        </table>
        <div id="productGridNavigation" class="scroll ac">
        </div>
    </div>
</div>
<span id="filterBy" class="hidden"></span>
<span id="locationFilter" class="hidden"></span>
<span id="collectionsFilter" class="hidden"></span>
<span id="statusFilter" class="hidden"></span>
<script type="text/javascript">
    $(document).ready(function () {
        if (tradelr.webdb.db != null) {
            tradelr.views.sidecategory(function (result) {
                $('#categoryList', '#inventory_mine').children().first().next().nextAll().remove(); // remove all but ALL
                $('#categoryList', '#inventory_mine').append(result);
            });

            tradelr.views.sidelocation(function (result) {
                $('#locationList', '#inventory_mine').children().first().next().nextAll().remove(); // remove all but ALL
                $('#locationList', '#inventory_mine').append(result);
                pageInit();
            });
        }
        else {
            pageInit();
        }
    });

    function pageInit() {
        /////// button row
        $('#deleteProduct', '#inventory_mine').click(function () {
            if ($('.selected-row', '#productGridView').length == 0) {
                $.jGrowl('No products selected');
                return false;
            }

            var ok = window.confirm('Are you sure? There is no UNDO.');
            if (!ok) {
                return false;
            }

            var ids = [];
            var entries = $('.selected-row', '#productGridView');
            $.each(entries, function () {
                var id = $(this).attr('id');
                ids.push(id);
            });
            $.ajaxswitch({
                type: 'POST',
                url: '/dashboard/product/delete',
                data: { ids: ids.join(',') },
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        var idData = json_data.data;
                        if (ids.length != 0) {
                            for (var i = 0; i < idData.length; i++) {
                                sideSelector_delete(ids[i], '#inventory_mine');
                            }
                            // remove deleted products
                            reloadProductGrid(getFilterByField('#inventory_mine'));
                        }
                        else {
                            $.jGrowl('Delete fail. Product in use.');
                        }
                    }
                    else {
                        if (json_data.data == tradelr.returncode.NOPERMISSION) {
                            $.jGrowl('You do not have permission to delete products');
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

        ///// export
        $('#exportProduct', '#inventory_mine').click(function () {
            dialogBox_open('/dashboard/inventory/export');
            return false;
        });

        // import
        $('#importProduct', '#inventory_mine').click(function () {
            dialogBox_open('/dashboard/inventory/import');
            return false;
        });

        $('#privateProduct', '#inventory_mine').click(function () {
            if ($('.selected-row', '#productGridView').length == 0) {
                $.jGrowl('No products selected');
                return false;
            }

            var ids = [];
            var entries = $('.selected-row', '#productGridView');
            $.each(entries, function () {
                var id = $(this).attr('id');
                ids.push(id);
            });
            $.ajaxswitch({
                type: 'POST',
                url: '/dashboard/product/private',
                data: { ids: ids.join(',') },
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        for (var i = 0; i < ids.length; i++) {
                            sideSelector_delete(ids[i], '#inventory_mine');
                        }
                        // reload grid
                        reloadProductGrid(getFilterByField('#inventory_mine'));
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    return false;
                }
            });
            return false;

        });

        $('#searchInput', '#inventory_mine').keyup(function(e) {
            if (!isEnterKey(e)) {
                return false;
            }
            reloadProductGrid(getFilterByField());
            return false;
        });

        ////////////// category
        // bind side filter click events
        var categoryList = $('#categoryList', '#inventory_mine').find('.sideboxEntry');
        $.each(categoryList, function (i, val) {
            if (i > 0) {
                $(this).append('<div class="del" title="delete category"></div>');
                $(this).append('<div class="edit" title="edit products in category"></div>');
                $(this).append('<div class="add" title="add subcategory"></div>');
                $(this).prepend('<span class="arrow_right"></span>');
            }
        });

        $('#categoryList', '#inventory_mine').find('.sideboxSubEntry')
        .append('<div class="del" title="delete subcategory"></div>')
        .append('<div class="edit" title="edit products in category"></div>');

        $('#categoryList', '#inventory_mine').find('.sideboxEntry, .sideboxSubEntry').live("click", function () {
            // remove select class and highlight current one
            $('#categoryList', '#inventory_mine').find('.sideboxEntry, .sideboxSubEntry').removeClass('selected');
            $(this).addClass('selected');

            // reload grid
            setFilterByField($(this).attr('fid'));
            reloadProductGrid(getFilterByField('#inventory_mine'));
        });

        // for hiding/showing subcategories
        $('#categoryList', '#inventory_mine').find('.sideboxEntry > .arrow_right').live('click', function () {
            var next = $(this).parent().next('.sideboxSubEntry');
            var elements = []; // used to contain elements to toggle visibility
            while (next.length != 0) {
                elements.push(next);
                next = $(next).next('.sideboxSubEntry');
            }

            $(this).toggleClass('arrow_down');

            $.each(elements, function (i, val) {
                $(this).toggle();
            });
            return false;
        });

        // bind add subcategory
        $('#categoryList', '#inventory_mine').find('.add').live('click', function () {
            var id = $(this).parent().attr('fid');
            dialogBox_open("/dashboard/category/addsub/" + id, 400);
            return false;
        }).attr('title', 'add subcategory');

        //bind add main category
        $('#categoryList', '#inventory_mine').find('.addmain').live('click', function () {
            dialogBox_open("/dashboard/category/add", 400);
            return false;
        }).attr('title', 'add category');

        // bind del subcategory
        $('#categoryList > .sideboxSubEntry', '#inventory_mine').find('.del').live('click', function () {
            var ok = window.confirm("Are you sure? This will delete the category '" + $(this).parent().text() + "'. Products WILL NOT be deleted.");
            if (!ok) {
                return false;
            }
            var entry = this;
            var id = $(this).parent().attr('fid');
            $.ajaxswitch({
                type: 'POST',
                url: '/dashboard/category/delete/' + id,
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        $(entry).parent().slideUp(function () {
                            $(this).remove();
                        });
                        // reload grid
                        setFilterByField('', '#inventory_mine');
                        reloadProductGrid(getFilterByField('#inventory_mine'));
                        $.jGrowl('Product category successfully deleted');
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    return false;
                }
            });
            return false;
        });

        // bind edit category
        $('#categoryList .edit', '#inventory_mine').live("click", function () {
            var id = $(this).parent().attr('fid');
            var url = "/dashboard/category/edit/" + id;
            dialogBox_open(url, 710);
            return false;
        }).attr("disabled", true);

        // bind del category
        $('#categoryList > .sideboxEntry', '#inventory_mine').find('.del').live('click', function () {
            var ok = window.confirm("Are you sure? This will delete the category '" + $(this).parent().text() +
                                                                            "' and all its subcategories. Products WILL NOT be deleted.");
            if (!ok) {
                return false;
            }
            var id = $(this).parent().attr('fid');
            var entry = this;
            $.ajaxswitch({
                type: 'POST',
                url: '/dashboard/category/delete/' + id,
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        var next = $(entry).parent().next('.sideboxSubEntry');
                        var elements = [];
                        while (next.length != 0) {
                            elements.push(next);
                            next = $(next).next('.sideboxSubEntry');
                        }

                        $.each(elements, function (i, val) {
                            $(this).remove();
                        });
                        $(entry).parent().slideUp(function () {
                            $(this).remove();
                        });
                        // reload grid
                        setFilterByField('', '#inventory_mine');
                        reloadProductGrid(getFilterByField('#inventory_mine'));
                        $.jGrowl('Product category successfully deleted');
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    return false;
                }
            });
            return false;
        });

        //////////////////// collection
        $('#collectionsList', '#inventory_mine').find('.addmain').live('click', function () {
            dialogBox_open("/dashboard/collections/add", 500);
            return false;
        }).attr('title', 'add a collection');

        $('#collectionsList', '#inventory_mine').find('.sideboxEntry').live("click", function () {
            // remove select class and highlight current one
            $('#collectionsList', '#inventory_mine').find('.sideboxEntry').removeClass('selected');
            $(this).addClass('selected');

            // reload grid
            $('#collectionsFilter', '#inventory_mine').html($(this).attr('fid'));
            $('#searchInput', '#inventory_mine').val('');
            reloadProductGrid(getFilterByField('#inventory_mine'));
        });

        var collectionList = $('#collectionsList', '#inventory_mine').find('.sideboxEntry');
        $.each(collectionList, function (i, val) {
            // skip the first entry
            if (i > 0) {
                $(this).append('<div class="edit" title="edit products in collection"></div>');
                $(this).append('<div class="del" title="delete collection"></div>');
            }
        });

        // bind edit
        $('#collectionsList .edit', '#inventory_mine').live("click", function () {
            var id = $(this).parent().attr('fid');
            var url = "/dashboard/collections/edit/" + id;
            dialogBox_open(url, 710);
            return false;
        }).attr("disabled", true);

        // bind del 
        $('#collectionsList .del', '#inventory_mine').live('click', function () {
            var ok = window.confirm("Are you sure? This will delete the collection '" + $(this).parent().text() +
                                                                            "'. Products WILL NOT be deleted.");
            if (!ok) {
                return false;
            }
            var id = $(this).parent().attr('fid');
            var entry = this;
            $.ajaxswitch({
                type: 'POST',
                url: '/dashboard/collections/delete/' + id,
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        $(entry).parent().slideUp(function () {
                            $(this).remove();
                        });
                        // reload grid
                        setFilterByField('', '#inventory_mine');
                        reloadProductGrid(getFilterByField('#inventory_mine'));
                        $.jGrowl('Collection successfully deleted');
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    return false;
                }
            });
            return false;
        });

        ////////////////////// location
        var locationList = $('#locationList', '#inventory_mine').find('.sideboxEntry');
        $.each(locationList, function (i, val) {
            // skip the first 2 entries
            if (i > 1) {
                $(this).append('<div class="del" title="delete category"></div>');
            }
        });

        $('#locationList', '#inventory_mine').find('.sideboxEntry').live("click", function () {
            // remove select class and highlight current one
            $('#locationList', '#inventory_mine').find('.sideboxEntry').removeClass('selected');
            $(this).addClass('selected');

            // reload grid
            $('#locationFilter', '#inventory_mine').html($(this).attr('fid'));
            $('#searchInput', '#inventory_mine').val('');
            reloadProductGrid(getFilterByField('#inventory_mine'));
        });

        // delete location
        $('#locationList', '#inventory_mine').find('.del').live('click', function () {
            var ok = window.confirm('Are you sure you want to delete "' + $(this).parent().text() + '"? Inventory data at this location will be merged with your Main location.');
            if (!ok) {
                return false;
            }
            var id = $(this).parent().attr('fid');
            var entry = this;
            $.ajaxswitch({
                type: 'POST',
                url: '/dashboard/inventory/locationDelete/' + id,
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        $(entry).parent('.sideboxEntry').slideUp(function () {
                            $(this).remove();
                        });
                        // reload grid
                        setFilterByField('');
                        $('#locationFilter').html('');
                        reloadProductGrid(getFilterByField());
                        $.jGrowl('Inventory location successfully deleted');
                    }
                    else {
                        $.jGrowl('Unable to delete inventory location');
                    }
                }
            });
            return false;
        }).attr('title', 'remove location');

        // bind add inventory location
        $('#locationList', '#inventory_mine').find('.addmain').live('click', function () {
            dialogBox_open("/dashboard/inventory/locationAdd", 400);
            return false;
        }).attr('title', 'add inventory location');


        /////////// status filter
        $('#statusList', '#inventory_mine').find('.sideboxEntry').live("click", function () {
            // remove select class and highlight current one
            $('#statusList', '#inventory_mine').find('.sideboxEntry').removeClass('selected');
            $(this).addClass('selected');

            // reload grid
            $('#statusFilter', '#inventory_mine').html($(this).attr('fid'));
            $('#searchInput', '#inventory_mine').val('');
            reloadProductGrid(getFilterByField('#inventory_mine'));
        });

        // toggle collapsing filter
        $('.filter > h4').click(function () {
            if ($(this).siblings('.hidden').length == 0) {
                // hide
                $(this).siblings('div:not(.selected)').addClass('hidden');
            }
            else {
                // show
                $(this).siblings('div:not(.selected)').removeClass('hidden');
            }
            equaliseHeight('#inventory_mine');
        });

        $('.variant_transactions_link').live('click', function () {
            var href = $(this).attr('href');
            var row = $(this).closest('tr');
            var product_title = $.trim($(row).find('.pt').text());
            var id = $(row).attr('id');
            tradelr.tabs.add(href, '#tab_product_' + id, product_title, '#inventory_tabs');
            return false;
        });
        
    }; // end pageinit
</script>