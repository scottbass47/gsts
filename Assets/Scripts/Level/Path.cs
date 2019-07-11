using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path 
{
    public Vector3[] Waypoints { get; set; }
    public float TargetThreshold { get; set; } = 1f;

    private int currentWaypointIndex = 0;
    public int CurrentWaypointIndex => currentWaypointIndex;

    public bool Done => currentWaypointIndex >= Waypoints.Length;
    public bool FirstWaypoint => currentWaypointIndex == 0;
    public Vector3 CurrentWaypoint => Waypoints[currentWaypointIndex];

	private Line[] turnBoundaries;
	public Line[] TurnBoundaries => turnBoundaries;

	private int finishLineIndex;

	public Path(Vector3[] waypoints, Vector3 startPos, float turnDst) {
        Waypoints = waypoints;
		turnBoundaries = new Line[Waypoints.Length];
		finishLineIndex = turnBoundaries.Length - 1;

		Vector2 previousPoint = V3ToV2 (startPos);
		for (int i = 0; i < Waypoints.Length; i++) {
			Vector2 currentPoint = V3ToV2 (Waypoints [i]);
			Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
			Vector2 turnBoundaryPoint = (i == finishLineIndex)?currentPoint : currentPoint - dirToCurrentPoint * turnDst;
			turnBoundaries [i] = new Line (turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
			previousPoint = turnBoundaryPoint;
		}
	}

	Vector2 V3ToV2(Vector3 v3) {
		return new Vector2 (v3.x, v3.y);
	}

    public void AdvanceWaypoint()
    {
        currentWaypointIndex++;
    }

    public bool TargetOnPath(Vector3 target)
    {
        if (Waypoints.Length == 0) return false;
        var dist = Waypoints[Waypoints.Length - 1] - target;
        return dist.sqrMagnitude < TargetThreshold * TargetThreshold;
    }

    public bool AtNextWaypoint(Vector3 pos)
    {
        return turnBoundaries[currentWaypointIndex].HasCrossedLine(pos);
    }

	public void DrawWithGizmos()
    {

		Gizmos.color = Color.black;
		foreach (Vector3 p in Waypoints) {
			Gizmos.DrawCube (p + Vector3.forward, Vector3.one);
		}

		Gizmos.color = Color.white;
		foreach (Line l in turnBoundaries) {
			l.DrawWithGizmos (10);
		}
	}
}
