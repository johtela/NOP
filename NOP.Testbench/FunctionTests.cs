using System;
using NOP;

namespace NOP.Testbench
{
	public class FunctionTests
	{
		[Test]
		public void TestCase ()
		{
			Func<int, int, int > add = (i, j) => i + j;
			var add1 = add.Curry () (1);
			Check.AreEqual (3, add1 (2));
		}
	}
}

