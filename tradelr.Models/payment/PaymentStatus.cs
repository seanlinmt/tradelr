using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.payment
{
    public enum PaymentStatus
    {
        New = 0,
        Chargeable = 1,
        Charging,
        Accepted,
        Declined,
        Cancelled
    }

    public static class PaymentStatusHelper
    {
        public static string ToDisplayString(this PaymentStatus val)
        {
            switch (val)
            {
                case PaymentStatus.New:
                    return "created";
                case PaymentStatus.Charging:
                    return "charged";
                case PaymentStatus.Accepted:
                    return "accepted";
                case PaymentStatus.Declined:
                    return "declined";
                case PaymentStatus.Cancelled:
                    return "cancelled";
                default:
                    throw new ArgumentOutOfRangeException("val");
            }
        }
    }
}
