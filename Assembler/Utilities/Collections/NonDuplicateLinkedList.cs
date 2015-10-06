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
            else
            {
                foreach (var currentNodeItem in this.LinkedList)
                {
                    if (item.IsEqual(currentNodeItem))
                    {
                        return item;
                    }
                }

                this.LinkedList.AddLast(item);
            }

            return default(T);
        }

        public bool Exists(T obj, Func<T,T, bool> areEqual)
        {
            if (this.LinkedList == null)
            {
                return false;
            }

            foreach (var item in this.LinkedList)
            {
                if (areEqual(item, obj))
                {
                    return true;
                }
            }

            return false;
        }

        public T Find(T obj, Func<T, T, bool> areEqual)
        {
            if (this.LinkedList == null)
            {
                return default(T);
            }

            foreach (var item in this.LinkedList)
            {
                if (areEqual(item, obj))
                {
                    return item;
                }
            }

            return default(T);
        }

        public IEnumerator GetEnumerator()
        {
            return this.LinkedList.GetEnumerator();
        }
    }
}