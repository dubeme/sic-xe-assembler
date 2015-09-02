using SIC.Assembler.Utilities.Extensions;
using System;
using System.Collections.Generic;

namespace SIC.Assembler.Utilities.BinarySearchTree
{
    public class Tree<T> where T : IComparable
    {
        private BSTNode<T> _Root;

        public Tree()
        {
        }

        public Tree(IList<T> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    this.Insert(item);
                }
            }
        }

        public BSTNode<T> Find(T item)
        {
            if (this._Root == null)
            {
                return null;
            }
            if (this._Root.Value.CompareTo(item) == 0)
            {
                return this._Root;
            }
            else
            {
                return this.Find(item, this._Root);
            }
        }

        public void Insert(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Can't insert null into the tree");
            }

            if (this._Root == null)
            {
                this._Root = new BSTNode<T>
                {
                    Value = item
                };
            }
            else
            {
                this.Insert(item, this._Root);
            }
        }

        public void Print()
        {
        }

        public void Remove(T item)
        {
            if (this._Root == null)
            {
                return;
            }

            this.Remove(item, ref this._Root);
        }

        private BSTNode<T> Find(T valueToFind, BSTNode<T> currentNode)
        {
            if (valueToFind == null)
            {
                throw new ArgumentNullException("Can't search for a null value");
            }

            if (currentNode == null)
            {
                return null;
            }

            if (valueToFind.IsEqual(currentNode.Value))
            {
                return currentNode;
            }
            else if (valueToFind.IsGreaterThan(currentNode.Value))
            {
                return this.Find(valueToFind, currentNode.Right);
            }
            else if (valueToFind.IsLessThan(currentNode.Value))
            {
                return this.Find(valueToFind, currentNode.Left);
            }
            else
            {
                return null;
            }
        }

        private void Insert(T item, BSTNode<T> currentNode)
        {
            if (currentNode == null)
            {
                return;
            }

            if (item == null)
            {
                throw new ArgumentNullException("Can't insert null into the tree");
            }

            if (currentNode.Value.CompareTo(item) > 0)
            {
                // If current node value is bigger
                currentNode.Height++;
                if (currentNode.Right != null)
                {
                    this.Insert(item, currentNode.Right);
                }
                else
                {
                    currentNode.Right = new BSTNode<T>
                    {
                        Value = item
                    };
                }
            }
            else if (currentNode.Value.CompareTo(item) < 0)
            {
                // If current node value is smaller
                currentNode.Height++;
                if (currentNode.Left != null)
                {
                    this.Insert(item, currentNode.Left);
                }
                else
                {
                    currentNode.Left = new BSTNode<T>
                    {
                        Value = item
                    };
                }
            }
            else
            {
                currentNode.HandleDuplicate(item);
            }
        }

        private void Remove(T item, ref BSTNode<T> currentRootNode)
        {
            if (currentRootNode.Value.CompareTo(item) == 0)
            {
                /*
                          0
                       4     7
                     2   5 6   8

                     On delete, it's assumed it happens in the root, hence these possible scenarios.

                0) Root has no children.
                    Delete root
                1)  Has 1 child
                    *) if left, set root to left node
                    *) If right, set root to right node
                2) Has 2 children
                    *) Traverse to the biggest node and swap with root

                */

                if (currentRootNode.LeftRightNodeIsSet)
                {
                    var tempNode = currentRootNode;

                    if (true)
                    {
                    }
                }
                else if (currentRootNode.LeftNodeIsSet)
                {
                    currentRootNode = currentRootNode.Left;
                }
                else if (currentRootNode.RightNodeIsSet)
                {
                    currentRootNode = currentRootNode.Right;
                }
                else
                {
                    // No child
                    currentRootNode = null;
                }
            }
        }

        //private void RemoveNode
    }
}