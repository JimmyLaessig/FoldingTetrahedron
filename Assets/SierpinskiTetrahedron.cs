using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum AnimationState
{
    PAUSED, FORWARD, BACKWARD
}

public class SierpinskiTetrahedron : MonoBehaviour
{
    [SerializeField]
    private float animationSpeed = 1.0f;


    private Tetrahedron tetrahedron;
    private Mesh mesh;
    private Mesh triangle;


    private List<Vector3> instances             = new List<Vector3>();
    private List<Matrix4x4> instanceMatrices    = new List<Matrix4x4>();
    private float scale                         = 1;
    private int numIterations                   = 0;


    [SerializeField]
    private bool drawInstanced;
    [SerializeField]
    private Material tetrahedronFoldingMaterial;
    [SerializeField]
    private Material triangleFoldingMaterial;

    [SerializeField]
    private int maxIterations = 3;

    private float animationProgress = 0.0f;

    private AnimationState state    = AnimationState.PAUSED;


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


    public int NumIterations
    {
        get{return numIterations;}       
    }


    // Use this for initialization
    void Start()
    {
        

        tetrahedron = Tetrahedron.Unit;
        instances.Add(Vector3.zero);

        mesh = new Mesh();
        mesh.vertices   = tetrahedron.Vertices;
        mesh.triangles  = tetrahedron.Indices;


        float h = Mathf.Sqrt(3f) * (1f / 2f);            
        var p0 = new Vector3( 0.5f, 0.0f,-h );
        var p1 = new Vector3(-0.5f, 0.0f, h);
        var p2 = new Vector3(1.5f, 0.0f, h);


        triangle = new Mesh();
        triangle.vertices   = new Vector3[] {p0, p1, p2 };
        triangle.triangles  = new int[] { 2, 1, 0 };

        Camera.main.transform.LookAt(Vector3.zero, Vector3.up);

        InitIterations();
    }


    public void InitIterations()
    {
        instances.Clear();
        instances.Add(Vector3.zero);
        scale = 1.0f;
        instanceMatrices.Clear();
        instanceMatrices = instances.ConvertAll(p => Matrix4x4.TRS(p - tetrahedron.s, Quaternion.identity, Vector3.one * scale));
        numIterations = 0;
    }


    public void ApplyForwardIteration()
    {
        Debug.Log("numIterations : " + numIterations + ", maxIterations: " + maxIterations);
        if (numIterations < maxIterations)
        {
            if (numIterations == 0)
            {
                numIterations++;
                return;
            }

            scale *= 0.5f;
            Vector3 v01 = (tetrahedron.p1 - tetrahedron.p0) * scale;
            Vector3 v02 = (tetrahedron.p2 - tetrahedron.p0) * scale;
            Vector3 v03 = (tetrahedron.p3 - tetrahedron.p0) * scale;

            var s = tetrahedron.s;

            var newInstances = new List<Vector3>();
            instances.ForEach(p =>
            {
                newInstances.Add(p);
                newInstances.Add(p + v01);
                newInstances.Add(p + v02);
                newInstances.Add(p + v03);
            });

            instances = newInstances;
            instanceMatrices = instances.ConvertAll(p => Matrix4x4.TRS(p - tetrahedron.s, Quaternion.identity, Vector3.one * scale));
            numIterations++;
        }
    }


    public void ApplyReversedIteration()
    {
        if (numIterations > 0)   
        {
            if(numIterations == 1)
            {
                numIterations--;
                return;
            }

            scale *= 2.0f;

            var newInstances = new List<Vector3>();
            for (int i = 0; i < instances.Count; i++)
            {
                if (i % 4 == 0) newInstances.Add(instances[i]);
            }

            instances = newInstances;
            instanceMatrices = instances.ConvertAll(p => Matrix4x4.TRS(p - tetrahedron.s, Quaternion.identity, Vector3.one * scale));
            numIterations--;
        }
    }



    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
        Draw();
    }


    public Toggle OnToggleChangedCallback
    {
        set {
            if (value.gameObject.name == "PauseButton")
            {
                this.state = AnimationState.PAUSED;
            }
            else if (value.gameObject.name == "PlayForwardButton")
            {            
                this.state = AnimationState.FORWARD;
            }
            else if (value.gameObject.name == "PlayReversedButton")
            {
                this.state = AnimationState.BACKWARD;
            }
        }
    }


    private void UpdateAnimation()
    {

        switch(state)
        {
            case AnimationState.PAUSED:
                return;
            case AnimationState.FORWARD:
                animationProgress += Time.deltaTime * animationSpeed;
                
                if (animationProgress >= 1.0f)
                {
                    animationProgress = 0.0f;
                    if (numIterations < maxIterations)
                    {
                        ApplyForwardIteration();
                    }
                    else
                    {
                        animationProgress = 1.0f;
                    }
                }
                
                break;
            case AnimationState.BACKWARD:
                animationProgress -= Time.deltaTime * animationSpeed;
                if (animationProgress <= 0.0f)
                {
                    animationProgress = 1.0f;
                    if (numIterations > 0)
                    {
                        ApplyReversedIteration();
                    }
                    else
                    {
                        animationProgress = 0.0f;
                    }               
                }
                break;
        }               
    }



    private void Draw()
    {




        // Draw Triangle Folding

        if (numIterations == 0)
        {
            // Material Uniforms
            var angle = animationProgress * (-109.5f) * Mathf.Deg2Rad;
            triangleFoldingMaterial.SetFloat("cosPhi", Mathf.Cos(angle));
            triangleFoldingMaterial.SetFloat("sinPhi", Mathf.Sin(angle));
            triangleFoldingMaterial.SetFloat("animationProgress", animationProgress);
            var matrix = Matrix4x4.TRS(Vector3.zero - tetrahedron.s, Quaternion.identity, Vector3.one);
            Graphics.DrawMesh(triangle, this.transform.localToWorldMatrix * matrix, triangleFoldingMaterial, 0);
        }
        else
        {
            // Material Uniforms
            var angle = animationProgress * (-109.5f) * Mathf.Deg2Rad;
            tetrahedronFoldingMaterial.SetFloat("cosPhi", Mathf.Cos(angle));
            tetrahedronFoldingMaterial.SetFloat("sinPhi", Mathf.Sin(angle));
            tetrahedronFoldingMaterial.SetFloat("animationProgress", animationProgress);


            // Draw Tetrahedrons
            var matrices = instanceMatrices.ConvertAll(m => this.transform.localToWorldMatrix * m);
            if (drawInstanced)
            {
                var chunks = matrices.Split(1023);
                chunks.ToList().ForEach(m => Graphics.DrawMeshInstanced(mesh, 0, tetrahedronFoldingMaterial, m.ToArray(), m.Count()));
            }
            else
            {
                matrices.ForEach(m => Graphics.DrawMesh(mesh, m, tetrahedronFoldingMaterial, 0));
            }
        }
    }
}