using SIC.Assembler.Utilities.Collections;
using SIC.Assembler.Utilities.Model;
using System;

namespace Assembler.UnitTest.Utilities.BinarySearchTree
{
    public class TreeUtilities
    {
        public static BinarySearchTree<T> CreateTree<T>(params T[] values) where T : IComparable
        {
            var root = TreeUtilities.MockTree<T>();

            foreach (var item in values)
            {
                root.Insert(item);
            }
            return root;
        }

        public static BSTNode<T> MockBSTNode<T>(T value, BSTNode<T> left = null, BSTNode<T> right = null)
        {
            return new BSTNode<T>
            {
                Value = value,
                Left = left,
                Right = right
            };
        }

        public static BinarySearchTree<T> MockTree<T>() where T : IComparable
        {
            return new BinarySearchTree<T> { };
        }
    }
}