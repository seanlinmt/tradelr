using System.ComponentModel;

namespace tradelr.Libraries.Extensions
{
    /// <summary>
    /// DO NOT USE UNDERSCORE FOR NAMES!!!!!!
    /// </summary>
    public enum TradelrControls
    {
        [Description("~/Views/shared/changeHistory.ascx")]
        changeHistory,
        [Description("~/Views/comments/comment.ascx")]
        comments,
        [Description("~/Views/support/contactUs.ascx")]
        contactUs,
        [Description("~/Views/fb/profile.ascx")]
        facebook_profile,
        [Description("~/Views/fb/importRow.ascx")]
        facebookImportRow,
        [Description("~/Views/fb/importRowContent.ascx")]
        facebookImportRowContent,
        [Description("~/Views/login/forgotPass.ascx")]
        forgotPass,
        [Description("~/Views/store/opengraph.ascx")]
        opengraph,
        [Description("~/Views/pricing/plans.ascx")]
        pricingPlans,
        //[Description("~/Views/shipping/profile.ascx")]
        //shipping_profile,
        [Description("~/Views/login/signIn.ascx")]
        signIn,
        [Description("~/Views/tour/contacts.ascx")]
        tour_contacts,
        [Description("~/Views/tour/engage.ascx")]
        tour_engage,
        [Description("~/Views/tour/inventory.ascx")]
        tour_inventory,
        [Description("~/Views/tour/reporting.ascx")]
        tour_reporting,
        [Description("~/Views/tour/security.ascx")]
        tour_security,
        [Description("~/Views/tour/store.ascx")]
        tour_store,
        [Description("~/Views/tour/transactions.ascx")]
        tour_transactions,
        [Description("~/Views/tour/web.ascx")]
        tour_web,
    }
}