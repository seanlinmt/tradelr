if ((typeof Tradelr) === 'undefined') {
    Tradelr = {};
}

/* 

Events (override!)

Example override:
  ... add to your theme.liquid's script tag....

  Tradelr.onItemAdded = function(line_item) {
    $('message').update('Added '+line_item.title + '...');
  }
  
*/

Tradelr.onError = function(XMLHttpRequest, textStatus) {
  // Returns a description of the error in XMLHttpRequest.responseText.
  // It is JSON.
    // Example: {"message":"The product 'Amelia - Small' is already sold out.","success":false}
  var data = eval('(' + XMLHttpRequest.responseText + ')');
    alert(data.message);
};

Tradelr.onCartUpdate = function(cart) {
  alert('There are now ' + cart.item_count + ' items in the cart.');
};  

Tradelr.onItemAdded = function(line_item) {
  alert(line_item.title + ' was added to your shopping cart.');
};

Tradelr.onProduct = function(product) {
  alert('Received everything we ever wanted to know about ' + product.title);
};

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

Tradelr.resizeImage = function(image, size) {
  try {
    if(size == 'original') { return image; }
    else {      
      var matches = image.match(/(.*\/[\w\-\_\.]+)\.(\w{2,4})/);
      return matches[1] + '_' + size + '.' + matches[2];
    }    
  } catch (e) { return image; }
};

/* Ajax API */

// -------------------------------------------------------------------------------------
// POST to cart/add.json returns the JSON of the line item associated with the added item.
// -------------------------------------------------------------------------------------
Tradelr.addItem = function(variant_id, quantity, callback) {
  var quantity = quantity || 1;
  var params = {
    type: 'POST',
    url: '/cart/add.json',
    data: 'quantity=' + quantity + '&id=' + variant_id,
    dataType: 'json',
    success: function(line_item) { 
      if ((typeof callback) === 'function') {
        callback(line_item);
      }
      else {
        Tradelr.onItemAdded(line_item);
      }
    },
    error: function(XMLHttpRequest, textStatus) {
      Tradelr.onError(XMLHttpRequest, textStatus);
    }
  };
  jQuery.ajax(params);
};

// ---------------------------------------------------------
// POST to cart/add.json returns the JSON of the line item.
// ---------------------------------------------------------
Tradelr.addItemFromForm = function(form_id, callback) {
    var params = {
      type: 'POST',
      url: '/cart/add.json',
      data: jQuery('#' + form_id).serialize(),
      dataType: 'json',
      success: function(line_item) { 
        if ((typeof callback) === 'function') {
          callback(line_item);
        }
        else {
          Tradelr.onItemAdded(line_item);
        }
      },
      error: function(XMLHttpRequest, textStatus) {
        Tradelr.onError(XMLHttpRequest, textStatus);
      }
    };
    jQuery.ajax(params);
};

// ---------------------------------------------------------
// GET cart.json returns the cart in JSON.
// ---------------------------------------------------------
Tradelr.getCart = function(callback) {
  jQuery.getJSON('/cart.json', function (cart, textStatus) {
    if ((typeof callback) === 'function') {
      callback(cart);
    }
    else {
      Tradelr.onCartUpdate(cart);
    }
  });
};

// ---------------------------------------------------------
// GET cart/shipping_rates.json returns the cart in JSON.
// ---------------------------------------------------------
Tradelr.getCartShippingRatesForDestination = function (shipping_address, callback) {
    var params = {
        type: 'GET',
        url: '/cart/shipping_rates.json',
        data: Tradelr.param({ 'shipping_address': shipping_address }),
        dataType: 'json',
        success: function(response) {
            rates = response.shipping_rates
            if ((typeof callback) === 'function') {
                callback(rates, shipping_address);
            }
            else {
                Tradelr.onCartShippingRatesUpdate(rates, shipping_address);
            }
        },
        error: function(XMLHttpRequest, textStatus) {
            Tradelr.onError(XMLHttpRequest, textStatus);
        }
    };
    jQuery.ajax(params);
};

