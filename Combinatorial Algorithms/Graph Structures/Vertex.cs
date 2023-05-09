using System.Collections;
using System.Collections.Generic;

namespace Combinatorial_Algorithms.Graph_Structures
{
    public class Vertex<TV, TE> : ITaggedVertex<TV>
    {
        public TV? Attr;

        // Key is named id of "to" vertex
        public readonly Dictionary<int, Edge<TV, TE>> IdDestToEdge = new();
        public int ID = Graph<TV, TE>.VidIllegal;
        public string? Label { get; set; } = null;

        public Edge<TV, TE>? FindOutEdge(int vidTo)
        {
            return IdDestToEdge[vidTo];
        }

        public List<IEdge> EdgesFrom { get; } = new();

        public List<IEdge> EdgesTo { get; } = new();

        public TV? Tag
        {
            get => Attr;
            set => Attr = value;
        }

        public override string ToString()
        {
            return $"{(Label == null ? string.Empty : Label)} ({ID})";
        }
    }
}

