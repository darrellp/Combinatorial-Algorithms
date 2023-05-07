using System.Diagnostics;
using System.Linq;
using Combinatorial_Algorithms.Graph_Algorithms;
using Combinatorial_Algorithms.Graph_Structures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CATests
{
    [TestClass]
    public class DjikstraTests
    {
        [TestMethod]
        public void TestDjikstra()
        {
            // Using the graph seen in this video:
            // https://www.youtube.com/watch?v=GazC3A4OQTE&ab_channel=Computerphile
            
            var gr = new Graph<object, double>();

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
            gr.AddDoubleEdge(s, a, 7);
            gr.AddDoubleEdge(s, b, 2);
            gr.AddDoubleEdge(a, b, 3);
            gr.AddDoubleEdge(a, d, 4);
            gr.AddDoubleEdge(b, d, 4);
            gr.AddDoubleEdge(b, h, 1);
            gr.AddDoubleEdge(d, f, 5);
            gr.AddDoubleEdge(h, f, 3);
            gr.AddDoubleEdge(h, g, 2);
            gr.AddDoubleEdge(g, e, 2);
            gr.AddDoubleEdge(e, k, 5);
            gr.AddDoubleEdge(k, i, 4);
            gr.AddDoubleEdge(k, j, 4);
            gr.AddDoubleEdge(i, j, 6);
            gr.AddDoubleEdge(i, l, 4);
            gr.AddDoubleEdge(j, l, 4);
            gr.AddDoubleEdge(l, c, 2);
            gr.AddDoubleEdge(c, s, 3);
            var grType = gr as IGraph<Vertex<object, double>, Edge<object, double>>;
            var path = Djikstra.FindShortestPath(grType, s, e);
            Assert.IsNotNull(path);
            Assert.AreEqual(((Vertex<object, double>)path[0]).Label, "S");
            Assert.AreEqual(((Vertex<object, double>)path[1]).Label, "B");
            Assert.AreEqual(((Vertex<object, double>)path[2]).Label, "H");
            Assert.AreEqual(((Vertex<object, double>)path[3]).Label, "G");
            Assert.AreEqual(((Vertex<object, double>)path[4]).Label, "E");
        }
    }
}
