using SIC.Assembler.Utilities.BinarySearchTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIC.Assembler
{
    internal class Program
    {
        private static void change(List<StringBuilder> strLst)
        {
            var temp = strLst[0];
            
            temp = new StringBuilder("New value - 0");
            strLst[1] = new StringBuilder("New value - 1");
            strLst[2] = new StringBuilder("New value - 2");
        }

        private static void Main(string[] args)
        {
            int n;
            var num = int.TryParse("2323", out n);
            List<StringBuilder> strLst = new List<StringBuilder>
            {
                new StringBuilder("A"),
                new StringBuilder("B"),
                new StringBuilder("C"),
                new StringBuilder("D")
            };

            BSTNode<int> k = new BSTNode<int>
            {
                Value = 3
            };
            k.Left = new BSTNode<int>
            {
                Value = 1
            };
            k.Right = new BSTNode<int>
            {
                Value = 6
            };

            var temp = k.Right;
            temp = null;

            Console.WriteLine(string.Format("{0,-10}{1,-10}{2,-10}{3,-10}{4,-10}", 0,1,2,3,4,5));

            change(strLst);
        }
    }
}