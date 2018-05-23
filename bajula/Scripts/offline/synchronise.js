/// <reference path="offline.js" />
/// <reference path="sqlutil.js" />
tradelr.sync = {};

// sync handlers
tradelr.sync.getInsertString = function (tablename) {
    var table = tradelr.webdb.getTableEntry(tablename);
    var columnNameArray = [];
    var columnValArray = [];
    for (var i = 0; i < table.columns.length; i++) {
        var name = table.columns[i].name;
        // don't insert id
        if (name == 'id') {
            continue;
        }
        columnNameArray.push(name);
        columnValArray.push('?');
    }
    return "(" + columnNameArray.join(',') + ") VALUES (" + columnValArray.join(',') + ")";
};

tradelr.sync.getUpdateString = function (tablename) {
    var table = tradelr.webdb.getTableEntry(tablename);
    var returnString = '';
    for (var i = 0; i < table.columns.length; i++) {
        if (table.columns[i].name == 'id') {
            continue;
        }
        returnString += table.columns[i].name;
        returnString += "=?,";
    }
    return returnString.slice(0, returnString.length - 1);
};

// special handling for synch responses
tradelr.sync.getResponseDataArray = function (row, tablename) {
    var rows = [];
    var table = tradelr.webdb.getTableEntry(tablename);
    for (var i = 0; i < table.columns.length; i++) {
        var name = table.columns[i].name;
        if (name == 'id') {
            continue;
        }
        if (name == 'cflag') {
            if (tablename == 'photos' && row['type'] == 'PRODUCT') {
                rows.push(tradelr.webdb.flags.NOTLINKED);
            }
            else {
                rows.push(tradelr.webdb.flags.NONE);
            }
        } 
        else if (name == 'ccount') {
            rows.push(0);
        }
        else {
            rows.push(row[name]);
        }
    }
    if (DEBUG) {
        tradelr.webdb.rows = rows;
    }
    return rows;
};


