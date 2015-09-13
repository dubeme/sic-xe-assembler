using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC.Assembler.Utilities.BinarySearchTree
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BSTNode<T>
    {
        /// <summary>
        /// Gets or sets the height of this BSTNode{T}.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Gets or sets the value of this BSTNode{T}.
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// Gets or sets the left of this BSTNode{T}.
        /// </summary>
        public BSTNode<T> Left { get; set; }
        /// <summary>
        /// Gets or sets the right of this BSTNode{T}.
        /// </summary>
        public BSTNode<T> Right { get; set; }

        /// <summary>
        /// Gets a value indicating whether [left and right node is set].
        /// </summary>
        public bool LeftAndRightNodeIsSet
        {
            get
            {
                return this.LeftNodeIsSet && this.RightNodeIsSet;
            }
        }
        /// <summary>
        /// Gets a value indicating whether [left or right node is set].
        /// </summary>
        public bool LeftOrRightNodeIsSet
        {
            get
            {
                return this.LeftNodeIsSet || this.RightNodeIsSet;
            }
        }
        /// <summary>
        /// Gets a value indicating whether [left node is set].
        /// </summary>
        public bool LeftNodeIsSet
        {
            get
            {
                return this.Left != null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether [right node is set].
        /// </summary>
        public bool RightNodeIsSet
        {
            get
            {
                return this.Right != null;
            }
        }
    }
}
