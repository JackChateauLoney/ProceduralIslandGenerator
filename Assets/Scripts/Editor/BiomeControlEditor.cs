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


    private void OnSceneGUI()
    {
        Input();
        Draw();
    }


    void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        //shift left click
        if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {

            

        }


    }


    void Draw()
    {

        //for (int i = 0; i < Path.NumSegments; i++)
        //{
        //    Vector2[] points = Path.GetPointsInSegment(i);

        //    if(controller.displayControlPoints)
        //    {
        //        Handles.color = Color.black;
        //        Handles.DrawLine(points[1], points[0]);
        //        Handles.DrawLine(points[2], points[3]); 
        //    }
            
        //    Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? controller.selectedSegmentCol : controller.segmentCol;
        //    Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, 2);
        //}



        ////draw handles
        //for (int i = 0; i < Path.NumPoints; i++)
        //{
        //    if(i % 3 == 0 || controller.displayControlPoints)
        //    {
        //        Handles.color = (i % 3 == 0) ? controller.anchorCol : controller.controlCol;
        //        float handlesSize = (i % 3 == 0) ? controller.anchorDiameter : controller.controlDiameter;
        //        Vector2 newPos = Handles.FreeMoveHandle(Path[i], Quaternion.identity, 0.1f, Vector2.zero, Handles.CylinderHandleCap);
        //        if(Path[i] != newPos)
        //        {
        //            Undo.RecordObject(controller, "Move point");
        //            Path.MovePoint(i, newPos);
        //        }
        //    }
            
        //}
    
    }
}
