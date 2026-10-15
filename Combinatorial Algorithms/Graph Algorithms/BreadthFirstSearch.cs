namespace Combinatorial_Algorithms.Graph_Algorithms
{
    public static class BreadthFirstSearch
    {
        // Returns all vertices in the component containing start, in breadth first order, ignoring
        // edge direction (weak connectivity).
        public static IEnumerable<TV> BreadthFirst<TV>(TV start)
            where TV : IVertex
        {
            var visited = new HashSet<IVertex> { start };
            var queue = new Queue<IVertex>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var vtx = queue.Dequeue();
                yield return (TV)vtx;

                foreach (var edge in vtx.EdgesFrom.Concat(vtx.EdgesTo))
                {
                    var other = edge.OtherEnd(vtx);
                    if (visited.Add(other))
                    {
                        queue.Enqueue(other);
                    }
                }
            }
        }
    }
}
