using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//And edge between two vertices
public class Edge2
{
    public Vertex v1;
    public Vertex v2;

    //Is this edge intersecting with another edge?
    public bool isIntersecting = false;

    public Edge2(Vertex v1, Vertex v2)
    {
        this.v1 = v1;
        this.v2 = v2;
    }

    public Edge2(Vector3 v1, Vector3 v2)
    {
        this.v1 = new Vertex(v1);
        this.v2 = new Vertex(v2);
    }

    //Get vertex in 2d space (assuming x, z)
    public Vector2 GetVertex2D(Vertex v)
    {
        return new Vector2(v.position.x, v.position.z);
    }

    //Flip edge
    public void FlipEdge()
    {
        Vertex temp = v1;

        v1 = v2;

        v2 = temp;
    }
}