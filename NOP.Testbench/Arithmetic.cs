using System;

namespace NOP.Testbench
{
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
}

