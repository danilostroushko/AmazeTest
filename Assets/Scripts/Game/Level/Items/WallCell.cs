using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCell : MonoBehaviour
{
    [SerializeField] private MeshRenderer cudeMesh;

    public void Init(Color wallColor)
    {
        cudeMesh.material.color = wallColor;
    }
}