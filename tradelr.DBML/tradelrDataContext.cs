using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace tradelr.DBML
{
    public partial class tradelrDataContext
    {
        [Function(Name = "NEWID", IsComposable = true)]
        public Guid Random()
        { 
            // to prove not used by our C# code... 
            throw new NotImplementedException();
        }

        public void LogChanges<T>(T modifiedEntity) where T : class
        {
            if (modifiedEntity == null)
                return;

            var sb = new StringBuilder();
            foreach (ModifiedMemberInfo modifiedProperty in GetTable<T>().GetModifiedMembers(modifiedEntity))
            {
                //log changes
                // field[original:new] field[original:new]
                sb.AppendFormat("{0}[{1} -> {2}] ", modifiedProperty.Member.Name, modifiedProperty.OriginalValue,
                                modifiedProperty.CurrentValue);
            }
        }
    }
}
