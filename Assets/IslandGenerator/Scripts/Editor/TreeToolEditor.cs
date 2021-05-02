using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreeTool))]
public class TreeToolEditor : Editor
{
    TreeTool creator;

    private void OnEnable()
    {
        creator = (TreeTool) target;
    }

    public override void OnInspectorGUI()
    {
        

        EditorGUI.BeginChangeCheck();
        if(GUILayout.Button("Create Forest"))
        {
            Undo.RecordObject(creator, "Create Forest");
            creator.CreateForest();
            
        }

        if(GUILayout.Button("Delete Forest"))
        {
            Undo.RecordObject(creator, "Delete Forest");
            creator.DeleteForest();
            
        }

        bool isClosed = GUILayout.Toggle(creator.showBox, "Show Red Box");
        if(isClosed != creator.showBox)
        {
            Undo.RecordObject(creator, "Toggle Box");
            creator.showBox = isClosed;
        }

        if(EditorGUI.EndChangeCheck())
            SceneView.RepaintAll();

        

        EditorGUILayout.Space(20);
        
        base.OnInspectorGUI();
    }


    
}
