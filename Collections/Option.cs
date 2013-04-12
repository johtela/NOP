using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOP.Collections
{
	public class Option<T>
	{
		public readonly bool HasValue;
		public readonly T Value;

		public Option()
		{
			HasValue = false;
		}

		public Option(T value)
		{
			HasValue = true;
			Value = value;
		}

	}
}
