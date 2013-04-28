using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOP.Collections;
using NOP.Parsing;

namespace NOP.Testbench
{
	public class ParserMonadTests
	{
		[Test]
		public void BindTest ()
		{
			var input = LazyList.FromEnumerable ("foo");
			var parseFoo = from x in Parser.Char ('f')
						   from y in Parser.Char ('o')
						   from z in Parser.Char ('o')
						   select new string (new char[] { x, y, z });

			var res = parseFoo (input);
			Check.AreEqual ("foo", res.Item1);
			Check.IsTrue (res.Item2.IsEmpty);
		}

		[Test]
		public void ParseWordTest ()
		{
			var input = LazyList.FromEnumerable ("abba");
			var res = Parser.Word () (input);
			Check.AreEqual ("abba", res.Item1);
			Check.IsTrue (res.Item2.IsEmpty);
		}

		[Test]
		public void ParseIntegerTest ()
		{
			var input = LazyList.FromEnumerable ("1000");
			var res = Parser.PositiveInteger () (input);
			Check.AreEqual (1000, res.Item1);
			Check.IsTrue (res.Item2.IsEmpty);
		}
	}
}
