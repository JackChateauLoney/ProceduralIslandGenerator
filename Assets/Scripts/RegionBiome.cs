using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionBiome : MonoBehaviour
{
    public enum BiomeType
    {
        Blank,
        Grassland,
        Forest,
        Field,
        Town,
        Mountain,
    };

    public BiomeType myBiome = BiomeType.Blank;
    public float centreHeight = 0.0f;
    [HideInInspector] public Vector3 originalCentreHeight = Vector3.zero;

    int XregionMax = 0, XregionMin = 9999, ZregionMax = 0, ZregionMin = 9999;
    [SerializeField] Material regionMat = null;


    [Header("Forest")]
<<<<<<< Updated upstream
    [SerializeField] int treeAreaWidth = 200;
    [SerializeField] int treeAreaLength = 200;
    [SerializeField] int treeSpacing = 1;
    [SerializeField] int treeK = 30;
    [SerializeField] GameObject treePrefab = null;
    List<GameObject> newTrees = new List<GameObject>();

    List<Vector3> randPoints = new List<Vector3>();

    List<Vector3> forestPoints = new List<Vector3>();
    bool forestGenerated = false;
=======
    [SerializeField] GameObject treePrefab = null;
    [SerializeField] int treeAreaWidth = 400;
    [SerializeField] int treeAreaLength = 400;
    [SerializeField] int treeSpacing = 7;
    [SerializeField] int treeK = 120;
    [SerializeField] float treeOffset = 1.5f;
    [SerializeField] float treeScaleMin = 2f;
    [SerializeField] float treeScaleMax = 3f;
    [Space(10)]
    [SerializeField] GameObject bushPrefab = null;
    [SerializeField] int bushSpacing = 16;
    [SerializeField] float bushOffset = 0.5f;
    [SerializeField] float bushScaleMin = 1.2f;
    [SerializeField] float bushScaleMax = 0.8f;
>>>>>>> Stashed changes

    List<Vector3> forestPoints = new List<Vector3>();
    List<Vector3> bushPoints = new List<Vector3>();

    [Header("Field")]
    [SerializeField] int cropAreaWidth = 400;
    [SerializeField] int cropAreaLength = 400;
    [SerializeField] int cropSpacing = 12;
    [SerializeField] int cropK = 120;
    [SerializeField] float cropOffset = -0.5f;
    [SerializeField] float cropScaleMin = 0.2f;
    [SerializeField] float cropScaleMax = 1.0f;
    [SerializeField] GameObject[] cropPrefabs = null;
    [SerializeField] int[] chancePerCrop = null;
    List<Vector3> cropPoints = new List<Vector3>();

    [Header("Town")]
    [SerializeField] GameObject centerObjectPrefab = null;
    [SerializeField] GameObject[] buildingPrefabs = null;
    [SerializeField] int[] chancePerBuilding = null;
    [SerializeField] int buildingAreaWidth = 400;
    [SerializeField] int buildingAreaLength = 400;
    [SerializeField] int buildingSpacing = 25;
    [SerializeField] int buildingK = 120;
    [SerializeField] float buildingOffset = 0.0f;
    [SerializeField] float buildingScaleMin = 0.2f;
    [SerializeField] float buildingScaleMax = 1.0f;
    List<Vector3> buildingPoints = new List<Vector3>();

    List<Vector3> roadPoints = new List<Vector3>();
    bool generatedTown = false;
    bool roadGenerated = false;
    int randPoint1 = 0;
    int randPoint2 = 0;

    [Header("Mountain")]
    [SerializeField] GameObject rockPrefab = null;


    public void MakeBlank()
    {
        int whileSafeCatch = 0;

        //remove all children
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            whileSafeCatch++;

            //stop the while loop being infinite, just in case because this is in editor
            if (whileSafeCatch > 10000)
            {
                Debug.Log("While loop went on for too long, exiting to keep the editor safe :)");
                break;
            }
        }


        //reset material
        if (transform.parent.GetComponentInParent<BiomeControl>())
            regionMat = GetComponentInParent<BiomeControl>().grassMaterial;


    }


    public void GenerateBiome()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        foreach (var item in vertices)
        {
            //get width
            if (item.x > XregionMax)
                XregionMax = (int)item.x;
            if (item.x < XregionMin)
                XregionMin = (int)item.x;

            //get height
            if (item.z > ZregionMax)
                ZregionMax = (int)item.z;
            if (item.z < ZregionMin)
                ZregionMin = (int)item.z;
        }

        centreHeight = vertices[0].y;
        originalCentreHeight = vertices[0];




        switch (myBiome)
        {
            case BiomeType.Blank:
                break;
            case BiomeType.Grassland:
                {
                    GenerateGrassland();
                    break;
                }
            case BiomeType.Forest:
                {
                    GenerateForest();
                    break;
                }
            case BiomeType.Field:
                {
                    GenerateField();
                    break;
                }
            case BiomeType.Town:
                {
                    GenerateTown();
                    break;
                }
            case BiomeType.Mountain:
                {
                    GenerateMountain();
                    break;
                }
            default:
                break;
        }

    }




    void GenerateGrassland()
    {
        //delete contents of biome
        MakeBlank();

        //set material for grass
        if (transform.parent.GetComponentInParent<BiomeControl>())
            regionMat = GetComponentInParent<BiomeControl>().grassMaterial;

        //spawn grass layer
        GameObject grassLayer = Instantiate(gameObject, transform);
        grassLayer.transform.localPosition = Vector3.zero;
        grassLayer.GetComponent<MeshRenderer>().material = regionMat;

    }

    void GenerateField()
    {
        //delete contents of biome
        MakeBlank();

        //set material for field
        if (transform.parent.GetComponentInParent<BiomeControl>())
        {
            regionMat = GetComponentInParent<BiomeControl>().fieldMaterial;
            GetComponent<MeshRenderer>().material = regionMat;
        }


        cropPoints = PoissonPoints(cropSpacing, cropK, cropAreaWidth, cropAreaLength, transform.position);

        for (int i = 0; i < Mathf.Min(1000, cropPoints.Count); i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(cropPoints[i], Vector3.down * 200, out hit, 200f))
            {
                //only spawn on this biome
                if (hit.transform.gameObject != gameObject)
                    continue;

                cropPoints[i] = hit.point;

                GameObject newCrop = Instantiate(cropPrefabs[0], transform);
                newCrop.transform.position = cropPoints[i];
                newCrop.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                newCrop.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

                float randomScale = Random.Range(cropScaleMin, cropScaleMax);
                newCrop.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                newCrop.transform.position += transform.up * randomScale * cropOffset;
            }
        }
    }






    void GenerateMountain()
    {
        //delete contents of biome
        MakeBlank();

        //set material for mountain
        if (transform.parent.GetComponentInParent<BiomeControl>())
        {
            regionMat = GetComponentInParent<BiomeControl>().mountainMaterial;
            GetComponent<MeshRenderer>().material = regionMat;
        }

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertices = mesh.vertices;
        vertices[0] += new Vector3(0, 120, 0);
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }



