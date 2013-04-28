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
			var parseFoo = from x in StringParser.Char ('f')
						   from y in StringParser.Char ('o')
						   from z in StringParser.Char ('o')
						   select new string (new char[] { x, y, z });

			var res = parseFoo (input);
			Check.AreEqual ("foo", res.Item1);
			Check.IsTrue (res.Item2.IsEmpty);
		}

		[Test]
		public void ParseWordTest ()
		{
			var input = LazyList.FromEnumerable ("abba");
			var res = StringParser.Word () (input);
			Check.AreEqual ("abba", res.Item1);
			Check.IsTrue (res.Item2.IsEmpty);
		}

		[Test]
		public void ParseIntegerTest ()
		{
			var input = LazyList.FromEnumerable ("1000");
			var res = StringParser.PositiveInteger () (input);
			Check.AreEqual (1000, res.Item1);
			Check.IsTrue (res.Item2.IsEmpty);
		}
	}
}
