using System.ComponentModel;

namespace tradelr.Email.Models
{
    public enum EmailViewType
    {
        [Description("~/Plugin/tradelr.Email.dll/Views.AccountConfirmation.ascx")]
        ACCOUNT_CONFIRMATION,
        [Description("~/Plugin/tradelr.Email.dll/Views.AccountPasswordReset.ascx")]
        ACCOUNT_PASSWORD_RESET,
        [Description("~/Views/email/contacts/ContactLinkRequest.ascx")]
        CONTACT_LINKREQUEST,
        [Description("~/Views/email/contacts/ContactNewEntry.ascx")]
        CONTACT_NEWENTRY,
        [Description("~/Views/email/transactions/OrderDownloadLinks.ascx")]
        ORDER_DOWNLOADLINKS,
        [Description("~/Views/email/genericMail.ascx")]
        GENERIC,
        [Description("~/Views/email/transactions/InvoiceOrderChanged.ascx")]
        INVOICEORDER_CHANGED,
        [Description("~/Views/email/transactions/InvoiceOrderNew.ascx")]
        INVOICEORDER_NEW,
        [Description("~/Views/email/comment/InvoiceOrderNewComment.ascx")]
        INVOICEORDER_NEW_COMMENT,
        [Description("~/Views/email/transactions/PaymentStatusChange.ascx")]
        PAYMENT_STATUS_CHANGE,
        [Description("~/Views/email/transactions/OrderReceipt.ascx")]
        ORDER_RECEIPT,
        [Description("~/Views/email/transactions/OrderShipped.ascx")]
        ORDER_SHIPPED,
        [Description("~/Views/email/store/NewAccount.ascx")]
        STORE_NEWUSER,
        [Description("~/Views/email/store/NewMessage.ascx")]
        STORE_NEWMESSAGE
    }
}