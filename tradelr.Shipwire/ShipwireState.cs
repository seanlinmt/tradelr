using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shipwire
{
    public enum ShipwireState
    {
        ORDER_SUBMITTED, // when order is first submitted and stays in this state until shipped or not found
        ORDER_NOTFOUND,  // tracking update could not find order 
        ORDER_SHIPPED // order has been shipped
    }
}
