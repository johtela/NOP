namespace NOP
{
    using System;
	using Collections;

	public class SExprPath
	{
		private class SExprNode
		{
			public SExprNode (NOPList<SExpr> current, int index, 
                SExprNode previous, SExprNode parent)
			{
				Current = current;
				Index = index;
				Previous = previous;
				Parent = parent;
			}

			public NOPList<SExpr> Current { get; private set; }

			public int Index { get; private set; }
            
			public SExprNode Previous { get; private set; }
            
			public SExprNode Parent { get; private set; }

			private SExprNode _next;
			private SExprNode _firstChild;

			public SExprNode Next
			{
				get {
					if (_next == null && !Current.Rest.IsEmpty)
						_next = new SExprNode (Current.Rest, Index + 1, this, Parent);
					return _next; 
				}
			}

			public SExprNode FirstChild
			{
				get { 
					if (_firstChild == null)
					{
						var lst = Current.First as SExpr.List;
						if (lst != null && !lst.Items.IsEmpty)
							_firstChild = new SExprNode (lst.Items, 0, null, this);
					}
					return null; 
				}
			}

			public SExprPath Path
			{
				get {
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

		public readonly NOPList<int> Path;

		public SExprPath (NOPList<int> path)
		{
			Path = path;
		}

		public SExprPath () : this (NOPList<int>.Empty)
		{
		}

		private SExprNode NodeOfPath (NOPList<SExpr> root)
		{
			var list = root;
			var inds = Path;
			SExprNode parent = null;

			while (list.NotEmpty && inds.NotEmpty)
			{
				SExprNode node = null, prev = null;
				for (var i = 0; list.NotEmpty && i <= inds.First; i++, list = list.Rest)
				{
					node = new SExprNode (list, i, prev, parent);
					prev = node;
				}
				if (list.NotEmpty && list.First is SExpr.List)
					list = (list.First as SExpr.List).Items;
				inds = inds.Rest;
				parent = node;
			}
			return parent;
		}

		public NOPList<SExpr> Target (NOPList<SExpr> root)
		{
			var node = NodeOfPath (root);
			return node != null ? node.Current : NOPList<SExpr>.Empty;
		}

		public Tuple<NOPList<SExpr>, SExprPath> NextSibling (NOPList<SExpr> root)
		{
			var node = NodeOfPath (root);

			while (node != null && node.Next == null)
				node = node.Parent;
			return node != null ?
                Tuple.Create (node.Next.Current, node.Next.Path) :
                Tuple.Create (NOPList<SExpr>.Empty, new SExprPath ());
		}

		public Tuple<NOPList<SExpr>, SExprPath> PrevSibling (NOPList<SExpr> root)
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
