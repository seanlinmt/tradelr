using System.Collections.Generic;

namespace tradelr.Library.geo
{
    public class Region
    {
        public int id { get; set; }
        public string name { get; set; }

        public static List<Region> Values = new List<Region>()
                                                {
                                                    new Region() {id = 14, name = "Africa"},
                                                    new Region() {id = 15, name = "Central America"},
                                                    new Region() {id = 11, name = "European Union"},
                                                    new Region() {id = 12, name = "Europe non-EU"},
                                                    new Region() {id = 13, name = "South America"}
                                                };
    }
}