using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shipwire.inventory
{
    public class InventoryUpdate
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Warehouse { get; set; }
        public string ProductCode { get; set; }

        public InventoryUpdate()
        {
            
        }

        public InventoryUpdate(string email, string password, string warehouse)
        {
            EmailAddress = email;
            Password = password;
            Warehouse = warehouse;
#if DEBUG
            Server = "Test";
#else
            Server = "Production";
#endif
        }
    }
}
