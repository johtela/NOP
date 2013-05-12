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
		/// Item of the stack of SExps.
		/// </summary>
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

		/// <summary>
		/// The path is represented by a list of indices.
		/// </summary>
		public readonly Sequence<int> Path;

		/// <summary>
		/// Create a SExprPath from a list of indices.
		/// </summary>
		public SExprPath (Sequence<int> path)
		{
			Path = path;
		}

		/// <summary>
		/// Create a SExprPath that points to root.
		/// </summary>
		public SExprPath () : this (Sequence<int>.Empty ) {}

		/// <summary>
		/// Create a SExprPath that is pointig to a given node.
		/// </summary>
		public SExprPath (SExpr root, SExpr target)
		{
			Path = FindSexp (root, target);
		}

		/// <summary>
		/// Find the path to the given node.
		/// </summary>
		private Sequence<int> FindSexp (SExpr root, SExpr target)
		{
			if (root == target)
				return Sequence.Create (0);
			if (!(root is SExpr.List))
				return Sequence<int>.Empty;
			var node = root.AsSequence ();
			var stack = Sequence.Create (new StackItem (0, node));

			while (!node.IsEmpty && !stack.IsEmpty && !ReferenceEquals (node.First, target))
			{
				if (node.First is SExpr.List)
				{
					node = node.First.AsSequence ();
					stack = stack + new StackItem (0, node);
				}
				else if (!node.RestL.IsEmpty)
				{
					node = node.RestL;
					stack = stack.RestR + new StackItem (stack.Last.Ind + 1, node);
				}
				else if (!stack.RestR.IsEmpty)
				{
					do
					{
						stack = stack.RestR;
						node = stack.Last.Seq.RestL;
					}
					while (node.IsEmpty && !stack.RestR.IsEmpty);
					stack = stack.RestR + new StackItem (stack.Last.Ind + 1, node);
				}
			}
			return node.IsEmpty ? Sequence<int>.Empty : stack.Map (i => i.Ind);
		}

		/// <summary>
		/// Convert a path to s-exp stack.
		/// </summary>
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

		/// <summary>
		/// Convert a stack of s-exps to path.
		/// </summary>
		private Tuple<SExpr, SExprPath> StackToPath (SExpr root, Sequence<StackItem> stack)
		{
			return stack.IsEmpty ?
				Tuple.Create (root, new SExprPath (Sequence<int>.Empty)) :
				Tuple.Create (stack.Last.Seq[stack.Last.Ind],
					new SExprPath (stack.Map (t => t.Ind)));
		}

		/// <summary>
		/// Helper function to move the stack to next s-exp, if one exists.
		/// </summary>
		private bool NextSexp (ref Sequence<StackItem> stack)
		{
			var top = stack.Last;
			if (top.Ind >= top.Seq.Length - 1)
				return false;
			stack = stack.RestR + new StackItem (top.Ind + 1, top.Seq);
			return true;
		}

		/// <summary>
		/// Helper function to move the stack to previous s-exp, if one exists.
		/// </summary>
		private bool PrevSexp (ref Sequence<StackItem> stack)
		{
			var top = stack.Last;
			if (top.Ind == 0)
				return false;
			stack = stack.RestR + new StackItem (top.Ind - 1, top.Seq);
			return true;
		}

		/// <summary>
		/// Helper function to move the stack to child s-exp, if one exists.
		/// </summary>
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
			var orig = PathToStack (root);
			var stack = orig;
			if (stack.Length == Path.Length)
			{
				while (!stack.IsEmpty && !NextSexp (ref stack))
					stack = stack.RestR;
			}
			return StackToPath (root, stack.IsEmpty ? orig : stack);
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
			var orig = PathToStack (root);
			var stack = orig;
			if (stack.Length == Path.Length)
			{
				if (!ChildSexp (root, ref stack))
				{
					while (!stack.IsEmpty && !NextSexp (ref stack))
						stack = stack.RestR;
				}
			}
			return StackToPath (root, stack.IsEmpty ? orig : stack);
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

		public override bool Equals (object obj)
		{
			var other = obj as SExprPath;
			return other != null && Path.Equals (other.Path);
		}

		public override int GetHashCode ()
		{
			return Path.GetHashCode ();
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