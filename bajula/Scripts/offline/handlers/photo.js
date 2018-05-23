tradelr.photo = {};
tradelr.photo.size = {};
tradelr.photo.size.MEDIUM = 260;

tradelr.photo.remove = function(imageid, type) {
    var item = {
        tablename: 'photos',
        matches: [{ id: imageid, type: ["'", type, "'"].join("").toUpperCase() }]
    };

    tradelr.webdb.sqlDeleteRows([item], true);
    // need to update other relevant tables
    switch (type) {
    case 'PRODUCT':
        var item = {
            tablename: 'products',
            params: [{ cflag: tradelr.webdb.flags.UPDATE, thumbnailid: null }],
            matches: [{ thumbnailid: imageid }]
        };
        tradelr.webdb.sqlUpdateRows([item], false);
        break;
    default:
        break;
    }
};

tradelr.photo.removeByProduct = function(productid) {
    var item = {
        tablename: 'photos',
        matches: [{ contextid: productid, type: "'PRODUCT'" }]
    };

    tradelr.webdb.sqlDeleteRows([item], true);
};

// url = dataurl
tradelr.photo.resize = function(url, imageid, imgsize, callback) {
    // create an image and assign it the datauri, when it finishes loading we draw the resized
    // image into a canvas object
    var img = document.createElement("img");
    img.style.display = "none";
    document.body.appendChild(img);

    img.addEventListener("load", function(e) {
        var ratio = 1;

        if (img.width > imgsize || img.height > imgsize) {
            var size = Math.max(img.width, img.height);
            ratio = imgsize / size;
        }

        var canvas = document.createElement("canvas");

        canvas.width = img.width * ratio;
        canvas.height = img.height * ratio;
        canvas.setAttribute('alt', imageid);
        canvas.ctx = canvas.getContext("2d");
        canvas.ctx.save();
        canvas.ctx.scale(ratio, ratio);
        canvas.ctx.drawImage(img, 0, 0);
        canvas.ctx.restore();

        document.body.removeChild(img);
        callback(canvas);
    }, true);
    img.src = url;
};