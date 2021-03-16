using UnityEngine;
using System.Collections;

//Are 2 rectangles with orientation (in 2d space) colliding?
public class OBBIntersection : MonoBehaviour
{
    //Rectangles
    public Transform r1;
    public Transform r2;

    void Update()
    {
        RectangleRectangle();
    }

    //Rectangle-rectangle collision
    private void RectangleRectangle()
    {
        //Find the corners of each rectangle
        //Rectangle 1
        Vector3 r1_FL, r1_FR, r1_BL, r1_BR = Vector3.zero;

        GetCornerPositions(r1, out r1_FL, out r1_FR, out r1_BL, out r1_BR);

        //Rectangle 2
        Vector3 r2_FL, r2_FR, r2_BL, r2_BR = Vector3.zero;

        GetCornerPositions(r2, out r2_FL, out r2_FR, out r2_BL, out r2_BR);


        //Are the rectangles intersecting?

        //Method 1 - Check for intersection with the Separating Axis Theorem (SAT)
        bool isIntersecting1 = Intersections.IsIntersectingOBBRectangleRectangle(r1_FL, r1_FR, r1_BL, r1_BR, r2_FL, r2_FR, r2_BL, r2_BR);

        //Method 2 - Check for intersection with triangle-triangle intersection
        //bool isIntersecting2 = TriangleTriangle(r1_FL, r1_FR, r1_BL, r1_BR, r2_FL, r2_FR, r2_BL, r2_BR);

        if (isIntersecting1)
        {
            r1.GetComponent<MeshRenderer>().material.color = Color.red;
            r2.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            r1.GetComponent<MeshRenderer>().material.color = Color.blue;
            r2.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }

    //Find the corners of a rectangle transform
    void GetCornerPositions(Transform r, out Vector3 FL, out Vector3 FR, out Vector3 BL, out Vector3 BR)
    {
        FL = r.position + r.forward * r.localScale.z * 0.5f - r.right * r.localScale.x * 0.5f;
        FR = r.position + r.forward * r.localScale.z * 0.5f + r.right * r.localScale.x * 0.5f;

        BL = r.position - r.forward * r.localScale.z * 0.5f - r.right * r.localScale.x * 0.5f;
        BR = r.position - r.forward * r.localScale.z * 0.5f + r.right * r.localScale.x * 0.5f;
    }

    //Rectangle-rectangle intersection by using triangle-triangle intersection for comparisons
    private bool TriangleTriangle(
        Vector3 r1_FL, Vector3 r1_FR, Vector3 r1_BL, Vector3 r1_BR,
        Vector3 r2_FL, Vector3 r2_FR, Vector3 r2_BL, Vector3 r2_BR)
    {
        bool isIntersecting = false;

        //The same algorithm from the rectangle-rectangle intersection tutorial so use the code from that one

        Intersections.Triangle tr1 = new Intersections.Triangle(r1_FL, r1_FR, r1_BR);
        Intersections.Triangle tr2 = new Intersections.Triangle(r2_FL, r2_FR, r2_BR);
        if (Intersections.IsTriangleTriangleIntersecting(tr1, tr2))
        {
            isIntersecting = true;

            return isIntersecting;
        }
        tr1 = new Intersections.Triangle(r1_FL, r1_FR, r1_BR);
        tr2 = new Intersections.Triangle(r2_FL, r2_BR, r2_BL);
        if (Intersections.IsTriangleTriangleIntersecting(tr1, tr2))
        {
            isIntersecting = true;

            return isIntersecting;
        }
        tr1 = new Intersections.Triangle(r1_FL, r1_BR, r1_BL);
        tr2 = new Intersections.Triangle(r2_FL, r2_BR, r2_BL);
        if (Intersections.IsTriangleTriangleIntersecting(tr1, tr2))
        {
            isIntersecting = true;

            return isIntersecting;
        }
        tr1 = new Intersections.Triangle(r1_FL, r1_BL, r1_BR);
        tr2 = new Intersections.Triangle(r2_FL, r2_FR, r2_BR);
        if (Intersections.IsTriangleTriangleIntersecting(tr1, tr2))
        {
            isIntersecting = true;

            return isIntersecting;
        }


        return isIntersecting;
    }
}