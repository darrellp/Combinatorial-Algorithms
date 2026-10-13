using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;

namespace Combinatorial_Algorithms.Graph_Algorithms
{
    public class TspSolution<TV>
    {
        // Vertices in visiting order, beginning with the start vertex.  The return to the start isn't repeated.
        public List<TV> Tour { get; }
        public double TotalDistance { get; }

        public TspSolution(List<TV> tour, double totalDistance)
        {
            Tour = tour;
            TotalDistance = totalDistance;
        }
    }

    public static class TravellingSalesman
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Solves the travelling salesman problem using OR-Tools routing. </summary>
        ///
        /// <remarks>   Edges are directed and must carry an IDistance tag.  Missing edges are treated as
        ///             unusable.  OR-Tools works with integer costs so distances are multiplied by
        ///             <paramref name="scale"/> and rounded. </remarks>
        ///
        /// <param name="graph">            The graph. </param>
        /// <param name="timeLimitSeconds"> Time limit for the local search. </param>
        /// <param name="scale">            Multiplier used to convert distances to integers. </param>
        ///
        /// <returns>   The tour or null if no Hamiltonian cycle was found. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static TspSolution<TV>? Solve<TV, TE>(
            IGraph<TV, TE> graph,
            int timeLimitSeconds = 1,
            double scale = 1000)
            where TV : IVertex
            where TE : ITaggedEdge<IDistance>
        {
            var vertices = graph.Vertices.ToList();
            var n = vertices.Count;
            if (n == 0)
            {
                return null;
            }
            if (n == 1)
            {
                return new TspSolution<TV>(new List<TV> { vertices[0] }, 0);
            }

            var matrix = BuildCostMatrix<TV, TE>(vertices, scale, out var penalty);

            var manager = new RoutingIndexManager(n, 1, 0);
            var routing = new RoutingModel(manager);
            var transit = routing.RegisterTransitCallback((fromIndex, toIndex) =>
                matrix[manager.IndexToNode(fromIndex), manager.IndexToNode(toIndex)]);
            routing.SetArcCostEvaluatorOfAllVehicles(transit);

            var parameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            parameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            parameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            parameters.TimeLimit = new Duration { Seconds = timeLimitSeconds };

            var solution = routing.SolveWithParameters(parameters);
            if (solution == null)
            {
                return null;
            }

            var tour = new List<TV>();
            var total = 0L;
            var index = routing.Start(0);
            while (!routing.IsEnd(index))
            {
                tour.Add(vertices[manager.IndexToNode(index)]);
                var next = solution.Value(routing.NextVar(index));
                total += matrix[manager.IndexToNode(index), manager.IndexToNode(next)];
                index = next;
            }

            // Penalties are larger than any legitimate tour so a tour using one is infeasible
            if (total >= penalty)
            {
                return null;
            }

            return new TspSolution<TV>(tour, total / scale);
        }

        private static long[,] BuildCostMatrix<TV, TE>(List<TV> vertices, double scale, out long penalty)
            where TV : IVertex
            where TE : ITaggedEdge<IDistance>
        {
            var n = vertices.Count;
            var idToIndex = new Dictionary<IVertex, int>();
            for (var i = 0; i < n; i++)
            {
                idToIndex[vertices[i]] = i;
            }

            var matrix = new long[n, n];
            var present = new bool[n, n];
            long sum = 0;

            for (var i = 0; i < n; i++)
            {
                foreach (var edge in vertices[i].EdgesFrom.Cast<TE>())
                {
                    if (!idToIndex.TryGetValue(edge.EndVertex, out var j) || i == j)
                    {
                        continue;
                    }

                    var cost = (long)Math.Round(edge.Tag!.Distance * scale);
                    if (!present[i, j] || cost < matrix[i, j])
                    {
                        sum += present[i, j] ? cost - matrix[i, j] : cost;
                        matrix[i, j] = cost;
                        present[i, j] = true;
                    }
                }
            }

            penalty = sum + 1;
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (i != j && !present[i, j])
                    {
                        matrix[i, j] = penalty;
                    }
                }
            }

            return matrix;
        }
    }
}
