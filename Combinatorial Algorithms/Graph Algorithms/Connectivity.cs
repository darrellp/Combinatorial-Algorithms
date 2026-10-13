namespace Combinatorial_Algorithms.Graph_Algorithms
{
    public static class Connectivity
    {
        // Finds the connected components of a graph, ignoring edge direction (weak connectivity).
        public static List<List<TV>> ConnectedComponents<TV, TE>(IGraph<TV, TE> graph)
            where TV : IVertex
            where TE : IEdge
        {
            var visited = new HashSet<IVertex>();
            var components = new List<List<TV>>();

            foreach (var start in graph.Vertices)
            {
                if (!visited.Add(start))
                {
                    continue;
                }

                var component = new List<TV>();
                var stack = new Stack<IVertex>();
                stack.Push(start);

                while (stack.Count > 0)
                {
                    var vtx = stack.Pop();
                    component.Add((TV)vtx);

                    foreach (var edge in vtx.EdgesFrom.Concat(vtx.EdgesTo))
                    {
                        var other = edge.OtherEnd(vtx);
                        if (visited.Add(other))
                        {
                            stack.Push(other);
                        }
                    }
                }

                components.Add(component);
            }

            return components;
        }

        public static int ComponentCount<TV, TE>(IGraph<TV, TE> graph)
            where TV : IVertex
            where TE : IEdge
        {
            return ConnectedComponents(graph).Count;
        }

        // An empty graph is considered connected.
        public static bool IsConnected<TV, TE>(IGraph<TV, TE> graph)
            where TV : IVertex
            where TE : IEdge
        {
            return ComponentCount(graph) <= 1;
        }
    }
}
