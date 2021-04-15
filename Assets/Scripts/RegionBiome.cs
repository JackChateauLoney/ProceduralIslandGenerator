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


    int XregionMax = 0, XregionMin = 9999, ZregionMax = 0, ZregionMin = 9999;
    [SerializeField] Material regionMat = null;


    [Header("Forest")]
    [SerializeField] int treeSpacing = 1;
    [SerializeField] int treeK = 30;
    [SerializeField] GameObject treePrefab = null;
    List<GameObject> newTrees = new List<GameObject>();
    List<Vector3> randPoints = new List<Vector3>();


    [Header("Field")]
    [SerializeField] GameObject fieldDirtPrefab = null;


    [Header("Town")]
    [SerializeField] GameObject centerObjectPrefab = null;
    [SerializeField] GameObject[] buildingPrefabs = null;
    [SerializeField] int[] chancePerBuilding = null;

    [Header("Mountain")]
    [SerializeField] GameObject rockPrefab = null;



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
        if (transform.parent.GetComponentInParent<BiomeControl>())
            regionMat = GetComponentInParent<BiomeControl>().grassMaterial;

        GameObject grassLayer = Instantiate(gameObject, transform);
        grassLayer.transform.localPosition = Vector3.zero;
        grassLayer.GetComponent<MeshRenderer>().material = regionMat;

    }

    void GenerateField()
    {
        if (transform.parent.GetComponentInParent<BiomeControl>())
        {
            regionMat = GetComponentInParent<BiomeControl>().fieldMaterial;
            GetComponent<MeshRenderer>().material = regionMat;
        }
    }

    void GenerateMountain()
    {
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

    void GenerateForest()
    {
        PoissonDiscSampling pds = new PoissonDiscSampling();

        Vector3 centrePoint = GetComponent<MeshFilter>().sharedMesh.vertices[0];

        //generate points with poisson disc sampling starting at current regions centre
        List<Vector3> points = pds.GeneratePoints(treeSpacing, treeK, 100, 100, transform.position);

        for (int i = 0; i < Mathf.Min(10, points.Count); i++)
        {
            points[i] += centrePoint + Vector3.up * 120;
        }


        for (int i = 0; i < Mathf.Min(10, points.Count); i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(points[i], Vector3.down * 200, out hit, 200f))
            {
                points[i] = hit.point;

                GameObject newTree = Instantiate(treePrefab, transform);
                newTree.transform.position = points[i];
                newTree.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                newTree.transform.position += transform.up * 1.3f;
                //newTree.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

                float randomScale = Random.Range(0.7f, 1f);
                newTree.transform.localScale = new Vector3(randomScale, randomScale, 1);
                newTrees.Add(newTree);
            }
        }

    }


    void GenerateTown()
    {

        Vector3 centrePoint = GetComponent<MeshFilter>().sharedMesh.vertices[0];

        GameObject newCenterObject = Instantiate(centerObjectPrefab, transform);
        newCenterObject.transform.position = GetComponent<MeshFilter>().sharedMesh.vertices[0];
        newCenterObject.transform.position = new Vector3(newCenterObject.transform.position.x, 55, newCenterObject.transform.position.z);

        PlaceObjectDown(newCenterObject, 1.5f);

        //TODO Make this scale to any number of prefabs ===================================================================================================================================
        //randomly pick one of the buildings
        for (int i = 0; i < 10; i++)
        {
            int selectedPrefab = 0;
            int num = Random.Range(0, 100);
            if (num <= chancePerBuilding[0])
            {
                selectedPrefab = 0;
            }
            else if (num <= chancePerBuilding[0] + chancePerBuilding[1])
            {
                selectedPrefab = 1;
            }
            else if (num <= chancePerBuilding[0] + chancePerBuilding[1] + chancePerBuilding[2])
            {
                selectedPrefab = 2;
            }

            //spawn building
            GameObject tempBuilding = Instantiate(buildingPrefabs[selectedPrefab], transform);
            tempBuilding.transform.position = new Vector3(centrePoint.x + Random.Range(-80, 80), 70, centrePoint.z + Random.Range(-80, 80));
            PlaceObjectDown(tempBuilding, 0.0f);

        }
    }


    void PlaceObjectDown(GameObject obj, float offset)
    {
        RaycastHit hit;
        if (Physics.Raycast(obj.transform.position, Vector3.down * 200, out hit, 200f))
        {
            obj.transform.position = hit.point;
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            obj.transform.position += transform.up * offset;
        }
    }

    IEnumerator FreezeObject(GameObject obj)
    {
        for (float i = 0; i < 2f; i += Time.deltaTime)
            yield return null;

        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        obj.GetComponent<Rigidbody>().isKinematic = true;
    }










}
