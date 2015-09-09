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
            Tree<int> root = TreeUtilities.CreateTree(5, 3, 4, 2, 1, 8, 6, 9, 7);
            string printOut = "";

            root.Print(TraverseOrder.InOrder, printFunction: (item) =>
            {
                printOut += item;
            });

            Assert.AreEqual("123456789", printOut);
        }

        [TestMethod]
        public void DeleteNode()
        {
            Tree<int> root = TreeUtilities.CreateTree(5, 3, 4, 2, 1, 8, 6, 9, 7);
            string printOut = "";

            root.Remove(5);

            root.Print(TraverseOrder.InOrder, printFunction: (item) =>
            {
                printOut += item;
            });

            Assert.AreEqual("12346789", printOut);
        }

        [TestMethod]
        public void Find()
        {
            Tree<int> root = TreeUtilities.CreateTree(5, 3, 4, 2, 1, 8, 6, 9, 7);
            Assert.IsTrue(root.FindNode(5).LeftAndRightNodeIsSet, "Left and right node should have been set (3,8)");
            Assert.IsFalse(root.FindNode(1).LeftOrRightNodeIsSet, "Left and right node shouldn't be set");
            Assert.IsNull(root.FindNode(800), "800 shouldn't exist in the tree");
        }

        [TestMethod]
        public void PrintTree_InOder()
        {
            Tree<int> root = TreeUtilities.CreateTree(5, 3, 4, 2, 1, 8, 6, 9, 7);
            string printOut = "";

            root.Remove(5);

            root.Print(TraverseOrder.InOrder, printFunction: (item) =>
            {
                printOut += item;
            });
            Assert.AreEqual("123456789", printOut);
        }

        [TestMethod]
        public void PrintTree_LevelOrder()
        {
            Tree<int> root = TreeUtilities.CreateTree(5, 3, 4, 2, 1, 8, 6, 9, 7);
            string printOut = "";

            root.Remove(5);

            root.Print(TraverseOrder.LevelOrder, printFunction: (levelItems) =>
            {
                foreach (var item in (int[])levelItems)
                {
                    printOut += item;
                }
            });
            Assert.AreEqual("538246917", printOut);
        }

        [TestMethod]
        public void PrintTree_PostOrder()
        {
            Tree<int> root = TreeUtilities.CreateTree(5, 3, 4, 2, 1, 8, 6, 9, 7);
            string printOut = "";

            root.Remove(5);

            root.Print(TraverseOrder.PostOrder, printFunction: (item) =>
            {
                printOut += item;
            });
            Assert.AreEqual("124376985", printOut);
        }

        [TestMethod]
        public void PrintTree_PreOrder()
        {
            Tree<int> root = TreeUtilities.CreateTree(5, 3, 4, 2, 1, 8, 6, 9, 7);
            string printOut = "";

            root.Remove(5);

            root.Print(TraverseOrder.PreOrder, printFunction: (item) =>
            {
                printOut += item;
            });
            Assert.AreEqual("532148679", printOut);
        }
    }
}