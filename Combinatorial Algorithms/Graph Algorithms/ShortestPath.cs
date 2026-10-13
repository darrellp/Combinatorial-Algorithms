using Google.OrTools.LinearSolver;

namespace Combinatorial_Algorithms.Graph_Algorithms
{
    public static class ShortestPath
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Searches for the shortest path using OR-Tools' linear solver. </summary>
        ///
        /// <remarks>   Edges are directed and must carry an IDistance tag with a non-negative distance.
        ///             The problem is modelled as a minimum cost flow of one unit from vtxStart to
        ///             vtxEnd: each edge gets a [0,1] flow variable, every vertex (other than the
        ///             start/end) must have balanced in/out flow, and the objective minimizes total
        ///             distance.  Since this network's incidence matrix is totally unimodular the LP
        ///             relaxation (solved with GLOP) naturally yields a 0/1 optimal solution, so no
        ///             integer programming is required.
        ///             Darrell Plank, 5/6/2023. </remarks>
        ///
        /// <param name="graph">    The graph. </param>
        /// <param name="vtxStart"> Starting vertex. </param>
        /// <param name="vtxEnd">   Ending vertex. </param>
        ///
        /// <returns>   The found shortest path or null if vtxEnd isn't reachable from vtxStart. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<TV>? FindShortestPath<TV, TE>(
            IGraph<TV, TE> graph,
            TV vtxStart,
            TV vtxEnd)
            where TV : IVertex
            where TE : ITaggedEdge<IDistance>
        {
            if (vtxStart.Equals(vtxEnd))
            {
                return new List<TV> { vtxStart };
            }

            var solver = Solver.CreateSolver("GLOP");
            if (solver == null)
            {
                throw new InvalidOperationException("Could not create GLOP solver");
            }

            // Collect edges along with the variable representing whether they're used in the path
            var edgeVars = new List<(TV from, TE edge, Variable flow)>();
            var flowByVertex = new Dictionary<TV, (List<Variable> outFlows, List<Variable> inFlows)>();

            TV VertexFor(IVertex v) => (TV)v;

            foreach (var vtx in graph.Vertices)
            {
                flowByVertex[vtx] = (new List<Variable>(), new List<Variable>());
            }

            var objective = solver.Objective();
            objective.SetMinimization();

            var iEdge = 0;
            foreach (var vtx in graph.Vertices)
            {
                foreach (var edge in vtx.EdgesFrom.Cast<TE>())
                {
                    var vtxTo = VertexFor(edge.EndVertex);
                    if (vtx.Equals(vtxTo))
                    {
                        // Self loops can never help find a shortest path
                        continue;
                    }

                    var flow = solver.MakeNumVar(0, 1, $"e{iEdge++}");
                    edgeVars.Add((vtx, edge, flow));
                    flowByVertex[vtx].outFlows.Add(flow);
                    flowByVertex[vtxTo].inFlows.Add(flow);
                    objective.SetCoefficient(flow, edge.Tag!.Distance);
                }
            }

            foreach (var (vtx, (outFlows, inFlows)) in flowByVertex)
            {
                // Net flow is +1 at the start, -1 at the end and 0 everywhere else
                double supply = vtx.Equals(vtxStart) ? 1 : vtx.Equals(vtxEnd) ? -1 : 0;
                var constraint = solver.MakeConstraint(supply, supply, $"v{vtx.GetHashCode()}");
                foreach (var flow in outFlows)
                {
                    constraint.SetCoefficient(flow, 1);
                }
                foreach (var flow in inFlows)
                {
                    constraint.SetCoefficient(flow, -1);
                }
            }

            var result = solver.Solve();
            if (result != Solver.ResultStatus.OPTIMAL)
            {
                // No feasible flow means vtxEnd isn't reachable from vtxStart
                return null;
            }

            // Walk the chosen edges from vtxStart to vtxEnd, following whichever used edge leaves
            // each vertex.  The LP relaxation is integral here so flow values will be (near) 0 or 1.
            var edgesFrom = edgeVars
                .Where(t => t.flow.SolutionValue() > 0.5)
                .ToLookup(t => t.from, t => t.edge);

            var path = new List<TV> { vtxStart };
            var vtxCur = vtxStart;
            var visited = new HashSet<TV> { vtxStart };

            while (!vtxCur.Equals(vtxEnd))
            {
                var edge = edgesFrom[vtxCur].FirstOrDefault();
                if (edge == null)
                {
                    // Shouldn't happen given an OPTIMAL result, but guard against malformed graphs
                    return null;
                }

                vtxCur = VertexFor(edge.EndVertex);
                if (!visited.Add(vtxCur))
                {
                    // Defend against cycles of zero-weight edges
                    return null;
                }
                path.Add(vtxCur);
            }

            return path;
        }
    }
}
