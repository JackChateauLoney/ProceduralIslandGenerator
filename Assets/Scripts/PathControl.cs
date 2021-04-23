using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathControl : MonoBehaviour
{

    [SerializeField] GameObject stonePrefab = null;
    [SerializeField] LayerMask pathInteractionMask = ~0;
    List<GameObject> stones = new List<GameObject>();
    List<Vector3> randPoints = new List<Vector3>();

    [SerializeField] float numStones = 50;
    [SerializeField] float width = 13;
    [SerializeField] float height = 1;
    public float depth = 40;
    [SerializeField] float stoneScaleMin = 9;
    [SerializeField] float stoneScaleMax = 15;

    [HideInInspector] public bool showBox = false;

    Quaternion rotation = Quaternion.identity;

    public void CreatePath()
    {
        DeletePath();


        rotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        for (int i = 0; i < numStones; i++)
            randPoints.Add(new Vector3(Random.Range(-width/2, width/2), 0, Random.Range(-depth/2, depth/2)) + transform.position);

        for (int i = 0; i < randPoints.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(randPoints[i], -transform.up, out hit, 120f, pathInteractionMask))
            {
                randPoints[i] = hit.point;

                GameObject stone = Instantiate(stonePrefab, transform);
                stone.transform.position = randPoints[i];
                stone.transform.up = hit.normal;
                stone.transform.Rotate(-90, Random.Range(0, 360), 0);

                float randomScale = Random.Range(stoneScaleMin, stoneScaleMax);
                stone.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                stones.Add(stone);
            }
        }

        transform.rotation = rotation;
    }


    public void DeletePath()
    {
        for (int i = 0; i < stones.Count; i++)
            DestroyImmediate(stones[i].gameObject);
        
        stones.Clear();
        randPoints.Clear();
        
    }

    private void OnDrawGizmos()
    {        
        if(!showBox)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, depth));
        Gizmos.matrix = Matrix4x4.identity;


    }
}
