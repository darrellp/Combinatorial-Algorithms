using Combinatorial_Algorithms.Graph_Algorithms;
using Combinatorial_Algorithms.Graph_Structures;

namespace CATests
{
    [TestClass]
    public class StronglyConnectedTests
    {
        private static int[][] Sccs(Graph g) =>
            StronglyConnected.Components(g)
                .Select(c => c.Cast<Vertex<object, object>>().Select(v => v.ID).OrderBy(i => i).ToArray())
                .OrderBy(a => a[0])
                .ToArray();

        [TestMethod]
        public void EmptyGraph()
        {
            var g = new Graph();
            Assert.AreEqual(0, StronglyConnected.Components(g).Count);
            Assert.IsTrue(StronglyConnected.IsStronglyConnected(g));
        }

        [TestMethod]
        public void Cycle()
        {
            var g = new Graph();
            g.InsertVertices(3);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            Assert.IsTrue(StronglyConnected.IsStronglyConnected(g));
            Assert.AreEqual(1, StronglyConnected.Components(g).Count);
        }

        [TestMethod]
        public void PathIsAllSingletons()
        {
            var g = new Graph();
            g.InsertVertices(3);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            Assert.IsFalse(StronglyConnected.IsStronglyConnected(g));
            Assert.AreEqual(3, StronglyConnected.Components(g).Count);
        }

        [TestMethod]
        public void SharedTargetIsNotStronglyConnected()
        {
            var g = new Graph();
            g.InsertVertices(3);
            g.AddEdge(0, 1);
            g.AddEdge(2, 1);
            Assert.AreEqual(3, StronglyConnected.Components(g).Count);
        }

        [TestMethod]
        public void TwoCyclesJoinedByBridge()
        {
            var g = new Graph();
            g.InsertVertices(6);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            g.AddEdge(2, 3);
            g.AddEdge(3, 4);
            g.AddEdge(4, 5);
            g.AddEdge(5, 3);
            var sccs = Sccs(g);
            Assert.AreEqual(2, sccs.Length);
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, sccs[0]);
            CollectionAssert.AreEqual(new[] { 3, 4, 5 }, sccs[1]);
        }

        [TestMethod]
        public void LongPathDoesNotOverflowStack()
        {
            var g = new Graph();
            const int n = 100000;
            g.InsertVertices(n);
            for (var i = 0; i < n - 1; i++)
            {
                g.AddEdge(i, i + 1);
            }
            g.AddEdge(n - 1, 0);
            Assert.IsTrue(StronglyConnected.IsStronglyConnected(g));
        }
    }
}