<<<<<<< Updated upstream
    private void OnDrawGizmos()
    {
        if (!forestGenerated)
            return;

        for (int i = 0; i < Mathf.Min(40, forestPoints.Count); i++)
        {
            Gizmos.DrawSphere(forestPoints[i], 0.2f);
        }


    }



=======
>>>>>>> Stashed changes
    void GenerateForest()
    {
        //delete contents of biome
        MakeBlank();

        forestPoints = PoissonPoints(treeSpacing, treeK, treeAreaWidth, treeAreaLength, transform.position);
        bushPoints = PoissonPoints(bushSpacing, treeK, treeAreaWidth, treeAreaLength, transform.position);

        //place bushes
        for (int i = 0; i < Mathf.Min(100, bushPoints.Count); i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(bushPoints[i], Vector3.down * 200, out hit, 200f))
            {
                //only spawn bushes on this biome
                if (hit.transform.gameObject != gameObject)
                    continue;

                bushPoints[i] = hit.point;

<<<<<<< Updated upstream
        //generate points with poisson disc sampling starting at current regions centre
        forestPoints = pds.GeneratePoints(treeSpacing, treeK, treeAreaWidth, treeAreaLength, transform.position);
        List<Vector3> newForestPoints = new List<Vector3>();


        for (int i = 0; i < forestPoints.Count; i++)
        {
            if (forestPoints[i] != Vector3.zero)
                newForestPoints.Add(forestPoints[i]);
        }

        Debug.Log("New Forest ======================");
        for (int i = 0; i < Mathf.Min(1000, newForestPoints.Count); i++)
        {
            Debug.Log("New Forest Points: " + newForestPoints[i]);
            newForestPoints[i] += centrePoint + Vector3.up * 120;
            newForestPoints[i] -= new Vector3(treeAreaWidth / 2, 0, treeAreaLength / 2);
        }
=======
                GameObject newBush = Instantiate(bushPrefab, transform);
                newBush.transform.position = bushPoints[i];
                newBush.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                newBush.transform.Rotate(new Vector3(-90, Random.Range(0, 360), 0));
>>>>>>> Stashed changes

                float randomScale = Random.Range(bushScaleMin, bushScaleMax);
                newBush.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                newBush.transform.position += transform.up * randomScale * bushOffset;
            }
        }

<<<<<<< Updated upstream
        for (int i = 0; i < Mathf.Min(100, newForestPoints.Count); i++)
=======
        //place trees
        for (int i = 0; i < Mathf.Min(1000, forestPoints.Count); i++)
