tradelr.inventory = {};

tradelr.inventory.location = {};

tradelr.inventory.location.order = function (a, b) {
    if (a.locid > b.locid) {
        return -1;
    }
    if (a.locid < b.locid) {
        return 1;
    }
    return 0;
};

tradelr.inventory.location.remove = function (locationid, callback) {
    // need to get MAIN locationid first
    tradelr.webdb.sqlGetValue("SELECT id FROM inventoryloc WHERE name='Main'", [], null, function (mainrow) {
        if (mainrow != null) {
            var mainlocationid = mainrow['id'];

            // need to move locitems to main entry
            var sql = "SELECT * FROM inventorylocitem WHERE locationid = ?";
            tradelr.webdb.sqlGetRows(sql, [locationid], null, function (rows) {
                var tobedeletedentries = [];
                // get all ilocitems matching 
                for (var i = 0; i < rows.length; i++) {
                    var row = rows.item(i);
                    tobedeletedentries.push({ productid: row['productid'], ilevel: row['inventoryLevel'], onOrder: row['onOrder'] });
                }

                var update = {
                    tablename: 'inventorylocitem',
                    params: [],
                    matches: []
                };
                for (var i = 0; i < tobedeletedentries.length; i++) {
                    var inventoryLevel = tobedeletedentries[i].ilevel;
                    var onOrder = tobedeletedentries[i].onOrder;
                    if (inventoryLevel != null && inventoryLevel != 0) {
                        update.params.push({
                            inventoryLevel: 'inventoryLevel+' + inventoryLevel
                        });

                        update.matches.push({
                            locationid: mainlocationid,
                            productid: tobedeletedentries[i].productid
                        });
                    }

                    if (onOrder != null && onOrder != 0) {
                        update.params.push({
                            onOrder: 'onOrder+' + onOrder
                        });

                        update.matches.push({
                            locationid: mainlocationid,
                            productid: tobedeletedentries[i].productid
                        });
                    }
                    

                }
                tradelr.webdb.sqlUpdateRows([update], true, function () {
                    // TODO: update orders (currently location unused in orders)

                    // delete inventorylocitem
                    var ilocitem = {
                        tablename: 'inventorylocitem',
                        matches: [{ locationid: locationid}]
                    }
                    tradelr.webdb.sqlDeleteRows([ilocitem], true);

                    // delete inventoryloc
                    var iloc = {
                        tablename: 'inventoryloc',
                        matches: [{ id: locationid}]
                    };
                    tradelr.webdb.sqlDeleteRows([iloc], true);

                    var result = {
                        success: true
                    };

                    callback(result);
                });
            });
        }
    });


};

tradelr.inventory.location.removeItem = function (productid) {
    var item = {
        tablename: 'inventorylocitem',
        matches: [{ productid: productid}]
    };

    tradelr.webdb.sqlDeleteRows([item], true);
};