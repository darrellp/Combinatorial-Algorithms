using Combinatorial_Algorithms.Graph_Algorithms;
using Combinatorial_Algorithms.Graph_Structures;

namespace CATests
{
    [TestClass]
    public class TravellingSalesmanTests
    {
        private static Graph<object, IDistance> Complete(double[,] d)
        {
            var n = d.GetLength(0);
            var g = new Graph<object, IDistance>();
            g.InsertVertices(n);
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        g.AddEdge(i, j, (DstStr)d[i, j]);
                    }
                }
            }
            return g;
        }

        [TestMethod]
        public void SquareTour()
        {
            // Points on a unit square: 0(0,0) 1(1,0) 2(1,1) 3(0,1).  Best tour has length 4.
            var d = new double[,]
            {
                { 0, 1, Math.Sqrt(2), 1 },
                { 1, 0, 1, Math.Sqrt(2) },
                { Math.Sqrt(2), 1, 0, 1 },
                { 1, Math.Sqrt(2), 1, 0 },
            };
            var sol = TravellingSalesman.Solve(Complete(d));
            Assert.IsNotNull(sol);
            Assert.AreEqual(4, sol.TotalDistance, 1e-6);
            Assert.AreEqual(4, sol.Tour.Count);
            Assert.AreEqual(4, sol.Tour.Select(v => ((Vertex<object, IDistance>)v).ID).Distinct().Count());
        }

        [TestMethod]
        public void AsymmetricTour()
        {
            var d = new double[,]
            {
                { 0, 1, 10 },
                { 10, 0, 1 },
                { 1, 10, 0 },
            };
            var sol = TravellingSalesman.Solve(Complete(d));
            Assert.IsNotNull(sol);
            Assert.AreEqual(3, sol.TotalDistance, 1e-6);
        }

        [TestMethod]
        public void NoTourWhenNotHamiltonian()
        {
            var g = new Graph<object, IDistance>();
            g.InsertVertices(3);
            g.AddEdge(0, 1, (DstStr)1);
            g.AddEdge(1, 2, (DstStr)1);
            Assert.IsNull(TravellingSalesman.Solve(g));
        }

        [TestMethod]
        public void DjikstraGraphTour()
        {
            // Same graph as in the Djikstra test.  Several vertices have degree 2 (C, F, G, E) which forces
            // most of the tour: S-C-L-(I,J)-K-E-G-H-F-D-A-B-S with total length 45.
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

            var sol = TravellingSalesman.Solve(gr, 2);
            Assert.IsNotNull(sol);
            Assert.AreEqual(13, sol.Tour.Count);
            Assert.AreEqual(13, sol.Tour.Select(v => ((Vertex<object, IDistance>)v).ID).Distinct().Count());
            Assert.AreEqual(45, sol.TotalDistance, 1e-6);
        }

        [TestMethod]
        public void SingleVertex()
        {
            var g = new Graph<object, IDistance>();
            g.InsertVertex();
            var sol = TravellingSalesman.Solve(g);
            Assert.IsNotNull(sol);
            Assert.AreEqual(1, sol.Tour.Count);
            Assert.AreEqual(0, sol.TotalDistance);
        }
    }
}
