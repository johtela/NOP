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
		public void SimpleTest ()
		{
			var input = LazyList.FromEnumerable ("foo");

			var parseFoo =
				Parser.Char ('f').Bind (x =>
				Parser.Char ('o').Bind (y =>
				Parser.Char ('o').Bind (z => 
				Parser.Return<char[], char> (new char[] { x, y, z }))));

			Check.AreEqual ("foo", new string(parseFoo (input).Item1));
		}
	}
}
