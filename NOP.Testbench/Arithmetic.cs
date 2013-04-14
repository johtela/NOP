namespace NOP.Testbench
{
	using System;
	using NOP;

	public class Arithmetic
	{
		public static readonly double Pi = Math.PI;
		
		public static int Add (int x, int y)
		{
			return x + y;
		}
		
		public static int Base { get; set; }
	}
	
	public class Number
	{
		public readonly int Value;
		
		public Number (int value)
		{
			Value = value;
		}
		
		public int Add (Number other)
		{
			return Value + other.Value;
		}
		
		public object ValueSquared
		{
			get { return Value * Value; }
		}
	}

	public class MutableNumber
	{
		private int _value;
		
		public MutableNumber (int value)
		{
			_value = value;
		}
		
		public int Value
		{
			get { return _value; }
			set { _value = value; }
		}
	}
}