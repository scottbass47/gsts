using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System;

public class Pathfinding : MonoBehaviour
{
    private PathRequestManager requestManager;
    private LevelGrid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<LevelGrid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos, PathParameters parameters)
    {
        StartCoroutine(FindPath(startPos, targetPos, parameters));
    }

    private IEnumerator FindPath(Vector3 start, Vector3 target, PathParameters parameters)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        var startNode = grid.NodeFromWorldPoint(start);
        var targetNode = grid.NodeFromWorldPoint(target);
        if (startNode == null || targetNode == null)
        {
            requestManager.FinishedProcessingPath(waypoints, pathSuccess, parameters);
            yield break;
        }

        var openSet = new Heap<Node>(grid.MaxSize);
        var closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        
		while (openSet.Count > 0) {
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

			if (currentNode == targetNode) {
                pathSuccess = true;
				break;
			}

			foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
				if (!neighbour.Walkable || closedSet.Contains(neighbour)) {
					continue;
				}

                if(Diagonal(currentNode, neighbour))
                {
                    var n1 = grid.GetNode(currentNode.xGrid, neighbour.yGrid);
                    var n2 = grid.GetNode(neighbour.xGrid, currentNode.yGrid);
                    if (!n1.Walkable || !n2.Walkable) continue;
                }

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.MovementPenalty;
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
					else {
                        openSet.UpdateItem(neighbour);
                    }
				}
			}
		}
        if (pathSuccess)
        {
            waypoints = RetracePath(start, startNode,targetNode, parameters.AllAngles); 
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess, parameters);
	}

    private bool Diagonal(Node one, Node two)
    {
        return one.xGrid != two.xGrid && one.yGrid != two.yGrid;
    }

	private Vector3[] RetracePath(Vector2 startPos, Node startNode, Node endNode, bool allAngles) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
        Vector3[] waypoints = allAngles ? SimplifyPath(startPos, path) : SimplifyPath45(startPos, path);
        return waypoints;
	}

    private Vector3[] SimplifyPath45(Vector2 startPos, List<Node> path)
    {
        path.Reverse();
		List<Vector3> waypoints = new List<Vector3>();
		
		for (int i = 0; i < path.Count - 2; i++)
        {
            var p1 = path[i];
            var p2 = path[i + 1];
            var p3 = path[i + 2];

            int a = p1.xGrid * (p2.yGrid - p3.yGrid) +
                p2.xGrid * (p3.yGrid - p1.yGrid) +
                p3.xGrid * (p1.yGrid - p2.yGrid);

            if(a == 0)
            {
                path.RemoveAt(i + 1);
                i--;
            }
        }
        path.ForEach((node) => waypoints.Add(node.WorldPosition));
		return waypoints.ToArray();
    }

    private Vector3[] SimplifyPath(Vector2 startPos, List<Node> path)
    {
        path.Reverse();

		List<Vector3> waypoints = new List<Vector3>();
		Vector2 lastWaypoint = startPos;
        int lastWaypointIndex = 0;
		
		for (int i = 1; i < path.Count; i++)
        {
            var radius = 1f;
            var dir = (Vector2)path[i].WorldPosition - lastWaypoint;
            if(lastWaypointIndex != i - 1 && Physics2D.CircleCast(lastWaypoint, radius, dir, dir.magnitude, LayerMask.GetMask("Wall")))
            {
                i--;
                lastWaypointIndex = i;
                lastWaypoint = path[i].WorldPosition;
                waypoints.Add(path[i].WorldPosition);
            }
		}
        waypoints.Add(path[path.Count - 1].WorldPosition);
		return waypoints.ToArray();
    }

    private int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.xGrid - nodeB.xGrid);
		int dstY = Mathf.Abs(nodeA.yGrid - nodeB.yGrid);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
}
