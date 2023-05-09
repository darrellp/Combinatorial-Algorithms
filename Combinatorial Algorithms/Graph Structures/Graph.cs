using System.Diagnostics;
using Combinatorial_Algorithms.Graph_Algorithms;

namespace Combinatorial_Algorithms.Graph_Structures
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A concrete implementation of IGraph. </summary>
    ///
    /// <remarks>   Note that the generic parameters here represent the types of tags on vertices/edges
    ///             respectively whereas the same names in IGraph represent the actual types of
    ///             vertices/edges.  TTV/TTE stands for Type of Tagged Vertex/Edge
    ///             Darrell Plank, 5/6/2023. </remarks>
    ///
    /// <typeparam name="TTV">   Type of the tagged vertex. </typeparam>
    /// <typeparam name="TTE">   Type of the tagged edge. </typeparam>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Graph<TTV, TTE> : IGraph<ITaggedVertex<TTV>, ITaggedEdge<TTE>>
    {
        #region Private variables
        // Keys are vertex IDs, values are the vertices themselves
        private readonly Dictionary<int, Vertex<TTV, TTE>> _mapIdToVtx = new();
        internal const int VidIllegal = -1;
        internal int _vidNext = 0;
        #endregion

        #region Properties
        public int VertexCount => _mapIdToVtx.Count;

        public IEnumerable<ITaggedVertex<TTV>> Vertices => _mapIdToVtx.Values;
        #endregion

        #region Accessors
        internal Vertex<TTV, TTE>? VertexFromId(int id)
        {
            if (!_mapIdToVtx.ContainsKey(id))
            {
                return null;
            }

            return _mapIdToVtx[id];
        }

        public TTV? GetVertexAttr(int id)
        {
            var v = VertexFromId(id);
            return v == null ? default : v.Attr;
        }

        public int InEdgeCount(int id)
        {
            var v = VertexFromId(id);
            return v == null ? -1 : v.EdgesTo.Count;
        }

        public int OutEdgeCount(int id)
        {
            var v = VertexFromId(id);
            return v == null ? -1 : v.EdgesFrom.Count;
        }
        #endregion

        #region Insertion/Deletion
        public Vertex<TTV, TTE> InsertVertex(string? label = null, TTV? attr = default)
        {
            // Every vertex is assigned a numeric ID which it maintains through it's life.  This makes it easier
            // to search for and identify individual vertices.  That id is assigned here.

            var vtx = new Vertex<TTV, TTE> { ID = _vidNext++, Attr = attr, Label = label};
            _mapIdToVtx.Add(vtx.ID, vtx);
            return vtx;
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public int InsertVertices(int cvtx, string? label = null, TTV? vattr = default)
        {
            var vid = InsertVertex(label == null ? null : $"{label}0", vattr).ID;

            for (var i = 0; i < cvtx - 1; i++)
            {
                InsertVertex(label == null ? null : $"{label}{i}", vattr);
            }

            return vid;
        }

        public void AddDoubleEdge(Vertex<TTV, TTE> v1, Vertex<TTV, TTE> v2, TTE? attr = default)
        {
            AddEdge(v1, v2, attr);
            AddEdge(v2, v1, attr);
        }

        public void AddEdge(Vertex<TTV, TTE> vFrom, Vertex<TTV, TTE> vTo, TTE? attr = default)
        {
            AddEdge(vFrom.ID, vTo.ID, attr);
        }

        public void AddDoubleEdge(int v1, int v2, TTE? attr = default)
        {
            AddEdge(v1, v2, attr);
            AddEdge(v2, v1, attr);
        }

        public void AddEdge(int vidFrom, int vidTo, TTE? attr = default)
        {
            var edge = new Edge<TTV, TTE>(vidFrom, vidTo, this);
            var vtxFrom = VertexFromId(vidFrom);
            var vtxTo = VertexFromId(vidTo);
            edge.Attr = attr;
            try
            {
                if (vtxFrom == null || vtxTo == null)
                {
                    throw new InvalidDataException();
                }

                vtxFrom.IdDestToEdge[vidTo] = edge;
                vtxFrom.EdgesFrom.Add(edge);
                vtxTo.EdgesTo.Add(edge);
            }
            catch (Exception)
            {
                VfException.Error("Inconsistent Data");
            }
        }

        public void DeleteVertex(int vid)
        {
            var vtx = VertexFromId(vid);
            if (vtx == null)
            {
                VfException.Error("Trying to delete nonexistent vertex");
            }

            Debug.Assert(vtx != null, nameof(vtx) + " != null");
            var arend = new Edge<TTE, TTV>[vtx.EdgesFrom.Count + vtx.EdgesTo.Count];
            vtx.EdgesFrom.CopyTo(arend, 0);
            vtx.EdgesTo.CopyTo(arend, vtx.EdgesFrom.Count);

            foreach (var end in arend)
            {
                DeleteEdge(end.IDFrom, end.IDTo);
            }

            _mapIdToVtx.Remove(vtx.ID);
        }

        public void DeleteEdge(int vidFrom, int vidTo)
        {
            var vtxFrom = VertexFromId(vidFrom);
            var vtxTo = VertexFromId(vidTo);
            var edge = vtxFrom?.EdgesFrom.FirstOrDefault(e => ReferenceEquals(vtxTo, e.EndVertex));

            if (edge == null)
            {
                VfException.Error("Inconsistent Data");
            }

            Debug.Assert(vtxFrom != null, nameof(vtxFrom) + " != null");
            Debug.Assert(vtxTo != null, nameof(vtxTo) + " != null");
            Debug.Assert(edge != null, nameof(edge) + " != null");

            vtxFrom.IdDestToEdge.Remove(vidTo);
            vtxTo.EdgesTo.Remove(edge);
            vtxFrom.EdgesFrom.Remove(edge);
        }
        #endregion
    }

    public class Graph : Graph<object, object>
    {
    }
}
