using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class MeshGeneration : MonoBehaviour
{

    public Transform topLeft = null;
    public Transform bottomRight = null;
    public GameObject voronoiParent = null;

    [SerializeField] Material terrainShader = null;
    [SerializeField] float halfMapSize = 10f;

    [SerializeField] const int sizeX = 50;
    [SerializeField] const int sizeY = 50;
    PoissonDiscSampling pds = null;
    List<Vector3> points;
    List<Vector3> activePoints;

    Mesh mesh = null;
    Vector3[] vertices;
    int[] triangles;

    [HideInInspector] public bool generated = false;

    //initalise lists
    List<Vertex> vertexes = new List<Vertex>();
    List<Triangle> delaunayPoints = new List<Triangle>();
    
    List<VoronoiCell> voronoiCells;

    Color[] gizmoColours = new Color[100];

    List<Mesh> voronoiMeshes = new List<Mesh>();
    [SerializeField] GameObject meshPrefabV = null;

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



        for (int i = 0; i < gizmoColours.Length; i++)
            gizmoColours[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        


            

        CreateShape();
        UpdateMesh();
    
        generated = true;    
        
        DisplayVoronoiCells(voronoiCells);
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
            vertexes.Add(new Vertex(activePoints[i]));
        
   

        //create delaunay triangles
        delaunayPoints = DelaunayTriangulation.TriangulateByFlippingEdges(activePoints);


        //init arrays for drawing mesh
        vertices = new Vector3[delaunayPoints.Count * 3];
        triangles = new int[delaunayPoints.Count * 3];
        int count = 0;

        List<Vector3> sites = new List<Vector3>();

        //convert triangles to mesh arrays
        for (int i = 0; count < delaunayPoints.Count; i += 3)
        {
            vertices[i    ] = delaunayPoints[count].v1.position;
            vertices[i + 1] = delaunayPoints[count].v2.position;
            vertices[i + 2] = delaunayPoints[count].v3.position;

            triangles[i    ] = i;
            triangles[i + 2] = i + 2;
            triangles[i + 1] = i + 1;

            sites.Add(delaunayPoints[count].v1.position);
            sites.Add(delaunayPoints[count].v2.position);
            sites.Add(delaunayPoints[count].v3.position);

            count++;
        }     

        //Points outside of the screen for voronoi which has some cells that are infinite
        float bigSize = halfMapSize * 5f;

        //Star shape which will give a better result when a cell is infinite large
        //When using other shapes, some of the infinite cells misses triangles
        sites.Add(new Vector3(0f, 0f, bigSize));
        sites.Add(new Vector3(0f, 0f, -bigSize));
        sites.Add(new Vector3(bigSize, 0f, 0f));
        sites.Add(new Vector3(-bigSize, 0f, 0f));

        voronoiCells = DelaunayToVoronoi.GenerateVoronoiDiagram(sites);
        

        
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


    //Display the voronoi diagram with mesh
    public void DisplayVoronoiCells(List<VoronoiCell> cells)
    {
        Mesh fullVoronoiMesh = new Mesh();
        List<Vector3> fullVoronoivertices = new List<Vector3>();
        List<int> fullVoronoitriangles = new List<int>();


        for (int i = 0; i < cells.Count; i++)
        {
            VoronoiCell c = cells[i];
            Vector3 p1 = c.sitePos;


            

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            vertices.Add(p1);
            fullVoronoivertices.Add(p1);

            //Gizmos.color = gizmoColours[i];

            for (int j = 0; j < c.edges.Count; j++)
            {
                Vector3 p3 = c.edges[j].v1;
                Vector3 p2 = c.edges[j].v2;

                vertices.Add(p2);
                vertices.Add(p3);
                fullVoronoivertices.Add(p2);
                fullVoronoivertices.Add(p3);

                triangles.Add(0);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);

                fullVoronoitriangles.Add(0);
                fullVoronoitriangles.Add(vertices.Count - 2);
                fullVoronoitriangles.Add(vertices.Count - 1);
            }

            Mesh triangleMesh = new Mesh();

            triangleMesh.Clear();

            triangleMesh.vertices = vertices.ToArray();

            triangleMesh.triangles = triangles.ToArray();

            triangleMesh.RecalculateNormals();

            //Gizmos.DrawMesh(triangleMesh);
            voronoiMeshes.Add(triangleMesh);
        }


        //create meshes for each region of voronoi
        for (int i = 0; i < voronoiMeshes.Count; i++)
        {
            GameObject newMesh = Instantiate(meshPrefabV, transform);
            newMesh.GetComponent<MeshFilter>().mesh = voronoiMeshes[i];
            newMesh.GetComponent<MeshCollider>().sharedMesh = null;
            newMesh.GetComponent<MeshCollider>().sharedMesh = voronoiMeshes[i];
            newMesh.GetComponent<Renderer>().material = terrainShader;
        }
    }
}
