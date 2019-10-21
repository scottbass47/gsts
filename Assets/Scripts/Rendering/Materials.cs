using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Cinemachine;
using System.Text;

public class Materials : MonoBehaviour
{
    public static Materials Instance = null;

    [SerializeField] private Material vfxMaterial;
    public Material VFXMaterial => vfxMaterial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
