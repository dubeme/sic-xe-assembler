using SIC.Assembler.Utilities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SIC.Assembler.Utilities.Collections
{
    public class NonDuplicateLinkedList<T> : IEnumerable where T : IComparable
    {
        public int Count
        {
            get
            {
                return this.LinkedList.Count;
            }
        }

        private LinkedList<T> LinkedList { get; set; }

        public T Add(T item)
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
                foreach (var currentNodeItem in this.LinkedList)
                {
                    if (item.IsEqual(currentNodeItem))
                    {
                        return item;
                    }
                    if (item.IsLessThan(currentNodeItem))
                    {
                        this.LinkedList.AddBefore(this.LinkedList.Find(currentNodeItem), item);
                        break;
                    }
                }
            }

            return default(T);
        }

        public T Find(T obj)
        {
            return this.LinkedList.Find(obj).Value;
        }

        public IEnumerator GetEnumerator()
        {
            return this.LinkedList.GetEnumerator();
        }
    }
}