using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour {

	private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	private PathRequest currentPathRequest;

	private Pathfinding pathfinding;

	private bool isProcessingPath;

    private Vector3[] path;
    public delegate void PathCallback(Vector3[] waypoings, bool success, PathParameters parameters);

	void Awake() {
		pathfinding = GetComponent<Pathfinding>();
	}

    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, PathCallback callback) 
    {
        RequestPath(pathStart, pathEnd, callback, PathParameters.Default);
    }

    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, PathCallback callback, PathParameters parameters) {
		PathRequest newRequest = new PathRequest(
            pathStart,
            pathEnd,
            callback,
            parameters == null ? PathParameters.Default : parameters
        );

		pathRequestQueue.Enqueue(newRequest);
		TryProcessNext();
	}

	void TryProcessNext() {
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.pathParameters);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success, PathParameters parameters) {
		currentPathRequest.callback(path,success,parameters);
		isProcessingPath = false;
		TryProcessNext();
	}

	struct PathRequest {
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public PathCallback callback;
        public PathParameters pathParameters;

		public PathRequest(Vector3 _start, Vector3 _end, PathCallback _callback, PathParameters _pathParameters) {
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
            pathParameters = _pathParameters;
		}
	}
}

public class PathParameters
{
    public readonly bool AllAngles;
    public readonly float TurningRadius;

    private static PathParameters defaultParameters = new PathParameters(true, 1f);
    public static PathParameters Default => defaultParameters;

    public PathParameters(bool allAngles, float turningRadius)
    {
        this.AllAngles = allAngles;
        this.TurningRadius = turningRadius;
    }

}
