namespace Combinatorial_Algorithms.Graph_Algorithms
{
    public static class StronglyConnected
    {
        // Finds the strongly connected components of a digraph using an iterative version of Tarjan's algorithm.
        public static List<List<TV>> Components<TV, TE>(IGraph<TV, TE> graph)
            where TV : IVertex
            where TE : IEdge
        {
            var index = new Dictionary<IVertex, int>();
            var lowLink = new Dictionary<IVertex, int>();
            var onStack = new HashSet<IVertex>();
            var sccStack = new Stack<IVertex>();
            var callStack = new Stack<(IVertex Vertex, int NextEdge)>();
            var components = new List<List<TV>>();
            var nextIndex = 0;

            foreach (var root in graph.Vertices)
            {
                if (index.ContainsKey(root))
                {
                    continue;
                }

                index[root] = lowLink[root] = nextIndex++;
                sccStack.Push(root);
                onStack.Add(root);
                callStack.Push((root, 0));

                while (callStack.Count > 0)
                {
                    var (vtx, iEdge) = callStack.Pop();

                    if (iEdge < vtx.EdgesFrom.Count)
                    {
                        callStack.Push((vtx, iEdge + 1));
                        var next = vtx.EdgesFrom[iEdge].EndVertex;

                        if (!index.ContainsKey(next))
                        {
                            index[next] = lowLink[next] = nextIndex++;
                            sccStack.Push(next);
                            onStack.Add(next);
                            callStack.Push((next, 0));
                        }
                        else if (onStack.Contains(next))
                        {
                            lowLink[vtx] = Math.Min(lowLink[vtx], index[next]);
                        }
                        continue;
                    }

                    // All edges processed - vtx is finished
                    if (lowLink[vtx] == index[vtx])
                    {
                        var component = new List<TV>();
                        IVertex member;
                        do
                        {
                            member = sccStack.Pop();
                            onStack.Remove(member);
                            component.Add((TV)member);
                        } while (!ReferenceEquals(member, vtx));
                        components.Add(component);
                    }

                    if (callStack.Count > 0)
                    {
                        var parent = callStack.Peek().Vertex;
                        lowLink[parent] = Math.Min(lowLink[parent], lowLink[vtx]);
                    }
                }
            }

            return components;
        }

        public static bool IsStronglyConnected<TV, TE>(IGraph<TV, TE> graph)
            where TV : IVertex
            where TE : IEdge
        {
            return Components(graph).Count <= 1;
        }
    }
}
