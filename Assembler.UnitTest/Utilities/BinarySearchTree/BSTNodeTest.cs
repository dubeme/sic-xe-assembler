using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIC.Assembler.Utilities.Model;

namespace Assembler.UnitTest.Utilities.BinarySearchTree
{
    [TestClass]
    public class BSTNodeTest
    {
        [TestMethod]
        public void BSTNode_LeftRightNotSet()
        {
            BSTNode<int> node = TreeUtilities.MockBSTNode(1);

            Assert.IsFalse(node.LeftNodeIsSet, "Left node is not set");
            Assert.IsFalse(node.RightNodeIsSet, "Right node is not set");
            Assert.IsFalse(node.LeftOrRightNodeIsSet, "Left OR right node is not set");
            Assert.IsFalse(node.LeftAndRightNodeIsSet, "Left AND right node is not set");
        }

        [TestMethod]
        public void BSTNode_LeftRightSet()
        {
            BSTNode<int> node = TreeUtilities.MockBSTNode(value: 1, left: new BSTNode<int>(), right: new BSTNode<int>());

            Assert.IsTrue(node.LeftNodeIsSet, "Left node is set");
            Assert.IsTrue(node.RightNodeIsSet, "Right node is set");
            Assert.IsTrue(node.LeftOrRightNodeIsSet, "Left AND right node is set");
            Assert.IsTrue(node.LeftAndRightNodeIsSet, "Left AND right node is set");
        }

        [TestMethod]
        public void BSTNode_LeftSet()
        {
            BSTNode<int> node = TreeUtilities.MockBSTNode(value: 1, left: new BSTNode<int>());

            Assert.IsTrue(node.LeftNodeIsSet, "Left node is set");
            Assert.IsFalse(node.RightNodeIsSet, "Right node is not set");
            Assert.IsTrue(node.LeftOrRightNodeIsSet, "Left node is set");
            Assert.IsFalse(node.LeftAndRightNodeIsSet, "Left AND right node is not set");
        }

        [TestMethod]
        public void BSTNode_RightSet()
        {
            BSTNode<int> node = TreeUtilities.MockBSTNode(value: 1, right: new BSTNode<int>());

            Assert.IsFalse(node.LeftNodeIsSet, "Left node is not set");
            Assert.IsTrue(node.RightNodeIsSet, "Right node is set");
            Assert.IsTrue(node.LeftOrRightNodeIsSet, "Right node is set");
            Assert.IsFalse(node.LeftAndRightNodeIsSet, "Left AND right node is not set");
        }
    }
}