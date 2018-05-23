using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Globalization;

[assembly: CLSCompliant(true)]
namespace Zayko.Finance
{
    /// <summary>
    /// Static Class contains currency codes and names
    /// </summary>
    public struct CurrencyData
    {
        #region Fields

        private string _baseCode;
        private string _targetCode;
        private DateTime _tradeDate;
        private double _rate;
        private double _min;
        private double _max;

        #endregion

        /// <summary>
        /// Object Constructor
        /// </summary>
        /// <param name="baseCode">Three-chars Currency code</param>
        /// <param name="targetCode">Three-chars Currency code</param>
        public CurrencyData(string baseCode, string targetCode)
        {
            if(String.IsNullOrEmpty(baseCode))
                throw new ArgumentNullException("baseCode");

            if(String.IsNullOrEmpty(targetCode))
                throw new ArgumentNullException("targetCode");

            _baseCode = baseCode;
            _targetCode = targetCode;
            _tradeDate = DateTime.Now;
            _rate = 0;
            _min = 0;
            _max = 0;
        }
        

        #region Properties

        /// <summary>
        /// Last Trade Date/Time
        /// </summary>
        public DateTime TradeDate
        {
            get { return _tradeDate; }
            set { _tradeDate = value; }
        }

        /// <summary>
        /// Current Exchange rate
        /// </summary>
        public double Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        /// <summary>
        /// Minimal Bid price
        /// </summary>
        public double Min
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// Maximal Ask price
        /// </summary>
        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// Three-chars Currency code to convert from
        /// </summary>
        public string BaseCode
        {
            get { return _baseCode; }
            set 
            {
                if(String.IsNullOrEmpty(value))
                    throw new ArgumentException(BaseCode);
                _baseCode = value.Trim().ToUpper(CultureInfo.InvariantCulture); 
            }
        }

        /// <summary>
        /// Three-chars Currency code to conver to
        /// </summary>
        public string TargetCode
        {
            get { return _targetCode; }
            set 
            {
                if(String.IsNullOrEmpty(value))
                    throw new ArgumentException(TargetCode);
                _targetCode = value.Trim().ToUpper(CultureInfo.InvariantCulture); 
            }
        }

        #endregion

        /// <summary>
        /// Compare two CurrencyData objects. Static method.
        /// </summary>
        /// <param name="leftCurrencyData">Left-side CurrencyData object to compare</param>
        /// <param name="rightCurrencyData">Right-side CurrencyData object to compare</param>
        /// <returns>true if these objects ARE equal, false if otherwise</returns>
        public static bool operator ==(CurrencyData leftCurrencyData, CurrencyData rightCurrencyData)
        {
            return ((leftCurrencyData.BaseCode.Equals(rightCurrencyData.BaseCode)) && 
                (leftCurrencyData.TargetCode.Equals(rightCurrencyData.TargetCode)));
        }

        /// <summary>
        /// Compare two CurrencyData objects. Static method.
        /// </summary>
        /// <param name="leftCurrencyData">Left-side CurrencyData object to compare</param>
        /// <param name="rightCurrencyData">Right-side CurrencyData object to compare</param>
        /// <returns>true if these objects are NOT equal, false if otherwise</returns>
        public static bool operator !=(CurrencyData leftCurrencyData, CurrencyData rightCurrencyData)
        {
            return ((!leftCurrencyData.BaseCode.Equals(rightCurrencyData.BaseCode)) || 
                (!leftCurrencyData.TargetCode.Equals(rightCurrencyData.TargetCode)));
        }

        /// <summary>
        /// Compare two CurrencyData object for equality
        /// </summary>
        /// <param name="obj">An Object to compare with the current</param>
        /// <returns>true if equal; false if otherwise</returns>
        public override bool Equals(object obj)
        {
            if(obj == null)
                return false;

            if(obj.GetType() != this.GetType())
                return false;

            CurrencyData tmp = (CurrencyData)obj;
            if((!tmp.BaseCode.Equals(this.BaseCode)) || (!tmp.TargetCode.Equals(this.TargetCode)))
                return false;

            return true;
        }

        /// <summary>
        /// Return Hash code for the object
        /// </summary>
        /// <returns>Object's Hash code</returns>
        public override int GetHashCode()
        {
            return (CurrencyList.GetCodeIndex(this.BaseCode) ^ CurrencyList.GetCodeIndex(this.TargetCode));
        }

