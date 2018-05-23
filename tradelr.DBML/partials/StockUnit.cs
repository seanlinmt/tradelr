using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public MASTERstockUnit AddMasterStockUnit(string unitName)
        {
            var exist =
                db.MASTERstockUnits.Where(x => x.name.ToLower() == unitName.ToLower())
                    .SingleOrDefault();
            if (exist == null)
            {
                exist = new MASTERstockUnit();
                exist.name = unitName;
                db.MASTERstockUnits.InsertOnSubmit(exist);
                db.SubmitChanges();
            }
            return exist;
        }

        public long AddStockUnit(stockUnit sunit)
        {
            var result = db.stockUnits.Where(x => x.unitID == sunit.unitID && x.subdomainid == sunit.subdomainid).SingleOrDefault();
            if (result == null)
            {
                db.stockUnits.InsertOnSubmit(sunit);
                db.SubmitChanges();
                return sunit.id;
            }
            return result.id;
        }
        
        public void DeleteStockUnit(long id, long subdomainid)
        {
            var item =
                db.stockUnits.Where(x => x.id == id && x.subdomainid == subdomainid).SingleOrDefault();
            if (item == null)
            {
                return;
            }
            db.stockUnits.DeleteOnSubmit(item);
            db.SubmitChanges();
        }

        public IQueryable<MASTERstockUnit> FindMASTERStockUnit(string query, long owner)
        {
            return db.MASTERstockUnits.Where(x => x.name.StartsWith(query));
        }

        public IQueryable<stockUnit> GetAllStockUnits(long ownerid)
        {
            return db.stockUnits.Where(x => x.subdomainid == ownerid);
        }

        
        
    }
}