// ---------------------------------------------------------
// GET products/<product-handle>.json returns the product in JSON.
// ---------------------------------------------------------
Tradelr.getProduct = function(handle, callback) {
  jQuery.getJSON('/products/' + handle + '.json', function (product, textStatus) {
    if ((typeof callback) === 'function') {
      callback(product);
    }
    else {
      Tradelr.onProduct(product);
    }
  });
};

// ---------------------------------------------------------
// POST to cart/change.json returns the cart in JSON.
// ---------------------------------------------------------
Tradelr.changeItem = function(variant_id, quantity, callback) {
  var params = {
    type: 'POST',
    url: '/cart/change.json',
    data:  'quantity='+quantity+'&id='+variant_id,
    dataType: 'json',
    success: function(cart) { 
      if ((typeof callback) === 'function') {
        callback(cart);
      }
      else {
        Tradelr.onCartUpdate(cart);
      }
    },
    error: function(XMLHttpRequest, textStatus) {
      Tradelr.onError(XMLHttpRequest, textStatus);
    }
  };
  jQuery.ajax(params);
};

// ---------------------------------------------------------
// POST to cart/change.js returns the cart in JSON.
// ---------------------------------------------------------
Tradelr.removeItem = function(variant_id, callback) {
  var params = {
    type: 'POST',
    url: '/cart/change.json',
    data:  'quantity=0&id='+variant_id,
    dataType: 'json',
    success: function(cart) { 
      if ((typeof callback) === 'function') {
        callback(cart);
      }
      else {
        Tradelr.onCartUpdate(cart);
      }
    },
    error: function(XMLHttpRequest, textStatus) {
      Tradelr.onError(XMLHttpRequest, textStatus);
    }
  };
  jQuery.ajax(params);
};

// ---------------------------------------------------------
// POST to cart/clear.json returns the cart in JSON.
// It removes all the items in the cart, but does
// not clear the cart attributes nor the cart note.
// ---------------------------------------------------------
Tradelr.clear = function(callback) {
  var params = {
    type: 'POST',
    url: '/cart/clear.json',
    data:  '',
    dataType: 'json',
    success: function(cart) { 
      if ((typeof callback) === 'function') {
        callback(cart);
      }
      else {
        Tradelr.onCartUpdate(cart);
      }
    },
    error: function(XMLHttpRequest, textStatus) {
      Tradelr.onError(XMLHttpRequest, textStatus);
    }
  };
  jQuery.ajax(params);
};

// ---------------------------------------------------------
// POST to cart/update.json returns the cart in JSON.
// ---------------------------------------------------------
Tradelr.updateCartFromForm = function(form_id, callback) {
  var params = {
    type: 'POST',
    url: '/cart/update.json',
    data: jQuery('#' + form_id).serialize(),
    dataType: 'json',
    success: function(cart) {
      if ((typeof callback) === 'function') {
        callback(cart);
      }
      else {
        Tradelr.onCartUpdate(cart);
      }
    },
    error: function(XMLHttpRequest, textStatus) {
      Tradelr.onError(XMLHttpRequest, textStatus);
    }
  };
  jQuery.ajax(params);
};

