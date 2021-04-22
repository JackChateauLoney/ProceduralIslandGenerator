using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RegionBiome))]

public class BiomeRegionEditor : Editor
{
    RegionBiome controller;
    private void OnEnable()
    {
        controller = (RegionBiome)target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Update Centre Height"))
        {
            //get centre point and apply new value
            Mesh mesh = controller.GetComponent<MeshFilter>().sharedMesh;
            Vector3 point = controller.GetComponent<MeshFilter>().sharedMesh.vertices[0];
            
            Vector3[] vertices = mesh.vertices;
            vertices[0] = new Vector3(point.x, controller.centreHeight, point.z);
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            controller.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        if (GUILayout.Button("Reset Centre Height"))
        {
            //get centre point and apply original value
            Mesh mesh = controller.GetComponent<MeshFilter>().sharedMesh;            
            Vector3[] vertices = mesh.vertices;
            vertices[0] = controller.originalCentreHeight;
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            controller.GetComponent<MeshCollider>().sharedMesh = mesh;
        }


        base.OnInspectorGUI();


    }





}
