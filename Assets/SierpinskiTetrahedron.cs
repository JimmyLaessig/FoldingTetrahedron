using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SierpinskiTetrahedron : MonoBehaviour {


    private FoldableTriangle triangle;

    

    private Mesh mesh;
    private Material material;

	// Use this for initialization
	void Start ()
    {
        float triangleHeight    = Mathf.Sqrt(3f) * (1f / 2f);
        float tetrahedronHeight = Mathf.Sqrt(6f) / 3.0f;

        // Base xz triangle
        var p0 = new Vector3(0, 0, 0);
        var p1 = new Vector3(1, 0, 0);
        var p2 = new Vector3(0.5f, triangleHeight, 0);

        var s = (p0 + p1 + p2) / 3;


        //p0 -= s;
        //p1 -= s;
        //p2 -= s;

        var p3 = new Vector3(s.x, tetrahedronHeight, s.z);

        triangle = new FoldableTriangle(new Triangle(p2, p1, p0, Color.red));
	
        mesh = triangle.ToMesh();
        material = new Material(new Material(Shader.Find("Diffuse")));
        //GameObject.Find("Cylinder").transform.position = new Vector3(s.x, 0, s.z)
    }


    // Update is called once per frame
    void Update()
    {
        //this.transform.Rotate(Vector3.up * 90 * Time.deltaTime);
        var innerAngle = Mathf.Atan(2f * Mathf.Sqrt(2f)) * Mathf.Rad2Deg;
        // Trafos in model space
        var matrix0 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90 - innerAngle, 0, 0), Vector3.one);
        var matrix1 = Matrix4x4.TRS(Vector3.right, Quaternion.Euler(0, -120, 0), Vector3.one) * matrix0;
        var matrix2 = Matrix4x4.TRS(Vector3.right, Quaternion.Euler(0, -120, 0), Vector3.one) * matrix1;
        var matrix3 = Matrix4x4.TRS(Vector3.right, Quaternion.Euler(-90, 180, 0), Vector3.one);// * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one);

       
        // To Worldspace
        Graphics.DrawMesh(mesh, this.transform.localToWorldMatrix * matrix0, material, 0);
        Graphics.DrawMesh(mesh, this.transform.localToWorldMatrix * matrix1, material, 0);
        Graphics.DrawMesh(mesh, this.transform.localToWorldMatrix * matrix2, material, 0);
        Graphics.DrawMesh(mesh, this.transform.localToWorldMatrix * matrix3, material, 0);
    }
}
