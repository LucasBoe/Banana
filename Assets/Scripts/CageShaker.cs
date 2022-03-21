using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CageShaker : MonoBehaviour
{
    [SerializeField] Cage cage;
    [SerializeField] Material mat;
    [SerializeField] MeshRenderer[] meshes;
    [SerializeField] float shakeIntensity = 0.25f;

    private void Awake()
    {
        mat = new Material(mat);
        foreach (MeshRenderer mesh in meshes)
            mesh.material = mat;
    }

    private void Start()
    {
        mat.SetVector("offsets", new Vector4(Random.Range(0f, 20f), Random.Range(0f, 20f), Random.Range(0f, 20f), Random.Range(0f, 20f)));
        mat.SetFloat("shake", shakeIntensity);
    }

    private void OnEnable()
    {
        cage.Open += OnOpenCage;
    }
    private void OnDisable()
    {
        cage.Open -= OnOpenCage;
    }
    private void OnOpenCage()
    {
        StartCoroutine(BlendShakeValueRoutine());
    }

    private IEnumerator BlendShakeValueRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(shakeIntensity / 10f);
        float shake = shakeIntensity;
        while (shake >= 0f)
        {
            shake = Mathf.Clamp(shake-(shakeIntensity / 10f), 0f, 1f);
            mat.SetFloat("shake", shake);
            yield return wait;
        }

    }
}
