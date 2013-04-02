namespace NOP.Testbench
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	public class FingerTreeTests
	{
		const int Count = 1000;
		FingerTree<int> TestTree = FingerTree<int>.FromEnumerable (Enumerable.Range (1, Count));

		[Test]
		public void TestLeftView ()
		{
			var viewl = TestTree.LeftView ();;

			for (int i = 1; i <= Count; i++)
			{
				Check.AreEqual (i, viewl.First);
				viewl = viewl.Rest.LeftView ();	
			}
			Check.IsNull (viewl);
		}

		[Test]
		public void TestRightView ()
		{
			var viewr = TestTree.RightView ();

			for (int i = Count; i > 0; i--)
			{
				Check.AreEqual (i, viewr.Last);
				viewr = viewr.Rest.RightView ();
			}
			Check.IsNull (viewr);
		}

		[Test]
		public void TestEnumeration ()
		{
			var i = 0;

			foreach (var item in TestTree)
			{
				Check.AreEqual (item, ++i);
			}
			Check.AreEqual (Count, i);
		}
	}
}
