using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace tradelr.Library.geo
{
    public class Country
    {
        public int id { get; set;}
        public string code { get; private set; }
        public string wbcode { get; private set; }
        public string name { get; private set; }
        public string latitude { get; private set; }
        public string longtitude { get; private set; }
        public int etsyid { get; set; }

        public static Country GetCountry(int id)
        {
            if (Values.ContainsKey(id))
            {
                return Values[id];
            }
            return null;
        }

        public static IEnumerable<SelectListItem> GetCountries()
        {
            var countries = new List<SelectListItem>();
            var values = Values.Values;
            foreach (var value in values)
            {
                var entry = new SelectListItem {Value = value.id.ToString(), Text = value.name};
                countries.Add(entry);
            }
            return countries;
        }

        #region countries
        public static readonly Dictionary<int, Country> Values = new Dictionary<int, Country>()
                                                                     {
                                                                         {
                                                                             2,
                                                                             new Country
                                                                                 {
                                                                                     id = 2,
                                                                                     etsyid = 55,
                                                                                     code = "AF",
                                                                                     wbcode = "AFG",
                                                                                     name = "Afghanistan",
                                                                                     latitude = "33.78",
                                                                                     longtitude = "66.17"
                                                                                 }
                                                                             },
                                                                         {
                                                                             3,
                                                                             new Country
                                                                                 {
                                                                                     id = 3,
                                                                                     etsyid = 57,
                                                                                     code = "AL",
                                                                                     wbcode = "ALB",
                                                                                     name = "Albania",
                                                                                     latitude = "41.14",
                                                                                     longtitude = "20.26"
                                                                                 }
                                                                             },
                                                                         {
                                                                             4,
                                                                             new Country
                                                                                 {
                                                                                     id = 4,
                                                                                     etsyid = 95,
                                                                                     code = "DZ",
                                                                                     wbcode = "DZA",
                                                                                     name = "Algeria",
                                                                                     latitude = "28.14",
                                                                                     longtitude = "2.83"
                                                                                 }
                                                                             },
                                                                         {
                                                                             239,
                                                                             new Country
                                                                                 {
                                                                                     id = 239,
                                                                                     etsyid = 250,
                                                                                     code = "AS",
                                                                                     wbcode = "ASM",
                                                                                     name = "American Samoa",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             5,
                                                                             new Country
                                                                                 {
                                                                                     id = 5,
                                                                                     etsyid = 228,
                                                                                     code = "AD",
                                                                                     wbcode = "AND",
                                                                                     name = "Andorra",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             6,
                                                                             new Country
                                                                                 {
                                                                                     id = 6,
                                                                                     etsyid = 56,
                                                                                     code = "AO",
                                                                                     wbcode = "AGO",
                                                                                     name = "Angola",
                                                                                     latitude = "-12.36",
                                                                                     longtitude = "17.74"
                                                                                 }
                                                                             },
                                                                         {
                                                                             224,
                                                                             new Country
                                                                                 {
                                                                                     id = 224,
                                                                                     etsyid = 251,
                                                                                     code = "AI",
                                                                                     wbcode = "AIA",
                                                                                     name = "Anguilla",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             7,
                                                                             new Country
                                                                                 {
                                                                                     id = 7,
                                                                                     etsyid = 252,
                                                                                     code = "AG",
                                                                                     wbcode = "ATG",
                                                                                     name = "Antigua and Barbuda",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             8,
                                                                             new Country
                                                                                 {
                                                                                     id = 8,
                                                                                     etsyid = 59,
                                                                                     code = "AR",
                                                                                     wbcode = "ARG",
                                                                                     name = "Argentina",
                                                                                     latitude = "-35.39",
                                                                                     longtitude = "-64.92"
                                                                                 }
                                                                             },
                                                                         {
                                                                             9,
                                                                             new Country
                                                                                 {
                                                                                     id = 9,
                                                                                     etsyid = 60,
                                                                                     code = "AM",
                                                                                     wbcode = "ARM",
                                                                                     name = "Armenia",
                                                                                     latitude = "40.31",
                                                                                     longtitude = "45.22"
                                                                                 }
                                                                             },
                                                                         {
                                                                             249,
                                                                             new Country
                                                                                 {
                                                                                     id = 249,
                                                                                     etsyid = 253,
                                                                                     code = "AW",
                                                                                     wbcode = "ABW",
                                                                                     name = "Aruba",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             10,
                                                                             new Country
                                                                                 {
                                                                                     id = 10,
                                                                                     etsyid = 61,
                                                                                     code = "AU",
                                                                                     wbcode = "AUS",
                                                                                     name = "Australia",
                                                                                     latitude = "-25.85",
                                                                                     longtitude = "134.61"
                                                                                 }
                                                                             },
                                                                         {
                                                                             11,
                                                                             new Country
                                                                                 {
                                                                                     id = 11,
                                                                                     etsyid = 62,
                                                                                     code = "AT",
                                                                                     wbcode = "AUT",
                                                                                     name = "Austria",
                                                                                     latitude = "47.6",
                                                                                     longtitude = "14.32"
                                                                                 }
                                                                             },
                                                                         {
                                                                             12,
                                                                             new Country
                                                                                 {
                                                                                     id = 12,
                                                                                     etsyid = 63,
                                                                                     code = "AZ",
                                                                                     wbcode = "AZE",
                                                                                     name = "Azerbaijan",
                                                                                     latitude = "40.27",
                                                                                     longtitude = "47.82"
                                                                                 }
                                                                             },
                                                                         {
                                                                             13,
                                                                             new Country
                                                                                 {
                                                                                     id = 13,
                                                                                     etsyid = 229,
                                                                                     code = "BS",
                                                                                     wbcode = "BHS",
                                                                                     name = "Bahamas",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             14,
                                                                             new Country
                                                                                 {
                                                                                     id = 14,
                                                                                     etsyid = 232,
                                                                                     code = "BH",
                                                                                     wbcode = "BHR",
                                                                                     name = "Bahrain",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             15,
                                                                             new Country
                                                                                 {
                                                                                     id = 15,
                                                                                     etsyid = 68,
                                                                                     code = "BD",
                                                                                     wbcode = "BGD",
                                                                                     name = "Bangladesh",
                                                                                     latitude = "23.81",
                                                                                     longtitude = "90.42"
                                                                                 }
                                                                             },
                                                                         {
                                                                             16,
                                                                             new Country
                                                                                 {
                                                                                     id = 16,
                                                                                     etsyid = 237,
                                                                                     code = "BB",
                                                                                     wbcode = "BRB",
                                                                                     name = "Barbados",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             17,
                                                                             new Country
                                                                                 {
                                                                                     id = 17,
                                                                                     etsyid = 71,
                                                                                     code = "BY",
                                                                                     wbcode = "BLR",
                                                                                     name = "Belarus",
                                                                                     latitude = "53.55",
                                                                                     longtitude = "28.23"
                                                                                 }
                                                                             },
                                                                         {
                                                                             18,
                                                                             new Country
                                                                                 {
                                                                                     id = 18,
                                                                                     etsyid = 65,
                                                                                     code = "BE",
                                                                                     wbcode = "BEL",
                                                                                     name = "Belgium",
                                                                                     latitude = "50.66",
                                                                                     longtitude = "4.83"
                                                                                 }
                                                                             },
                                                                         {
                                                                             19,
                                                                             new Country
                                                                                 {
                                                                                     id = 19,
                                                                                     etsyid = 72,
                                                                                     code = "BZ",
                                                                                     wbcode = "BLZ",
                                                                                     name = "Belize",
                                                                                     latitude = "17.22",
                                                                                     longtitude = "-88.53"
                                                                                 }
                                                                             },
                                                                         {
                                                                             20,
                                                                             new Country
                                                                                 {
                                                                                     id = 20,
                                                                                     etsyid = 66,
                                                                                     code = "BJ",
                                                                                     wbcode = "BEN",
                                                                                     name = "Benin",
                                                                                     latitude = "9.62",
                                                                                     longtitude = "2.55"
                                                                                 }
                                                                             },
                                                                         {
                                                                             225,
                                                                             new Country
                                                                                 {
                                                                                     id = 225,
                                                                                     etsyid = 225,
                                                                                     code = "BM",
                                                                                     wbcode = "BMU",
                                                                                     name = "Bermuda",
                                                                                     latitude = "32.4",
                                                                                     longtitude = "64.7"
                                                                                 }
                                                                             },
                                                                         {
                                                                             21,
                                                                             new Country
                                                                                 {
                                                                                     id = 21,
                                                                                     etsyid = 76,
                                                                                     code = "BT",
                                                                                     wbcode = "BTN",
                                                                                     name = "Bhutan",
                                                                                     latitude = "27.4",
                                                                                     longtitude = "90.58"
                                                                                 }
                                                                             },
                                                                         {
                                                                             22,
                                                                             new Country
                                                                                 {
                                                                                     id = 22,
                                                                                     etsyid = 73,
                                                                                     code = "BO",
                                                                                     wbcode = "BOL",
                                                                                     name = "Bolivia",
                                                                                     latitude = "-16.73",
                                                                                     longtitude = "-64.44"
                                                                                 }
                                                                             },
                                                                         {
                                                                             23,
                                                                             new Country
                                                                                 {
                                                                                     id = 23,
                                                                                     etsyid = 70,
                                                                                     code = "BA",
                                                                                     wbcode = "BIH",
                                                                                     name = "Bosnia and Herzegovina",
                                                                                     latitude = "44.18",
                                                                                     longtitude = "17.99"
                                                                                 }
                                                                             },
                                                                         {
                                                                             24,
                                                                             new Country
                                                                                 {
                                                                                     id = 24,
                                                                                     etsyid = 77,
                                                                                     code = "BW",
                                                                                     wbcode = "BWA",
                                                                                     name = "Botswana",
                                                                                     latitude = "-22.26",
                                                                                     longtitude = "23.96"
                                                                                 }
                                                                             },
                                                                         {
                                                                             217,
                                                                             new Country
                                                                                 {
                                                                                     id = 217,
                                                                                     etsyid = 254,
                                                                                     code = "BV",
                                                                                     wbcode = "BVT",
                                                                                     name = "Bouvet Island",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             25,
                                                                             new Country
                                                                                 {
                                                                                     id = 25,
                                                                                     etsyid = 74,
                                                                                     code = "BR",
                                                                                     wbcode = "BRA",
                                                                                     name = "Brazil",
                                                                                     latitude = "-10.83",
                                                                                     longtitude = "-52.87"
                                                                                 }
                                                                             },
                                                                         {
                                                                             226,
                                                                             new Country
                                                                                 {
                                                                                     id = 226,
                                                                                     etsyid = 255,
                                                                                     code = "IO",
                                                                                     wbcode = "IOT",
                                                                                     name =
                                                                                         "British Indian Ocean Territory",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             228,
                                                                             new Country
                                                                                 {
                                                                                     id = 228,
                                                                                     etsyid = 231,
                                                                                     code = "VG",
                                                                                     wbcode = "BVI",
                                                                                     name = "British Virgin Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             26,
                                                                             new Country
                                                                                 {
                                                                                     id = 26,
                                                                                     etsyid = 75,
                                                                                     code = "BN",
                                                                                     wbcode = "BRN",
                                                                                     name = "Brunei",
                                                                                     latitude = "4.48",
                                                                                     longtitude = "114.86"
                                                                                 }
                                                                             },
                                                                         {
                                                                             27,
                                                                             new Country
                                                                                 {
                                                                                     id = 27,
                                                                                     etsyid = 69,
                                                                                     code = "BG",
                                                                                     wbcode = "BGR",
                                                                                     name = "Bulgaria",
                                                                                     latitude = "42.79",
                                                                                     longtitude = "25.35"
                                                                                 }
                                                                             },
                                                                         {
                                                                             28,
                                                                             new Country
                                                                                 {
                                                                                     id = 28,
                                                                                     etsyid = 67,
                                                                                     code = "BF",
                                                                                     wbcode = "BFA",
                                                                                     name = "Burkina Faso",
                                                                                     latitude = "12.27",
                                                                                     longtitude = "-1.54"
                                                                                 }
                                                                             },
                                                                         {
                                                                             29,
                                                                             new Country
                                                                                 {
                                                                                     id = 29,
                                                                                     etsyid = 64,
                                                                                     code = "BI",
                                                                                     wbcode = "BDI",
                                                                                     name = "Burundi",
                                                                                     latitude = "-3.51",
                                                                                     longtitude = "30.07"
                                                                                 }
                                                                             },
                                                                         {
                                                                             30,
                                                                             new Country
                                                                                 {
                                                                                     id = 30,
                                                                                     etsyid = 135,
                                                                                     code = "KH",
                                                                                     wbcode = "KHM",
                                                                                     name = "Cambodia",
                                                                                     latitude = "12.69",
                                                                                     longtitude = "105.04"
                                                                                 }
                                                                             },
                                                                         {
                                                                             31,
                                                                             new Country
                                                                                 {
                                                                                     id = 31,
                                                                                     etsyid = 84,
                                                                                     code = "CM",
                                                                                     wbcode = "CMR",
                                                                                     name = "Cameroon",
                                                                                     latitude = "5.68",
                                                                                     longtitude = "12.92"
                                                                                 }
                                                                             },
                                                                         {
                                                                             32,
                                                                             new Country
                                                                                 {
                                                                                     id = 32,
                                                                                     etsyid = 79,
                                                                                     code = "CA",
                                                                                     wbcode = "CAN",
                                                                                     name = "Canada",
                                                                                     latitude = "61.06",
                                                                                     longtitude = "-98.38"
                                                                                 }
                                                                             },
                                                                         {
                                                                             33,
                                                                             new Country
                                                                                 {
                                                                                     id = 33,
                                                                                     etsyid = 222,
                                                                                     code = "CV",
                                                                                     wbcode = "CPV",
                                                                                     name = "Cape Verde",
                                                                                     latitude = "14.9",
                                                                                     longtitude = "23.5"
                                                                                 }
                                                                             },
                                                                         {
                                                                             229,
                                                                             new Country
                                                                                 {
                                                                                     id = 229,
                                                                                     etsyid = 247,
                                                                                     code = "KY",
                                                                                     wbcode = "KTD",
                                                                                     name = "Cayman Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             34,
                                                                             new Country
                                                                                 {
                                                                                     id = 34,
                                                                                     etsyid = 78,
                                                                                     code = "CF",
                                                                                     wbcode = "CAF",
                                                                                     name = "Central African Republic",
                                                                                     latitude = "6.5",
                                                                                     longtitude = "20.6"
                                                                                 }
                                                                             },
                                                                         {
                                                                             35,
                                                                             new Country
                                                                                 {
                                                                                     id = 35,
                                                                                     etsyid = 196,
                                                                                     code = "TD",
                                                                                     wbcode = "TCD",
                                                                                     name = "Chad",
                                                                                     latitude = "15.31",
                                                                                     longtitude = "18.81"
                                                                                 }
                                                                             },
                                                                         {
                                                                             36,
                                                                             new Country
                                                                                 {
                                                                                     id = 36,
                                                                                     etsyid = 81,
                                                                                     code = "CL",
                                                                                     wbcode = "CHL",
                                                                                     name = "Chile",
                                                                                     latitude = "-35.82",
                                                                                     longtitude = "-70.89"
                                                                                 }
                                                                             },
                                                                         {
                                                                             37,
                                                                             new Country
                                                                                 {
                                                                                     id = 37,
                                                                                     etsyid = 82,
                                                                                     code = "CN",
                                                                                     wbcode = "CHN",
                                                                                     name = "China",
                                                                                     latitude = "36.59",
                                                                                     longtitude = "103.94"
                                                                                 }
                                                                             },
                                                                         {
                                                                             203,
                                                                             new Country
                                                                                 {
                                                                                     id = 203,
                                                                                     etsyid = 257,
                                                                                     code = "CX",
                                                                                     wbcode = "CXR",
                                                                                     name = "Christmas Island",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             204,
                                                                             new Country
                                                                                 {
                                                                                     id = 204,
                                                                                     etsyid = 258,
                                                                                     code = "CC",
                                                                                     wbcode = "CCK",
                                                                                     name = "Cocos (Keeling) Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             38,
                                                                             new Country
                                                                                 {
                                                                                     id = 38,
                                                                                     etsyid = 86,
                                                                                     code = "CO",
                                                                                     wbcode = "COL",
                                                                                     name = "Colombia",
                                                                                     latitude = "3.88",
                                                                                     longtitude = "-72.87"
                                                                                 }
                                                                             },
                                                                         {
                                                                             39,
                                                                             new Country
                                                                                 {
                                                                                     id = 39,
                                                                                     etsyid = 259,
                                                                                     code = "KM",
                                                                                     wbcode = "COM",
                                                                                     name = "Comoros",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             41,
                                                                             new Country
                                                                                 {
                                                                                     id = 41,
                                                                                     etsyid = 85,
                                                                                     code = "CG",
                                                                                     wbcode = "COG",
                                                                                     name = "Congo, Republic of",
                                                                                     latitude = "-0.88",
                                                                                     longtitude = "15.34"
                                                                                 }
                                                                             },
                                                                         {
                                                                             218,
                                                                             new Country
                                                                                 {
                                                                                     id = 218,
                                                                                     etsyid = 260,
                                                                                     code = "CK",
                                                                                     wbcode = "COK",
                                                                                     name = "Cook Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             42,
                                                                             new Country
                                                                                 {
                                                                                     id = 42,
                                                                                     etsyid = 87,
                                                                                     code = "CR",
                                                                                     wbcode = "CRI",
                                                                                     name = "Costa Rica",
                                                                                     latitude = "10.01",
                                                                                     longtitude = "-83.95"
                                                                                 }
                                                                             },
                                                                         {
                                                                             44,
                                                                             new Country
                                                                                 {
                                                                                     id = 44,
                                                                                     etsyid = 118,
                                                                                     code = "HR",
                                                                                     wbcode = "HRV",
                                                                                     name = "Croatia",
                                                                                     latitude = "45.11",
                                                                                     longtitude = "16.65"
                                                                                 }
                                                                             },
                                                                         {
                                                                             45,
                                                                             new Country
                                                                                 {
                                                                                     id = 45,
                                                                                     etsyid = 88,
                                                                                     code = "CU",
                                                                                     wbcode = "CUB",
                                                                                     name = "Cuba",
                                                                                     latitude = "21.7",
                                                                                     longtitude = "-78.79"
                                                                                 }
                                                                             },
                                                                         {
                                                                             46,
                                                                             new Country
                                                                                 {
                                                                                     id = 46,
                                                                                     etsyid = 89,
                                                                                     code = "CY",
                                                                                     wbcode = "CYP",
                                                                                     name = "Cyprus",
                                                                                     latitude = "35.03",
                                                                                     longtitude = "33.29"
                                                                                 }
                                                                             },
                                                                         {
                                                                             47,
                                                                             new Country
                                                                                 {
                                                                                     id = 47,
                                                                                     etsyid = 90,
                                                                                     code = "CZ",
                                                                                     wbcode = "CZE",
                                                                                     name = "Czech Republic",
                                                                                     latitude = "49.78",
                                                                                     longtitude = "15.54"
                                                                                 }
                                                                             },
                                                                         {
                                                                             48,
                                                                             new Country
                                                                                 {
                                                                                     id = 48,
                                                                                     etsyid = 93,
                                                                                     code = "DK",
                                                                                     wbcode = "DNK",
                                                                                     name = "Denmark",
                                                                                     latitude = "56.11",
                                                                                     longtitude = "9.97"
                                                                                 }
                                                                             },
                                                                         {
                                                                             49,
                                                                             new Country
                                                                                 {
                                                                                     id = 49,
                                                                                     etsyid = 92,
                                                                                     code = "DJ",
                                                                                     wbcode = "DJI",
                                                                                     name = "Djibouti",
                                                                                     latitude = "11.7",
                                                                                     longtitude = "42.78"
                                                                                 }
                                                                             },
                                                                         {
                                                                             50,
                                                                             new Country
                                                                                 {
                                                                                     id = 50,
                                                                                     etsyid = 261,
                                                                                     code = "DM",
                                                                                     wbcode = "DMA",
                                                                                     name = "Dominica",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             51,
                                                                             new Country
                                                                                 {
                                                                                     id = 51,
                                                                                     etsyid = 94,
                                                                                     code = "DO",
                                                                                     wbcode = "DOM",
                                                                                     name = "Dominican Republic",
                                                                                     latitude = "18.95",
                                                                                     longtitude = "-70.32"
                                                                                 }
                                                                             },
                                                                         {
                                                                             52,
                                                                             new Country
                                                                                 {
                                                                                     id = 52,
                                                                                     etsyid = 96,
                                                                                     code = "EC",
                                                                                     wbcode = "ECU",
                                                                                     name = "Ecuador",
                                                                                     latitude = "-1.46",
                                                                                     longtitude = "-78.19"
                                                                                 }
                                                                             },
                                                                         {
                                                                             53,
                                                                             new Country
                                                                                 {
                                                                                     id = 53,
                                                                                     etsyid = 97,
                                                                                     code = "EG",
                                                                                     wbcode = "EGY",
                                                                                     name = "Egypt",
                                                                                     latitude = "26.41",
                                                                                     longtitude = "30.03"
                                                                                 }
                                                                             },
                                                                         {
                                                                             54,
                                                                             new Country
                                                                                 {
                                                                                     id = 54,
                                                                                     etsyid = 187,
                                                                                     code = "SV",
                                                                                     wbcode = "SLV",
                                                                                     name = "El Salvador",
                                                                                     latitude = "13.78",
                                                                                     longtitude = "-88.68"
                                                                                 }
                                                                             },
                                                                         {
                                                                             55,
                                                                             new Country
                                                                                 {
                                                                                     id = 55,
                                                                                     etsyid = 111,
                                                                                     code = "GQ",
                                                                                     wbcode = "GNQ",
                                                                                     name = "Equatorial Guinea",
                                                                                     latitude = "1.64",
                                                                                     longtitude = "10.48"
                                                                                 }
                                                                             },
                                                                         {
                                                                             56,
                                                                             new Country
                                                                                 {
                                                                                     id = 56,
                                                                                     etsyid = 98,
                                                                                     code = "ER",
                                                                                     wbcode = "ERI",
                                                                                     name = "Eritrea",
                                                                                     latitude = "15.26",
                                                                                     longtitude = "39.17"
                                                                                 }
                                                                             },
                                                                         {
                                                                             57,
                                                                             new Country
                                                                                 {
                                                                                     id = 57,
                                                                                     etsyid = 100,
                                                                                     code = "EE",
                                                                                     wbcode = "EST",
                                                                                     name = "Estonia",
                                                                                     latitude = "58.63",
                                                                                     longtitude = "25.99"
                                                                                 }
                                                                             },
                                                                         {
                                                                             58,
                                                                             new Country
                                                                                 {
                                                                                     id = 58,
                                                                                     etsyid = 101,
                                                                                     code = "ET",
                                                                                     wbcode = "ETH",
                                                                                     name = "Ethiopia",
                                                                                     latitude = "8.56",
                                                                                     longtitude = "39.81"
                                                                                 }
                                                                             },
                                                                         {
                                                                             230,
                                                                             new Country
                                                                                 {
                                                                                     id = 230,
                                                                                     etsyid = 262,
                                                                                     code = "FK",
                                                                                     wbcode = "FLK",
                                                                                     name = "Falkland Islands (Malvinas)",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             261,
                                                                             new Country
                                                                                 {
                                                                                     id = 261,
                                                                                     etsyid = 241,
                                                                                     code = "FO",
                                                                                     wbcode = "FRO",
                                                                                     name = "Faroe Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             59,
                                                                             new Country
                                                                                 {
                                                                                     id = 59,
                                                                                     etsyid = 234,
                                                                                     code = "FJ",
                                                                                     wbcode = "FJI",
                                                                                     name = "Fiji",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             60,
                                                                             new Country
                                                                                 {
                                                                                     id = 60,
                                                                                     etsyid = 102,
                                                                                     code = "FI",
                                                                                     wbcode = "FIN",
                                                                                     name = "Finland",
                                                                                     latitude = "64.43",
                                                                                     longtitude = "26.45"
                                                                                 }
                                                                             },
                                                                         {
                                                                             61,
                                                                             new Country
                                                                                 {
                                                                                     id = 61,
                                                                                     etsyid = 103,
                                                                                     code = "FR",
                                                                                     wbcode = "FRA",
                                                                                     name = "France",
                                                                                     latitude = "46.53",
                                                                                     longtitude = "2.72"
                                                                                 }
                                                                             },
                                                                         {
                                                                             244,
                                                                             new Country
                                                                                 {
                                                                                     id = 244,
                                                                                     etsyid = 115,
                                                                                     code = "GF",
                                                                                     wbcode = "GUF",
                                                                                     name = "French Guiana",
                                                                                     latitude = "3.86",
                                                                                     longtitude = "-52.97"
                                                                                 }
                                                                             },
                                                                         {
                                                                             209,
                                                                             new Country
                                                                                 {
                                                                                     id = 209,
                                                                                     etsyid = 263,
                                                                                     code = "PF",
                                                                                     wbcode = "PYF",
                                                                                     name = "French Polynesia",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             215,
                                                                             new Country
                                                                                 {
                                                                                     id = 215,
                                                                                     etsyid = 264,
                                                                                     code = "TF",
                                                                                     wbcode = "ATF",
                                                                                     name = "French Southern Territories",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             62,
                                                                             new Country
                                                                                 {
                                                                                     id = 62,
                                                                                     etsyid = 104,
                                                                                     code = "GA",
                                                                                     wbcode = "GAB",
                                                                                     name = "Gabon",
                                                                                     latitude = "-0.65",
                                                                                     longtitude = "11.99"
                                                                                 }
                                                                             },
                                                                         {
                                                                             63,
                                                                             new Country
                                                                                 {
                                                                                     id = 63,
                                                                                     etsyid = 109,
                                                                                     code = "GM",
                                                                                     wbcode = "GMB",
                                                                                     name = "Gambia",
                                                                                     latitude = "13.46",
                                                                                     longtitude = "-15.3"
                                                                                 }
                                                                             },
                                                                         {
                                                                             64,
                                                                             new Country
                                                                                 {
                                                                                     id = 64,
                                                                                     etsyid = 106,
                                                                                     code = "GE",
                                                                                     wbcode = "GEO",
                                                                                     name = "Georgia",
                                                                                     latitude = "42.18",
                                                                                     longtitude = "43.77"
                                                                                 }
                                                                             },
                                                                         {
                                                                             65,
                                                                             new Country
                                                                                 {
                                                                                     id = 65,
                                                                                     etsyid = 91,
                                                                                     code = "DE",
                                                                                     wbcode = "DEU",
                                                                                     name = "Germany",
                                                                                     latitude = "51.08",
                                                                                     longtitude = "10.56"
                                                                                 }
                                                                             },
                                                                         {
                                                                             66,
                                                                             new Country
                                                                                 {
                                                                                     id = 66,
                                                                                     etsyid = 107,
                                                                                     code = "GH",
                                                                                     wbcode = "GHA",
                                                                                     name = "Ghana",
                                                                                     latitude = "7.94",
                                                                                     longtitude = "-1.01"
                                                                                 }
                                                                             },
                                                                         {
                                                                             231,
                                                                             new Country
                                                                                 {
                                                                                     id = 231,
                                                                                     etsyid = 226,
                                                                                     code = "GI",
                                                                                     wbcode = "GIB",
                                                                                     name = "Gibraltar",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             67,
                                                                             new Country
                                                                                 {
                                                                                     id = 67,
                                                                                     etsyid = 112,
                                                                                     code = "GR",
                                                                                     wbcode = "GRC",
                                                                                     name = "Greece",
                                                                                     latitude = "39.16",
                                                                                     longtitude = "22.88"
                                                                                 }
                                                                             },
                                                                         {
                                                                             243,
                                                                             new Country
                                                                                 {
                                                                                     id = 243,
                                                                                     etsyid = 113,
                                                                                     code = "GL",
                                                                                     wbcode = "GRL",
                                                                                     name = "Greenland",
                                                                                     latitude = "74.7",
                                                                                     longtitude = "-41.2"
                                                                                 }
                                                                             },
                                                                         {
                                                                             68,
                                                                             new Country
                                                                                 {
                                                                                     id = 68,
                                                                                     etsyid = 245,
                                                                                     code = "GD",
                                                                                     wbcode = "GRD",
                                                                                     name = "Grenada",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             245,
                                                                             new Country
                                                                                 {
                                                                                     id = 245,
                                                                                     etsyid = 265,
                                                                                     code = "GP",
                                                                                     wbcode = "GLP",
                                                                                     name = "Guadeloupe",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             240,
                                                                             new Country
                                                                                 {
                                                                                     id = 240,
                                                                                     etsyid = 266,
                                                                                     code = "GU",
                                                                                     wbcode = "GUM",
                                                                                     name = "Guam",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             69,
                                                                             new Country
                                                                                 {
                                                                                     id = 69,
                                                                                     etsyid = 114,
                                                                                     code = "GT",
                                                                                     wbcode = "GTM",
                                                                                     name = "Guatemala",
                                                                                     latitude = "15.74",
                                                                                     longtitude = "-90.18"
                                                                                 }
                                                                             },
                                                                         {
                                                                             70,
                                                                             new Country
                                                                                 {
                                                                                     id = 70,
                                                                                     etsyid = 108,
                                                                                     code = "GN",
                                                                                     wbcode = "GIN",
                                                                                     name = "Guinea",
                                                                                     latitude = "10.4",
                                                                                     longtitude = "-10.66"
                                                                                 }
                                                                             },
                                                                         {
                                                                             71,
                                                                             new Country
                                                                                 {
                                                                                     id = 71,
                                                                                     etsyid = 110,
                                                                                     code = "GW",
                                                                                     wbcode = "GNB",
                                                                                     name = "Guinea-Bissau",
                                                                                     latitude = "12.08",
                                                                                     longtitude = "-14.64"
                                                                                 }
                                                                             },
                                                                         {
                                                                             72,
                                                                             new Country
                                                                                 {
                                                                                     id = 72,
                                                                                     etsyid = 116,
                                                                                     code = "GY",
                                                                                     wbcode = "GUY",
                                                                                     name = "Guyana",
                                                                                     latitude = "4.74",
                                                                                     longtitude = "-58.7"
                                                                                 }
                                                                             },
                                                                         {
                                                                             73,
                                                                             new Country
                                                                                 {
                                                                                     id = 73,
                                                                                     etsyid = 119,
                                                                                     code = "HT",
                                                                                     wbcode = "HTI",
                                                                                     name = "Haiti",
                                                                                     latitude = "18.98",
                                                                                     longtitude = "-72.49"
                                                                                 }
                                                                             },
                                                                         {
                                                                             206,
                                                                             new Country
                                                                                 {
                                                                                     id = 206,
                                                                                     etsyid = 267,
                                                                                     code = "HM",
                                                                                     wbcode = "HMD",
                                                                                     name =
                                                                                         "Heard Island and McDonald Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             189,
                                                                             new Country
                                                                                 {
                                                                                     id = 189,
                                                                                     etsyid = 268,
                                                                                     code = "VA",
                                                                                     wbcode = "VAT",
                                                                                     name = "Holy See (Vatican City State)",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             74,
                                                                             new Country
                                                                                 {
                                                                                     id = 74,
                                                                                     etsyid = 117,
                                                                                     code = "HN",
                                                                                     wbcode = "HND",
                                                                                     name = "Honduras",
                                                                                     latitude = "14.84",
                                                                                     longtitude = "-86.46"
                                                                                 }
                                                                             },
                                                                         {
                                                                             241,
                                                                             new Country
                                                                                 {
                                                                                     id = 241,
                                                                                     etsyid = 219,
                                                                                     code = "HK",
                                                                                     wbcode = "HKG",
                                                                                     name = "Hong Kong",
                                                                                     latitude = "22.2823",
                                                                                     longtitude = "114.15"
                                                                                 }
                                                                             },
                                                                         {
                                                                             75,
                                                                             new Country
                                                                                 {
                                                                                     id = 75,
                                                                                     etsyid = 120,
                                                                                     code = "HU",
                                                                                     wbcode = "HUN",
                                                                                     name = "Hungary",
                                                                                     latitude = "47.21",
                                                                                     longtitude = "19.61"
                                                                                 }
                                                                             },
                                                                         {
                                                                             76,
                                                                             new Country
                                                                                 {
                                                                                     id = 76,
                                                                                     etsyid = 126,
                                                                                     code = "IS",
                                                                                     wbcode = "ISL",
                                                                                     name = "Iceland",
                                                                                     latitude = "64.92",
                                                                                     longtitude = "-18.3"
                                                                                 }
                                                                             },
                                                                         {
                                                                             77,
                                                                             new Country
                                                                                 {
                                                                                     id = 77,
                                                                                     etsyid = 122,
                                                                                     code = "IN",
                                                                                     wbcode = "IND",
                                                                                     name = "India",
                                                                                     latitude = "22.93",
                                                                                     longtitude = "79.81"
                                                                                 }
                                                                             },
                                                                         {
                                                                             78,
                                                                             new Country
                                                                                 {
                                                                                     id = 78,
                                                                                     etsyid = 121,
                                                                                     code = "ID",
                                                                                     wbcode = "IDN",
                                                                                     name = "Indonesia",
                                                                                     latitude = "-1.66",
                                                                                     longtitude = "116.28"
                                                                                 }
                                                                             },
                                                                         {
                                                                             79,
                                                                             new Country
                                                                                 {
                                                                                     id = 79,
                                                                                     etsyid = 124,
                                                                                     code = "IR",
                                                                                     wbcode = "IRN",
                                                                                     name = "Iran",
                                                                                     latitude = "32.52",
                                                                                     longtitude = "54.49"
                                                                                 }
                                                                             },
                                                                         {
                                                                             80,
                                                                             new Country
                                                                                 {
                                                                                     id = 80,
                                                                                     etsyid = 125,
                                                                                     code = "IQ",
                                                                                     wbcode = "IRQ",
                                                                                     name = "Iraq",
                                                                                     latitude = "32.95",
                                                                                     longtitude = "43.99"
                                                                                 }
                                                                             },
                                                                         {
                                                                             81,
                                                                             new Country
                                                                                 {
                                                                                     id = 81,
                                                                                     etsyid = 123,
                                                                                     code = "IE",
                                                                                     wbcode = "IRL",
                                                                                     name = "Ireland",
                                                                                     latitude = "53.16",
                                                                                     longtitude = "-7.96"
                                                                                 }
                                                                             },
                                                                         {
                                                                             222,
                                                                             new Country
                                                                                 {
                                                                                     id = 222,
                                                                                     etsyid = 269,
                                                                                     code = "IM",
                                                                                     wbcode = "IMN",
                                                                                     name = "Isle of Man",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             82,
                                                                             new Country
                                                                                 {
                                                                                     id = 82,
                                                                                     etsyid = 127,
                                                                                     code = "IL",
                                                                                     wbcode = "ISR",
                                                                                     name = "Israel",
                                                                                     latitude = "31.41",
                                                                                     longtitude = "35.21"
                                                                                 }
                                                                             },
                                                                         {
                                                                             83,
                                                                             new Country
                                                                                 {
                                                                                     id = 83,
                                                                                     etsyid = 128,
                                                                                     code = "IT",
                                                                                     wbcode = "ITA",
                                                                                     name = "Italy",
                                                                                     latitude = "42.88",
                                                                                     longtitude = "12.27"
                                                                                 }
                                                                             },
                                                                         {
                                                                             43,
                                                                             new Country
                                                                                 {
                                                                                     id = 43,
                                                                                     etsyid = 83,
                                                                                     code = "IC",
                                                                                     wbcode = "CIV",
                                                                                     name = "Ivory Coast",
                                                                                     latitude = "7.61",
                                                                                     longtitude = "-5.33"
                                                                                 }
                                                                             },
                                                                         {
                                                                             84,
                                                                             new Country
                                                                                 {
                                                                                     id = 84,
                                                                                     etsyid = 129,
                                                                                     code = "JM",
                                                                                     wbcode = "JAM",
                                                                                     name = "Jamaica",
                                                                                     latitude = "18.19",
                                                                                     longtitude = "-77.15"
                                                                                 }
                                                                             },
                                                                         {
                                                                             85,
                                                                             new Country
                                                                                 {
                                                                                     id = 85,
                                                                                     etsyid = 131,
                                                                                     code = "JP",
                                                                                     wbcode = "JPN",
                                                                                     name = "Japan",
                                                                                     latitude = "36.96",
                                                                                     longtitude = "138.85"
                                                                                 }
                                                                             },
                                                                         {
                                                                             86,
                                                                             new Country
                                                                                 {
                                                                                     id = 86,
                                                                                     etsyid = 130,
                                                                                     code = "JO",
                                                                                     wbcode = "JOR",
                                                                                     name = "Jordan",
                                                                                     latitude = "31.16",
                                                                                     longtitude = "37.03"
                                                                                 }
                                                                             },
                                                                         {
                                                                             87,
                                                                             new Country
                                                                                 {
                                                                                     id = 87,
                                                                                     etsyid = 132,
                                                                                     code = "KZ",
                                                                                     wbcode = "KAZ",
                                                                                     name = "Kazakhstan",
                                                                                     latitude = "48.19",
                                                                                     longtitude = "67.56"
                                                                                 }
                                                                             },
                                                                         {
                                                                             88,
                                                                             new Country
                                                                                 {
                                                                                     id = 88,
                                                                                     etsyid = 133,
                                                                                     code = "KE",
                                                                                     wbcode = "KEN",
                                                                                     name = "Kenya",
                                                                                     latitude = "0.42",
                                                                                     longtitude = "38.01"
                                                                                 }
                                                                             },
                                                                         {
                                                                             89,
                                                                             new Country
                                                                                 {
                                                                                     id = 89,
                                                                                     etsyid = 270,
                                                                                     code = "KI",
                                                                                     wbcode = "KIR",
                                                                                     name = "Kiribati",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             255,
                                                                             new Country
                                                                                 {
                                                                                     id = 255,
                                                                                     etsyid = 271,
                                                                                     code = "KV",
                                                                                     wbcode = "",
                                                                                     name = "Kosovo",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             92,
                                                                             new Country
                                                                                 {
                                                                                     id = 92,
                                                                                     etsyid = 137,
                                                                                     code = "KW",
                                                                                     wbcode = "KWT",
                                                                                     name = "Kuwait",
                                                                                     latitude = "29.16",
                                                                                     longtitude = "47.76"
                                                                                 }
                                                                             },
                                                                         {
                                                                             93,
                                                                             new Country
                                                                                 {
                                                                                     id = 93,
                                                                                     etsyid = 134,
                                                                                     code = "KG",
                                                                                     wbcode = "KGZ",
                                                                                     name = "Kyrgyzstan",
                                                                                     latitude = "41.49",
                                                                                     longtitude = "74.82"
                                                                                 }
                                                                             },
                                                                         {
                                                                             94,
                                                                             new Country
                                                                                 {
                                                                                     id = 94,
                                                                                     etsyid = 138,
                                                                                     code = "LA",
                                                                                     wbcode = "LAO",
                                                                                     name = "Laos",
                                                                                     latitude = "18.45",
                                                                                     longtitude = "103.87"
                                                                                 }
                                                                             },
                                                                         {
                                                                             95,
                                                                             new Country
                                                                                 {
                                                                                     id = 95,
                                                                                     etsyid = 146,
                                                                                     code = "LV",
                                                                                     wbcode = "LVA",
                                                                                     name = "Latvia",
                                                                                     latitude = "56.85",
                                                                                     longtitude = "25.17"
                                                                                 }
                                                                             },
                                                                         {
                                                                             96,
                                                                             new Country
                                                                                 {
                                                                                     id = 96,
                                                                                     etsyid = 139,
                                                                                     code = "LB",
                                                                                     wbcode = "LBN",
                                                                                     name = "Lebanon",
                                                                                     latitude = "33.79",
                                                                                     longtitude = "35.98"
                                                                                 }
                                                                             },
                                                                         {
                                                                             97,
                                                                             new Country
                                                                                 {
                                                                                     id = 97,
                                                                                     etsyid = 143,
                                                                                     code = "LS",
                                                                                     wbcode = "LSO",
                                                                                     name = "Lesotho",
                                                                                     latitude = "-29.69",
                                                                                     longtitude = "28.43"
                                                                                 }
                                                                             },
                                                                         {
                                                                             98,
                                                                             new Country
                                                                                 {
                                                                                     id = 98,
                                                                                     etsyid = 140,
                                                                                     code = "LR",
                                                                                     wbcode = "LBR",
                                                                                     name = "Liberia",
                                                                                     latitude = "6.43",
                                                                                     longtitude = "-9.06"
                                                                                 }
                                                                             },
                                                                         {
                                                                             99,
                                                                             new Country
                                                                                 {
                                                                                     id = 99,
                                                                                     etsyid = 141,
                                                                                     code = "LY",
                                                                                     wbcode = "LBY",
                                                                                     name = "Libya",
                                                                                     latitude = "26.99",
                                                                                     longtitude = "18.18"
                                                                                 }
                                                                             },
                                                                         {
                                                                             100,
                                                                             new Country
                                                                                 {
                                                                                     id = 100,
                                                                                     etsyid = 272,
                                                                                     code = "LI",
                                                                                     wbcode = "LIE",
                                                                                     name = "Liechtenstein",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             101,
                                                                             new Country
                                                                                 {
                                                                                     id = 101,
                                                                                     etsyid = 144,
                                                                                     code = "LT",
                                                                                     wbcode = "LTU",
                                                                                     name = "Lithuania",
                                                                                     latitude = "55.35",
                                                                                     longtitude = "24.06"
                                                                                 }
                                                                             },
                                                                         {
                                                                             102,
                                                                             new Country
                                                                                 {
                                                                                     id = 102,
                                                                                     etsyid = 145,
                                                                                     code = "LU",
                                                                                     wbcode = "LUX",
                                                                                     name = "Luxembourg",
                                                                                     latitude = "49.78",
                                                                                     longtitude = "6.3"
                                                                                 }
                                                                             },
                                                                         {
                                                                             242,
                                                                             new Country
                                                                                 {
                                                                                     id = 242,
                                                                                     etsyid = 273,
                                                                                     code = "MO",
                                                                                     wbcode = "MAC",
                                                                                     name = "Macao",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             103,
                                                                             new Country
                                                                                 {
                                                                                     id = 103,
                                                                                     etsyid = 151,
                                                                                     code = "MK",
                                                                                     wbcode = "MKD",
                                                                                     name = "Macedonia",
                                                                                     latitude = "41.57",
                                                                                     longtitude = "21.91"
                                                                                 }
                                                                             },
                                                                         {
                                                                             104,
                                                                             new Country
                                                                                 {
                                                                                     id = 104,
                                                                                     etsyid = 149,
                                                                                     code = "MG",
                                                                                     wbcode = "MDG",
                                                                                     name = "Madagascar",
                                                                                     latitude = "-19.43",
                                                                                     longtitude = "46.92"
                                                                                 }
                                                                             },
                                                                         {
                                                                             105,
                                                                             new Country
                                                                                 {
                                                                                     id = 105,
                                                                                     etsyid = 158,
                                                                                     code = "MW",
                                                                                     wbcode = "MWI",
                                                                                     name = "Malawi",
                                                                                     latitude = "-13.53",
                                                                                     longtitude = "34.49"
                                                                                 }
                                                                             },
                                                                         {
                                                                             106,
                                                                             new Country
                                                                                 {
                                                                                     id = 106,
                                                                                     etsyid = 159,
                                                                                     code = "MY",
                                                                                     wbcode = "MYS",
                                                                                     name = "Malaysia",
                                                                                     latitude = "2.16",
                                                                                     longtitude = "112.12"
                                                                                 }
                                                                             },
                                                                         {
                                                                             107,
                                                                             new Country
                                                                                 {
                                                                                     id = 107,
                                                                                     etsyid = 238,
                                                                                     code = "MV",
                                                                                     wbcode = "MDV",
                                                                                     name = "Maldives",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             108,
                                                                             new Country
                                                                                 {
                                                                                     id = 108,
                                                                                     etsyid = 152,
                                                                                     code = "ML",
                                                                                     wbcode = "MLI",
                                                                                     name = "Mali",
                                                                                     latitude = "17.33",
                                                                                     longtitude = "-3.31"
                                                                                 }
                                                                             },
                                                                         {
                                                                             109,
                                                                             new Country
                                                                                 {
                                                                                     id = 109,
                                                                                     etsyid = 227,
                                                                                     code = "MT",
                                                                                     wbcode = "MLT",
                                                                                     name = "Malta",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             110,
                                                                             new Country
                                                                                 {
                                                                                     id = 110,
                                                                                     etsyid = 274,
                                                                                     code = "MH",
                                                                                     wbcode = "MHL",
                                                                                     name = "Marshall Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             246,
                                                                             new Country
                                                                                 {
                                                                                     id = 246,
                                                                                     etsyid = 275,
                                                                                     code = "MQ",
                                                                                     wbcode = "MTQ",
                                                                                     name = "Martinique",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             111,
                                                                             new Country
                                                                                 {
                                                                                     id = 111,
                                                                                     etsyid = 157,
                                                                                     code = "MR",
                                                                                     wbcode = "MRT",
                                                                                     name = "Mauritania",
                                                                                     latitude = "20.27",
                                                                                     longtitude = "-10.07"
                                                                                 }
                                                                             },
                                                                         {
                                                                             112,
                                                                             new Country
                                                                                 {
                                                                                     id = 112,
                                                                                     etsyid = 239,
                                                                                     code = "MU",
                                                                                     wbcode = "MUS",
                                                                                     name = "Mauritius",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             210,
                                                                             new Country
                                                                                 {
                                                                                     id = 210,
                                                                                     etsyid = 276,
                                                                                     code = "YT",
                                                                                     wbcode = "MYT",
                                                                                     name = "Mayotte",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             113,
                                                                             new Country
                                                                                 {
                                                                                     id = 113,
                                                                                     etsyid = 150,
                                                                                     code = "MX",
                                                                                     wbcode = "MEX",
                                                                                     name = "Mexico",
                                                                                     latitude = "23.92",
                                                                                     longtitude = "-102.3"
                                                                                 }
                                                                             },
                                                                         {
                                                                             114,
                                                                             new Country
                                                                                 {
                                                                                     id = 114,
                                                                                     etsyid = 277,
                                                                                     code = "FM",
                                                                                     wbcode = "FSM",
                                                                                     name =
                                                                                         "Micronesia, Federated States of",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             115,
                                                                             new Country
                                                                                 {
                                                                                     id = 115,
                                                                                     etsyid = 148,
                                                                                     code = "MD",
                                                                                     wbcode = "MDA",
                                                                                     name = "Moldova",
                                                                                     latitude = "47.2",
                                                                                     longtitude = "28.66"
                                                                                 }
                                                                             },
                                                                         {
                                                                             116,
                                                                             new Country
                                                                                 {
                                                                                     id = 116,
                                                                                     etsyid = 278,
                                                                                     code = "MC",
                                                                                     wbcode = "MCO",
                                                                                     name = "Monaco",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             117,
                                                                             new Country
                                                                                 {
                                                                                     id = 117,
                                                                                     etsyid = 154,
                                                                                     code = "MN",
                                                                                     wbcode = "MNG",
                                                                                     name = "Mongolia",
                                                                                     latitude = "46.85",
                                                                                     longtitude = "103.27"
                                                                                 }
                                                                             },
                                                                         {
                                                                             118,
                                                                             new Country
                                                                                 {
                                                                                     id = 118,
                                                                                     etsyid = 155,
                                                                                     code = "ME",
                                                                                     wbcode = "MON",
                                                                                     name = "Montenegro",
                                                                                     latitude = "42.79",
                                                                                     longtitude = "19.45"
                                                                                 }
                                                                             },
                                                                         {
                                                                             232,
                                                                             new Country
                                                                                 {
                                                                                     id = 232,
                                                                                     etsyid = 279,
                                                                                     code = "MS",
                                                                                     wbcode = "MSR",
                                                                                     name = "Montserrat",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             119,
                                                                             new Country
                                                                                 {
                                                                                     id = 119,
                                                                                     etsyid = 147,
                                                                                     code = "MA",
                                                                                     wbcode = "MAR",
                                                                                     name = "Morocco",
                                                                                     latitude = "31.83",
                                                                                     longtitude = "-6.1"
                                                                                 }
                                                                             },
                                                                         {
                                                                             120,
                                                                             new Country
                                                                                 {
                                                                                     id = 120,
                                                                                     etsyid = 156,
                                                                                     code = "MZ",
                                                                                     wbcode = "MOZ",
                                                                                     name = "Mozambique",
                                                                                     latitude = "-17.29",
                                                                                     longtitude = "35.73"
                                                                                 }
                                                                             },
                                                                         {
                                                                             121,
                                                                             new Country
                                                                                 {
                                                                                     id = 121,
                                                                                     etsyid = 153,
                                                                                     code = "MM",
                                                                                     wbcode = "MMR",
                                                                                     name = "Myanmar (Burma)",
                                                                                     latitude = "21.17",
                                                                                     longtitude = "96.67"
                                                                                 }
                                                                             },
                                                                         {
                                                                             122,
                                                                             new Country
                                                                                 {
                                                                                     id = 122,
                                                                                     etsyid = 160,
                                                                                     code = "NA",
                                                                                     wbcode = "NAM",
                                                                                     name = "Namibia",
                                                                                     latitude = "-22.1",
                                                                                     longtitude = "17.41"
                                                                                 }
                                                                             },
                                                                         {
                                                                             123,
                                                                             new Country
                                                                                 {
                                                                                     id = 123,
                                                                                     etsyid = 280,
                                                                                     code = "NR",
                                                                                     wbcode = "NRU",
                                                                                     name = "Nauru",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             124,
                                                                             new Country
                                                                                 {
                                                                                     id = 124,
                                                                                     etsyid = 166,
                                                                                     code = "NP",
                                                                                     wbcode = "NPL",
                                                                                     name = "Nepal",
                                                                                     latitude = "28.3",
                                                                                     longtitude = "84.17"
                                                                                 }
                                                                             },
                                                                         {
                                                                             125,
                                                                             new Country
                                                                                 {
                                                                                     id = 125,
                                                                                     etsyid = 164,
                                                                                     code = "NL",
                                                                                     wbcode = "NLD",
                                                                                     name = "Netherlands",
                                                                                     latitude = "52.21",
                                                                                     longtitude = "5.82"
                                                                                 }
                                                                             },
                                                                         {
                                                                             250,
                                                                             new Country
                                                                                 {
                                                                                     id = 250,
                                                                                     etsyid = 243,
                                                                                     code = "AN",
                                                                                     wbcode = "ANT",
                                                                                     name = "Netherlands Antilles",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             208,
                                                                             new Country
                                                                                 {
                                                                                     id = 208,
                                                                                     etsyid = 233,
                                                                                     code = "NC",
                                                                                     wbcode = "NCL",
                                                                                     name = "New Caledonia",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             126,
                                                                             new Country
                                                                                 {
                                                                                     id = 126,
                                                                                     etsyid = 167,
                                                                                     code = "NZ",
                                                                                     wbcode = "NZL",
                                                                                     name = "New Zealand",
                                                                                     latitude = "-41.81",
                                                                                     longtitude = "172.9"
                                                                                 }
                                                                             },
                                                                         {
                                                                             127,
                                                                             new Country
                                                                                 {
                                                                                     id = 127,
                                                                                     etsyid = 163,
                                                                                     code = "NI",
                                                                                     wbcode = "NIC",
                                                                                     name = "Nicaragua",
                                                                                     latitude = "12.86",
                                                                                     longtitude = "-84.9"
                                                                                 }
                                                                             },
                                                                         {
                                                                             128,
                                                                             new Country
                                                                                 {
                                                                                     id = 128,
                                                                                     etsyid = 161,
                                                                                     code = "NE",
                                                                                     wbcode = "NER",
                                                                                     name = "Niger",
                                                                                     latitude = "17.42",
                                                                                     longtitude = "9.58"
                                                                                 }
                                                                             },
                                                                         {
                                                                             129,
                                                                             new Country
                                                                                 {
                                                                                     id = 129,
                                                                                     etsyid = 162,
                                                                                     code = "NG",
                                                                                     wbcode = "NGA",
                                                                                     name = "Nigeria",
                                                                                     latitude = "9.58",
                                                                                     longtitude = "8.28"
                                                                                 }
                                                                             },
                                                                         {
                                                                             219,
                                                                             new Country
                                                                                 {
                                                                                     id = 219,
                                                                                     etsyid = 281,
                                                                                     code = "NU",
                                                                                     wbcode = "NIU",
                                                                                     name = "Niue",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             207,
                                                                             new Country
                                                                                 {
                                                                                     id = 207,
                                                                                     etsyid = 282,
                                                                                     code = "NF",
                                                                                     wbcode = "NFK",
                                                                                     name = "Norfolk Island",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             237,
                                                                             new Country
                                                                                 {
                                                                                     id = 237,
                                                                                     etsyid = 283,
                                                                                     code = "MP",
                                                                                     wbcode = "MNP",
                                                                                     name = "Northern Mariana Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             90,
                                                                             new Country
                                                                                 {
                                                                                     id = 90,
                                                                                     etsyid = 176,
                                                                                     code = "KP",
                                                                                     wbcode = "PRK",
                                                                                     name = "North Korea",
                                                                                     latitude = "40.13",
                                                                                     longtitude = "127.25"
                                                                                 }
                                                                             },
                                                                         {
                                                                             130,
                                                                             new Country
                                                                                 {
                                                                                     id = 130,
                                                                                     etsyid = 165,
                                                                                     code = "NO",
                                                                                     wbcode = "NOR",
                                                                                     name = "Norway",
                                                                                     latitude = "67.47",
                                                                                     longtitude = "15.77"
                                                                                 }
                                                                             },
                                                                         {
                                                                             131,
                                                                             new Country
                                                                                 {
                                                                                     id = 131,
                                                                                     etsyid = 168,
                                                                                     code = "OM",
                                                                                     wbcode = "OMN",
                                                                                     name = "Oman",
                                                                                     latitude = "20.43",
                                                                                     longtitude = "56.14"
                                                                                 }
                                                                             },
                                                                         {
                                                                             132,
                                                                             new Country
                                                                                 {
                                                                                     id = 132,
                                                                                     etsyid = 169,
                                                                                     code = "PK",
                                                                                     wbcode = "PAK",
                                                                                     name = "Pakistan",
                                                                                     latitude = "29.92",
                                                                                     longtitude = "69.56"
                                                                                 }
                                                                             },
                                                                         {
                                                                             133,
                                                                             new Country
                                                                                 {
                                                                                     id = 133,
                                                                                     etsyid = 284,
                                                                                     code = "PW",
                                                                                     wbcode = "PLW",
                                                                                     name = "Palau",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             256,
                                                                             new Country
                                                                                 {
                                                                                     id = 256,
                                                                                     etsyid = 285,
                                                                                     code = "PS",
                                                                                     wbcode = "PSE",
                                                                                     name =
                                                                                         "Palestinian Territory, Occupied",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             134,
                                                                             new Country
                                                                                 {
                                                                                     id = 134,
                                                                                     etsyid = 170,
                                                                                     code = "PA",
                                                                                     wbcode = "PAN",
                                                                                     name = "Panama",
                                                                                     latitude = "8.57",
                                                                                     longtitude = "-79.91"
                                                                                 }
                                                                             },
                                                                         {
                                                                             135,
                                                                             new Country
                                                                                 {
                                                                                     id = 135,
                                                                                     etsyid = 173,
                                                                                     code = "PG",
                                                                                     wbcode = "PNG",
                                                                                     name = "Papua New Guinea",
                                                                                     latitude = "-6.62",
                                                                                     longtitude = "144.83"
                                                                                 }
                                                                             },
                                                                         {
                                                                             136,
                                                                             new Country
                                                                                 {
                                                                                     id = 136,
                                                                                     etsyid = 178,
                                                                                     code = "PY",
                                                                                     wbcode = "PRY",
                                                                                     name = "Paraguay",
                                                                                     latitude = "-23.2",
                                                                                     longtitude = "-58.17"
                                                                                 }
                                                                             },
                                                                         {
                                                                             137,
                                                                             new Country
                                                                                 {
                                                                                     id = 137,
                                                                                     etsyid = 171,
                                                                                     code = "PE",
                                                                                     wbcode = "PER",
                                                                                     name = "Peru",
                                                                                     latitude = "-9.18",
                                                                                     longtitude = "-74.14"
                                                                                 }
                                                                             },
                                                                         {
                                                                             138,
                                                                             new Country
                                                                                 {
                                                                                     id = 138,
                                                                                     etsyid = 172,
                                                                                     code = "PH",
                                                                                     wbcode = "PHL",
                                                                                     name = "Philippines",
                                                                                     latitude = "11.07",
                                                                                     longtitude = "122.99"
                                                                                 }
                                                                             },
                                                                         {
                                                                             139,
                                                                             new Country
                                                                                 {
                                                                                     id = 139,
                                                                                     etsyid = 174,
                                                                                     code = "PL",
                                                                                     wbcode = "POL",
                                                                                     name = "Poland",
                                                                                     latitude = "52.14",
                                                                                     longtitude = "19.64"
                                                                                 }
                                                                             },
                                                                         {
                                                                             140,
                                                                             new Country
                                                                                 {
                                                                                     id = 140,
                                                                                     etsyid = 177,
                                                                                     code = "PT",
                                                                                     wbcode = "PRT",
                                                                                     name = "Portugal",
                                                                                     latitude = "39.69",
                                                                                     longtitude = "-7.76"
                                                                                 }
                                                                             },
                                                                         {
                                                                             238,
                                                                             new Country
                                                                                 {
                                                                                     id = 238,
                                                                                     etsyid = 175,
                                                                                     code = "PR",
                                                                                     wbcode = "PRI",
                                                                                     name = "Puerto Rico",
                                                                                     latitude = "18.28",
                                                                                     longtitude = "-66.32"
                                                                                 }
                                                                             },
                                                                         {
                                                                             141,
                                                                             new Country
                                                                                 {
                                                                                     id = 141,
                                                                                     etsyid = 179,
                                                                                     code = "QA",
                                                                                     wbcode = "QAT",
                                                                                     name = "Qatar",
                                                                                     latitude = "25.23",
                                                                                     longtitude = "51.43"
                                                                                 }
                                                                             },
                                                                         {
                                                                             142,
                                                                             new Country
                                                                                 {
                                                                                     id = 142,
                                                                                     etsyid = 180,
                                                                                     code = "RO",
                                                                                     wbcode = "ROM",
                                                                                     name = "Romania",
                                                                                     latitude = "45.91",
                                                                                     longtitude = "25.18"
                                                                                 }
                                                                             },
                                                                         {
                                                                             143,
                                                                             new Country
                                                                                 {
                                                                                     id = 143,
                                                                                     etsyid = 181,
                                                                                     code = "RU",
                                                                                     wbcode = "RUS",
                                                                                     name = "Russia",
                                                                                     latitude = "61.7",
                                                                                     longtitude = "96.86"
                                                                                 }
                                                                             },
                                                                         {
                                                                             144,
                                                                             new Country
                                                                                 {
                                                                                     id = 144,
                                                                                     etsyid = 182,
                                                                                     code = "RW",
                                                                                     wbcode = "RWA",
                                                                                     name = "Rwanda",
                                                                                     latitude = "-2.08",
                                                                                     longtitude = "30.07"
                                                                                 }
                                                                             },
                                                                         {
                                                                             234,
                                                                             new Country
                                                                                 {
                                                                                     id = 234,
                                                                                     etsyid = 286,
                                                                                     code = "SH",
                                                                                     wbcode = "SHN",
                                                                                     name = "Saint Helena",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             145,
                                                                             new Country
                                                                                 {
                                                                                     id = 145,
                                                                                     etsyid = 287,
                                                                                     code = "KN",
                                                                                     wbcode = "KNA",
                                                                                     name = "Saint Kitts and Nevis",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             146,
                                                                             new Country
                                                                                 {
                                                                                     id = 146,
                                                                                     etsyid = 244,
                                                                                     code = "LC",
                                                                                     wbcode = "LCA",
                                                                                     name = "Saint Lucia",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             212,
                                                                             new Country
                                                                                 {
                                                                                     id = 212,
                                                                                     etsyid = 288,
                                                                                     code = "MF",
                                                                                     wbcode = "MAF",
                                                                                     name = "Saint Martin (French part)",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             213,
                                                                             new Country
                                                                                 {
                                                                                     id = 213,
                                                                                     etsyid = 289,
                                                                                     code = "PM",
                                                                                     wbcode = "SPM",
                                                                                     name = "Saint Pierre and Miquelon",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             147,
                                                                             new Country
                                                                                 {
                                                                                     id = 147,
                                                                                     etsyid = 249,
                                                                                     code = "VC",
                                                                                     wbcode = "VCT",
                                                                                     name =
                                                                                         "Saint Vincent and the Grenadines",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             148,
                                                                             new Country
                                                                                 {
                                                                                     id = 148,
                                                                                     etsyid = 290,
                                                                                     code = "WS",
                                                                                     wbcode = "WSM",
                                                                                     name = "Samoa",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             149,
                                                                             new Country
                                                                                 {
                                                                                     id = 149,
                                                                                     etsyid = 291,
                                                                                     code = "SM",
                                                                                     wbcode = "SMR",
                                                                                     name = "San Marino",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             150,
                                                                             new Country
                                                                                 {
                                                                                     id = 150,
                                                                                     etsyid = 292,
                                                                                     code = "ST",
                                                                                     wbcode = "STP",
                                                                                     name = "Sao Tome and Principe",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             151,
                                                                             new Country
                                                                                 {
                                                                                     id = 151,
                                                                                     etsyid = 183,
                                                                                     code = "SA",
                                                                                     wbcode = "SAU",
                                                                                     name = "Saudi Arabia",
                                                                                     latitude = "23.93",
                                                                                     longtitude = "44.68"
                                                                                 }
                                                                             },
                                                                         {
                                                                             152,
                                                                             new Country
                                                                                 {
                                                                                     id = 152,
                                                                                     etsyid = 185,
                                                                                     code = "SN",
                                                                                     wbcode = "SEN",
                                                                                     name = "Senegal",
                                                                                     latitude = "14.45",
                                                                                     longtitude = "-14.23"
                                                                                 }
                                                                             },
                                                                         {
                                                                             153,
                                                                             new Country
                                                                                 {
                                                                                     id = 153,
                                                                                     etsyid = 189,
                                                                                     code = "RS",
                                                                                     wbcode = "SRB",
                                                                                     name = "Serbia",
                                                                                     latitude = "44.05",
                                                                                     longtitude = "20.99"
                                                                                 }
                                                                             },
                                                                         {
                                                                             154,
                                                                             new Country
                                                                                 {
                                                                                     id = 154,
                                                                                     etsyid = 293,
                                                                                     code = "SC",
                                                                                     wbcode = "SYC",
                                                                                     name = "Seychelles",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             155,
                                                                             new Country
                                                                                 {
                                                                                     id = 155,
                                                                                     etsyid = 186,
                                                                                     code = "SL",
                                                                                     wbcode = "SLE",
                                                                                     name = "Sierra Leone",
                                                                                     latitude = "8.54",
                                                                                     longtitude = "-11.51"
                                                                                 }
                                                                             },
                                                                         {
                                                                             156,
                                                                             new Country
                                                                                 {
                                                                                     id = 156,
                                                                                     etsyid = 220,
                                                                                     code = "SG",
                                                                                     wbcode = "SGP",
                                                                                     name = "Singapore",
                                                                                     latitude = "1.3403",
                                                                                     longtitude = "103.7758"
                                                                                 }
                                                                             },
                                                                         {
                                                                             157,
                                                                             new Country
                                                                                 {
                                                                                     id = 157,
                                                                                     etsyid = 191,
                                                                                     code = "SK",
                                                                                     wbcode = "SVK",
                                                                                     name = "Slovakia",
                                                                                     latitude = "48.79",
                                                                                     longtitude = "19.75"
                                                                                 }
                                                                             },
                                                                         {
                                                                             158,
                                                                             new Country
                                                                                 {
                                                                                     id = 158,
                                                                                     etsyid = 192,
                                                                                     code = "SI",
                                                                                     wbcode = "SVN",
                                                                                     name = "Slovenia",
                                                                                     latitude = "46.13",
                                                                                     longtitude = "15.01"
                                                                                 }
                                                                             },
                                                                         {
                                                                             159,
                                                                             new Country
                                                                                 {
                                                                                     id = 159,
                                                                                     etsyid = 242,
                                                                                     code = "SB",
                                                                                     wbcode = "SLB",
                                                                                     name = "Solomon Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             160,
                                                                             new Country
                                                                                 {
                                                                                     id = 160,
                                                                                     etsyid = 188,
                                                                                     code = "SO",
                                                                                     wbcode = "SOM",
                                                                                     name = "Somalia",
                                                                                     latitude = "6.04",
                                                                                     longtitude = "46.06"
                                                                                 }
                                                                             },
                                                                         {
                                                                             161,
                                                                             new Country
                                                                                 {
                                                                                     id = 161,
                                                                                     etsyid = 215,
                                                                                     code = "ZA",
                                                                                     wbcode = "ZAF",
                                                                                     name = "South Africa",
                                                                                     latitude = "-29.05",
                                                                                     longtitude = "25.22"
                                                                                 }
                                                                             },
                                                                         {
                                                                             235,
                                                                             new Country
                                                                                 {
                                                                                     id = 235,
                                                                                     etsyid = 294,
                                                                                     code = "GS",
                                                                                     wbcode = "SGS",
                                                                                     name =
                                                                                         "South Georgia and the South Sandwich Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             91,
                                                                             new Country
                                                                                 {
                                                                                     id = 91,
                                                                                     etsyid = 136,
                                                                                     code = "KR",
                                                                                     wbcode = "KOR",
                                                                                     name = "South Korea",
                                                                                     latitude = "36.45",
                                                                                     longtitude = "127.94"
                                                                                 }
                                                                             },
                                                                         {
                                                                             162,
                                                                             new Country
                                                                                 {
                                                                                     id = 162,
                                                                                     etsyid = 99,
                                                                                     code = "ES",
                                                                                     wbcode = "ESP",
                                                                                     name = "Spain",
                                                                                     latitude = "40.4",
                                                                                     longtitude = "-3.39"
                                                                                 }
                                                                             },
                                                                         {
                                                                             163,
                                                                             new Country
                                                                                 {
                                                                                     id = 163,
                                                                                     etsyid = 142,
                                                                                     code = "LK",
                                                                                     wbcode = "LKA",
                                                                                     name = "Sri Lanka",
                                                                                     latitude = "7.57",
                                                                                     longtitude = "80.88"
                                                                                 }
                                                                             },
                                                                         {
                                                                             164,
                                                                             new Country
                                                                                 {
                                                                                     id = 164,
                                                                                     etsyid = 184,
                                                                                     code = "SD",
                                                                                     wbcode = "SDN",
                                                                                     name = "Sudan",
                                                                                     latitude = "13.77",
                                                                                     longtitude = "30.22"
                                                                                 }
                                                                             },
                                                                         {
                                                                             165,
                                                                             new Country
                                                                                 {
                                                                                     id = 165,
                                                                                     etsyid = 190,
                                                                                     code = "SR",
                                                                                     wbcode = "SUR",
                                                                                     name = "Suriname",
                                                                                     latitude = "4.1",
                                                                                     longtitude = "-55.63"
                                                                                 }
                                                                             },
                                                                         {
                                                                             251,
                                                                             new Country
                                                                                 {
                                                                                     id = 251,
                                                                                     etsyid = 295,
                                                                                     code = "SJ",
                                                                                     wbcode = "SJM",
                                                                                     name = "Svalbard and Jan Mayen",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             166,
                                                                             new Country
                                                                                 {
                                                                                     id = 166,
                                                                                     etsyid = 194,
                                                                                     code = "SZ",
                                                                                     wbcode = "SWZ",
                                                                                     name = "Swaziland",
                                                                                     latitude = "-26.65",
                                                                                     longtitude = "31.7"
                                                                                 }
                                                                             },
                                                                         {
                                                                             167,
                                                                             new Country
                                                                                 {
                                                                                     id = 167,
                                                                                     etsyid = 193,
                                                                                     code = "SE",
                                                                                     wbcode = "SWE",
                                                                                     name = "Sweden",
                                                                                     latitude = "62.75",
                                                                                     longtitude = "16.86"
                                                                                 }
                                                                             },
                                                                         {
                                                                             168,
                                                                             new Country
                                                                                 {
                                                                                     id = 168,
                                                                                     etsyid = 80,
                                                                                     code = "CH",
                                                                                     wbcode = "CHE",
                                                                                     name = "Switzerland",
                                                                                     latitude = "46.84",
                                                                                     longtitude = "8.35"
                                                                                 }
                                                                             },
                                                                         {
                                                                             169,
                                                                             new Country
                                                                                 {
                                                                                     id = 169,
                                                                                     etsyid = 195,
                                                                                     code = "SY",
                                                                                     wbcode = "SYR",
                                                                                     name = "Syria",
                                                                                     latitude = "34.93",
                                                                                     longtitude = "38.68"
                                                                                 }
                                                                             },
                                                                         {
                                                                             196,
                                                                             new Country
                                                                                 {
                                                                                     id = 196,
                                                                                     etsyid = 204,
                                                                                     code = "TW",
                                                                                     wbcode = "TWN",
                                                                                     name = "Taiwan",
                                                                                     latitude = "23.64",
                                                                                     longtitude = "121"
                                                                                 }
                                                                             },
                                                                         {
                                                                             170,
                                                                             new Country
                                                                                 {
                                                                                     id = 170,
                                                                                     etsyid = 199,
                                                                                     code = "TJ",
                                                                                     wbcode = "TJK",
                                                                                     name = "Tajikistan",
                                                                                     latitude = "38.51",
                                                                                     longtitude = "71.28"
                                                                                 }
                                                                             },
                                                                         {
                                                                             171,
                                                                             new Country
                                                                                 {
                                                                                     id = 171,
                                                                                     etsyid = 205,
                                                                                     code = "TZ",
                                                                                     wbcode = "TZA",
                                                                                     name = "Tanzania",
                                                                                     latitude = "-6.4",
                                                                                     longtitude = "35"
                                                                                 }
                                                                             },
                                                                         {
                                                                             172,
                                                                             new Country
                                                                                 {
                                                                                     id = 172,
                                                                                     etsyid = 198,
                                                                                     code = "TH",
                                                                                     wbcode = "THA",
                                                                                     name = "Thailand",
                                                                                     latitude = "15.08",
                                                                                     longtitude = "101.14"
                                                                                 }
                                                                             },
                                                                         {
                                                                             173,
                                                                             new Country
                                                                                 {
                                                                                     id = 173,
                                                                                     etsyid = 296,
                                                                                     code = "TL",
                                                                                     wbcode = "TLS",
                                                                                     name = "Timor-Leste",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             174,
                                                                             new Country
                                                                                 {
                                                                                     id = 174,
                                                                                     etsyid = 197,
                                                                                     code = "TG",
                                                                                     wbcode = "TGO",
                                                                                     name = "Togo",
                                                                                     latitude = "8.55",
                                                                                     longtitude = "1.16"
                                                                                 }
                                                                             },
                                                                         {
                                                                             220,
                                                                             new Country
                                                                                 {
                                                                                     id = 220,
                                                                                     etsyid = 297,
                                                                                     code = "TK",
                                                                                     wbcode = "TKL",
                                                                                     name = "Tokelau",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             175,
                                                                             new Country
                                                                                 {
                                                                                     id = 175,
                                                                                     etsyid = 298,
                                                                                     code = "TO",
                                                                                     wbcode = "TON",
                                                                                     name = "Tonga",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             176,
                                                                             new Country
                                                                                 {
                                                                                     id = 176,
                                                                                     etsyid = 201,
                                                                                     code = "TT",
                                                                                     wbcode = "TTO",
                                                                                     name = "Trinidad",
                                                                                     latitude = "10.42",
                                                                                     longtitude = "-61.29"
                                                                                 }
                                                                             },
                                                                         {
                                                                             177,
                                                                             new Country
                                                                                 {
                                                                                     id = 177,
                                                                                     etsyid = 202,
                                                                                     code = "TN",
                                                                                     wbcode = "TUN",
                                                                                     name = "Tunisia",
                                                                                     latitude = "34.09",
                                                                                     longtitude = "9.74"
                                                                                 }
                                                                             },
                                                                         {
                                                                             178,
                                                                             new Country
                                                                                 {
                                                                                     id = 178,
                                                                                     etsyid = 203,
                                                                                     code = "TR",
                                                                                     wbcode = "TUR",
                                                                                     name = "Turkey",
                                                                                     latitude = "39.02",
                                                                                     longtitude = "35.36"
                                                                                 }
                                                                             },
                                                                         {
                                                                             179,
                                                                             new Country
                                                                                 {
                                                                                     id = 179,
                                                                                     etsyid = 200,
                                                                                     code = "TM",
                                                                                     wbcode = "TKM",
                                                                                     name = "Turkmenistan",
                                                                                     latitude = "39.13",
                                                                                     longtitude = "59.7"
                                                                                 }
                                                                             },
                                                                         {
                                                                             236,
                                                                             new Country
                                                                                 {
                                                                                     id = 236,
                                                                                     etsyid = 299,
                                                                                     code = "TC",
                                                                                     wbcode = "TCA",
                                                                                     name = "Turks and Caicos Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             180,
                                                                             new Country
                                                                                 {
                                                                                     id = 180,
                                                                                     etsyid = 300,
                                                                                     code = "TV",
                                                                                     wbcode = "TUV",
                                                                                     name = "Tuvalu",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             181,
                                                                             new Country
                                                                                 {
                                                                                     id = 181,
                                                                                     etsyid = 206,
                                                                                     code = "UG",
                                                                                     wbcode = "UGA",
                                                                                     name = "Uganda",
                                                                                     latitude = "1.22",
                                                                                     longtitude = "32.52"
                                                                                 }
                                                                             },
                                                                         {
                                                                             182,
                                                                             new Country
                                                                                 {
                                                                                     id = 182,
                                                                                     etsyid = 207,
                                                                                     code = "UA",
                                                                                     wbcode = "UKR",
                                                                                     name = "Ukraine",
                                                                                     latitude = "49.06",
                                                                                     longtitude = "31.56"
                                                                                 }
                                                                             },
                                                                         {
                                                                             183,
                                                                             new Country
                                                                                 {
                                                                                     id = 183,
                                                                                     etsyid = 58,
                                                                                     code = "AE",
                                                                                     wbcode = "ARE",
                                                                                     name = "United Arab Emirates",
                                                                                     latitude = "23.54",
                                                                                     longtitude = "54.3"
                                                                                 }
                                                                             },
                                                                         {
                                                                             184,
                                                                             new Country
                                                                                 {
                                                                                     id = 184,
                                                                                     etsyid = 105,
                                                                                     code = "GB",
                                                                                     wbcode = "GBR",
                                                                                     name = "United Kingdom",
                                                                                     latitude = "53.89",
                                                                                     longtitude = "-2.59"
                                                                                 }
                                                                             },
                                                                         {
                                                                             185,
                                                                             new Country
                                                                                 {
                                                                                     id = 185,
                                                                                     etsyid = 209,
                                                                                     code = "US",
                                                                                     wbcode = "USA",
                                                                                     name = "United States",
                                                                                     latitude = "45.62",
                                                                                     longtitude = "-112.1"
                                                                                 }
                                                                             },
                                                                         {
                                                                             262,
                                                                             new Country
                                                                                 {
                                                                                     id = 262,
                                                                                     etsyid = 302,
                                                                                     code = "UM",
                                                                                     wbcode = "UMI",
                                                                                     name =
                                                                                         "United States Minor Outlying Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             186,
                                                                             new Country
                                                                                 {
                                                                                     id = 186,
                                                                                     etsyid = 208,
                                                                                     code = "UY",
                                                                                     wbcode = "URY",
                                                                                     name = "Uruguay",
                                                                                     latitude = "-32.87",
                                                                                     longtitude = "-55.79"
                                                                                 }
                                                                             },
                                                                         {
                                                                             263,
                                                                             new Country
                                                                                 {
                                                                                     id = 263,
                                                                                     etsyid = 248,
                                                                                     code = "VI",
                                                                                     wbcode = "VIR",
                                                                                     name = "U.S. Virgin Islands",
                                                                                     latitude = "",
                                                                                     longtitude = ""
                                                                                 }
                                                                             },
                                                                         {
                                                                             187,
                                                                             new Country
                                                                                 {
                                                                                     id = 187,
                                                                                     etsyid = 210,
                                                                                     code = "UZ",
                                                                                     wbcode = "UZB",
                                                                                     name = "Uzbekistan",
                                                                                     latitude = "41.77",
                                                                                     longtitude = "63.49"
                                                                                 }
                                                                             },
                                                                         {
                                                                             188,
                                                                             new Country
                                                                                 {
                                                                                     id = 188,
                                                                                     etsyid = 221,
                                                                                     code = "VU",
                                                                                     wbcode = "VUT",
                                                                                     name = "Vanuatu",
                                                                                     latitude = "17.1",
                                                                                     longtitude = "168.7"
                                                                                 }
                                                                             },
                                                                         {
                                                                             190,
                                                                             new Country
                                                                                 {
                                                                                     id = 190,
                                                                                     etsyid = 211,
                                                                                     code = "VE",
                                                                                     wbcode = "VEN",
                                                                                     name = "Venezuela",
                                                                                     latitude = "7.08",
                                                                                     longtitude = "-65.91"
                                                                                 }
                                                                             },
                                                                         {
                                                                             191,
                                                                             new Country
                                                                                 {
                                                                                     id = 191,
                                                                                     etsyid = 212,
                                                                                     code = "VN",
                                                                                     wbcode = "VNM",
                                                                                     name = "Vietnam",
                                                                                     latitude = "16.69",
                                                                                     longtitude = "106.41"
                                                                                 }
                                                                             },
                                                                         {
                                                                             214,
                                                                             new Country
                                                                                 {
                                                                                     id = 214,
                                                                                     etsyid = 224,
                                                                                     code = "WF",
                                                                                     wbcode = "WLF",
                                                                                     name = "Wallis and Futuna",
                                                                                     latitude = "13.2",
                                                                                     longtitude = "176.2"
                                                                                 }
                                                                             },
                                                                         {
                                                                             264,
                                                                             new Country
                                                                                 {
                                                                                     id = 264,
                                                                                     etsyid = 213,
                                                                                     code = "EH",
                                                                                     wbcode = "WSH",
                                                                                     name = "Western Sahara",
                                                                                     latitude = "24.62",
                                                                                     longtitude = "-12.9"
                                                                                 }
                                                                             },
                                                                         {
                                                                             192,
                                                                             new Country
                                                                                 {
                                                                                     id = 192,
                                                                                     etsyid = 214,
                                                                                     code = "YE",
                                                                                     wbcode = "YEM",
                                                                                     name = "Yemen",
                                                                                     latitude = "15.79",
                                                                                     longtitude = "47.82"
                                                                                 }
                                                                             },
                                                                         {
                                                                             265,
                                                                             new Country
                                                                                 {
                                                                                     id = 265,
                                                                                     etsyid = 216,
                                                                                     code = "CD",
                                                                                     wbcode = "COD",
                                                                                     name =
                                                                                         "Zaire (Democratic Republic of Congo)",
                                                                                     latitude = "-2.92",
                                                                                     longtitude = "23.77"
                                                                                 }
                                                                             },
                                                                         {
                                                                             193,
                                                                             new Country
                                                                                 {
                                                                                     id = 193,
                                                                                     etsyid = 217,
                                                                                     code = "ZM",
                                                                                     wbcode = "ZMB",
                                                                                     name = "Zambia",
                                                                                     latitude = "-13.5",
                                                                                     longtitude = "27.95"
                                                                                 }
                                                                             },
                                                                         {
                                                                             194,
                                                                             new Country
                                                                                 {
                                                                                     id = 194,
                                                                                     etsyid = 218,
                                                                                     code = "ZW",
                                                                                     wbcode = "ZWE",
                                                                                     name = "Zimbabwe",
                                                                                     latitude = "-19.06",
                                                                                     longtitude = "30.06"
                                                                                 }
                                                                             },

                                                                     }; 
        #endregion
    }

    public static class CountryHelper
    {
        public static string ToCountryCode(this int countryid)
        {
            var country = Country.GetCountry(countryid);
            if (country == null)
            {
                return "";
            }
            return country.code;
        }

        public static Country ToCountry(this int? countryid)
        {
            if (!countryid.HasValue)
            {
                return new Country();
            }

            var country = Country.GetCountry(countryid.Value);
            if (country == null)
            {
                return new Country();
            }

            return country;
        }

        public static Country ToCountry(this string countrycode)
        {
            return Country.Values.Values.SingleOrDefault(x => x.code == countrycode);
        }
    }


}
