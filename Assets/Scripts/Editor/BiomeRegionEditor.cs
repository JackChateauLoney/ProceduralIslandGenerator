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
            Vector3 point = controller.GetComponent<MeshFilter>().sharedMesh.vertices[0];
            controller.GetComponent<MeshFilter>().sharedMesh.vertices[0] = new Vector3(point.x, controller.centreHeight, point.z);
            
            //apply mesh

        }

        if (GUILayout.Button("Reset Centre Height"))
        {
            controller.GetComponent<MeshFilter>().sharedMesh.vertices[0] = controller.originalCentreHeight;
        }


        base.OnInspectorGUI();


    }





}
