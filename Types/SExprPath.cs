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
		/// When a S-expression referred by the path is searched for, a graph of
		/// SExprNodes is created. The SExprNode class makes navigation in a sexp 
		/// tree easier by storing explicitly the previous and parent expressions.
		/// </summary>
		/// <remarks>
		/// Note that SExprNode works on NOPList{SExpr} objects instead of SExpr
		/// objects. This is because we need to be able to move in the sexp list
		/// where the sexps reside. The assumption is that all S-expression except
		/// the root expression is inside a list.
		/// </remarks>
		private class SExprNode
		{
			/// <summary>
			/// Create a new SExprNode.
			/// </summary>
			/// <param name="current">The current item in the sexp list.</param>
			/// <param name="index">The index of the sexp in the list.</param>
			/// <param name="previous">The previous SExprNode in the list, or null
			/// if the sexp is the first in the list.</param>
			/// <param name="parent">The parent SExprNode, or null if the sexp is
			/// the root.</param>
			public SExprNode (NOPList<SExpr> current, int index, 
				SExprNode previous, SExprNode parent)
			{
				Current = current;
				Index = index;
				Previous = previous;
				Parent = parent;
			}

			/// <summary>
			/// The current sexp in the list.
			/// </summary>
			public NOPList<SExpr> Current { get; private set; }

			/// <summary>
			/// The index of the current sexp.
			/// </summary>
			public int Index { get; private set; }
			
			/// <summary>
			/// The SExprNode representing the previous sexp in the list.
			/// </summary>
			public SExprNode Previous { get; private set; }
			
			/// <summary>
			/// The SExprNode representing the parent sexp.
			/// </summary>
			public SExprNode Parent { get; private set; }

			private SExprNode _next;
			private SExprNode _firstChild;

			/// <summary>
			/// The SExprNode representing the next sexp in the list.
			/// </summary>
			public SExprNode Next
			{
				get 
				{
					if (_next == null && !Current.Rest.IsEmpty)
						_next = new SExprNode (Current.Rest, Index + 1, this, Parent);
					return _next; 
				}
			}

			/// <summary>
			/// The SExprNode representing the first child sexp.
			/// </summary>
			public SExprNode FirstChild
			{
				get 
				{ 
					if (_firstChild == null)
					{
						var lst = Current.First as SExpr.List;
						if (lst != null && !lst.Items.IsEmpty)
							_firstChild = new SExprNode (lst.Items, 0, null, this);
					}
					return null; 
				}
			}

			/// <summary>
			/// The path from this SExprNode to the root sexp.
			/// </summary>
			public SExprPath Path
			{
				get 
				{
					var result = NOPList<int>.Empty;
					var node = this;

					while (node != null)
					{
						result = node.Index | result;
						node = node.Parent;
					}
					return new SExprPath (result);
				}
			}
		}

		/// <summary>
		/// The path is represented by a list of indices.
		/// </summary>
		public readonly NOPList<int> Path;

		/// <summary>
		/// Create a SExprPath from a list of indices.
		/// </summary>
		/// <param name="path"></param>
		public SExprPath (NOPList<int> path)
		{
			Path = path;
		}

		/// <summary>
		/// Create an empty SExprPath.
		/// </summary>
		public SExprPath () : this (NOPList<int>.Empty)
		{
		}

		/// <summary>
		/// Return the graph of SExprNodes based on the path.
		/// </summary>
		/// <param name="root">The root sexp.</param>
		/// <returns>The SExprNode representing the target of the path.
		/// If the actual target is not found, then SExprNode of the 
		/// closest previous sexp is returned.</returns>
		private SExprNode NodeOfPath (SExpr root)
		{
			var slist = root as SExpr.List;
			if (slist == null)
				return null;
			var list = slist.Items;
			var inds = Path;
			SExprNode parent = null;

			while (list.NotEmpty && inds.NotEmpty)
			{
				SExprNode node = null, prev = null;
				var i = 0;

				while (list.NotEmpty && i <= inds.First)
				{
					node = new SExprNode (list, i++, prev, parent);
					prev = node;
					if (i <= inds.First)
						list = list.Rest;
				}
				if (list.NotEmpty && list.First is SExpr.List)
					list = (list.First as SExpr.List).Items;
				inds = inds.Rest;
				parent = node;
			}
			return parent;
		}

		/// <summary>
		/// Return the sexp list item pointed by the path, or the
		/// closest previous sexp item, if the path refers to a
		/// non-existent S-expression.
		/// </summary>
		public NOPList<SExpr> Target (SExpr root)
		{
			var node = NodeOfPath (root);
			return node != null ? node.Current : NOPList<SExpr>.Empty;
		}

		/// <summary>
		/// Return a tuple containing the sexp next to the sexp pointed by 
		/// the path, and the path to the next S-expression.
		/// </summary>
		public Tuple<NOPList<SExpr>, SExprPath> NextSibling (SExpr root)
		{
			var node = NodeOfPath (root);

			while (node != null && node.Next == null)
				node = node.Parent;
			return node != null ?
				Tuple.Create (node.Next.Current, node.Next.Path) :
				Tuple.Create (NOPList<SExpr>.Empty, new SExprPath ());
		}

		/// <summary>
		/// Return a tuple containing the sexp previous to the sexp pointed by 
		/// the path, and the path to the previous S-expression.
		/// </summary>
		public Tuple<NOPList<SExpr>, SExprPath> PrevSibling (SExpr root)
		{
			var node = NodeOfPath (root);

			if (node != null)
				node = node.Previous ?? node.Parent;
			return node != null ?
				Tuple.Create (node.Current, node.Path) :
				Tuple.Create (NOPList<SExpr>.Empty, new SExprPath ());
		}
	}
}
