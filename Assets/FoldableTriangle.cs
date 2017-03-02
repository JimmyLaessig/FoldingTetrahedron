using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public struct Triangle
{
    public Vector3 p0, p1, p2;
    public Color color;

    public Triangle(Vector3 p0, Vector3 p1, Vector3 p2, Color color)
    {
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;
        this.color = color;
    }
}


public class FoldableTriangle
{
    public Triangle t0;
    public Triangle t1;
    public Triangle t2;

    
    public Triangle center0;
    public Triangle center1;
    public Triangle center2;


    public FoldableTriangle(Triangle t)
    {        
        var p01 = (t.p0 + t.p1) / 2;
        var p12 = (t.p1 + t.p2) / 2;
        var p02 = (t.p2 + t.p0) / 2;

        // Emit 3 triangles lying at the cornser of the old triangle
        t0 = new Triangle(t.p0, p01, p02, Color.red);
        t1 = new Triangle(t.p1, p12, p01, Color.red);
        t2 = new Triangle(t.p2, p02, p12, Color.red);

        // Emit center triangle
        center0 = new Triangle(p01, p12, p02, Color.red);
        center1 = new Triangle(p01, p12, p02, Color.red);
        center2 = new Triangle(p01, p12, p02, Color.red);
    }


    public Vector3[] Vertices
    {
        get {
            var vertices = new List<Vector3>();
            vertices.Add(t0.p0);
            vertices.Add(t0.p1);
            vertices.Add(t0.p2);

            vertices.Add(t1.p0);
            vertices.Add(t1.p1);
            vertices.Add(t1.p2);

            vertices.Add(t2.p0);
            vertices.Add(t2.p1);
            vertices.Add(t2.p2);

            vertices.Add(t2.p0);
            vertices.Add(t2.p1);
            vertices.Add(t2.p2);

            //vertices.Add(center0.p0);
            //vertices.Add(center0.p1);
            //vertices.Add(center0.p2);

            //vertices.Add(center1.p0);
            //vertices.Add(center1.p1);
            //vertices.Add(center1.p2);

            //vertices.Add(center2.p0);
            //vertices.Add(center2.p1);
            //vertices.Add(center2.p2);

            return vertices.ToArray();
        }
    }

    public Mesh ToMesh()
    {
        var mesh        = new Mesh();
        mesh.vertices   = this.Vertices;
        mesh.colors     = this.Colors;
        mesh.triangles  = this.Indices;

        return mesh;
    }


    public int[] Indices
    {
        get { return Enumerable.Range(0, Vertices.Length).ToArray(); }
    }


    public Color[] Colors
    {
        get {
            var colors = new List<Color>();
            colors.Add(t0.color);
            colors.Add(t0.color);
            colors.Add(t0.color);

            colors.Add(t1.color);
            colors.Add(t1.color);
            colors.Add(t1.color);
                          
            colors.Add(t2.color);
            colors.Add(t2.color);
            colors.Add(t2.color);
                          
            colors.Add(t2.color);
            colors.Add(t2.color);
            colors.Add(t2.color);

            //colors.Add(center0.color);
            //colors.Add(center0.color);
            //colors.Add(center0.color);
                               
            //colors.Add(center1.color);
            //colors.Add(center1.color);
            //colors.Add(center1.color);
                               
            //colors.Add(center2.color);
            //colors.Add(center2.color);
            //colors.Add(center2.color);

            return colors.ToArray();
        }                
    }
}