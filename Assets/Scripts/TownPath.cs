using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownPath : MonoBehaviour
{
    public static Mesh ExtrudeAlongPath(Vector3[] points, float width)
    {
        if (points.Length < 2)
            return null;
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
    
        for (int i = 0; i < points.Length; i++) 
        {
            if(i != points.Length-1){
                Vector3 perpendicularDirection = new Vector3(-( points[i+1].z-points[i].z ), points[i].y, points[i+1].x -points[i].x).normalized;
                verts.Add (points[i]+perpendicularDirection*width);
                norms.Add (Vector3.up);
                verts.Add (points[i]+perpendicularDirection*-width);
                norms.Add (Vector3.up);
            }
            else
            {
                Vector3 perpendicularDirection = new Vector3(-( points[i].z-points[i-1].z ), points[i].y, points[i].x -points[i-1].x).normalized;
                verts.Add (points[i]+perpendicularDirection*-width);
                norms.Add (Vector3.up);
                verts.Add (points[i]+perpendicularDirection*width);
                norms.Add (Vector3.up);
            }
        }
        m.vertices = verts.ToArray ();
        m.normals = norms.ToArray ();
    
        List<int> tris = new List<int> ();
        for(int i = 0; i < m.vertices.Length-3; i++)
        {
            if(i%2 == 0){
            tris.Add(i+2);
            tris.Add(i+1);
            tris.Add(i);
            }
            else
            {
                tris.Add(i);
                tris.Add(i+1);
                tris.Add(i+2);
            }
        }
        m.triangles = tris.ToArray ();
    
        m.name = "pathMesh";
        m.RecalculateNormals ();
        m.RecalculateBounds ();
        m.Optimize ();
        return m;
    }
}
