tradelr.category = {};
tradelr.category.add = function (title, callback) {
    // check that category does not already exist
    tradelr.category.exist(title, null, function (exist) {
        if (exist) {
            var item = {
                success: false,
                message: 'Category already exist'
            };
            callback(item);
        }
        else {
            var row = {
                tablename: 'category',
                params: [{ name: title, parentid: null}]
            };
            tradelr.webdb.sqlInsertRows([row], function (rowids) {
                var item = {
                    success: true,
                    data: {
                        title: title,
                        id: rowids[0]
                    }
                };
                callback(item);
            });
        }
    });
};

tradelr.category.addsub = function (title, parentid, callback) {
    tradelr.category.exist(title, parentid, function (exist) {
        if (exist) {
            var item = {
                success: false,
                message: 'Category already exist'
            };
            callback(item);
        }
        else {
            var row = {
                tablename: 'category',
                params: [{ name: title, parentid: parentid}]
            };
            tradelr.webdb.sqlInsertRows([row], function (rowids) {
                var item = {
                    success: true,
                    data: {
                        title: title,
                        id: rowids[0]
                    }
                };
                callback(item);
            });
        }
    });

};

tradelr.category.exist = function (name, parentid, callback) {
    if (parentid == null) {
        tradelr.webdb.sqlGetValue("SELECT id FROM category WHERE name=?", [name], null, function (row) {
            if (row == null) {
                callback(false);
            }
            else {
                callback(true);
            }
        });
    }
    else {
        tradelr.webdb.sqlGetValue("SELECT id FROM category WHERE name=? AND parentid=?", [name, parentid], null, function (row) {
            if (row == null) {
                callback(false);
            }
            else {
                callback(true);
            }
        });
    }
};

tradelr.category.getsub = function (id, callback) {
    var sql = "SELECT * FROM category WHERE parentid=?";
    tradelr.webdb.sqlGetRows(sql, [id], id, function (rows) {
        var cats = [];
        if (rows != null) {
            for (var i = 0; i < rows.length; i++) {
                var row = rows.item(i);
                var cat = {
                    id: row['id'],
                    title: row['name']
                };
                cats.push(cat);
            }
        }
        var item = {
            success: true,
            data: cats
        };
        callback(item);
    });
};

tradelr.category.remove = function (id, callback) {
    // delete category and all subcategories
    var rows = [{
        tablename: 'category',
        matches: [{ id: id }, { parentid: id}]
    }];

    tradelr.webdb.sqlDeleteRows(rows, true, function () {
        // null category in products
        var p = {
            tablename: 'products',
            params: [{ categoryid: null}],
            matches: [{ categoryid: id}]
        };
        tradelr.webdb.sqlUpdateRows(p, false, function () {
            var item = {
                success: true,
                data: id
            };
            callback(item);
        });
    });
};