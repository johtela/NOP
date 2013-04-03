namespace NOP.Testbench
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using NOP.Collections;

	public class FingerTreeTests
	{
		const int Count = 10000;
		FingerTree<Elem<int>, TreeSize> TestTree =
			FingerTree<Elem<int>, TreeSize>.FromEnumerable (
				Enumerable.Range (1, Count).Select (Elem.Create));
		FingerTree<Elem<int>, TreeSize> OtherTree =
			FingerTree<Elem<int>, TreeSize>.FromEnumerable (
				Enumerable.Range (Count + 1, Count).Select (Elem.Create));

		[Test]
		public void TestLeftView ()
		{
			var viewl = TestTree.LeftView ();

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

			foreach (var item in List.FromReducible(TestTree))
			{
				Check.AreEqual (item, ++i);
			}
			Check.AreEqual (Count, i);
		}

		[Test]
		public void TestForeach ()
		{
			TestTree.Foreach (1, (e, i) => Check.AreEqual(e, i));
		}

		[Test]
		public void TestAppend ()
		{
			TestTree.Append (OtherTree).Foreach (1, (e, i) => Check.AreEqual (e, i));
		}
	}
}
