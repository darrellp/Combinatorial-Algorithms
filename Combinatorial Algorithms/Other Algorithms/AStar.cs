using Priority_Queue;
#if PRINTASTAR
using System.Diagnostics;
#endif

namespace ChallengeCore.Challenges
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Class AStarState. </summary>
	///
	/// <remarks>
	/// The simplest possible implementation of IState.  Users of A* should inherit from this class.
	/// They will be required to implement Successors, IsEqual and GetHashCode.  If the path length
	/// is just "number of steps" then the default SuccessorDistance() can be used.  If there is no
	/// good distance estimator available then the default of 0 can be used which is equivalent to
	/// using Djikstra's algorithm.
	/// </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class AStarState : IState
	{
		public abstract IEnumerable<IState> Successors();
		public abstract bool IsEqual(IState istate);
		public abstract override int GetHashCode();

		public override bool Equals(object obj)
		{
            if (!(obj is AStarState state))
			{
				throw new ArgumentException("Bad compare with AStarState");
			}
			return IsEqual(state);
		}

		// Estimated distance of 0 implies that we're implementing Djikstra's algorithm which
		// essentially wanders at random in increasingly long paths until it stumbles on the
		// goal state.
		public virtual double EstDistance(IState target)
		{
			return 0;
		}

		public virtual double SuccessorDistance(IState successor)
		{
			return 1;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Implements the A* algorithm. </summary>
	///
	/// <remarks>
	/// This implementation is based on states which produce other states rather than a direct graph.
	/// That means that you usually don't have to produce a graph to use this solver.  If you've got
	/// a graph it's trivial to use it in the solver though.  It allows for a set of accepting states
	/// rather than just one.
	/// 
	/// A REALLY great source for into on A* can be found at:
	/// 	http://theory.stanford.edu/~amitp/GameProgramming/AStarComparison.html.
	/// </remarks>
	///
	/// <typeparam name="T">	. </typeparam>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class AStar<T> where T : class, IState
	{
		// TODO: Extend to optionally use a function to define an accepting state
		// This would involve us having no direct targets and using a distance estimation that depended
		// solely on the current node rather than on a specific target node.
		readonly FibonacciPriorityQueue<AStarNode<T>> _openSet = new FibonacciPriorityQueue<AStarNode<T>>();
		private readonly HashSet<T> _targets;
		private readonly HashSet<T> _closed = new HashSet<T>();

		public AStar(T start, HashSet<T> targets)
		{
			_targets = targets;
			_openSet.Add(new AStarNode<T>(start));
		}

		public AStar(T start, T target) : this(start, new HashSet<T> { target }) { }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Performs the A* algorithm to find minimum distance path. </summary>
		///
		/// <remarks>	Darrell Plank, 5/28/2020. </remarks>
		///
		/// <returns>
		/// A list of T objects which minimizes the distance between _start and one of the _targets.
		/// </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public List<T> Solve()
		{
			// Given an IState we need to be able to locate the corresponding
			// AStarNode in the Fibonacci heap and since we are letting Pqt
			// handle the messy details, we actually are looking for the
			// Pqt<AStarNode<T>> which is in the heap...
			var dictOpen = new Dictionary<T, AStarNode<T>>();

			// While there are elements in Open
			while (_openSet.Count != 0)
			{
				// Get the most likely candidate in Open
				var curOpenNode = (AStarNode<T>)_openSet.Pop();

#if PRINTASTAR
                Debug.WriteLine($"Opening {curOpenNode.StateData.ToString()}");
#endif

				// Is it a target node?
				if (_targets.Contains(curOpenNode.StateData))
				{
					// We're here!  Reconstruct the path and return it to the user.
					return ReconstructPath(curOpenNode);
				}
				// Move this node to Closed
				_closed.Add(curOpenNode.StateData);

				// For each of this node's successor nodes
				foreach (var nodeState in curOpenNode.StateData.Successors())
				{
					// Set neighbor to successor
					var neighbor = (T)nodeState;

					// Is neighbor in Closed?
					if (_closed.Contains(neighbor))
					{
						// TODO: if the dist est is inadmissable we may need to move this node to Open
						// This is because we can't be guaranteed of finding the absolute minimal
						// distance to nodes in _closed and so the new distance we found to our
						// closed neighbor may be smaller than it's original distance at which point
						// we need to reintroduce the node into the _open mix.

						// We've already handled this neighbor so continue on to next one
						continue;
					}

					// Is the neighbor in Open?
					if (dictOpen.ContainsKey(neighbor))
					{
						// Get the Open node for the neighbor
						var nodeInOpen = dictOpen[neighbor];

						// Make a proposed replacement using current node as the back link
						var nodeCandidate = new AStarNode<T>(neighbor, _targets, curOpenNode);

						// Is our proposed replacement a better solution?
						if (((AStarNode<T>)nodeInOpen).CompareTo(nodeCandidate) > 0)
						{
							// Then replace old neighbor node with our replacement
							_openSet.DecreaseKey(nodeInOpen, nodeCandidate);
						}
					}
					else
					{
						// Neighbor is outside both Closed and Open sets so add it to Open
						dictOpen[neighbor] = _openSet.Add(new AStarNode<T>(neighbor, _targets, curOpenNode));
					}
				}
			}
			return null;
		}

		private static List<T> ReconstructPath(AStarNode<T> next)
		{
			var ret = new List<T>();

			// Pull out just the state data in reverse order
			while (next != null)
			{
				ret.Add(next.StateData);
				next = next.BackLink;
			}

			// Reverse the created list...
			ret.Reverse();

			// ...and return it.
			return ret;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	A node in the A* algorithm. </summary>
		///
		/// <remarks>
		/// IState is the publicly exposed part of A* that the user needs to know about but internally we
		/// need more information to implement A* which is why we have the private AStarNode class.
		/// </remarks>
		///
		/// <typeparam name="TS">	The state type. </typeparam>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private class AStarNode<TS> : IComparable where TS : IState
		{
			public TS StateData { get; }					// The state this node is based on
			private readonly double _distanceFromStart;		// Distance this node is from the start node
			private readonly HashSet<TS> _targets;			// Targets we're searching for
            //	from the start node to this one.

			public AStarNode<TS> BackLink { get; }

            public AStarNode(TS data, HashSet<TS> targets, AStarNode<TS> backLink = null)
			{
				StateData = data;
				_targets = targets;
				BackLink = backLink;
				if (backLink != null)
				{
					_distanceFromStart = backLink._distanceFromStart + backLink.StateData.SuccessorDistance(data);
				}
			}

			public AStarNode(TS data) : this(data, new HashSet<TS> {data}) { }

			////////////////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>	Returns our estimate of the distance to nearest target from start node. </summary>
			///
			/// <remarks>
			/// This is the classic computation for "dist from start" + "est dist to target" and forms the
			/// heart of the A* algorithm.
			/// </remarks>
			///
			/// <returns>	Distance estimate. </returns>
			////////////////////////////////////////////////////////////////////////////////////////////////////

			private double DistToNearestTarget()
			{
				// This is the classic computation for <dist from start> + <est dist to target>
				// and forms the heart of the A* algorithm
				return _distanceFromStart + _targets.Select(t => StateData.EstDistance(t)).Min();
			}

			////////////////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>	Does comparison by using DistToNearestTarget and comparing the results. </summary>
			///
			/// <remarks>	Darrell Plank, 5/27/2020. </remarks>
			///
			/// <exception cref="ArgumentException">	Wrong type of object compared to a AStarNode. </exception>
			///
			/// <param name="obj">	The AStarNode to compare with. </param>
			///
			/// <returns>	A value that indicates the relative order of the objects being compared. </returns>
			////////////////////////////////////////////////////////////////////////////////////////////////////

			public int CompareTo(object obj)
			{
				var other = obj as AStarNode<TS>;
				if (other == null)
				{
					throw new ArgumentException("Wrong type of object compared to a AStarNode");
				}
				return DistToNearestTarget().CompareTo(other.DistToNearestTarget());
			}
		}
    }
}
