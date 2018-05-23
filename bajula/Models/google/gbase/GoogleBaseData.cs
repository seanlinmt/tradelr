using System.Collections.Generic;
using System.Web.Mvc;
using tradelr.Library.geo;

namespace tradelr.Models.google.gbase
{
	public class GoogleBaseData
	{
	    public static readonly Country[] SupportedCountries = new[]
	                                                     {
	                                                         Country.Values[10], // Australia
	                                                         Country.Values[37], // China
	                                                         Country.Values[61], // France
	                                                         Country.Values[65], // Germany
	                                                         Country.Values[83], // Italy
	                                                         Country.Values[85], // Japan
	                                                         Country.Values[125], // Netherlands
	                                                         Country.Values[162], // Spain
	                                                         Country.Values[184], // United Kingdom
	                                                         Country.Values[185], // United States

	                                                     };

        public IEnumerable<SelectListItem> countries { get; set; }
	}
}