        /// <summary>
        /// Gets string representation of the object
        /// </summary>
        /// <returns>string contains Base code, Target code, current Price, Trade date and Bid and Ask values</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "From: {0}; To: {1}; Price: {2}; Trade date: {3}; Min: {4}; Max: {5}", 
                this.BaseCode, this.TargetCode, this.Rate, this.TradeDate, this.Min, this.Max);
        }
    }

    /// <summary>
    /// Connect to Web service and get currency exchange rates
    /// </summary>
    public sealed class CurrencyConverter
    {
        private const string urltemplate = "http://finance.yahoo.com/d/quotes.csv?{0}&f=sl1d1t1ba&e=.csv";  // put list of currencies like this: "s=CUR1CUR2=X&s=CUR3CUR4=X&..."
        private const string paretemplate = "s={0}{1}=X";
        private const string NA = "N/A";
        private WebProxy _proxy;
        private bool _ajustTime;
        private int _timeout = 30000;
        private int _readWriteTimeout = 30000;

        /// <summary>
        /// Enums data fields
        /// </summary>
        private enum DataFieldNames : byte
        {
            CurrencyCodes = 0,
            CurrentRate,
            TradeDate,
            TradeTime,
            MinPrice,
            MaxPrice
        }

        /// <summary>
        /// Set to true if you want to convert Trade Date/Time to your local value
        /// </summary>
        public bool AdjustToLocalTime
        {
            get { return _ajustTime; }
            set { _ajustTime = value; }
        }

        /// <summary>
        /// Gets or Sets Web Proxy settings
        /// </summary>
        public WebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        /// <summary>
        /// Gets or sets the time-out value for the GetResponse and GetRequestStream methods. Default - 30 sec.
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Gets or sets a time-out when writing to or reading from a stream. Default - 30 sec.
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _readWriteTimeout; }
            set { _readWriteTimeout = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CurrencyConverter()
        {
            _proxy = new WebProxy();
        }

        /// <summary>
        /// Connect to Finance!Yahoo site and gets the info
        /// </summary>
        /// <param name="url">Finance!Yahoo URL with actual currencies as params</param>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when the service is unavailable</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when supplied currencies are not supported</exception>
        /// <returns>List of CurrencyData objects</returns>
        private List<CurrencyData> GetData(Uri url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.Proxy = _proxy;
            req.Timeout = _timeout;
            req.ReadWriteTimeout = _readWriteTimeout;

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            try
            {
                Stream respStream = resp.GetResponseStream();
                try
                {
                    StreamReader respReader = new StreamReader(respStream, Encoding.ASCII);
                    try
                    {
                        char[] trimchars = new char[] { '"', '\'' };

                        List<CurrencyData> Result = new List<CurrencyData>();

                        while(respReader.Peek() > 0)
                        {
                            string[] fields = respReader.ReadLine().Split(new char[] { ',' });

                            if((fields.Length == 0) || (fields[(int)DataFieldNames.TradeDate].Trim(trimchars).Equals(NA, StringComparison.InvariantCultureIgnoreCase)))
                                throw new UnauthorizedAccessException("The currency data is not available!");

                            if(fields[(int)DataFieldNames.CurrencyCodes].Trim(trimchars).Equals(NA, StringComparison.InvariantCultureIgnoreCase))
                                throw new ArgumentOutOfRangeException("These Currencies are not supported!");

                            string from = fields[(int)DataFieldNames.CurrencyCodes].Trim(trimchars).Substring(0, 3);
                            string to = fields[(int)DataFieldNames.CurrencyCodes].Trim(trimchars).Substring(3, 3);

                            CurrencyData data = new CurrencyData(from, to);

                            CultureInfo culture = new CultureInfo("en-US", false);
                            if(fields[(int)DataFieldNames.CurrentRate].Equals(NA, StringComparison.InvariantCultureIgnoreCase))
                                data.Rate = 0;
                            else
                                data.Rate = Double.Parse(fields[(int)DataFieldNames.CurrentRate], culture);

                            string datetime = String.Format(CultureInfo.InvariantCulture,
                                "{0} {1}", fields[(int)DataFieldNames.TradeDate].Trim(trimchars),
                                fields[(int)DataFieldNames.TradeTime].Trim(trimchars));
                            data.TradeDate = DateTime.Parse(datetime, culture);

                            if(AdjustToLocalTime)
                            {
                                DateTime utcDateTime = DateTime.SpecifyKind(data.TradeDate.AddHours(5), DateTimeKind.Utc);
                                data.TradeDate = utcDateTime.ToLocalTime();
                                if(data.TradeDate.IsDaylightSavingTime())
                                {
                                    TimeSpan ts = new TimeSpan(1, 0, 0);
                                    data.TradeDate = data.TradeDate.Subtract(ts);
                                }
                            }

                            try
                            {
                                if(fields[(int)DataFieldNames.MinPrice].Equals(NA, StringComparison.InvariantCultureIgnoreCase))
                                    data.Min = 0;
                                else
                                    data.Min = Double.Parse(fields[(int)DataFieldNames.MinPrice], culture);
                            }
                            catch(Exception)
                            {
                                data.Min = 0;
                                throw;
                            }

                            try
                            {
                                if(fields[(int)DataFieldNames.MaxPrice].Equals(NA, StringComparison.InvariantCultureIgnoreCase))
                                    data.Max = 0;
                                else
                                    data.Max = Double.Parse(fields[(int)DataFieldNames.MaxPrice], culture);
                            }
                            catch(Exception)
                            {
                                data.Max = 0;
                                throw;
                            }

                            Result.Add(data);
                        }

                        return Result;
                    }
                    finally
                    {
                        respReader.Close();
                    }
                }
                finally
                {
                    respStream.Close();
                }
            }
            finally
            {
                resp.Close();
            }
        }

        /// <summary>
        /// Checks CurrencyData
        /// </summary>
        /// <param name="data">CurrencyData object to check</param>
        /// <exception cref="System.ArgumentNullException">Thrown when either BaseCode or TargetCode is empty, or BaseCode is equal to TargetCode</exception>
        private void CheckParams(CurrencyData data)
        {
             if(data.BaseCode.Length == 0)
                throw new ArgumentNullException("data.BaseCode", "Base currency code is not specified!");

            if(data.TargetCode.Length == 0)
                throw new ArgumentNullException("data.TargetCode", "Target currency code is not specified!");

            if(data.BaseCode.Equals(data.TargetCode))
                throw new ArgumentException("data.BaseCode, data.TargetCode", "Base currency code is equal to Target code!");
        }

        /// <summary>
        /// Receive Currency data form multi currencies stored in ICollection
        /// </summary>
        /// <param name="listData">List of Currencies to get</param>
        /// <seealso cref="System.Collections.Generic.IList&lt;T&gt;"/>
        /// <example>
        /// This example shows how to call GetCurrencyData with IList&lt;CurrencyData&gt; param
        /// <code>
        /// CurrencyConverter cc = new CurrencyConverter();
        /// if(useProxy)
        ///     cc.Proxy = new System.Net.WebProxy(proxyAddress, proxyPort);
        /// IList&lt;CurrencyData&gt; list = new List&lt;CurrencyData&gt;(listViewRate.Items.Count);
        /// list.Add(new CurrencyData("USD", "RUB"));
        /// cc.GetCurrencyData(ref list);
        /// </code>
        /// </example>
        public void GetCurrencyData(ref IList<CurrencyData> listData)
        {
            // Create the URL:
            StringBuilder urlpart = new StringBuilder();

            foreach(CurrencyData cd in listData)
            {
                CheckParams(cd);

                if(urlpart.Length > 0)
                    urlpart.Append('&');

                urlpart.AppendFormat(CultureInfo.InvariantCulture, paretemplate, cd.BaseCode,
                    cd.TargetCode);
            }

            string url = String.Format(CultureInfo.InvariantCulture, urltemplate, urlpart.ToString());
            Uri uri = new Uri(url);

            listData.Clear();
            listData = GetData(uri);
        }

        /// <summary>
        /// Receive current Currency data by specified CurrencyData param
        /// </summary>
        /// <param name="data">Reference to a CurrencyData class containing the Currency codes</param>
        /// <seealso cref="Zayko.Finance.CurrencyData"/>
        public void GetCurrencyData(ref CurrencyData data)
        {
            CheckParams(data);

            // Create the URL:
            StringBuilder urlpart = new StringBuilder();

            urlpart.AppendFormat(CultureInfo.InvariantCulture, paretemplate, data.BaseCode,
                    data.TargetCode);

            List<CurrencyData> listData = GetData(new Uri(String.Format(CultureInfo.InvariantCulture, urltemplate, urlpart.ToString())));

            if((listData != null) && (listData.Count > 0))
                data = listData[0];
        }

        /// <summary>
        /// Return CurrencyData by suplied Currency codes
        /// </summary>
        /// <param name="source">Three-chars Currency code</param>
        /// <param name="target">Three-chars Currency code</param>
        /// <returns>CurrencyData class contains exchange rate information</returns>
        public CurrencyData GetCurrencyData(string source, string target)
        {
            CurrencyData data = new CurrencyData(source, target);

            GetCurrencyData(ref data);

            return data;
        }
    }
}
