if ((typeof Tradelr) == 'undefined') {
    var Tradelr = {};
}

/* 

Events (override!) 

Example override:
  ... add to your theme.liquid's script tag....

  Tradelr.onItemAdded = function(line_item) {	    
    $('message').update('Added '+line_item.title + '...');	    
  }	  
*/	

Tradelr.onError = function(data) {   
  if (data.message) {
    alert(data.message + '(' + data.status  + '): ' + data.description);
  } else {
    alert('Error: ' + Tradelr.fullMessagesFromErrors(data).join('; ') + '.');
  }
},

Tradelr.fullMessagesFromErrors = function(errors) {
  var fullMessages = [];
  console.log(errors);
  $H(errors).each(function(attribute_and_messages) {
    attribute_and_messages[1].each(function(message, index) {
      fullMessages.push(attribute_and_messages[0] + ' ' + message);
    });
  });
  return fullMessages;
}

Tradelr.onCartUpdate = function (cart) {
    alert("There are now " + cart.item_count + " items in the cart.");
},

Tradelr.onCartShippingRatesUpdate = function (rates, shipping_address) {
    var readable_address = '';
    if (shipping_address.zip) readable_address += shipping_address.zip + ', ';
    if (shipping_address.province) readable_address += shipping_address.province + ', ';
    readable_address += shipping_address.country
    alert('There are ' + rates.length + ' shipping rates available for ' + readable_address + ', starting at ' + Tradelr.formatMoney(rates[0].price) + '.');
},

Tradelr.onItemAdded = function (line_item) {
    alert(line_item.title + ' Was added to your shopping cart');
},

Tradelr.onProduct = function (product) {
    alert('Received everything we ever wanted to know about ' + product.title);
},

/* Tools */

Tradelr.formatMoney = function (value, symbol, decimalcount) {
    if (isNaN(parseFloat(value))) {
        return value;
    }
    value = value.toFixed(decimalcount);
    var delimiter = ","; // replace comma if desired
    var d;
    var i;
    var a;
    if (value.indexOf('.') == -1) {
        d = '';
        i = value;
    }
    else {
        a = value.split('.', 2);
        d = a[1];
        i = parseInt(a[0]);
    }

    if (isNaN(i)) { return ''; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    var n = new String(i);
    a = [];
    while (n.length > 3) {
        var nn = n.substr(n.length - 3);
        a.unshift(nn);
        n = n.substr(0, n.length - 3);
    }
    if (n.length > 0) { a.unshift(n); }
    n = a.join(delimiter);
    if (d.length < 1) { value = n; }
    else { value = n + '.' + d; }
    value = minus + value;
    return symbol + value;
},

Tradelr.resizeImage = function (image, size) {
    try {
        if (size == 'original') { return image; }
        else {
            var matches = image.match(/(.*\/[\w\-\_\.]+)\.(\w{2,4})/);
            return matches[1] + '_' + size + '.' + matches[2];
        }
    } catch (e) { return image; }
},
/* API */

Tradelr.addItem = function (variant_id, quantity, callback) {
    var quantity = quantity || 1;
    new Ajax.Request("/cart/add.js", this.params('post', 'quantity=' + quantity + '&id=' + variant_id, callback || this.onItemAdded.bind(this)));
},

Tradelr.addItemFromForm = function (form_id, callback) {
    new Ajax.Request("/cart/add.js", this.params('post', Form.serialize(form_id), callback || this.onItemAdded.bind(this)));
},

Tradelr.getCart = function (callback) {
    new Ajax.Request("/cart.js", this.params('get', null, (callback || this.onCartUpdate.bind(this))));
},

Tradelr.getCartShippingRatesForDestination = function (shipping_address, callback) {
    callback = (callback || this.onCartShippingRatesUpdate.bind(this));
    var shipping_address_params = {};
    $H(shipping_address).each(function (pair) {
        shipping_address_params['shipping_address[' + pair[0] + ']'] = pair[1]
    });
    var params = this.params('get', shipping_address_params, callback);
    params.onSuccess = function (t) {
        var response = $(eval('(' + t.responseText + ')'))
        try { callback(response.shipping_rates, shipping_address); }
        catch (e) { alert("API Error: " + e + "\n\n" + t.responseText); };
    } .bind(this);
    new Ajax.Request("/cart/shipping_rates.json", params);
},

Tradelr.getProduct = function (handle, callback) {
    new Ajax.Request("/products/" + handle + '.js', this.params('get', null, (callback || this.onProduct.bind(this))));
},

Tradelr.changeItem = function (variant_id, quantity) {
    new Ajax.Request("/cart/change.js", this.params('post', 'quantity=' + quantity + '&id=' + variant_id, this.onCartUpdate.bind(this)));
},

Tradelr.removeItem = function (variant_id) {
    new Ajax.Request("/cart/change.js", this.params('post', 'quantity=0&id=' + variant_id, this.onCartUpdate.bind(this)));
},


Tradelr.clear = function () {
    new Ajax.Request("/cart/clear.js", this.params('post', '', this.onCartUpdate.bind(this)));
},

Tradelr.updateCart = function (updates, callback) {
    var query = '';

    if (updates.type == Array) {
        $A(array).flatten().each(function (qty) {
            query += ('updates[]=' + qty.toString()) + "&";
        });
    }
    else if (updates.type == Object) {
        $H(array).flatten().each(function (id, qty) {
            query += ('updates[' + id.toString() + ']=' + qty.toString()) + "&";
        });
    }
    else {
        throw "updates parameter must be array of quantities or a hash of {item_ids: quantities}";
    }

    new Ajax.Request("/cart/update.js", this.params('post', query, (callback || this.onCartUpdate.bind(this))));
},

Tradelr.updateCartFromForm = function (form_id, callback) {
    new Ajax.Request("/cart/update.js", this.params('post', Form.serialize(form_id), (callback || this.onCartUpdate.bind(this))));
};

/* private */

Tradelr.params = function(method, parameters, callback) {

    var hash = {
        method: (method || 'post'),
        parameters: (parameters || ''),
        evalScripts: false,
        asynchronous: true,
        requestHeaders: {
            'If-Modified-Since': "Sat, 1 Jan 2000 00:00:00 GMT"
        }
    };


    if (callback == null) {
        callback = this.onDebug.bind(this);
    }

    hash.onSuccess = function(t) {
        try { callback($(eval('(' + t.responseText + ')'))); }
        catch(e) { alert("API Error: " + e + "\n\n" + t.responseText); };
    }.bind(this);

    hash.onFailure = function(t) {
        try { this.onError($(eval('(' + t.responseText + ')'))); }
        catch(e) { alert("API Error: " + e + "\n\n" + t.responseText); };
    }.bind(this);

    return hash;
};
  
function floatToString(numeric, decimals) {  
  var amount = numeric.toFixed(decimals).toString();  
  if(amount.match(/^\.\d+/)) {return "0"+amount; }
  else { return amount; }
}
  
