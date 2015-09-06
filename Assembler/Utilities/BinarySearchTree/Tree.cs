﻿using SIC.Assembler.Utilities.Extensions;
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

        public BSTNode<T> FindNode(object value)
        {
            if (this._Root == null)
            {
                return null;
            }

            return this.Find(value, this._Root);
        }

        public T FindValue(object key)
        {
            var node = FindNode(key);

            if (node == null)
            {
                return default(T);
            }

            return node.Value;
        }

        public void Insert(T item, Action<T> duplicateHandler = null)
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
                this.Insert(item, this._Root, duplicateHandler);
            }
        }

        public void Print(TraverseOrder traverseOrder = TraverseOrder.InOrder, Action<BSTNode<T>> printFunction)
        {
            switch (traverseOrder)
            {
                case TraverseOrder.InOrder:
                    this.PrintInOrder(this._Root, printFunction);
                    break;

                case TraverseOrder.PostOrder:
                    this.PrintPostOrder(this._Root, printFunction);
                    break;

                case TraverseOrder.PreOrder:
                    this.PrintPreOrder(this._Root, printFunction);
                    break;

                case TraverseOrder.LevelOrder:
                    this.PrintLevelOrder(this._Root, printFunction);
                    break;
            }
        }

        public void Remove(T item)
        {
            if (this._Root == null)
            {
                return;
            }

            this.Remove(item, ref this._Root);
        }

        private BSTNode<T> DetachBiggestNodeWithNoChild(ref BSTNode<T> root)
        {
            BSTNode<T> detachedNode = null;

            if (root == null)
            {
                throw new ArgumentNullException("Root can't be null");
            }
            else if (!root.LeftAndRightNodeIsSet)
            {
                // has only root
                detachedNode = root;
                root = null;
            }
            else if (!root.RightNodeIsSet)
            {
                // root has no right child
                detachedNode = root;
                root = root.Left;
            }
            else if (!root.Right.LeftOrRightNodeIsSet)
            {
                // root has only one right child
                detachedNode = root.Right;
                root.Right = null;
            }
            else
            {
                // At least one level from root
                var currentNode = root;
                do
                {
                    // In order to maintain the pointer to enable the modification of the main tree
                    if (currentNode.RightNodeIsSet && currentNode.Right.LeftOrRightNodeIsSet)
                    {
                        currentNode = currentNode.Right;
                    }
                    else
                    {
                        if (currentNode.RightNodeIsSet)
                        {
                            detachedNode = currentNode.Right;
                            currentNode.Right = null;
                        }
                        else
                        {
                            detachedNode = currentNode;
                            currentNode.Right = currentNode.Left;
                        }

                        break;
                    }
                } while (currentNode != null);
            }

            return detachedNode;
        }

        private BSTNode<T> Find(object valueToFind, BSTNode<T> currentNode)
        {
            if (valueToFind == null)
            {
                throw new ArgumentNullException("Can't search for a null value");
            }

            if (currentNode == null)
            {
                return null;
            }

            if (currentNode.Value.IsEqual(valueToFind))
            {
                return currentNode;
            }
            else if (currentNode.Value.IsGreaterThan(valueToFind))
            {
                return this.Find(valueToFind, currentNode.Left);
            }
            else if (currentNode.Value.IsLessThan(valueToFind))
            {
                return this.Find(valueToFind, currentNode.Right);
            }
            else
            {
                return null;
            }
        }

        private void Insert(T item, BSTNode<T> currentNode, Action<T> duplicateHandler = null)
        {
            if (currentNode == null)
            {
                return;
            }

            if (item == null)
            {
                throw new ArgumentNullException("Can't insert null into the tree");
            }

            if (item.IsGreaterThan(currentNode.Value))
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
            else if (item.IsLessThan(currentNode.Value))
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
                if (duplicateHandler != null)
                {
                    duplicateHandler(currentNode.Value);
                }
            }
        }

        private void PrintInOrder(BSTNode<T> currentNode, Action<BSTNode<T>> printFunction)
        {
            if (currentNode != null)
            {
                PrintInOrder(currentNode.Left, printFunction);
                printFunction(currentNode);
                PrintInOrder(currentNode.Right, printFunction);
            }
        }

        private void PrintLevelOrder(BSTNode<T> currentNode, Action<BSTNode<T>> printFunction)
        {
            if (currentNode != null)
            {
                var nodes = new Queue<BSTNode<T>>();
                nodes.Enqueue(currentNode);

                while (nodes.Count > 0)
                {
                    var node = nodes.Dequeue();
                    printFunction(node);

                    if (node.LeftNodeIsSet)
                    {
                        nodes.Enqueue(node.Left);
                    }
                    if (node.RightNodeIsSet)
                    {
                        nodes.Enqueue(node.Right);
                    }
                }
            }
        }

        private void PrintPostOrder(BSTNode<T> currentNode, Action<BSTNode<T>> printFunction)
        {
            if (currentNode != null)
            {
                PrintPostOrder(currentNode.Left, printFunction);
                PrintPostOrder(currentNode.Right, printFunction);
                printFunction(currentNode);
            }
        }

        private void PrintPreOrder(BSTNode<T> currentNode, Action<BSTNode<T>> printFunction)
        {
            if (currentNode != null)
            {
                printFunction(currentNode);
                PrintPreOrder(currentNode.Left, printFunction);
                PrintPreOrder(currentNode.Right, printFunction);
            }
        }

        private void Remove(T item, ref BSTNode<T> currentRootNode)
        {
            if (item.IsEqual(currentRootNode.Value))
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

                if (currentRootNode.LeftAndRightNodeIsSet)
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
    }
}