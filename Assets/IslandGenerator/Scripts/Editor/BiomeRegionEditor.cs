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
            controller.SetCentreHeight(controller.centreHeight);

        

        if (GUILayout.Button("Reset Centre Height"))
            controller.SetCentreHeight(controller.originalCentreHeight.y);

        EditorGUILayout.Space(20);

        base.OnInspectorGUI();


    }
}
