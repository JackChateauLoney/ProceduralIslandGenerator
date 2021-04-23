using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCollisions : MonoBehaviour
{
    Transform topLeft = null;
    Transform bottomRight = null;
    GameObject islandParent = null;



    public void Init()
    {
        //get defined area
        topLeft = transform.parent.GetComponent<MeshGeneration>().topLeft;
        bottomRight = transform.parent.GetComponent<MeshGeneration>().bottomRight;

        

        Vector3 pos = GetComponent<MeshFilter>().sharedMesh.vertices[0];
        
        //check if self is within area defined
        if(pos.x < topLeft.position.x || pos.x > bottomRight.position.x ||
           pos.z < topLeft.position.z || pos.z > bottomRight.position.z)
        { 
            DestroyImmediate(gameObject);
        }
        else
        {
            islandParent = transform.parent.GetComponent<MeshGeneration>().islandParent;
            transform.parent = islandParent.transform;
        }


        
    }
}
