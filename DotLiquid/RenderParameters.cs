using System;
using System.Collections.Generic;

namespace DotLiquid
{
	public class RenderParameters
	{
		/// <summary>
		/// If you provide a Context object, you do not need to set any other parameters.
		/// </summary>
		public Context Context { get; set; }

		public Hash LocalVariables { get; set; }
		public IEnumerable<Type> Filters { get; set; }
		public Hash Registers { get; set; }
        public Hash Values { get; set; }

		/// <summary>
		/// Gets or sets a value that controls whether errors are thrown as exceptions.
		/// </summary>
		public bool RethrowErrors { get; set; }

		internal void Evaluate(Template template, out Context context, out Hash registers, out IEnumerable<Type> filters, out Hash values)
		{
			if (Context != null)
			{
				context = Context;
				registers = null;
				filters = null;
			    values = null;
				return;
			}

			List<Hash> environments = new List<Hash>();
			if (LocalVariables != null)
				environments.Add(LocalVariables);
			environments.Add(template.Assigns);
			context = new Context(environments, template.InstanceAssigns, template.Registers, template.InstanceValues, RethrowErrors);
			registers = Registers;
			filters = Filters;
		    values = Values;
		}

		public static RenderParameters FromContext(Context context)
		{
			return new RenderParameters { Context = context };
		}
	}
}