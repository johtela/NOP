namespace NOP
{
	using System;
	using Collections;

	/// <summary>
	/// Class that represents a location in a S-expression tree. 
	/// </summary>
	/// <remarks>
	/// SExprPath points to a S-expression in a tree by storing the indices of the 
	/// sexp nodes along the path. Thus, even if the sexp nodes are changed in the 
	/// tree, which occurs regularly when sexp trees are manipulated, the path 
	/// still points to a valid S-expression.
	/// 
	/// If the S-expression pointed by the path does not exist any more, then the
	/// closest previous S-expression is returned.
	/// </remarks>
	public class SExprPath
	{
		/// <summary>
		/// The path is represented by a list of indices.
		/// </summary>
		public readonly Sequence<int> Path;

		/// <summary>
		/// Create a SExprPath from a list of indices.
		/// </summary>
		/// <param name="path"></param>
		public SExprPath (Sequence<int> path)
		{
			Path = path;
		}

		/// <summary>
		/// Create an empty SExprPath.
		/// </summary>
		public SExprPath () : this (Sequence.Create (0)) {}

		public SExprPath (SExpr root, SExpr target)
		{
			//Path = FindNode (root, target);
		}

		//private NOPList<int> FindNode (SExpr root, SExpr target)
		//{
		//    if (root == target)
		//        return List.Create (0);
		//    if (!(root is SExpr.List))
		//        return NOPList<int>.Empty;
		//    var node = root.AsSequence;
		//    var path = List.Create (Tuple.Create (0, node));

		//    while (node.NotEmpty && path.NotEmpty && !ReferenceEquals(node.First, target))
		//    {
		//        if (node.First is SExpr.List)
		//        {
		//            node = node.First.AsSequence;
		//            path = Tuple.Create (0, node) | path;
		//        }
		//        else if (node.Rest.NotEmpty)
		//        {
		//            node = node.Rest;
		//            path = Tuple.Create (path.First.Item1 + 1, node) | path.Rest;
		//        }
		//        else if (path.Rest.NotEmpty)
		//        {
		//            do
		//            {
		//                path = path.Rest;
		//                node = path.First.Item2.Rest;
		//            }
		//            while (node.IsEmpty && path.Rest.NotEmpty);
		//            path = Tuple.Create (path.First.Item1 + 1, node) | path.Rest;
		//        }
		//    }
		//    return node.IsEmpty ? 
		//        NOPList<int>.Empty :
		//        path.Map (t => t.Item1).Reverse ();
		//}
		private class StackItem
		{
			public readonly int Ind;
			public readonly Sequence<SExpr> Seq;

			public StackItem (int i, Sequence<SExpr> s)
			{
				Ind = i;
				Seq = s;
			}
		}

		private Sequence<StackItem> PathToStack (SExpr root)
		{
			var result = Sequence<StackItem>.Empty;
			var sexp = root;

			for (var p = Path; !p.IsEmpty; p = p.RestL)
			{
				var lst = sexp as SExpr.List;
				if (lst == null) break;
				var i = p.First;
				var l = lst.Items.Length;
				if (l <= i)
					return result + new StackItem (l - 1, lst.Items);
				sexp = lst.Items[i];
				result = result + new StackItem (i, lst.Items);
			}
			return result;
		}

		private Tuple<SExpr, SExprPath> StackToPath (SExpr root, Sequence<StackItem> stack)
		{
			return stack.IsEmpty ?
				Tuple.Create (root, new SExprPath (Sequence.Create (int.MaxValue))) :
				Tuple.Create (stack.Last.Seq[stack.Last.Ind],
					new SExprPath (stack.Map (t => t.Ind)));
		}

		private bool NextSexp (ref Sequence<StackItem> stack)
		{
			var top = stack.Last;
			if (top.Ind >= top.Seq.Length - 1)
				return false;
			stack = stack.RestR + new StackItem (top.Ind + 1, top.Seq);
			return true;
		}

		private bool PrevSexp (ref Sequence<StackItem> stack)
		{
			var top = stack.Last;
			if (top.Ind == 0)
				return false;
			stack = stack.RestR + new StackItem (top.Ind - 1, top.Seq);
			return true;
		}

		private bool ChildSexp (SExpr root, ref Sequence<StackItem> stack)
		{
			var sexp = stack.IsEmpty ? root : stack.Last.Seq[stack.Last.Ind];
			var list = sexp as SExpr.List;
			if (list == null)
				return false;
			stack = stack + new StackItem (0, list.Items);
			return true;
		}

		/// <summary>
		/// Return the sexp list item pointed by the path, or the closest previous 
		/// sexp item, if the path refers to a non-existent S-expression.
		/// </summary>
		public SExpr Target (SExpr root)
		{
			var stack = PathToStack (root);
			return stack.IsEmpty ? root : stack.Last.Seq[stack.Last.Ind];
		}

		/// <summary>
		/// Return a tuple containing the sexp next to the sexp pointed by 
		/// the path and its path.
		/// </summary>
		public Tuple<SExpr, SExprPath> NextSibling (SExpr root)
		{
			var stack = PathToStack (root);
			if (stack.Length == Path.Length)
			{
				while (!stack.IsEmpty && !NextSexp (ref stack))
					stack = stack.RestR;
			}
			return StackToPath (root, stack);
		}

		/// <summary>
		/// Return a tuple containing the sexp previous to the sexp pointed by 
		/// the path and its path.
		/// </summary>
		public Tuple<SExpr, SExprPath> PrevSibling (SExpr root)
		{
			var stack = PathToStack (root);
			if (!stack.IsEmpty && stack.Length == Path.Length)
			{
				if (!PrevSexp (ref stack))
					stack = stack.RestR;
			}
			return StackToPath (root, stack);
		}

		/// <summary>
		/// Return the next S-expression of the path when traversing the
		/// tree in depth-first manner.
		/// </summary>
		public Tuple<SExpr, SExprPath> Next (SExpr root)
		{
			var stack = PathToStack (root);
			if (stack.Length == Path.Length)
			{
				if (!ChildSexp (root, ref stack))
				{
					while (!stack.IsEmpty && !NextSexp (ref stack))
						stack = stack.RestR;
				}
			}
			return StackToPath (root, stack);
		}

		/// <summary>
		/// Return the next S-expression of the path when traversing the
		/// tree in depth-first manner.
		/// </summary>
		public Tuple<SExpr, SExprPath> Previous (SExpr root)
		{
			var stack = PathToStack (root);
			if (!stack.IsEmpty && stack.Length == Path.Length)
			{
				if (PrevSexp (ref stack))
				{
					if (ChildSexp (root, ref stack))
						while (NextSexp (ref stack) || ChildSexp (root, ref stack)) ;
				}
				else stack = stack.RestR;
			}
			return StackToPath (root, stack);
		}

		/// <summary>
		/// Return the string representation of the path.
		/// </summary>
		public override string ToString ()
		{
			return Path.ToString ();
		}
	}
}