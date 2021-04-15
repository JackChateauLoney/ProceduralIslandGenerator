using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGeneration))]

public class GenerationEditor : Editor
{

    MeshGeneration creator;

    private void OnEnable()
    {
        creator = (MeshGeneration) target;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Terrain"))
        {
            Undo.RecordObject(creator, "Generate Terrain");
            creator.GenerateTerrain();
        }


        if(GUILayout.Button("Delete Terrain"))
        {
            Undo.RecordObject(creator, "Delete Terrain");


            int whileSafeCatch = 0;

            while(creator.voronoiParent.transform.childCount > 0)
            {
                DestroyImmediate( creator.voronoiParent.transform.GetChild(0).gameObject);
                whileSafeCatch++;

                //stop the while loop being infinite, just in case because this is in editor
                if(whileSafeCatch > 10000)
                {
                    Debug.Log("While loop went on for too long, exiting to keep the editor safe :)");
                    break;
                }
            }
        }


    }




}
