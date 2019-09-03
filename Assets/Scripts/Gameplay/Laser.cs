using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private GameObject laserChunkPrefab;

    [SerializeField] private float rotation;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float laserLength;

    private readonly int MAX_CHUNKS = 16;
    private GameObject[] laserChunks;
    private float laserChunkSize = 0.5f;
    private float negativeSpacing = -0.05f;
    private int numChunks;

    private float LaserDist => laserChunkSize + negativeSpacing;

    private void Start()
    {
        laserChunks = new GameObject[MAX_CHUNKS];

        for(int i = 0; i < laserChunks.Length; i++)
        {
            laserChunks[i] = Instantiate(laserChunkPrefab, transform);
            laserChunks[i].SetActive(false);
        }

        numChunks = (int)(laserLength / LaserDist);

        for (int i = 0; i < numChunks; i++)
        {
            laserChunks[i].SetActive(true);
            laserChunks[i].transform.localPosition = new Vector3(i * LaserDist, 0, 0);
        }
    }

    public void Update()
    {
        rotation += rotationSpeed * Time.deltaTime;
        RotateChunks();
    }

    private void RotateChunks()
    {
        for (int i = 0; i < numChunks; i++)
        {
            var chunk = laserChunks[i]; 
            var quaternion = Quaternion.Euler(0, 0, rotation);
            var pos = new Vector3(i * LaserDist, 0, 0);
            chunk.transform.localPosition = quaternion * pos;
            chunk.GetComponent<LaserChunk>().Rotate(rotation);
        }
    }
}
