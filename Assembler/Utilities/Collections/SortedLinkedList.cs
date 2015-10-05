using SIC.Assembler.Utilities.Extensions;
using System;
using System.Collections.Generic;

namespace SIC.Assembler.Utilities.Collections
{
    public class SortedLinkedList<T> where T : IComparable
    {
        private LinkedList<T> LinkedList { get; set; }

        public void Add(T item)
        {
            if (this.LinkedList == null)
            {
                this.LinkedList = new LinkedList<T>(new T[] { item });
            }
            else if (item.IsLessThan(this.LinkedList.First.Value))
            {
                this.LinkedList.AddFirst(item);
            }
            else if (item.IsGreaterThan(this.LinkedList.Last.Value))
            {
                this.LinkedList.AddLast(item);
            }
            else
            {
                foreach (var node in this.LinkedList)
                {
                    if (item.IsLessThan(node))
                    {
                        this.LinkedList.AddBefore(this.LinkedList.Find(node), item);
                        break;
                    }
                }
            }
        }
    }
}