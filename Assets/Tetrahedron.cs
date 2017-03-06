using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Tetrahedron
{

    public Vector3 p0, p1, p2, p3;

    public Tetrahedron(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }

    public float Length
    {
        get { return (p1 - p0).magnitude; }
    }


    public Vector3 s
    {
        get { return (p0 + p1 + p2 + p3) / 4.0f; }
    }


    public Vector3[] Vertices
    {
        get
        {
            return new Vector3[] { 
                    // Bottom
                    p0, p1, p2, 
                    // Front
                    p3, p1, p0, 
                    // Right
                    p3, p2, p1,
                    // Left
                    p3, p0, p2,
                    };
        }
    }


    public Color[] Colors
    {
        get {
            var green   = new Color(103f /255f, 255f /255f, 93f / 255f);
            var blue    = new Color(110f /255f, 189f /255f, 253f / 255f);
            var yellow  = new Color(242f /255f, 255f /255f, 90f / 255f);
            var red     = new Color(246f / 255f, 73f / 255f, 138f / 255f);

            return new Color[] 
                {            
                // Bottom    
                yellow, yellow, yellow,
                // Front
                green, green, green,
                // Right
                red, red, red,
                // Left
                blue, blue, blue,
                };
        }
    }


    public int[] Indices
    {
        get{ return new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; }
    }
    
    
    public static Tetrahedron Unit
    {
        get
        {
            float triangleHeight    = Mathf.Sqrt(3f) * (1f / 2f);
            float tetrahedronHeight = Mathf.Sqrt(6f) / 3.0f;

            var p0 = new Vector3(0, 0, 0);
            var p1 = new Vector3(1, 0, 0);
            var p2 = new Vector3(0.5f, 0, triangleHeight);

            var s   = (p0 + p1 + p2) / 3f;
            var p3 = new Vector3(s.x, tetrahedronHeight, s.z);
            s = (p0 + p1 + p2 + p3) / 4f;

            return new Tetrahedron(p0, p1, p2, p3);
        }
    }     
}