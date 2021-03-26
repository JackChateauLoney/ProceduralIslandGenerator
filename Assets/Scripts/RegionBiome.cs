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

    [Header("Field")]
    [SerializeField] GameObject fieldDirtPrefab = null;


    [Header("Town")]
    [SerializeField] GameObject centerObjectPrefab = null;
    [SerializeField] GameObject buildingPrefab = null;


    [Header("Mountain")]
    [SerializeField] GameObject rockPrefab = null;



    public void GenerateBiome()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        foreach (var item in vertices)
        {
            //get width
            if(item.x > XregionMax)
                XregionMax = (int)item.x;
            if(item.x < XregionMin)
                XregionMin = (int)item.x;
            
            //get height
            if(item.z > ZregionMax)
                ZregionMax = (int)item.z;
            if(item.z < ZregionMin)
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
                break;
            default:
                break;
        }
        
    }




    void GenerateGrassland()
    {
        regionMat = GetComponentInParent<BiomeControl>().grassMaterial;

        GameObject grassLayer = Instantiate(gameObject, transform);
        grassLayer.transform.localPosition = Vector3.zero;
        grassLayer.GetComponent<MeshRenderer>().material = regionMat;
    
    }

    void GenerateField()
    {
        regionMat = GetComponentInParent<BiomeControl>().fieldMaterial;
        GetComponent<MeshRenderer>().material = regionMat;
    }


    void GenerateForest()
    {
        //PoissonDiscSampling pds = new PoissonDiscSampling();

        //List<Vector3> points = pds.PoissonDiscSample(treeSpacing, treeK, XregionMax, ZregionMax);

        GameObject newTree = Instantiate(treePrefab, transform);
        newTree.transform.position = GetComponent<MeshFilter>().mesh.vertices[0];
        newTree.transform.position = new Vector3(newTree.transform.position.x , 55, newTree.transform.position.z);
        StartCoroutine(FreezeObject(newTree));

        for (int i = 0; i < 300; i++)
        {
            GameObject tempTree = Instantiate(treePrefab, transform);
            tempTree.transform.position = GetComponent<MeshFilter>().mesh.vertices[0];
            tempTree.transform.position = new Vector3(newTree.transform.position.x + Random.Range(-80, 80) , 55, newTree.transform.position.z + Random.Range(-80, 80));
            tempTree.transform.Rotate(new Vector3(0, Random.Range(0f,1f), 0));
            StartCoroutine(FreezeObject(tempTree));
        }

    }



    void GenerateTown()
    {
        GameObject newCenterObject = Instantiate(centerObjectPrefab, transform);
        newCenterObject.transform.position = GetComponent<MeshFilter>().mesh.vertices[0];
        newCenterObject.transform.position = new Vector3(newCenterObject.transform.position.x , 55, newCenterObject.transform.position.z);
        StartCoroutine(FreezeObject(newCenterObject));
    

        for (int i = 0; i < 10; i++)
        {
            GameObject tempBuilding = Instantiate(buildingPrefab, transform);
            tempBuilding.transform.position = GetComponent<MeshFilter>().mesh.vertices[0];
            tempBuilding.transform.position = new Vector3(tempBuilding.transform.position.x + Random.Range(-80, 80) , 55, tempBuilding.transform.position.z + Random.Range(-80, 80));
            tempBuilding.transform.Rotate(new Vector3(0, Random.Range(0f,1f), 0));
            StartCoroutine(FreezeObject(tempBuilding));
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
