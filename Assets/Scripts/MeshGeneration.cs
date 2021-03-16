using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class MeshGeneration : MonoBehaviour
{

    [SerializeField] const int sizeX = 50;
    [SerializeField] const int sizeY = 50;
    PoissonDiscSampling pds = null;
    List<Vector3> points;
    List<Vector3> activePoints;

    Mesh mesh = null;
    Vector3[] vertices;
    int[] triangles;

    bool generated = false;

    //initalise lists
    List<Vertex> vertexes = new List<Vertex>();
    List<Vertex> convexVertexList = new List<Vertex>();
    List<Triangle> triangulatedConvexVertexList = new List<Triangle>();

    List<Triangle> triangleSplittingPoints = new List<Triangle>();
    List<Triangle> delaunayPoints = new List<Triangle>();
    

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;


        pds = GetComponent<PoissonDiscSampling>();
        points = pds.points;


        activePoints = new List<Vector3>();

        //only take the points that are not at 0
        for (int i = 0; i < points.Count; i++)
            if(points[i] != Vector3.zero)
                activePoints.Add(points[i]);
        

        
            

        CreateShape();
        UpdateMesh();
    
        generated = true;    
        
    }



    void OnDrawGizmos()
    {
        if(!generated)
            return;

        Gizmos.color = Color.green;
            Gizmos.DrawSphere(delaunayPoints[0].v1.position + Vector3.up, 0.1f);
        Gizmos.color = Color.white;


        //show points within delaunay
        for (int i = 1; i < delaunayPoints.Count; i++)
		{ 
			Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(delaunayPoints[i].v1.position + Vector3.up, 0.1f);
            Gizmos.DrawSphere(delaunayPoints[i].v2.position + Vector3.up, 0.1f);
            Gizmos.DrawSphere(delaunayPoints[i].v3.position + Vector3.up, 0.1f);
		}

        //show points
        for (int i = 1; i < vertexes.Count; i++)
		{ 
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(vertexes[i].position + Vector3.up * 2, 0.1f);
			Gizmos.color = Color.white;
        }
    }





    void CreateShape()
    {
        for (int i = 0; i < activePoints.Count; i++)
        {
            vertexes.Add(new Vertex(activePoints[i]));
            //convexVertexList.Add(new Vertex(activePoints[i]));

            //triangulatedConvexVertexList.Add(new Triangle(new Vertex(Vector3.zero), new Vertex(Vector3.zero), new Vertex(Vector3.zero)));
        }


        //get convex hull of points
        //convexVertexList = JarvisMarchAlgorithm.GetConvexHull(vertexes);

        //triangulate convex points
        //triangulatedConvexVertexList = Trianglulation.TriangulateConvexPolygon(convexVertexList);


        //get triangulation of all points
        //triangleSplittingPoints = TriangleSplittingAlgorithm.TriangulatePoints(vertexes);
        //
        //
        //Debug.Log("triangleSplittingPoints.Count: " + triangleSplittingPoints.Count);
        //        
        //vertices = new Vector3[triangleSplittingPoints.Count * 3];
        //
        //int count = 0;
        //triangles = new int[triangleSplittingPoints.Count * 3];
        //
        //for (int i = 0; count < triangleSplittingPoints.Count; i += 3)
        //{
        //    vertices[i    ] = triangleSplittingPoints[count].v1.position;
        //    vertices[i + 2] = triangleSplittingPoints[count].v2.position;
        //    vertices[i + 1] = triangleSplittingPoints[count].v3.position;
        //
        //    triangles[i    ] = i;
        //    triangles[i + 2] = i + 2;
        //    triangles[i + 1] = i + 1;
        //
        //    count++;
        //}         

        
        



        //create delaunay triangles
        delaunayPoints = DelaunayTriangulation.TriangulateByFlippingEdges(activePoints);


        //init arrays for drawing mesh
        vertices = new Vector3[delaunayPoints.Count * 3];
        triangles = new int[delaunayPoints.Count * 3];
        int count = 0;

        //convert triangles to mesh arrays
        for (int i = 0; count < delaunayPoints.Count; i += 3)
        {
            vertices[i    ] = delaunayPoints[count].v1.position;
            vertices[i + 1] = delaunayPoints[count].v2.position;
            vertices[i + 2] = delaunayPoints[count].v3.position;

            triangles[i    ] = i;
            triangles[i + 2] = i + 2;
            triangles[i + 1] = i + 1;

            count++;
        }     




    }


    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;        
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        
        //set collisions for mesh
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        
    }


}
