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
		Sequence<int> TestSeq = Sequence<int>.Create (Enumerable.Range (0, Count));
		Sequence<int> OtherSeq = Sequence<int>.Create (Enumerable.Range (Count, Count));
		

		[Test]
		public void TestLeftView ()
		{
			var viewl = TestSeq.LeftView;

			for (int i = 0; i < Count; i++)
			{
				Check.AreEqual (i, viewl.Item1);
				viewl = viewl.Item2.LeftView;	
			}
			Check.IsNull (viewl);
		}

		[Test]
		public void TestRightView ()
		{
			var viewr = TestSeq.RightView;

			for (int i = Count - 1; i >= 0; i--)
			{
				Check.AreEqual (i, viewr.Item2);
				viewr = viewr.Item1.RightView;
			}
			Check.IsNull (viewr);
		}

		[Test]
		public void TestSequenceReduction ()
		{
			TestSeq.Foreach (0, Check.AreEqual);
		}

		[Test]
		public void TestAsArray ()
		{
			TestEnumeration (TestSeq.AsArray ());
		}

		[Test]
		public void TestEnumeration ()
		{
			TestEnumeration (TestSeq);
		}

		private void TestEnumeration (IEnumerable<int> e)
		{
			var i = 0;
			foreach (var item in e)
				Check.AreEqual (item, i++);
			Check.AreEqual (Count, i);
		}

		[Test]
		public void TestAppend ()
		{
			TestSeq.AppendWith (NOPList<int>.Empty, OtherSeq).Foreach (0, Check.AreEqual);
		}

		[Test]
		public void TestSequenceIndexing ()
		{
			for (int i = 0; i < Count; i++)	
			{
				Check.AreEqual (i, TestSeq[i]);
			}
		}

		[Test]
		public void TestReductionFromList ()
		{
			TestEnumeration (List.FromReducible (TestSeq));
		}

	}
}
