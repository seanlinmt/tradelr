namespace tradelr.Library.Caching
{
    public enum CacheItemType
    {
        contacts_single,
        store,
        categories,
        products_single,  // needs to have per session
        products_store,  // needs to have per session
        activities,
        liquid_assets
    }
}
