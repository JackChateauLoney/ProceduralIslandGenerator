using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class WaterNoise : MonoBehaviour
{
    [SerializeField] float power = 10;
    [SerializeField] float scale = 2;
    [SerializeField] float timeScale = 0.5f;

    float offsetX = 0;
    float offsetY = 0;
    MeshFilter filter;


    private void Start()
    {
        filter = GetComponent<MeshFilter>();
        MakeNoise();
    }


    private void Update()
    {
        MakeNoise();
        offsetX += Time.deltaTime * timeScale;
        offsetY += Time.deltaTime * timeScale;
    }



    void MakeNoise()
    {
        Vector3[] vertices = filter.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = CalculateHeight(vertices[i].x, vertices[i].z) * power;
        }

        filter.mesh.vertices = vertices;

    }


    float CalculateHeight(float x, float y)
    {
        float xCord = x * scale + offsetX;
        float yCord = y * scale + offsetY;

        return Mathf.PerlinNoise(xCord, yCord);
    }


}
