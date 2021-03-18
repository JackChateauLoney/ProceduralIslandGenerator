using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class RoadCreator : MonoBehaviour
{

    [Range(0.5f, 1.5f), SerializeField] float spacing = 1f;
    [SerializeField] float roadWidth = 1;


    public void UpdateRoad()
    {
        //Path path = GetComponent<PathCreator>().path;
        //Vector2[] points = path.CalculateEvenlySpacedPoints(spacing);
        //GetComponent<MeshFilter>().mesh = CreateRoadMesh(points);

    }



    Mesh CreateRoadMesh(Vector2[] points)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        int[] tris = new int[2 * (points.Length - 1) * 3];
        int vertIndex = 0;
        int triIndex = 0;


        for (int i = 0; i < points.Length; i++)
        {
            Vector2 forward = Vector2.zero;
            if(i < points.Length - 1)
                forward += points[i + 1] - points[i];    
            if(i > 0)
                forward += points[i] - points[i - 1];    

            forward.Normalize();
            Vector2 left = new Vector2(-forward.y, -forward.x);


            verts[vertIndex] = points[i] + left * roadWidth * 0.5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * 0.5f;
            
            if(i < points.Length - 1)
            { 
                tris[triIndex] = vertIndex;    
                tris[triIndex + 1] = vertIndex + 2;  
                tris[triIndex + 2] = vertIndex + 1;  

                tris[triIndex + 3] = vertIndex + 1;    
                tris[triIndex + 4] = vertIndex + 2;  
                tris[triIndex + 5] = vertIndex + 3; 
            }
            
            vertIndex += 2;
            triIndex += 6;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        return mesh;
    }
}
