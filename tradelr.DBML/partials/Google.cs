namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void DeleteGoogleBaseProduct(gbase_product gbase)
        {
            db.gbase_products.DeleteOnSubmit(gbase);
        }

        public void DeleteGoogleBaseSync(MASTERsubdomain sd)
        {
            if (sd.googleBase != null)
            {
                db.googleBases.DeleteOnSubmit(sd.googleBase);
                sd.googleBase = null;
                Save();
            }
        }
    }
}