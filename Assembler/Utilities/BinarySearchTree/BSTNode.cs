using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC.Assembler.Utilities.BinarySearchTree
{
    public class BSTNode<T>
    {
        public int Height { get; set; }
        public T Value { get; set; }
        public BSTNode<T> Left { get; set; }
        public BSTNode<T> Right { get; set; }
        public void HandleDuplicate(T duplicate) { }

        public bool LeftRightNodeIsSet
        {
            get
            {
                return this.LeftNodeIsSet && this.RightNodeIsSet;
            }
        }
        public bool LeftNodeIsSet
        {
            get
            {
                return this.Left != null;
            }
        }
        public bool RightNodeIsSet
        {
            get
            {
                return this.Right != null;
            }
        }
    }
}
