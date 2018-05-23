using System.Web.Script.Serialization;

namespace tradelr.Models.shipwire
{
    public class Shipwire
    {
        public byte pnp { get; set; }
        public ShipwireCategory category { get; set; }
        public bool fragile { get; set; }
        public bool dangerous { get; set; }
        public bool perishable { get; set; }

        public Shipwire()
        {
            
        }
        public Shipwire(byte pnp, ShipwireCategory category, bool fragile, bool dangerous, bool perishable)
        {
            this.pnp = pnp;
            this.category = category;
            this.fragile = fragile;
            this.dangerous = dangerous;
            this.perishable = perishable;
        }

        public string Serialize()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }
    }

    public static class ShipwireHelper
    {
        public static Shipwire ToShipwire(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new Shipwire();
            }
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<Shipwire>(value);
        }
    }
}