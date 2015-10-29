using SIC.Assembler.Utilities.Extensions;
using SIC.Assembler.Utilities.Model;
using System;
using System.Collections.Generic;

namespace SIC.Assembler.Utilities.Collections
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinarySearchTree<T> where T : IComparable
    {
        /// <summary>
        /// The _ root
        /// </summary>
        private BSTNode<T> _Root;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySearchTree{T}"/> class.
        /// </summary>
        public BinarySearchTree()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySearchTree{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public BinarySearchTree(IList<T> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    this.Insert(item);
                }
            }
        }

        /// <summary>
        /// Finds the node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public BSTNode<T> FindNode(object value)
        {
            if (this._Root == null)
            {
                return null;
            }

            return this.Find(value, this._Root);
        }

        /// <summary>
        /// Finds the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public T FindValue(object value)
        {
            var node = FindNode(value);

            if (node == null)
            {
                return default(T);
            }

            return node.Value;
        }

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="duplicateHandler">The duplicate handler.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Insert(T item, Action<T> duplicateHandler = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
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

        /// <summary>
        /// Prints the specified traverse order.
        /// </summary>
        /// <param name="traverseOrder">The traverse order.</param>
        /// <param name="printFunction">The print function.</param>
        public void Print(TraverseOrder traverseOrder = TraverseOrder.InOrder, Action<object> printFunction = null)
        {
            printFunction = printFunction ?? new Action<object>((value) =>
            {
                Console.WriteLine(value);
            });

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

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(T item)
        {
            if (this._Root == null)
            {
                return;
            }

            this.Remove(item, ref this._Root);
        }

        /// <summary>
        /// Detaches the biggest node with no child.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Root can't be null</exception>
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

        public bool IsEmpty()
        {
            return this._Root == null || this._Root.Value == null;
        }

        /// <summary>
        /// Finds the specified value to find.
        /// </summary>
        /// <param name="valueToFind">The value to find.</param>
        /// <param name="currentNode">The current node.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Can't search for a null value</exception>
        private BSTNode<T> Find(object valueToFind, BSTNode<T> currentNode)
        {
            if (valueToFind == null)
            {
                throw new ArgumentNullException(nameof(valueToFind), "Can't search for a null value");
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

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="currentNode">The current node.</param>
        /// <param name="duplicateHandler">The duplicate handler.</param>
        /// <exception cref="System.ArgumentNullException">Can't insert null into the tree</exception>
        private void Insert(T item, BSTNode<T> currentNode, Action<T> duplicateHandler)
        {
            if (currentNode == null)
            {
                return;
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Can't insert null into the tree");
            }

            if (item.IsGreaterThan(currentNode.Value))
            {
                // If current node value is bigger
                // currentNode.Height++;
                if (currentNode.Right != null)
                {
                    this.Insert(item, currentNode.Right, duplicateHandler);
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
                // currentNode.Height++;
                if (currentNode.Left != null)
                {
                    this.Insert(item, currentNode.Left, duplicateHandler);
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

        /// <summary>
        /// Prints the in order.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="printFunction">The print function.</param>
        private void PrintInOrder(BSTNode<T> currentNode, Action<object> printFunction)
        {
            if (currentNode != null)
            {
                PrintInOrder(currentNode.Left, printFunction);
                printFunction(currentNode.Value);
                PrintInOrder(currentNode.Right, printFunction);
            }
        }

        /// <summary>
        /// Prints the level order.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="printFunction">The print function.</param>
        private void PrintLevelOrder(BSTNode<T> currentNode, Action<object> printFunction)
        {
            if (currentNode != null)
            {
                // Todo: Find a more efficient algorithm
                var nodes = new List<BSTNode<T>> { currentNode };
                var tempNodes = new List<BSTNode<T>> { };
                var printOut = new List<T>();

                do
                {
                    foreach (var node in nodes)
                    {
                        printOut.Add(node.Value);
                    }

                    printFunction(printOut.ToArray());

                    printOut.Clear();
                    tempNodes.Clear();

                    foreach (var node in nodes)
                    {
                        if (node.LeftNodeIsSet)
                        {
                            tempNodes.Add(node.Left);
                        }
                        if (node.RightNodeIsSet)
                        {
                            tempNodes.Add(node.Right);
                        }
                    }

                    nodes = new List<BSTNode<T>>(tempNodes);
                } while (nodes.Count > 0);
            }
        }

        /// <summary>
        /// Prints the post order.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="printFunction">The print function.</param>
        private void PrintPostOrder(BSTNode<T> currentNode, Action<object> printFunction)
        {
            if (currentNode != null)
            {
                PrintPostOrder(currentNode.Left, printFunction);
                PrintPostOrder(currentNode.Right, printFunction);
                printFunction(currentNode.Value);
            }
        }

        /// <summary>
        /// Prints the pre order.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="printFunction">The print function.</param>
        private void PrintPreOrder(BSTNode<T> currentNode, Action<object> printFunction)
        {
            if (currentNode != null)
            {
                printFunction(currentNode.Value);
                PrintPreOrder(currentNode.Left, printFunction);
                PrintPreOrder(currentNode.Right, printFunction);
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="currentRootNode">The current root node.</param>
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