// ---------------------------------------------------------
// POST to cart/update.json returns the cart in JSON.
// To clear a particular attribute, set its value to an empty string.
// Receives attributes as a hash or array. Look at comments below.
// ---------------------------------------------------------
Tradelr.updateCartAttributes = function(attributes, callback) {
  var data = '';
  // If attributes is an array of the form:
  // [ { key: 'my key', value: 'my value' }, ... ]
  if (jQuery.isArray(attributes)) {
    jQuery.each(attributes, function(indexInArray, valueOfElement) {
      var key = attributeToString(valueOfElement.key);
      if (key !== '') {
        data += 'attributes[' + key + ']=' + attributeToString(valueOfElement.value) + '&';
      }
    });
  }
  // If attributes is a hash of the form:
  // { 'my key' : 'my value', ... }
  else if ((typeof attributes === 'object') && attributes !== null) {
    jQuery.each(attributes, function(key, value) {
        data += 'attributes[' + attributeToString(key) + ']=' + attributeToString(value) + '&';
    });
  }
  var params = {
    type: 'POST',
    url: '/cart/update.json',
    data: data,
    dataType: 'json',
    success: function(cart) {
      if ((typeof callback) === 'function') {
        callback(cart);
      }
      else {
        Tradelr.onCartUpdate(cart);
      }
    },
    error: function(XMLHttpRequest, textStatus) {
      Tradelr.onError(XMLHttpRequest, textStatus);
    }
  };
  jQuery.ajax(params);
};

// ---------------------------------------------------------
// POST to cart/update.json returns the cart in JSON.
// ---------------------------------------------------------
Tradelr.updateCartNote = function(note, callback) {
  var params = {
    type: 'POST',
    url: '/cart/update.json',
    data: 'note=' + attributeToString(note),
    dataType: 'json',
    success: function(cart) {
      if ((typeof callback) === 'function') {
        callback(cart);
      }
      else {
        Tradelr.onCartUpdate(cart);
      }
    },
    error: function(XMLHttpRequest, textStatus) {
      Tradelr.onError(XMLHttpRequest, textStatus);
    }
  };
  jQuery.ajax(params);
};


if (jQuery.fn.jquery >= '1.4') {
  Tradelr.param = jQuery.param;
} else {
  Tradelr.param = function( a ) {
    var s = [],
      add = function( key, value ) {
        // If value is a function, invoke it and return its value
        value = jQuery.isFunction(value) ? value() : value;
        s[ s.length ] = encodeURIComponent(key) + "=" + encodeURIComponent(value);
      };
  
    // If an array was passed in, assume that it is an array of form elements.
    if ( jQuery.isArray(a) || a.jquery ) {
      // Serialize the form elements
      jQuery.each( a, function() {
        add( this.name, this.value );
      });
    
    } else {
      for ( var prefix in a ) {
        Tradelr.buildParams( prefix, a[prefix], add );
      }
    }

    // Return the resulting serialization
    return s.join("&").replace(/%20/g, "+");
  }

  Tradelr.buildParams = function( prefix, obj, add ) {
    if ( jQuery.isArray(obj) && obj.length ) {
      // Serialize array item.
      jQuery.each( obj, function( i, v ) {
        if ( rbracket.test( prefix ) ) {
          // Treat each array item as a scalar.
          add( prefix, v );

        } else {
          Tradelr.buildParams( prefix + "[" + ( typeof v === "object" || jQuery.isArray(v) ? i : "" ) + "]", v, add );
        }
      });
      
    } else if ( obj != null && typeof obj === "object" ) {
      if ( Tradelr.isEmptyObject( obj ) ) {
        add( prefix, "" );

      // Serialize object item.
      } else {
        jQuery.each( obj, function( k, v ) {
          Tradelr.buildParams( prefix + "[" + k + "]", v, add );
        });
      }
          
    } else {
      // Serialize scalar item.
      add( prefix, obj );
    }
  }
  
  Tradelr.isEmptyObject = function( obj ) {
    for ( var name in obj ) {
      return false;
    }
    return true;
  }
}


/* Used by Tools */

function floatToString(numeric, decimals) {
  var amount = numeric.toFixed(decimals).toString();
  if(amount.match(/^\.\d+/)) {return "0"+amount; }
  else { return amount; }
}

/* Used by API */

function attributeToString(attribute) {
  if ((typeof attribute) !== 'string') {
    // Converts to a string.
    attribute += '';
    if (attribute === 'undefined') {
      attribute = '';
    }
  }
  // Removing leading and trailing whitespace.
  return jQuery.trim(attribute);
}
