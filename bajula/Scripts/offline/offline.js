/// <reference path="../root/general.js" />
/// <reference path="synchronise.js" />
/// <reference path="sqlutil.js" />

tradelr.webdb = {};
tradelr.webdb.version = "1"; // if version is different we need to execute ALTER statements to change schema

tradelr.webdb.flags = {};
tradelr.webdb.flags.NONE = 0;
tradelr.webdb.flags.UPDATE = 1;
tradelr.webdb.flags.DELETE = 2;
tradelr.webdb.flags.CREATE = 3;
tradelr.webdb.flags.CLEAR = 4;
tradelr.webdb.flags.NOTLINKED = 5; // to identify rows that need to delay rowid linking updates
/*
tradelr.webdb.flags.isSet = function (value, flag) {
    return value & flag;
};

tradelr.webdb.flags.set = function (value, flag) {
    return value | flag;
};

tradelr.webdb.flags.unset = function (value, flag) {
    return value & ~flag;
};
*/
tradelr.webdb.db = null;

tradelr.webdb.createTables = function (tx) {
    for (var index = 0; index < tradelr.webdb.tables.length; index++) {
        var table = tradelr.webdb.tables[index];
        tx.executeSql("CREATE TABLE IF NOT EXISTS " + table.name + tradelr.webdb.getCreateString(table.name) + ";");
    }
};

tradelr.webdb.getCreateString = function (tablename) {
    var table = tradelr.webdb.getTableEntry(tablename);
    var columnArray = [];
    for (var i = 0; i < table.columns.length; i++) {
        columnArray.push(table.columns[i].name + ' ' + table.columns[i].type);
    }
    return '(' + columnArray.join(',') + ')';
};

tradelr.webdb.getDirtyEntries = function (tablename, callback) {
    if (tradelr.webdb.db == null) {
        tradelr.error('failed to open db');
        return false;
    }
    var sql = "";
    // replaces refid with serverid of corresponding link to be sent to server
    switch (tablename) {
        case "category":
            sql = "SELECT pc.serverid AS parentid, c.* FROM category AS c " +
            " LEFT OUTER JOIN category AS pc ON pc.id = c.parentid " +
            " WHERE c.cflag != 0 OR c.serverid = (SELECT MAX(serverid) FROM category)";
            break;
        case "inventorylocitem":
            sql = "SELECT loc.serverid AS locationid, p.serverid AS productid, iloc.* FROM inventorylocitem AS iloc " +
            "LEFT OUTER JOIN inventoryloc AS loc ON loc.id = iloc.locationid " +
            "LEFT OUTER JOIN products AS p ON p.id = iloc.productid " +
            " WHERE iloc.cflag != 0 OR iloc.serverid = (SELECT MAX(serverid) FROM inventorylocitem)";
            break;
        case "photos":
            sql = "SELECT p.serverid AS contextid, ph.* FROM photos AS ph " +
            " LEFT OUTER JOIN products AS p ON p.id = ph.contextid" +
            " WHERE (ph.cflag != 0 AND ph.cflag != 5 AND ph.type = 'PRODUCT') OR ph.serverid = (SELECT MAX(serverid) FROM photos WHERE cflag != 5)";
            break;
        case "products":
            sql = "SELECT c.serverid AS categoryid, ph.serverid AS thumbnailid, p.* FROM products AS p " +
            "LEFT OUTER JOIN category AS c ON c.id = p.categoryid " +
            "LEFT OUTER JOIN photos AS ph ON ph.id = p.thumbnailid " +
            "LEFT OUTER JOIN stockunit AS s ON s.id = p.stockunitid " +
            " WHERE p.cflag != 0 OR p.serverid = (SELECT MAX(serverid) FROM products)";
            break;
        default:
            sql = "SELECT * FROM " + tablename + " WHERE cflag != 0 OR serverid = (SELECT MAX(serverid) FROM " + tablename + ")";
            break;
    }
    tradelr.webdb.db.readTransaction(function (tx) {
        tx.executeSql(sql, [], function (tx, results) {
            //console.log('callback from getDirtyEntries');
            var rows = [];
            for (var i = 0; i < results.rows.length; i++) {
                rows.push(results.rows.item(i));
            }

            var req = {
                type: tablename,
                data: $.toJSON(rows)
            };
            // post sync info to server
            tradelr.hideLoader = true;
            $.ajax({
                type: 'POST',
                url: '/sync',
                data: req,
                complete: function () {
                    tradelr.hideLoader = false;

                },
                success: function (json_result) {
                    if (json_result && json_result.success) {
                        tradelr.sync.handleResponse(json_result.data, tablename);
                    }
                    else {

                    }
                    callback();   // so if there's an error synch will stop and not cause strange happenings
                },
                dataType: 'json'
            });

        }, tradelr.webdb.onDbError); // executesql
    });
};

