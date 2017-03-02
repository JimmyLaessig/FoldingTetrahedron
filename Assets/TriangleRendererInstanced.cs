using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TriangleRendererInstanced : MonoBehaviour {

    Mesh mesh;
    Material material;


    
    float triangleHeight = Mathf.Sqrt(3f) * (1f / 2f);               
    float tetrahedronHeight = Mathf.Sqrt(6f) / 3.0f;
    // Use this for initialization
    void Start()
    {

        mesh = new Mesh();
        material = new Material(new Material(Shader.Find("Diffuse")));

        var p0 = new Vector3(0, 0, 0);
        var p1 = new Vector3(1, 0, 0);
        var p2 = new Vector3(0.5f, 0, triangleHeight);

        var s = (p0 + p1 + p2) / 3;
        

        p0 -= s;
        p1 -= s;
        p2 -= s;
        var p3 = new Vector3(0, tetrahedronHeight, 0);
        
        //var vertices = new List<Vector3>()
        //{
        //    p0, 
        //    p1, 
        //    p2, 
        //    p3                                  
        //};
                                
        //var indices = new int[] {
        //    //floor
        //    0, 1, 2,
        //    // Side 1
        //    1, 0, 3,
        //    // Side 2
        //    2, 1, 3,
        //    // Side 3
        //    0, 2, 3
        //};


        var triangles = new List<Triangle>()
        {
            new Triangle(p0, p1, p2, Color.red),
            new Triangle(p1, p0, p3, Color.red),
            new Triangle(p2, p1, p3, Color.red),
            new Triangle(p0, p2, p3, Color.red),
        };

        triangles = Subdivide(Subdivide(Subdivide(Subdivide(triangles))));

        var vertices    = ToVertexList(triangles);
        var colors      = ToColorList(triangles);
        var numVertices = vertices.Count;


        mesh.SetVertices(vertices);
        mesh.SetColors(colors);
        mesh.SetTriangles(Enumerable.Range(0, numVertices).ToArray(), 0);
        
        //mesh.SetIndices(indices, MeshTopology.Triangles, 0);
             
    }
	

    public static List<Vector3> ToVertexList(List<Triangle> triangles)
    {
        var vertices = new List<Vector3>();
        triangles.ForEach(t => { vertices.Add(t.p0); vertices.Add(t.p1); vertices.Add(t.p2); });
        return vertices;
    }

    public static List<Color> ToColorList(List<Triangle> triangles)
    {
        var colors = new List<Color>();
        triangles.ForEach(t => { colors.Add(t.color); colors.Add(t.color); colors.Add(t.color); });
        return colors;
    }

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


    private List<Triangle> Subdivide(List<Triangle> triangles)
    {
        var newTriangles = new List<Triangle>();
        triangles.ForEach(t =>
        {
            var p01 = (t.p0 + t.p1) / 2;
            var p12 = (t.p1 + t.p2) / 2;
            var p02 = (t.p2 + t.p0) / 2;

            // Emit 3 triangles lying at the cornser of the old triangle
            newTriangles.Add(new Triangle(t.p0, p01, p02, Color.red));
            newTriangles.Add(new Triangle(t.p1, p12, p01, Color.red));
            newTriangles.Add(new Triangle(t.p2, p02, p12, Color.red));
            // Emit center triangle
        });

        return newTriangles;
    }

    void Update()
    {
        //this.transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
        //Debug.Log("ONRenderObject");
        var matrix0 = Matrix4x4.identity;
        // material.
        //var matrix1 = matrix0 * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 120, 0), Vector3.one);
        //var matrix2 = matrix0 * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 240, 0), Vector3.one);
        //var matrix3 = matrix0 * Matrix4x4.TRS(Vector)
        Graphics.DrawMesh(mesh, matrix0, material, 0);
        
        //Graphics.DrawMesh(mesh, transform.localToWorldMatrix * matrix1, material, 0);
        //Graphics.DrawMesh(mesh, transform.localToWorldMatrix * matrix2, material, 0);

    }
}
