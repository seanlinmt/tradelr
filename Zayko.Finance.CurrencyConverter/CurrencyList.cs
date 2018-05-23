using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zayko.Finance
{

    /// <summary>
    /// Base class contains list of supported currencies
    /// </summary>
    public sealed class CurrencyList
    {
        // Number of supported currencies:
        private static int initialCount = 159;

        private static ReadOnlyCollection<string> _currencyCodes = 
            new ReadOnlyCollection<string>(InitializeCurrencyCodes());

        private static readonly ReadOnlyCollection<string> _currencyDescriptions = 
            new ReadOnlyCollection<string>(InitializeCurrencyDescriptions());

        /// <summary>
        /// Return number of supported Currencies
        /// </summary>
        public static int Count
        {
            get { return _currencyCodes.Count; }
        }

        #region Methods

        #region Initializing Currency Codes
        private static IList<string> InitializeCurrencyCodes()
        {
            List<string> codes = new List<string>(initialCount);

            codes.Add("ALL");
            codes.Add("DZD");
            codes.Add("XAL");
            codes.Add("ARS");
            codes.Add("AWG");
            codes.Add("AUD");
            codes.Add("BSD");
            codes.Add("BHD");
            codes.Add("BDT");
            codes.Add("BBD");
            codes.Add("BYR");
            codes.Add("BZD");
            codes.Add("BMD");
            codes.Add("BTN");
            codes.Add("BOB");
            codes.Add("BWP");
            codes.Add("BRL");
            codes.Add("GBP");
            codes.Add("BND");
            codes.Add("BGN");
            codes.Add("BIF");
            codes.Add("KHR");
            codes.Add("CAD");
            codes.Add("CVE");
            codes.Add("KYD");
            codes.Add("XOF");
            codes.Add("XAF");
            codes.Add("CLP");
            codes.Add("CNY");
            codes.Add("COP");
            codes.Add("KMF");
            codes.Add("XCP");
            codes.Add("CRC");
            codes.Add("HRK");
            codes.Add("CUP");
            codes.Add("CYP");
            codes.Add("CZK");
            codes.Add("DKK");
            codes.Add("DJF");
            codes.Add("DOP");
            codes.Add("XCD");
            codes.Add("ECS");
            codes.Add("EGP");
            codes.Add("SVC");
            codes.Add("ERN");
            codes.Add("EEK");
            codes.Add("ETB");
            codes.Add("EUR");
            codes.Add("FKP");
            codes.Add("FJD");
            codes.Add("GMD");
            codes.Add("GHC");
            codes.Add("GIP");
            codes.Add("XAU");
            codes.Add("GTQ");
            codes.Add("GNF");
            codes.Add("GYD");
            codes.Add("HTG");
            codes.Add("HNL");
            codes.Add("HKD");
            codes.Add("HUF");
            codes.Add("ISK");
            codes.Add("INR");
            codes.Add("IDR");
            codes.Add("IRR");
            codes.Add("IQD");
            codes.Add("ILS");
            codes.Add("JMD");
            codes.Add("JPY");
            codes.Add("JOD");
            codes.Add("KZT");
            codes.Add("KES");
            codes.Add("KRW");
            codes.Add("KWD");
            codes.Add("LAK");
            codes.Add("LVL");
            codes.Add("LBP");
            codes.Add("LSL");
            codes.Add("LRD");
            codes.Add("LYD");
            codes.Add("LTL");
            codes.Add("MOP");
            codes.Add("MKD");
            codes.Add("MGF");
            codes.Add("MWK");
            codes.Add("MYR");
            codes.Add("MVR");
            codes.Add("MTL");
            codes.Add("MRO");
            codes.Add("MUR");
            codes.Add("MXN");
            codes.Add("MDL");
            codes.Add("MNT");
            codes.Add("MAD");
            codes.Add("MZM");
            codes.Add("MMK");
            codes.Add("NAD");
            codes.Add("NPR");
            codes.Add("ANG");
            codes.Add("TRY");
            codes.Add("NZD");
            codes.Add("ZWN");
            codes.Add("NIO");
            codes.Add("NGN");
            codes.Add("KPW");
            codes.Add("NOK");
            codes.Add("OMR");
            codes.Add("XPF");
            codes.Add("PKR");
            codes.Add("XPD");
            codes.Add("PAB");
            codes.Add("PGK");
            codes.Add("PYG");
            codes.Add("PEN");
            codes.Add("PHP");
            codes.Add("XPT");
            codes.Add("PLN");
            codes.Add("QAR");
            codes.Add("ROL");
            codes.Add("RON");
            codes.Add("RUB");
            codes.Add("RWF");
            codes.Add("WST");
            codes.Add("STD");
            codes.Add("SAR");
            codes.Add("SCR");
            codes.Add("SLL");
            codes.Add("XAG");
            codes.Add("SGD");
            codes.Add("SKK");
            codes.Add("SIT");
            codes.Add("SBD");
            codes.Add("SOS");
            codes.Add("ZAR");
            codes.Add("LKR");
            codes.Add("SHP");
            codes.Add("SDD");
            codes.Add("SRG");
            codes.Add("SZL");
            codes.Add("SEK");
            codes.Add("CHF");
            codes.Add("SYP");
            codes.Add("TWD");
            codes.Add("TZS");
            codes.Add("THB");
            codes.Add("TOP");
            codes.Add("TTD");
            codes.Add("TND");
            codes.Add("USD");
            codes.Add("AED");
            codes.Add("UGX");
            codes.Add("UAH");
            codes.Add("UYU");
            codes.Add("VUV");
            codes.Add("VEB");
            codes.Add("VND");
            codes.Add("YER");
            codes.Add("ZMK");
            codes.Add("ZWD");

            return codes;
        }
        #endregion

        #region Initializing Currency Descriptions
        private static IList<string> InitializeCurrencyDescriptions()
        {
            List<string> descriptions = new List<string>(initialCount);

            descriptions.Add("Albanian Lek");
            descriptions.Add("Algerian Dinar");
            descriptions.Add("Aluminium Ounces");
            descriptions.Add("Argentine Peso");
            descriptions.Add("Aruba Florin");
            descriptions.Add("Australian Dollar");
            descriptions.Add("Bahamian Dollar");
            descriptions.Add("Bahraini Dinar");
            descriptions.Add("Bangladesh Taka");
            descriptions.Add("Barbados Dollar");
            descriptions.Add("Belarus Ruble");
            descriptions.Add("Belize Dollar");
            descriptions.Add("Bermuda Dollar");
            descriptions.Add("Bhutan Ngultrum");
            descriptions.Add("Bolivian Boliviano");
            descriptions.Add("Botswana Pula");
            descriptions.Add("Brazilian Real");
            descriptions.Add("British Pound");
            descriptions.Add("Brunei Dollar");
            descriptions.Add("Bulgarian Lev");
            descriptions.Add("Burundi Franc");
            descriptions.Add("Cambodia Riel");
            descriptions.Add("Canadian Dollar");
            descriptions.Add("Cape Verde Escudo");
            descriptions.Add("Cayman Islands Dollar");
            descriptions.Add("CFA Franc");
            descriptions.Add("CFA Franc");
            descriptions.Add("Chilean Peso");
            descriptions.Add("Chinese Yuan");
            descriptions.Add("Colombian Peso");
            descriptions.Add("Comoros Franc");
            descriptions.Add("Copper Ounces");
            descriptions.Add("Costa Rica Colon");
            descriptions.Add("Croatian Kuna");
            descriptions.Add("Cuban Peso");
            descriptions.Add("Cyprus Pound");
            descriptions.Add("Czech Koruna");
            descriptions.Add("Danish Krone");
            descriptions.Add("Dijibouti Franc");
            descriptions.Add("Dominican Peso");
            descriptions.Add("East Caribbean Dollar");
            descriptions.Add("Ecuador Sucre");
            descriptions.Add("Egyptian Pound");
            descriptions.Add("El Salvador Colon");
            descriptions.Add("Eritrea Nakfa");
            descriptions.Add("Estonian Kroon");
            descriptions.Add("Ethiopian Birr");
            descriptions.Add("Euro");
            descriptions.Add("Falkland Islands Pound");
            descriptions.Add("Fiji Dollar");
            descriptions.Add("Gambian Dalasi");
            descriptions.Add("Ghanian Cedi");
            descriptions.Add("Gibraltar Pound");
            descriptions.Add("Gold Ounces");
            descriptions.Add("Guatemala Quetzal");
            descriptions.Add("Guinea Franc");
            descriptions.Add("Guyana Dollar");
            descriptions.Add("Haiti Gourde");
            descriptions.Add("Honduras Lempira");
            descriptions.Add("Hong Kong Dollar");
            descriptions.Add("Hungarian Forint");
            descriptions.Add("Iceland Krona");
            descriptions.Add("Indian Rupee");
            descriptions.Add("Indonesian Rupiah");
            descriptions.Add("Iran Rial");
            descriptions.Add("Iraqi Dinar");
            descriptions.Add("Israeli Shekel");
            descriptions.Add("Jamaican Dollar");
            descriptions.Add("Japanese Yen");
            descriptions.Add("Jordanian Dinar");
            descriptions.Add("Kazakhstan Tenge");
            descriptions.Add("Kenyan Shilling");
            descriptions.Add("Korean Won");
            descriptions.Add("Kuwaiti Dinar");
            descriptions.Add("Lao Kip");
            descriptions.Add("Latvian Lat");
            descriptions.Add("Lebanese Pound");
            descriptions.Add("Lesotho Loti");
            descriptions.Add("Liberian Dollar");
            descriptions.Add("Libyan Dinar");
            descriptions.Add("Lithuanian Lita");
            descriptions.Add("Macau Pataca");
            descriptions.Add("Macedonian Denar");
            descriptions.Add("Malagasy Franc");
            descriptions.Add("Malawi Kwacha");
            descriptions.Add("Malaysian Ringgit");
            descriptions.Add("Maldives Rufiyaa");
            descriptions.Add("Maltese Lira");
            descriptions.Add("Mauritania Ougulya");
            descriptions.Add("Mauritius Rupee");
            descriptions.Add("Mexican Peso");
            descriptions.Add("Moldovan Leu");
            descriptions.Add("Mongolian Tugrik");
            descriptions.Add("Moroccan Dirham");
            descriptions.Add("Mozambique Metical");
            descriptions.Add("Myanmar Kyat");
            descriptions.Add("Namibian Dollar");
            descriptions.Add("Nepalese Rupee");
            descriptions.Add("Neth Antilles Guilder");
            descriptions.Add("New Turkish Lira");
            descriptions.Add("New Zealand Dollar");
            descriptions.Add("New Zimbabwe Dollar");
            descriptions.Add("Nicaragua Cordoba");
            descriptions.Add("Nigerian Naira");
            descriptions.Add("North Korean Won");
            descriptions.Add("Norwegian Krone");
            descriptions.Add("Omani Rial");
            descriptions.Add("Pacific Franc");
            descriptions.Add("Pakistani Rupee");
            descriptions.Add("Palladium Ounces");
            descriptions.Add("Panama Balboa");
            descriptions.Add("Papua New Guinea Kina");
            descriptions.Add("Paraguayan Guarani");
            descriptions.Add("Peruvian Nuevo Sol");
            descriptions.Add("Philippine Peso");
            descriptions.Add("Platinum Ounces");
            descriptions.Add("Polish Zloty");
            descriptions.Add("Qatar Rial");
            descriptions.Add("Romanian Leu");
            descriptions.Add("Romanian New Leu");
            descriptions.Add("Russian Rouble");
            descriptions.Add("Rwanda Franc");
            descriptions.Add("Samoa Tala");
            descriptions.Add("Sao Tome Dobra");
            descriptions.Add("Saudi Arabian Riyal");
            descriptions.Add("Seychelles Rupee");
            descriptions.Add("Sierra Leone Leone");
            descriptions.Add("Silver Ounces");
            descriptions.Add("Singapore Dollar");
            descriptions.Add("Slovak Koruna");
            descriptions.Add("Slovenian Tolar");
            descriptions.Add("Solomon Islands Dollar");
            descriptions.Add("Somali Shilling");
            descriptions.Add("South African Rand");
            descriptions.Add("Sri Lanka Rupee");
            descriptions.Add("St Helena Pound");
            descriptions.Add("Sudanese Dinar");
            descriptions.Add("Surinam Guilder");
            descriptions.Add("Swaziland Lilageni");
            descriptions.Add("Swedish Krona");
            descriptions.Add("Swiss Franc");
            descriptions.Add("Syrian Pound");
            descriptions.Add("Taiwan Dollar");
            descriptions.Add("Tanzanian Shilling");
            descriptions.Add("Thai Baht");
            descriptions.Add("Tonga Paanga");
            descriptions.Add("Trinidad and Tobago Dollar");
            descriptions.Add("Tunisian Dinar");
            descriptions.Add("U.S. Dollar");
            descriptions.Add("UAE Dirham");
            descriptions.Add("Ugandan Shilling");
            descriptions.Add("Ukraine Hryvnia");
            descriptions.Add("Uruguayan New Peso");
            descriptions.Add("Vanuatu Vatu");
            descriptions.Add("Venezuelan Bolivar");
            descriptions.Add("Vietnam Dong");
            descriptions.Add("Yemen Riyal");
            descriptions.Add("Zambian Kwacha");
            descriptions.Add("Zimbabwe Dollar");

            return descriptions;
        }
        #endregion

        /// <summary>
        /// Returns list of supported Currency Codes
        /// </summary>
        public static ReadOnlyCollection<string> Codes
        {
            get 
            { 
                return _currencyCodes; 
            }
        }

        /// <summary>
        /// Returns list of supported Currency Names
        /// </summary>
        public static ReadOnlyCollection<string> Descriptions
        {
            get 
            { 
                return _currencyDescriptions; 
            }
        }

        /// <summary>
        /// Returns Currency code by its number
        /// </summary>
        /// <param name="index">Number to get</param>
        /// <returns>Tree-chars currency code</returns>
        public static string GetCode(int index)
        {
            return _currencyCodes[index];
        }

        /// <summary>
        /// Returns currency Code by its Name
        /// </summary>
        /// <param name="description">A Currency Name</param>
        /// <returns>Tree-chars currency code</returns>
        public static string GetCode(string description)
        {
            return _currencyCodes[_currencyDescriptions.IndexOf(description)];
        }

        /// <summary>
        /// Returns Currency index by its Code
        /// </summary>
        /// <param name="code">Tree-chars currency code</param>
        /// <returns>Position in the list</returns>
        public static int GetCodeIndex(string code)
        {
            return _currencyCodes.IndexOf(code);
        }

        /// <summary>
        /// Returns Currency Name by its position in the list
        /// </summary>
        /// <param name="index">Position in the list</param>
        /// <returns>A Currency Name</returns>
        public static string GetDescription(int index)
        {
            return _currencyDescriptions[index];
        }

        /// <summary>
        /// Returns Currency Name by its code
        /// </summary>
        /// <param name="code">Three-chars Currency Code</param>
        /// <returns>A Currency Name</returns>
        public static string GetDescription(string code)
        {
            return _currencyDescriptions[_currencyCodes.IndexOf(code)];
        }

        /// <summary>
        /// Returns Currency position in the list by its Name
        /// </summary>
        /// <param name="description">A Currency Name</param>
        /// <returns>Currency position</returns>
        public static int GetDescriptionIndex(string description)
        {
            return _currencyDescriptions.IndexOf(description);
        }

        #endregion
    }
}
