using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        var vertices = new List<Vector3>()
        {
            p0, 
            p1, 
            p2, 
            p3                                  
        };
                                
        var indices = new int[] {
            //floor
            0, 1, 2,
            // Side 1
            1, 0, 3,
            // Side 2
            2, 1, 3,
            // Side 3
            0, 2, 3 };
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        var cubeObj = GameObject.Find("Cube");
       // mesh = cubeObj.GetComponent<MeshFilter>().mesh;
        cubeObj.SetActive(false);
       
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
