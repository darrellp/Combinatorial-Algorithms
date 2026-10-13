using System.Collections.Generic;
using System.Linq;

namespace ChallengeCore.Challenges
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>	Interface IState. </summary>
    ///
    /// <remarks>
    /// This algorithm works with the concept of states and successors rather that the usual graph
    /// nomenclature for A*.  This is because I find the notion of "graph" to be more concrete than
    /// necessary for most A* problems.  Also, when using a graph we have to implement a graph data
    /// structure and then populate that structure fully to pass to the A* algorithm.  This is quite
    /// often unnecessary and wasteful since a large portion of the graph may never be traversed.
    /// Where the standard graph has "nodes" we have "states" and where a graph's nodes have
    /// "neighbors" we have "successors".  The main advantage of the state idea is that the neighbors
    /// are produced at run time and, if not required for A*, are never actually produced.  Also,
    /// rather than enforcing some sort of rigid graph structure with edges, weights, etc. we're able
    /// to make do with this very simple IState interface.
    /// 
    /// >>>> IMPORTANT!!!!
    /// If your state derives from AStarState then it will use IsEqual for hashsets.  If not,
    /// YOU NEED TO IMPLEMENT GetHashCode() and Equals() or you will have serious problems
    /// (i.e., infinite loops).
    /// </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    // TODO: Handle the need to implement Equals() and GetHashCode() better than a note in the comments

    public interface IState
    {
        // Enumeration of neighboring states
        IEnumerable<IState> Successors();

        // Distance to an individual successor/neighbor
        double SuccessorDistance(IState successor);

        // In general, the following should be an underestimate (i.e., 
        // admissible).  0 works but may cause it to take longer to
        // ferret out the correct answer.  Values which overestimate
        // the goal distance will find a path and perhaps work quicker
        // but it won't be guaranteed minimal.

        double EstDistance(IState target);

        int GetHashCode();
        bool IsEqual(IState state);

        int SuccessorCount() => Successors().Count();
    }
}