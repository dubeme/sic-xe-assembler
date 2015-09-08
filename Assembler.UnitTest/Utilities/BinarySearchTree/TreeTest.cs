using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assembler.UnitTest.Utilities.BinarySearchTree
{
    [TestClass]
    public class TreeTest
    {
        [TestMethod]
        public void CreateTree()
        {
            var root = TreeUtilities.MockTree<int>();

            root.Insert(5);
            root.Insert(3);
            root.Insert(4);
            root.Insert(2);
            root.Insert(8);
            root.Insert(6);
            root.Insert(9);

            root.Print();

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