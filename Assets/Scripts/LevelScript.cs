using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private bool showSpawns;
    [SerializeField] private Vector3[] portalSpawns;
    public Vector3[] PortalSpawns => portalSpawns;

    [SerializeField] private GameObject door;
    public GameObject Door => door;

    private void OnDrawGizmos()
    {
        if (!showSpawns) return;
        foreach(var spawn in portalSpawns)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(spawn, 0.5f);
        } 
    }
}
