namespace Combinatorial_Algorithms.Graph_Structures
{

    public class Edge<TV, TE> : ITaggedEdge<TE>
    {
        public TE? Attr;
        internal int IDFrom { get; }
        internal int IDTo { get; }
        public Vertex<TV, TE> From { get; }
        public Vertex<TV, TE> To { get; }

        public Edge(int idFrom, int idTo, Graph<TV, TE> graph)
        {
            IDFrom = idFrom;
            IDTo = idTo;
            From = graph.VertexFromId(idFrom)!;
            To = graph.VertexFromId(idTo)!;
        }

        public IVertex StartVertex => From;
        public IVertex EndVertex => To;

        public TE? Tag
        {
            get => Attr;
            set => Attr = value;
        }
    }
}
