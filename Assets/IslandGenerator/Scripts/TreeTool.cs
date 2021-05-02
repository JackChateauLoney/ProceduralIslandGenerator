using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTool : MonoBehaviour
{

    [SerializeField] GameObject treePrefab = null;
    [SerializeField] int treeAreaWidth = 400;
    [SerializeField] int treeAreaLength = 400;
    [SerializeField] int treeSpacing = 7;
    [SerializeField] int treeK = 120;
    [SerializeField] float treeOffset = 1.5f;
    [SerializeField] float treeScaleMin = 2f;
    [SerializeField] float treeScaleMax = 3f;
    List<Vector3> forestPoints = new List<Vector3>();
    List<GameObject> activeTrees = new List<GameObject>();
    [HideInInspector] public bool showBox = false;

    public void CreateForest()
    {
        DeleteForest();


        forestPoints = PoissonPoints(treeSpacing, treeK, treeAreaWidth, treeAreaLength, transform.position);


        Debug.Log("forest points length: " + forestPoints.Count);

        //place trees
        for (int i = 0; i < Mathf.Min(1000, forestPoints.Count); i++)
        {
            if (Random.Range(0, 10) == 1)
                continue;

            RaycastHit hit;
            if (Physics.Raycast(forestPoints[i], Vector3.down * 200, out hit, 200f))
            {
                forestPoints[i] = hit.point;

                GameObject newTree = Instantiate(treePrefab, transform);
                newTree.transform.position = forestPoints[i];
                //newTree.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                newTree.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

                float randomScale = Random.Range(treeScaleMin, treeScaleMin);
                newTree.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                newTree.transform.position += transform.up * randomScale * treeOffset;
                activeTrees.Add(newTree);
            }
        }

    }



    public void DeleteForest()
    {
        for (int i = 0; i < activeTrees.Count; i++)
            DestroyImmediate(activeTrees[i].gameObject);

        forestPoints.Clear();
    }


    private List<Vector3> PoissonPoints(int spacing, int k, int areaWidth, int areaLength, Vector3 pos)
    {
        PoissonDiscSampling pds = new PoissonDiscSampling();

        //generate points with poisson disc sampling starting at current regions centre
        List<Vector3> points = pds.GeneratePoints(spacing, k, areaWidth, areaLength, Vector3.zero);
        List<Vector3> newPoints = new List<Vector3>();

        //remove extra points from poisson disc sampler
        for (int i = 0; i < points.Count; i++)
        {
            Debug.Log("Points: " + points[i]);
            if (points[i] != Vector3.zero)
                newPoints.Add(points[i] + transform.position - transform.right * treeAreaWidth / 2 - transform.forward * treeAreaLength / 2);
        }

        return newPoints;
    }


    private void OnDrawGizmos()
    {
        if (!showBox)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(treeAreaWidth, 1, treeAreaLength));
        Gizmos.color = Color.white;


    }
}
