using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
	private Node[,] nodeGrid;

	/// <summary>
	/// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
	/// Note that you will probably need to add some helper functions
	/// from the startPos to the endPos
	/// </summary>
	/// <param name="startPos"></param>
	/// <param name="endPos"></param>
	/// <param name="grid"></param>
	/// <returns></returns>
	public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {			   
		if(endPos.x < 0 || endPos.x > grid.GetLength(0) || 
			endPos.y < 0 || endPos.y > grid.GetLength(1)) 
		{
			return null;
		}

		HashSet<Node> closedSet = new HashSet<Node>();
		List<Node> openSet = new List<Node>();
		CreateNodes(grid, endPos);

		Node startNode = nodeGrid[startPos.x, startPos.y];
		Node endNode = nodeGrid[endPos.x, endPos.y];
		openSet.Add(startNode);


		while(openSet.Count > 0) 
		{
			Node currentNode = openSet[0];

			for(int i = 1; i < openSet.Count; i++) 
			{
				if(openSet[i].FScore < currentNode.FScore ||
					(openSet[i].FScore == currentNode.FScore && 
					openSet[i].HScore < currentNode.HScore)) 
				{
					currentNode = openSet[i];
				}
			}

			openSet.Remove(currentNode);
			closedSet.Add(currentNode);

			if(currentNode == endNode) 
			{			
				List<Vector2Int> pathPositions = new List<Vector2Int>();

				while(currentNode != startNode) 
				{
					pathPositions.Add(currentNode.position);

					if(currentNode.parent != null) 
					{
						currentNode = currentNode.parent;
					}
				}

				pathPositions.Reverse();
				return pathPositions;
			}

			foreach(Node neighbour in GetNeighbourNodes(currentNode, grid)) 
			{
				if(closedSet.Contains(neighbour)) 
				{
					continue;
				}

				float newMoveCostToNeighbour = currentNode.GScore + GetDistance(currentNode.position, neighbour.position);

				if(newMoveCostToNeighbour < neighbour.GScore || 
					!openSet.Contains(neighbour)) 
				{
					neighbour.GScore = newMoveCostToNeighbour;
					neighbour.HScore = GetDistance(neighbour.position, endPos);
					neighbour.parent = currentNode;

					if(!openSet.Contains(neighbour)) 
					{
						openSet.Add(neighbour);
					}
				}
			}
		}

		return null;
    }

	private void CreateNodes(Cell[,] grid, Vector2Int endPos) 
	{
		int gridSizeX = grid.GetLength(0);
		int gridSizeY = grid.GetLength(1);

		nodeGrid = new Node[gridSizeX, gridSizeY];

		for(int x = 0; x < gridSizeX; x++) 
		{
			for(int y = 0; y < gridSizeY; y++) 
			{
				Vector2Int pos = new Vector2Int(x, y);
				int hScore = GetDistance(pos, endPos);

				nodeGrid[pos.x, pos.y] = new Node(pos, null, int.MaxValue, hScore);
			}
		}
	}

	private int GetDistance(Vector2Int nodePosA, Vector2Int nodePosB) 
	{
		int distX = Mathf.Abs(nodePosA.x - nodePosB.x);
		int distY = Mathf.Abs(nodePosA.y - nodePosB.y);

		return distX + distY;
	}

	private List<Node> GetNeighbourNodes(Node current, Cell[,] grid) 
	{
		List<Node> neigbours = new List<Node>();

		if(!grid[current.position.x, current.position.y].HasWall(Wall.UP)) 
		{
			Vector2Int pos = new Vector2Int(current.position.x, current.position.y + 1);
			Node neighbourNode = nodeGrid[pos.x, pos.y];

			neigbours.Add(neighbourNode);
		}

		if(!grid[current.position.x, current.position.y].HasWall(Wall.DOWN)) 
		{
			Vector2Int pos = new Vector2Int(current.position.x, current.position.y - 1);
			Node neighbourNode = nodeGrid[pos.x, pos.y];

			neigbours.Add(neighbourNode);
		}

		if(!grid[current.position.x, current.position.y].HasWall(Wall.LEFT)) 
		{
			Vector2Int pos = new Vector2Int(current.position.x - 1, current.position.y);
			Node neighbourNode = nodeGrid[pos.x, pos.y];

			neigbours.Add(neighbourNode);
		}

		if(!grid[current.position.x, current.position.y].HasWall(Wall.RIGHT)) 
		{
			Vector2Int pos = new Vector2Int(current.position.x + 1, current.position.y);
			Node neighbourNode = nodeGrid[pos.x, pos.y];

			neigbours.Add(neighbourNode);
		}

		return neigbours;
	}

	/// <summary>
	/// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
	/// </summary>
	public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
