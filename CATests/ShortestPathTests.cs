using Combinatorial_Algorithms.Graph_Algorithms;
using Combinatorial_Algorithms.Graph_Structures;

namespace CATests
{
    [TestClass]
    public class ShortestPathTests
    {
        [TestMethod]
        public void TestShortestPath()
        {
            // Using the same graph as DjikstraTests, seen in this video:
            // https://www.youtube.com/watch?v=GazC3A4OQTE&ab_channel=Computerphile

            var gr = new Graph<object, IDistance>();

            var s = gr.InsertVertex("S");
            var a = gr.InsertVertex("A");
            var b = gr.InsertVertex("B");
            var c = gr.InsertVertex("C");
            var d = gr.InsertVertex("D");
            var e = gr.InsertVertex("E");
            var f = gr.InsertVertex("F");
            var g = gr.InsertVertex("G");
            var h = gr.InsertVertex("H");
            var i = gr.InsertVertex("I");
            var j = gr.InsertVertex("J");
            var k = gr.InsertVertex("K");
            var l = gr.InsertVertex("L");
            gr.AddDoubleEdge(s, a, new DstStr(7));
            gr.AddDoubleEdge(s, b, new DstStr(2));
            gr.AddDoubleEdge(a, b, new DstStr(3));
            gr.AddDoubleEdge(a, d, new DstStr(4));
            gr.AddDoubleEdge(b, d, new DstStr(4));
            gr.AddDoubleEdge(b, h, new DstStr(1));
            gr.AddDoubleEdge(d, f, new DstStr(5));
            gr.AddDoubleEdge(h, f, new DstStr(3));
            gr.AddDoubleEdge(h, g, new DstStr(2));
            gr.AddDoubleEdge(g, e, new DstStr(2));
            gr.AddDoubleEdge(e, k, new DstStr(5));
            gr.AddDoubleEdge(k, i, new DstStr(4));
            gr.AddDoubleEdge(k, j, new DstStr(4));
            gr.AddDoubleEdge(i, j, new DstStr(6));
            gr.AddDoubleEdge(i, l, new DstStr(4));
            gr.AddDoubleEdge(j, l, new DstStr(4));
            gr.AddDoubleEdge(l, c, new DstStr(2));
            gr.AddDoubleEdge(c, s, new DstStr(3));
            var path = ShortestPath.FindShortestPath(gr, s, e);
            Assert.IsNotNull(path);
            Assert.AreEqual(5, path.Count);
            Assert.AreEqual(((Vertex<object, IDistance>)path[0]).Label, "S");
            Assert.AreEqual(((Vertex<object, IDistance>)path[1]).Label, "B");
            Assert.AreEqual(((Vertex<object, IDistance>)path[2]).Label, "H");
            Assert.AreEqual(((Vertex<object, IDistance>)path[3]).Label, "G");
            Assert.AreEqual(((Vertex<object, IDistance>)path[4]).Label, "E");
        }

        [TestMethod]
        public void TestUnreachableReturnsNull()
        {
            var gr = new Graph<object, IDistance>();
            var s = gr.InsertVertex("S");
            var t = gr.InsertVertex("T");
            gr.InsertVertex("Isolated");
            gr.AddEdge(s, t, new DstStr(1));

            var isolated = gr.Vertices.First(v => ((Vertex<object, IDistance>)v).Label == "Isolated");
            var path = ShortestPath.FindShortestPath(gr, s, isolated);
            Assert.IsNull(path);
        }

        [TestMethod]
        public void TestStartEqualsEnd()
        {
            var gr = new Graph<object, IDistance>();
            var s = gr.InsertVertex("S");
            var path = ShortestPath.FindShortestPath(gr, s, s);
            Assert.IsNotNull(path);
            Assert.AreEqual(1, path.Count);
        }
    }
}
