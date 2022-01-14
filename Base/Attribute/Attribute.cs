using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class BaseAttribute : Attribute
	{
		public Type AttributeType { get; }

		public BaseAttribute()
		{
			this.AttributeType = this.GetType();
		}
	}
}
