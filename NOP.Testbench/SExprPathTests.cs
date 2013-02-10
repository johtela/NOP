namespace NOP.Testbench
{
	using NOP.Collections;

	class SExprPathTests : ExprUser
	{
		[Test]
		public void TestEmptyPath ()
		{
			var prog = TestPrograms.NestedLambdas ().Build ();
			var path = new SExprPath (NOPList<int>.Empty);
			Check.AreEqual (path.Target (prog), prog);
		}

		[Test]
		public void TestFirstInPath ()
		{
			var prog = TestPrograms.NestedLambdas ().Build ();
			var path = new SExprPath (List.Create (0));
			Check.AreEqual (path.Target (prog), prog.AsList.First);
		}

		[Test]
		public void TestPathPastEnd ()
		{
			var prog = TestPrograms.NestedLambdas ().Build ();
			var path = new SExprPath (List.Create (10));
			Check.AreEqual (path.Target (prog), prog.AsList.Last);
		}

		[Test]
		public void TestSiblings ()
		{
			var prog = TestPrograms.NestedLambdas ().Build ();
			var path = new SExprPath (List.Create (0, 2, 2, 1, 1));

			Check.AreEqual (path.Target (prog), S ("y").Build ());
			var prev = path.PrevSibling (prog);

			Check.AreEqual (prev.Item1, S ("x").Build ());
			Check.AreEqual (prev.Item2.PrevSibling (prog).Item1, 
				L (S ("x"), S ("y")).Build ());

			path = new SExprPath (List.Create (0, 2));
			Check.AreEqual (path.NextSibling (prog).Item1, A (4).Build ());
		}

		[Test]
		public void TestNextAndPrevious ()
		{
			var prog = TestPrograms.NestedLambdas ().Build ();
			var path = new SExprPath (List.Create (0, 2, 2, 1));
			Check.AreEqual (path.Target (prog), L (S ("x"), S ("y")).Build ());

			var prev = path.Previous (prog);
			Check.AreEqual (prev.Item1, S ("lambda").Build ());
			Check.AreEqual (prev.Item2.Previous (prog).Item2.Previous (prog).Item1, 
				S ("bar").Build ());

			var next = path.Next (prog);
			Check.AreEqual (next.Item1, S ("x").Build ());
			Check.AreEqual (next.Item2.Next (prog).Item2.Next (prog).Item1, 
				L (S ("eq?"), S ("x"), S ("y")).Build ());			
		}

		[Test]
		public void TestCreatingPathToTarget ()
		{
			var prog = TestPrograms.NestedLambdas ().Build ();
			var path = new SExprPath (prog, 
				prog.AsList.First.AsList.RestNth(2).First.AsList.Rest.First);
			Check.AreEqual (path.Path, List.Create (0, 2, 1));
		}
	}
}
