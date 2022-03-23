using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInstantiator : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    public Material Material;

    private void Awake()
    {
        Material = new Material(meshRenderer.material);
        meshRenderer.material = Material;
    }
}
