using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSearchTest : MonoBehaviour
{
    private List<Node> nodes;
    private LevelGrid grid;

    public int min = 0;
    public int max = 0;

    private void Start()
    {
        grid = GetComponent<LevelGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var node = grid.NodeFromWorldPoint(point);
        nodes = grid.GetNodesInMinMaxRange(node, min, max);
    }

    private void OnDrawGizmos()
    {
        if (nodes == null) return;
        foreach(var node in nodes)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(node.WorldPosition, Vector3.one * 0.9f);
        }
    }
}
