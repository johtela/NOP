namespace NOP.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Abstract tree data structure.
    /// </summary>
    /// <typeparam name="K">The key type of the tree.</typeparam>
    public abstract class Tree<K> where K : IComparable<K>
    {
        /// <summary>
        /// Returns the left subtree of this tree.
        /// </summary>
        protected internal abstract Tree<K> Left { get; }

        /// <summary>
        /// Returns the right subtree of this tree.
        /// </summary>
        protected internal abstract Tree<K> Right { get; }

        /// <summary>
        /// Returns the key of this tree.
        /// </summary>
        protected internal abstract K Key { get; }

        /// <summary>
        /// Returns the weight of the tree. That is, the number of nodes in the tree including
        /// the root.
        /// </summary>
        protected internal abstract int Weight { get; }

        /// <summary>
        /// Clones the root of the tree.
        /// </summary>
        /// <param name="newLeft">The tree that is assigned to left subtree of the cloned root.</param>
        /// <param name="newRight">The tree that is assigned to right subtree of the cloned root.</param>
        /// <param name="inPlace">If the inPlace parameter is true, the root is mutated in place, 
        /// instead of copying it.</param>
        /// <returns>The cloned root tree.</returns>
        protected internal abstract Tree<K> Clone(Tree<K> newLeft, Tree<K> newRight, bool inPlace);
    }

    /// <summary>
    /// Static class that contains the operations for trees.
    /// </summary>
    /// <typeparam name="T">The tree type.</typeparam>
    /// <typeparam name="K">The key type of the tree.</typeparam>
    public static class Tree<T, K>
        where T : Tree<K>
        where K : IComparable<K>
    {
        /// <summary>
        /// The comparer class for comparing trees.
        /// </summary>
        private class Comparer : IComparer<T>
        {
            /// <summary>
            /// Returns the comparison between two tree nodes.
            /// </summary>
            /// <param name="x">The first tree.</param>
            /// <param name="y">The second tree.</param>
            /// <returns>-1, if the key of tree x is less than key of tree y.<br/>
            /// 0, if the key of tree x is less than key of tree y.<br/>
            /// 1, oif the key of tree x is greater that key of tree y.</returns>
            public int Compare(T x, T y)
            {
                return x.Key.CompareTo(y.Key);
            }
        }

        private static Comparer _comparer = new Comparer();
        internal static T _empty;

        /// <summary>
        /// Tests if the given tree is an empty tree.
        /// </summary>
        /// <param name="tree">The tree to be tested.</param>
        /// <returns>True, if the tree is an empty tree; false otherwise.</returns>
        private static bool IsEmpty(T tree) 
        {
            return tree == _empty;
        }

        /// <summary>
        /// Helper method for getting the left subtree of a tree.
        /// </summary>
        /// <param name="tree">The tree whose subtree is returned.</param>
        /// <returns>The left subtree of the tree.</returns>
        private static T Left(Tree<K> tree)
        {
            return (T)tree.Left;
        }

        /// <summary>
        /// Helper method for getting the right subtree of a tree.
        /// </summary>
        /// <param name="tree">The tree whose subtree is returned.</param>
        /// <returns>The right subtree of the tree.</returns>
        private static T Right(Tree<K> tree)
        {
            return (T)tree.Right;
        }
		
        /// <summary>
        /// Search for a given key in the tree.
        /// </summary>
        /// <param name="tree">The tree from which the key is searched.</param>
        /// <param name="key">The key that is searched.</param>
        /// <returns>The subtree that contains the given key, or an empty tree
        /// if the key is not found.</returns>
        public static T Search(T tree, K key)
        {
            while (true)
            {
                if (IsEmpty(tree))
                    return tree;

                int compare = key.CompareTo(tree.Key);

                if (compare == 0)
                    return tree;
                else if (compare > 0)
                    tree = Right(tree);
                else
                    tree = Left(tree);
            }
        }

        /// <summary>
        /// Add a new item to the tree.
        /// </summary>
        /// <param name="tree">The tree to which the new item is added.</param>
        /// <param name="item">The item to be added.</param>
        /// <param name="height">The height of the node specified by the 
        /// tree<see cref="tree"/> parameter.</param>
        /// <returns>A new tree that contains the given item.</returns>
        public static T Add(T tree, T item, int height)
        {
            if (IsEmpty(tree))
                return item;

            bool rebalance = (height > 0) && (height > (10 * Math.Log(tree.Weight + 1, 2)));
            int nextHeight = (height > 0) && (!rebalance) ? height + 1 : 0; 
			
            T result = item.Key.CompareTo(tree.Key) > 0 ?
                (T)tree.Clone(tree.Left, Add(Right(tree), item, nextHeight), false) :
                (T)tree.Clone(Add(Left(tree), item, nextHeight), tree.Right, false);
            if (rebalance)
                result = Rebalance(result);
            return result;
        }

        /// <summary>
        /// Remove the item with a given key from the tree.
        /// </summary>
        /// <param name="tree">The tree from where the item is removed.</param>
        /// <param name="key">The key of the item to be removed.</param>
        /// <returns>A new tree from which the item with the given key is removed.</returns>
        public static T Remove(T tree, K key)
        {
            if (IsEmpty(tree))
                return tree;
            
            int compare = key.CompareTo(tree.Key);
            
            if (compare == 0)
            {
                // We have a match. If this is a leaf, just remove it 
                // by returning Empty.  If we have only one child,
                // replace the node with the child.
                if (IsEmpty(Right(tree)) && IsEmpty(Left(tree)))
                    return Right(tree);
                else if (IsEmpty(Right(tree)) && !IsEmpty(Left(tree)))
                    return Left(tree);
                else if (!IsEmpty(Right(tree)) && IsEmpty(Left(tree)))
                    return Right(tree);
                else
                {
                    // We have two children. Remove the next-highest node and replace
                    // this node with it.
                    T successor = Right(tree);
                    while (!IsEmpty(Left(successor)))
                        successor = Left(successor);
                    return (T)successor.Clone(tree.Left, Remove(Right(tree), successor.Key), false);
                }
            }
            else if (compare < 0)
                return (T)tree.Clone(Remove(Left(tree), key), Right(tree), false);
            else
                return (T)tree.Clone(Left(tree), Remove(Right(tree), key), false);
        }

        /// <summary>
        /// Enumerate the nodes of the tree in-order.
        /// </summary>
        /// <param name="tree">The tree to be traversed.</param>
        /// <returns>An enumerator that provides the items of the tree in the correct order.
        /// </returns>
        public static IEnumerable<T> Enumerate(T tree)
        {
            var stack = new System.Collections.Generic.Stack<T>();

            for (T current = tree; !IsEmpty(current) || stack.Count > 0; current = Right(current))
            {
                while (!IsEmpty(current))
                {
                    stack.Push(current);
                    current = Left(current);
                }
                current = stack.Pop();
                yield return current;
            }
        }

        /// <summary>
        /// Return the items of the tree in an array.
        /// </summary>
        /// <param name="tree">The tree to be traversed.</param>
        /// <returns>An array that contains the items of the the tree in-order.</returns>
        public static T[] ToArray(T tree)
        {
            var result = new T[tree.Weight];
            int i = 0;

            foreach (var node in Enumerate(tree))
                result[i++] = node;
            return result;
        }

        /// <summary>
        /// Inserts the items in the array to a new tree.
        /// </summary>
        /// <param name="array">The array that contains the items. The array is sorted in-place
        /// and the items are attached to the tree. This means that the callers should not use
        /// the list any more after this function returns.</param>
        /// <returns>A new tree that contains the items in the list.</returns>
        public static T FromArray (T[] array, bool throwIfDuplicate)
		{
			if (array.Length == 0)
				return _empty;
			    
			Array.Sort (array, _comparer);
			var last = RemoveDuplicates (array, throwIfDuplicate);
			return RebalanceList (array, 0, last, true);
		}

		public static int RemoveDuplicates (T[] array, bool throwIfDuplicate)
		{
			var res = 0;
			
			for (int i = 1; i < array.Length; i++)
			{
				if (array[res].Key.CompareTo(array[i].Key) != 0)
					array[++res] = array[i];
				else if (throwIfDuplicate) 
					throw new ArgumentException("Duplicate key: " + array[i].Key);
			}
			return res;
		}

        /// <summary>
        /// Rebalances the items in the tree.
        /// </summary>
        /// <param name="tree">The tree to be rebalanced.</param>
        /// <returns>A new tree that contains the same items as the original tree,
        /// but is in perfect balance.</returns>
        private static T Rebalance(T tree)
        {
            var array = ToArray(tree);
            return RebalanceList(array, 0, array.Length - 1, false);
        }

        /// <summary>
        /// Rebalances the items in a list.
        /// </summary>
        /// <param name="array">The array that contains the items to be rebalanced.</param>
        /// <param name="low">The index of the lowest item in the list.</param>
        /// <param name="high">The index of the highest item in the list.</param>
        /// <param name="inPlace">If the inPlace parameter is true, then the items
        /// in the list are recycled in the new tree. That is, the items are mutated
        /// to create the new tree.</param>
        /// <returns></returns>
        private static T RebalanceList(T[] array, int low, int high, bool inPlace) 
        {
            if (low <= high)
            {
                var middle = (low + high) / 2;
                return (T)array[middle].Clone(
                    RebalanceList(array, low, middle - 1, inPlace),
                    RebalanceList(array, middle + 1, high, inPlace), inPlace);
            }
            else
                return _empty;
        }
    }
}
