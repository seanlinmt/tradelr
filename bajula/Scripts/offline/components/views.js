tradelr.views = {};

tradelr.views.sidelocation = function (callback) {
    var sql = "SELECT * FROM inventoryloc WHERE cflag !=" + tradelr.webdb.flags.DELETE + " ORDER BY serverid";
    tradelr.webdb.sqlGetRows(sql, [], null, function (rows) {
        var locationlist = [];
        if (rows != null) {
            for (var i = 0; i < rows.length; i++) {
                var row = rows.item(i);
                var name = row['name'];
                var id = row['id'];
                var loc = {
                    id: id,
                    name: name
                };
                locationlist.push(loc);
            }

            // draw
            var sb = [];
            for (var i = 0; i < locationlist.length; i++) {
                var loc = locationlist[i];
                sb.push("<div class='sideboxEntry' fid='");
                sb.push(loc.id);
                sb.push("'><div class='title'>");
                sb.push(loc.name);
                sb.push("</div>");
                sb.push("<div class='del'></div>");
                sb.push("</div>");
            }
            callback(sb.join(''));
        }
    });
};

tradelr.views.sidecategory = function (callback) {

    var sql = "SELECT * FROM category WHERE cflag !=" + tradelr.webdb.flags.DELETE + " ORDER BY parentid,name";
    tradelr.webdb.sqlGetRows(sql, [], null, function (rows) {
        var categorylist = [];
        if (rows != null) {
            for (var i = 0; i < rows.length; i++) {
                var row = rows.item(i);
                var parentid = row['parentid'];
                var name = row['name'];
                var id = row['id'];
                if (parentid == null) {
                    var cat = {
                        id: id,
                        name: name,
                        subcats: []
                    };
                    categorylist.push(cat);
                }
                else {
                    // find matching parentid in array
                    for (var j = 0; j < categorylist.length; j++) {
                        var parentcat = categorylist[j];
                        if (parentcat.id == parentid) {
                            parentcat.subcats.push({
                                id: id,
                                name: name
                            });
                        }
                    }
                }
            }

            // draw
            var sb = [];
            for (var i = 0; i < categorylist.length; i++) {
                var maincat = categorylist[i];
                sb.push("<div class='sideboxEntry' fid='");
                sb.push(maincat.id);
                sb.push("'><div class='title'>");
                sb.push(maincat.name);
                sb.push("</div>");
                sb.push("<div class='del'></div>");
                sb.push("</div>");
                for (var j = 0; j < maincat.subcats.length; j++) {
                    var subcat = maincat.subcats[j];
                    sb.push("<div class='sideboxSubEntry' fid='");
                    sb.push(subcat.id);
                    sb.push("'><div class='title'>");
                    sb.push(subcat.name);
                    sb.push("</div>");
                    sb.push("<div class='del'></div>");
                    sb.push("</div>");
                }
            }
            callback(sb.join(''));
        }
    });
};