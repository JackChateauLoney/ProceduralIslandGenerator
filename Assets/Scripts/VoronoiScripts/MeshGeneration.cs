using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class MeshGeneration : MonoBehaviour
{
    [SerializeField] GameObject meshPrefabV = null;
    public Transform topLeft = null;
    public Transform bottomRight = null;
    public GameObject islandParent = null;

    [SerializeField] Material terrainShader = null;
    [SerializeField] float halfMapSize = 2000f;

    [Header("Biome probablility, should add up to 100")]
    [SerializeField] int grasslandChance = 50;
    [SerializeField] int forestChance = 30;
    [SerializeField] int fieldChance = 10;
    [SerializeField] int townChance = 5;
    [SerializeField] int mountainChance = 5;

    [Header("Island settings")]
    [SerializeField] int islandWidth = 4000;
    [SerializeField] int islandLength = 4000;
    PoissonDiscSampling pds = null;
    List<Vector3> points = new List<Vector3>();
    List<Vector3> activePoints = new List<Vector3>();

    Mesh mesh = null;
    Vector3[] vertices;
    int[] triangles;

    [HideInInspector] public bool generated = false;

    //initalise lists
    List<Vertex> vertexes = new List<Vertex>();
    List<Triangle> delaunayPoints = new List<Triangle>();

    List<VoronoiCell> voronoiCells;

    List<Mesh> voronoiMeshes = new List<Mesh>();



    private void OnEnable()
    {
        generated = false;
    }

    public void GenerateTerrain()
    {
        voronoiMeshes.Clear();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        pds = new PoissonDiscSampling();

        points.Clear();
        points = pds.GeneratePoints(200, 60, islandWidth, islandLength, Vector3.zero);

        activePoints.Clear();
        activePoints = new List<Vector3>();

        //only take the points that are not at 0
        for (int i = 0; i < points.Count; i++)
            if (points[i] != Vector3.zero)
                activePoints.Add(points[i]);

        //move points to line up with object
        for (int i = 0; i < activePoints.Count; i++)
            activePoints[i] += new Vector3(islandWidth / 2, 0, islandLength / 2) - transform.position;



        CreateShape();
        UpdateMesh();

        generated = true;

        DisplayVoronoiCells(voronoiCells);
    }



    void CreateShape()
    {
        for (int i = 0; i < activePoints.Count; i++)
            vertexes.Add(new Vertex(activePoints[i]));


        //create delaunay triangles
        delaunayPoints = DelaunayTriangulation.TriangulateByFlippingEdges(activePoints);

        //randomise height of terrain
        for (int i = 0; i < delaunayPoints.Count; i++)
        {
            delaunayPoints[i].v1.position.y += Random.Range(9, 12);
            delaunayPoints[i].v2.position.y += Random.Range(9, 12);
            delaunayPoints[i].v3.position.y += Random.Range(9, 12);
        }


        //init arrays for drawing mesh
        vertices = new Vector3[delaunayPoints.Count * 3];
        triangles = new int[delaunayPoints.Count * 3];
        int count = 0;

        List<Vector3> sites = new List<Vector3>();

        //convert triangles to mesh arrays
        for (int i = 0; count < delaunayPoints.Count; i += 3)
        {
            vertices[i] = delaunayPoints[count].v1.position;
            vertices[i + 1] = delaunayPoints[count].v2.position;
            vertices[i + 2] = delaunayPoints[count].v3.position;

            triangles[i] = i;
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
        List<Vector3> fullVoronoivertices = new List<Vector3>();

        for (int i = 0; i < cells.Count; i++)
        {
            VoronoiCell c = cells[i];
            Vector3 p1 = c.sitePos;




            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            vertices.Add(p1);
            fullVoronoivertices.Add(p1);

            for (int j = 0; j < c.edges.Count; j++)
            {
                Vector3 p3 = c.edges[j].v1;
                Vector3 p2 = c.edges[j].v2;

                //ensure points are valid
                if(float.IsNaN(p2.x) || float.IsNaN(p2.y) || float.IsNaN(p2.z))
                    p2 = new Vector3(1,1,1);
                if(float.IsNaN(p3.x) || float.IsNaN(p3.y) || float.IsNaN(p3.z))
                    p3 = new Vector3(1,1,1);

                vertices.Add(p2);
                vertices.Add(p3);
                fullVoronoivertices.Add(p2);
                fullVoronoivertices.Add(p3);

                triangles.Add(0);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);

            }

            Mesh triangleMesh = new Mesh();

            triangleMesh.Clear();

            triangleMesh.vertices = vertices.ToArray();

            triangleMesh.triangles = triangles.ToArray();

            triangleMesh.RecalculateNormals();
            
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
            newMesh.GetComponent<CellCollisions>().Init();

            //init may destroy cell, so return if it has
            if (newMesh == null)
                continue;


            DistributeBiomes(newMesh);


            newMesh.GetComponent<RegionBiome>().GenerateBiome();

        }
    }




    void DistributeBiomes(GameObject newMesh)
    {
        //random percentage
        int randType = Random.Range(0, 100);

        //check based on ranges set by user
        if (randType <= grasslandChance) //grassland
            newMesh.GetComponent<RegionBiome>().myBiome = RegionBiome.BiomeType.Grassland;
        else if (randType <= grasslandChance + forestChance) //forest
            newMesh.GetComponent<RegionBiome>().myBiome = RegionBiome.BiomeType.Forest;
        else if (randType <= grasslandChance + forestChance + fieldChance) //field
            newMesh.GetComponent<RegionBiome>().myBiome = RegionBiome.BiomeType.Field;
        else if (randType <= grasslandChance + forestChance + fieldChance + townChance) //town
            newMesh.GetComponent<RegionBiome>().myBiome = RegionBiome.BiomeType.Town;
        else if (randType <= grasslandChance + forestChance + fieldChance + townChance + mountainChance) //mountain
            newMesh.GetComponent<RegionBiome>().myBiome = RegionBiome.BiomeType.Mountain;
        else
            newMesh.GetComponent<RegionBiome>().myBiome = RegionBiome.BiomeType.Blank;

    }


}
