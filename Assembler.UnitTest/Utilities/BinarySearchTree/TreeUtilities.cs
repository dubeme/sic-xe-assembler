using SIC.Assembler.Utilities.BinarySearchTree;
using System;

namespace Assembler.UnitTest.Utilities.BinarySearchTree
{
    public class TreeUtilities
    {
        public static BSTNode<T> MockBSTNode<T>(T value, BSTNode<T> left = null, BSTNode<T> right = null)
        {
            return new BSTNode<T>
            {
                Value = value,
                Left = left,
                Right = right
            };
        }

        public static BSTNode<object> MockTestNode(object value = null)
        {
            return new BSTNode<object>
            {
                Value = value,
                Left = null,
                Right = null
            };
        }

        public static Tree<T> MockTree<T>() where T : IComparable
        {
            return new Tree<T> { };
        }
    }
}