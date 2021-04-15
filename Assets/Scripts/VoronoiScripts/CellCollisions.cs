using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCollisions : MonoBehaviour
{
    Transform topLeft = null;
    Transform bottomRight = null;
    GameObject voronoiParent = null;



    public void Init()
    {
        //get defined area
        topLeft = transform.parent.GetComponent<MeshGeneration>().topLeft;
        bottomRight = transform.parent.GetComponent<MeshGeneration>().bottomRight;

        

        Vector3 pos = GetComponent<MeshFilter>().sharedMesh.vertices[0];
        Debug.Log("pos: " + pos);
        
        //check if self is within area defined
        if(pos.x < topLeft.position.x || pos.x > bottomRight.position.x ||
           pos.z < topLeft.position.z || pos.z > bottomRight.position.z)
        { 
            DestroyImmediate(gameObject);
        }
        else
        {
            voronoiParent = transform.parent.GetComponent<MeshGeneration>().voronoiParent;
            transform.parent = voronoiParent.transform;
        }


        
    }
}
