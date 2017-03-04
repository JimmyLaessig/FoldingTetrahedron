using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;




public class SierpinskiTetrahedron : MonoBehaviour
{
    [SerializeField]
    private float animationSpeed = 1.0f;

    [SerializeField]
    private bool isLooping = true;


    private Tetrahedron tetrahedron;
    private Mesh mesh;


    private List<Vector3> instances             = new List<Vector3>();
    private List<Matrix4x4> instanceMatrices    = new List<Matrix4x4>();
    private float scale                         = 1;
    private int numSubDivisions                 = 0;


    [SerializeField]
    private bool drawInstanced;
    [SerializeField]
    private Material material;


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
    }


    // Use this for initialization
    void Start()
    {
        tetrahedron = Tetrahedron.Unit;
        instances.Add(Vector3.zero);

        mesh = new Mesh();
        mesh.vertices   = tetrahedron.Vertices;
        mesh.triangles  = tetrahedron.Indices;

        resetSubDivision();
    }



    public void subdivide()
    {
        scale *= 0.5f;
        Vector3 v01 = (tetrahedron.p1 - tetrahedron.p0) * scale;
        Vector3 v02 = (tetrahedron.p2 - tetrahedron.p0) * scale;
        Vector3 v03 = (tetrahedron.p3 - tetrahedron.p0) * scale;

        var s = tetrahedron.s;

        var newInstances = new List<Vector3>();
        instances.ForEach(p =>
        {          
            newInstances.Add(p       );
            newInstances.Add(p + v01 );
            newInstances.Add(p + v02 );
            newInstances.Add(p + v03 );
        });

        instances = newInstances;
        instanceMatrices = instances.ConvertAll(p => Matrix4x4.TRS(p, Quaternion.identity, Vector3.one * scale));
        numSubDivisions++;
    }


    public void resetSubDivision()
    {
        instances.Clear();
        instances.Add(Vector3.zero);
        scale = 1.0f;
        instanceMatrices.Clear();
        instanceMatrices = instances.ConvertAll(p => Matrix4x4.TRS(p, Quaternion.identity, Vector3.one * scale));
        numSubDivisions = 0;
}

    


    private float animationTimeStep     = 1.0f;
    private float animationTimeCounter  = 0.0f;


    // Update is called once per frame
    void Update()
    {
        if (animationTimeCounter >= animationTimeStep)
        {
            animationTimeCounter -= animationTimeStep;
            if (numSubDivisions >= 8)
            {
                if (isLooping)
                    resetSubDivision();
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
            chunks.ToList().ForEach(m => Graphics.DrawMeshInstanced(mesh, 0, material, m.ToArray(), m.Count()));
        }
        else
        {
            instanceMatrices.ForEach(m => Graphics.DrawMesh(mesh, m, material, 0));
        }
    }
}