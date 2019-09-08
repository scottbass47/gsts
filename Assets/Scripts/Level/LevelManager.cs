﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] levelPrefabs;
    [SerializeField] private GameObject levelBranchPrefab;

    private int currentLevelIndex;
    public int LevelIndex => currentLevelIndex;

    private GameObject currentLevel;
    private LevelScript currentLevelScript;
    private CinemachineVirtualCamera levelCam;

    public GameObject Level => currentLevel;
    public LevelScript LevelScript => currentLevelScript;
    //public LevelData LevelData => LevelScript.LevelData;

    private GameObject nextLevel;
    private GameObject levelBranch;

    private EventManager events;

    private void Start()
    {
        events = GameManager.Instance.Events;

        events.AddListener<LevelBranchEnter>(this.gameObject, OnBranchEnter);
        events.AddListener<LevelBranchExit>(this.gameObject, OnBranchExit);

        levelCam = GameManager.Instance.Vcam;
    }

    public void SetupLevel()
    {
        currentLevelIndex = 0;
        var level = Instantiate(levelPrefabs[currentLevelIndex]);
        var levelScript = level.GetComponent<LevelScript>();

        currentLevel = level;
        currentLevelScript = level.GetComponent<LevelScript>();
        //levelScript.LevelData = GenerateLevelData(levelScript);

        levelCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = levelScript.LevelBoundary;
        levelCam.m_Lens.OrthographicSize = GameSettings.Settings.OrthographicZoom;
    }

    //private LevelData GenerateLevelData(LevelScript levelScript)
    //{
    //    var walls = levelScript.wallDecor;

    //    // Use floor dimensions to determine size of map
    //    LevelData levelData = new LevelData();
    //    levelData.Width = walls.size.x;
    //    levelData.Height = walls.size.y;
    //    levelData.map = new bool[levelData.Width, levelData.Height];
    //    levelData.origin = new Vector2(walls.cellBounds.xMin, walls.cellBounds.yMin) + (Vector2)levelScript.transform.position; 

    //    int colRef = walls.cellBounds.xMin;
    //    int rowRef = walls.cellBounds.yMin;
    //    for (int row = 0; row < levelData.Height; row++)
    //    {
    //        for (int col = 0; col < levelData.Width; col++)
    //        {
    //            if (walls.GetTile(new Vector3Int(col + colRef, row + rowRef, 0)) != null)
    //            {
    //                levelData.map[col, row] = true;
    //            }
    //        }
    //    }

    //    return levelData;
    //}

    public void NextLevel()
    {
        currentLevelIndex++;

        LevelScript.SetupLevelChange();

        var branch = Instantiate(levelBranchPrefab);
        var branchLevel = branch.GetComponent<LevelScript>();
        branch.transform.position = LevelScript.transform.position + LevelScript.LevelExit.localPosition - branchLevel.LevelEnter.localPosition;

        var next = Instantiate(levelPrefabs[currentLevelIndex]);
        var nextLevel = next.GetComponent<LevelScript>();
        next.transform.position = branch.transform.position + branchLevel.LevelExit.localPosition - nextLevel.LevelEnter.localPosition;
        nextLevel.OpenBottom();

        var levelBranch = branch.GetComponent<LevelBranch>();
        levelBranch.BranchCamera.gameObject.SetActive(false);
        levelBranch.BranchCamera.m_Lens.OrthographicSize = GameSettings.Settings.OrthographicZoom;
        levelBranch.OldLevel = LevelScript;
        levelBranch.NewLevel = nextLevel;

        //levelCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = null;
        StartCoroutine(CheckCameraBounds(nextLevel, branchLevel.LevelExit.position.y, levelCam));

        this.nextLevel = next;
        this.levelBranch = branch;
    }

    private void OnBranchEnter(LevelBranchEnter enter)
    {
        enter.LevelBranch.BranchCamera.gameObject.SetActive(true);
        levelCam.gameObject.SetActive(false);
        enter.LevelBranch.BranchCamera.Follow = GameManager.Instance.Player.transform;
    }

    private void OnBranchExit(LevelBranchExit exit)
    {
        exit.LevelBranch.BranchCamera.gameObject.SetActive(false);
        levelCam.gameObject.SetActive(true);
        levelCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = null;

        var nextLevelScript = nextLevel.GetComponent<LevelScript>();
    }

    private IEnumerator CheckCameraBounds(LevelScript nextLevelScript, float levelBranchExitY, CinemachineVirtualCamera cam)
    {
        while(true)
        {
            if(cam.transform.position.y - cam.m_Lens.OrthographicSize > levelBranchExitY + 2)
            {
                Destroy(currentLevel);
                Destroy(levelBranch);
                currentLevel = nextLevel;
                currentLevelScript = nextLevelScript;
                currentLevelScript.CloseBottom();
                levelCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = nextLevelScript.LevelBoundary;

                events.FireEvent(new LevelChange());
                yield break;
            }
            yield return null;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    var pos = levelCam.transform.position;
    //    Gizmos.DrawSphere(new Vector3(pos.x, pos.y - levelCam.m_Lens.OrthographicSize, 0), 1f);
    //}
}
