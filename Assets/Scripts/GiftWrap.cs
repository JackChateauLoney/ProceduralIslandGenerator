using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftWrap : MonoBehaviour
{
    private List<Vertex> pointList = new List<Vertex>(200);

    void Start()
    {
        
        for (int i = 0; i < pointList.Capacity; i++)
        {
            pointList[i].position = new Vector3(Random.Range(0, 300), 0, Random.Range(0, 300));




        }

        JarvisMarchAlgorithm.GetConvexHull(pointList);
    }





}
