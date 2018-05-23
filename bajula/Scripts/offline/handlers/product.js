tradelr.product = {};

tradelr.product.create = function (params, callback) {
    tradelr.product.modify(params, false, callback);
};

tradelr.product.list = function (cat, rowcount, page, sidx, sord, alarm, location, callback) {
    var sql = "SELECT p.id, p.sellingPrice, p.SKU, p.title,ph.url, c.name, SUM(loc.inventoryLevel), SUM(loc.onOrder), setting.currency FROM products AS p, settings AS setting " +
        " LEFT OUTER JOIN photos AS ph ON p.thumbnailid = ph.id AND ph.cflag !=" + tradelr.webdb.flags.DELETE +
        " LEFT OUTER JOIN inventorylocitem AS loc ON loc.productid = p.id";
    if (location) {
        sql += " AND loc.locationid = " + location;
    }

    sql += " LEFT OUTER JOIN category AS c ON c.id = p.categoryid";
    if (cat) {
        sql += " WHERE p.categoryid = " + cat + " OR c.parentid = " + cat;
    }
    if (sql.indexOf('WHERE') == -1) {
        sql += " WHERE p.cflag !=" + tradelr.webdb.flags.DELETE;
    }
    else {
        sql += " AND p.cflag !=" + tradelr.webdb.flags.DELETE;
    }
    sql += " GROUP BY p.id,p.SKU,p.title,ph.url,c.name,p.sellingPrice";
    if (sidx != undefined && sord != undefined) {
        if (sord == 'asc') {
            if (sidx == 'id') {
                sql += " ORDER BY p.serverid";
            }
            else {
                sql += " ORDER BY upper(p." + sidx + ")";
            }

        }
        else if (sord == 'desc') {
            if (sidx == 'id') {
                sql += " ORDER BY p.serverid DESC";
            }
            else {
                sql += " ORDER BY upper(p." + sidx + ") DESC";
            }

        }
    }

    tradelr.webdb.sqlGetRows(sql, [], null, function (results) {
        if (results == null) {
            return tradelr.webdb.ajaxsuccess(results, callback);
        }
        var records = results.length;
        var total = Math.floor(records / rowcount);
        if (records % rowcount != 0) {
            total++;
        }
        var rows = [];
        var start = rowcount * (page - 1);
        var limit;
        if ((records - start) < rowcount) {
            limit = records;
        }
        else {
            limit = parseInt(rowcount, 10) + parseInt(start, 10);
        }
        for (var i = start; i < limit; i++) {
            var datarow = results.item(i);
            try {
                var cell = [
                    datarow['id'],
                    "",
                    datarow['url'] ? "<img src='" + datarow['url'] + "' class='jqgrid_thumb' />" : "<img src='/Content/img/thumbs_none.png' />",
                    datarow['SKU'],
                    tradelr.product.ToProductTitle(datarow['title'], datarow['sellingPrice'], datarow['name'], datarow['currency']),
                    datarow['SUM(loc.inventoryLevel)'],
                    datarow['SUM(loc.onOrder)'],
                    "n/a",
                    ""
                ];

                var row = {
                    id: datarow['id'],
                    cell: cell
                };
                rows.push(row);
            } catch (e) {
                if (DEBUG) {
                    tradelr.row = datarow;
                    tradelr.log(e.message + " " + datarow['id'] + " " + start + " " + limit);
                }
            }

        }
        var data = {
            page: page,
            total: total,
            records: records,
            rows: rows
        };
        if (DEBUG) {
            tradelr.rows = rows;
        }
        tradelr.webdb.ajaxsuccess(data, callback);
    });
};

tradelr.product.ToProductTitle = function (title, price, category, currency) {
    if (price && category) {
        return ["<span class='pt'>", title, "</span><span class='pc'>",
                    category, ", </span><span class='sp'>", currency, parseFloat(price).toFixed(2), "</span>"].join('');
    }
    if (price) {
        return ["<span class='pt'>", title, "</span><span class='sp'>", currency,
                parseFloat(price).toFixed(2), "</span>"].join('');
    }
    if (category) {
        return ["<span class='pt'>", title, "</span><span class='pc'>",
                    category, "</span>"].join('');
    }

    return ["<span class='pt'>", title, "</span>"].join('');
};

