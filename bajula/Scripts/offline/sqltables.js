/// <reference path="../root/general.js" />
/// <reference path="synchronise.js" />

// need to ensure least dependent tables on top ????
tradelr.webdb.tables = [
    { name: "category", columns: [{ name: "id", type: " INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT" },
                                { name: "name", type: " TEXT" },
                                { name: "parentid", type: " INTEGER", link: "category" },
                                { name: "ccount", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "cflag", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "serverid", type: " INTEGER UNIQUE"}]
    },
    { name: "inventoryloc", columns: [{ name: "id", type: " INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT" },
                                { name: "name", type: " TEXT" },
                                { name: "ccount", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "cflag", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "serverid", type: " INTEGER UNIQUE"}]
    },
    { name: "inventorylocitem", columns: [{ name: "id", type: " INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT" },
                                { name: "locationid", type: " INTEGER", link: "inventoryloc" },
                                { name: "productid", type: " INTEGER", link: "products" },
                                { name: "inventoryLevel", type: " INTEGER" },
                                { name: "onOrder", type: " INTEGER" },
                                { name: "alarmLevel", type: " INTEGER" },
                                { name: "ccount", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "cflag", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "serverid", type: " INTEGER UNIQUE"}]
    },
    { name: "photos", columns: [{ name: "id", type: " INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT" },
                                { name: "url", type: " TEXT" },
                                { name: "contextid", type: " INTEGER" },
                                { name: "type", type: " TEXT" },
                                { name: "ccount", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "cflag", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "serverid", type: " INTEGER UNIQUE"}]
    },
    { name: "stockunit", columns: [{ name: "id", type: " INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT" },
                                { name: "name", type: " TEXT" },
                                { name: "ccount", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "cflag", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "serverid", type: " INTEGER UNIQUE"}]
    },
    { name: "products", columns: [{ name: "id", type: " INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT" },
                                { name: "categoryid", type: " INTEGER", link: "category" },
                                { name: "thumbnailid", type: " INTEGER", link: "photos" },
                                { name: "SKU", type: " TEXT" },
                                { name: "title", type: " TEXT" },
                                { name: "details", type: " TEXT" },
                                { name: "sellingPrice", type: " TEXT" },
                                { name: "costPrice", type: " TEXT" },
                                { name: "notes", type: " TEXT" },
                                { name: "paymentterms", type: " TEXT" },
                                { name: "shippingterms", type: " TEXT" },
                                { name: "stockunitid", type: " INTEGER", link: "stockunit" },
                                { name: "flags", type: " INTEGER" },
                                { name: "ccount", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "cflag", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "serverid", type: " INTEGER UNIQUE"}]
    },
    { name: "settings", columns: [{ name: "id", type: " INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT" },
                                { name: "currency", type: " TEXT" },
                                { name: "flags", type: " INTEGER" },
                                { name: "ccount", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "cflag", type: " INTEGER NOT NULL DEFAULT 0" },
                                { name: "serverid", type: " INTEGER UNIQUE"}]
    }
];
