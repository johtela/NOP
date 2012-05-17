using System;
using System.Collections.Generic;
using NOP.Collections;

namespace NOP.Testbench
{
	public class SetTests
	{
		private const int _itemCount = 10000;

		private IEnumerable<int> Range (int max)
		{
			for (int i = 0; i < _itemCount; i++)
			{
				yield return i;
			}
		}

		private Set<int> CreateTestSet ()
		{
			return Set<int>.Create (Range (_itemCount));
		}

        [Test]
		public void TestTreeStructure ()
		{
			var s = Set<string>.Empty;

			s = s.Add ("0");
			s = s.Add ("1");
			Check.IsTrue (s.Contains ("0"));
			Check.IsTrue (s.Contains ("1"));
			Check.IsFalse (s.Contains ("2"));
		}

        [Test]
		public void TestAddition ()
		{
			var s = CreateTestSet ();

			for (int i = 0; i < _itemCount; i++)
			{
				Check.IsTrue (s.Contains (i));
			}
		}

        [Test]
		public void TestRemoval ()
		{
			var s = CreateTestSet ();

			s = s.Remove (33);
			Check.IsFalse (s.Contains (33));
			Check.AreEqual (_itemCount - 1, s.Count);

			s = s.Remove (77);
			Check.IsFalse (s.Contains (77));
			Check.AreEqual (_itemCount - 2, s.Count);

		}

        [Test]
		public void TestImmutability ()
		{
			var s = CreateTestSet ();

			Check.IsFalse (s.Remove (42).Contains (42));
			Check.IsTrue (s.Contains (42));
			Check.IsFalse (s.Remove (64).Contains (64));
			Check.IsTrue (s.Contains (64));
		}

        [Test]
		public void TestEnumeration ()
		{
			var s = CreateTestSet ();
			int i = 0;

			foreach (var item in s)
			{
				Check.AreEqual (i, item);
				i++;
			}                        
		}

        [Test]
		public void TestCount ()
		{
			var s = CreateTestSet ();

			Check.AreEqual (_itemCount, s.Count);
		}
	}
}
