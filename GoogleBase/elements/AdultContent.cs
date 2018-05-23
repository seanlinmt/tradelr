using Google.GData.Extensions;

namespace GoogleBase.elements
{
    public class AdultContent : SimpleElement
    {
        /// <summary>
        /// default constructor for sc:adult 
        /// </summary>
        public AdultContent()
            : base(AccountForShoppingNameTable.AdultContent,
               AccountForShoppingNameTable.scDataPrefix,
               AccountForShoppingNameTable.BaseNamespace)
        {
        }
    }
}
