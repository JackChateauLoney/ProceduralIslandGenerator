using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class PoissonTest : MonoBehaviour
{
    List<Vector3> points;
    List<Vector3> treePoints;

    bool generated = false;

    private void Start()
    {
        generated = false;
        TestIt(transform.position);
    }

    void TestIt(Vector3 startingPoint)
    {
        PoissonDiscSampling pds = new PoissonDiscSampling();

        Vector3 centrePoint = startingPoint;

        points = pds.GeneratePoints(3, 60, 100, 100, centrePoint);
        treePoints = new List<Vector3>();

        generated = true;
    }

    private void OnDrawGizmos()
    {
        if(!generated)
            return;

        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(points[i], 0.1f);
            Gizmos.color = Color.white;
        }
    }

}
