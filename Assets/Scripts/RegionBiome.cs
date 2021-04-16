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
    
    List<Vector3> forestPoints = new List<Vector3>();
    bool forestGenerated = false;


    [Header("Field")]
    [SerializeField] GameObject fieldDirtPrefab = null;
    [SerializeField] GameObject[] cropPrefabs = null;
    [SerializeField] int[] chancePerCrop = null;

    [Header("Town")]
    [SerializeField] GameObject centerObjectPrefab = null;
    [SerializeField] GameObject[] buildingPrefabs = null;
    [SerializeField] int[] chancePerBuilding = null;

    [Header("Mountain")]
    [SerializeField] GameObject rockPrefab = null;


    public void MakeBlank()
    {
        int whileSafeCatch = 0;

        //remove all children
        while(transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            whileSafeCatch++;

            //stop the while loop being infinite, just in case because this is in editor
            if(whileSafeCatch > 10000)
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



        //TODO Make this scale to any number of prefabs ===================================================================================================================================
        //randomly pick one of the crops
        for (int i = 0; i < 200; i++)
        {
            //spawn crop
            GameObject tempCrop = Instantiate(cropPrefabs[0], transform);
            Vector3 centrePoint = GetComponent<MeshFilter>().sharedMesh.vertices[0];
            tempCrop.transform.position = new Vector3(centrePoint.x + Random.Range(-80, 80), 70, centrePoint.z + Random.Range(-80, 80));
            PlaceObjectDown(tempCrop, 0.0f, true);
            tempCrop.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

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



    private void OnDrawGizmos()
    {
        if(!forestGenerated)
        return;

        for (int i = 0; i < Mathf.Min(40, forestPoints.Count); i++)
        {
            Gizmos.DrawSphere(forestPoints[i], 0.2f);
        }

        
    }



    void GenerateForest()
    {
        //delete contents of biome
        MakeBlank();



        PoissonDiscSampling pds = new PoissonDiscSampling();
        Vector3 centrePoint = GetComponent<MeshFilter>().sharedMesh.vertices[0];

        //generate points with poisson disc sampling starting at current regions centre
        forestPoints = pds.GeneratePoints(treeSpacing, treeK, 100, 100, transform.position);

        for (int i = 0; i < Mathf.Min(10, forestPoints.Count); i++)
        {
            forestPoints[i] += centrePoint + Vector3.up * 120;
        }


        for (int i = 0; i < Mathf.Min(10, forestPoints.Count); i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(forestPoints[i], Vector3.down * 200, out hit, 200f))
            {
                forestPoints[i] = hit.point;

                GameObject newTree = Instantiate(treePrefab, transform);
                newTree.transform.position = forestPoints[i];
                newTree.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                newTree.transform.position += transform.up * 1.3f;
                //newTree.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

                float randomScale = Random.Range(0.7f, 1f);
                newTree.transform.localScale = new Vector3(randomScale, randomScale, 1);
                newTrees.Add(newTree);
            }
        }
        forestGenerated = true;
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
            PlaceObjectDown(tempBuilding, 0.0f, true);
            tempBuilding.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

        }
    }


    void PlaceObjectDown(GameObject obj, float offset, bool rotateToNormal)
    {
        RaycastHit hit;
        if (Physics.Raycast(obj.transform.position, Vector3.down * 200, out hit, 200f))
        {
            obj.transform.position = hit.point;
            if(rotateToNormal)
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            obj.transform.position += transform.up * offset;
        }
    }


}
