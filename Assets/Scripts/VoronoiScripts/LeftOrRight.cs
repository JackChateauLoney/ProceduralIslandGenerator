using UnityEngine;
using System.Collections;

namespace LinearAlgebra
{
    //Figure out if you should turn left or right to reach a waypoint
    public class LeftOrRight : MonoBehaviour
    {
        public Transform youTrans;
        public Transform wayPointTrans;

        void Update()
        {
            //The right direction of the direction you are facing
            Vector3 youDir = youTrans.right;

            //The direction you want to reach by turning left or right
            //Here we want to reach the forward direction of the waypoint
            Vector3 waypointDir = wayPointTrans.forward;

            //The dot product between the vectors
            float dotProduct = Vector3.Dot(youDir, waypointDir);

            //Now we can decide if we should turn left or right
            if (dotProduct > 0f)
                Debug.Log("Turn right");
            else
                Debug.Log("Turn left");
        }
    }
}