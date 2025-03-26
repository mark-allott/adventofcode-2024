using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class MazePath<T>
	where T : class, IMazeNode
{
	#region Fields

	/// <summary>
	/// Debug counter - increments each time a route is generated
	/// </summary>
	private static int _counter = 1;

	/// <summary>
	/// Holds the maze nodes of the path taken
	/// </summary>
	private readonly List<T> _pathNodes = new List<T>();

	/// <summary>
	/// Holds the location of the exit coordinate
	/// </summary>
	private IGraphCoordinate _exitNode;

	#endregion

	#region Properties

	/// <summary>
	/// Helper property: set if the path includes the exit node
	/// </summary>
	public bool CompletesMaze { get; private set; }

	/// <summary>
	/// Yields the distance to the exit node, if found, otherwise int.MaxValue
	/// </summary>
	public int DistanceToExit => CompletesMaze
		? _pathNodes.FirstOrDefault(n => n.Location.Equals(_exitNode))?.Distance ?? int.MaxValue
		: int.MaxValue;

	/// <summary>
	/// Yields the coordinates of the maze nodes in the path
	/// </summary>
	public List<IGraphCoordinate> PathCoordinates => _pathNodes.Select(n => n.Location).ToList();

	/// <summary>
	/// Yields a readonly list of maze nodes in the path
	/// </summary>
	public List<T> Nodes => _pathNodes.AsReadOnly().ToList();

	/// <summary>
	/// Yeilds a linked list of maze nodes in the path
	/// </summary>
	public LinkedList<T> Path { get; } = new LinkedList<T>();

	/// <summary>
	/// Yields the last node in the path
	/// </summary>
	public T CurrentNode => Nodes.LastOrDefault()!;

	/// <summary>
	/// Holds the counter for the path - allows sorting in order of paths discovered
	/// </summary>
	public int PathCounter { get; }

	#endregion

	#region ctor

	public MazePath(IGraphCoordinate exitCoordinate)
	{
		_exitNode = exitCoordinate;
		PathCounter = _counter++;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Adds the specified <paramref name="node"/> to the path
	/// </summary>
	/// <param name="node">The node to be added to the path</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <remarks>If the node being added is duplicated or null, an exception is raised.
	/// If the node being added is the exit node, the <see cref="CompletesMaze"/>
	/// property shall be updated. If the maze is already completed, adding new
	/// nodes will fail.</remarks>
	public void AddToPath(T node)
	{
		ArgumentNullException.ThrowIfNull(node, nameof(node));

		//	If maze is complete, why add more nodes?!?
		if (CompletesMaze)
			return;

		if (_pathNodes.Select(n => n.Location).Contains(node.Location))
			throw new ArgumentOutOfRangeException(nameof(node), node, $"Duplicate locations are not permitted");

		//	Add the node to the list of nodes in the path
		_pathNodes.Add(node);
		//	Also add the node to the linked list
		Path.AddLast(node);

		//	If the node added is the same location as the exit, then the path completes the maze
		CompletesMaze = node.Location.Equals(_exitNode);
	}

	/// <summary>
	/// Replicates the current path into a new object
	/// </summary>
	/// <returns>This path as a new object</returns>
	public MazePath<T> Replicate()
	{
		var copy = new MazePath<T>(_exitNode);
		_pathNodes.ForEach(n => copy.AddToPath(n));
		return copy;
	}

	/// <summary>
	/// Debug helper: shows a friendly description of the path
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return $"{PathCounter}:{(CompletesMaze ? "C" : "N")}:{CurrentNode}";
	}

	#endregion
}