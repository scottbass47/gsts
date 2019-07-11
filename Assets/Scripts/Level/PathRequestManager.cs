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
    public Transform seeker, target;

	void Awake() {
		pathfinding = GetComponent<Pathfinding>();
	}

    private void Update()
    {
        if (!isProcessingPath)
        {
            RequestPath(seeker.transform.position, target.transform.position, (path, success) =>
            {
                if (success)
                {
                    this.path = path;
                }
            });
        }
    }

    private void OnDrawGizmos()
    {
        if(path != null)
        {
            var lastPos = seeker.transform.position;
            for(int i = 0; i < path.Length; i++)
            {
                Debug.DrawLine(lastPos, path[i], Color.cyan);
                lastPos = path[i];
            }
        }
    }

    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
		PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
		pathRequestQueue.Enqueue(newRequest);
		TryProcessNext();
	}

	void TryProcessNext() {
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success) {
		currentPathRequest.callback(path,success);
		isProcessingPath = false;
		TryProcessNext();
	}

	struct PathRequest {
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) {
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}

	}

}
