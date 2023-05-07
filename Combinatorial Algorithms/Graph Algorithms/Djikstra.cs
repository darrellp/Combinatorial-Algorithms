using System.Diagnostics;
using Combinatorial_Algorithms.Graph_Structures;
using Combinatorial_Algorithms.PriorityQueue;
using TV = Combinatorial_Algorithms.Graph_Structures.ITaggedVertex<object>;
using TE = Combinatorial_Algorithms.Graph_Structures.ITaggedEdge<double>;

namespace Combinatorial_Algorithms.Graph_Algorithms
{
    public static class Djikstra
    {
        private static readonly Dictionary<IVertex, VertexProxy> MapTnProxy = new();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Searches for the shortest path. </summary>
        ///
        /// <remarks>   Darrell Plank, 5/6/2023. </remarks>
        ///
        /// <param name="graph">    The graph. </param>
        /// <param name="vtxStart"> Starting vertex. </param>
        /// <param name="vtxEnd">   Ending vertex. </param>
        ///
        /// <returns>   The found shortest path or null if vtxEnd isn't reachable from vtxStart. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<TV>? FindShortestPath(
            IGraph<TV,  TE> graph, 
            TV vtxStart, 
            TV vtxEnd)
        {
            var ret = new List<TV>();
            var pq = new PriorityQueueWithDeletions<VertexProxy>();
            MapTnProxy.Clear();

            var start = new VertexProxy(vtxStart)
            {
                Distance = 0
            };
            pq.Add(start);
            MapTnProxy[vtxStart] = start;

            foreach (var vtx in graph.Vertices.Where(n => !n.Equals(vtxStart)))
            {
                var proxy = new VertexProxy(vtx);
                pq.Add(proxy);
                MapTnProxy[vtx] = proxy;
            }

            while (true)
            {
                if (pq.Count == 0)
                {
                    // Dang!  End vertex doesn't seem reachable from start vertex!
                    return null;
                }
                var proxyCur = pq.Pop();
                if (proxyCur.Vertex.Equals(vtxEnd))
                {
                    break;
                }

                proxyCur.IsFinal = true;
                var distCur = proxyCur.Distance;
                var vtxCur = proxyCur.Vertex;
                foreach (var edgeCur in vtxCur.EdgesFrom.Select(edge => edge as ITaggedEdge<double>))
                {
                    Debug.Assert(edgeCur != null);
                    var vtx = edgeCur.EndVertex;
                    var proxy = MapTnProxy[vtx];
                    if (proxy.IsFinal)
                    {
                        // We've already figured shortest distance to this vertex
                        continue;
                    }
                    var distTo = edgeCur.Tag;
                    if (distCur + distTo < proxy.Distance)
                    {
                        pq.Delete(proxy);
                        proxy.Distance = distCur + distTo;
                        proxy.Previous = proxyCur;
                        pq.Add(proxy);
                    }
                }
            }

            var proxyList = MapTnProxy[vtxEnd];
            while (proxyList.Previous != null)
            {
                ret.Add(proxyList.Vertex);
                proxyList = proxyList.Previous;
            }
            Debug.Assert(vtxStart.Equals(proxyList.Vertex));
            ret.Add(proxyList.Vertex);
            ret.Reverse();

            return ret;
        }

        internal class VertexProxy : IPriorityQueueElement
        {
            internal TV Vertex { get; }
            internal double Distance { get; set; }
            internal VertexProxy? Previous { get; set; }
            internal bool IsFinal { get; set; }

            public VertexProxy(TV vertex)
            {
                Vertex = vertex;
                Distance = double.MaxValue;
                Previous = null;
                IsFinal = false;
            }

            public int CompareTo(object? obj)
            {
                if (obj is not VertexProxy proxyOther)
                {
                    throw new ArgumentException("obj not VertexProxy in CompareTo");
                }
                return -Distance.CompareTo(proxyOther.Distance);
            }

            public void SetIndex(int i)
            {
                Index = i;
            }

            public int Index { get; set; } = -1;
        }
    }
}
