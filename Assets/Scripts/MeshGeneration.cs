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
    }



    void CreateShape()
    {

        //initalise lists
        List<Vertex> vertexes = new List<Vertex>();
        List<Vertex> convexVertexList = new List<Vertex>();
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
        



        //get triangulation of all points
        List<Triangle> triangleSplittingPoints = new List<Triangle>();

        triangleSplittingPoints = TriangleSplittingAlgorithm.TriangulatePoints(vertexes);

        
        //int convexSize  = triangulatedConvexVertexList.Count;
        


        
        //int count = 0;
        //convert points from poisson into vertices array
        //for (int i = 0; i < convexSize - 2; i += 3)
        //{
        //    vertices[i] = triangulatedConvexVertexList[count].v1.position;

        //    vertices[i+1] = triangulatedConvexVertexList[count].v2.position;

        //    vertices[i+2] = triangulatedConvexVertexList[count].v3.position;

        //    count++;

        //}


        //ensure multiple of 3 to make triangles
        //while(convexSize % 3 != 0)
        //    convexSize--;


        //triangles = new int[convexSize];
        
        Debug.Log("vertices Size: " + vertices.Length);


        //if(convexVertexList.Count < 6)
        //    return;


        //for (int i = 0; i < convexVertexList.Count; i++)
        //{
        //    vertices[i] = convexVertexList[i].position;
        //}


        vertices[0] = triangleSplittingPoints[0].v1.position;
        vertices[1] = triangleSplittingPoints[0].v2.position;
        vertices[2] = triangleSplittingPoints[0].v3.position;

        vertices[3] = triangleSplittingPoints[1].v1.position;
        vertices[4] = triangleSplittingPoints[1].v2.position;
        vertices[5] = triangleSplittingPoints[1].v3.position;

        vertices[6] = triangleSplittingPoints[2].v1.position;
        vertices[7] = triangleSplittingPoints[2].v2.position;
        vertices[8] = triangleSplittingPoints[2].v3.position;

        vertices[9] = triangleSplittingPoints[3].v1.position;
        vertices[10] = triangleSplittingPoints[3].v2.position;
        vertices[11] = triangleSplittingPoints[3].v3.position;
                                              
        vertices[12] = triangleSplittingPoints[4].v1.position;
        vertices[13] = triangleSplittingPoints[4].v2.position;
        vertices[14] = triangleSplittingPoints[4].v3.position;


        Debug.Log("triangleSplittingPoints 0: " + triangleSplittingPoints[0].v1.position);
        Debug.Log("triangleSplittingPoints 0: " + triangleSplittingPoints[1].v1.position);
        Debug.Log("triangleSplittingPoints 0: " + triangleSplittingPoints[2].v1.position);

        Debug.Log("triangleSplittingPoints 0: " + triangleSplittingPoints[3].v1.position);
        Debug.Log("triangleSplittingPoints 0: " + triangleSplittingPoints[4].v1.position);
        Debug.Log("triangleSplittingPoints 0: " + triangleSplittingPoints[5].v1.position);



        
        //Debug.Log("Vertices 0: " + vertices[0]);
        //Debug.Log("Vertices 1: " + vertices[1]);
        //Debug.Log("Vertices 2: " + vertices[2]);
        //Debug.Log("Vertices 3: " + vertices[3]);
        //Debug.Log("Vertices 4: " + vertices[4]);
        //Debug.Log("Vertices 5: " + vertices[5]);
        //Debug.Log("Vertices 6: " + vertices[6]);
        //Debug.Log("Vertices 7: " + vertices[7]);
        //Debug.Log("Vertices 8: " + vertices[8]);
        //Debug.Log("Vertices 9: " + vertices[9]);
        //Debug.Log("Vertices 10: " + vertices[10]);
        //Debug.Log("Vertices 11: " + vertices[11]);
        //Debug.Log("Vertices 12: " + vertices[12]);
        //Debug.Log("Vertices 13: " + vertices[13]);
        //Debug.Log("Vertices 14: " + vertices[14]);

        //vertices[0] = convexVertexList[0].position;
        //vertices[1] = convexVertexList[1].position;
        //vertices[2] = convexVertexList[2].position;
        //              
        //vertices[3] = convexVertexList[3].position;
        //vertices[4] = convexVertexList[4].position;
        //vertices[5] = convexVertexList[5].position;
        //
        //vertices[6] = convexVertexList[6].position;
        //vertices[7] = convexVertexList[7].position;
        //vertices[8] = convexVertexList[8].position;


        //set order of triangles
        triangles = new int[]
        {
            0, 2, 1,
            1, 3, 2,
            2, 4, 3,
            3, 5, 4,
            4, 6, 5,
            5, 7, 6,


        };
    }


    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        Debug.Log("Vertices: " + vertices.Length);

        
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        
        
    }


}
