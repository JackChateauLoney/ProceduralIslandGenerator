using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
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
    List<Vertex> convexVertexList = new List<Vertex>();


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
            Gizmos.DrawSphere(convexVertexList[0].position + Vector3.up, 0.1f);
        Gizmos.color = Color.white;


        //show points along convex hull
        for (int i = 1; i < convexVertexList.Count; i++)
		{ 
			Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(convexVertexList[i].position + Vector3.up, 0.1f);
			Gizmos.color = Color.white;
		}

    }





    void CreateShape()
    {

        //initalise lists
        List<Vertex> vertexes = new List<Vertex>();
        
        List<Triangle> triangulatedConvexVertexList = new List<Triangle>();
        vertices = new Vector3[activePoints.Count];
        
        

        for (int i = 0; i < activePoints.Count; i++)
        {
            vertexes.Add(new Vertex(activePoints[i]));
            //convexVertexList.Add(new Vertex(activePoints[i]));

            triangulatedConvexVertexList.Add(new Triangle(new Vertex(Vector3.zero), new Vertex(Vector3.zero), new Vertex(Vector3.zero)));
        }


        //get convex hull of points
        convexVertexList = JarvisMarchAlgorithm.GetConvexHull(vertexes);

        //triangulate convex points
        triangulatedConvexVertexList = Trianglulation.TriangulateConvexPolygon(convexVertexList);


        
        for (int i = 0; i < triangulatedConvexVertexList.Count; i++)
        {
            Debug.Log("triangulatedConvexVertexList v1 before: " + triangulatedConvexVertexList[i].v1.position);
            Debug.Log("triangulatedConvexVertexList v2 before: " + triangulatedConvexVertexList[i].v2.position);
            Debug.Log("triangulatedConvexVertexList v3 before: " + triangulatedConvexVertexList[i].v3.position);
        }


        Debug.Log("triangulatedConvexVertexList size: " + triangulatedConvexVertexList.Count);

        //get triangulation of all points
        //List<Triangle> triangleSplittingPoints = new List<Triangle>();
        
        //triangleSplittingPoints = TriangleSplittingAlgorithm.TriangulatePoints(vertexes);


        

        int count = 0;
        Debug.Log("count: " + count);

        triangles = new int[triangulatedConvexVertexList.Count * 3];

        for (int i = 0; count < triangulatedConvexVertexList.Count; i += 3)
        {
            vertices[i    ] = triangulatedConvexVertexList[count].v1.position;
            vertices[i + 2] = triangulatedConvexVertexList[count].v2.position;
            vertices[i + 1] = triangulatedConvexVertexList[count].v3.position;

            triangles[i    ] = i;
            triangles[i + 2] = i + 2;
            triangles[i + 1] = i + 1;

            Debug.Log("triangulatedConvexVertexList v1 after: " + triangulatedConvexVertexList[count].v1.position);
            Debug.Log("triangulatedConvexVertexList v2 after: " + triangulatedConvexVertexList[count].v2.position);
            Debug.Log("triangulatedConvexVertexList v3 after: " + triangulatedConvexVertexList[count].v3.position);


            count++;
            Debug.Log("count: " + count);
        }

        for (int i = 0; i < triangles.Length; i++)
        {
            Debug.Log("triangles: " + triangles[i]);
        }
        
        

        


        //set order of triangles
        //triangles = new int[]
        //{
        //    0, 1, 2,
        //    0, 2, 3,
        //    0, 3, 4,
        //    0, 4, 5

        //};

        for (int i = 0; i < triangles.Length; i++)
        {
            Debug.Log("triangles: " + triangles[i]);
        }
         
    }


    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;        
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        
        
        
    }


}
