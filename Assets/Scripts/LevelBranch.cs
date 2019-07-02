using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelBranch : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera branchCamera;
    public CinemachineVirtualCamera BranchCamera => branchCamera;

    private LevelScript levelScript;

    public LevelScript OldLevel { get; set; }
    public LevelScript NewLevel { get; set; }

    private EventManager events;

    private void Start()
    {
        events = GameManager.Instance.Events;

        levelScript = GetComponent<LevelScript>();
    }

    public void PlayerEnter(GameObject player)
    {
        events.FireEvent(new LevelBranchEnter { LevelBranch = this });
    }

    public void PlayerExit(GameObject player)
    {
        var playerBody = player.GetComponent<Body>();
        bool nextLevel = playerBody.CenterFeet.position.y >= levelScript.LevelExit.position.y;
        events.FireEvent(new LevelBranchExit { LevelBranch = this, Level = nextLevel ? NewLevel : OldLevel });
    }
}
