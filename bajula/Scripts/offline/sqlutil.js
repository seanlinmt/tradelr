/// <reference path="../root/general.js" />
/// <reference path="offline.js" />
/// <reference path="synchronise.js" />
tradelr.webdb.ajaxsuccess = function (data, callback) {
    if (data == null) {
        data = {};
    }
    var resp = {
        statusText: 'OK',
        status: '200',
        responseText: $.toJSON(data)
    };
    callback(resp, 'success');
};


tradelr.webdb.emptyIfNull = function (txt) {
    return txt == null ? "" : txt;
};

// functions to handle various data types
tradelr.webdb.parsestring = function (val) {
    return val ? val : null;
};

tradelr.webdb.parseinteger = function (val) {
    return val ? parseInt(val, 10) : null;
};

tradelr.webdb.parsefloat = function (val) {
    if (val == null || val == undefined) {
        return null;
    }
    if (typeof val == "string") {
        return parseFloat(val);
    }
    return val;
};

// CRUD utils for webdb
// usually a row of items to match
// matches: array of columns and values to match with
// params: array of columns and values to operate on
tradelr.webdb.sqlDeleteRows = function (rows, markOnly, callback) {
    if (DEBUG) {
        tradelr.log(rows);
    }

    tradelr.webdb.db.transaction(function (tx) {
        for (var g = 0; g < rows.length; g++) {
            var tablename = rows[g].tablename;
            var matches = rows[g].matches;
            for (var h = 0; h < matches.length; h++) {
                var matchArray = [];
                for (var prop in matches[h]) {
                    matchArray.push([prop, "=", matches[h][prop]].join(''));
                }
                var sql = '';
                if (markOnly) {
                    sql = 'UPDATE ' + tablename + ' SET cflag=' + tradelr.webdb.flags.DELETE + ' WHERE ' + matchArray.join(' AND ');
                }
                else {
                    sql = 'DELETE FROM ' + tablename + ' WHERE ' + matchArray.join(' AND ');
                }

                if (DEBUG) {
                    tradelr.log(sql);
                }

                tx.executeSql(sql, [], function (tx, results) {
                    if (!results.rowsAffected) {
                        if (DEBUG) {
                            tradelr.log('DELETE FAIL, ' + sql);
                        }
                    }
                }, tradelr.webdb.onDbError);
            }
        }
    }, tradelr.webdb.onDbError, function () {
        if (callback != undefined) {
            callback();
        }
    });
};

tradelr.webdb.sqlGetRows = function (sql, params, identifier, callback) {
    if (DEBUG) {
        tradelr.log(sql);
    }
    tradelr.webdb.db.readTransaction(function (tx) {
        tx.executeSql(sql, params, function (tx, results) {
            if (DEBUG) {
                tradelr.sqlrows = results.rows;
            }
            if (callback != undefined) {
                if (results.rows.length == 0) {
                    callback(null, identifier);
                }
                else {
                    callback(results.rows, identifier);
                }
            }
        }, tradelr.webdb.onDbError);
    });
};

tradelr.webdb.sqlGetValue = function (sql, params, identifier, callback) {
    if (DEBUG) {
        tradelr.log(sql);
    }
    tradelr.webdb.db.readTransaction(function (tx) {
        tx.executeSql(sql, params, function (tx, results) {
            var row = null;
            if (results.rows.length > 0) {
                row = results.rows.item(0);
            }

            if (DEBUG) {
                tradelr.sqlval = row;
            }
            if (callback != undefined) {
                callback(row, identifier);
            }
        }, tradelr.webdb.onDbError);
    });
};

tradelr.webdb.sqlInsertRows = function (rows, callback) {
    if (DEBUG) {
        tradelr.log(rows);
    }

    var rowids = [];
    tradelr.webdb.db.transaction(function (tx) {
        for (var g = 0; g < rows.length; g++) {
            var tablename = rows[g].tablename;
            var params = rows[g].params;
            for (var h = 0; h < params.length; h++) {
                var names = [];
                var values = [];
                for (var prop in params[h]) {
                    // this exist if insert when update of row not found
                    if (prop == 'cflag' || prop == 'ccount' || prop == 'serverid') {
                        continue;
                    }
                    names.push(prop);
                    values.push(params[h][prop]);
                }

                // add ccount
                names.push('ccount');
                values.push(0);

                // add CREATE flag
                names.push('cflag');
                values.push(tradelr.webdb.flags.CREATE);

                names.push('serverid');
                values.push(null);

                var paramArray = [];
                for (var i = 0; i < names.length; i++) {
                    paramArray.push('?');
                }

                var sql = 'INSERT INTO ' + tablename + ' (' + names.join(',') + ') VALUES (' + paramArray.join(',') + ')';

                if (DEBUG) {
                    tradelr.log(sql);
                }

                tx.executeSql(sql, values, function (tx, result) {
                    rowids.push(result.insertId);
                }, tradelr.webdb.onDbError);
            }
        }

    }, tradelr.webdb.onDbError, function () {
        if (callback != undefined) {
            callback(rowids);
        }
    });
};

