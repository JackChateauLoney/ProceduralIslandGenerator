using UnityEngine;
using System.Collections;

namespace LinearAlgebra
{
    //How to figure out if two lines are intersecting
    public class LineLineIntersection : MonoBehaviour
    {
		//Line segment-line segment intersection in 2d space by using the dot product
        //p1 and p2 belongs to line 1, and p3 and p4 belongs to line 2 
        public static bool AreLineSegmentsIntersectingDotProduct(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            bool isIntersecting = false;

            if (IsPointsOnDifferentSides(p1, p2, p3, p4) && IsPointsOnDifferentSides(p3, p4, p1, p2))
            {
                isIntersecting = true;
            }

            return isIntersecting;
        }

        //Are the points on different sides of a line?
        private static bool IsPointsOnDifferentSides(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            bool isOnDifferentSides = false;

            //The direction of the line
            Vector3 lineDir = p2 - p1;

            //The normal to a line is just flipping x and z and making z negative
            Vector3 lineNormal = new Vector3(-lineDir.z, lineDir.y, lineDir.x);

            //Now we need to take the dot product between the normal and the points on the other line
            float dot1 = Vector3.Dot(lineNormal, p3 - p1);
            float dot2 = Vector3.Dot(lineNormal, p4 - p1);

            //If you multiply them and get a negative value then p3 and p4 are on different sides of the line
            if (dot1 * dot2 < 0f)
            {
                isOnDifferentSides = true;
            }

            return isOnDifferentSides;
        }
	}
}