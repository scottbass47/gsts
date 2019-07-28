using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    private bool walkable;
    public bool Walkable => walkable;

    private Vector3 worldPosition;
    public Vector3 WorldPosition => worldPosition;

	public int xGrid { get; private set; }
	public int yGrid { get; private set; }
	public int MovementPenalty { get; private set; }
    public int BrushFireDist { get; set; }

    public int gCost { get; set; }
    public int hCost { get; set; }
    public int fCost => gCost + hCost;

	public Node parent { get; set; }

    private int heapIndex;

	public int HeapIndex
    {
        get => heapIndex;
        set => heapIndex = value;
	}

    public Node(bool walkable, Vector3 worldPosition, int xGrid, int yGrid, int movementPenalty)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.xGrid = xGrid;
        this.yGrid = yGrid;
        this.MovementPenalty = movementPenalty;
    }

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
