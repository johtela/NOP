using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOP.Testing
{
	public abstract class Property
	{
		public readonly string Name;

		public Property (string name)
		{
			Name = name;
		}

		protected void Failed<T> (T input)
		{
			throw new TestFailed (string.Format ("Property {0} failed for input {1}", Name, input));
		}

		public abstract void Check (int tries, int size);

		private class Func1<T> : Property
		{
			public readonly Func<T, bool> Function;

			public Func1 (string name, Func<T, bool> function) : base(name)
			{
				Function = function;
			}

			public override void Check (int tries, int size)
			{
				var rnd = new Random ();
				for (int i = 0; i < tries; i++)
				{
					var input = Arbitrary.Generate<T>(rnd, size);
					if (!Function (input))
						Failed (input);
				}
			}
		}
	}
}