tradelr.webdb.sqlLinkUpdate = function (tablename, callback) {
    switch (tablename) {
        case 'photos':
            //  get rows with unlinked flag set
            var sql = "SELECT * FROM photos WHERE cflag=" + tradelr.webdb.flags.NOTLINKED;
            tradelr.webdb.sqlGetRows(sql, [], null, function (rows) {
                if (rows != null) {
                    for (var i = 0; i < rows.length; i++) { /////////// here's an i loop
                        var row = rows.item(i);
                        var tables = [];
                        switch (row['type']) {
                            case 'PRODUCT':
                                tables.push('products');
                                break;
                            case 'PROFILE':
                                break;
                            case 'COMPANY':
                                break;
                            default:
                                break;
                        }
                        if (tables.length != 0) {
                            var idarray = [row['contextid']];
                            tradelr.webdb.getOfflineID(tables, idarray, row, function (orow, result) {
                                var abortUpdate = false;
                                for (var j = 0; j < idarray.length; j++) {
                                    if (idarray[j] != null) {
                                        if (result[j] == null) {
                                            // cant find row
                                            abortUpdate = true;
                                        }
                                    }
                                }
                                if (!abortUpdate) {
                                    var sql = 'UPDATE photos SET contextid=?,cflag=? WHERE id=?';
                                    var params = [result[0], tradelr.webdb.flags.NONE, orow['id']];
                                    if (DEBUG) {
                                        tradelr.log(sql);
                                    }
                                    tradelr.webdb.db.transaction(function (tx) {
                                        tx.executeSql(sql, params, null, tradelr.webdb.onDbError);
                                    });
                                }
                            });
                        }
                    }
                }
                callback();
            });
            break;
        default:
            break;
    }
};

// params = parameter object with matching property names
// matches = object of fields to match
tradelr.webdb.sqlUpdateRows = function (rows, insertIfNotExist, callback) {
    if (DEBUG) {
        tradelr.log(rows);
    }

    tradelr.webdb.db.transaction(function (tx) {

        var executeUpdate = function (sql, values, tablename, params) {
            tx.executeSql(sql, values, function (tx, results) {
                if (!results.rowsAffected) {
                    // this means row does not exist so we need to create it
                    if (insertIfNotExist) {
                        tradelr.webdb.sqlInsertRows([{ tablename: tablename, params: [params]}]);
                    }
                }
            }, tradelr.webdb.onDbError);
        };

        for (var g = 0; g < rows.length; g++) {
            var tablename = rows[g].tablename;
            var params = rows[g].params;
            var matches = rows[g].matches;
            for (var h = 0; h < params.length; h++) {
                var names = [];
                var values = [];
                var parameterArray = [];

                for (var prop in params[h]) {
                    // handle column increments
                    var value = params[h][prop];
                    if (value.indexOf(prop + "+") == 0) {
                        parameterArray.push(prop + "=" + value + ",");
                    }
                    else {
                        names.push(prop);
                        values.push(value);
                    }
                }
                for (var i = 0; i < names.length; i++) {
                    parameterArray.push(names[i]);
                }

                if (parameterArray.indexOf('ccount') == -1) {
                    parameterArray.push('ccount=ccount+1');  // this will break code at [1]
                }

                // join params
                var parameterString;
                for (var i = 0; i < parameterArray.length; i++) {
                    var param = parameterArray[i];
                    if (param.charAt(param.length - 1) == ',') {
                        parameterString += param;
                    }
                    else if (i == parameterArray.length - 1) {  // handles last entry
                        if (param != 'ccount=ccount+1') {  // [1]
                            parameterString += '=?';
                        }
                    }
                    else {
                        parameterString += param;
                        parameterString += '=?,';
                    }
                }

                var matchArray = [];
                for (var prop in matches[h]) {
                    matchArray.push([prop, "=", matches[h][prop]].join(''));
                }
                var sql = 'UPDATE ' + tablename + ' SET ' + parameterString + ' WHERE ' + matchArray.join(' AND ');

                if (DEBUG) {
                    tradelr.log(sql);
                }
                executeUpdate(sql, values, tablename, params[h]);
            }
        }

    }, tradelr.webdb.onDbError, callback);
};

