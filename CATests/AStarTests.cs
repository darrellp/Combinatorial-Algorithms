using Combinatorial_Algorithms.Other_Algorihms;

namespace CATests
{
    [TestClass]
    public class AStarTests
    {
        private class GridState : AStarState
        {
            private readonly HashSet<(int, int)> _walls;
            private readonly int _size;
            public int X { get; }
            public int Y { get; }

            public GridState(int x, int y, int size, HashSet<(int, int)> walls)
            {
                X = x;
                Y = y;
                _size = size;
                _walls = walls;
            }

            public override IEnumerable<IState> Successors()
            {
                foreach (var (dx, dy) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
                {
                    var nx = X + dx;
                    var ny = Y + dy;
                    if (nx < 0 || ny < 0 || nx >= _size || ny >= _size || _walls.Contains((nx, ny)))
                    {
                        continue;
                    }
                    yield return new GridState(nx, ny, _size, _walls);
                }
            }

            public override bool IsEqual(IState istate) => istate is GridState g && g.X == X && g.Y == Y;
            public override int GetHashCode() => HashCode.Combine(X, Y);

            public override double EstDistance(IState target)
            {
                var t = (GridState)target;
                return Math.Abs(t.X - X) + Math.Abs(t.Y - Y);
            }
        }

        private class WeightedState : AStarState
        {
            private readonly Dictionary<string, Dictionary<string, double>> _graph;
            public string Name { get; }

            public WeightedState(string name, Dictionary<string, Dictionary<string, double>> graph)
            {
                Name = name;
                _graph = graph;
            }

            public override IEnumerable<IState> Successors() =>
                _graph[Name].Keys.Select(n => (IState)new WeightedState(n, _graph));

            public override bool IsEqual(IState istate) => istate is WeightedState w && w.Name == Name;
            public override int GetHashCode() => Name.GetHashCode();
            public override double SuccessorDistance(IState successor) => _graph[Name][((WeightedState)successor).Name];
        }

        private static GridState G(int x, int y, int size, HashSet<(int, int)> walls) => new(x, y, size, walls);

        [TestMethod]
        public void StartEqualsTarget()
        {
            var walls = new HashSet<(int, int)>();
            var path = new AStar<GridState>(G(2, 2, 5, walls), G(2, 2, 5, walls)).Solve();
            Assert.IsNotNull(path);
            Assert.AreEqual(1, path.Count);
        }

        [TestMethod]
        public void OpenGridShortestPath()
        {
            var walls = new HashSet<(int, int)>();
            var start = G(0, 0, 5, walls);
            var target = G(4, 4, 5, walls);
            var path = new AStar<GridState>(start, target).Solve();
            Assert.IsNotNull(path);
            Assert.AreEqual(9, path.Count);
            Assert.IsTrue(path[0].IsEqual(start));
            Assert.IsTrue(path[^1].IsEqual(target));
            for (var i = 1; i < path.Count; i++)
            {
                Assert.AreEqual(1, path[i - 1].EstDistance(path[i]));
            }
        }

        [TestMethod]
        public void PathAroundWall()
        {
            var walls = new HashSet<(int, int)> { (1, 0), (1, 1), (1, 2) };
            var path = new AStar<GridState>(G(0, 0, 4, walls), G(2, 0, 4, walls)).Solve();
            Assert.IsNotNull(path);
            Assert.AreEqual(9, path.Count);
            Assert.IsFalse(path.Any(p => walls.Contains((p.X, p.Y))));
        }

        [TestMethod]
        public void UnreachableTargetReturnsNull()
        {
            var walls = new HashSet<(int, int)> { (1, 0), (1, 1), (0, 1) };
            Assert.IsNull(new AStar<GridState>(G(0, 0, 3, walls), G(2, 2, 3, walls)).Solve());
        }

        [TestMethod]
        public void MultipleTargetsFindsNearest()
        {
            var walls = new HashSet<(int, int)>();
            var targets = new HashSet<GridState> { G(4, 4, 5, walls), G(1, 0, 5, walls) };
            var path = new AStar<GridState>(G(0, 0, 5, walls), targets).Solve();
            Assert.IsNotNull(path);
            Assert.AreEqual(2, path.Count);
            Assert.IsTrue(path[^1].IsEqual(G(1, 0, 5, walls)));
        }

        [TestMethod]
        public void WeightedGraphPrefersLowerCost()
        {
            var graph = new Dictionary<string, Dictionary<string, double>>
            {
                ["S"] = new() { ["A"] = 1, ["B"] = 5 },
                ["A"] = new() { ["B"] = 1, ["T"] = 10 },
                ["B"] = new() { ["T"] = 1 },
                ["T"] = new()
            };
            var path = new AStar<WeightedState>(new WeightedState("S", graph), new WeightedState("T", graph)).Solve();
            Assert.IsNotNull(path);
            CollectionAssert.AreEqual(new[] { "S", "A", "B", "T" }, path.Select(p => p.Name).ToArray());
        }
    }
}