tradelr.sync.handleResponse = function (rows, tablename) {
    tradelr.webdb.db.transaction(function (tx) {
        $.each(rows, function () {
            var cflag = this['cflag'];
            var id = this['id'];
            var serverid = this['serverid'];
            var rrow = this; // received row
            //try {
            switch (cflag) {
                case tradelr.webdb.flags.UPDATE:
                    if (serverid == null) {
                        if (DEBUG) {
                            tradelr.log("FAIL: trying to update " + tablename);
                        }
                    }
                    else {
                        var sql = 'UPDATE ' + tablename + ' SET ' + tradelr.sync.getUpdateString(tablename) + ' WHERE serverid=' + serverid;
                        if (DEBUG) {
                            tradelr.log(sql);
                        }
                        tx.executeSql(sql, tradelr.sync.getResponseDataArray(rrow, tablename),
                                null, tradelr.webdb.onDbError);
                    }
                    break;
                case tradelr.webdb.flags.DELETE: // this is not working
                    var sql = 'DELETE FROM ' + tablename + ' WHERE id=?';
                    if (DEBUG) {
                        tradelr.log(sql);
                    }
                    tx.executeSql(sql, [id], null, tradelr.webdb.onDbError);
                    break;
                case tradelr.webdb.flags.CREATE:
                    // for some tables we need to replace serverid with offline id ref
                    switch (tablename) {
                        case 'category':
                            var tables = ['category'];
                            var idarray = [rrow['parentid']];
                            tradelr.webdb.getOfflineID(tables, idarray, rrow, function (orow, result) {
                                var columns = ['parentid'];
                                var idarray = [orow['parentid']];
                                var abortInsert = false;
                                for (var i = 0; i < idarray.length; i++) {
                                    if (idarray[i] != null) {
                                        if (result[i] == null) {
                                            // cant find row
                                            abortInsert = true;
                                        }
                                        else {
                                            orow[columns[i]] = result[i];
                                        }
                                    }
                                }
                                if (!abortInsert) {
                                    var sql = 'INSERT INTO ' + tablename + tradelr.sync.getInsertString(tablename);
                                    if (DEBUG) {
                                        tradelr.log(sql);
                                    }
                                    tradelr.webdb.db.transaction(function (tx) {
                                        tx.executeSql(sql, tradelr.sync.getResponseDataArray(orow, tablename), null, tradelr.webdb.onDbError);
                                    });
                                }
                            });
                            break;
                        case 'inventorylocitem':
                            var tables = ['inventoryloc', 'products'];
                            var idarray = [rrow['locationid'], rrow['productid']];
                            tradelr.webdb.getOfflineID(tables, idarray, rrow, function (orow, result) {
                                var columns = ['locationid', 'productid'];
                                var idarray = [orow['locationid'], orow['productid']];
                                var abortInsert = false;
                                for (var i = 0; i < idarray.length; i++) {
                                    if (idarray[i] != null) {
                                        if (result[i] == null) {
                                            // cant find row
                                            abortInsert = true;
                                        }
                                        else {
                                            orow[columns[i]] = result[i];
                                        }
                                    }
                                }
                                if (!abortInsert) {
                                    var sql = 'INSERT INTO ' + tablename + tradelr.sync.getInsertString(tablename);
                                    if (DEBUG) {
                                        tradelr.log(sql);
                                    }
                                    tradelr.webdb.db.transaction(function (tx) {
                                        tx.executeSql(sql, tradelr.sync.getResponseDataArray(orow, tablename), null, tradelr.webdb.onDbError);
                                    });
                                }
                            });
                            break;
                        case 'photos':
                            // just insert, we'll do the link updates later because product table have not been created yet

                            var sql = 'INSERT INTO ' + tablename + tradelr.sync.getInsertString(tablename);
                            if (DEBUG) {
                                tradelr.log(sql);
                            }
                            tx.executeSql(sql, tradelr.sync.getResponseDataArray(rrow, tablename), null, tradelr.webdb.onDbError);
                            break;
                        case 'products':
                            var tables = ['category', 'photos', 'stockunit'];
                            var idarray = [rrow['categoryid'], rrow['thumbnailid'], rrow['stockunitid']];
                            tradelr.webdb.getOfflineID(tables, idarray, rrow, function (orow, result) {
                                var columns = ['categoryid', 'thumbnailid', 'stockunitid'];
                                var idarray = [orow['categoryid'], orow['thumbnailid'], orow['stockunitid']];
                                var abortInsert = false;
                                for (var i = 0; i < idarray.length; i++) {
                                    if (idarray[i] != null) {
                                        if (result[i] == null) {
                                            // cant find row
                                            abortInsert = true;
                                        }
                                        else {
                                            orow[columns[i]] = result[i];
                                        }
                                    }
                                }
                                if (!abortInsert) {
                                    var sql = 'INSERT INTO ' + tablename + tradelr.sync.getInsertString(tablename);
                                    if (DEBUG) {
                                        tradelr.log(sql);
                                    }
                                    tradelr.webdb.db.transaction(function (tx) {
                                        tx.executeSql(sql, tradelr.sync.getResponseDataArray(orow, tablename), null, tradelr.webdb.onDbError);
                                    });
                                }
                            });
                            break;
                        default:
                            var sql = 'INSERT INTO ' + tablename + tradelr.sync.getInsertString(tablename);
                            if (DEBUG) {
                                tradelr.log(sql);
                            }
                            tx.executeSql(sql, tradelr.sync.getResponseDataArray(rrow, tablename), null, tradelr.webdb.onDbError);
                            break;
                    }
                    break;
                case tradelr.webdb.flags.CLEAR: // responses due to offline initiated syncs
                    // get ccount and cflag
                    var sql = "SELECT ccount,cflag FROM " + tablename + " WHERE id=" + id;
                    tradelr.webdb.sqlGetRows(sql, [], id, function (results) {
                        var row = results.item(0);
                        var ccount = row['ccount'];
                        var cflag = row['cflag'];
                        switch (cflag) {
                            case tradelr.webdb.flags.CREATE:
                                if (ccount > 0) {
                                    throw 'FAIL: modification of row that has been created (cflag not set to UPDATE) table:' + tablename + " id:" + id;
                                }
                                var item = {
                                    tablename: tablename,
                                    params: [{ cflag: tradelr.webdb.flags.NONE, ccount: 0, serverid: serverid}],
                                    matches: [{ id: id}]
                                };
                                tradelr.webdb.sqlUpdateRows([item], false, null);
                                break;
                            case tradelr.webdb.flags.UPDATE:
                                var newcflag = tradelr.webdb.flags.NONE;
                                var newccount = 0;
                                if (ccount > 1) {
                                    // row has been modified again since we were away, set to 1 to mark it for synchronisation
                                    newccount = 1;
                                    newcflag = tradelr.webdb.flags.UPDATE;
                                }
                                var item = {
                                    tablename: tablename,
                                    params: [{ cflag: newcflag, ccount: newccount}],
                                    matches: [{ id: id}]
                                };
                                tradelr.webdb.sqlUpdateRows([item], false, null);
                                break;
                            case tradelr.webdb.flags.DELETE:
                                // UPDATE -> DELETE = ccount > 0   ;OK
                                var item = {
                                    tablename: tablename,
                                    matches: [{ id: id}]
                                };
                                tradelr.webdb.sqlDeleteRows([item], false);
                                break;
                            default:
                                if (DEBUG) {
                                    tradelr.log('unknown client flag');
                                    tradelr.log(row);
                                }
                                break;
                        }
                    });
                    break;
                default:
                    if (DEBUG) {
                        tradelr.log('unknown server flag');
                        tradelr.log(row);
                    }
                    break;
            }
            //} catch (e) {
            //    if (DEBUG) {
            //        throw e;
            //    }
            //}

        });
    });


};

// timer to poll offline status
tradelr.sync.init = function () {
    if (navigator.onLine && tradelr.webdb.db != null) {
        tradelr.sync.start();
    }
    if (DEBUG) {
       // return false;
    }
    setInterval(function () {
        if (navigator.onLine && tradelr.webdb.db != null) {
            tradelr.sync.start();
        }
    }, 60000);
};

tradelr.sync.start = function () {
    // order by no dependecies first
    tradelr.webdb.getDirtyEntries('settings', function () {
        tradelr.webdb.getDirtyEntries('stockunit', function () {
            tradelr.webdb.getDirtyEntries('inventoryloc', function () {
                tradelr.webdb.getDirtyEntries('category', function () {
                    tradelr.webdb.getDirtyEntries('photos', function () {
                        tradelr.webdb.getDirtyEntries('products', function () {
                            tradelr.webdb.sqlLinkUpdate('photos', function () {
                                tradelr.webdb.getDirtyEntries('inventorylocitem', function () {
                                    // send update lastOfflineCheck date to server
                                    $.post('/sync/touched');
                                });
                            });
                        });
                    });
                });
            });
        });
    });

};