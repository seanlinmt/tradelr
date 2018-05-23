using System.ComponentModel;

namespace tradelr.Common.Constants
{
    public enum InventoryImportConstants
    {
        [Description("Category")]
        CATEGORY,
        [Description("SKU")]
        SKU ,
        [Description("Title")]
        TITLE,
        [Description("Description")]
        DESCRIPTION,
        [Description("Subcategory")]
        SUBCATEGORY,
        [Description("Stock Unit")]
        STOCKUNIT,
        [Description("Selling Price")]
        SELLINGPRICE,
        [Description("In Stock")]
        INSTOCK,
        [Description("On Order")]
        ONORDER,
        [Description("ReOrder Level")]
        REORDERLEVEL
    }

    public enum SupplierImportConstants
    {
        [Description("Category")]
        CATEGORY,
        [Description("Title")]
        TITLE,
        [Description("Description")]
        DESCRIPTION,
        [Description("Stock Unit")]
        STOCKUNIT,
        [Description("Subcategory")]
        SUBCATEGORY,
        [Description("Selling Price")]
        SELLINGPRICE,
        [Description("Supplier SKU")]
        SUPPLIERSKU,
        [Description("Supplier Stock")]
        SUPPLIERSTOCK 
    }
}
