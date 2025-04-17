using System.Diagnostics;
using AdventOfCode.Enums;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class DijkstraMazeSolver
{
	#region Fields

	private readonly MazeGrid _maze;

	private DijkstraNode _startNode = null!;

	private DijkstraNode _endNode = null!;

	private List<DijkstraNode> _mazeNodes = null!;

	#endregion

	#region Ctor

	/// <summary>
	/// ctor
	/// </summary>
	/// <param name="mazeGrid">The maze to be solved, represented by a populated <see cref="MazeGrid"/> object
	public DijkstraMazeSolver(MazeGrid mazeGrid)
	{
		ArgumentNullException.ThrowIfNull(mazeGrid, nameof(mazeGrid));
		_maze = mazeGrid;
	}

	#endregion

	#region Methods

	/// <summary>
	/// From the supplied maze in the constructor, locate all unvisited nodes in the maze
	/// </summary>
	/// <returns>All unvisited nodes, initialises the start node with distance zero and sets the end location, if known</returns>
	private List<DijkstraNode> GetUnvisitedCells()
	{
		(_mazeNodes, _startNode, _endNode) = _maze.GetMazeNodes<DijkstraNode>();
		return new List<DijkstraNode>(_mazeNodes);
	}

	/// <summary>
	/// Maze solving method, requires the initial direction of travel for the start node
	/// </summary>
	/// <param name="initialDirection">A <see cref="DirectionOfTravel"/> value for the start node</param>
	/// <returns>The minimum distance value for the solution</returns>
	public int Solve(DirectionOfTravel initialDirection, List<DijkstraNode> unvisited, IDijkstraDistanceStrategy<MazeMovement> distanceStrategy)
	{
		unvisited ??= GetUnvisitedCells();
		var start = unvisited.FirstOrDefault(q => q.Distance == 0);

		//	If no start node located, then we're not solving
		if (start is null)
			return int.MaxValue;

		//	Is start the same as _startNode?
		Debug.Assert(start == _startNode);

		//	Set the initial direction of travel from the start node
		start.Direction = initialDirection;

		return Solve(unvisited, distanceStrategy);
	}

	/// <summary>
	/// Solves the maze using the shortest path found, given the nodes in
	/// <paramref name="unvisited"/>, applying distance units to the nodes
	/// yielded by <paramref name="distanceStrategy"/>
	/// </summary>
	/// <param name="unvisited">A list of unvisited nodes in the maze</param>
	/// <param name="distanceStrategy">The strategy used to calculate the distance units between nodes</param>
	/// <returns>The shortest path distance between start and end nodes</returns>
	/// <remarks>This can be called to provide alternate solutions by modifying
	/// the underlying nodes as visited/unvisited and blocking certain paths by
	/// setting distances to maximum</remarks>
	private int Solve(List<DijkstraNode> unvisited, IDijkstraDistanceStrategy<MazeMovement> distanceStrategy)
	{
		if (unvisited is null || unvisited.Count == 0)
			return int.MaxValue;

		//	Loop whilst we have unvisited nodes
		while (unvisited.Count > 0)
		{
			//	Take the minimum distance entry from the list of unvisited nodes
			var current = unvisited
				//	Distance of int.MaxValue is unreachable
				.Where(q => q.Distance != int.MaxValue)
				//	Don't try to solve for the end location
				.Where(q => !q.Location.Equals(_endNode.Location))
				.OrderBy(o => o.Distance)
				.FirstOrDefault();

			//	All possible visits have been made
			if (current is null)
				break;

			//	Remove the current cell from the unvisited list
			unvisited.Remove(current);
			//	Apply changes to the neighbours
			current.UpdateNeighbours(_maze, unvisited, distanceStrategy);
		}

		//	Do not rely on _endNode to extract the solution, search for the
		//	unvisited node at the same location and extract the solution value from there
		var endNode = unvisited.FirstOrDefault(q => q.Location.Equals(_endNode.Location));
		return endNode?.Distance ?? int.MaxValue;
	}

	/// <summary>
	/// Examines the solution of the maze and constructs a <see cref="MazePath{T}"/>
	/// object which contains all nodes from start node to <paramref name="endNode"/>
	/// </summary>
	/// <param name="nodes">the list of nodes within the maze which contain the solution</param>
	/// <param name="endNode">The end node of the maze</param>
	/// <returns>A path from start to end taken to solve the maze</returns>
	public MazePath<DijkstraNode> GetPath(List<DijkstraNode> nodes = null!, DijkstraNode endNode = null!)
	{
		nodes ??= _mazeNodes;
		endNode ??= _endNode;

		//	If endNode isn't defined, or there is no solution, no path is available
		if (endNode is null || endNode.Distance == int.MaxValue)
			return null!;

		//	Find path in reverse, from end node back to start
		var pathStack = new Stack<DijkstraNode>();
		pathStack.Push(endNode);

		//	Remove the end node from the nodes list
		nodes.Remove(endNode);
		var lastNode = endNode;

		while (nodes.Count > 0)
		{
			//	Reverse offset direction of travel from last move to find previous location
			var previousLocationOffset = lastNode.Direction.ToReverseOffset();
			var previousLocation = lastNode.Location.OffsetBy(previousLocationOffset.yOffset, previousLocationOffset.xOffset);
			var previousNode = nodes.Where(q => q.Distance < lastNode.Distance)
				.Where(q => q.Location.X == previousLocation.X && q.Location.Y == previousLocation.Y)
				.FirstOrDefault();

			//	Somehow must have got backed into a corner / dead-end?!
			if (previousNode is null)
				break;

			//	Push the newly discovered backwards node to the stack
			pathStack.Push(previousNode);

			//	is this the start? (in a Dijkstra graph the start is at distance zero)
			if (previousNode.Distance == 0)
				break;

			//	Not at the beginning so, loop around
			lastNode = previousNode;
			nodes.Remove(lastNode);
		}

		//	If the walk backwards along the path was successful, the last entry should have distance zero
		//	If the last entry in the stack does not have distance zero, no path from start to end was found
		if (pathStack.Peek().Distance != 0)
			return null!;

		var path = new MazePath<DijkstraNode>(endNode.Location);
		while (pathStack.Count > 0)
			path.AddToPath(pathStack.Pop());

		return path;
	}

	/// <summary>
	/// Solves the maze multiple times, attempting to locate all possible "best path" solutions
	/// </summary>
	/// <param name="initialDirection">The direction of travel for the start node</param>
	/// <param name="distanceStrategy">The strategy being used to calculate distances between nodes</param>
	/// <returns></returns>
	public List<MazePath<DijkstraNode>> SolveMultipleBestPaths(DirectionOfTravel initialDirection, IDijkstraDistanceStrategy<MazeMovement> distanceStrategy)
	{
		//	What is the best distance solution?
		//	Force solution to store results in _mazeNodes so they can be re-used
		var bestDistance = Solve(initialDirection, null!, distanceStrategy);

		if (bestDistance == int.MaxValue)
			return new List<MazePath<DijkstraNode>>();

		//	Store for results
		var bestPaths = new List<MazePath<DijkstraNode>>();

		//	Get the path for the best solution
		var bestPath = GetPath();
		//	Add to results
		bestPaths.Add(bestPath);

		//	Get the possible alternate route branching points from the best path
		var branchPoints = GetBranchNodesFromPath(bestPath);
		//	Store branches already under investigation
		var completedBranches = new List<BranchNode>(branchPoints);
		//	Branches needing investigation go into a stack to be investigated
		var branchesToInvestigate = new Stack<BranchNode>();
		//	Load the stack with branch points from the best path
		branchPoints.ForEach(b => branchesToInvestigate.Push(b));

		//	Loop until all branches investigated
		while (branchesToInvestigate.Count > 0)
		{
			var branchToCheck = branchesToInvestigate.Pop();
			//	Get all nodes for the maze
			var allNodes = GetNodesForMaze(bestPath, branchToCheck);
			//	Remove all nodes with valid distances, except for the next node to follow
			var unvisited = GetUnvisitedNodesForMaze(allNodes, branchToCheck.NextLocation);

			//	Solve the maze with the alternate route set
			var branchDistance = Solve(unvisited, distanceStrategy);

			//	If the solution is longer than the best distance, skip to next option
			if (branchDistance > bestDistance)
				continue;

			//	Get the path taken by this solution
			var branchPath = GetPath(allNodes);
			//	Add to results
			bestPaths.Add(branchPath);

			//	Now to check the path for new branch points
			branchPoints = GetBranchNodesFromPath(branchPath);
			//	Exclude any points already discovered
			branchPoints = branchPoints
				.Where(bp => !completedBranches.Contains(bp))
				.ToList();
			//	Add new branch points to list of known
			completedBranches.AddRange(branchPoints);
			//	Push new locations onto stack for discovery
			branchPoints.ForEach(bp => branchesToInvestigate.Push(bp));
		}

		return bestPaths;
	}

	/// <summary>
	/// Given a list of <paramref name="nodes"/>, find all unvisited nodes, except for the node at <paramref name="nextCoordinate"/>
	/// </summary>
	/// <param name="nodes">A list of nodes within the maze to be checked</param>
	/// <param name="nextCoordinate">The coordinate of the node to be excluded as "visited"</param>
	/// <returns>The list of nodes that have not been visited</returns>
	private List<DijkstraNode> GetUnvisitedNodesForMaze(List<DijkstraNode> nodes, Coordinate nextCoordinate)
	{
		//	Find all nodes that have a pre-calculated distance for them
		var visited = nodes.Where(n => n.Distance < int.MaxValue).ToList();
		//	Copy all nodes to unvisited
		var unvisited = new List<DijkstraNode>(nodes);

		//	For all "visited" nodes, remove from the unvisited list
		visited.ForEach(vn =>
		{
			//	Check: is this node the one to exclude?
			if (vn.Location.Equals(nextCoordinate))
				return;
			unvisited.Remove(vn);
		});
		return unvisited;
	}

	/// <summary>
	/// Creates a list of unvisited maze nodes, then applies solution information
	/// to the nodes along the <paramref name="mazePath"/> until the <paramref name="branch"/>
	/// node is reached
	/// </summary>
	/// <param name="mazePath">The path for the solution to the maze</param>
	/// <param name="branch">The location in the maze where the branch point is located</param>
	/// <returns>All nodes for the maze, with a partial solution applied</returns>
	private List<DijkstraNode> GetNodesForMaze(MazePath<DijkstraNode> mazePath, BranchNode branch)
	{
		//	Get all maze nodes, initialised to int.MaxValue
		var (mazeNodes, _, _) = _maze.GetMazeNodes<DijkstraNode>();
		var walking = true;

		var currentPathNode = mazePath.Path.First;

		//	Walk the path until the branch is reached
		while (walking)
		{
			//	get the actual DijkstraNode from the linked list object
			var currentNode = currentPathNode?.Value;

			//	If it cannot be found, there's a problem
			if (currentNode is null)
				return new List<DijkstraNode>();

			//	Find the equivalent node in the unvisited
			var currentUnvisited = mazeNodes.FirstOrDefault(q => q.Location.Equals(currentNode.Location));
			if (currentUnvisited is null)
				return new List<DijkstraNode>();

			//	Remove the "blank" node
			mazeNodes.Remove(currentUnvisited);
			//	Copy the distance and travel details from the best path solution
			currentUnvisited = new DijkstraNode(currentNode);
			//	Add back into the list
			mazeNodes.Add(currentUnvisited);

			//	Has the branch point been located?
			if (branch.BranchLocation.Equals(currentUnvisited.Location))
			{
				//	Get the node from unvisited
				var nextUnvisited = mazeNodes.FirstOrDefault(q => q.Location.Equals(branch.NextLocation));
				if (nextUnvisited is null)
					return new List<DijkstraNode>();
				//	Remove the "blank" node
				mazeNodes.Remove(nextUnvisited);

				//	Get the same node from the solution and extract distance
				var nextNode = _mazeNodes.FirstOrDefault(q => q.Location.Equals(branch.NextLocation));
				if (nextNode is null)
					return new List<DijkstraNode>();
				mazeNodes.Add(new DijkstraNode(nextNode));
				break;
			}
			currentPathNode = currentPathNode?.Next;
			walking = currentPathNode is not null;
		}

		return mazeNodes;
	}

	/// <summary>
	/// Helper class for storing branch points in the maze
	/// </summary>
	private class BranchNode
		: IEquatable<BranchNode>
	{
		public Coordinate BranchLocation { get; set; } = default!;
		public Coordinate NextLocation { get; set; } = default!;
		public Coordinate NextLocationInPath { get; set; } = default!;

		public bool Equals(BranchNode? other)
		{
			return other is not null &&
				BranchLocation.Equals(other.BranchLocation) &&
				NextLocation.Equals(other.NextLocation);
		}

		public override string ToString()
		{
			return $"BL:{BranchLocation}|NL:{NextLocation}";
		}
	}

	/// <summary>
	/// Obtain a list of potential branch points from <paramref name="mazePath"/>
	/// </summary>
	/// <param name="mazePath">The path being taken through the maze</param>
	/// <returns>A list of nodes where a branch decision could be made</returns>
	private List<BranchNode> GetBranchNodesFromPath(MazePath<DijkstraNode> mazePath)
	{
		var branches = new List<BranchNode>();
		var currentLinkedNode = mazePath.Path.First ?? null!;
		var nextLinkedNode = currentLinkedNode?.Next ?? null!;
		//	Get all nodes in the maze solution
		var allNodes = new List<DijkstraNode>(_mazeNodes);

		//	Path is a linked list, next node is null when at the end
		while (nextLinkedNode is not null)
		{
			//	Extract the DijkstraNode objects
			var currentNode = currentLinkedNode?.Value;
			var nextNode = nextLinkedNode?.Value;

			//	What are the node neighbours?
			var neighbours = currentNode?.GetNeighbours(_maze, allNodes) ?? new List<DijkstraNode>();

			//	Which nodes are not on the path?
			var notOnPath = neighbours
				.Where(n => !(nextNode?.Location.Equals(n.Location) ?? true))
				.ToList();
			//	Which one is?
			var onPath = neighbours
				.FirstOrDefault(n => nextNode?.Location.Equals(n.Location) ?? false);

			//	For all non-path nodes, add a new BranchNode:
			//		storing the location of the decision point
			//		which node should be taken next
			//		which one shouldn be ignored (this was on the optimal route)
			notOnPath.ForEach(n =>
			{
				var node = new BranchNode()
				{
					BranchLocation = currentNode?.Location!,
					NextLocation = n.Location,
					NextLocationInPath = onPath?.Location!
				};
				branches.Add(node);
			});
			currentLinkedNode = nextLinkedNode;
			nextLinkedNode = currentLinkedNode?.Next;
		}
		return branches;
	}

	/// <summary>
	/// Helper method to return the list of nodes which make up the solution to the maze
	/// </summary>
	/// <returns>All nodes that are part of the solution (i.e. have distance values that are not <see cref="int.MaxValue"/>)</returns>
	public List<DijkstraNode> GetSolutionNodes()
	{
		return new List<DijkstraNode>(_mazeNodes.Where(n => n.Distance != int.MaxValue).OrderBy(o => o.Distance));
	}

	#endregion
}