using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBranchCameraTrigger : MonoBehaviour
{
    private LevelBranch levelBranch;

    private void Start()
    {
        levelBranch = GetComponentInParent<LevelBranch>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        levelBranch.PlayerEnter(collision.transform.root.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        levelBranch.PlayerExit(collision.transform.root.gameObject);
    }
}
