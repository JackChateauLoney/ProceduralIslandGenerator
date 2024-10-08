using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Sort the points along one axis. The first 3 points form a triangle. Consider the next point and connect it with all
//previously connected points which are visible to the point. An edge is visible if the center of the edge is visible to the point.
public static class IncrementalTriangulationAlgorithm
{
    public static List<Triangle> TriangulatePoints(List<Vertex> points)
    {
        List<Triangle> triangles = new List<Triangle>();

        //Sort the points along x-axis
        //OrderBy is always soring in ascending order - use OrderByDescending to get in the other order
        points = points.OrderBy(n => n.position.x).ToList();

        //The first 3 vertices are always forming a triangle
        Triangle newTriangle = new Triangle(points[0].position, points[1].position, points[2].position);
        
        triangles.Add(newTriangle);

        //All edges that form the triangles, so we have something to test against
        List<Edge2> edges = new List<Edge2>();

        edges.Add(new Edge2(newTriangle.v1, newTriangle.v2));
        edges.Add(new Edge2(newTriangle.v2, newTriangle.v3));
        edges.Add(new Edge2(newTriangle.v3, newTriangle.v1));

        //Add the other triangles one by one
        //Starts at 3 because we have already added 0,1,2
        for (int i = 3; i < points.Count; i++)
        {
            Vector3 currentPoint = points[i].position;

            //The edges we add this loop or we will get stuck in an endless loop
            List<Edge2> newEdges = new List<Edge2>();

            //Is this edge visible? We only need to check if the midpoint of the edge is visible 
            for (int j = 0; j < edges.Count; j++)
            {
                Edge2 currentEdge = edges[j];

                Vector3 midPoint = (currentEdge.v1.position + currentEdge.v2.position) / 2f;

                Edge2 edgeToMidpoint = new Edge2(currentPoint, midPoint);

                //Check if this line is intersecting
                bool canSeeEdge = true;

                for (int k = 0; k < edges.Count; k++)
                {
                    //Dont compare the edge with itself
                    if (k == j)
                    {
                        continue;
                    }

                    if (AreEdgesIntersecting(edgeToMidpoint, edges[k]))
                    {
                        canSeeEdge = false;

                        break;
                    }
                }

                //This is a valid triangle
                if (canSeeEdge)
                {
                    Edge2 edgeToPoint1 = new Edge2(currentEdge.v1, new Vertex(currentPoint));
                    Edge2 edgeToPoint2 = new Edge2(currentEdge.v2, new Vertex(currentPoint));

                    newEdges.Add(edgeToPoint1);
                    newEdges.Add(edgeToPoint2);

                    Triangle newTri = new Triangle(edgeToPoint1.v1, edgeToPoint1.v2, edgeToPoint2.v1);

                    triangles.Add(newTri);
                }
            }


            for (int j = 0; j < newEdges.Count; j++)
            {
                edges.Add(newEdges[j]);
            }
        }


        return triangles;
    }



    private static bool AreEdgesIntersecting(Edge2 edge1, Edge2 edge2)
    {
        Vector2 l1_p1 = new Vector2(edge1.v1.position.x, edge1.v1.position.z);
        Vector2 l1_p2 = new Vector2(edge1.v2.position.x, edge1.v2.position.z);
        
        Vector2 l2_p1 = new Vector2(edge2.v1.position.x, edge2.v1.position.z);
        Vector2 l2_p2 = new Vector2(edge2.v2.position.x, edge2.v2.position.z);

        bool isIntersecting = Geometry.AreLinesIntersecting(l1_p1, l1_p2, l2_p1, l2_p2, true);

        return isIntersecting;
    }
}