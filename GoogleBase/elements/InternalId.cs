using Google.GData.Extensions;

namespace GoogleBase.elements
{
    public class InternalId : SimpleElement
    {
        /// <summary>
        /// default constructor for sc:id 
        /// </summary>
        public InternalId()
            : base(AccountForShoppingNameTable.InternalId,
               AccountForShoppingNameTable.scDataPrefix,
               AccountForShoppingNameTable.BaseNamespace)
        {
        }
    }
}
