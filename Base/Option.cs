﻿namespace NOP
{
	using System;

	public struct Option<T>
	{
		private readonly T _value;
			
		public readonly bool HasValue;

		public T Value
		{
			get
			{
				if (HasValue)
					return _value;
				throw new InvalidOperationException ("Option has no value.");
			}
		}

		public Option(T value)
		{
			HasValue = true;
			_value = value;
		}

		public static implicit operator T (Option<T> option)
		{
			return option.Value;
		}
	}
}
