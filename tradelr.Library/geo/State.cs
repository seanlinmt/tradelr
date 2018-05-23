using System.Collections.Generic;

namespace tradelr.Library.geo
{
    public static class State
    {
        public static string ToStateName(this string statecode, string countrycode)
        {
            if (string.IsNullOrEmpty(statecode))
            {
                return "";
            }
            string name;
            switch (countrycode)
            {
                case "32": // canada
                    if (!canada.TryGetValue(statecode, out name))
                    {
                        name = ""; // set to empty string otherwise it's NULL
                    }
                    break;
                case "185": // usa
                    if (!usa.TryGetValue(statecode, out name))
                    {
                        name = "";
                    }
                    break;
                default:
                    // set to statecode for unsupported countries
                    name = statecode;
                    break;
            }
            
            return name;
        }

        private static readonly Dictionary<string, string> canada = new Dictionary<string, string>()
                                                                        {
                                                                            {"ON", "Ontario"},
                                                                            {"QC", "Quebec"},
                                                                            {"NS", "Nova Scotia"},
                                                                            {"NB", "New Brunswick"},
                                                                            {"MB", "Manitoba"},
                                                                            {"BC", "British Columbia"},
                                                                            {"PE", "Prince Edward Island"},
                                                                            {"SK", "Saskatchewan"},
                                                                            {"AB", "Alberta"},
                                                                            {"NL", "Newfoundland and Labrador"}
                                                                        };

        private static readonly Dictionary<string, string> usa = new Dictionary<string, string>()
                                                    {
                                                        {
                                                            "AL",
                                                            "Alabama"
                                                            }
                                                        ,
                                                        {
                                                            "AK",
                                                            "Alaska"
                                                            }
                                                        ,



                                                        {
                                                            "AZ",
                                                            "Arizona"
                                                            }
                                                        ,

                                                        {
                                                            "AR",
                                                            "Arkansas"
                                                            }
                                                        ,

                                                        {
                                                            "CA",
                                                            "California"
                                                            }
                                                        ,

                                                        {
                                                            "CO",
                                                            "Colorado"
                                                            }
                                                        ,

                                                        {
                                                            "CT",
                                                            "Connecticut"
                                                            }
                                                        ,

                                                        {
                                                            "DC",
                                                            "District of Columbia"
                                                            }
                                                        ,

                                                        {
                                                            "DE",
                                                            "Delaware"
                                                            }
                                                        ,

                                                        {
                                                            "FL",
                                                            "Florida"
                                                            }
                                                        ,

                                                        {
                                                            "GA",
                                                            "Georgia"
                                                            }
                                                        ,

                                                        {
                                                            "HI",
                                                            "Hawaii"
                                                            }
                                                        ,

                                                        {
                                                            "ID",
                                                            "Idaho"
                                                            }
                                                        ,

                                                        {
                                                            "IL",
                                                            "Illinois"
                                                            }
                                                        ,

                                                        {
                                                            "IN",
                                                            "Indiana"
                                                            }
                                                        ,

                                                        {
                                                            "IA",
                                                            "Iowa"
                                                            }
                                                        ,

                                                        {
                                                            "KS",
                                                            "Kansas"
                                                            }
                                                        ,

                                                        {
                                                            "KY",
                                                            "Kentucky"
                                                            }
                                                        ,

                                                        {
                                                            "LA",
                                                            "Louisiana"
                                                            }
                                                        ,

                                                        {
                                                            "ME",
                                                            "Maine"
                                                            }
                                                        ,

                                                        {
                                                            "MD",
                                                            "Maryland"
                                                            }
                                                        ,

                                                        {
                                                            "MA",
                                                            "Massachusetts"
                                                            }
                                                        ,

                                                        {
                                                            "MI",
                                                            "Michigan"
                                                            }
                                                        ,

                                                        {
                                                            "MN",
                                                            "Minnesota"
                                                            }
                                                        ,

                                                        {
                                                            "MS",
                                                            "Mississippi"
                                                            }
                                                        ,

                                                        {
                                                            "MO",
                                                            "Missouri"
                                                            }
                                                        ,

                                                        {
                                                            "MT",
                                                            "Montana"
                                                            }
                                                        ,

                                                        {
                                                            "NE",
                                                            "Nebraska"
                                                            }
                                                        ,

                                                        {
                                                            "NV",
                                                            "Nevada"
                                                            }
                                                        ,

                                                        {
                                                            "NH",
                                                            "New Hampshire"
                                                            }
                                                        ,

                                                        {
                                                            "NJ",
                                                            "New Jersey"
                                                            }
                                                        ,

                                                        {
                                                            "NM",
                                                            "New Mexico"
                                                            }
                                                        ,

                                                        {
                                                            "NY",
                                                            "New York"
                                                            }
                                                        ,

                                                        {
                                                            "NC",
                                                            "North Carolina"
                                                            }
                                                        ,

                                                        {
                                                            "ND",
                                                            "North Dakota"
                                                            }
                                                        ,

                                                        {
                                                            "OH",
                                                            "Ohio"
                                                            }
                                                        ,

                                                        {
                                                            "OK",
                                                            "Oklahoma"
                                                            }
                                                        ,

                                                        {
                                                            "OR",
                                                            "Oregon"
                                                            }
                                                        ,

                                                        {
                                                            "PA",
                                                            "Pennsylvania"
                                                            }
                                                        ,

                                                        {
                                                            "RI",
                                                            "Rhode Island"
                                                            }
                                                        ,

                                                        {
                                                            "SC",
                                                            "South Carolina"
                                                            }
                                                        ,

                                                        {
                                                            "SD",
                                                            "South Dakota"
                                                            }
                                                        ,

                                                        {
                                                            "TN",
                                                            "Tennessee"
                                                            }
                                                        ,

                                                        {
                                                            "TX",
                                                            "Texas"
                                                            }
                                                        ,

                                                        {
                                                            "UT",
                                                            "Utah"
                                                            }
                                                        ,

                                                        {
                                                            "VT",
                                                            "Vermont"
                                                            }
                                                        ,

                                                        {
                                                            "VA",
                                                            "Virginia"
                                                            }
                                                        ,

                                                        {
                                                            "WA",
                                                            "Washington"
                                                            }
                                                        ,

                                                        {
                                                            "WV",
                                                            "West Virginia"
                                                            }
                                                        ,

                                                        {
                                                            "WI",
                                                            "Wisconsin"
                                                            }
                                                        ,

                                                        {
                                                            "WY",
                                                            "Wyoming"
                                                            }
                                                    };
    }
}