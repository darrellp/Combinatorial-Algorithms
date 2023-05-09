namespace Combinatorial_Algorithms.Graph_Algorithms
{
    public interface IGraph <TV, TE> where TV : IVertex where TE : IEdge
    {
        IEnumerable<TV> Vertices { get; }
    }

    public interface IVertex
    {
        List<IEdge> EdgesFrom { get; }
        List<IEdge> EdgesTo { get; }
    }

    public interface IEdge
    {
        IVertex OtherEnd(IVertex nThis) => nThis == StartVertex ? EndVertex : StartVertex;
        IVertex StartVertex { get; }
        IVertex EndVertex { get; }
    }

    public interface ITaggedVertex<T> : IVertex, ITagged<T> {}
    public interface ITaggedEdge<T> : IEdge, ITagged<T> {}
}