tradelr.webdb.getOfflineID = function (tables, ids, originalrow, callback) {
    // all input are arrays of the same length
    var alias = 'a';
    var _ids = [];
    var _tables = [];
    var _aliases = [];
    // we only want to get if ref id is not null
    for (var i = 0; i < ids.length; i++) {
        var id = ids[i];
        if (id != null) {
            _aliases.push(alias + i + '.id AS ' + alias + i + 'id');
            _ids.push(alias + i + '.serverid=' + id);
            _tables.push(tables[i] + ' AS ' + alias + i);
        }
    }
    // no result is returned if one of the WHEREs dont succeed
    if (_ids.length > 0) {
        var sql = "SELECT " + _aliases.join(',') + " FROM " + _tables.join(',') + " WHERE " + _ids.join(' AND ');
        if (DEBUG) {
            tradelr.log(sql);
        }
        tradelr.webdb.sqlGetValue(sql, [], null, function (row) {
            var result = [];
            if (row != null) {
                for (var i = 0; i < ids.length; i++) {
                    if (ids[i] != null) {
                        result.push(row[alias + i + 'id']);
                    }
                    else {
                        result.push(null);
                    }
                }
            }
            callback(originalrow, result);
        });
    }
    else {
        // ref ids are null
        var result = [];
        for (var i = 0; i < ids.length; i++) {
            result.push(null);
        }
        callback(originalrow, result);
    }

}

tradelr.webdb.getTableEntry = function (tablename) {
    for (var i = 0; i < tradelr.webdb.tables.length; i++) {
        var table = tradelr.webdb.tables[i];
        if (table.name == tablename) {
            return table;
        }
    }
    return null;
};

tradelr.webdb.init = function () {
    // no need access to db if no manifest is defined
    if (!$('html').attr('manifest')) {
        return false;
    }
    if (!tradelr.webdb.open()) {
        tradelr.error('failed to open db');
        return false;
    }

    if (tradelr.webdb.db.version != tradelr.webdb.version) {
        //console.log('db version change');
        tradelr.webdb.db.changeVersion(tradelr.webdb.db.version, tradelr.webdb.version,
            tradelr.webdb.createTables,
            tradelr.webdb.onDbError,
            function () {
                tradelr.webdb.start(); 
            });
    }
    else {
        tradelr.webdb.db.transaction(
                tradelr.webdb.createTables,
                tradelr.webdb.onDbError,
                function () {
                    tradelr.webdb.start(); 
                });
    }
};

tradelr.webdb.loadData = function () {
    tradelr.sync.init();
};

// error functions
// errors, http://kobesearch.cpan.org/htdocs/HTML-DOM/HTML/DOM/Exception.pm.html#INVALID_STATE_ERR_11
tradelr.webdb.onDbError = function (tx, e) {
    //console.log(tx);
    tradelr.error(e.message);
};

// open db connection
tradelr.webdb.open = function () {
    var dbsize = 50 * 1024 * 1024; // 50 mb
    if (typeof window.openDatabase == "undefined") {
        return false;
    }

    try {
        tradelr.webdb.db = openDatabase('tradelr', '', 'tradelr Local DB', dbsize);
    }
    catch (e) {
        tradelr.error(e.message);
        return false;
    }
    return true;
};

tradelr.webdb.start = function (tx) {
    tradelr.webdb.loadData();
};
