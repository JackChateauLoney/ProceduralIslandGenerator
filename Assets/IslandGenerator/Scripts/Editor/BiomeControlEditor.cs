using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BiomeControl))]
public class BiomeControlEditor : Editor
{

    BiomeControl controller;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


    }

    private void OnEnable()
    {
        controller = (BiomeControl)target;
    }

    private void OnSceneGUI()
    {
        Input();
    }


    void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        //shift left click
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            RaycastHit hit;

            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition), out hit))
            {
                if (hit.transform.parent.name == "IslandParent")
                {
                    Debug.Log("clicked on: " + hit.transform.name);

                    //change clicked biome to brush type and regenerate it
                    if (hit.transform.GetComponent<RegionBiome>())
                    {
                        hit.transform.GetComponent<RegionBiome>().myBiome = controller.brushType;
                        hit.transform.GetComponent<RegionBiome>().GenerateBiome();
                    }
                    else
                        Debug.Log("No region biome script attached to clicked object");




                }
            }


        }


    }
}