>>>>>>> Stashed changes
        {
            if (Random.Range(0, 10) == 1)
                continue;

            RaycastHit hit;
            if (Physics.Raycast(newForestPoints[i], Vector3.down * 200, out hit, 200f))
            {
<<<<<<< Updated upstream
                newForestPoints[i] = hit.point;
=======
                //only spawn trees on this biome
                if (hit.transform.gameObject != gameObject)
                    continue;

                forestPoints[i] = hit.point;
>>>>>>> Stashed changes

                GameObject newTree = Instantiate(treePrefab, transform);
                newTree.transform.position = newForestPoints[i];
                newTree.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
<<<<<<< Updated upstream
                
                //newTree.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

                float randomScale = Random.Range(2f, 3f);
                newTree.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                newTree.transform.position += transform.up * randomScale;
                newTrees.Add(newTree);
=======

                newTree.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

                float randomScale = Random.Range(treeScaleMin, treeScaleMin);
                newTree.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                newTree.transform.position += transform.up * randomScale * treeOffset;
>>>>>>> Stashed changes
            }
        }
    }



    private void OnDrawGizmos()
    {
        if (generatedTown)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(roadPoints[0], (roadPoints[randPoint1] + roadPoints[randPoint1 - 1]) / 2);
            Gizmos.DrawLine(roadPoints[0], (roadPoints[randPoint2] + roadPoints[randPoint2 - 1]) / 2);
            Gizmos.color = Color.white;
        }
    }

    void GenerateTown()
    {
        //delete contents of biome
        MakeBlank();


        //get centre of biome to place monument
        Vector3 centrePoint = GetComponent<MeshFilter>().sharedMesh.vertices[0];
        GameObject newCenterObject = Instantiate(centerObjectPrefab, transform);
        newCenterObject.transform.position = GetComponent<MeshFilter>().sharedMesh.vertices[0];
        newCenterObject.transform.position = new Vector3(newCenterObject.transform.position.x, 90, newCenterObject.transform.position.z);
        PlaceObjectDown(newCenterObject, -0.2f, false);

        //first road point
        roadPoints.Clear();
        roadPoints.Add(centrePoint);

        //draw road to edge of biome
        Vector3[] edgePoints = GetComponent<MeshFilter>().sharedMesh.vertices;

        for (int i = 0; i < edgePoints.Length; i++)
            roadPoints.Add(edgePoints[i]);


        if (!roadGenerated)
        {
            //new road 
            randPoint1 = Random.Range(2, roadPoints.Count);
            randPoint2 = Random.Range(2, roadPoints.Count - 1);
            if (randPoint2 == randPoint1)
                randPoint2++;
            roadGenerated = true;
        }
        generatedTown = true;



        //place buildings
        buildingPoints = PoissonPoints(buildingSpacing, buildingK, buildingAreaWidth, buildingAreaLength, transform.position);

        for (int i = 0; i < Mathf.Min(1000, buildingPoints.Count); i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(buildingPoints[i], Vector3.down * 200, out hit, 200f))
            {
                //only spawn on this biome
                if (hit.transform.gameObject != gameObject)
                    continue;

                buildingPoints[i] = hit.point;

                GameObject newBuilding = Instantiate(buildingPrefabs[0], transform);
                newBuilding.transform.position = buildingPoints[i];
                newBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                newBuilding.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

                float randomScale = Random.Range(buildingScaleMin, buildingScaleMax);
                newBuilding.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                newBuilding.transform.position += transform.up * randomScale * buildingOffset;
            }
        }
    }


    void PlaceObjectDown(GameObject obj, float offset, bool rotateToNormal)
    {
        RaycastHit hit;
        if (Physics.Raycast(obj.transform.position, Vector3.down * 200, out hit, 200f))
        {
            obj.transform.position = hit.point;
            if (rotateToNormal)
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            obj.transform.position += transform.up * offset;
        }
    }



    private List<Vector3> PoissonPoints(int spacing, int k, int areaWidth, int areaLength, Vector3 pos)
    {
        PoissonDiscSampling pds = new PoissonDiscSampling();
        Vector3 centrePoint = GetComponent<MeshFilter>().sharedMesh.vertices[0];

        //generate points with poisson disc sampling starting at current regions centre
        List<Vector3> points = pds.GeneratePoints(spacing, k, areaWidth, areaLength, pos);
        List<Vector3> newPoints = new List<Vector3>();

        //remove extra points from poisson disc sampler
        for (int i = 0; i < points.Count; i++)
            if (points[i] != Vector3.zero)
                newPoints.Add(points[i]);


        //move to centre of biome
        for (int i = 0; i < Mathf.Min(1000, newPoints.Count); i++)
        {
            newPoints[i] += centrePoint + Vector3.up * 120;
            newPoints[i] -= new Vector3(areaWidth / 2, 0, areaLength / 2);
        }

        return newPoints;
    }

}
