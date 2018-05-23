using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using tradelr.Common.Models.currency;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.coupons;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class couponsController : baseController
    {
        public ActionResult Add()
        {
            var viewdata = new Coupon();
            viewdata.currency = MASTERdomain.currency.ToCurrency();
            return View(viewdata);
        }

        public ActionResult Create(string code, string description, DurationType duration_type, string end_date,
            string start_date, bool hasDuration, int? maxImpressions, bool minimumPurchaseOnly, decimal? minimumPurchase,
            decimal value, string valuetype)
        {
            // check required fields
            if (string.IsNullOrEmpty(code))
            {
                return SendJsonErrorResponse("Coupon code not specified");
            }
            if (string.IsNullOrEmpty(description))
            {
                return SendJsonErrorResponse("Coupon description not specified");
            }
            if (valuetype == "%" && value > 100)
            {
                return SendJsonErrorResponse("Discount cannot be more than 100%");
            }

            DateTime startdate;
            DateTime? expirydate = null;
            if (!string.IsNullOrEmpty(start_date))
            {
                startdate = DateTime.ParseExact(start_date, GeneralConstants.DATEFORMAT_STANDARD,
                                                 CultureInfo.InvariantCulture);
            }
            else
            {
                startdate = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(end_date))
            {
                expirydate = DateTime.ParseExact(end_date, GeneralConstants.DATEFORMAT_STANDARD,
                                                 CultureInfo.InvariantCulture);
            }

            if (expirydate.HasValue && expirydate.Value <= startdate)
            {
                return SendJsonErrorResponse("End date is earlier than start date");
            }

            // check if coupon with same code already exist
            var coupon = repository.GetCoupons(subdomainid.Value).Where(x => x.code == code).SingleOrDefault();
            if (coupon != null)
            {
                return SendJsonErrorResponse("Coupon code already in use");
            }

            // now we fill up coupon
            coupon = new coupon
                         {
                             subdomainid = subdomainid.Value,
                             code = code.StripWhitespace(),
                             description = description,
                             impressions = 0,
                             expired = DateTime.UtcNow < startdate? true:false,
                             startDate = startdate
                         };
            switch (duration_type)
            {
                case DurationType.UNLIMITED:
                    break;
                case DurationType.IMPRESSION:
                    coupon.maxImpressions = maxImpressions;
                    break;
                default:
                    break;
            }
            if (minimumPurchaseOnly)
            {
                coupon.minimumPurchase = minimumPurchase;
            }
            if (valuetype == "%")
            {
                coupon.couponPercentage = value;
            }
            else
            {
                coupon.couponValue = value;
            }

            if (hasDuration)
            {
                coupon.expiryDate = expirydate.Value;
            }
            try
            {
                repository.AddCoupon(coupon);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            
            return Json("Coupon added".ToJsonOKMessage());
        }

        public ActionResult Delete(long id)
        {
            var coupon = repository.GetCoupons(subdomainid.Value).Where(x => x.id == id).SingleOrDefault();
            if (coupon == null)
            {
                return SendJsonErrorResponse("Coupon not found");
            }

            try
            {
                repository.DeleteCoupon(coupon);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json("Coupon deleted".ToJsonOKMessage());
        }

        public ActionResult Edit(long id)
        {
            var coupon = repository.GetCoupons(subdomainid.Value).Where(x => x.id == id).SingleOrDefault();
            if (coupon == null)
            {
                return SendJsonErrorResponse(string.Format("Coupon {0} not found", id));
            }
            var currency = MASTERdomain.currency.ToCurrency();
            
            var viewdata = new Coupon
                               {
                                   id = coupon.id.ToString(),
                                   description = coupon.description,
                                   code = coupon.code,
                                   currency = currency
                               };

            if (coupon.couponValue.HasValue)
            {
                // absolute value
                viewdata.value = coupon.couponValue.Value.ToString("n" + currency.decimalCount);
                
            }
            else
            {
                // percentage value
                viewdata.value = coupon.couponPercentage.Value.ToString("n2");
            }
            viewdata.maxImpressions = coupon.maxImpressions.HasValue ? coupon.maxImpressions.Value.ToString() : "";

            viewdata.start_date = coupon.startDate.ToString(GeneralConstants.DATEFORMAT_STANDARD);
            if (coupon.expiryDate.HasValue)
            {
                viewdata.hasDuration = true;
                viewdata.end_date = coupon.expiryDate.Value.ToString(GeneralConstants.DATEFORMAT_STANDARD);
            }

            if (coupon.minimumPurchase.HasValue)
            {
                viewdata.minimumPurchaseOnly = true;
                viewdata.minimumPurchase = coupon.minimumPurchase.Value.ToString("n" + currency.decimalCount);
            }

            return View("Add", viewdata);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int rows, int page, string sidx, string sord)
        {
            IQueryable<coupon> results = repository.GetCoupons(subdomainid.Value, sidx, sord);

            var records = results.Count();
            var total = (records / rows);
            if (records % rows != 0)
            {
                total++;
            }
            // return in the format required for jqgrid
            results = results.Skip(rows * (page - 1)).Take(rows);

            var coupons = results.ToCouponsJqGrid();
            coupons.page = page;
            coupons.records = records;
            coupons.total = total;
            return Json(coupons);
        }

        public ActionResult Select()
        {
            var coupons = repository.GetCoupons(subdomainid.Value).Where(x => !x.expired);

            var viewdata = coupons.ToModel();
            return View(viewdata);
        }

        public ActionResult Update(long id, string code, string description, DurationType duration_type, string end_date,
            string start_date, bool hasDuration, int? maxImpressions, bool minimumPurchaseOnly, decimal? minimumPurchase,
            bool newUsers, decimal value, string valuetype)
        {
            // check required fields
            if (string.IsNullOrEmpty(code))
            {
                return SendJsonErrorResponse("Coupon code not specified");
            }
            if (string.IsNullOrEmpty(description))
            {
                return SendJsonErrorResponse("Coupon description not specified");
            }
            if (valuetype == "%" && value > 100)
            {
                return SendJsonErrorResponse("Discount cannot be more than 100%");
            }

            DateTime startdate;
            DateTime? expirydate = null;
            if (!string.IsNullOrEmpty(start_date))
            {
                startdate = DateTime.ParseExact(start_date, GeneralConstants.DATEFORMAT_STANDARD,
                                                 CultureInfo.InvariantCulture);
            }
            else
            {
                startdate = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(end_date))
            {
                expirydate = DateTime.ParseExact(end_date, GeneralConstants.DATEFORMAT_STANDARD,
                                                 CultureInfo.InvariantCulture);
            }

            if (expirydate.HasValue && expirydate.Value <= startdate)
            {
                return SendJsonErrorResponse("End date is earlier than start date");
            }

            // check if coupon with same code already exist
            var coupon = repository.GetCoupons(subdomainid.Value).Where(x => x.id == id).SingleOrDefault();
            if (coupon == null)
            {
                return SendJsonErrorResponse("Unable to find coupon");
            }

            // now we fill up coupon
            coupon.code = code.StripWhitespace();
            coupon.description = description;
            switch (duration_type)
            {
                case DurationType.UNLIMITED:
                    break;
                case DurationType.IMPRESSION:
                    coupon.maxImpressions = maxImpressions;
                    break;
                default:
                    break;
            }
            if (minimumPurchaseOnly)
            {
                coupon.minimumPurchase = minimumPurchase;
            }
            if (valuetype == "%")
            {
                coupon.couponPercentage = value;
                coupon.couponValue = null;
            }
            else
            {
                coupon.couponPercentage = null;
                coupon.couponValue = value;
            }
            coupon.expired = DateTime.UtcNow < startdate? true:false;
            coupon.startDate = startdate;

            if (hasDuration)
            {
                coupon.expiryDate = expirydate.Value;
            }

            try
            {
                repository.Save();
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json("Coupon updated".ToJsonOKMessage());
        }
    }
}
