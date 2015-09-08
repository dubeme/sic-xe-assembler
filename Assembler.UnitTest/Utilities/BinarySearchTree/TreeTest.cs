using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIC.Assembler.Utilities.BinarySearchTree;

namespace Assembler.UnitTest.Utilities.BinarySearchTree
{
    [TestClass]
    public class TreeTest
    {
        [TestMethod]
        public void CreateTree()
        {
            string printOut = "";
            var root = TreeUtilities.MockTree<int>();
            root.Insert(5);
            root.Insert(3);
            root.Insert(4);
            root.Insert(2);
            root.Insert(1);
            root.Insert(8);
            root.Insert(6);
            root.Insert(9);
            root.Insert(7);

            root.Print(TraverseOrder.InOrder, printFunction: (item) =>
            {
                printOut += item;
            });

            Assert.AreEqual("123456789", printOut);
        }

        [TestMethod]
        public void DeleteNode()
        {
        }

        [TestMethod]
        public void FindByKey()
        {
        }

        [TestMethod]
        public void FindByValue()
        {
        }

        [TestMethod]
        public void PrintTree()
        {
        }
    }
}