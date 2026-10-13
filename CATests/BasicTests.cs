using Combinatorial_Algorithms.Graph_Structures;

namespace CATests
{
    [TestClass]
    public class Basics
    {
        [TestMethod]
        public void TestConstructor()
        {
            var g = new Graph();
            Assert.IsNotNull(g);
        }

        [TestMethod]
        public void TestDeleteEdge()
        {
            var gr = new Graph();
            Assert.AreEqual(0, gr.InsertVertex().ID);
            Assert.AreEqual(1, gr.InsertVertex().ID);
            Assert.AreEqual(2, gr.InsertVertex().ID);
            gr.AddEdge(0, 1);
            gr.AddEdge(1, 2);
            gr.AddEdge(2, 0);
            gr.DeleteEdge(1, 2);
            Assert.AreEqual(1, gr.OutEdgeCount(0));
            Assert.AreEqual(0, gr.OutEdgeCount(1));
            Assert.AreEqual(1, gr.OutEdgeCount(2));

            Assert.AreEqual(3, gr.InsertVertex(null, 20).ID);

            // Trigger the exception - no edge from 1 to 0...
            Assert.ThrowsException<VfException>(() => gr.DeleteEdge(1, 0));
        }

        [TestMethod]
        public void TestDeleteVertex()
        {
            var gr = new Graph();
            Assert.AreEqual(0, gr.InsertVertex().ID);
            Assert.AreEqual(1, gr.InsertVertex().ID);
            Assert.AreEqual(2, gr.InsertVertex(null, 10).ID);
            gr.AddEdge(0, 1);
            gr.AddEdge(1, 2);
            gr.AddEdge(2, 0);
            gr.DeleteVertex(0);

            Assert.AreEqual(1, gr.OutEdgeCount(1));
            Assert.AreEqual(0, gr.OutEdgeCount(2));

            Assert.AreEqual(10, gr.GetVertexAttr(2));
            Assert.AreEqual(3, gr.InsertVertex().ID);
            Assert.AreEqual(10, gr.GetVertexAttr(2));

            // Trigger the exception - shouldn't be a zero vertex any more...
            Assert.IsNull(gr.VertexFromId(0));
        }

        [TestMethod]
        public void TestFindVertexNotFound()
        {
            var gr = new Graph();
            Assert.IsNull(gr.VertexFromId(0));
        }

        [TestMethod]
        public void TestInsertEdge()
        {
            var gr = new Graph();
            var idFrom = gr.InsertVertex(null, 0).ID;
            var idTo = gr.InsertVertex(null, 1).ID;
            gr.AddEdge(idFrom, idTo, 100);
            Assert.AreEqual(gr.OutEdgeCount(idFrom), 1);
            Assert.AreEqual(gr.OutEdgeCount(idTo), 0);
            var vtxFrom = gr.VertexFromId(idFrom)!;
            var edge = (Edge<object, object>)vtxFrom.EdgesFrom.First();
            Assert.AreEqual(100, (int)edge.Tag!);
            Assert.AreEqual(idTo, edge.IDTo);

            // Try inserting the same edge twice to trigger exception...
            Assert.ThrowsException<VfException>(() => gr.AddEdge(0, 1, 200));
        }

        [TestMethod]
        public void TestInsertVertex()
        {
            var gr = new Graph();
            gr.InsertVertex(null, 1);
            var vtx = gr.VertexFromId(0);
            Assert.IsNotNull(vtx);
            Assert.IsNull(gr.VertexFromId(1));
        }
    }
}