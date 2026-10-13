using Combinatorial_Algorithms.Graph_Algorithms;
using Combinatorial_Algorithms.Graph_Structures;

namespace CATests
{
    [TestClass]
    public class ConnectivityTests
    {
        [TestMethod]
        public void EmptyGraphIsConnected()
        {
            var g = new Graph();
            Assert.IsTrue(Connectivity.IsConnected(g));
            Assert.AreEqual(0, Connectivity.ComponentCount(g));
        }

        [TestMethod]
        public void SingleVertexIsConnected()
        {
            var g = new Graph();
            g.InsertVertex();
            Assert.IsTrue(Connectivity.IsConnected(g));
            Assert.AreEqual(1, Connectivity.ComponentCount(g));
        }

        [TestMethod]
        public void ConnectedGraph()
        {
            var g = new Graph();
            g.InsertVertices(4);
            g.AddEdge(0, 1);
            g.AddEdge(2, 1);
            g.AddEdge(2, 3);
            Assert.IsTrue(Connectivity.IsConnected(g));
        }

        [TestMethod]
        public void DisconnectedGraph()
        {
            var g = new Graph();
            g.InsertVertices(5);
            g.AddDoubleEdge(0, 1);
            g.AddEdge(2, 3);
            var components = Connectivity.ConnectedComponents(g);
            Assert.IsFalse(Connectivity.IsConnected(g));
            Assert.AreEqual(3, components.Count);
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, components.Select(c => c.Count).OrderByDescending(n => n).ToArray());
        }

        [TestMethod]
        public void DeletingVertexDisconnects()
        {
            var g = new Graph();
            g.InsertVertices(3);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            Assert.IsTrue(Connectivity.IsConnected(g));
            g.DeleteVertex(1);
            Assert.AreEqual(2, Connectivity.ComponentCount(g));
        }
    }
}
