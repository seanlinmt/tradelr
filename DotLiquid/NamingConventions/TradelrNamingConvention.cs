using System;

namespace DotLiquid.NamingConventions
{
    public class TradelrNamingConvention : INamingConvention
	{
		public StringComparer StringComparer
		{
			get { return StringComparer.OrdinalIgnoreCase; }
		}

		public string GetMemberName(string name)
		{
			return name.ToLower();
		}
	}
}