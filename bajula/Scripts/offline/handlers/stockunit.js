tradelr.stockunit = {};
tradelr.stockunit.add = function (title, callback) {
    // check that stock unit does not already exist
    tradelr.stockunit.exist(title, function (exist) {
        if (exist) {
            var item = {
                success: false,
                message: 'Stock Unit already exist'
            };
            callback(item);
        }
        else {
            var row = {
                tablename: 'stockunit',
                params: [{ name: title}]
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

tradelr.stockunit.exist = function (name, callback) {
    tradelr.webdb.sqlGetValue("SELECT id FROM stockunit WHERE name=?", [name], null, function (row) {
        if (row == null) {
            callback(false);
        }
        else {
            callback(true);
        }
    });
};