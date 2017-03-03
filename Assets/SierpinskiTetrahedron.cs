using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public struct Tetrahedron
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
}


public class SierpinskiTetrahedron : MonoBehaviour
{
    [SerializeField]
    private float animationSpeed = 1.0f;

    [SerializeField]
    private bool isLooping = true;


    private FoldableTriangle triangle;
    private Mesh mesh;


    private Tetrahedron baseTetrahedron;
    private List<Tetrahedron> instances         = new List<Tetrahedron>();
    private List<Matrix4x4> instanceMatrices    = new List<Matrix4x4>();
    private int numSubDivisions = 0;


    [SerializeField]
    private bool drawInstanced;
    [SerializeField]
    private Material materialInstanced;


    public bool DrawInstanced
    {
        get{return drawInstanced;}
        set{drawInstanced = value;}
    }


    public float AnimationSpeed
    {
        get{ return animationSpeed;}
        set{ animationSpeed = value;}
    }


    public bool IsLooping
    {
        get{ return isLooping; }
        set{ isLooping = value;}
    }

    public int NumSubDivisions
    {
        get{return numSubDivisions;}
        set{numSubDivisions = value;}
    }


    // Use this for initialization
    void Start()
    {
        float triangleHeight = Mathf.Sqrt(3f) * (1f / 2f);
        float tetrahedronHeight = Mathf.Sqrt(6f) / 3.0f;


        var p0 = new Vector3(0, 0, 0);
        var p1 = new Vector3(1, 0, 0);
        var p2 = new Vector3(0.5f, 0, triangleHeight);
        var s = (p0 + p1 + p2) / 3;
        var p3 = new Vector3(s.x, tetrahedronHeight, s.z);

        baseTetrahedron = new Tetrahedron(p0, p1, p2, p3);
        instances.Add(baseTetrahedron);
        computeInstanceMatrices();


        // Triangles for visualization
        var tp0 = new Vector3(0, 0, 0);
        var tp1 = new Vector3(1, 0, 0);
        var tp2 = new Vector3(0.5f, triangleHeight, 0);


        triangle = new FoldableTriangle(new Triangle(tp2, tp1, tp0, Color.red));
        mesh = triangle.ToMesh();
    }



    public void subdivide()
    {

        var newInstances = new List<Tetrahedron>();
        instances.ForEach(t =>
        {
            var p0 = t.p0;
            var p1 = t.p1;
            var p2 = t.p2;
            var p3 = t.p3;

            var p01 = (p0 + p1) / 2;
            var p02 = (p0 + p2) / 2;
            var p03 = (p0 + p3) / 2;

            var p12 = (p1 + p2) / 2;
            var p13 = (p1 + p3) / 2;

            var p23 = (p2 + p3) / 2;

            newInstances.Add(new Tetrahedron(p0, p01, p02, p03));
            newInstances.Add(new Tetrahedron(p01, p1, p12, p13));
            newInstances.Add(new Tetrahedron(p02, p12, p2, p23));
            newInstances.Add(new Tetrahedron(p03, p13, p23, p3));
        });

        instances = newInstances;
        computeInstanceMatrices();
        numSubDivisions++;
    }


    private void computeInstanceMatrices()
    {
        instanceMatrices.Clear();

        instances.ForEach(t => {
            var translation = t.p0;
            var scale = new Vector3(t.Length, t.Length, t.Length);
            var biasMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scale);

            var innerAngle = Mathf.Atan(2f * Mathf.Sqrt(2f)) * Mathf.Rad2Deg;
            // Trafos in model space
            var matrix0 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90 - innerAngle, 0, 0), Vector3.one);
            var matrix1 = Matrix4x4.TRS(Vector3.right, Quaternion.Euler(0, -120, 0), Vector3.one) * matrix0;
            var matrix2 = Matrix4x4.TRS(Vector3.right, Quaternion.Euler(0, -120, 0), Vector3.one) * matrix1;
            var matrix3 = Matrix4x4.TRS(Vector3.right, Quaternion.Euler(-90, 180, 0), Vector3.one);// * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one);           

            instanceMatrices.Add(biasMatrix * matrix0);
            instanceMatrices.Add(biasMatrix * matrix1);
            instanceMatrices.Add(biasMatrix * matrix2);
            instanceMatrices.Add(biasMatrix * matrix3);
        });
    }


    private float animationTimeStep     = 1.0f;
    private float animationTimeCounter  = 0.0f;


    // Update is called once per frame
    void Update()
    {
        if(animationTimeCounter >= animationTimeStep)
        {
            animationTimeCounter -= animationTimeStep;
            if (numSubDivisions >= 8 && isLooping)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                subdivide();               
            }          
        }
        animationTimeCounter += Time.deltaTime * animationSpeed;

        if (drawInstanced)
        {
            var chunks = instanceMatrices.Split(1023);
            chunks.ToList().ForEach(m => Graphics.DrawMeshInstanced(mesh, 0, materialInstanced, m.ToArray(), m.Count()));
        }
        else
        {
            instanceMatrices.ForEach(m => Graphics.DrawMesh(mesh, m, materialInstanced, 0));
        }
    }
}