// either create or update
tradelr.product.modify = function (params, isUpdate, callback) {
    var identifier;
    var postObj = convertParamStringToObject(params);
    var cflag = tradelr.webdb.flags.CREATE;
    if (isUpdate) {
        identifier = tradelr.webdb.parseinteger(postObj['id']);
        cflag = tradelr.webdb.flags.UPDATE;
    }

    // now need to separate them into separate tables
    var rows = [];
    // update product
    var productParam = {};
    if (postObj.costPrice) {
        productParam.costPrice = tradelr.webdb.parsestring(postObj.costPrice);
    }
    if (postObj.defaultPhotoID) {
        productParam.thumbnailid = tradelr.webdb.parseinteger(postObj.defaultPhotoID);
    }
    if (postObj.details) {
        productParam.details = postObj.details;
    }
    if (postObj.subcategory) {
        productParam.categoryid = tradelr.webdb.parseinteger(postObj.subcategory);
    }
    else if (postObj.maincategory) {
        productParam.categoryid = tradelr.webdb.parseinteger(postObj.maincategory);
    }
    else {
        productParam.categoryid = null;
    }
    if (postObj.notes) {
        productParam.notes = postObj.notes;
    }
    if (postObj.sellingPrice) {
        productParam.sellingPrice = tradelr.webdb.parsestring(postObj.sellingPrice);
    }
    if (postObj.sku) {
        productParam.SKU = postObj.sku;
    }
    if (postObj.title) {
        productParam.title = postObj.title;
    }
    if (postObj.stockUnit) {
        productParam.stockunitid = tradelr.webdb.parseinteger(postObj.stockUnit);
    }

    // set appropriate flag
    productParam.cflag = cflag;

    // for use later
    var addInventoryLocation = function (productid, isUpdate, callback) {
        // update inventoryloc items
        if (postObj.location) {
            var inventlocitemParam = [];
            var inventlocitemMatch = [];
            if (postObj.location.length) {
                for (var i = 0; i < postObj.location.length; i++) {
                    var item = {
                        inventoryLevel: tradelr.webdb.parseinteger(postObj.inStock[i]),
                        alarmLevel: tradelr.webdb.parseinteger(postObj.reorderLevel[i]),
                        locationid: tradelr.webdb.parseinteger(postObj.location[i]),
                        productid: productid,
                        cflag: cflag
                    };
                    inventlocitemParam.push(item);
                    var match = {
                        locationid: tradelr.webdb.parseinteger(postObj.location[i]),
                        productid: productid
                    };
                    inventlocitemMatch.push(match);
                }
            }
            else {
                var item = {
                    inventoryLevel: tradelr.webdb.parseinteger(postObj.inStock),
                    alarmLevel: tradelr.webdb.parseinteger(postObj.reorderLevel),
                    cflag: cflag
                };
                inventlocitemParam.push(item);
                var match = {
                    locationid: tradelr.webdb.parseinteger(postObj.location),
                    productid: productid
                };
                inventlocitemMatch.push(match);
            }

            rows.push({
                tablename: 'inventorylocitem',
                params: inventlocitemParam,
                matches: inventlocitemMatch
            });
        }

        if (isUpdate) {
            tradelr.webdb.sqlUpdateRows(rows, true, function () {
                var result = {
                    success: true,
                    data: productid
                };
                callback(result);
            });
        }
        else {
            tradelr.webdb.sqlInsertRows(rows, function () {
                var result = {
                    success: true,
                    data: productid
                };
                callback(result);
            });
        }
    } // addInventoryLocation


    // if create we need to add product first because we need the product id
    if (isUpdate) {
        rows.push({
            tablename: 'products',
            params: [productParam],
            matches: [{ id: identifier}]
        });
        addInventoryLocation(identifier, isUpdate, callback);
    }
    else {
        // create product
        var p = {
            tablename: 'products',
            params: [productParam]
        };
        tradelr.webdb.sqlInsertRows([p], function (rowids) {
            var productid = rowids[0];
            addInventoryLocation(productid, isUpdate, callback);
        });
    }
};

tradelr.product.remove = function (productid, ids, callback) {
    
    // TODO: need to handle multiple deletes
    // delete photos
    tradelr.photo.removeByProduct(productid);

    // delete inventoryloc items
    tradelr.inventory.location.removeItem(productid);

    // delete product
    var item = {
        tablename: 'products',
        matches: [{ id: productid}]
    };

    tradelr.webdb.sqlDeleteRows([item], true, function () {
        var result = {
            success: true,
            data: productid
        };
        callback(result);
    });
};

// params is post parameter string
tradelr.product.update = function (params, callback) {
    tradelr.product.modify(params, true, callback);
};