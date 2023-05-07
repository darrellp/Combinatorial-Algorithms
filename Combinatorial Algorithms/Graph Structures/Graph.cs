using System.Diagnostics;

namespace Combinatorial_Algorithms.Graph_Structures
{

    public class Graph<TV, TE> : IGraph<ITaggedVertex<TV>, ITaggedEdge<TE>>
    {
        #region Private variables
        // Keys are vertex IDs, values are the vertices themselves
        private readonly Dictionary<int, Vertex<TV, TE>> _mapIdToVtx = new();
        internal const int VidIllegal = -1;
        internal int _vidNext = 0;
        #endregion

        #region Properties
        public int VertexCount => _mapIdToVtx.Count;

        public IEnumerable<ITaggedVertex<TV>> Vertices => _mapIdToVtx.Values;
        #endregion

        #region Accessors
        internal Vertex<TV, TE>? VertexFromId(int id)
        {
            if (!_mapIdToVtx.ContainsKey(id))
            {
                return null;
            }

            return _mapIdToVtx[id];
        }

        public TV? GetVertexAttr(int id)
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
        public Vertex<TV, TE> InsertVertex(string? label = null, TV? attr = default)
        {
            // Every vertex is assigned a numeric ID which it maintains through it's life.  This makes it easier
            // to search for and identify individual vertices.  That id is assigned here.

            var vtx = new Vertex<TV, TE> { ID = _vidNext++, Attr = attr, Label = label};
            _mapIdToVtx.Add(vtx.ID, vtx);
            return vtx;
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public int InsertVertices(int cvtx, string? label = null, TV? vattr = default)
        {
            var vid = InsertVertex(label == null ? null : $"{label}0", vattr).ID;

            for (var i = 0; i < cvtx - 1; i++)
            {
                InsertVertex(label == null ? null : $"{label}{i}", vattr);
            }

            return vid;
        }

        public void AddDoubleEdge(Vertex<TV, TE> v1, Vertex<TV, TE> v2, TE? attr = default)
        {
            AddEdge(v1, v2, attr);
            AddEdge(v2, v1, attr);
        }

        public void AddEdge(Vertex<TV, TE> vFrom, Vertex<TV, TE> vTo, TE? attr = default)
        {
            AddEdge(vFrom.ID, vTo.ID, attr);
        }

        public void AddDoubleEdge(int v1, int v2, TE? attr = default)
        {
            AddEdge(v1, v2, attr);
            AddEdge(v2, v1, attr);
        }

        public void AddEdge(int vidFrom, int vidTo, TE? attr = default)
        {
            var edge = new Edge<TV, TE>(vidFrom, vidTo, this);
            var vtxFrom = VertexFromId(vidFrom);
            var vtxTo = VertexFromId(vidTo);
            edge.Attr = attr;
            try
            {
                if (vtxFrom == null || vtxTo == null)
                {
                    throw new InvalidDataException();
                }

                vtxFrom.IdDestToEdge.Add(vidTo, edge);
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
            var arend = new Edge<TE, TV>[vtx.EdgesFrom.Count + vtx.EdgesTo.Count];
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
