using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using csDelaunay;


[RequireComponent(typeof(MeshFilter))]
public class VoronoiMesh : MonoBehaviour
{

    // The number of polygons/sites we want
    public int polygonNumber = 200;
 
    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;


    [SerializeField] const int sizeX = 50;
    [SerializeField] const int sizeY = 50;
    PoissonDiscSampling pds = null;
    List<Vector3> points;
    List<Vector3> activePoints;

    Mesh mesh = null;
    Vector3[] vertices;
    int[] triangles;


    private void OnDrawGizmos()
    {
        if(sites == null)
            return;
        if(sites.Count < 1)
            return;

        Debug.Log("Drawing spheres in voronoi mesh");

        Gizmos.color = Color.green;
        foreach (KeyValuePair<Vector2f,Site> kv in sites) 
            Gizmos.DrawSphere(new Vector3((int)kv.Key.x, 1f, (int)kv.Key.y), 0.1f);
			
        Gizmos.color = Color.white;
    }


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
        //UpdateMesh();
    }


    private void DisplayVoronoiDiagram() 
    {
        Texture2D tx = new Texture2D(512,512);
        foreach (KeyValuePair<Vector2f,Site> kv in sites) 
        {
            tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);
        }

        foreach (Edge edge in edges) 
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;
 
            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();
 
        GetComponent<Renderer>().material.mainTexture = tx;
        Debug.Log("Rendering texture to quad");
        //renderer.material.mainTexture = tx;
    }

    // Bresenham line algorithm
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0) {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;
       
        int dx = Mathf.Abs(x1-x0);
        int dy = Mathf.Abs(y1-y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx-dy;
       
        while (true) {
            tx.SetPixel(x0+offset,y0+offset,c);
           
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2*err;
            if (e2 > -dy) {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx) {
                err += dx;
                y0 += sy;
            }
        }
    }


    void CreateShape()
    {

        //initalise lists
        List<Vertex> vertexes = new List<Vertex>();

        vertices = new Vector3[activePoints.Count];
        
        

        for (int i = 0; i < activePoints.Count; i++)
        {
            vertexes.Add(new Vertex(activePoints[i]));
        }




        // Create your sites (lets call that the center of your polygons)

        List<Vector2f> points2d = new List<Vector2f>();

        for (int i = 0; i < activePoints.Count; i++)
        {
            points2d.Add(new Vector2f(activePoints[i].x,activePoints[i].y));
        }   
        

        
        List<Vector2f> points = points2d;
       
        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        Rectf bounds = new Rectf(0,0,512,512);
       
        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        Voronoi voronoi = new Voronoi(points,bounds,5);
 
        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);
 
        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;
         
        //DisplayVoronoiDiagram();


        vertices[0] = new Vector3(points[0].x, points[0].y, 0);
        vertices[1] = new Vector3(points[1].x, points[1].y, 0);
        vertices[2] = new Vector3(points[2].x, points[2].y, 0);
        
        //vertices[3] = triangleSplittingPoints[1].v1.position;
        //vertices[4] = triangleSplittingPoints[1].v2.position;
        //vertices[5] = triangleSplittingPoints[1].v3.position;
        //
        //vertices[6] = triangleSplittingPoints[2].v1.position;
        //vertices[7] = triangleSplittingPoints[2].v2.position;
        //vertices[8] = triangleSplittingPoints[2].v3.position;
        //
        //vertices[9] = triangleSplittingPoints[3].v1.position;
        //vertices[10] = triangleSplittingPoints[3].v2.position;
        //vertices[11] = triangleSplittingPoints[3].v3.position;
        //                                      
        //vertices[12] = triangleSplittingPoints[4].v1.position;
        //vertices[13] = triangleSplittingPoints[4].v2.position;
        //vertices[14] = triangleSplittingPoints[4].v3.position;

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
