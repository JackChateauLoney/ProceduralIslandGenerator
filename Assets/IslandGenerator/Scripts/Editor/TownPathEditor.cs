using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathControl))]
public class TownPathEditor : Editor
{

    PathControl creator;

    private void OnEnable()
    {
        creator = (PathControl) target;
    }

    public override void OnInspectorGUI()
    {
        

        EditorGUI.BeginChangeCheck();
        if(GUILayout.Button("Create Path"))
        {
            Undo.RecordObject(creator, "Create Path");
            creator.CreatePath();
            
        }

        if(GUILayout.Button("Delete Path"))
        {
            Undo.RecordObject(creator, "Delete Path");
            creator.DeletePath();
            
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
