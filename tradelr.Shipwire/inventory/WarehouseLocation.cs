using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Shipwire.inventory
{
    public static class WarehouseLocation
    {
        public const string CHI = "Shipwire Chicago";
        public const string LAX = "Shipwire Los Angeles";
        public const string TOR = "Shipwire Toronto";
        public const string VAN = "Shipwire Vancouver";
        public const string UK = "Shipwire UK";
        public const string REN = "Shipwire Reno";

        public static readonly string[] Values = new[] {CHI, LAX, TOR, VAN, UK, REN};
    }
}
